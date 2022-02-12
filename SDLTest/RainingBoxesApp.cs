namespace SDLTest
{
    using SDLC;
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// C# Port of SDL2 - Lesson 08 by Maxwell Slater, found here: "https://thenumbat.github.io/cpp-course/sdl2/08/08.html".
    /// </summary>
    internal class RainingBoxesApp : SDLApplet
    {
        private static readonly Random random = new();
        private const float GRAVITY = 750.0f;
        private readonly List<Square> squares = new();
        private bool leftMouseDown;
        private int addX = -1;
        private int addY = -1;
        private SDLTexture? box;
        public RainingBoxesApp() : base("Boxes")
        {

        }

        protected override void OnWindowLoad(SDLWindowLoadEventArgs e)
        {
            box = LoadTexture(nameof(Properties.Resources.box));
        }

        protected override void OnDispose()
        {
            box?.Dispose();
        }

        protected override void OnWindowUpdate(SDLWindowUpdateEventArgs e)
        {
            double time = e.TotalTime / 1000;
            if (addX >= 0 && addY >= 0)
            {
                squares.Add(new Square(addX, addY, time));
                addX = -1;
                addY = -1;
            }
            int index = 0;
            int h = Height;
            while (index < squares.Count)
            {
                Square s = squares[index];
                float dT = (float)(time - s.lastUpdate);
                s.yvelocity += dT * GRAVITY;
                s.y += s.yvelocity * dT;
                s.x += s.xvelocity * dT;
                if (s.y > h - s.h)
                {
                    s.y = h - s.h;
                    s.xvelocity = 0;
                    s.yvelocity = 0;
                }
                s.lastUpdate = time;
                if (s.yvelocity <= 0 && s.lastUpdate > s.born + s.duration)
                {
                    squares.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
        }

        protected override void OnWindowPaint(SDLWindowPaintEventArgs e)
        {
            e.Renderer.BlendMode = BlendMode.Blend;
            foreach (Square s in squares)
            {
                e.Renderer.DrawTexture(box, new Rectangle((int)s.x, (int)s.y, (int)s.w, (int)s.h));
            }
        }

        protected override void OnMouseButtonDown(SDLMouseEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                leftMouseDown = true;
                addX = e.X;
                addY = e.Y;
            }
        }

        protected override void OnMouseButtonUp(SDLMouseEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                leftMouseDown = false;
            }
        }

        protected override void OnMouseMove(SDLMouseEventArgs e)
        {
            if (leftMouseDown)
            {
                addX = e.X;
                addY = e.Y;
            }
        }
        private static int Rand() { return random.Next(); }
        private class Square
        {
            public Square(int x, int y, double time)
            {
                w = Rand() % 80 + 40;
                h = Rand() % 80 + 40;
                this.x = x - w / 2;
                this.y = y - h / 2;
                yvelocity = -10;
                xvelocity = Rand() % 100 - 50;
                born = time;
                lastUpdate = time;
                duration = Rand() % 4 + 1;
            }
            public float x;
            public float y;
            public float w;
            public float h;
            public float xvelocity;
            public float yvelocity;
            public double born;
            public double lastUpdate;
            public double duration;
        }

    }
}
