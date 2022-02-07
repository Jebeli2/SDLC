namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

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
            _ = SDLRenderer.SDL_QueryTexture(this.handle, out format, out access, out width, out height);
            _ = SDLRenderer.SDL_GetTextureScaleMode(this.handle, out textureFilter);
            _ = SDLRenderer.SDL_GetTextureAlphaMod(this.handle, out alphaMod);
            _ = SDLRenderer.SDL_GetTextureColorMod(this.handle, out byte r, out byte g, out byte b);
            colorMod = Color.FromArgb(r, g, b);
            _ = SDLRenderer.SDL_GetTextureBlendMode(this.handle, out blendMode);
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
                    _ = SDLRenderer.SDL_SetTextureScaleMode(handle, value);
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
                    _ = SDLRenderer.SDL_SetTextureAlphaMod(handle, alphaMod);
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
                    _ = SDLRenderer.SDL_SetTextureColorMod(handle, colorMod.R, colorMod.G, colorMod.B);
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
                    _ = SDLRenderer.SDL_SetTextureBlendMode(handle, blendMode);
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

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr IMG_LoadTexture(IntPtr renderer, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string fileName);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr IMG_LoadTexture_RW(IntPtr renderer, IntPtr src, int freesrc);

    }
}
