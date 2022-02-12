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
        private Requester? requester;

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
                    Gadget prop1 = gui.AddGadget(winPropTest, leftEdge: 10, topEdge: 10, width: -20, height: 20, type: GadgetType.PropGadget);
                    gui.ModifyProp(prop1, PropFlags.FreeHoriz, 0x1000, 0, 0x5555, 0);
                    Gadget prop2 = gui.AddGadget(winPropTest, leftEdge: 10, topEdge: 40, width: -20, height: 20, type: GadgetType.PropGadget);
                    gui.ModifyProp(prop2, PropFlags.FreeHoriz, 0x5000, 0, 0x1000, 0);
                    Gadget prop3 = gui.AddGadget(winPropTest, leftEdge: 10, topEdge: 70, width: -20, height: 100, type: GadgetType.PropGadget);
                    gui.ModifyProp(prop3, PropFlags.FreeHoriz | PropFlags.FreeVert, 0x5000, 0x4000, 0x2000, 0x8000);
                    Gadget str1 = gui.AddGadget(winPropTest, leftEdge: 10, topEdge: 180, width: -20, height: 22, type: GadgetType.StrGadget, buffer: "Hello World");
                    Gadget str2 = gui.AddGadget(winPropTest, leftEdge: 10, topEdge: 210, width: -20, height: 22, type: GadgetType.StrGadget, buffer: "Example Text");

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
                    //winButTest.Superbitmap = false;
                    winButTest.WindowClose += WinButTest_WindowClose;
                    requester = gui.InitRequester(winButTest);
                    requester.Flags = ReqFlags.PointRel;
                    requester.Width = 200;
                    requester.Height = 200;
                    _ = gui.AddGadget(winButTest, requester, leftEdge: 10, topEdge: -30, width: -20, height: 20, endGadget: true, text: "OK");

                    fullScreenGadget = gui.AddGadget(winButTest, leftEdge: 10, topEdge: 10, width: -20, height: 30, text: IsFullScreen ? "Windowed" : "Fullscreen", clickAction: ToggleFullScreen);
                    _ = gui.AddGadget(winButTest, leftEdge: 10, topEdge: 50, width: -20, height: 30, text: "Toggle Button", toggleSelect: true);
                    _ = gui.AddGadget(winButTest, leftEdge: 10, topEdge: 90, width: -20, height: 30, text: "Icon Button", icon: Icons.ENTYPO_ICON_YOUKO);
                    _ = gui.AddGadget(winButTest, leftEdge: 10, topEdge: 130, width: -20, height: 30, text: "Disabled Button", disabled: true);
                    _ = gui.AddGadget(winButTest, leftEdge: 10, topEdge: 170, width: -20, height: 30, text: "Color Button", bgColor: Color.Blue);
                    _ = gui.AddGadget(winButTest, leftEdge: 10, topEdge: 210, width: -262, height: 30, text: "Play Next", clickAction:
                        () => { GetApplet<MusicPlayer>().NextMusic(); });
                    _ = gui.AddGadget(winButTest, leftEdge: -248, topEdge: 210, width: 240, height: 30, text: "Play Prev", clickAction:
                        () => { GetApplet<MusicPlayer>().PrevMusic(); });
                    _ = gui.AddGadget(winButTest, leftEdge: 10, topEdge: 250, width: 30, height: 30, icon: Icons.ENTYPO_ICON_MUSIC, toggleSelect: true,
                        selected: GetApplet<MusicVisualizer>().Enabled, clickAction: () => { GetApplet<MusicVisualizer>().Enabled ^= true; });
                    _ = gui.AddGadget(winButTest, leftEdge: 50, topEdge: 250, width: 30, height: 30, icon: Icons.ENTYPO_ICON_LINE_GRAPH, toggleSelect: true,
                        selected: GetApplet<LinesApp>().Enabled, clickAction: () => { GetApplet<LinesApp>().Enabled ^= true; });
                    _ = gui.AddGadget(winButTest, leftEdge: 90, topEdge: 250, width: 30, height: 30, icon: Icons.ENTYPO_ICON_BOX, toggleSelect: true,
                        selected: GetApplet<RainingBoxesApp>().Enabled, clickAction: () => { GetApplet<RainingBoxesApp>().Enabled ^= true; });
                    _ = gui.AddGadget(winButTest, leftEdge: 130, topEdge: 250, width: 30, height: 30, icon: Icons.ENTYPO_ICON_ADDRESS, clickAction:
                        () => { gui.Request(requester, winButTest); });
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
