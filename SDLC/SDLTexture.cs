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
        SDLApplication.LogSDLError(SDLRenderer.SDL_QueryTexture(this.handle, out format, out access, out width, out height));
        SDLApplication.LogSDLError(SDLRenderer.SDL_GetTextureScaleMode(this.handle, out textureFilter));
        SDLApplication.LogSDLError(SDLRenderer.SDL_GetTextureAlphaMod(this.handle, out alphaMod));
        SDLApplication.LogSDLError(SDLRenderer.SDL_GetTextureColorMod(this.handle, out byte r, out byte g, out byte b));
        colorMod = Color.FromArgb(r, g, b);
        SDLApplication.LogSDLError(SDLRenderer.SDL_GetTextureBlendMode(this.handle, out blendMode));
        this.renderer.Track(this);
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
                SDLApplication.LogSDLError(SDLRenderer.SDL_SetTextureScaleMode(handle, value));
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
                SDLApplication.LogSDLError(SDLRenderer.SDL_SetTextureAlphaMod(handle, alphaMod));
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
                SDLApplication.LogSDLError(SDLRenderer.SDL_SetTextureColorMod(handle, colorMod.R, colorMod.G, colorMod.B));
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
                SDLApplication.LogSDLError(SDLRenderer.SDL_SetTextureBlendMode(handle, blendMode));
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

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    internal static extern IntPtr IMG_LoadTexture(IntPtr renderer, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string fileName);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr IMG_LoadTexture_RW(IntPtr renderer, IntPtr src, int freesrc);

}
