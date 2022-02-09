namespace SDLC.Applets
{
    using SDLC.GUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GUISystem : SDLApplet, IGUISystem
    {
        private readonly List<Screen> screens = new();
        private IGUIRenderer guiRenderer = new DefaultGUIRenderer();
        private int nextGadgetID;
        private int nextWindowID;
        private int prevMouseX;
        private int prevMouseY;
        private int mouseX;
        private int mouseY;
        private int diffMouseX;
        private int diffMouseY;
        private readonly Queue<Window> activationWindows = new();
        private readonly Queue<Gadget> activationGadgets = new();
        private Screen? activeScreen;
        private Screen? mouseScreen;
        private Window? activeWindow;
        private Window? mouseWindow;
        private Gadget? activeGadget;
        private Gadget? mouseGadget;
        private Gadget? downGadget;
        private Gadget? upGadget;
        private Gadget? selectedGadget;
        private bool selectMouseDown;
        private int timerTick;
        private int tickIntervall = 100;

        private int sysGadgetWidth = 32;
        private int sysGadgetHeight = 28;

        private const int SYSGAD_DRAG = -1;
        private const int SYSGAD_CLOSE = -2;
        private const int SYSGAD_DEPTH = -3;
        private const int SYSGAD_ZOOM = -4;
        private const int SYSGAD_SIZE = -5;

        public GUISystem() : base("GUI System")
        {
            RenderPrio = 1000;
            InputPrio = -1000;
        }

        protected override void OnWindowLoad(SDLWindowLoadEventArgs e)
        {
            UpdateScreenSize(e.Renderer.Width, e.Renderer.Height);

        }
        protected internal override void OnWindowSizeChanged(SDLWindowSizeEventArgs e)
        {
            UpdateScreenSize(e.Width, e.Height);
        }

        protected internal override void OnWindowResized(SDLWindowSizeEventArgs e)
        {
            UpdateScreenSize(e.Width, e.Height);
        }

        protected override void OnWindowUpdate(SDLWindowUpdateEventArgs e)
        {
            CheckWindowActivationQueue();
            CheckGadgetActivationQueue();
            CheckTimer(e.TotalTime);
        }
        protected override void OnWindowPaint(SDLWindowPaintEventArgs e)
        {
            foreach (Screen screen in screens)
            {
                screen.Render(e.Renderer, guiRenderer);
            }
        }

        protected override void OnDispose()
        {
            foreach (Screen screen in screens)
            {
                screen.Close();
            }
            screens.Clear();
        }
        private void UpdateScreenSize(int width, int height)
        {
            foreach (Screen screen in screens)
            {
                screen.Width = width;
                screen.Height = height;
            }
        }
        private void SetMouseScreen(Screen? scr)
        {
            if (mouseScreen != scr)
            {
                if (mouseScreen != null)
                {
                    //mouseScreen.MouseHover = false;
                }
                mouseScreen = scr;
                if (mouseScreen != null)
                {
                    //mouseScreen.MouseHover = true;
                }
            }
        }
        private void SetActiveScreen(Screen? scr)
        {
            if (activeScreen != scr)
            {
                if (activeScreen != null)
                {
                    //activeScreen.Active = false;
                    SDLLog.Debug(LogCategory.APPLICATION, $"Screen deactivated: {activeScreen}");
                }
                activeScreen = scr;
                if (activeScreen != null)
                {
                    //activeScreen.Active = true;
                    SDLLog.Debug(LogCategory.APPLICATION, $"Screen activated: {activeScreen}");
                }
            }
        }

        private void SetMouseWindow(Window? win)
        {
            if (mouseWindow != win)
            {
                if (mouseWindow != null)
                {
                    mouseWindow.MouseHover = false;
                }
                mouseWindow = win;
                if (mouseWindow != null)
                {
                    mouseWindow.MouseHover = true;
                }
            }
        }

        private void SetActiveWindow(Window? win)
        {
            if (activeWindow != win)
            {
                if (activeWindow != null)
                {
                    activeWindow.Active = false;
                    SDLLog.Debug(LogCategory.APPLICATION, $"Window deactivated: {activeWindow}");
                }
                activeWindow = win;
                if (activeWindow != null)
                {
                    activeWindow.Active = true;
                    SDLLog.Debug(LogCategory.APPLICATION, $"Window activated: {activeWindow}");
                }
            }
        }
        private void SetMouseGadget(Gadget? gad)
        {
            if (mouseGadget != gad)
            {
                if (mouseGadget != null)
                {
                    mouseGadget.MouseHover = false;
                }
                mouseGadget = gad;
                if (mouseGadget != null)
                {
                    mouseGadget.MouseHover = true;
                }
            }
        }

        private void SetActiveGadget(Gadget? gad)
        {
            if (activeGadget != gad)
            {
                if (activeGadget != null)
                {
                    activeGadget.Active = false;
                    SDLLog.Debug(LogCategory.APPLICATION, $"Gadget deactivated: {activeGadget}");
                }
                activeGadget = gad;
                if (activeGadget != null)
                {
                    activeGadget.Active = true;
                    SDLLog.Debug(LogCategory.APPLICATION, $"Gadget activated: {activeGadget}");
                }
            }
        }
        private void SetSelectedGadget(Gadget? gad)
        {
            if (selectedGadget != gad)
            {
                if (selectedGadget != null)
                {
                    if (!selectedGadget.ToggleSelect)
                    {
                        selectedGadget.Selected = false;
                        selectedGadget.HandleDeselection();
                        SDLLog.Debug(LogCategory.APPLICATION, $"Gadget deselected: {selectedGadget}");
                    }
                }
                selectedGadget = gad;
                if (selectedGadget != null)
                {
                    selectedGadget.Selected = true;
                    SDLLog.Debug(LogCategory.APPLICATION, $"Gadget selected: {selectedGadget}");
                }
            }
        }

        private void AddSystemGadgets(Window win)
        {
            if ((win.WindowFlags & WindowFlags.DragBar) == WindowFlags.DragBar)
            {
                _ = new Gadget(this, win)
                {
                    LeftEdge = 0,
                    TopEdge = 0,
                    Width = 0,
                    Height = sysGadgetHeight,
                    Flags = GadgetFlags.RelWidth,
                    GadgetType = GadgetType.SysGadget | GadgetType.BoolGadget | GadgetType.WDragging,
                    TransparentBackground = true,
                    GadgetId = SYSGAD_DRAG,
                    Activation = GadgetActivation.TopBorder | GadgetActivation.RightBorder | GadgetActivation.LeftBorder
                };
            }

            if ((win.WindowFlags & WindowFlags.CloseGadget) == WindowFlags.CloseGadget)
            {
                Gadget closeGadget = new Gadget(this, win)
                {
                    LeftEdge = 0,
                    TopEdge = 0,
                    Width = sysGadgetWidth,
                    Height = sysGadgetHeight,
                    GadgetType = GadgetType.SysGadget | GadgetType.BoolGadget | GadgetType.Close,
                    TransparentBackground = true,
                    Icon = Icons.ENTYPO_ICON_CROSS,
                    GadgetId = SYSGAD_CLOSE,
                    Activation = GadgetActivation.RelVerify | GadgetActivation.TopBorder | GadgetActivation.LeftBorder
                };
                closeGadget.GadgetUp += CloseGadget_GadgetUp;
            }

            int gadX = 0;


            if ((win.WindowFlags & WindowFlags.DepthGadget) == WindowFlags.DepthGadget)
            {
                gadX += sysGadgetWidth;
                Gadget depthGadget = new Gadget(this, win)
                {
                    LeftEdge = -gadX,
                    TopEdge = 0,
                    Width = sysGadgetWidth,
                    Height = sysGadgetHeight,
                    Flags = GadgetFlags.RelRight,
                    GadgetType = GadgetType.SysGadget | GadgetType.BoolGadget | GadgetType.WDepth,
                    TransparentBackground = true,
                    Icon = Icons.ENTYPO_ICON_DOCUMENTS,
                    GadgetId = SYSGAD_DEPTH,
                    Activation = GadgetActivation.RelVerify | GadgetActivation.TopBorder | GadgetActivation.RightBorder
                };
                depthGadget.GadgetUp += DepthGadget_GadgetUp;
            }

            if ((win.WindowFlags & WindowFlags.HasZoom) == WindowFlags.HasZoom)
            {
                gadX += sysGadgetWidth;
                Gadget zoomGadget = new Gadget(this, win)
                {
                    LeftEdge = -gadX,
                    TopEdge = 0,
                    Width = sysGadgetWidth,
                    Height = sysGadgetHeight,
                    Flags = GadgetFlags.RelRight,
                    GadgetType = GadgetType.SysGadget | GadgetType.BoolGadget | GadgetType.WZoom,
                    TransparentBackground = true,
                    Icon = Icons.ENTYPO_ICON_RESIZE_FULL_SCREEN,
                    GadgetId = SYSGAD_ZOOM,
                    Activation = GadgetActivation.RelVerify | GadgetActivation.TopBorder | GadgetActivation.RightBorder
                };
                zoomGadget.GadgetUp += ZoomGadget_GadgetUp;
            }

            if ((win.WindowFlags & WindowFlags.SizeGadget) == WindowFlags.SizeGadget)
            {
                if ((win.WindowFlags & WindowFlags.SizeBRight) == WindowFlags.SizeBRight)
                {
                    win.BorderRight = sysGadgetWidth;
                }
                if ((win.WindowFlags & WindowFlags.SizeBBottom) == WindowFlags.SizeBBottom)
                {
                    win.BorderBottom = sysGadgetHeight;
                }
                _ = new Gadget(this, win)
                {
                    LeftEdge = -sysGadgetWidth,
                    TopEdge = -sysGadgetHeight,
                    Width = sysGadgetWidth,
                    Height = sysGadgetHeight,
                    Flags = GadgetFlags.RelRight | GadgetFlags.RelBottom,
                    GadgetType = GadgetType.SysGadget | GadgetType.BoolGadget | GadgetType.Sizing,
                    TransparentBackground = true,
                    Icon = Icons.ENTYPO_ICON_RETWEET,
                    GadgetId = SYSGAD_SIZE,
                    Activation = GadgetActivation.BottomBorder | GadgetActivation.RightBorder
                };
            }
        }

        private void CloseGadget_GadgetUp(object? sender, EventArgs e)
        {
            if (sender is Gadget gad)
            {
                if (gad.Window is Window win)
                {
                    win.RaiseWindowClose();
                }
            }
        }

        private void ZoomGadget_GadgetUp(object? sender, EventArgs e)
        {
            if (sender is Gadget gad)
            {
                if (gad.Window is Window win)
                {
                    win.Zip();
                    gad.Icon = win.Zoomed ? Icons.ENTYPO_ICON_RESIZE_100_PERCENT : Icons.ENTYPO_ICON_RESIZE_FULL_SCREEN;
                }
            }
        }

        private void DepthGadget_GadgetUp(object? sender, EventArgs e)
        {
            if (sender is Gadget gad)
            {
                if (gad.Window is Window win)
                {
                    if (win.IsFrontWindow)
                    {
                        win.ToBack();
                    }
                    else
                    {
                        win.ToFront();
                    }
                }
            }
        }
        private static void CheckAndClear(ref Screen? screen, Screen test)
        {
            if (screen != null && screen == test)
            {
                screen = null;
            }
        }

        private static void CheckAndClear(ref Window? window, Window test)
        {
            if (window != null && window == test)
            {
                window = null;
            }
        }
        private static void CheckAndClear(ref Gadget? gadget, Gadget test)
        {
            if (gadget != null && gadget == test)
            {
                gadget = null;
            }
        }
        private bool CheckWindowDragging(SDLMouseEventArgs e)
        {
            if (activeScreen != null &&
                selectMouseDown &&
                activeGadget != null &&
                activeGadget.SysGadgetType == GadgetType.WDragging &&
                activeGadget.GadgetId == SYSGAD_DRAG &&
                activeWindow != null &&
                activeGadget.Window == activeWindow)
            {
                activeWindow.MoveWindow(diffMouseX, diffMouseY);
                e.Handled = true;
                return true;
            }

            return false;
        }
        private bool CheckWindowSizing(SDLMouseEventArgs e)
        {
            if (activeScreen != null &&
                selectMouseDown &&
                activeGadget != null &&
                activeGadget.SysGadgetType == GadgetType.Sizing &&
                activeGadget.GadgetId == SYSGAD_SIZE &&
                activeWindow != null &&
                activeGadget.Window == activeWindow)
            {
                activeWindow.SizeWindow(diffMouseX, diffMouseY);
                return true;
            }
            return false;
        }
        private bool CheckGadgetMove(SDLMouseEventArgs e)
        {
            bool result = false;
            if (downGadget != null)
            {
                if (downGadget.IsPropGadget)
                {
                    downGadget.HanldePropMouseMove(e.X, e.Y);
                    result |= true;
                }
            }
            if (mouseGadget != null)
            {
                if (mouseGadget.IsPropGadget)
                {
                    mouseGadget.HanldePropMouseMove(e.X, e.Y);
                    result |= true;
                }
            }
            return result;
        }
        private bool CheckGadgetDown(SDLMouseEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                downGadget = mouseGadget;
                if (downGadget != null)
                {
                    SetSelectedGadget(downGadget);
                    if (downGadget.IsPropGadget)
                    {
                        downGadget.HandlePropMouseDown(e.X, e.Y);
                    }
                    if (downGadget.Immediate)
                    {
                        downGadget.RaiseGadgetDown();
                        return true;
                    }
                }
            }
            return false;
        }
        private bool CheckGadgetUp(SDLMouseEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                upGadget = mouseGadget;
                if (upGadget != null && upGadget == downGadget)
                {
                    if (upGadget.IsPropGadget)
                    {
                        upGadget.HandlePropMouseUp(e.X, e.Y);
                    }
                    if (upGadget.RelVeriy)
                    {
                        upGadget.RaiseGadgetUp();
                        return true;
                    }
                }
                SetSelectedGadget(null);
            }
            return false;
        }

        private bool CheckGadgetTimer(double time)
        {
            //SDLLog.Debug(LogCategory.APPLICATION, $"Checking Timer {timerTick} - {(int)time}");
            if (selectedGadget != null && downGadget != null && selectMouseDown)
            {
                if (downGadget.IsPropGadget)
                {
                    downGadget.HandlePropMouseDown(mouseX, mouseY);
                }
                if (downGadget.Immediate)
                {
                    downGadget.RaiseGadgetDown();
                    return true;
                }
            }
            return false;
        }

        private void CheckWindowActivationQueue()
        {
            if (activationWindows.Count > 0)
            {
                Window win = activationWindows.Dequeue();
                SetActiveWindow(win);
            }
        }

        private void CheckGadgetActivationQueue()
        {
            if (activationGadgets.Count > 0)
            {
                Gadget gad = activationGadgets.Peek();
                if (gad.Window == activeWindow)
                {
                    SetActiveGadget(activationGadgets.Dequeue());
                }
            }
        }

        private void CheckTimer(double time)
        {
            if (timerTick == 0)
            {
                timerTick = EventHelper.GetExpirationTime(time, tickIntervall);
            }
            if (EventHelper.HasExpired(time, timerTick))
            {
                CheckGadgetTimer(time);
                timerTick = 0;
            }
        }

        protected internal override void OnMouseMove(SDLMouseEventArgs e)
        {
            UpdateMouse(e.X, e.Y);
            Screen? screen = FindScreen(mouseX, mouseY);
            Window? window = screen?.FindWindow(mouseX, mouseY);
            Gadget? gadget = window?.FindGadget(mouseX, mouseY);
            SetMouseScreen(screen);
            SetMouseWindow(window);
            SetMouseGadget(gadget);
            e.Handled = gadget != null || window != null;
            if (CheckWindowDragging(e) || CheckWindowSizing(e) || CheckGadgetMove(e)) { e.Handled = true; }
        }

        protected internal override void OnMouseButtonDown(SDLMouseEventArgs e)
        {
            UpdateMouse(e.X, e.Y);
            Screen? screen = FindScreen(mouseX, mouseY);
            Window? window = screen?.FindWindow(mouseX, mouseY);
            Gadget? gadget = window?.FindGadget(mouseX, mouseY);
            SetMouseScreen(screen);
            SetMouseWindow(window);
            SetMouseGadget(gadget);
            SetActiveScreen(screen);
            SetActiveWindow(window);
            SetActiveGadget(gadget);
            e.Handled = gadget != null || window != null;
            if (e.Button == MouseButton.Left) { selectMouseDown = true; }
            if (CheckGadgetDown(e)) { e.Handled = true; }
        }

        protected internal override void OnMouseButtonUp(SDLMouseEventArgs e)
        {
            UpdateMouse(e.X, e.Y);
            Screen? screen = FindScreen(mouseX, mouseY);
            Window? window = screen?.FindWindow(mouseX, mouseY);
            Gadget? gadget = window?.FindGadget(mouseX, mouseY);
            SetMouseScreen(screen);
            SetMouseWindow(window);
            SetMouseGadget(gadget);
            e.Handled = gadget != null || window != null;
            if (e.Button == MouseButton.Left) { selectMouseDown = false; }
            if (CheckGadgetUp(e)) { e.Handled = true; }
        }

        private bool UpdateMouse(int x, int y)
        {
            if (mouseX != x || mouseY != y)
            {
                prevMouseX = mouseX;
                prevMouseY = mouseY;
                mouseX = x;
                mouseY = y;
                diffMouseX = mouseX - prevMouseX;
                diffMouseY = mouseY - prevMouseY;
                return true;
            }
            return false;
        }
        private Screen? FindScreen(int mouseX, int mouseY)
        {
            foreach (Screen screen in screens)
            {
                if (screen.Contains(mouseX, mouseY)) { return screen; }
            }
            return null;
        }

        public Screen OpenScreen(bool keepOldScreens = false)
        {
            if (!keepOldScreens)
            {
                while (screens.Count > 0)
                {
                    CloseScreen(screens[0]);
                }
            }
            Screen screen = new Screen(this);
            screen.Width = Width;
            screen.Height = Height;
            screens.Add(screen);
            return screen;
        }

        public void CloseScreen(Screen s)
        {
            if (screens.Remove(s))
            {
                CheckAndClear(ref activeScreen, s);
                CheckAndClear(ref mouseScreen, s);
                s.Close();
            }
        }

        public Window OpenWindow(Screen s,
            int leftEdge = 0,
            int topEdge = 0,
            int width = 256,
            int height = 256,
            string title = "",
            int minWidth = 0,
            int minHeight = 0)
        {
            Window window = new Window(this, s);
            window.LeftEdge = leftEdge;
            window.TopEdge = topEdge;
            window.Width = width;
            window.Height = height;
            window.Title = title;
            window.MinWidth = minWidth;
            window.MinHeight = minHeight;
            window.WindowId = nextWindowID++;
            AddSystemGadgets(window);
            if ((window.WindowFlags & WindowFlags.Activate) != 0)
            {
                activationWindows.Enqueue(window);
            }
            return window;
        }

        public void CloseWindow(Window window)
        {
            Screen s = window.Screen;
            s.RemoveWindow(window);
            CheckAndClear(ref activeWindow, window);
            CheckAndClear(ref mouseWindow, window);
            window.Close();
        }

        public Gadget AddGadget(Window w,
            int leftEdge = 0,
            int topEdge = 0,
            int width = 100,
            int height = 50,
            GadgetFlags flags = GadgetFlags.None,
            GadgetActivation activation = GadgetActivation.Immediate | GadgetActivation.RelVerify,
            GadgetType type = GadgetType.BoolGadget,
            string? text = null,
            bool disabled = false,
            bool selected = false,
            bool toggleSelect = false,
            Action? clickAction = null,
            int gadgetId = -1)
        {
            if (topEdge <= 0) { flags |= GadgetFlags.RelBottom; }
            if (leftEdge <= 0) { flags |= GadgetFlags.RelRight; }
            if (width <= 0) { flags |= GadgetFlags.RelWidth; }
            if (height <= 0) { flags |= GadgetFlags.RelHeight; }
            if (disabled) { flags |= GadgetFlags.Disabled; }

            if (gadgetId < 0) { gadgetId = nextGadgetID++; }
            else { nextGadgetID = Math.Max(nextGadgetID, gadgetId); }

            if (toggleSelect) { activation |= GadgetActivation.ToggleSelect; }
            if (selected) { flags |= GadgetFlags.Selected; }

            Gadget gadget = new Gadget(this, w);
            gadget.LeftEdge = leftEdge;
            gadget.TopEdge = topEdge;
            gadget.Width = width;
            gadget.Height = height;
            gadget.Flags = flags;
            gadget.Activation = activation;
            gadget.GadgetType = type;
            gadget.GadgetId = gadgetId;
            gadget.Text = text;
            if (clickAction != null) { gadget.GadgetUp += (s, e) => { clickAction(); }; }
            return gadget;
        }

        public void RemoveGadget(Window window, Gadget gadget)
        {
        }

        public void WindowToFront(Window window)
        {
            window.ToFront();
        }
        public void WindowToBack(Window window)
        {
            window.ToBack();
        }

        public void ActivateWindow(Window window)
        {
            activationWindows.Enqueue(window);
        }

        public void ActivateGadget(Gadget gadget)
        {
            activationGadgets.Enqueue(gadget);
        }

        public void ModifyProp(Gadget gadget, PropFlags flags, int horizPot, int vertPot, int horizBody, int vertBody)
        {
            gadget.ModifyProp(flags, horizPot, vertPot, horizBody, vertBody);
        }
    }
}
