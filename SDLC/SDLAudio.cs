﻿// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

public static class SDLAudio
{
    private static readonly MixFuncDelegate postMix = MixPostMix;
    private static readonly MusicFinishedDelegate musicFinished = MixMusicFinished;
    private static readonly EventHandlerList eventHandlerList = new();
    private static readonly object audioDataReceivedKey = new();
    private static readonly object audioMusicDoneKey = new();
    private static readonly object audioMusicStartKey = new();
    private static short[] musicData = Array.Empty<short>();
    private static int musicVolume = MIX_MAX_VOLUME;
    private static int soundVolume = MIX_MAX_VOLUME;
    private static string? driverName;
    private static SDLMusic? currentMusic;
    private static readonly SDLObjectTracker<SDLMusic> musicTracker = new(LogCategory.AUDIO, "Music");

    public static bool UseTmpFilesForMusic { get; set; } = true;
    public static bool AttemptToDeleteOldTmpFiles { get; set; } = true;

    public static event SDLMusicEventHandler MusicStarted
    {
        add { eventHandlerList.AddHandler(audioMusicStartKey, value); }
        remove { eventHandlerList.RemoveHandler(audioMusicStartKey, value); }
    }

    public static event SDLMusicFinishedEventHandler MusicFinished
    {
        add { eventHandlerList.AddHandler(audioMusicDoneKey, value); }
        remove { eventHandlerList.RemoveHandler(audioMusicDoneKey, value); }
    }

    public static event SDLMusicDataEventHandler MusicDataReceived
    {
        add { eventHandlerList.AddHandler(audioDataReceivedKey, value); }
        remove { eventHandlerList.RemoveHandler(audioDataReceivedKey, value); }

    }

    public static bool IsPlaying
    {
        get { return Mix_PlayingMusic() == 1; }
    }
    public static bool IsPaused
    {
        get { return Mix_PausedMusic() == 1; }
    }

    public static void Initialize()
    {
        _ = Mix_Init(MIX_InitFlags.MIX_INIT_MP3 | MIX_InitFlags.MIX_INIT_OGG | MIX_InitFlags.MIX_INIT_MID);
        if (Mix_OpenAudio(22050, MIX_DEFAULT_FORMAT, 2, 1024) != 0)
        {
            SDLLog.Error(LogCategory.AUDIO, "Couldn't open Audio");
        }
        else
        {
            _ = Mix_AllocateChannels(128);
            driverName = Marshal.PtrToStringUTF8(SDL_GetCurrentAudioDriver());
            SDLLog.Info(LogCategory.AUDIO, "Audio opened: {0}", driverName);
            Mix_HookMusicFinished(musicFinished);
            Mix_SetPostMix(postMix, IntPtr.Zero);
        }

    }

    public static void Shutdown()
    {
        _ = Mix_HaltMusic();
        Mix_SetPostMix(IntPtr.Zero, IntPtr.Zero);
        Mix_HookMusicFinished(IntPtr.Zero);
        musicTracker.Dispose();
        Mix_CloseAudio();
        Mix_Quit();
        SDLLog.Info(LogCategory.AUDIO, "Audio closed");
    }

    public static void PlayMusic(SDLMusic? music, int loops = -1, bool forceRestart = false)
    {
        if (music == null) return;
        IntPtr handle = music.Handle;
        if (handle == IntPtr.Zero) return;
        SDLMusicEventArgs e = new SDLMusicEventArgs(music);
        if (currentMusic != null)
        {
            if (currentMusic != music)
            {
                OnMusicFinished(new SDLMusicFinishedEventArgs(currentMusic, MusicFinishReason.Interrupted));
            }
            else if (forceRestart)
            {
                Mix_RewindMusic();
                SDLLog.Verbose(LogCategory.AUDIO, "Music '{0}' restarted", music.Name);
                OnMusicStarted(e);
                return;
            }
            else
            {
                SDLLog.Verbose(LogCategory.AUDIO, "Music '{0}' already playing, continuing...", music.Name);
                OnMusicStarted(e); // or maybe not?
                return;
            }
        }
        if (Mix_PlayMusic(handle, loops) != 0)
        {
            SDLLog.Error(LogCategory.AUDIO, "Could not play Music '{0}' of type {1}: {2}", music.Name, music.MusicType, SDLApplication.GetError());
        }
        else
        {
            _ = Mix_VolumeMusic(musicVolume);
            SDLLog.Verbose(LogCategory.AUDIO, "Music '{0}' started", music.Name);
            OnMusicStarted(e);
            currentMusic = music;
        }
    }
    public static void PauseMusic()
    {
        if (currentMusic != null)
        {
            Mix_PauseMusic();
        }
    }

    public static void ResumeMusic()
    {
        if (currentMusic != null)
        {
            Mix_ResumeMusic();
        }
    }

    public static void RewindMusic()
    {
        if (currentMusic != null)
        {
            Mix_RewindMusic();
        }
    }

    public static void StopMusic()
    {
        if (currentMusic != null)
        {
            _ = Mix_HaltMusic();
            currentMusic = null;
        }
    }

    public static SDLMusic? LoadMusic(string name)
    {
        SDLMusic? music = musicTracker.Find(name);
        if (music == null && File.Exists(name))
        {
            IntPtr handle = Mix_LoadMUS(name);
            if (handle != IntPtr.Zero)
            {
                music = new SDLMusic(handle, name);
                SDLLog.Verbose(LogCategory.AUDIO, "Music loaded from file '{0}'", name);
            }
        }
        return music;
    }

    public static SDLMusic? LoadMusic(string name, byte[]? data)
    {
        if (data != null)
        {
            return InternalLoadMusic(name, data);
        }
        return null;
    }

    private static SDLMusic? InternalLoadMusic(string name, byte[] data)
    {
        SDLMusic? music = musicTracker.Find(name);
        if (music == null)
        {
            if (UseTmpFilesForMusic)
            {
                string fileName = FileUtils.GetTempFile(name, AttemptToDeleteOldTmpFiles);
                try
                {
                    File.WriteAllBytes(fileName, data);
                    IntPtr handle = Mix_LoadMUS(fileName);
                    if (handle != IntPtr.Zero)
                    {
                        music = new SDLMusic(handle, name, fileName);
                        SDLLog.Verbose(LogCategory.AUDIO, "Music loaded from resource '{0}' (via temporary file '{1}')", name, fileName);
                    }
                }
                catch (Exception ex)
                {
                    SDLLog.Error(LogCategory.AUDIO, "Could not load Music from resource '{0}' (via temporary file '{1}'): {2}", name, fileName, ex.Message);
                }
            }
            else
            {
                IntPtr rw = SDLApplication.SDL_RWFromMem(data, data.Length);
                IntPtr handle = Mix_LoadMUS_RW(rw, 1);
                if (handle != IntPtr.Zero)
                {
                    music = new SDLMusic(handle, name);
                    SDLLog.Verbose(LogCategory.AUDIO, "Music loaded from resource '{0}'", name);
                }
            }
        }
        return music;
    }


    private static void MixPostMix(IntPtr udata, IntPtr stream, int len)
    {
        if (currentMusic != null && HasMusicDataHandler)
        {
            if (musicData.Length != len / 2)
            {
                musicData = new short[len / 2];
            }
            Marshal.Copy(stream, musicData, 0, musicData.Length);
            OnMusicDataReceived(new SDLMusicDataEventArgs(currentMusic, musicData));
        }
    }

    private static void MixMusicFinished()
    {
        if (currentMusic != null)
        {
            SDLMusicFinishedEventArgs e = new SDLMusicFinishedEventArgs(currentMusic, MusicFinishReason.Finished);
            currentMusic = null;
            OnMusicFinished(e);
        }
        else
        {
            SDLLog.Warn(LogCategory.AUDIO, $"Music finished callback called on no music");
        }
    }

    private static void OnMusicStarted(SDLMusicEventArgs e)
    {
        ((SDLMusicEventHandler?)eventHandlerList[audioMusicStartKey])?.Invoke(null, e);
    }
    private static void OnMusicFinished(SDLMusicFinishedEventArgs e)
    {
        ((SDLMusicFinishedEventHandler?)eventHandlerList[audioMusicDoneKey])?.Invoke(null, e);
    }
    private static void OnMusicDataReceived(SDLMusicDataEventArgs e)
    {
        ((SDLMusicDataEventHandler?)eventHandlerList[audioDataReceivedKey])?.Invoke(null, e);
    }

    private static bool HasMusicDataHandler => eventHandlerList[audioDataReceivedKey] != null;


    internal static void Track(SDLMusic music)
    {
        musicTracker.Track(music);
    }

    internal static void Untrack(SDLMusic music)
    {
        musicTracker.Untrack(music);
    }


    private const string LibName = "SDL2_mixer";

    private const int MIX_CHANNEL_POST = -2;

    private const ushort AUDIO_U8 = 0x0008;
    private const ushort AUDIO_S8 = 0x8008;
    private const ushort AUDIO_U16LSB = 0x0010;
    private const ushort AUDIO_S16LSB = 0x8010;
    private const ushort AUDIO_U16MSB = 0x1010;
    private const ushort AUDIO_S16MSB = 0x9010;
    private const ushort AUDIO_U16 = AUDIO_U16LSB;
    private const ushort AUDIO_S16 = AUDIO_S16LSB;
    private const ushort AUDIO_S32LSB = 0x8020;
    private const ushort AUDIO_S32MSB = 0x9020;
    private const ushort AUDIO_S32 = AUDIO_S32LSB;
    private const ushort AUDIO_F32LSB = 0x8120;
    private const ushort AUDIO_F32MSB = 0x9120;
    private const ushort AUDIO_F32 = AUDIO_F32LSB;

    private const byte MIX_MAX_VOLUME = 128;
    private const int MIX_DEFAULT_FREQUENCY = 44100;
    private static readonly ushort MIX_DEFAULT_FORMAT = BitConverter.IsLittleEndian ? AUDIO_S16LSB : AUDIO_S16MSB;

    [Flags]
    private enum MIX_InitFlags
    {
        MIX_INIT_FLAC = 0x00000001,
        MIX_INIT_MOD = 0x00000002,
        MIX_INIT_MP3 = 0x00000008,
        MIX_INIT_OGG = 0x00000010,
        MIX_INIT_MID = 0x00000020,
        MIX_INIT_OPUS = 0x00000040
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void MixFuncDelegate(IntPtr udata, IntPtr stream, int len);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void MusicFinishedDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Mix_EffectFunc_t(int chan, IntPtr stream, int len, IntPtr udata);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Mix_EffectDone_t(int channel, IntPtr udata);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int Mix_Init(MIX_InitFlags flags);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void Mix_Quit();
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int Mix_OpenAudio(int frequency, ushort format, int channels, int chunksize);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void Mix_CloseAudio();
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int Mix_AllocateChannels(int numchans);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr Mix_LoadWAV_RW(IntPtr src, int freesrc);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr Mix_LoadMUS([In()][MarshalAs(UnmanagedType.LPUTF8Str)] string file);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr Mix_LoadMUS_RW(IntPtr rwops, int freesrc);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr Mix_QuickLoad_WAV([In()][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] byte[] mem);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr Mix_QuickLoad_RAW([In()][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 1)] byte[] mem, uint len);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Mix_FreeChunk(IntPtr chunk);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Mix_FreeMusic(IntPtr music);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int Mix_PlayMusic(IntPtr music, int loops);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int Mix_FadeInMusic(IntPtr music, int loops, int ms);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int Mix_FadeOutMusic(int ms);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern double Mix_MusicDuration(IntPtr music);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int Mix_VolumeMusic(int volume);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int Mix_HaltMusic();
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void Mix_PauseMusic();
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void Mix_ResumeMusic();
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void Mix_RewindMusic();
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int Mix_RegisterEffect(int channel, Mix_EffectFunc_t f, Mix_EffectDone_t d, IntPtr arg);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int Mix_UnregisterEffect(int channel, Mix_EffectFunc_t f);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int Mix_UnregisterAllEffects(int channel);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void Mix_HookMusicFinished(MusicFinishedDelegate music_finished);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void Mix_HookMusicFinished(IntPtr music_finished);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern MusicType Mix_GetMusicType(IntPtr music);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void Mix_SetPostMix(MixFuncDelegate mix_func, IntPtr arg);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void Mix_SetPostMix(IntPtr mix_func, IntPtr arg);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int Mix_PausedMusic();
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int Mix_PlayingMusic();


    [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr SDL_GetCurrentAudioDriver();

}
