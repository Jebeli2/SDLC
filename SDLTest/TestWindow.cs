namespace SDLTest
{
    using SDLC;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class TestWindow : SDLWindow
    {
        private SDLTexture? img1;
        private SDLTexture? img2;
        private SDLMusic? mus1;
        private double angle;
        private double diff;
        private int diffDirection = 1;
        private byte r = 0;
        private byte g = 40;
        private byte b = 80;
        private bool rinv;
        private bool ginv;
        private bool binv;
        private LinesApp lines = new();
        private RainingBoxesApp boxes = new();
        public TestWindow()
            : base("Test Window")
        {
            //Driver = "opengl";
            //Driver = "direct3d";
            Driver = "direct3d11";
            //SDLApplication.MaxFramesPerSecond = 120;
            SDLApplication.MaxFramesPerSecond = 90;
            ShowFPS = true;
            FPSPosX = 10;
            FPSPosY = 10;
            Width = 1440;
            Height = 900;
            //MouseGrab = true;
            //SizeMode = RendererSizeMode.BackBuffer;
            //BackBufferWidth = 720;
            //BackBufferHeight = 480;
        }

        protected override void OnWindowLoad(SDLWindowLoadEventArgs e)
        {
            img1 = e.Renderer.LoadTexture(nameof(Properties.Resources.ice_palace), Properties.Resources.ice_palace);
            img2 = e.Renderer.LoadTexture(nameof(Properties.Resources.badlands), Properties.Resources.badlands);
            //mus1 = SDLAudio.LoadMusic(nameof(Properties.Resources.bach), Properties.Resources.bach);
            mus1 = SDLAudio.LoadMusic(nameof(Properties.Resources.jesu_joy), Properties.Resources.jesu_joy);
            //mus1 = SDLAudio.LoadMusic(nameof(Properties.Resources.loss_of_me), Properties.Resources.loss_of_me);
            //mus1 = SDLAudio.LoadMusic(nameof(Properties.Resources.loss_of_me_2_), Properties.Resources.loss_of_me_2_);
            //mus1 = SDLAudio.LoadMusic(nameof(Properties.Resources.loss_of_me_3_), Properties.Resources.loss_of_me_3_);
            //mus1 = SDLAudio.LoadMusic(nameof(Properties.Resources.loss_of_me_metallic_pad_), Properties.Resources.loss_of_me_metallic_pad_);
            //mus1 = SDLAudio.LoadMusic(nameof(Properties.Resources.loss_of_me_minstral_remix_), Properties.Resources.loss_of_me_minstral_remix_);
            //mus1 = SDLAudio.LoadMusic(nameof(Properties.Resources.jesters_of_the_moon), Properties.Resources.jesters_of_the_moon);
            //mus1 = SDLAudio.LoadMusic(nameof(Properties.Resources.find_your_way), Properties.Resources.find_your_way);
            AddApplet(boxes);
            AddApplet(lines);
            SDLAudio.PlayMusic(mus1);
            base.OnWindowLoad(e);
        }

        protected override void OnWindowClose(EventArgs e)
        {
            SDLAudio.StopMusic();
            img1?.Dispose();
            img2?.Dispose();
            mus1?.Dispose();
            base.OnWindowClose(e);
        }

        protected override void OnKeyUp(SDLKeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case KeyCode.F10:
                    if (FullScreen)
                    {
                        FullScreen = false;
                        MouseGrab = false;
                    }
                    else
                    {
                        FullScreenMode = FullScreenMode.Desktop;
                        FullScreen = true;
                        MouseGrab = true;
                    }
                    break;
                case KeyCode.F11:
                    if (FullScreen)
                    {
                        FullScreen = false;
                        MouseGrab = false;
                    }
                    else
                    {
                        FullScreenMode = FullScreenMode.FullSize;
                        FullScreen = true;
                        MouseGrab = true;
                    }
                    break;
                case KeyCode.F12:
                    if (FullScreen)
                    {
                        FullScreen = false;
                        MouseGrab = false;
                    }
                    else
                    {
                        FullScreenMode = FullScreenMode.MultiMonitor;
                        FullScreen = true;
                        MouseGrab = true;
                    }
                    break;
            }
            base.OnKeyUp(e);
        }

        protected override void OnWindowUpdate(SDLWindowUpdateEventArgs e)
        {
            diff += (diffDirection * (e.ElapsedTime / 10));
            if (diff > 100)
            {
                diffDirection = -1;
            }
            else if (diff < 1)
            {
                diffDirection = 1;
            }
            r = ShiftColor(r, ref rinv);
            g = ShiftColor(g, ref ginv);
            b = ShiftColor(b, ref binv);
            angle += (e.ElapsedTime / 10);
            if (angle > 360) { angle -= 360; }
            base.OnWindowUpdate(e);
        }

        private byte ShiftColor(byte b, ref bool inv)
        {
            if (b == 255)
            {
                inv = true;
            }
            else if (b == 0)
            {
                inv = false;
            }
            if (inv) { b--; } else { b++; }
            return b;
        }
        protected override void OnWindowPaint(SDLWindowPaintEventArgs e)
        {
            e.Renderer.DrawTexture(img1);
            int midX = e.Width / 2;
            int midY = e.Height / 2;
            e.Renderer.Color = Color.FromArgb(100, r, g, b);
            int d = (int)diff;

            e.Renderer.FillRoundedRectangle(100 + d, 100 + d, e.Width - 100 - d, e.Height - 100 - d, 10 + d);
            e.Renderer.DrawTexture(img2, midX - 100, midY - 100, 200, 200, angle);
            e.Renderer.Color = Color.White;
            e.Renderer.RoundedRectangle(10, 10, e.Width - 20, e.Height - 20, 10);
            e.Renderer.AACircle(midX, midY, 100);
            e.Renderer.AACircle(midX, midY, 110);

            e.Renderer.FillColorRect(new RectangleF(100, 100, 100, 100), Color.Black, Color.Black, Color.White, Color.White);
            e.Renderer.FillColorRect(new RectangleF(100, 200, 100, 100), Color.Black, Color.White, Color.Black, Color.White);
            e.Renderer.FillColorRect(new RectangleF(100, 300, 100, 100), Color.Black, Color.White, Color.White, Color.Black);
            string t1 = $"{e.Width}x{e.Height}";
            Size tsize = e.Renderer.MeasureText(null, t1);
            e.Renderer.DrawText(null, t1, midX - tsize.Width / 2, 10);
            base.OnWindowPaint(e);
        }
    }
}
