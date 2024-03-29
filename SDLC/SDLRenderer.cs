﻿// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.
namespace SDLC;

using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal sealed class SDLRenderer : IRenderer, IDisposable
{
    private readonly SDLWindow window;
    private readonly StringBuilder stringBuffer = new(512);
    private readonly uint format;
    private IntPtr handle;
    private IntPtr backBuffer;
    private readonly Stack<IntPtr> prevTargets = new();
    private readonly Stack<Rectangle> prevClips = new();
    private BlendMode blendMode;
    private TextureFilter textureFilter = TextureFilter.Nearest;
    private byte colorR;
    private byte colorG;
    private byte colorB;
    private byte colorA;
    private Color color;
    private RendererSizeMode sizeMode;
    private bool checkStateOnPaint = true;
    private bool disableClipping = false;
    private int windowWidth;
    private int windowHeight;
    private int backBufferWidth;
    private int backBufferHeight;
    private int width;
    private int height;
    private readonly Dictionary<int, TextCache> textCache = new();
    private readonly List<int> textCacheKeys = new();
    private int textCacheLimit = 100;
    private readonly Dictionary<int, IconCache> iconCache = new();
    private readonly List<int> iconCacheKeys = new();
    private int iconCacheLimit = 100;
    private readonly SDLObjectTracker<SDLTexture> textureTracker = new(LogCategory.RENDER, "Texture");
    // Indices for 4 rectangle vertices: bottomleft-topleft-topright, topright,bottomright,bottomleft
    private readonly int[] rectIndices = new int[] { 2, 0, 1, 1, 3, 2 };
    private const int NUM_RECT_INDICES = 6;
    private readonly SDL_Vertex[] rectVertices = new SDL_Vertex[4];

    internal SDLRenderer(SDLWindow window)
    {
        this.window = window;
        format = SDL_PIXELFORMAT_ARGB8888;
        windowWidth = window.Width;
        windowHeight = window.Height;
        backBufferWidth = window.BackBufferWidth;
        backBufferHeight = window.BackBufferHeight;
        width = window.Width;
        height = window.Height;
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
            SDLApplication.LogSDLError(SDL_SetRenderDrawColor(handle, colorR, colorG, colorB, colorA));
        }
    }

    public void SetBlendMode(BlendMode value)
    {
        if (blendMode != value)
        {
            blendMode = value;
            SDLApplication.LogSDLError(SDL_SetRenderDrawBlendMode(handle, blendMode));
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
            SDLApplication.LogSDLError(SDL_GetRendererInfo(handle, out SDL_RendererInfo info));
            SDLLog.Info(LogCategory.RENDER, "Renderer {0} created: {1} ({2}x{3} max texture size)", window.WindowId, Marshal.PtrToStringUTF8(info.name), info.max_texture_width, info.max_texture_height);
            backBufferWidth = window.BackBufferWidth;
            backBufferHeight = window.BackBufferHeight;
            windowWidth = window.Width;
            windowHeight = window.Height;
            CheckBackBuffer();
        }
        else
        {
            SDLLog.Critical(LogCategory.RENDER, "Could not create Renderer: {0}", SDLApplication.GetError());
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
                if (backBuffer != IntPtr.Zero)
                {
                    SDLApplication.LogSDLError(SDL_SetTextureAlphaMod(backBuffer, 255));
                    SDLApplication.LogSDLError(SDL_SetTextureColorMod(backBuffer, 255, 255, 255));
                    SDLApplication.LogSDLError(SDL_SetTextureBlendMode(backBuffer, BlendMode.Blend));
                    SDLApplication.LogSDLError(SDL_SetTextureScaleMode(backBuffer, textureFilter));
                    width = backBufferWidth;
                    height = backBufferHeight;
                }
            }
            else
            {
                width = windowWidth;
                height = windowHeight;
            }
        }
    }

    internal void WindowResized(int width, int height, WindowResizeSource source)
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
            ClearIconCache();
            if (backBuffer != IntPtr.Zero)
            {
                SDL_DestroyTexture(backBuffer);
                backBuffer = IntPtr.Zero;
            }
            textureTracker.Dispose();
            SDL_DestroyRenderer(handle);
            handle = IntPtr.Zero;
            SDLLog.Info(LogCategory.RENDER, "Renderer {0} destroyed", window.WindowId);
        }
    }
    internal void BeginPaint()
    {
        prevTargets.Clear();
        prevClips.Clear();
        switch (sizeMode)
        {
            case RendererSizeMode.Window:
                SetColor(Color.Black);
                SetBlendMode(BlendMode.Blend);
                SDLApplication.LogSDLError(SDL_RenderClear(handle));
                break;
            case RendererSizeMode.BackBuffer:
                SDLApplication.LogSDLError(SDL_SetRenderTarget(handle, backBuffer));
                SetColor(Color.Black);
                SetBlendMode(BlendMode.Blend);
                SDLApplication.LogSDLError(SDL_RenderClear(handle));
                break;
        }
    }

    internal void EndPaint()
    {
        if (checkStateOnPaint)
        {
            if (prevClips.Count > 0) { SDLLog.Warn(LogCategory.RENDER, $"ClipRects not empty: {prevClips.Count} {(prevClips.Count == 1 ? "was" : "were")} not popped"); }
            if (prevTargets.Count > 0) { SDLLog.Warn(LogCategory.RENDER, $"Targets not empty: {prevTargets.Count} {(prevTargets.Count == 1 ? "was" : "were")} not popped"); }
        }
        switch (sizeMode)
        {
            case RendererSizeMode.Window:
                break;
            case RendererSizeMode.BackBuffer:
                SDLApplication.LogSDLError(SDL_SetRenderTarget(handle, IntPtr.Zero));
                //SDLApplication.LogSDLError(SDL_RenderCopy(handle, backBuffer, IntPtr.Zero, IntPtr.Zero));
                Rectangle src = new Rectangle(0, 0, backBufferWidth, backBufferHeight);
                Rectangle dst = new Rectangle(0, 0, windowWidth, windowHeight);
                SDLApplication.LogSDLError(SDL_RenderCopy(handle, backBuffer, ref src, ref dst));
                break;
        }
        SDL_RenderPresent(handle);
    }

    public void DrawRect(Rectangle rect)
    {
        SDLApplication.LogSDLError(SDL_RenderDrawRect(handle, ref rect));
    }
    public void DrawRect(RectangleF rect)
    {
        SDLApplication.LogSDLError(SDL_RenderDrawRectF(handle, ref rect));
    }
    public void DrawRects(IEnumerable<Rectangle> rects)
    {
        Rectangle[] rcts = rects.AsArray();
        if (rcts.Length > 0)
        {
            SDLApplication.LogSDLError(SDL_RenderDrawRects(handle, rcts, rcts.Length));
        }
    }
    public void DrawRects(IEnumerable<RectangleF> rects)
    {
        RectangleF[] rcts = rects.AsArray();
        if (rcts.Length > 0)
        {
            SDLApplication.LogSDLError(SDL_RenderDrawRectsF(handle, rcts, rcts.Length));
        }
    }

    public void FillRect(Rectangle rect)
    {
        SDLApplication.LogSDLError(SDL_RenderFillRect(Handle, ref rect));
    }
    public void FillRect(RectangleF rect)
    {
        SDLApplication.LogSDLError(SDL_RenderFillRectF(Handle, ref rect));
    }

    public void DrawLine(int x1, int y1, int x2, int y2)
    {
        SDLApplication.LogSDLError(SDL_RenderDrawLine(handle, x1, y1, x2, y2));
    }

    public void DrawLine(float x1, float y1, float x2, float y2)
    {
        SDLApplication.LogSDLError(SDL_RenderDrawLineF(handle, x1, y1, x2, y2));
    }

    public void DrawLines(IEnumerable<Point> points)
    {
        Point[] pts = points.AsArray();
        if (pts.Length > 0)
        {
            SDLApplication.LogSDLError(SDL_RenderDrawLines(handle, pts, pts.Length));
        }
    }
    public void DrawLines(IEnumerable<PointF> points)
    {
        PointF[] pts = points.AsArray();
        if (pts.Length > 0)
        {
            SDLApplication.LogSDLError(SDL_RenderDrawLinesF(handle, pts, pts.Length));
        }
    }

    public void DrawPoint(int x, int y)
    {
        SDLApplication.LogSDLError(SDL_RenderDrawPoint(handle, x, y));
    }
    public void DrawPoint(float x, float y)
    {
        SDLApplication.LogSDLError(SDL_RenderDrawPointF(handle, x, y));
    }

    private static int ToSDLColor(Color c)
    {
        int i = c.A;
        i <<= 8;
        i |= c.B;
        i <<= 8;
        i |= c.G;
        i <<= 8;
        i |= c.R;
        return i;
    }

    public void FillColorRect(Rectangle rect, Color colorTopLeft, Color colorTopRight, Color colorBottomLeft, Color colorBottomRight)
    {
        rectVertices[0].color = ToSDLColor(colorTopLeft);
        rectVertices[0].position.X = rect.X;
        rectVertices[0].position.Y = rect.Y;
        rectVertices[1].color = ToSDLColor(colorTopRight);
        rectVertices[1].position.X = rect.Right;
        rectVertices[1].position.Y = rect.Y;
        rectVertices[2].color = ToSDLColor(colorBottomLeft);
        rectVertices[2].position.X = rect.X;
        rectVertices[2].position.Y = rect.Bottom;
        rectVertices[3].color = ToSDLColor(colorBottomRight);
        rectVertices[3].position.X = rect.Right;
        rectVertices[3].position.Y = rect.Bottom;
        SDLApplication.LogSDLError(SDL_RenderGeometry(handle, IntPtr.Zero, rectVertices, 4, rectIndices, NUM_RECT_INDICES));
    }

    public void FillColorRect(RectangleF rect, Color colorTopLeft, Color colorTopRight, Color colorBottomLeft, Color colorBottomRight)
    {
        rectVertices[0].color = ToSDLColor(colorTopLeft);
        rectVertices[0].position.X = rect.X;
        rectVertices[0].position.Y = rect.Y;
        rectVertices[1].color = ToSDLColor(colorTopRight);
        rectVertices[1].position.X = rect.Right;
        rectVertices[1].position.Y = rect.Y;
        rectVertices[2].color = ToSDLColor(colorBottomLeft);
        rectVertices[2].position.X = rect.X;
        rectVertices[2].position.Y = rect.Bottom;
        rectVertices[3].color = ToSDLColor(colorBottomRight);
        rectVertices[3].position.X = rect.Right;
        rectVertices[3].position.Y = rect.Bottom;
        SDLApplication.LogSDLError(SDL_RenderGeometry(handle, IntPtr.Zero, rectVertices, 4, rectIndices, NUM_RECT_INDICES));
    }

    public void DrawTexture(SDLTexture? texture, Rectangle src, Rectangle dst)
    {
        if (texture.GetTextureHandle(out IntPtr th))
        {
            SDLApplication.LogSDLError(SDL_RenderCopy(handle, th, ref src, ref dst));
        }
    }
    public void DrawTexture(SDLTexture? texture, Rectangle dst)
    {
        if (texture.GetTextureHandle(out IntPtr th))
        {
            SDLApplication.LogSDLError(SDL_RenderCopy(handle, th, IntPtr.Zero, ref dst));
        }
    }
    public void DrawTexture(SDLTexture? texture)
    {
        if (texture.GetTextureHandle(out IntPtr th))
        {
            SDLApplication.LogSDLError(SDL_RenderCopy(handle, th, IntPtr.Zero, IntPtr.Zero));
        }
    }
    public void DrawTexture(SDLTexture? texture, Rectangle src, RectangleF dst)
    {
        if (texture.GetTextureHandle(out IntPtr th))
        {
            SDLApplication.LogSDLError(SDL_RenderCopyF(handle, th, ref src, ref dst));
        }
    }
    public void DrawTexture(SDLTexture? texture, RectangleF dst)
    {
        if (texture.GetTextureHandle(out IntPtr th))
        {
            SDLApplication.LogSDLError(SDL_RenderCopyF(handle, th, IntPtr.Zero, ref dst));
        }
    }

    public void DrawTexture(SDLTexture? texture, Rectangle src, Rectangle dst, double angle, Point center, RendererFlip flip = RendererFlip.None)
    {
        if (texture.GetTextureHandle(out IntPtr th))
        {
            SDLApplication.LogSDLError(SDL_RenderCopyEx(handle, th, ref src, ref dst, angle, ref center, flip));
        }
    }
    public void DrawTexture(SDLTexture? texture, Rectangle src, Rectangle dst, double angle, RendererFlip flip = RendererFlip.None)
    {
        if (texture.GetTextureHandle(out IntPtr th))
        {
            SDLApplication.LogSDLError(SDL_RenderCopyEx(handle, th, ref src, ref dst, angle, IntPtr.Zero, flip));
        }
    }
    public void DrawTexture(SDLTexture? texture, Rectangle dst, double angle, Point center, RendererFlip flip = RendererFlip.None)
    {
        if (texture.GetTextureHandle(out IntPtr th))
        {
            SDLApplication.LogSDLError(SDL_RenderCopyEx(handle, th, IntPtr.Zero, ref dst, angle, ref center, flip));
        }
    }
    public void DrawTexture(SDLTexture? texture, Rectangle dst, double angle, RendererFlip flip = RendererFlip.None)
    {
        if (texture.GetTextureHandle(out IntPtr th))
        {
            SDLApplication.LogSDLError(SDL_RenderCopyEx(handle, th, IntPtr.Zero, ref dst, angle, IntPtr.Zero, flip));
        }
    }
    public void DrawTexture(SDLTexture? texture, RectangleF dst, double angle, RendererFlip flip = RendererFlip.None)
    {
        if (texture.GetTextureHandle(out IntPtr th))
        {
            SDLApplication.LogSDLError(SDL_RenderCopyExF(handle, th, IntPtr.Zero, ref dst, angle, IntPtr.Zero, flip));
        }
    }
    public void DrawTextureRects(SDLTexture? texture, IList<RectangleF> rects, IList<Color> colors)
    {
        if (texture.GetTextureHandle(out IntPtr th))
        {
            SDL_Vertex[] vertices = new SDL_Vertex[rects.Count * 4];
            int[] indices = new int[rects.Count * 6];
            int index = 0;
            int j = 0;
            for (int i = 0; i < rects.Count; i++)
            {
                RectangleF rect = rects[i];
                var c = ToSDLColor(colors[i]);
                vertices[index + 0].color = c;
                vertices[index + 0].position.X = rect.X;
                vertices[index + 0].position.Y = rect.Y;
                vertices[index + 0].tex_coord.X = 0;
                vertices[index + 0].tex_coord.Y = 0;
                vertices[index + 1].color = c;
                vertices[index + 1].position.X = rect.Right;
                vertices[index + 1].position.Y = rect.Y;
                vertices[index + 1].tex_coord.X = 1;
                vertices[index + 1].tex_coord.Y = 0;
                vertices[index + 2].color = c;
                vertices[index + 2].position.X = rect.X;
                vertices[index + 2].position.Y = rect.Bottom;
                vertices[index + 2].tex_coord.X = 0;
                vertices[index + 2].tex_coord.Y = 1;
                vertices[index + 3].color = c;
                vertices[index + 3].position.X = rect.Right;
                vertices[index + 3].position.Y = rect.Bottom;
                vertices[index + 3].tex_coord.X = 1;
                vertices[index + 3].tex_coord.Y = 1;
                indices[j + 0] = index + 2;
                indices[j + 1] = index;
                indices[j + 2] = index + 1;
                indices[j + 3] = index + 1;
                indices[j + 4] = index + 3;
                indices[j + 5] = index + 2;
                index += 4;
                j += 6;
            }
            SDLApplication.LogSDLError(SDL_RenderGeometry(handle, th, vertices, vertices.Length, indices, indices.Length));
        }
    }
    public void DrawIcon(Icons icon, float x, float y, float width, float height, Color color, HorizontalAlignment hAlign = HorizontalAlignment.Center, VerticalAlignment vAlign = VerticalAlignment.Center, float offsetX = 0, float offsetY = 0)
    {
        if (icon == Icons.NONE) return;
        SDLFont? font = SDLApplication.IconFont;
        if (font == null) return;
        DrawIconCache(GetIconCache(font, icon, color), x, y, width, height, hAlign, vAlign, offsetX, offsetY);
    }

    public void DrawText(SDLFont? font, ReadOnlySpan<char> text, float x, float y, float width, float height, Color color, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center, VerticalAlignment verticalAlignment = VerticalAlignment.Center, float offsetX = 0, float offsetY = 0)
    {
        if (text == null) return;
        if (text.Length == 0) return;
        font ??= SDLApplication.DefaultFont;
        if (font == null) return;
        DrawTextCache(GetTextCache(font, text, color), x, y, width, height, horizontalAlignment, verticalAlignment, offsetX, offsetY);
    }

    public Size MeasureText(SDLFont? font, ReadOnlySpan<char> text)
    {
        if (text != null && text.Length > 0)
        {
            font ??= SDLApplication.DefaultFont;
            if (font != null)
            {
                return font.MeasureText(text);
            }
        }
        return Size.Empty;
    }

    public void GetGlyphMetrics(SDLFont? font, char c, out int minx, out int maxx, out int miny, out int maxy, out int advance)
    {
        font ??= SDLApplication.DefaultFont;
        if (font != null)
        {
            font.GetGlyphMetrics(c, out minx, out maxx, out miny, out maxy, out advance);
        }
        else
        {
            minx = 0;
            maxx = 0;
            miny = 0;
            maxy = 0;
            advance = 0;
        }
    }
    public SDLTexture? LoadTexture(string fileName)
    {
        SDLTexture? texture = textureTracker.Find(fileName);
        if (texture == null && !string.IsNullOrEmpty(fileName))
        {
            _ = SDLApplication.SetHint(SDLApplication.SDL_HINT_RENDER_SCALE_QUALITY, (int)textureFilter);
            IntPtr tex = SDLTexture.IMG_LoadTexture(handle, fileName);
            if (tex != IntPtr.Zero)
            {
                texture = new SDLTexture(this, tex, fileName);
                SDLLog.Verbose(LogCategory.RENDER, "Texture loaded from file '{0}'", fileName);
            }
        }
        return texture;
    }

    public SDLTexture? LoadTexture(string name, byte[]? data)
    {
        SDLTexture? texture = textureTracker.Find(name);
        if (texture == null && data != null)
        {
            IntPtr rw = SDLApplication.SDL_RWFromMem(data, data.Length);
            if (rw != IntPtr.Zero)
            {
                _ = SDLApplication.SetHint(SDLApplication.SDL_HINT_RENDER_SCALE_QUALITY, (int)textureFilter);
                IntPtr tex = SDLTexture.IMG_LoadTexture_RW(handle, rw, 1);
                if (tex != IntPtr.Zero)
                {
                    texture = new SDLTexture(this, tex, name);
                    SDLLog.Verbose(LogCategory.RENDER, "Texture loaded from resource '{0}'", name);
                }
            }
        }
        return texture;
    }

    public SDLTexture? CreateTexture(string name, int width, int height)
    {
        SDLTexture? texture = textureTracker.Find(name);
        if (texture == null)
        {
            IntPtr tex = SDL_CreateTexture(handle, format, SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, width, height);
            if (tex != IntPtr.Zero)
            {
                texture = new SDLTexture(this, tex, name);
                SDLLog.Verbose(LogCategory.RENDER, "Texture created from scratch '{0}'", name);
            }
        }
        return texture;
    }
    public void ClearScreen()
    {
        SDLApplication.LogSDLError(SDL_RenderClear(handle));
    }
    public void PushTarget(SDLTexture? texture)
    {
        if (texture != null)
        {
            IntPtr oldTarget = SDL_GetRenderTarget(handle);
            prevTargets.Push(oldTarget);
            SDLApplication.LogSDLError(SDL_SetRenderTarget(handle, texture.Handle));
            SDLApplication.LogSDLError(SDL_SetRenderDrawBlendMode(handle, blendMode));
        }
    }
    public void PopTarget()
    {
        if (prevTargets.Count > 0)
        {
            IntPtr oldTarget = prevTargets.Pop();
            SDLApplication.LogSDLError(SDL_SetRenderTarget(handle, oldTarget));
        }
    }

    public Rectangle CurrentClip
    {
        get
        {
            SDLApplication.LogSDLError(SDL_RenderGetClipRect(handle, out Rectangle clip));
            return clip;
        }
    }

    private Rectangle CombineClip(Rectangle clip)
    {
        if (prevClips.Count > 0)
        {
            Rectangle current = prevClips.Peek();
            return Rectangle.Intersect(current, clip);
        }
        return clip;
    }

    public void PushClip(Rectangle clip)
    {
        if (disableClipping) return;
        clip = CombineClip(clip);
        SDLApplication.LogSDLError(SDL_RenderSetClipRect(handle, ref clip));
        prevClips.Push(clip);
    }
    public void PopClip()
    {
        if (disableClipping) return;
        if (prevClips.Count > 0) { _ = prevClips.Pop(); }
        if (prevClips.Count > 0)
        {
            Rectangle clip = prevClips.Peek();
            SDLApplication.LogSDLError(SDL_RenderSetClipRect(handle, ref clip));
        }
        else
        {
            SDLApplication.LogSDLError(SDL_RenderSetClipRect(handle, IntPtr.Zero));
        }
    }

    public void SetClip(Rectangle clip)
    {
        if (disableClipping) return;
        SDLApplication.LogSDLError(SDL_RenderSetClipRect(handle, ref clip));
    }
    public void ClearClip()
    {
        if (disableClipping) return;
        SDLApplication.LogSDLError(SDL_RenderSetClipRect(handle, IntPtr.Zero));
    }


    internal void Track(SDLTexture texture)
    {
        textureTracker.Track(texture);
    }

    internal void Untrack(SDLTexture texture)
    {
        textureTracker.Untrack(texture);
    }

    private void DrawTextCache(TextCache? textCache, float x, float y, float width, float height, HorizontalAlignment hAlign, VerticalAlignment vAlign, float offsetX, float offsetY)
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
        RectangleF dstRect = new RectangleF(x + offsetX, y + offsetY, w, h);
        BlendMode = BlendMode.Blend;
        SDLApplication.LogSDLError(SDL_RenderCopyF(handle, textCache.Handle, IntPtr.Zero, ref dstRect));
    }

    private void DrawIconCache(IconCache? iconCache, float x, float y, float width, float height, HorizontalAlignment hAlign, VerticalAlignment vAlign, float offsetX, float offsetY)
    {
        if (iconCache == null) return;
        int w = iconCache.Width;
        int h = iconCache.Height;
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
        RectangleF dstRect = new RectangleF(x + offsetX, y + offsetY, w, h);
        BlendMode = BlendMode.Blend;
        SDLApplication.LogSDLError(SDL_RenderCopyF(handle, iconCache.Handle, IntPtr.Zero, ref dstRect));
    }

    private TextCache? GetTextCache(SDLFont font, ReadOnlySpan<char> text, Color color)
    {
        int hash = string.GetHashCode(text);
        int key = MakeTextCacheKey(font, hash, color);
        CheckTextCache();
        if (textCache.TryGetValue(key, out var tc))
        {
            if (tc.Matches(font, hash, color)) return tc;
        }
        tc = CreateTextCache(font, text, color);
        if (tc != null)
        {
            textCache[key] = tc;
            textCacheKeys.Add(key);
        }
        return tc;
    }


    private IconCache? GetIconCache(SDLFont font, Icons icon, Color color)
    {
        int key = MakeIconCacheKey(icon, color);
        CheckIconCache();
        if (iconCache.TryGetValue(key, out var ic))
        {
            if (ic.Matches(icon, color)) { return ic; }
        }
        ic = CreateIconCache(font, icon, color);
        if (ic != null)
        {
            iconCache[key] = ic;
            iconCacheKeys.Add(key);
        }
        return ic;
    }
    private TextCache? CreateTextCache(SDLFont? font, ReadOnlySpan<char> text, Color color)
    {
        TextCache? textCache = null;
        if (font != null && text.Length > 0)
        {
            IntPtr fontHandle = font.Handle;
            if (fontHandle != IntPtr.Zero)
            {
                stringBuffer.Clear();
                stringBuffer.Append(text);
                int hash = string.GetHashCode(text);
                IntPtr surface = SDLFont.TTF_RenderUTF8_Blended(fontHandle, stringBuffer, color.ToArgb());
                if (surface != IntPtr.Zero)
                {
                    IntPtr texHandle = SDL_CreateTextureFromSurface(handle, surface);
                    if (texHandle != IntPtr.Zero)
                    {
                        SDLApplication.LogSDLError(SDL_QueryTexture(texHandle, out _, out _, out int w, out int h));
                        SDLApplication.LogSDLError(SDL_SetTextureAlphaMod(texHandle, color.A));
                        textCache = new TextCache(font, hash, color, w, h, texHandle);
                    }
                    SDL_FreeSurface(surface);
                }
            }
        }
        return textCache;
    }

    private IconCache? CreateIconCache(SDLFont? font, Icons icon, Color color)
    {
        IconCache? iconCache = null;
        if (font != null)
        {
            IntPtr fontHandle = font.Handle;
            if (fontHandle != IntPtr.Zero)
            {
                IntPtr surface = SDLFont.TTF_RenderGlyph_Blended(fontHandle, (ushort)icon, color.ToArgb());
                if (surface != IntPtr.Zero)
                {
                    IntPtr texHandle = SDL_CreateTextureFromSurface(handle, surface);
                    if (texHandle != IntPtr.Zero)
                    {
                        if (texHandle != IntPtr.Zero)
                        {
                            SDLApplication.LogSDLError(SDL_QueryTexture(texHandle, out _, out _, out int w, out int h));
                            SDLApplication.LogSDLError(SDL_SetTextureAlphaMod(texHandle, color.A));
                            iconCache = new IconCache(icon, color, w, h, texHandle);
                        }
                    }
                    SDL_FreeSurface(surface);
                }
            }
        }
        return iconCache;
    }


    private void CheckTextCache()
    {
        if (textCache.Count >= textCacheLimit)
        {
            int len = textCacheKeys.Count / 2;
            var halfKeys = textCacheKeys.GetRange(0, len);
            textCacheKeys.RemoveRange(0, len);
            SDLLog.Verbose(LogCategory.RENDER, "Text cache limit {0} reached. Cleaning up...", textCacheLimit);
            ClearTextCache(halfKeys);
        }
    }

    private void CheckIconCache()
    {
        if (iconCache.Count > iconCacheLimit)
        {
            int len = iconCacheKeys.Count / 2;
            var halfKeys = iconCacheKeys.GetRange(0, len);
            iconCacheKeys.RemoveRange(0, len);
            SDLLog.Verbose(LogCategory.RENDER, "Icon cache limit {0} reached. Cleaning up...", iconCacheLimit);
            ClearIconCache(halfKeys);
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
    private void ClearTextCache(IEnumerable<int> keys)
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

    private void ClearIconCache()
    {
        foreach (var kvp in iconCache)
        {
            IconCache tc = kvp.Value;
            SDL_DestroyTexture(tc.Handle);
        }
        iconCache.Clear();
    }

    private void ClearIconCache(IEnumerable<int> keys)
    {
        foreach (var key in keys)
        {
            if (iconCache.TryGetValue(key, out var tc))
            {
                if (iconCache.Remove(key))
                {
                    SDL_DestroyTexture(tc.Handle);
                }
            }
        }
    }

    //TODO: Make reasonable hashes for icon and text caches...
    private static int MakeIconCacheKey(Icons icon, Color color)
    {
        return HashCode.Combine(icon.GetHashCode(), color.GetHashCode());
    }
    //TODO: Make reasonable hashes for icon and text caches...
    private static int MakeTextCacheKey(SDLFont font, int textHash, Color color)
    {

        return HashCode.Combine(font.FontId, textHash, color.GetHashCode());
    }

    private class TextCache
    {
        internal TextCache(SDLFont font, int textHash, Color color, int width, int height, IntPtr handle)
        {
            Font = font;
            TextHash = textHash;
            Color = color;
            Width = width;
            Height = height;
            Handle = handle;
        }

        public SDLFont Font;
        public int TextHash;
        public Color Color;
        public int Width;
        public int Height;
        public IntPtr Handle;

        public bool Matches(SDLFont font, int textHash, Color color)
        {
            return (textHash == TextHash) && (font == Font) && (color == Color);
        }
    }

    private class IconCache
    {
        public Icons Icon;
        public Color Color;
        public int Width;
        public int Height;
        public IntPtr Handle;

        internal IconCache(Icons icon, Color color, int width, int height, IntPtr handle)
        {
            Icon = icon;
            Color = color;
            Width = width;
            Height = height;
            Handle = handle;
        }
        public bool Matches(Icons icon, Color color)
        {
            return (icon == Icon) && (color == Color);
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
        public IntPtr name;
        public SDL_RendererFlags flags;
        public uint num_texture_formats;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public uint[] texture_formats;
        public int max_texture_width;
        public int max_texture_height;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_Vertex
    {
        public PointF position;
        public int color;
        public PointF tex_coord;
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
    private static extern int SDL_RenderDrawRects(IntPtr renderer, [In] Rectangle[] rects, int count);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int SDL_RenderDrawRectF(IntPtr renderer, [In()] ref RectangleF rect);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int SDL_RenderDrawRectsF(IntPtr renderer, [In] RectangleF[] rects, int count);
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
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int SDL_RenderGeometry(IntPtr renderer, IntPtr texture, [In] SDL_Vertex[] vertices, int num_vertices, [In] int[] indices, int num_indices);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int SDL_RenderGeometry(IntPtr renderer, IntPtr texture, [In] SDL_Vertex[] vertices, int num_vertices, IntPtr indices, int num_indices);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int SDL_RenderSetClipRect(IntPtr renderer, ref Rectangle rect);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int SDL_RenderSetClipRect(IntPtr renderer, IntPtr rect);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int SDL_RenderGetClipRect(IntPtr renderer, out Rectangle rect);


}
