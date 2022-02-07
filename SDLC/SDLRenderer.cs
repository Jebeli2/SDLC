namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class SDLRenderer : IDisposable
    {
        private readonly SDLWindow window;
        private readonly uint format;
        private IntPtr handle;
        private IntPtr backBuffer;
        private BlendMode blendMode;
        private TextureFilter textureFilter = TextureFilter.Nearest;
        private byte colorR;
        private byte colorG;
        private byte colorB;
        private byte colorA;
        private Color color;
        private RendererSizeMode sizeMode;
        private int windowWidth;
        private int windowHeight;
        private int backBufferWidth;
        private int backBufferHeight;
        private int width;
        private int height;
        private readonly Dictionary<string, TextCache> textCache = new();
        private readonly List<string> textCacheKeys = new();
        private int textCacheLimit = 100;
        private readonly SDLObjectTracker<SDLTexture> textureTracker = new("Texture");

        internal SDLRenderer(SDLWindow window)
        {
            this.window = window;
            format = SDL_PIXELFORMAT_ARGB8888;
            windowWidth = backBufferWidth = width = window.Width;
            windowHeight = backBufferHeight = height = window.Height;
        }

        public IntPtr Handle => handle;
        public bool HandleCreated => handle != IntPtr.Zero;
        public int Width => width;
        public int Height => height;

        internal RendererSizeMode SizeMode
        {
            get => sizeMode;
            set
            {
                if (sizeMode != value)
                {
                    sizeMode = value;
                    if (HandleCreated)
                    {
                        CheckBackBuffer();
                    }
                }
            }
        }

        public Color Color
        {
            get => color;
            set => SetColor(value);
        }

        public BlendMode BlendMode
        {
            get => blendMode;
            set => SetBlendMode(value);
        }

        public void SetColor(Color value)
        {
            if (color != value)
            {
                color = value;
                colorR = value.R;
                colorG = value.G;
                colorB = value.B;
                colorA = value.A;
                _ = SDL_SetRenderDrawColor(handle, colorR, colorG, colorB, colorA);
            }
        }

        public void SetBlendMode(BlendMode value)
        {
            if (blendMode != value)
            {
                blendMode = value;
                _ = SDL_SetRenderDrawBlendMode(handle, blendMode);
            }
        }

        internal void SetBackBufferSize(int w, int h)
        {
            backBufferWidth = w;
            backBufferHeight = h;
            CheckBackBuffer();
        }

        internal void CreateHandle()
        {
            int driverIndex = SDLApplication.GetDriverIndex(window.Driver);
            SDL_RendererFlags flags = SDL_RendererFlags.ACCELERATED;
            handle = SDL_CreateRenderer(window.Handle, driverIndex, flags);
            if (handle != IntPtr.Zero)
            {
                _ = SDL_GetRendererInfo(handle, out SDL_RendererInfo info);
                SDLLog.Info($"SDLRenderer {window.WindowId} created: {Marshal.PtrToStringUTF8(info.name)} ({info.max_texture_width}x{info.max_texture_height} max texture size)");
                backBufferWidth = window.BackBufferWidth;
                backBufferHeight = window.BackBufferHeight;
                windowWidth = window.Width;
                windowHeight = window.Height;
                CheckBackBuffer();
            }
            else
            {
                SDLLog.Error($"Could not create SDLRenderer: {SDLApplication.GetError()}");
            }
        }

        private void CheckBackBuffer()
        {
            if (HandleCreated)
            {
                if (backBuffer != IntPtr.Zero)
                {
                    SDL_DestroyTexture(backBuffer);
                    backBuffer = IntPtr.Zero;
                }
                if (sizeMode == RendererSizeMode.BackBuffer)
                {
                    backBuffer = SDL_CreateTexture(handle, format, SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, backBufferWidth, backBufferHeight);
                    _ = SDL_SetTextureAlphaMod(backBuffer, 255);
                    _ = SDL_SetTextureColorMod(backBuffer, 255, 255, 255);
                    _ = SDL_SetTextureBlendMode(backBuffer, BlendMode.Blend);
                    _ = SDL_SetTextureScaleMode(backBuffer, textureFilter);
                    width = backBufferWidth;
                    height = backBufferHeight;
                }
                else
                {
                    width = windowWidth;
                    height = windowHeight;
                }
            }
        }

        internal void WindowResized(int width, int height)
        {
            windowWidth = width;
            windowHeight = height;
            switch (sizeMode)
            {
                case RendererSizeMode.Window:
                    this.width = windowWidth;
                    this.height = windowHeight;
                    break;
                case RendererSizeMode.BackBuffer:
                    this.width = backBufferWidth;
                    this.height = backBufferHeight;
                    break;
            }
            CheckBackBuffer();
        }

        public void Dispose()
        {
            if (handle != IntPtr.Zero)
            {
                ClearTextCache();
                if (backBuffer != IntPtr.Zero)
                {
                    SDL_DestroyTexture(backBuffer);
                    backBuffer = IntPtr.Zero;
                }
                textureTracker.Dispose();
                SDL_DestroyRenderer(handle);
                handle = IntPtr.Zero;
                SDLLog.Info($"SDLRenderer {window.WindowId} destroyed");
            }
        }
        internal void BeginPaint()
        {
            switch (sizeMode)
            {
                case RendererSizeMode.Window:
                    SetColor(Color.Black);
                    SetBlendMode(BlendMode.Blend);
                    _ = SDL_RenderClear(handle);
                    break;
                case RendererSizeMode.BackBuffer:
                    _ = SDL_SetRenderTarget(handle, backBuffer);
                    SetColor(Color.Black);
                    SetBlendMode(BlendMode.Blend);
                    _ = SDL_RenderClear(handle);
                    break;
            }
        }

        internal void EndPaint()
        {
            switch (sizeMode)
            {
                case RendererSizeMode.Window:
                    break;
                case RendererSizeMode.BackBuffer:
                    _ = SDL_SetRenderTarget(handle, IntPtr.Zero);
                    _ = SDL_RenderCopy(handle, backBuffer, IntPtr.Zero, IntPtr.Zero);
                    break;
            }
            SDL_RenderPresent(handle);
        }

        public void DrawRect(int x, int y, int w, int h)
        {
            Rectangle rect = new(x, y, w, h);
            _ = SDL_RenderDrawRect(handle, ref rect);
        }
        public void DrawRect(int x, int y, int w, int h, Color color)
        {
            SetColor(color);
            Rectangle rect = new(x, y, w, h);
            _ = SDL_RenderDrawRect(handle, ref rect);
        }
        public void DrawRect(Rectangle rect)
        {
            _ = SDL_RenderDrawRect(handle, ref rect);
        }
        public void DrawRect(Rectangle rect, Color color)
        {
            SetColor(color);
            _ = SDL_RenderDrawRect(handle, ref rect);
        }
        public void DrawRect(float x, float y, float w, float h)
        {
            RectangleF rect = new(x, y, w, h);
            _ = SDL_RenderDrawRectF(handle, ref rect);
        }
        public void DrawRect(float x, float y, float w, float h, Color color)
        {
            SetColor(color);
            RectangleF rect = new(x, y, w, h);
            _ = SDL_RenderDrawRectF(handle, ref rect);
        }
        public void DrawRect(RectangleF rect)
        {
            _ = SDL_RenderDrawRectF(handle, ref rect);
        }
        public void DrawRect(RectangleF rect, Color color)
        {
            SetColor(color);
            _ = SDL_RenderDrawRectF(handle, ref rect);
        }

        public void DrawTexture(SDLTexture? texture)
        {
            if (texture != null)
            {
                IntPtr texHandle = texture.Handle;
                if (texHandle != IntPtr.Zero)
                {
                    SDL_RenderCopy(handle, texHandle, IntPtr.Zero, IntPtr.Zero);
                }
            }
        }

        public void DrawTexture(SDLTexture? texture, int cx, int cy, double angle)
        {
            if (texture != null)
            {
                IntPtr texHandle = texture.Handle;
                if (texHandle != IntPtr.Zero)
                {
                    Point center = new Point(cx, cy);
                    SDL_RenderCopyEx(handle, texHandle, IntPtr.Zero, IntPtr.Zero, angle, ref center, RendererFlip.None);
                }
            }
        }
        public void DrawTexture(SDLTexture? texture, int x, int y, int width, int height, int cx, int cy, double angle)
        {
            if (texture != null)
            {
                IntPtr texHandle = texture.Handle;
                if (texHandle != IntPtr.Zero)
                {
                    Point center = new Point(cx, cy);
                    Rectangle dst = new Rectangle(x, y, width, height);
                    SDL_RenderCopyEx(handle, texHandle, IntPtr.Zero, ref dst, angle, ref center, RendererFlip.None);
                }
            }
        }
        public void DrawTexture(SDLTexture? texture, int x, int y, int width, int height, double angle)
        {
            if (texture != null)
            {
                IntPtr texHandle = texture.Handle;
                if (texHandle != IntPtr.Zero)
                {
                    Rectangle dst = new Rectangle(x, y, width, height);
                    SDL_RenderCopyEx(handle, texHandle, IntPtr.Zero, ref dst, angle, IntPtr.Zero, RendererFlip.None);
                }
            }
        }

        public void DrawText(SDLFont? font, string? text, float x, float y, float width, float height, Color color, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center, VerticalAlignment verticalAlignment = VerticalAlignment.Center)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (font == null) { font = SDLApplication.DefaultFont; }
            if (font == null) return;
            DrawTextCache(GetTextCache(font, text, color), x, y, width, height, horizontalAlignment, verticalAlignment);
        }

        public void DrawText(SDLFont? font, string? text, float x, float y, Color color)
        {
            DrawText(font, text, x, y, 0, 0, color, HorizontalAlignment.Left, VerticalAlignment.Top);
        }
        public void DrawText(SDLFont? font, string? text, float x, float y)
        {
            DrawText(font, text, x, y, Color);
        }

        public Size MeasureText(SDLFont? font, string? text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (font == null) { font = SDLApplication.DefaultFont; }
                if (font != null)
                {
                    return font.MeasureText(text);
                }
            }
            return Size.Empty;
        }

        public SDLTexture? LoadTexture(string fileName)
        {
            SDLTexture? texture = null;
            if (!string.IsNullOrEmpty(fileName))
            {
                _ = SDLApplication.SDL_SetHint(SDLApplication.SDL_HINT_RENDER_SCALE_QUALITY, ((int)textureFilter).ToString());
                IntPtr tex = SDLTexture.IMG_LoadTexture(handle, fileName);
                if (tex != IntPtr.Zero)
                {
                    texture = new SDLTexture(this, tex, fileName);
                    SDLLog.Info($"SDLTexture loaded from file '{fileName}'");
                }
            }
            return texture;
        }

        public SDLTexture? LoadTexture(string name, byte[]? data)
        {
            SDLTexture? texture = null;
            if (data != null)
            {
                IntPtr rw = SDLApplication.SDL_RWFromMem(data, data.Length);
                if (rw != IntPtr.Zero)
                {
                    _ = SDLApplication.SDL_SetHint(SDLApplication.SDL_HINT_RENDER_SCALE_QUALITY, ((int)textureFilter).ToString());
                    IntPtr tex = SDLTexture.IMG_LoadTexture_RW(handle, rw, 1);
                    if (tex != IntPtr.Zero)
                    {
                        texture = new SDLTexture(this, tex, name);
                        SDLLog.Info($"SDLTexture loaded from resource '{name}'");
                    }
                }
            }
            return texture;
        }

        internal void Track(SDLTexture texture)
        {
            textureTracker.Track(texture);
        }

        internal void Untrack(SDLTexture texture)
        {
            textureTracker.Untrack(texture);
        }

        private void DrawTextCache(TextCache? textCache, float x, float y, float width, float height, HorizontalAlignment hAlign, VerticalAlignment vAlign)
        {
            if (textCache == null) return;
            int w = textCache.Width;
            int h = textCache.Height;
            switch (hAlign)
            {
                case HorizontalAlignment.Left:
                    //nop
                    break;
                case HorizontalAlignment.Right:
                    x = x + width - w;
                    break;
                case HorizontalAlignment.Center:
                    x = x + width / 2 - w / 2;
                    break;
            }
            switch (vAlign)
            {
                case VerticalAlignment.Top:
                    // nop
                    break;
                case VerticalAlignment.Bottom:
                    y = y + height - h;
                    break;
                case VerticalAlignment.Center:
                    y = y + height / 2 - h / 2;
                    break;
            }
            RectangleF dstRect = new RectangleF(x, y, w, h);
            BlendMode = BlendMode.Blend;
            _ = SDL_RenderCopyF(handle, textCache.Handle, IntPtr.Zero, ref dstRect);
        }

        private TextCache? GetTextCache(SDLFont font, string text, Color color)
        {
            CheckTextCache();
            if (textCache.TryGetValue(text, out var tc))
            {
                if (tc.Matches(font, text, color)) return tc;
            }
            tc = CreateTextCache(font, text, color);
            if (tc != null)
            {
                textCache[text] = tc;
                textCacheKeys.Add(text);
            }
            return tc;
        }

        private TextCache? CreateTextCache(SDLFont? font, string? text, Color color)
        {
            TextCache? textCache = null;
            if (font != null && !string.IsNullOrEmpty(text))
            {
                IntPtr fontHandle = font.Handle;
                if (fontHandle != IntPtr.Zero)
                {
                    IntPtr surface = SDLFont.TTF_RenderUTF8_Blended(fontHandle, text, color.ToArgb());
                    if (surface != IntPtr.Zero)
                    {
                        IntPtr texHandle = SDL_CreateTextureFromSurface(handle, surface);
                        if (texHandle != IntPtr.Zero)
                        {
                            _ = SDL_QueryTexture(texHandle, out _, out _, out int w, out int h);
                            _ = SDL_SetTextureAlphaMod(texHandle, color.A);
                            textCache = new TextCache(font, text, color, w, h, texHandle);
                        }
                        SDL_FreeSurface(surface);
                    }
                }
            }
            return textCache;
        }

        private void CheckTextCache()
        {
            if (textCache.Count >= textCacheLimit)
            {
                int len = textCacheKeys.Count / 2;
                var halfKeys = textCacheKeys.GetRange(0, len);
                textCacheKeys.RemoveRange(0, len);
                SDLLog.Verbose($"Text cache limit {textCacheLimit} reached. Cleaning up...");
                ClearTextCache(halfKeys);
            }
        }
        private void ClearTextCache()
        {
            foreach (var kvp in textCache)
            {
                TextCache tc = kvp.Value;
                SDL_DestroyTexture(tc.Handle);
            }
            textCache.Clear();
        }
        private void ClearTextCache(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                if (textCache.TryGetValue(key, out var tc))
                {
                    if (textCache.Remove(key))
                    {
                        SDL_DestroyTexture(tc.Handle);
                    }
                }
            }
        }

        public static uint SDL_FOURCC(byte A, byte B, byte C, byte D)
        {
            return (uint)(A | (B << 8) | (C << 16) | (D << 24));
        }

        public static uint SDL_DEFINE_PIXELFOURCC(byte A, byte B, byte C, byte D)
        {
            return SDL_FOURCC(A, B, C, D);
        }

        private class TextCache
        {
            internal TextCache(SDLFont font, string text, Color color, int width, int height, IntPtr handle)
            {
                Font = font;
                Text = text;
                Color = color;
                Width = width;
                Height = height;
                Handle = handle;
            }

            public SDLFont Font;
            public string Text;
            public Color Color;
            public int Width;
            public int Height;
            public IntPtr Handle;

            public bool Matches(SDLFont font, string text, Color color)
            {
                return (text == Text) && (font == Font) && (color == Color);
            }
        }

        public static uint SDL_DEFINE_PIXELFORMAT(
            SDL_PixelType type,
            uint order,
            SDL_PackedLayout layout,
            byte bits,
            byte bytes
        )
        {
            return (uint)(
                (1 << 28) |
                (((byte)type) << 24) |
                (((byte)order) << 20) |
                (((byte)layout) << 16) |
                (bits << 8) |
                (bytes)
            );
        }

        public static byte SDL_PIXELFLAG(uint X)
        {
            return (byte)((X >> 28) & 0x0F);
        }

        public static byte SDL_PIXELTYPE(uint X)
        {
            return (byte)((X >> 24) & 0x0F);
        }

        public static byte SDL_PIXELORDER(uint X)
        {
            return (byte)((X >> 20) & 0x0F);
        }

        public static byte SDL_PIXELLAYOUT(uint X)
        {
            return (byte)((X >> 16) & 0x0F);
        }

        public static byte SDL_BITSPERPIXEL(uint X)
        {
            return (byte)((X >> 8) & 0xFF);
        }

        public static byte SDL_BYTESPERPIXEL(uint X)
        {
            if (SDL_ISPIXELFORMAT_FOURCC(X))
            {
                if ((X == SDL_PIXELFORMAT_YUY2) ||
                        (X == SDL_PIXELFORMAT_UYVY) ||
                        (X == SDL_PIXELFORMAT_YVYU))
                {
                    return 2;
                }
                return 1;
            }
            return (byte)(X & 0xFF);
        }

        public static bool SDL_ISPIXELFORMAT_INDEXED(uint format)
        {
            if (SDL_ISPIXELFORMAT_FOURCC(format))
            {
                return false;
            }
            SDL_PixelType pType =
                (SDL_PixelType)SDL_PIXELTYPE(format);
            return (
                pType == SDL_PixelType.SDL_PIXELTYPE_INDEX1 ||
                pType == SDL_PixelType.SDL_PIXELTYPE_INDEX4 ||
                pType == SDL_PixelType.SDL_PIXELTYPE_INDEX8
            );
        }

        public static bool SDL_ISPIXELFORMAT_PACKED(uint format)
        {
            if (SDL_ISPIXELFORMAT_FOURCC(format))
            {
                return false;
            }
            SDL_PixelType pType =
                (SDL_PixelType)SDL_PIXELTYPE(format);
            return (
                pType == SDL_PixelType.SDL_PIXELTYPE_PACKED8 ||
                pType == SDL_PixelType.SDL_PIXELTYPE_PACKED16 ||
                pType == SDL_PixelType.SDL_PIXELTYPE_PACKED32
            );
        }

        public static bool SDL_ISPIXELFORMAT_ARRAY(uint format)
        {
            if (SDL_ISPIXELFORMAT_FOURCC(format))
            {
                return false;
            }
            SDL_PixelType pType =
                (SDL_PixelType)SDL_PIXELTYPE(format);
            return (
                pType == SDL_PixelType.SDL_PIXELTYPE_ARRAYU8 ||
                pType == SDL_PixelType.SDL_PIXELTYPE_ARRAYU16 ||
                pType == SDL_PixelType.SDL_PIXELTYPE_ARRAYU32 ||
                pType == SDL_PixelType.SDL_PIXELTYPE_ARRAYF16 ||
                pType == SDL_PixelType.SDL_PIXELTYPE_ARRAYF32
            );
        }

        public static bool SDL_ISPIXELFORMAT_ALPHA(uint format)
        {
            if (SDL_ISPIXELFORMAT_PACKED(format))
            {
                SDL_PackedOrder pOrder =
                    (SDL_PackedOrder)SDL_PIXELORDER(format);
                return (
                    pOrder == SDL_PackedOrder.SDL_PACKEDORDER_ARGB ||
                    pOrder == SDL_PackedOrder.SDL_PACKEDORDER_RGBA ||
                    pOrder == SDL_PackedOrder.SDL_PACKEDORDER_ABGR ||
                    pOrder == SDL_PackedOrder.SDL_PACKEDORDER_BGRA
                );
            }
            else if (SDL_ISPIXELFORMAT_ARRAY(format))
            {
                SDL_ArrayOrder aOrder =
                    (SDL_ArrayOrder)SDL_PIXELORDER(format);
                return (
                    aOrder == SDL_ArrayOrder.SDL_ARRAYORDER_ARGB ||
                    aOrder == SDL_ArrayOrder.SDL_ARRAYORDER_RGBA ||
                    aOrder == SDL_ArrayOrder.SDL_ARRAYORDER_ABGR ||
                    aOrder == SDL_ArrayOrder.SDL_ARRAYORDER_BGRA
                );
            }
            return false;
        }

        public static bool SDL_ISPIXELFORMAT_FOURCC(uint format)
        {
            return (format == 0) && (SDL_PIXELFLAG(format) != 1);
        }

        public enum SDL_PixelType
        {
            SDL_PIXELTYPE_UNKNOWN,
            SDL_PIXELTYPE_INDEX1,
            SDL_PIXELTYPE_INDEX4,
            SDL_PIXELTYPE_INDEX8,
            SDL_PIXELTYPE_PACKED8,
            SDL_PIXELTYPE_PACKED16,
            SDL_PIXELTYPE_PACKED32,
            SDL_PIXELTYPE_ARRAYU8,
            SDL_PIXELTYPE_ARRAYU16,
            SDL_PIXELTYPE_ARRAYU32,
            SDL_PIXELTYPE_ARRAYF16,
            SDL_PIXELTYPE_ARRAYF32
        }

        public enum SDL_BitmapOrder
        {
            SDL_BITMAPORDER_NONE,
            SDL_BITMAPORDER_4321,
            SDL_BITMAPORDER_1234
        }

        public enum SDL_PackedOrder
        {
            SDL_PACKEDORDER_NONE,
            SDL_PACKEDORDER_XRGB,
            SDL_PACKEDORDER_RGBX,
            SDL_PACKEDORDER_ARGB,
            SDL_PACKEDORDER_RGBA,
            SDL_PACKEDORDER_XBGR,
            SDL_PACKEDORDER_BGRX,
            SDL_PACKEDORDER_ABGR,
            SDL_PACKEDORDER_BGRA
        }

        public enum SDL_ArrayOrder
        {
            SDL_ARRAYORDER_NONE,
            SDL_ARRAYORDER_RGB,
            SDL_ARRAYORDER_RGBA,
            SDL_ARRAYORDER_ARGB,
            SDL_ARRAYORDER_BGR,
            SDL_ARRAYORDER_BGRA,
            SDL_ARRAYORDER_ABGR
        }

        public enum SDL_PackedLayout
        {
            SDL_PACKEDLAYOUT_NONE,
            SDL_PACKEDLAYOUT_332,
            SDL_PACKEDLAYOUT_4444,
            SDL_PACKEDLAYOUT_1555,
            SDL_PACKEDLAYOUT_5551,
            SDL_PACKEDLAYOUT_565,
            SDL_PACKEDLAYOUT_8888,
            SDL_PACKEDLAYOUT_2101010,
            SDL_PACKEDLAYOUT_1010102
        }


        public static readonly uint SDL_PIXELFORMAT_UNKNOWN = 0;
        public static readonly uint SDL_PIXELFORMAT_INDEX1LSB =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_INDEX1,
                (uint)SDL_BitmapOrder.SDL_BITMAPORDER_4321,
                0,
                1, 0
            );
        public static readonly uint SDL_PIXELFORMAT_INDEX1MSB =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_INDEX1,
                (uint)SDL_BitmapOrder.SDL_BITMAPORDER_1234,
                0,
                1, 0
            );
        public static readonly uint SDL_PIXELFORMAT_INDEX4LSB =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_INDEX4,
                (uint)SDL_BitmapOrder.SDL_BITMAPORDER_4321,
                0,
                4, 0
            );
        public static readonly uint SDL_PIXELFORMAT_INDEX4MSB =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_INDEX4,
                (uint)SDL_BitmapOrder.SDL_BITMAPORDER_1234,
                0,
                4, 0
            );
        public static readonly uint SDL_PIXELFORMAT_INDEX8 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_INDEX8,
                0,
                0,
                8, 1
            );
        public static readonly uint SDL_PIXELFORMAT_RGB332 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED8,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_XRGB,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_332,
                8, 1
            );
        public static readonly uint SDL_PIXELFORMAT_XRGB444 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED16,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_XRGB,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_4444,
                12, 2
            );
        public static readonly uint SDL_PIXELFORMAT_RGB444 =
            SDL_PIXELFORMAT_XRGB444;
        public static readonly uint SDL_PIXELFORMAT_XBGR444 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED16,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_XBGR,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_4444,
                12, 2
            );
        public static readonly uint SDL_PIXELFORMAT_BGR444 =
            SDL_PIXELFORMAT_XBGR444;
        public static readonly uint SDL_PIXELFORMAT_XRGB1555 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED16,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_XRGB,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_1555,
                15, 2
            );
        public static readonly uint SDL_PIXELFORMAT_RGB555 =
            SDL_PIXELFORMAT_XRGB1555;
        public static readonly uint SDL_PIXELFORMAT_XBGR1555 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_INDEX1,
                (uint)SDL_BitmapOrder.SDL_BITMAPORDER_4321,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_1555,
                15, 2
            );
        public static readonly uint SDL_PIXELFORMAT_BGR555 =
            SDL_PIXELFORMAT_XBGR1555;
        public static readonly uint SDL_PIXELFORMAT_ARGB4444 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED16,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_ARGB,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_4444,
                16, 2
            );
        public static readonly uint SDL_PIXELFORMAT_RGBA4444 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED16,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_RGBA,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_4444,
                16, 2
            );
        public static readonly uint SDL_PIXELFORMAT_ABGR4444 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED16,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_ABGR,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_4444,
                16, 2
            );
        public static readonly uint SDL_PIXELFORMAT_BGRA4444 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED16,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_BGRA,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_4444,
                16, 2
            );
        public static readonly uint SDL_PIXELFORMAT_ARGB1555 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED16,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_ARGB,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_1555,
                16, 2
            );
        public static readonly uint SDL_PIXELFORMAT_RGBA5551 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED16,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_RGBA,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_5551,
                16, 2
            );
        public static readonly uint SDL_PIXELFORMAT_ABGR1555 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED16,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_ABGR,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_1555,
                16, 2
            );
        public static readonly uint SDL_PIXELFORMAT_BGRA5551 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED16,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_BGRA,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_5551,
                16, 2
            );
        public static readonly uint SDL_PIXELFORMAT_RGB565 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED16,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_XRGB,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_565,
                16, 2
            );
        public static readonly uint SDL_PIXELFORMAT_BGR565 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED16,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_XBGR,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_565,
                16, 2
            );
        public static readonly uint SDL_PIXELFORMAT_RGB24 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_ARRAYU8,
                (uint)SDL_ArrayOrder.SDL_ARRAYORDER_RGB,
                0,
                24, 3
            );
        public static readonly uint SDL_PIXELFORMAT_BGR24 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_ARRAYU8,
                (uint)SDL_ArrayOrder.SDL_ARRAYORDER_BGR,
                0,
                24, 3
            );
        public static readonly uint SDL_PIXELFORMAT_XRGB888 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED32,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_XRGB,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_8888,
                24, 4
            );
        public static readonly uint SDL_PIXELFORMAT_RGB888 =
            SDL_PIXELFORMAT_XRGB888;
        public static readonly uint SDL_PIXELFORMAT_RGBX8888 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED32,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_RGBX,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_8888,
                24, 4
            );
        public static readonly uint SDL_PIXELFORMAT_XBGR888 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED32,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_XBGR,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_8888,
                24, 4
            );
        public static readonly uint SDL_PIXELFORMAT_BGR888 =
            SDL_PIXELFORMAT_XBGR888;
        public static readonly uint SDL_PIXELFORMAT_BGRX8888 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED32,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_BGRX,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_8888,
                24, 4
            );
        public static readonly uint SDL_PIXELFORMAT_ARGB8888 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED32,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_ARGB,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_8888,
                32, 4
            );
        public static readonly uint SDL_PIXELFORMAT_RGBA8888 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED32,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_RGBA,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_8888,
                32, 4
            );
        public static readonly uint SDL_PIXELFORMAT_ABGR8888 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED32,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_ABGR,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_8888,
                32, 4
            );
        public static readonly uint SDL_PIXELFORMAT_BGRA8888 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED32,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_BGRA,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_8888,
                32, 4
            );
        public static readonly uint SDL_PIXELFORMAT_ARGB2101010 =
            SDL_DEFINE_PIXELFORMAT(
                SDL_PixelType.SDL_PIXELTYPE_PACKED32,
                (uint)SDL_PackedOrder.SDL_PACKEDORDER_ARGB,
                SDL_PackedLayout.SDL_PACKEDLAYOUT_2101010,
                32, 4
            );
        public static readonly uint SDL_PIXELFORMAT_YV12 =
            SDL_DEFINE_PIXELFOURCC(
                (byte)'Y', (byte)'V', (byte)'1', (byte)'2'
            );
        public static readonly uint SDL_PIXELFORMAT_IYUV =
            SDL_DEFINE_PIXELFOURCC(
                (byte)'I', (byte)'Y', (byte)'U', (byte)'V'
            );
        public static readonly uint SDL_PIXELFORMAT_YUY2 =
            SDL_DEFINE_PIXELFOURCC(
                (byte)'Y', (byte)'U', (byte)'Y', (byte)'2'
            );
        public static readonly uint SDL_PIXELFORMAT_UYVY =
            SDL_DEFINE_PIXELFOURCC(
                (byte)'U', (byte)'Y', (byte)'V', (byte)'Y'
            );
        public static readonly uint SDL_PIXELFORMAT_YVYU =
            SDL_DEFINE_PIXELFOURCC(
                (byte)'Y', (byte)'V', (byte)'Y', (byte)'U'
            );



        private const string LibName = "SDL2";

        [Flags]
        internal enum SDL_RendererFlags : uint
        {
            SOFTWARE = 0x00000001,
            ACCELERATED = 0x00000002,
            PRESENTVSYNC = 0x00000004,
            TARGETTEXTURE = 0x00000008
        }

        private enum SDL_TextureAccess
        {
            SDL_TEXTUREACCESS_STATIC = 0,
            SDL_TEXTUREACCESS_STREAMING = 1,
            SDL_TEXTUREACCESS_TARGET = 2
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct SDL_RendererInfo
        {
            //public IntPtr name; // const char*
            //[MarshalAs(UnmanagedType.LPUTF8Str)]
            public IntPtr name;
            public SDL_RendererFlags flags;
            public uint num_texture_formats;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public uint[] texture_formats;
            public int max_texture_width;
            public int max_texture_height;
        }



        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateRenderer(IntPtr window, int index, SDL_RendererFlags flags);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_DestroyRenderer(IntPtr renderer);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderClear(IntPtr renderer);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_RenderPresent(IntPtr renderer);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetRenderDrawBlendMode(IntPtr renderer, BlendMode blendMode);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetRenderDrawColor(IntPtr renderer, byte r, byte g, byte b, byte a);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawLine(IntPtr renderer, int x1, int y1, int x2, int y2);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawLines(IntPtr renderer, [In()] Point[] points, int count);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawLineF(IntPtr renderer, float x1, float y1, float x2, float y2);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawLinesF(IntPtr renderer, [In()] PointF[] points, int count);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawPoint(IntPtr renderer, int x, int y);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawPointF(IntPtr renderer, float x, float y);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawRect(IntPtr renderer, [In()] ref Rectangle rect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawRectF(IntPtr renderer, [In()] ref RectangleF rect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderFillRect(IntPtr renderer, [In()] ref Rectangle rect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderFillRectF(IntPtr renderer, [In()] ref RectangleF rect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetRenderTarget(IntPtr renderer, IntPtr texture);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_GetRenderTarget(IntPtr renderer);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_QueryTexture(IntPtr texture, out uint format, out int access, out int w, out int h);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_GetTextureScaleMode(IntPtr texture, out TextureFilter scaleMode);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_GetTextureAlphaMod(IntPtr texture, out byte alpha);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_GetTextureBlendMode(IntPtr texture, out BlendMode blendMode);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_GetTextureColorMod(IntPtr texture, out byte r, out byte g, out byte b);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_SetTextureScaleMode(IntPtr texture, TextureFilter scaleMode);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_SetTextureAlphaMod(IntPtr texture, byte alpha);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_SetTextureBlendMode(IntPtr texture, BlendMode blendMode);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_SetTextureColorMod(IntPtr texture, byte r, byte g, byte b);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopy(IntPtr renderer, IntPtr texture, [In()] ref Rectangle srcrect, [In()] ref Rectangle dstrect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopy(IntPtr renderer, IntPtr texture, IntPtr srcrect, [In()] ref Rectangle dstrect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopy(IntPtr renderer, IntPtr texture, [In()] ref Rectangle srcrect, IntPtr dstrect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopy(IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr dstrect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyF(IntPtr renderer, IntPtr texture, [In()] ref Rectangle srcrect, [In()] ref RectangleF dstrect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyF(IntPtr renderer, IntPtr texture, IntPtr srcrect, [In()] ref RectangleF dstrect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyF(IntPtr renderer, IntPtr texture, [In()] ref Rectangle srcrect, IntPtr dstrect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyF(IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr dstrect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, ref Rectangle srcrect, ref Rectangle dstrect, double angle, ref Point center, RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, IntPtr srcrect, ref Rectangle dstrect, double angle, ref Point center, RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, ref Rectangle srcrect, IntPtr dstrect, double angle, ref Point center, RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, ref Rectangle srcrect, ref Rectangle dstrect, double angle, IntPtr center, RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr dstrect, double angle, ref Point center, RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, IntPtr srcrect, ref Rectangle dstrect, double angle, IntPtr center, RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, ref Rectangle srcrect, IntPtr dstrect, double angle, IntPtr center, RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr dstrect, double angle, IntPtr center, RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, ref RectangleF srcrect, ref RectangleF dstrect, double angle, ref PointF center, RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, IntPtr srcrect, ref RectangleF dstrect, double angle, ref PointF center, RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, ref RectangleF srcrect, IntPtr dstrect, double angle, ref PointF center, RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, ref RectangleF srcrect, ref RectangleF dstrect, double angle, IntPtr center, RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr dstrect, double angle, ref PointF center, RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, IntPtr srcrect, ref RectangleF dstrect, double angle, IntPtr center, RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, ref RectangleF srcrect, IntPtr dstrect, double angle, IntPtr center, RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr dstrect, double angle, IntPtr center, RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateTexture(IntPtr renderer, uint format, SDL_TextureAccess access, int w, int h);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SDL_DestroyTexture(IntPtr texture);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateTextureFromSurface(IntPtr renderer, IntPtr surface);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_FreeSurface(IntPtr surface);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_GetRenderDriverInfo(int index, out SDL_RendererInfo info);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetRendererInfo(IntPtr renderer, out SDL_RendererInfo info);


    }
}
