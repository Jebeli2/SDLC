namespace SDLTest
{
    using SDLC;
    using SDLC.Applets;
    using SDLC.GUI;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class TestScreen : SDLScreen
    {
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
        //private SDLC.Applets.GUISystem gui = new();
        //private SDLC.Applets.BackgroundImage bg = new();
        //private LinesApp lines = new();
        //private RainingBoxesApp boxes = new();

        public TestScreen()
            : base("Test Screen")
        {
            Configuration.WindowTitle = "Test Window";
            Configuration.MaxFramesPerSecond = 120;
            Configuration.ShowFPS = true;
            //Configuration.SkipTaskbar = true;
        }

        public override void Show(IWindow window)
        {
            base.Show(window);
            GetApplet<BackgroundImage>().Image = LoadTexture(nameof(Properties.Resources.ice_palace), Properties.Resources.ice_palace);
            var boxes = GetApplet<RainingBoxesApp>();
            var lines = GetApplet<LinesApp>();

            boxes.RenderPrio = -500;
            lines.RenderPrio = -750;


            var gui = GetApplet<GUISystem>();

            Screen screen1 = gui.OpenScreen();
            Window window1 = gui.OpenWindow(screen1, leftEdge: 50, topEdge: 50, width: 300, height: 300, title: "Window 1", minWidth: 200, minHeight: 220);
            Gadget gad1 = gui.AddGadget(window1, leftEdge: 10, topEdge: 10, width: -20, height: 40, text: "GUI Test", clickAction: GoToGUITest);
            Gadget gad2 = gui.AddGadget(window1, leftEdge: 10, topEdge: 60, width: -20, height: 40, text: "Gadget 2");
            Gadget gad3 = gui.AddGadget(window1, leftEdge: 10, topEdge: 110, width: -20, height: 40, text: "Gadget 3");

            img2 = LoadTexture(nameof(Properties.Resources.badlands), Properties.Resources.badlands);
            mus1 = LoadMusic(nameof(Properties.Resources.jesu_joy), Properties.Resources.jesu_joy);
            //mus1 = LoadMusic(nameof(Properties.Resources.bach), Properties.Resources.bach);
            //mus1 = LoadMusic(nameof(Properties.Resources.loss_of_me), Properties.Resources.loss_of_me);
            //mus1 = LoadMusic(nameof(Properties.Resources.loss_of_me_2_), Properties.Resources.loss_of_me_2_);
            //mus1 = LoadMusic(nameof(Properties.Resources.loss_of_me_3_), Properties.Resources.loss_of_me_3_);
            //mus1 = LoadMusic(nameof(Properties.Resources.loss_of_me_metallic_pad_), Properties.Resources.loss_of_me_metallic_pad_);
            //mus1 = LoadMusic(nameof(Properties.Resources.loss_of_me_minstral_remix_), Properties.Resources.loss_of_me_minstral_remix_);
            //mus1 = LoadMusic(nameof(Properties.Resources.jesters_of_the_moon), Properties.Resources.jesters_of_the_moon);
            //mus1 = LoadMusic(nameof(Properties.Resources.find_your_way), Properties.Resources.find_your_way);
            SDLAudio.PlayMusic(mus1);
        }

        public override void Hide(IWindow window)
        {
            base.Hide(window);
            SDLAudio.StopMusic();
            mus1?.Dispose();
            img2?.Dispose();

        }


        private void GoToGUITest()
        {
            ChangeScreen(new GUITest());
        }

        public override void Update(IRenderer renderer, double totalTime, double elapsedTime)
        {
            base.Update(renderer, totalTime, elapsedTime);
            diff += (diffDirection * (elapsedTime / 10));
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
            angle += (elapsedTime / 10);
            if (angle > 360) { angle -= 360; }
        }

        public override void Render(IRenderer renderer, double totalTime, double elapsedTime)
        {
            base.Render(renderer, totalTime, elapsedTime);
            int width = renderer.Width;
            int height = renderer.Height;
            int midX = width / 2;
            int midY = height / 2;
            renderer.Color = Color.FromArgb(100, r, g, b);
            int d = (int)diff;

            renderer.FillRoundedRectangle(100 + d, 100 + d, width - 100 - d, height - 100 - d, 10 + d);
            renderer.DrawTexture(img2, midX - 100, midY - 100, 200, 200, angle);
            renderer.Color = Color.White;
            renderer.RoundedRectangle(10, 10, width - 20, height - 20, 10);
            renderer.AACircle(midX, midY, 100);
            renderer.AACircle(midX, midY, 110);

            //renderer.FillColorRect(new RectangleF(100, 100, 100, 100), Color.Black, Color.Black, Color.White, Color.White);
            //renderer.FillColorRect(new RectangleF(100, 200, 100, 100), Color.Black, Color.White, Color.Black, Color.White);
            //renderer.FillColorRect(new RectangleF(100, 300, 100, 100), Color.Black, Color.White, Color.White, Color.Black);
            string t1 = $"{width}x{height}";
            Size tsize = renderer.MeasureText(null, t1);
            renderer.DrawText(null, t1, midX - tsize.Width / 2, 10);
        }

        private static byte ShiftColor(byte b, ref bool inv)
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

    }
}
