// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;

using System;
using System.Drawing;

public static class SDLGfx
{
    public static void ClearScreen(this IRenderer renderer, Color color)
    {
        renderer.Color = color;
        renderer.ClearScreen();
    }

    public static void PushClip(this IRenderer renderer, int x, int y, int width, int height)
    {
        renderer.PushClip(new Rectangle(x, y, width, height));
    }
    public static void SetClip(this IRenderer renderer, int x, int y, int width, int height)
    {
        renderer.SetClip(new Rectangle(x, y, width, height));
    }
    public static void DrawRect(this IRenderer renderer, int x, int y, int width, int height)
    {
        renderer.DrawRect(new Rectangle(x, y, width, height));
    }
    public static void DrawRect(this IRenderer renderer, int x, int y, int width, int height, Color color)
    {
        renderer.Color = color;
        renderer.DrawRect(new Rectangle(x, y, width, height));
    }
    public static void DrawRect(this IRenderer renderer, Rectangle rect, Color color)
    {
        renderer.Color = color;
        renderer.DrawRect(rect);
    }

    public static void DrawRect(this IRenderer renderer, float x, float y, float width, float height)
    {
        renderer.DrawRect(new RectangleF(x, y, width, height));
    }
    public static void DrawRect(this IRenderer renderer, float x, float y, float width, float height, Color color)
    {
        renderer.Color = color;
        renderer.DrawRect(new RectangleF(x, y, width, height));
    }
    public static void DrawRect(this IRenderer renderer, RectangleF rect, Color color)
    {
        renderer.Color = color;
        renderer.DrawRect(rect);
    }

    public static void FillRect(this IRenderer renderer, int x, int y, int width, int height)
    {
        renderer.FillRect(new Rectangle(x, y, width, height));
    }
    public static void FillRect(this IRenderer renderer, int x, int y, int width, int height, Color color)
    {
        renderer.Color = color;
        renderer.FillRect(new Rectangle(x, y, width, height));
    }
    public static void FillRect(this IRenderer renderer, Rectangle rect, Color color)
    {
        renderer.Color = color;
        renderer.FillRect(rect);
    }
    public static void FillRect(this IRenderer renderer, float x, float y, float width, float height)
    {
        renderer.FillRect(new RectangleF(x, y, width, height));
    }
    public static void FillRect(this IRenderer renderer, float x, float y, float width, float height, Color color)
    {
        renderer.Color = color;
        renderer.FillRect(new RectangleF(x, y, width, height));
    }
    public static void FillRect(this IRenderer renderer, RectangleF rect, Color color)
    {
        renderer.Color = color;
        renderer.FillRect(rect);
    }
    public static void FillVertGradient(this IRenderer renderer, int x, int y, int w, int h, Color top, Color bottom)
    {
        FillVertGradient(renderer, new Rectangle(x, y, w, h), top, bottom);
    }
    public static void FillVertGradient(this IRenderer renderer, float x, float y, float w, float h, Color top, Color bottom)
    {
        FillVertGradient(renderer, new RectangleF(x, y, w, h), top, bottom);
    }
    public static void FillVertGradient(this IRenderer renderer, Rectangle rect, Color top, Color bottom)
    {
        renderer.FillColorRect(rect, top, top, bottom, bottom);
    }
    public static void FillVertGradient(this IRenderer renderer, RectangleF rect, Color top, Color bottom)
    {
        renderer.FillColorRect(rect, top, top, bottom, bottom);
    }

    public static void DrawLine(this IRenderer renderer, Point p1, Point p2)
    {
        renderer.DrawLine(p1.X, p1.Y, p2.X, p2.Y);
    }
    public static void DrawLine(this IRenderer renderer, Point p1, Point p2, Color color)
    {
        renderer.Color = color;
        renderer.DrawLine(p1.X, p1.Y, p2.X, p2.Y);
    }
    public static void DrawLine(this IRenderer renderer, int x1, int y1, int x2, int y2, Color color)
    {
        renderer.Color = color;
        renderer.DrawLine(x1, y1, x2, y2);
    }
    public static void DrawLine(this IRenderer renderer, PointF p1, PointF p2)
    {
        renderer.DrawLine(p1.X, p1.Y, p2.X, p2.Y);
    }
    public static void DrawLine(this IRenderer renderer, PointF p1, PointF p2, Color color)
    {
        renderer.Color = color;
        renderer.DrawLine(p1.X, p1.Y, p2.X, p2.Y);
    }
    public static void DrawLine(this IRenderer renderer, float x1, float y1, float x2, float y2, Color color)
    {
        renderer.Color = color;
        renderer.DrawLine(x1, y1, x2, y2);
    }

    public static void DrawPoint(this IRenderer renderer, Point p)
    {
        renderer.DrawPoint(p.X, p.Y);
    }
    public static void DrawPoint(this IRenderer renderer, Point p, Color color)
    {
        renderer.Color = color;
        renderer.DrawPoint(p.X, p.Y);
    }
    public static void DrawPoint(this IRenderer renderer, int x, int y, Color color)
    {
        renderer.Color = color;
        renderer.DrawPoint(x, y);
    }
    public static void DrawPoint(this IRenderer renderer, PointF p)
    {
        renderer.DrawPoint(p.X, p.Y);
    }
    public static void DrawPoint(this IRenderer renderer, PointF p, Color color)
    {
        renderer.Color = color;
        renderer.DrawPoint(p.X, p.Y);
    }
    public static void DrawPoint(this IRenderer renderer, float x, float y, Color color)
    {
        renderer.Color = color;
        renderer.DrawPoint(x, y);
    }

    public static void DrawTexture(this IRenderer renderer, SDLTexture? texture, int x, int y, int width, int height, double angle, RendererFlip flip = RendererFlip.None)
    {
        renderer.DrawTexture(texture, new Rectangle(x, y, width, height), angle, flip);
    }

    public static void DrawTexture(this IRenderer renderer, SDLTexture? texture, Rectangle src, Rectangle dst, byte alpha)
    {
        if (texture != null)
        {
            texture.AlphaMod = alpha;
            texture.BlendMode = BlendMode.Blend;
            renderer.BlendMode = BlendMode.Blend;
            renderer.DrawTexture(texture, src, dst);
        }
    }

    public static void DrawText(this IRenderer renderer, SDLFont? font, string? text, float x, float y, Color color)
    {
        renderer.DrawText(font, text, x, y, 0, 0, color, HorizontalAlignment.Left, VerticalAlignment.Top);
    }
    public static void DrawText(this IRenderer renderer, SDLFont? font, string? text, float x, float y)
    {
        renderer.DrawText(font, text, x, y, 0, 0, renderer.Color, HorizontalAlignment.Left, VerticalAlignment.Top);
    }

    // from sdl2_gfx

    private static void Box(this IRenderer renderer, int x1, int y1, int x2, int y2)
    {
        int tmp;
        //SDL_Rect rect;

        /*
        * Test for special cases of straight lines or single point 
        */
        if (x1 == x2)
        {
            if (y1 == y2)
            {
                renderer.DrawPoint(x1, y1);
            }
            else
            {
                VLine(renderer, x1, y1, y2);
            }
        }
        else
        {
            if (y1 == y2)
            {
                HLine(renderer, x1, x2, y1);
            }
        }

        /*
        * Swap x1, x2 if required 
        */
        if (x1 > x2)
        {
            tmp = x1;
            x1 = x2;
            x2 = tmp;
        }

        /*
        * Swap y1, y2 if required 
        */
        if (y1 > y2)
        {
            tmp = y1;
            y1 = y2;
            y2 = tmp;
        }

        ///* 
        //* Create destination rect
        //*/
        //rect.x = x1;
        //rect.y = y1;
        //rect.w = x2 - x1 + 1;
        //rect.h = y2 - y1 + 1;

        /*
        * Draw
        */
        renderer.FillRect(x1, y1, x2 - x1 + 1, y2 - y1 + 1);

    }
    public static void HLine(this IRenderer renderer, int x1, int x2, int y)
    {
        renderer.DrawLine(x1, y, x2, y);
    }
    public static void HLine(this IRenderer renderer, int x1, int x2, int y, Color color)
    {
        renderer.Color = color;
        renderer.DrawLine(x1, y, x2, y);
    }
    public static void VLine(this IRenderer renderer, int x, int y1, int y2)
    {
        renderer.DrawLine(x, y1, x, y2);
    }
    public static void VLine(this IRenderer renderer, int x, int y1, int y2, Color color)
    {
        renderer.Color = color;
        renderer.DrawLine(x, y1, x, y2);
    }
    public static void RoundedRectangle(this IRenderer renderer, Rectangle rect, int rad, Color color)
    {
        renderer.Color = color;
        RoundedRectangle(renderer, rect.X, rect.Y, rect.Right, rect.Bottom, rad);
    }
    public static void RoundedRectangle(this IRenderer renderer, int x1, int y1, int x2, int y2, int rad)
    {
        int tmp;
        int w, h;
        int xx1, xx2;
        int yy1, yy2;

        if (rad < 0) { return; }

        /*
        * Special case - no rounding
        */
        if (rad <= 1) { Box(renderer, x1, y1, x2, y2); }

        /*
        * Test for special cases of straight lines or single point 
        */
        if (x1 == x2)
        {
            if (y1 == y2)
            {
                renderer.DrawPoint(x1, y1);
            }
            else
            {
                VLine(renderer, x1, y1, y2);
            }
        }
        else
        {
            if (y1 == y2)
            {
                HLine(renderer, x1, x2, y1);
            }
        }

        /*
        * Swap x1, x2 if required 
        */
        if (x1 > x2)
        {
            tmp = x1;
            x1 = x2;
            x2 = tmp;
        }

        /*
        * Swap y1, y2 if required 
        */
        if (y1 > y2)
        {
            tmp = y1;
            y1 = y2;
            y2 = tmp;
        }

        /*
        * Calculate width&height 
        */
        w = x2 - x1;
        h = y2 - y1;

        /*
        * Maybe adjust radius
        */
        if ((rad * 2) > w)
        {
            rad = w / 2;
        }
        if ((rad * 2) > h)
        {
            rad = h / 2;
        }

        /*
        * Draw corners
        */
        xx1 = x1 + rad;
        xx2 = x2 - rad;
        yy1 = y1 + rad;
        yy2 = y2 - rad;
        Arc(renderer, xx1, yy1, rad, 180, 270);
        Arc(renderer, xx2, yy1, rad, 270, 360);
        Arc(renderer, xx1, yy2, rad, 90, 180);
        Arc(renderer, xx2, yy2, rad, 0, 90);

        /*
        * Draw lines
        */
        if (xx1 <= xx2)
        {
            HLine(renderer, xx1, xx2, y1);
            HLine(renderer, xx1, xx2, y2);
        }
        if (yy1 <= yy2)
        {
            VLine(renderer, x1, yy1, yy2);
            VLine(renderer, x2, yy1, yy2);
        }
    }

    public static void RoundedRectangle(this IRenderer renderer, int x1, int y1, int x2, int y2, int rad, Color color)
    {
        renderer.Color = color;
        RoundedRectangle(renderer, x1, y1, x2, y2, rad);
    }

    public static void FillRoundedRectangle(this IRenderer renderer, int x1, int y1, int x2, int y2, int rad)
    {
        int w, h, r2, tmp;
        int cx = 0;
        int cy = rad;
        int ocx = 0xffff;
        int ocy = 0xffff;
        int df = 1 - rad;
        int d_e = 3;
        int d_se = -2 * rad + 5;
        int xpcx, xmcx, xpcy, xmcy;
        int ypcy, ymcy, ypcx, ymcx;
        int x, y, dx, dy;

        /*
        * Check radius vor valid range
        */
        if (rad < 0) { return; }

        /*
        * Special case - no rounding
        */
        if (rad <= 1)
        {
            renderer.Box(x1, y1, x2, y2);
        }

        /*
        * Test for special cases of straight lines or single point 
        */
        if (x1 == x2)
        {
            if (y1 == y2)
            {
                renderer.DrawPoint(x1, y1);
            }
            else
            {
                VLine(renderer, x1, y1, y2);
            }
        }
        else
        {
            if (y1 == y2)
            {
                HLine(renderer, x1, x2, y1);
            }
        }

        /*
        * Swap x1, x2 if required 
        */
        if (x1 > x2)
        {
            tmp = x1;
            x1 = x2;
            x2 = tmp;
        }

        /*
        * Swap y1, y2 if required 
        */
        if (y1 > y2)
        {
            tmp = y1;
            y1 = y2;
            y2 = tmp;
        }

        /*
        * Calculate width&height 
        */
        w = x2 - x1 + 1;
        h = y2 - y1 + 1;

        /*
        * Maybe adjust radius
        */
        r2 = rad + rad;
        if (r2 > w)
        {
            rad = w / 2;
            r2 = rad + rad;
        }
        if (r2 > h)
        {
            rad = h / 2;
        }

        /* Setup filled circle drawing for corners */
        x = x1 + rad;
        y = y1 + rad;
        dx = x2 - x1 - rad - rad;
        dy = y2 - y1 - rad - rad;

        /*
        * Draw corners
        */
        do
        {
            xpcx = x + cx;
            xmcx = x - cx;
            xpcy = x + cy;
            xmcy = x - cy;
            if (ocy != cy)
            {
                if (cy > 0)
                {
                    ypcy = y + cy;
                    ymcy = y - cy;
                    HLine(renderer, xmcx, xpcx + dx, ypcy + dy);
                    HLine(renderer, xmcx, xpcx + dx, ymcy);
                }
                else
                {
                    HLine(renderer, xmcx, xpcx + dx, y);
                }
                ocy = cy;
            }
            if (ocx != cx)
            {
                if (cx != cy)
                {
                    if (cx > 0)
                    {
                        ypcx = y + cx;
                        ymcx = y - cx;
                        HLine(renderer, xmcy, xpcy + dx, ymcx);
                        HLine(renderer, xmcy, xpcy + dx, ypcx + dy);
                    }
                    else
                    {
                        HLine(renderer, xmcy, xpcy + dx, y);
                    }
                }
                ocx = cx;
            }

            /*
            * Update 
            */
            if (df < 0)
            {
                df += d_e;
                d_e += 2;
                d_se += 2;
            }
            else
            {
                df += d_se;
                d_e += 2;
                d_se += 4;
                cy--;
            }
            cx++;
        } while (cx <= cy);

        /* Inside */
        if (dx > 0 && dy > 0)
        {
            Box(renderer, x1, y1 + rad + 1, x2, y2 - rad);
        }


    }
    public static void FillRoundedRectangle(this IRenderer renderer, int x1, int y1, int x2, int y2, int rad, Color color)
    {
        renderer.Color = color;
        FillRoundedRectangle(renderer, x1, y1, x2, y2, rad);
    }

    public static void Arc(this IRenderer renderer, int x, int y, int rad, int start, int end)
    {
        int cx = 0;
        int cy = rad;
        int df = 1 - rad;
        int d_e = 3;
        int d_se = -2 * rad + 5;
        int xpcx, xmcx, xpcy, xmcy;
        int ypcy, ymcy, ypcx, ymcx;
        int drawoct;
        int startoct, endoct, oct, stopval_start = 0, stopval_end = 0;
        double dstart, dend, temp = 0.0;

        /*
        * Sanity check radius 
        */
        if (rad < 0) { return; }

        /*
        * Special case for rad=0 - draw a point 
        */
        if (rad == 0) { renderer.DrawPoint(x, y); }

        /*
         Octant labeling

          \ 5 | 6 /
           \  |  /
          4 \ | / 7
             \|/
        ------+------ +x
             /|\
          3 / | \ 0
           /  |  \
          / 2 | 1 \
              +y

         Initially reset bitmask to 0x00000000
         the set whether or not to keep drawing a given octant.
         For example: 0x00111100 means we're drawing in octants 2-5
        */
        drawoct = 0;

        /*
        * Fixup angles
        */
        start %= 360;
        end %= 360;
        /* 0 <= start & end < 360; note that sometimes start > end - if so, arc goes back through 0. */
        while (start < 0) start += 360;
        while (end < 0) end += 360;
        start %= 360;
        end %= 360;

        /* now, we find which octants we're drawing in. */
        startoct = start / 45;
        endoct = end / 45;
        oct = startoct - 1;

        /* stopval_start, stopval_end; what values of cx to stop at. */
        do
        {
            oct = (oct + 1) % 8;

            if (oct == startoct)
            {
                /* need to compute stopval_start for this octant.  Look at picture above if this is unclear */
                dstart = (double)start;
                switch (oct)
                {
                    case 0:
                    case 3:
                        temp = Math.Sin(dstart * Math.PI / 180.0);
                        break;
                    case 1:
                    case 6:
                        temp = Math.Cos(dstart * Math.PI / 180.0);
                        break;
                    case 2:
                    case 5:
                        temp = -Math.Cos(dstart * Math.PI / 180.0);
                        break;
                    case 4:
                    case 7:
                        temp = -Math.Sin(dstart * Math.PI / 180.0);
                        break;
                }
                temp *= rad;
                stopval_start = (int)temp;

                /* 
                This isn't arbitrary, but requires graph paper to explain well.
                The basic idea is that we're always changing drawoct after we draw, so we
                stop immediately after we render the last sensible pixel at x = ((int)temp).
                and whether to draw in this octant initially
                */
                if ((oct % 2) != 0) drawoct |= (1 << oct);         /* this is basically like saying drawoct[oct] = true, if drawoct were a bool array */
                else drawoct &= 255 - (1 << oct);   /* this is basically like saying drawoct[oct] = false */
            }
            if (oct == endoct)
            {
                /* need to compute stopval_end for this octant */
                dend = (double)end;
                switch (oct)
                {
                    case 0:
                    case 3:
                        temp = Math.Sin(dend * Math.PI / 180);
                        break;
                    case 1:
                    case 6:
                        temp = Math.Cos(dend * Math.PI / 180);
                        break;
                    case 2:
                    case 5:
                        temp = -Math.Cos(dend * Math.PI / 180);
                        break;
                    case 4:
                    case 7:
                        temp = -Math.Sin(dend * Math.PI / 180);
                        break;
                }
                temp *= rad;
                stopval_end = (int)temp;

                /* and whether to draw in this octant initially */
                if (startoct == endoct)
                {
                    /* note:      we start drawing, stop, then start again in this case */
                    /* otherwise: we only draw in this octant, so initialize it to false, it will get set back to true */
                    if (start > end)
                    {
                        /* unfortunately, if we're in the same octant and need to draw over the whole circle, */
                        /* we need to set the rest to true, because the while loop will end at the bottom. */
                        drawoct = 255;
                    }
                    else
                    {
                        drawoct &= 255 - (1 << oct);
                    }
                }
                else if ((oct % 2) != 0) drawoct &= 255 - (1 << oct);
                else drawoct |= (1 << oct);
            }
            else if (oct != startoct)
            { /* already verified that it's != endoct */
                drawoct |= (1 << oct); /* draw this entire segment */
            }
        } while (oct != endoct);

        /* so now we have what octants to draw and when to draw them. all that's left is the actual raster code. */

        /*
        * Draw arc 
        */
        do
        {
            ypcy = y + cy;
            ymcy = y - cy;
            if (cx > 0)
            {
                xpcx = x + cx;
                xmcx = x - cx;

                /* always check if we're drawing a certain octant before adding a pixel to that octant. */
                if ((drawoct & 4) != 0) renderer.DrawPoint(xmcx, ypcy);
                if ((drawoct & 2) != 0) renderer.DrawPoint(xpcx, ypcy);
                if ((drawoct & 32) != 0) renderer.DrawPoint(xmcx, ymcy);
                if ((drawoct & 64) != 0) renderer.DrawPoint(xpcx, ymcy);
            }
            else
            {
                if ((drawoct & 96) != 0) renderer.DrawPoint(x, ymcy);
                if ((drawoct & 6) != 0) renderer.DrawPoint(x, ypcy);
            }

            xpcy = x + cy;
            xmcy = x - cy;
            if (cx > 0 && cx != cy)
            {
                ypcx = y + cx;
                ymcx = y - cx;
                if ((drawoct & 8) != 0) renderer.DrawPoint(xmcy, ypcx);
                if ((drawoct & 1) != 0) renderer.DrawPoint(xpcy, ypcx);
                if ((drawoct & 16) != 0) renderer.DrawPoint(xmcy, ymcx);
                if ((drawoct & 128) != 0) renderer.DrawPoint(xpcy, ymcx);
            }
            else if (cx == 0)
            {
                if ((drawoct & 24) != 0) renderer.DrawPoint(xmcy, y);
                if ((drawoct & 129) != 0) renderer.DrawPoint(xpcy, y);
            }

            /*
            * Update whether we're drawing an octant
            */
            if (stopval_start == cx)
            {
                /* works like an on-off switch. */
                /* This is just in case start & end are in the same octant. */
                if ((drawoct & (1 << startoct)) != 0) drawoct &= 255 - (1 << startoct);
                else drawoct |= (1 << startoct);
            }
            if (stopval_end == cx)
            {
                if ((drawoct & (1 << endoct)) != 0) drawoct &= 255 - (1 << endoct);
                else drawoct |= (1 << endoct);
            }

            /*
            * Update pixels
            */
            if (df < 0)
            {
                df += d_e;
                d_e += 2;
                d_se += 2;
            }
            else
            {
                df += d_se;
                d_e += 2;
                d_se += 4;
                cy--;
            }
            cx++;
        } while (cx <= cy);
    }

    public static void Arc(this IRenderer renderer, int x, int y, int rad, int start, int end, Color color)
    {
        renderer.Color = color;
        Arc(renderer, x, y, rad, start, end);
    }
    private static void DrawQuadrants(this IRenderer renderer, int x, int y, int dx, int dy, bool f)
    {
        int xpdx, xmdx;
        int ypdy, ymdy;

        if (dx == 0)
        {
            if (dy == 0)
            {
                renderer.DrawPoint(x, y);
            }
            else
            {
                ypdy = y + dy;
                ymdy = y - dy;
                if (f)
                {
                    VLine(renderer, x, ymdy, ypdy);
                }
                else
                {
                    renderer.DrawPoint(x, ypdy);
                    renderer.DrawPoint(x, ymdy);
                }
            }
        }
        else
        {
            xpdx = x + dx;
            xmdx = x - dx;
            ypdy = y + dy;
            ymdy = y - dy;
            if (f)
            {
                VLine(renderer, xpdx, ymdy, ypdy);
                VLine(renderer, xmdx, ymdy, ypdy);
            }
            else
            {
                renderer.DrawPoint(xpdx, ypdy);
                renderer.DrawPoint(xmdx, ypdy);
                renderer.DrawPoint(xpdx, ymdy);
                renderer.DrawPoint(xmdx, ymdy);
            }
        }

    }

    private const int DEFAULT_ELLIPSE_OVERSCAN = 4;
    public static void Ellipse(this IRenderer renderer, int x, int y, int rx, int ry, bool f = false)
    {
        int rxi, ryi;
        int rx2, ry2, rx22, ry22;
        int error;
        int curX, curY, curXp1, curYm1;
        int scrX, scrY, oldX, oldY;
        int deltaX, deltaY;
        int ellipseOverscan;

        /*
        * Sanity check radii 
        */
        if ((rx < 0) || (ry < 0)) { return; }

        /*
        * Special cases for rx=0 and/or ry=0: draw a hline/vline/pixel 
        */
        if (rx == 0)
        {
            if (ry == 0)
            {
                renderer.DrawPoint(x, y);
            }
            else
            {
                VLine(renderer, x, y - ry, y + ry);
            }
        }
        else
        {
            if (ry == 0)
            {
                HLine(renderer, x - rx, x + rx, y);
            }
        }

        /*
         * Adjust overscan 
         */
        rxi = rx;
        ryi = ry;
        if (rxi >= 512 || ryi >= 512)
        {
            ellipseOverscan = DEFAULT_ELLIPSE_OVERSCAN / 4;
        }
        else if (rxi >= 256 || ryi >= 256)
        {
            ellipseOverscan = DEFAULT_ELLIPSE_OVERSCAN / 2;
        }
        else
        {
            ellipseOverscan = DEFAULT_ELLIPSE_OVERSCAN / 1;
        }

        /*
         * Top/bottom center points.
         */
        oldX = scrX = 0;
        oldY = scrY = ryi;
        DrawQuadrants(renderer, x, y, 0, ry, f);

        /* Midpoint ellipse algorithm with overdraw */
        rxi *= ellipseOverscan;
        ryi *= ellipseOverscan;
        rx2 = rxi * rxi;
        rx22 = rx2 + rx2;
        ry2 = ryi * ryi;
        ry22 = ry2 + ry2;
        curX = 0;
        curY = ryi;
        deltaX = 0;
        deltaY = rx22 * curY;

        /* Points in segment 1 */
        error = ry2 - rx2 * ryi + rx2 / 4;
        while (deltaX <= deltaY)
        {
            curX++;
            deltaX += ry22;

            error += deltaX + ry2;
            if (error >= 0)
            {
                curY--;
                deltaY -= rx22;
                error -= deltaY;
            }

            scrX = curX / ellipseOverscan;
            scrY = curY / ellipseOverscan;
            if ((scrX != oldX && scrY == oldY) || (scrX != oldX && scrY != oldY))
            {
                DrawQuadrants(renderer, x, y, scrX, scrY, f);
                oldX = scrX;
                oldY = scrY;
            }
        }

        /* Points in segment 2 */
        if (curY > 0)
        {
            curXp1 = curX + 1;
            curYm1 = curY - 1;
            error = ry2 * curX * curXp1 + ((ry2 + 3) / 4) + rx2 * curYm1 * curYm1 - rx2 * ry2;
            while (curY > 0)
            {
                curY--;
                deltaY -= rx22;

                error += rx2;
                error -= deltaY;

                if (error <= 0)
                {
                    curX++;
                    deltaX += ry22;
                    error += deltaX;
                }

                scrX = curX / ellipseOverscan;
                scrY = curY / ellipseOverscan;
                if ((scrX != oldX && scrY == oldY) || (scrX != oldX && scrY != oldY))
                {
                    oldY--;
                    for (; oldY >= scrY; oldY--)
                    {
                        DrawQuadrants(renderer, x, y, scrX, oldY, f);
                        /* prevent overdraw */
                        if (f)
                        {
                            oldY = scrY - 1;
                        }
                    }
                    oldX = scrX;
                    oldY = scrY;
                }
            }

            /* Remaining points in vertical */
            if (!f)
            {
                oldY--;
                for (; oldY >= 0; oldY--)
                {
                    DrawQuadrants(renderer, x, y, scrX, oldY, f);
                }
            }
        }
    }
    public static void FillEllipse(this IRenderer renderer, int x, int y, int rx, int ry)
    {
        Ellipse(renderer, x, y, rx, ry, true);
    }
    public static void Ellipse(this IRenderer renderer, int x, int y, int rx, int ry, Color color)
    {
        renderer.Color = color;
        Ellipse(renderer, x, y, rx, ry);
    }
    public static void FillEllipse(this IRenderer renderer, int x, int y, int rx, int ry, Color color)
    {
        renderer.Color = color;
        Ellipse(renderer, x, y, rx, ry, true);
    }

    public static void Circle(this IRenderer renderer, int x, int y, int r)
    {
        Ellipse(renderer, x, y, r, r);
    }
    public static void Circle(this IRenderer renderer, int x, int y, int r, Color color)
    {
        renderer.Color = color;
        Ellipse(renderer, x, y, r, r);
    }
    public static void FillCircle(this IRenderer renderer, int x, int y, int r)
    {
        FillEllipse(renderer, x, y, r, r);
    }
    public static void FillCircle(this IRenderer renderer, int x, int y, int r, Color color)
    {
        renderer.Color = color;
        FillEllipse(renderer, x, y, r, r);
    }

    private static long lrint(double d)
    {
        return (long)Math.Round(d, MidpointRounding.AwayFromZero);
    }
    public static void AAEllipse(this IRenderer renderer, int x, int y, int rx, int ry)
    {
        BlendMode oldBlendMode = renderer.BlendMode;
        Color oldColor = renderer.Color;
        try
        {

            int i;
            int a2, b2, ds, dt, dxt, t, s, d;
            int xp, yp, xs, ys, dyt, od, xx, yy, xc2, yc2;
            float cp;
            double sab;
            byte weight, iweight;
            byte r = oldColor.R;
            byte g = oldColor.G;
            byte b = oldColor.B;
            byte a = oldColor.A;

            /*
            * Sanity check radii 
            */
            if ((rx < 0) || (ry < 0)) { return; }

            /*
            * Special cases for rx=0 and/or ry=0: draw a hline/vline/pixel 
            */
            if (rx == 0)
            {
                if (ry == 0)
                {
                    renderer.DrawPoint(x, y);
                }
                else
                {
                    VLine(renderer, x, y - ry, y + ry);
                }
            }
            else
            {
                if (ry == 0)
                {
                    HLine(renderer, x - rx, x + rx, y);
                }
            }

            /* Variable setup */
            a2 = rx * rx;
            b2 = ry * ry;

            ds = 2 * a2;
            dt = 2 * b2;

            xc2 = 2 * x;
            yc2 = 2 * y;

            sab = Math.Sqrt((double)(a2 + b2));
            od = (int)lrint(sab * 0.01) + 1; /* introduce some overdraw */
            dxt = (int)lrint((double)a2 / sab) + od;

            t = 0;
            s = -2 * a2 * ry;
            d = 0;

            xp = x;
            yp = y - ry;

            /* Draw */
            //result = 0;
            //result |= SDL_SetRenderDrawBlendMode(renderer, (a == 255) ? SDL_BLENDMODE_NONE : SDL_BLENDMODE_BLEND);

            /* "End points" */
            renderer.DrawPoint(xp, yp);
            renderer.DrawPoint(xc2 - xp, yp);
            renderer.DrawPoint(xp, yc2 - yp);
            renderer.DrawPoint(xc2 - xp, yc2 - yp);
            for (i = 1; i <= dxt; i++)
            {
                xp--;
                d += t - b2;

                if (d >= 0)
                    ys = yp - 1;
                else if ((d - s - a2) > 0)
                {
                    if ((2 * d - s - a2) >= 0)
                        ys = yp + 1;
                    else
                    {
                        ys = yp;
                        yp++;
                        d -= s + a2;
                        s += ds;
                    }
                }
                else
                {
                    yp++;
                    ys = yp + 1;
                    d -= s + a2;
                    s += ds;
                }

                t -= dt;

                /* Calculate alpha */
                if (s != 0)
                {
                    cp = (float)Math.Abs(d) / (float)Math.Abs(s);
                    if (cp > 1.0)
                    {
                        cp = 1.0f;
                    }
                }
                else
                {
                    cp = 1.0f;
                }

                /* Calculate weights */
                weight = (byte)(cp * 255);
                iweight = (byte)(255 - weight);

                /* Upper half */
                xx = xc2 - xp;
                PixelRGBAWeight(renderer, xp, yp, r, g, b, a, iweight);
                PixelRGBAWeight(renderer, xx, yp, r, g, b, a, iweight);

                PixelRGBAWeight(renderer, xp, ys, r, g, b, a, weight);
                PixelRGBAWeight(renderer, xx, ys, r, g, b, a, weight);

                /* Lower half */
                yy = yc2 - yp;
                PixelRGBAWeight(renderer, xp, yy, r, g, b, a, iweight);
                PixelRGBAWeight(renderer, xx, yy, r, g, b, a, iweight);

                yy = yc2 - ys;
                PixelRGBAWeight(renderer, xp, yy, r, g, b, a, weight);
                PixelRGBAWeight(renderer, xx, yy, r, g, b, a, weight);
            }

            /* Replaces original approximation code dyt = abs(yp - yc); */
            dyt = (int)lrint((double)b2 / sab) + od;

            for (i = 1; i <= dyt; i++)
            {
                yp++;
                d -= s + a2;

                if (d <= 0)
                    xs = xp + 1;
                else if ((d + t - b2) < 0)
                {
                    if ((2 * d + t - b2) <= 0)
                        xs = xp - 1;
                    else
                    {
                        xs = xp;
                        xp--;
                        d += t - b2;
                        t -= dt;
                    }
                }
                else
                {
                    xp--;
                    xs = xp - 1;
                    d += t - b2;
                    t -= dt;
                }

                s += ds;

                /* Calculate alpha */
                if (t != 0)
                {
                    cp = (float)Math.Abs(d) / (float)Math.Abs(t);
                    if (cp > 1.0)
                    {
                        cp = 1.0f;
                    }
                }
                else
                {
                    cp = 1.0f;
                }

                /* Calculate weight */
                weight = (byte)(cp * 255);
                iweight = (byte)(255 - weight);

                /* Left half */
                xx = xc2 - xp;
                yy = yc2 - yp;
                PixelRGBAWeight(renderer, xp, yp, r, g, b, a, iweight);
                PixelRGBAWeight(renderer, xx, yp, r, g, b, a, iweight);

                PixelRGBAWeight(renderer, xp, yy, r, g, b, a, iweight);
                PixelRGBAWeight(renderer, xx, yy, r, g, b, a, iweight);

                /* Right half */
                xx = xc2 - xs;
                PixelRGBAWeight(renderer, xs, yp, r, g, b, a, weight);
                PixelRGBAWeight(renderer, xx, yp, r, g, b, a, weight);

                PixelRGBAWeight(renderer, xs, yy, r, g, b, a, weight);
                PixelRGBAWeight(renderer, xx, yy, r, g, b, a, weight);
            }
        }
        finally
        {
            renderer.BlendMode = oldBlendMode;
            renderer.Color = oldColor;
        }
    }
    public static void AAEllipse(this IRenderer renderer, int x, int y, int rx, int ry, Color color)
    {
        renderer.Color = color;
        AAEllipse(renderer, x, y, rx, ry);
    }
    public static void AACircle(this IRenderer renderer, int x, int y, int r)
    {
        AAEllipse(renderer, x, y, r, r);
    }

    public static void AACircle(this IRenderer renderer, int x, int y, int r, Color color)
    {
        renderer.Color = color;
        AAEllipse(renderer, x, y, r, r);
    }
    private const int AAlevels = 256;
    private const int AAbits = 8;

    public static void AALine(this IRenderer renderer, int x1, int y1, int x2, int y2, bool draw_endpoint = true)
    {
        BlendMode oldBlendMode = renderer.BlendMode;
        Color oldColor = renderer.Color;
        try
        {
            int xx0, yy0, xx1, yy1;
            int intshift, erracc, erradj;
            int erracctmp, wgt;
            int dx, dy, tmp, xdir, y0p1, x0pxdir;

            /*
            * Keep on working with 32bit numbers 
            */
            xx0 = x1;
            yy0 = y1;
            xx1 = x2;
            yy1 = y2;

            /*
            * Reorder points to make dy positive 
            */
            if (yy0 > yy1)
            {
                tmp = yy0;
                yy0 = yy1;
                yy1 = tmp;
                tmp = xx0;
                xx0 = xx1;
                xx1 = tmp;
            }

            /*
            * Calculate distance 
            */
            dx = xx1 - xx0;
            dy = yy1 - yy0;

            /*
            * Adjust for negative dx and set xdir 
            */
            if (dx >= 0)
            {
                xdir = 1;
            }
            else
            {
                xdir = -1;
                dx = (-dx);
            }

            /*
            * Check for special cases 
            */
            if (dx == 0)
            {
                /*
                * Vertical line 
                */
                if (draw_endpoint)
                {
                    VLine(renderer, x1, y1, y2);
                }
                else
                {
                    if (dy > 0)
                    {
                        VLine(renderer, x1, yy0, yy0 + dy);
                    }
                    else
                    {
                        renderer.DrawPoint(x1, y1);
                    }
                }
            }
            else if (dy == 0)
            {
                /*
                * Horizontal line 
                */
                if (draw_endpoint)
                {
                    HLine(renderer, x1, x2, y1);
                }
                else
                {
                    if (dx > 0)
                    {
                        HLine(renderer, xx0, xx0 + (xdir * dx), y1);
                    }
                    else
                    {
                        renderer.DrawPoint(x1, y1);
                    }
                }
            }
            else if ((dx == dy) && (draw_endpoint))
            {
                /*
                * Diagonal line (with endpoint)
                */
                renderer.DrawLine(x1, y1, x2, y2);
            }

            /*
            * Zero accumulator 
            */
            erracc = 0;

            /*
            * # of bits by which to shift erracc to get intensity level 
            */
            intshift = 32 - AAbits;

            /*
            * Mask used to flip all bits in an intensity weighting 
            */
            //wgtcompmask = AAlevels - 1;

            /*
            * Draw the initial pixel in the foreground color 
            */
            renderer.DrawPoint(x1, y1);

            byte r = oldColor.R;
            byte g = oldColor.G;
            byte b = oldColor.B;
            byte a = oldColor.A;
            /*
            * x-major or y-major? 
            */
            if (dy > dx)
            {

                /*
                * y-major.  Calculate 16-bit fixed point fractional part of a pixel that
                * X advances every time Y advances 1 pixel, truncating the result so that
                * we won't overrun the endpoint along the X axis 
                */
                /*
                * Not-so-portable version: erradj = ((Uint64)dx << 32) / (Uint64)dy; 
                */
                erradj = ((dx << 16) / dy) << 16;

                /*
                * draw all pixels other than the first and last 
                */
                x0pxdir = xx0 + xdir;
                while (--dy != 0)
                {
                    erracctmp = erracc;
                    erracc += erradj;
                    if (erracc <= erracctmp)
                    {
                        /*
                        * rollover in error accumulator, x coord advances 
                        */
                        xx0 = x0pxdir;
                        x0pxdir += xdir;
                    }
                    yy0++;      /* y-major so always advance Y */

                    /*
                    * the AAbits most significant bits of erracc give us the intensity
                    * weighting for this pixel, and the complement of the weighting for
                    * the paired pixel. 
                    */
                    wgt = (erracc >> intshift) & 255;
                    PixelRGBAWeight(renderer, xx0, yy0, r, g, b, a, 255 - wgt);
                    PixelRGBAWeight(renderer, x0pxdir, yy0, r, g, b, a, wgt);
                }

            }
            else
            {

                /*
                * x-major line.  Calculate 16-bit fixed-point fractional part of a pixel
                * that Y advances each time X advances 1 pixel, truncating the result so
                * that we won't overrun the endpoint along the X axis. 
                */
                /*
                * Not-so-portable version: erradj = ((Uint64)dy << 32) / (Uint64)dx; 
                */
                erradj = ((dy << 16) / dx) << 16;

                /*
                * draw all pixels other than the first and last 
                */
                y0p1 = yy0 + 1;
                while (--dx != 0)
                {

                    erracctmp = erracc;
                    erracc += erradj;
                    if (erracc <= erracctmp)
                    {
                        /*
                        * Accumulator turned over, advance y 
                        */
                        yy0 = y0p1;
                        y0p1++;
                    }
                    xx0 += xdir;    /* x-major so always advance X */
                    /*
                    * the AAbits most significant bits of erracc give us the intensity
                    * weighting for this pixel, and the complement of the weighting for
                    * the paired pixel. 
                    */
                    wgt = (erracc >> intshift) & 255;
                    PixelRGBAWeight(renderer, xx0, yy0, r, g, b, a, 255 - wgt);
                    PixelRGBAWeight(renderer, xx0, y0p1, r, g, b, a, wgt);
                }
            }

            /*
            * Do we have to draw the endpoint 
            */
            if (draw_endpoint)
            {
                /*
                * Draw final pixel, always exactly intersected by the line and doesn't
                * need to be weighted. 
                */
                renderer.DrawPoint(x2, y2, oldColor);
            }
        }
        finally
        {
            renderer.BlendMode = oldBlendMode;
            renderer.Color = oldColor;
        }

    }
    public static void AALine(this IRenderer renderer, int x1, int y1, int x2, int y2, Color color)
    {
        renderer.Color = color;
        AALine(renderer, x1, y1, x2, y2, true);
    }
    private static void PixelRGBAWeight(this IRenderer renderer, int x, int y, byte r, byte g, byte b, byte a, int weight)
    {
        int ax = a;
        ax = ((ax * weight) >> 8);
        if (ax > 255)
        {
            a = 255;
            renderer.BlendMode = BlendMode.None;
        }
        else
        {
            a = (byte)(ax & 0xFF);
            renderer.BlendMode = BlendMode.Blend;
        }
        renderer.Color = Color.FromArgb(a, r, g, b);
        renderer.DrawPoint(x, y);
    }

}
