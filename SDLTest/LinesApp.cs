namespace SDLTest
{
    using SDLC;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class LinesApp : SDLApplet
    {
        private class Line
        {
            public int x1;
            public int y1;
            public int x2;
            public int y2;
        }
        private int lastWidth;
        private int lastHeight;
        private int linesPerQuadrant = 80;
        private int lineStep = 10;

        private byte amigaRed = 0;
        private byte amigaGreen = 80;
        private byte amigaBlue = 160;

        private int lineIndex = 0;
        private int lineCount;
        private IList<Line> lines = Array.Empty<Line>();

        private int delay;
        private int delayFrames = 2;
        private bool useAA = false;
        public LinesApp() : base("Lines")
        {

        }

        protected override void OnWindowLoad(SDLWindowLoadEventArgs e)
        {
            Init();
        }

        protected override void OnWindowUpdate(SDLWindowUpdateEventArgs e)
        {
            Init();
            UpdateLines();
        }

        protected override void OnWindowPaint(SDLWindowPaintEventArgs e)
        {
            PaintLines(e.Renderer);
        }

        private void PaintLines(IRenderer gfx)
        {
            int scale = Math.Max(1, lineCount);
            float rs = (255.0f - amigaRed) / scale;
            float gs = (255.0f - amigaGreen) / scale;
            float bs = (255.0f - amigaBlue) / scale;
            int r = amigaRed;
            int g = amigaGreen;
            int b = amigaBlue;
            int lineNum = lineIndex;
            for (int i = 0; i < lineCount && lineNum < lines.Count; ++i)
            {
                Line l = lines[lineNum];
                r += (int)rs;
                g += (int)gs;
                b += (int)bs;
                if (r > 255) { r = amigaRed; }
                if (g > 255) { g = amigaGreen; }
                if (b > 255) { b = amigaBlue; }
                gfx.Color = Color.FromArgb(r, g, b);
                if (useAA)
                {
                    gfx.AALine(l.x1, l.y1, l.x2, l.y2);
                }
                else
                {
                    gfx.DrawLine(l.x1, l.y1, l.x2, l.y2);
                }
                lineNum++;
                lineNum %= lines.Count;
            }
        }
        private void UpdateLines()
        {
            if (lines.Count > 0)
            {
                delay++;
                if (delayFrames < delay)
                {
                    delay = 0;
                    lineIndex++;
                    lineIndex %= lines.Count;
                    lineCount++;
                    if (lineCount > linesPerQuadrant * 2)
                    {
                        lineCount = linesPerQuadrant * 2;
                    }
                }
            }
        }
        private void Init()
        {
            if (lastWidth != Width || lastHeight != Height)
            {
                lastWidth = Width;
                lastHeight = Height;
                lines = MakeLines(linesPerQuadrant, lineStep, 0, 0, Width - 1, Height - 1);
                lineIndex = 0;
            }
        }

        private static IList<Line> MakeLines(int linesPerQuadrant, int lineStep, int left, int top, int right, int bottom)
        {
            Line[] list = new Line[linesPerQuadrant * 4];
            int idx = 0;
            for (int q = 0; q < linesPerQuadrant; ++q)
            {
                list[idx++] = CreateLine(left, lineStep * (linesPerQuadrant - q), lineStep * q, top);
            }
            for (int q = 0; q < linesPerQuadrant; ++q)
            {
                list[idx++] = CreateLine(right - lineStep * (linesPerQuadrant - q), top, right, lineStep * q);
            }
            for (int q = 0; q < linesPerQuadrant; ++q)
            {
                list[idx++] = CreateLine(right, bottom - lineStep * (linesPerQuadrant - q), right - lineStep * q, bottom);
            }
            for (int q = 0; q < linesPerQuadrant; ++q)
            {
                list[idx++] = CreateLine(left + lineStep * (linesPerQuadrant - q), bottom, left, bottom - lineStep * q);
            }
            return list;
        }
        private static Line CreateLine(int x1, int y1, int x2, int y2)
        {
            Line n = new Line { x1 = x1, y1 = y1, x2 = x2, y2 = y2 };
            return n;
        }

    }
}
