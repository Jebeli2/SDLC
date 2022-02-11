// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;

using System;
using System.Drawing;
using System.Runtime.InteropServices;

public class SDLTexture : SDLObject
{
    private readonly SDLRenderer renderer;
    private readonly int width;
    private readonly int height;
    private readonly uint format;
    private readonly int access;

    private TextureFilter textureFilter;
    private byte alphaMod;
    private Color colorMod;
    private BlendMode blendMode;
    private bool disposedValue;

    internal SDLTexture(SDLRenderer renderer, IntPtr handle, string name)
        : base(handle, name)
    {
        this.renderer = renderer;
        int result = SDLRenderer.SDL_QueryTexture(this.handle, out format, out access, out width, out height);
        if (result == 0)
        {
            CheckedSDLCall(() => SDLRenderer.SDL_GetTextureScaleMode(this.handle, out textureFilter), nameof(SDLRenderer.SDL_GetTextureScaleMode));
            CheckedSDLCall(() => SDLRenderer.SDL_GetTextureAlphaMod(this.handle, out alphaMod), nameof(SDLRenderer.SDL_GetTextureAlphaMod));
            byte r = 0;
            byte g = 0;
            byte b = 0;
            CheckedSDLCall(() => SDLRenderer.SDL_GetTextureColorMod(this.handle, out r, out g, out b), nameof(SDLRenderer.SDL_GetTextureColorMod));
            colorMod = Color.FromArgb(r, g, b);
            CheckedSDLCall(() => SDLRenderer.SDL_GetTextureBlendMode(this.handle, out blendMode), nameof(SDLRenderer.SDL_GetTextureBlendMode));
            this.renderer.Track(this);
        }
        else
        {
            SDLLog.Error(LogCategory.RENDER, "{0} returned an error: {1} ({2})", nameof(SDLRenderer.SDL_QueryTexture), result, SDLApplication.GetError());
        }
    }
    public int Width => width;
    public int Height => height;
    public TextureFilter TextureFilter
    {
        get => textureFilter;
        set
        {
            if (textureFilter != value)
            {
                textureFilter = value;
                CheckedSDLCall(() => SDLRenderer.SDL_SetTextureScaleMode(handle, value), nameof(SDLRenderer.SDL_SetTextureScaleMode));
            }
        }
    }

    public byte AlphaMod
    {
        get => alphaMod;
        set
        {
            if (alphaMod != value)
            {
                alphaMod = value;
                CheckedSDLCall(() => SDLRenderer.SDL_SetTextureAlphaMod(handle, alphaMod), nameof(SDLRenderer.SDL_SetTextureAlphaMod));
            }
        }
    }

    public Color ColorMod
    {
        get => colorMod;
        set
        {
            if (colorMod != value)
            {
                colorMod = value;
                CheckedSDLCall(() => SDLRenderer.SDL_SetTextureColorMod(handle, colorMod.R, colorMod.G, colorMod.B), nameof(SDLRenderer.SDL_SetTextureColorMod));
            }
        }
    }

    public BlendMode BlendMode
    {
        get => blendMode;
        set
        {
            if (blendMode != value)
            {
                blendMode = value;
                CheckedSDLCall(() => SDLRenderer.SDL_SetTextureBlendMode(handle, blendMode), nameof(SDLRenderer.SDL_SetTextureBlendMode));
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                renderer.Untrack(this);
            }
            if (handle != IntPtr.Zero)
            {
                SDLRenderer.SDL_DestroyTexture(handle);
            }
            disposedValue = true;
        }
        base.Dispose(disposing);
    }

    private static void CheckedSDLCall(Func<int> func, string funcName)
    {
        int result = func();
        if (result != 0)
        {
            SDLLog.Error(LogCategory.RENDER, "{0} returned an error: {1} ({2})", funcName, result, SDLApplication.GetError());
        }
    }


    private const string LibName = "SDL2_image";
    [Flags]
    internal enum IMG_InitFlags
    {
        IMG_INIT_JPG = 0x00000001,
        IMG_INIT_PNG = 0x00000002,
        IMG_INIT_TIF = 0x00000004,
        IMG_INIT_WEBP = 0x00000008
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int IMG_Init(IMG_InitFlags flags);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void IMG_Quit();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr IMG_LoadTexture(IntPtr renderer, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string fileName);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr IMG_LoadTexture_RW(IntPtr renderer, IntPtr src, int freesrc);

}
