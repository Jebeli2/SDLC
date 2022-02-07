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
        public TestWindow()
            : base("Test Window")
        {
            //Driver = "opengl";
            //Driver = "direct3d";
            Driver = "direct3d11";
            SDLApplication.MaxFramesPerSecond = 75;
            ShowFPS = true;
            FPSPosX = 10;
            FPSPosY = 10;
            Width = 1440;
            Height = 900;
            //SizeMode = RendererSizeMode.BackBuffer;
            //BackBufferWidth = 1440;
            //BackBufferHeight = 900;
        }

        protected override void OnWindowLoad(SDLWindowLoadEventArgs e)
        {
            img1 = e.Renderer.LoadTexture(nameof(Properties.Resources.ice_palace), Properties.Resources.ice_palace);
            img2 = e.Renderer.LoadTexture(nameof(Properties.Resources.badlands), Properties.Resources.badlands);
            mus1 = SDLAudio.LoadMusic(nameof(Properties.Resources.bach), Properties.Resources.bach);
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
                    }
                    else
                    {
                        FullScreenMode = FullScreenMode.Desktop;
                        FullScreen = true;
                    }
                    break;
                case KeyCode.F11:
                    if (FullScreen)
                    {
                        FullScreen = false;
                    }
                    else
                    {
                        FullScreenMode = FullScreenMode.FullSize;
                        FullScreen = true;
                    }
                    break;
                case KeyCode.F12:
                    if (FullScreen)
                    {
                        FullScreen = false;
                    }
                    else
                    {
                        FullScreenMode = FullScreenMode.MultiMonitor;
                        FullScreen = true;
                    }
                    break;
            }
            base.OnKeyUp(e);
        }

        protected override void OnWindowUpdate(SDLWindowUpdateEventArgs e)
        {
            angle += (e.ElapsedTime / 10);
            //angle += 1;
            if (angle > 360) { angle -= 360; }
            base.OnWindowUpdate(e);
        }
        protected override void OnWindowPaint(SDLWindowPaintEventArgs e)
        {
            e.Renderer.DrawTexture(img1);
            int midX = e.Width / 2;
            int midY = e.Height / 2;
            e.Renderer.DrawTexture(img2, midX - 100, midY - 100, 200, 200, angle);
            e.Renderer.DrawRect(10, 10, e.Width - 20, e.Height - 20, Color.White);
            base.OnWindowPaint(e);
        }
    }
}
