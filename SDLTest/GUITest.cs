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
        private const string DC = @"D:\Users\jebel\Music\iTunes\iTunes Media\Music\Alt-J\Relaxer\05 Deadcrush.mp3";
        private const string FF9_1 = @"d:\Users\jebel\Music\iTunes\iTunes Media\Music\Nobuo Uematsu\Final Fantasy IX\05 Black Mage Village.mp3";
        private const string FF9_2 = @"d:\Users\jebel\Music\iTunes\iTunes Media\Music\Nobuo Uematsu\Final Fantasy IX\02 Jesters of the Moon.mp3";
        private const string FF9_3 = @"d:\Users\jebel\Music\iTunes\iTunes Media\Music\Nobuo Uematsu\Final Fantasy IX\03 Loss of Me.mp3";
        private Screen? screen1;
        private Window? window1;

        private Window? winButTest;
        private Window? winPropTest;
        private Requester? requester;
        private string musicText = "";
        private float scrollX;
        private float scrollY;
        private float scrollSpeed = 0.1f;
        private double lastScrollTime;
        public GUITest() : base("GUI Test")
        {

        }

        public override void Update(IRenderer renderer, double totalTime, double elapsedTime)
        {
            scrollY = Height - 40;
            base.Update(renderer, totalTime, elapsedTime);
            double dT = totalTime - lastScrollTime;
            scrollX += (float)(dT * scrollSpeed);
            if (scrollX > Width) { scrollX = -200; }
            lastScrollTime = totalTime;
        }

        public override void Render(IRenderer renderer, double totalTime, double elapsedTime)
        {
            base.Render(renderer, totalTime, elapsedTime);
            renderer.DrawShadowedText(null, musicText, scrollX, scrollY);
        }
        public override void Show(IWindow window)
        {
            base.Show(window);
            GetApplet<BackgroundImage>().Image = LoadTexture(nameof(Properties.Resources.fire_temple));
            GetApplet<MusicPlayer>().AddToPlayList(nameof(Properties.Resources.loss_of_me_3_), "Loss of Me 3");
            GetApplet<MusicPlayer>().AddToPlayList(nameof(Properties.Resources.jesu_joy), "Bach 2");
            GetApplet<MusicPlayer>().AddToPlayList(nameof(Properties.Resources.jesters_of_the_moon), "Jesters of the Moon 1");
            GetApplet<MusicPlayer>().AddToPlayList(FF9_1, "Black Mage Village");
            GetApplet<MusicPlayer>().AddToPlayList(FF9_2, "Jesters of the Moon 2");
            GetApplet<MusicPlayer>().AddToPlayList(FF9_3, "Loss of Me 1");
            GetApplet<MusicPlayer>().AddToPlayList(SONG2, "Song 2");
            GetApplet<MusicPlayer>().AddToPlayList(DC, "Deathcrush");
            musicText = GetApplet<MusicPlayer>().CurrentEntry?.Title ?? "no music";
            SDLAudio.MusicStarted += SDLAudio_MusicStarted;
            IGUISystem gui = GUI;
            screen1 = gui.OpenScreen();
            window1 = gui.OpenWindow(screen1, leftEdge: 66, topEdge: 66, width: 400, height: 400, title: "Test GUI", minWidth: 200, minHeight: 220);
            window1.WindowClose += Window1_WindowClose;
            GadTools.CreateContext(gui, window1);
            _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 10, topEdge: 10, width: -20, height: 40, text: "Back", clickAction: GoToTestScreen);
            _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 10, topEdge: 60, width: -20, height: 40, text: "Buttons", clickAction: ShowButtonTest);
            _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 10, topEdge: 110, width: -20, height: 40, text: "Props & Strings", clickAction: ShowPropTest);
        }

        public override void Hide(IWindow window)
        {
            base.Hide(window);
            SDLAudio.MusicStarted += SDLAudio_MusicStarted;
        }
        private void SDLAudio_MusicStarted(object? sender, SDLMusicEventArgs e)
        {
            musicText = GetApplet<MusicPlayer>().CurrentEntry?.Title ?? "no music";
        }

        private void GoToTestScreen()
        {
            ChangeScreen(new TestScreen());
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
                    var nu = GadTools.CreateGadget(GadgetKind.Number, leftEdge: 10, topEdge: 300, width: 200, height: 30, text: "Level {0}", intValue: 1);
                    _ = GadTools.CreateGadget(GadgetKind.Slider, leftEdge: 10, topEdge: 10, width: -20, height: 20, min: 1, max: 3, level: 2, valueChangedAction:
                        (level) =>
                        {
                            SDLLog.Info(LogCategory.APPLICATION, "Slider Level changed to {0}", level);
                        });
                    _ = GadTools.CreateGadget(GadgetKind.Slider, leftEdge: 10, topEdge: 40, width: -20, height: 20, min: 1, max: 16, valueChangedAction:
                        (level) =>
                        {
                            GadTools.SetAttrs(nu, intValue: level);
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
                    _ = GadTools.CreateGadget(GadgetKind.Integer, leftEdge: 10, topEdge: 270, width: -20, height: 22, intValue: 1234);
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

                    _ = GadTools.CreateGadget(GadgetKind.Text, leftEdge: 10, topEdge: 10, width: -20, height: 30, text: "Button Demo");
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
                    var cb = GadTools.CreateGadget(GadgetKind.Checkbox, leftEdge: 10, topEdge: 290, width: 200, height: 30, text: "Checkbox", _checked: true, disabled: false);
                    _ = GadTools.CreateGadget(GadgetKind.Checkbox, leftEdge: 220, topEdge: 290, width: 200, height: 30, text: "Fullscreen", _checked: IsFullScreen, checkedStateChangedAction:
                        (b) => { IsFullScreen = b; });
                    var mx = GadTools.CreateGadget(GadgetKind.Mx, leftEdge: 220, topEdge: 320, width: 200, height: 30,
                        options: new FullScreenMode[] { FullScreenMode.Desktop, FullScreenMode.FullSize, FullScreenMode.MultiMonitor },
                        selectedIndex: (int)FullScreenMode,
                        valueChangedAction: (index) => { FullScreenMode = (FullScreenMode)index; });
                    _ = GadTools.CreateGadget(GadgetKind.Checkbox, leftEdge: 10, topEdge: 320, width: 200, height: 30, text: "Disabled", _checked: false, disabled: false, checkedStateChangedAction:
                        (b) => { cb.Enabled = !b; mx.Enabled = !b; });
                    _ = GadTools.CreateGadget(GadgetKind.Checkbox, leftEdge: 10, topEdge: 350, width: 200, height: 30, text: "Debug Bounds", _checked: gui.ShowDebugBounds, disabled: false, checkedStateChangedAction:
                        (b) => { gui.ShowDebugBounds = b; });
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
            }
        }

        private void Window1_WindowClose(object? sender, EventArgs e)
        {
            GoToTestScreen();
        }

    }
}
