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
        private double angle;
        private double diff;
        private int diffDirection = 1;
        private byte r = 0;
        private byte g = 40;
        private byte b = 80;
        private bool rinv;
        private bool ginv;
        private bool binv;
        private string text1 = "";
        private string text2 = "";
        private float scrollX = 0;
        private float scrollSpeed = 0.1f;
        private double lastScrollTime;

        public TestScreen()
            : base("Test Screen")
        {
            //Configuration.Driver = "opengl";
            Configuration.WindowTitle = "Test Window";
            Configuration.MaxFramesPerSecond = 120;
            Configuration.ShowFPS = true;
            //Configuration.SkipTaskbar = true;
        }

        public override void Show(IWindow window)
        {
            base.Show(window);
            AddResourceManager(Properties.Resources.ResourceManager);
            GetApplet<BackgroundImage>().Image = LoadTexture(nameof(Properties.Resources.ice_palace));
            GetApplet<MusicPlayer>().PlayNow(nameof(Properties.Resources.bach));
            var boxes = GetApplet<RainingBoxesApp>();
            var lines = GetApplet<LinesApp>();
            var music = GetApplet<MusicVisualizer>();

            boxes.RenderPrio = -500;
            lines.RenderPrio = -750;
            music.RenderPrio = -800;

            IGUISystem gui = GUI;

            Screen screen1 = gui.OpenScreen();
            Window window1 = gui.OpenWindow(screen1, leftEdge: 50, topEdge: 50, width: 300, height: 300,
                minWidth: 200, minHeight: 220,
                borderless: true,
                sizing: false,
                dragging: false,
                zooming: false,
                closing: false,
                depth: false,
                backdrop: true);
            Gadget gad1 = gui.AddGadget(window1, leftEdge: 10, topEdge: 10, width: -20, height: 40, text: "GUI Test", clickAction: GoToGUITest);
            Gadget gad2 = gui.AddGadget(window1, leftEdge: 10, topEdge: 60, width: -20, height: 40, text: "Gadget 2");
            Gadget gad3 = gui.AddGadget(window1, leftEdge: 10, topEdge: 110, width: -20, height: 40, text: "Gadget 3");

            img2 = LoadTexture(nameof(Properties.Resources.badlands));
            text1 = $"{Width}x{Height}";
            text2 = "Test Scroll Text";
        }

        public override void Hide(IWindow window)
        {
            base.Hide(window);
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
            double dT = totalTime - lastScrollTime;
            scrollX += (float)(dT * scrollSpeed);
            if (scrollX > Width) { scrollX = -200; }
            lastScrollTime = totalTime;
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


            Size tsize = renderer.MeasureText(null, text1);
            renderer.DrawText(null, text1, midX - tsize.Width / 2, 10);
            renderer.DrawText(null, text2, scrollX, midX, Color.White);
        }

        public override void Resized(IWindow window, int width, int height)
        {
            base.Resized(window, width, height);
            text1 = $"{width}x{height}";
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
