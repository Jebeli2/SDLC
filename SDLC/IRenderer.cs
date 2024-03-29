﻿// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;

using System;
using System.Collections.Generic;
using System.Drawing;

public interface IRenderer
{
    IntPtr Handle { get; }
    bool HandleCreated { get; }
    int Width { get; }
    int Height { get; }
    Color Color { get; set; }
    BlendMode BlendMode { get; set; }
    Rectangle CurrentClip { get; }
    void DrawRect(Rectangle rect);
    void DrawRect(RectangleF rect);
    void DrawRects(IEnumerable<Rectangle> rects);
    void DrawRects(IEnumerable<RectangleF> rects);
    void FillRect(Rectangle rect);
    void FillRect(RectangleF rect);
    void DrawLine(int x1, int y1, int x2, int y2);
    void DrawLine(float x1, float y1, float x2, float y2);
    void DrawLines(IEnumerable<Point> points);
    void DrawLines(IEnumerable<PointF> points);
    void DrawPoint(int x, int y);
    void DrawPoint(float x, float y);
    void FillColorRect(Rectangle rect, Color colorTopLeft, Color colorTopRight, Color colorBottomLeft, Color colorBottomRight);
    void FillColorRect(RectangleF rect, Color colorTopLeft, Color colorTopRight, Color colorBottomLeft, Color colorBottomRight);
    void DrawTexture(SDLTexture? texture, Rectangle src, Rectangle dst);
    void DrawTexture(SDLTexture? texture, Rectangle dst);
    void DrawTexture(SDLTexture? texture);
    void DrawTexture(SDLTexture? texture, Rectangle src, RectangleF dst);
    void DrawTexture(SDLTexture? texture, RectangleF dst);
    void DrawTexture(SDLTexture? texture, Rectangle src, Rectangle dst, double angle, Point center, RendererFlip flip = RendererFlip.None);
    void DrawTexture(SDLTexture? texture, Rectangle src, Rectangle dst, double angle, RendererFlip flip = RendererFlip.None);
    void DrawTexture(SDLTexture? texture, Rectangle dst, double angle, Point center, RendererFlip flip = RendererFlip.None);
    void DrawTexture(SDLTexture? texture, Rectangle dst, double angle, RendererFlip flip = RendererFlip.None);
    void DrawTexture(SDLTexture? texture, RectangleF dst, double angle, RendererFlip flip = RendererFlip.None);

    void DrawTextureRects(SDLTexture? texture, IList<RectangleF> rects, IList<Color> colors);

    void DrawIcon(Icons icon, float x, float y, float width, float height, Color color, HorizontalAlignment hAlign = HorizontalAlignment.Center, VerticalAlignment vAlign = VerticalAlignment.Center, float offsetX = 0, float offsetY = 0);
    void DrawText(SDLFont? font, ReadOnlySpan<char> text, float x, float y, float width, float height, Color color, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center, VerticalAlignment verticalAlignment = VerticalAlignment.Center, float offsetX = 0, float offsetY = 0);
    Size MeasureText(SDLFont? font, ReadOnlySpan<char> text);
    void GetGlyphMetrics(SDLFont? font, char c, out int minx, out int maxx, out int miny, out int maxy, out int advance);
    SDLTexture? LoadTexture(string fileName);
    SDLTexture? LoadTexture(string name, byte[]? data);
    SDLTexture? CreateTexture(string name, int width, int height);
    void ClearScreen();
    void PushTarget(SDLTexture? texture);
    void PopTarget();
    void PushClip(Rectangle clip);
    void PopClip();
    void SetClip(Rectangle clip);
    void ClearClip();
}
