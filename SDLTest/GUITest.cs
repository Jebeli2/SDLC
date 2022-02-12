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
            IGUISystem gui = GUI;
            screen1 = gui.OpenScreen();
            window1 = gui.OpenWindow(screen1, leftEdge: 66, topEdge: 66, width: 400, height: 400, title: "Test GUI", minWidth: 200, minHeight: 220);
            window1.WindowClose += Window1_WindowClose;
            GadTools.CreateContext(gui, window1);
            _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 10, topEdge: 10, width: -20, height: 40, text: "Back", clickAction: GoToTestScreen);
            _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 10, topEdge: 60, width: -20, height: 40, text: "Buttons", clickAction: ShowButtonTest);
            _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 10, topEdge: 110, width: -20, height: 40, text: "Props & Strings", clickAction: ShowPropTest);

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
            if (screen1 != null)
            {
                IGUISystem gui = GUI;
                if (winPropTest == null)
                {
                    winPropTest = gui.OpenWindow(screen1, 400, 10, 500, 500, "Props & Strings");
                    winPropTest.WindowClose += WinPropTest_WindowClose;
                    GadTools.CreateContext(gui, winPropTest);
                    _ = GadTools.CreateGadget(GadgetKind.Slider, leftEdge: 10, topEdge: 10, width: -20, height: 20, min: 1, max: 3, level: 2, valueChangedAction:
                        (level) =>
                        {
                            SDLLog.Info(LogCategory.APPLICATION, "Slider Level changed to {0}", level);
                        });
                    _ = GadTools.CreateGadget(GadgetKind.Slider, leftEdge: 10, topEdge: 40, width: -20, height: 20, min: 1, max: 16, valueChangedAction:
                        (level) =>
                        {
                            SDLLog.Info(LogCategory.APPLICATION, "Slider Level changed to {0}", level);
                        });
                    Gadget prop3 = gui.AddGadget(winPropTest, leftEdge: 10, topEdge: 70, width: -20, height: 100, type: GadgetType.PropGadget);
                    gui.ModifyProp(prop3, PropFlags.FreeHoriz | PropFlags.FreeVert, 0x5000, 0x5000, 0x2000, 0x4000);


                    _ = GadTools.CreateGadget(GadgetKind.String, leftEdge: 10, topEdge: 180, width: -20, height: 22, buffer: "Hello World");
                    _ = GadTools.CreateGadget(GadgetKind.String, leftEdge: 10, topEdge: 210, width: -20, height: 22, buffer: "Example Text");
                    _ = GadTools.CreateGadget(GadgetKind.Scroller, leftEdge: 10, topEdge: 240, width: -20, height: 22, total: 20, visible: 10, top: 3, valueChangedAction:
                        (top) =>
                        {
                            SDLLog.Info(LogCategory.APPLICATION, "Scroller Top changed to {0}", top);
                        });
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
            if (screen1 != null)
            {
                IGUISystem gui = GUI;
                if (winButTest == null)
                {
                    winButTest = gui.OpenWindow(screen1, 400, 10, 500, 500, "Buttons");
                    winButTest.WindowClose += WinButTest_WindowClose;
                    GadTools.CreateContext(gui, winButTest);

                    requester = gui.InitRequester(winButTest);
                    requester.Flags = ReqFlags.PointRel;
                    requester.Width = 200;
                    requester.Height = 200;
                    _ = GadTools.CreateGadget(GadgetKind.Text, requester: requester, leftEdge: 10, topEdge: 10, width: -20, height: 20, text: "Question?");
                    _ = GadTools.CreateGadget(GadgetKind.Button, requester: requester, leftEdge: 10, topEdge: -30, width: -20, height: 20, endGadget: true, text: "OK");
                    fullScreenGadget = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 10, topEdge: 10, width: -20, height: 30, text: IsFullScreen ? "Windowed" : "Fullscreen", clickAction: ToggleFullScreen);
                    _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 10, topEdge: 50, width: -20, height: 30, text: "Toggle Button", toggleSelect: true);
                    _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 10, topEdge: 90, width: -20, height: 30, text: "Icon Button", icon: Icons.ENTYPO_ICON_YOUKO);
                    _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 10, topEdge: 130, width: -20, height: 30, text: "Disabled Button", disabled: true);
                    _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 10, topEdge: 170, width: -20, height: 30, text: "Color Button", bgColor: Color.Blue);
                    _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 10, topEdge: 210, width: -262, height: 30, text: "Play Next", clickAction:
                        () => { GetApplet<MusicPlayer>().NextMusic(); });
                    _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: -248, topEdge: 210, width: 240, height: 30, text: "Play Prev", clickAction:
                        () => { GetApplet<MusicPlayer>().PrevMusic(); });
                    _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 10, topEdge: 250, width: 30, height: 30, icon: Icons.ENTYPO_ICON_MUSIC, toggleSelect: true,
                        selected: GetApplet<MusicVisualizer>().Enabled, clickAction: () => { GetApplet<MusicVisualizer>().Enabled ^= true; });
                    _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 50, topEdge: 250, width: 30, height: 30, icon: Icons.ENTYPO_ICON_LINE_GRAPH, toggleSelect: true,
                        selected: GetApplet<LinesApp>().Enabled, clickAction: () => { GetApplet<LinesApp>().Enabled ^= true; });
                    _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 90, topEdge: 250, width: 30, height: 30, icon: Icons.ENTYPO_ICON_BOX, toggleSelect: true,
                        selected: GetApplet<RainingBoxesApp>().Enabled, clickAction: () => { GetApplet<RainingBoxesApp>().Enabled ^= true; });
                    _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 130, topEdge: 250, width: 30, height: 30, icon: Icons.ENTYPO_ICON_ADDRESS, clickAction:
                        () => { GUI.Request(requester, winButTest); });
                    _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 170, topEdge: 250, width: 30, height: 30, icon: Icons.ENTYPO_ICON_BRIEFCASE, clickAction:
                        () =>
                        {
                            if (window1 != null && winButTest != null && winPropTest != null)
                            {
                                GUI.WindowToBack(window1);
                                GUI.MoveWindowInFrontOf(winButTest, window1);
                                GUI.MoveWindowInFrontOf(winPropTest, winButTest);
                            }
                        });
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
                GUI.CloseWindow(winPropTest);
                winPropTest = null;
            }
        }
        private void WinButTest_WindowClose(object? sender, EventArgs e)
        {
            if (winButTest != null)
            {
                GUI.CloseWindow(winButTest);
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
