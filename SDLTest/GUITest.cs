namespace SDLTest
{
    using SDLC;
    using SDLC.Applets;
    using SDLC.GUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class GUITest : SDLScreen
    {
        private SDLMusic? mus1;
        private GUISystem? gui;
        private Screen? screen1;
        private Window? window1;

        private Window? winButTest;
        private Gadget? fullScreenGadget;
        private Window? winPropTest;

        public GUITest() : base("GUI Test")
        {

        }

        public override void Show(IWindow window)
        {
            base.Show(window);
            GetApplet<BackgroundImage>().Image = LoadTexture(nameof(Properties.Resources.fire_temple), Properties.Resources.fire_temple);
            gui = GetApplet<GUISystem>();

            screen1 = gui.OpenScreen();
            window1 = gui.OpenWindow(screen1, leftEdge: 66, topEdge: 66, width: 400, height: 400, title: "Test GUI", minWidth: 200, minHeight: 220);
            window1.WindowClose += Window1_WindowClose;
            Gadget gad1 = gui.AddGadget(window1, leftEdge: 10, topEdge: 10, width: -20, height: 40, text: "Back", clickAction: GoToTestScreen);
            Gadget gad2 = gui.AddGadget(window1, leftEdge: 10, topEdge: 60, width: -20, height: 40, text: "Buttons", clickAction: ShowButtonTest);
            Gadget gad3 = gui.AddGadget(window1, leftEdge: 10, topEdge: 110, width: -20, height: 40, text: "Props & Strings", clickAction: ShowPropTest);

            mus1 = LoadMusic(nameof(Properties.Resources.loss_of_me_3_), Properties.Resources.loss_of_me_3_);
            SDLAudio.PlayMusic(mus1);
        }


        public override void Hide(IWindow window)
        {
            base.Hide(window);
            SDLAudio.StopMusic();
            mus1?.Dispose();
        }

        private void GoToTestScreen()
        {
            ChangeScreen(new TestScreen());
        }

        private void ToggleFullScreen()
        {
            IsFullScreen = !IsFullScreen;
            if (fullScreenGadget != null)
            {
                fullScreenGadget.Text = IsFullScreen ? "Windowed" : "Fullscreen";
            }
        }

        private void ShowPropTest()
        {
            if (gui != null && screen1 != null)
            {
                if (winPropTest == null)
                {
                    winPropTest = gui.OpenWindow(screen1, 400, 10, 500, 500, "Props & Strings");
                    winPropTest.WindowClose += WinPropTest_WindowClose;
                    Gadget prop1 = gui.AddGadget(winPropTest, 10, 10, -20, 20, type: GadgetType.PropGadget);
                    gui.ModifyProp(prop1, PropFlags.FreeHoriz, 0x1000, 0, 0x5555, 0);
                    Gadget prop2 = gui.AddGadget(winPropTest, 10, 40, -20, 20, type: GadgetType.PropGadget);
                    gui.ModifyProp(prop2, PropFlags.FreeHoriz, 0x5000, 0, 0x1000, 0);
                    Gadget prop3 = gui.AddGadget(winPropTest, 10, 70, -20, 100, type: GadgetType.PropGadget);
                    gui.ModifyProp(prop3, PropFlags.FreeHoriz | PropFlags.FreeVert, 0x5000, 0x4000, 0x2000, 0x8000);
                    Gadget str1 = gui.AddGadget(winPropTest, 10, 180, -20, 22, type: GadgetType.StrGadget, buffer: "Hello World");

                }
                else
                {
                    gui.WindowToFront(winPropTest);
                    gui.ActivateWindow(winPropTest);
                }
            }

        }


        private void ShowButtonTest()
        {
            if (gui != null && screen1 != null)
            {
                if (winButTest == null)
                {
                    winButTest = gui.OpenWindow(screen1, 400, 10, 500, 500, "Buttons");
                    winButTest.WindowClose += WinButTest_WindowClose;
                    fullScreenGadget = gui.AddGadget(winButTest, 10, 10, -20, 30, text: IsFullScreen ? "Windowed" : "Fullscreen", clickAction: ToggleFullScreen);
                }
                else
                {
                    gui.WindowToFront(winButTest);
                    gui.ActivateWindow(winButTest);
                }
            }
        }

        private void WinPropTest_WindowClose(object? sender, EventArgs e)
        {
            if (winPropTest != null)
            {
                gui?.CloseWindow(winPropTest);
                winPropTest = null;
            }
        }
        private void WinButTest_WindowClose(object? sender, EventArgs e)
        {
            if (winButTest != null)
            {
                gui?.CloseWindow(winButTest);
                winButTest = null;
                fullScreenGadget = null;
            }
        }

        private void Window1_WindowClose(object? sender, EventArgs e)
        {
            GoToTestScreen();
        }

    }
}
