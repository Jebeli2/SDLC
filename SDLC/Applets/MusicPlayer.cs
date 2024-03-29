﻿// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.Applets;
using System.Collections.Generic;

public class MusicPlayer : SDLApplet
{

    public class PlayListEntry
    {
        public string Title { get; set; } = "";
        public string Name { get; set; } = "";
        public int NumLoops { get; set; } = 1;
    }

    private readonly List<PlayListEntry> playList = new();
    private readonly Queue<PlayListEntry> playQueue = new();
    private int playListIndex = -1;
    private bool repeatPlayList = true;
    private SDLMusic? currentMusic;
    private SDLMusic? lastMusic;

    public MusicPlayer() : base("Music Player")
    {
        noRender = true;
    }

    public PlayListEntry? CurrentEntry
    {
        get
        {
            if (playListIndex >= 0 && playListIndex < playList.Count)
            {
                return playList[playListIndex];
            }
            return null;
        }
    }

    public bool RepeatPlayList
    {
        get => repeatPlayList;
        set
        {
            if (repeatPlayList != value)
            {
                repeatPlayList = value;
            }
        }
    }

    public void ClearPlayList()
    {
        playList.Clear();
        playListIndex = -1;
    }

    public void AddToPlayList(string name, string? title = null, int numLoops = 1)
    {
        PlayListEntry entry = new PlayListEntry() { Name = name, NumLoops = numLoops, Title = title ?? name };
        playList.Add(entry);
    }
    public void PlayNow(string name, string? title = null, int numLoops = 1)
    {
        PlayListEntry entry = new PlayListEntry() { Name = name, NumLoops = numLoops, Title = title ?? name };
        ClearPlayList();
        playList.Add(entry);
        NextMusic();
    }

    public void PrevMusic()
    {
        playListIndex--;
        if (playListIndex < 0 && repeatPlayList)
        {
            playListIndex = playList.Count - 1;
        }
        CheckPlayList();
    }
    public void NextMusic()
    {
        playListIndex++;
        if ((playListIndex >= playList.Count) && repeatPlayList)
        {
            playListIndex = 0;
        }
        CheckPlayList();
    }

    private void CheckPlayList()
    {
        PlayListEntry? entry = CurrentEntry;
        if (entry != null)
        {
            playQueue.Enqueue(entry);
        }
    }

    private void CheckPlayListQueue()
    {
        if (playQueue.Count > 0)
        {
            PlayListEntry entry = playQueue.Dequeue();
            if (!Play(entry))
            {
                SDLLog.Warn(LogCategory.AUDIO, "Could not play '{0}' from playlist. Removing entry, trying next song", entry.Name);
                if (playList.Remove(entry))
                {
                    NextMusic();
                }
            }
        }
    }

    private bool Play(PlayListEntry entry)
    {
        SDLMusic? music = LoadPlayListEntry(entry);
        if (music != null)
        {
            lastMusic = currentMusic;
            currentMusic = music;
            SDLAudio.PlayMusic(currentMusic, entry.NumLoops);
            return true;
        }
        return false;
    }
    private SDLMusic? LoadPlayListEntry(PlayListEntry? entry)
    {
        SDLMusic? music = null;
        if (entry != null)
        {
            music = LoadMusic(entry.Name);
        }
        return music;
    }
    private void SDLAudio_MusicFinished(object? sender, SDLMusicFinishedEventArgs e)
    {
        SDLLog.Verbose(LogCategory.AUDIO, "Music '{0}' stopped ({1})", e.Music.Name, e.Reason);
        if (e.Music == currentMusic)
        {
            currentMusic.Dispose();
            currentMusic = null;
        }
        else if (e.Music == lastMusic)
        {
            lastMusic.Dispose();
            lastMusic = null;
        }
        if (e.Reason == MusicFinishReason.Finished && playList.Count > 0)
        {
            NextMusic();
        }
    }

    protected override void OnWindowUpdate(SDLWindowUpdateEventArgs e)
    {
        CheckPlayListQueue();
    }

    protected override void OnWindowLoad(SDLWindowLoadEventArgs e)
    {
        SDLAudio.MusicFinished += SDLAudio_MusicFinished;
    }


    protected override void OnDispose()
    {
        SDLAudio.MusicFinished -= SDLAudio_MusicFinished;
        currentMusic?.Dispose();
    }
}
