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

    internal class GUITest : SDLScreen
    {
        private const string SONG2 = @"D:\Users\jebel\Music\iTunes\iTunes Media\Music\Blur\Blur\02 Song 2.mp3";
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
            GetApplet<BackgroundImage>().Image = LoadTexture(nameof(Properties.Resources.fire_temple));
            GetApplet<MusicPlayer>().AddToPlayList(nameof(Properties.Resources.loss_of_me_3_));
            GetApplet<MusicPlayer>().AddToPlayList(nameof(Properties.Resources.jesu_joy));
            GetApplet<MusicPlayer>().AddToPlayList(nameof(Properties.Resources.jesters_of_the_moon));
            GetApplet<MusicPlayer>().AddToPlayList(SONG2);

            gui = GetApplet<GUISystem>();

            screen1 = gui.OpenScreen();
            window1 = gui.OpenWindow(screen1, leftEdge: 66, topEdge: 66, width: 400, height: 400, title: "Test GUI", minWidth: 200, minHeight: 220);
            window1.WindowClose += Window1_WindowClose;
            Gadget gad1 = gui.AddGadget(window1, leftEdge: 10, topEdge: 10, width: -20, height: 40, text: "Back", clickAction: GoToTestScreen);
            Gadget gad2 = gui.AddGadget(window1, leftEdge: 10, topEdge: 60, width: -20, height: 40, text: "Buttons", clickAction: ShowButtonTest);
            Gadget gad3 = gui.AddGadget(window1, leftEdge: 10, topEdge: 110, width: -20, height: 40, text: "Props & Strings", clickAction: ShowPropTest);

        }


        public override void Hide(IWindow window)
        {
            base.Hide(window);
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
                    Gadget str2 = gui.AddGadget(winPropTest, 10, 210, -20, 22, type: GadgetType.StrGadget, buffer: "Example Text");

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
                    _ = gui.AddGadget(winButTest, 10, 50, -20, 30, text: "Toggle Button", toggleSelect: true);
                    _ = gui.AddGadget(winButTest, 10, 90, -20, 30, text: "Icon Button", icon: Icons.ENTYPO_ICON_YOUKO);
                    _ = gui.AddGadget(winButTest, 10, 130, -20, 30, text: "Disabled Button", disabled: true);
                    var colorGad = gui.AddGadget(winButTest, 10, 170, -20, 30, text: "Color Button");
                    colorGad.BackgroundColor = Color.Blue;
                    _ = gui.AddGadget(winButTest, 10, 210, -262, 30, text: "Play Next", clickAction:
                        () => { GetApplet<MusicPlayer>().NextMusic(); });
                    _ = gui.AddGadget(winButTest, -248, 210, 240, 30, text: "Play Prev", clickAction:
                        () => { GetApplet<MusicPlayer>().PrevMusic(); });
                    _ = gui.AddGadget(winButTest, 10, 250, 30, 30, icon: Icons.ENTYPO_ICON_MUSIC, toggleSelect: true,
                        selected: GetApplet<MusicVisualizer>().Enabled, clickAction: () => { GetApplet<MusicVisualizer>().Enabled ^= true; });
                    _ = gui.AddGadget(winButTest, 50, 250, 30, 30, icon: Icons.ENTYPO_ICON_LINE_GRAPH, toggleSelect: true,
                        selected: GetApplet<LinesApp>().Enabled, clickAction: () => { GetApplet<LinesApp>().Enabled ^= true; });
                    _ = gui.AddGadget(winButTest, 90, 250, 30, 30, icon: Icons.ENTYPO_ICON_BOX, toggleSelect: true,
                        selected: GetApplet<RainingBoxesApp>().Enabled, clickAction: () => { GetApplet<RainingBoxesApp>().Enabled ^= true; });
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
