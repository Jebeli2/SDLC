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
        private int prevMouseX;
        private int prevMouseY;
        private int mouseX;
        private int mouseY;
        private int diffMouseX;
        private int diffMouseY;
        private Screen? activeScreen;
        private Screen? mouseScreen;
        private Window? activeWindow;
        private Window? mouseWindow;
        private Gadget? activeGadget;
        private Gadget? mouseGadget;
        private bool selectMouseDown;

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
            UpdateScreenSize(Width, Height);

        }
        protected internal override void OnWindowSizeChanged(SDLWindowSizeEventArgs e)
        {
            UpdateScreenSize(Width, Height);
        }

        protected internal override void OnWindowResized(SDLWindowSizeEventArgs e)
        {
            UpdateScreenSize(Width, Height);
        }

        protected override void OnWindowUpdate(SDLWindowUpdateEventArgs e)
        {

        }
        protected override void OnWindowPaint(SDLWindowPaintEventArgs e)
        {
            foreach (IGUIScreen screen in screens)
            {
                guiRenderer.RenderScreen(e.Renderer, screen);
            }
        }
        private void UpdateScreenSize(int width, int height)
        {
            foreach (IGUIScreen screen in screens)
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
                    TransparentBackground = true,
                    GadgetId = SYSGAD_DRAG,
                    Activation = GadgetActivation.TopBorder | GadgetActivation.RightBorder | GadgetActivation.LeftBorder
                };
            }

            if ((win.WindowFlags & WindowFlags.CloseGadget) == WindowFlags.CloseGadget)
            {
                _ = new Gadget(this, win)
                {
                    LeftEdge = 0,
                    TopEdge = 0,
                    Width = sysGadgetWidth,
                    Height = sysGadgetHeight,
                    TransparentBackground = true,
                    Icon = Icons.ENTYPO_ICON_CROSS,
                    GadgetId = SYSGAD_CLOSE,
                    Activation = GadgetActivation.TopBorder | GadgetActivation.LeftBorder
                };
            }

            int gadX = 0;


            if ((win.WindowFlags & WindowFlags.DepthGadget) == WindowFlags.DepthGadget)
            {
                gadX += sysGadgetWidth;
                _ = new Gadget(this, win)
                {
                    LeftEdge = -gadX,
                    TopEdge = 0,
                    Width = sysGadgetWidth,
                    Height = sysGadgetHeight,
                    Flags = GadgetFlags.RelRight,
                    TransparentBackground = true,
                    Icon = Icons.ENTYPO_ICON_DOCUMENTS,
                    GadgetId = SYSGAD_DEPTH,
                    Activation = GadgetActivation.TopBorder | GadgetActivation.RightBorder
                };
            }

            if ((win.WindowFlags & WindowFlags.HasZoom) == WindowFlags.HasZoom)
            {
                gadX += sysGadgetWidth;
                _ = new Gadget(this, win)
                {
                    LeftEdge = -gadX,
                    TopEdge = 0,
                    Width = sysGadgetWidth,
                    Height = sysGadgetHeight,
                    Flags = GadgetFlags.RelRight,
                    TransparentBackground = true,
                    Icon = Icons.ENTYPO_ICON_RESIZE_FULL_SCREEN,
                    GadgetId = SYSGAD_ZOOM,
                    Activation = GadgetActivation.TopBorder | GadgetActivation.RightBorder
                };
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
                    TransparentBackground = true,
                    Icon = Icons.ENTYPO_ICON_RETWEET,
                    GadgetId = SYSGAD_SIZE,
                    Activation = GadgetActivation.BottomBorder | GadgetActivation.RightBorder
                };
            }
        }

        private bool CheckWindowDragging(SDLMouseEventArgs e)
        {
            if (activeScreen != null &&
                selectMouseDown &&
                activeGadget != null &&
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
                activeGadget.GadgetId == SYSGAD_SIZE &&
                activeWindow != null &&
                activeGadget.Window == activeWindow)
            {
                activeWindow.SizeWindow(diffMouseX, diffMouseY);
                return true;
            }
            return false;
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
            if (CheckWindowDragging(e) || CheckWindowSizing(e))
            {
                e.Handled = true;
            }
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

        public IGUIScreen OpenScreen()
        {
            Screen screen = new Screen(this);
            screen.Width = Width;
            screen.Height = Height;
            screens.Add(screen);
            return screen;
        }

        public void CloseScreen(IGUIScreen screen)
        {
            if (screen is Screen s)
            {
                if (screens.Remove(s))
                {

                }
            }
            else
            {
                throw new InvalidOperationException("Invalid Screen");
            }
        }

        public IGUIWindow OpenWindow(IGUIScreen screen,
            int leftEdge = 0,
            int topEdge = 0,
            int width = 256,
            int height = 256,
            string title = "",
            int minWidth = 0,
            int minHeight = 0)
        {
            if (screen is Screen s)
            {
                Window window = new Window(this, s);
                window.LeftEdge = leftEdge;
                window.TopEdge = topEdge;
                window.Width = width;
                window.Height = height;
                window.Title = title;
                window.MinWidth = minWidth;
                window.MinHeight = minHeight;
                AddSystemGadgets(window);
                return window;
            }
            else
            {
                throw new InvalidOperationException("Invalid Screen");
            }
        }

        public void CloseWindow(IGUIWindow window)
        {
            if (window is Window w)
            {

            }
            else
            {
                throw new InvalidOperationException("Invalid Window");
            }
        }

        public IGUIGadget AddGadget(IGUIWindow window, int leftEdge = 0, int topEdge = 0, int width = 100, int height = 50,
            GadgetFlags flags = GadgetFlags.None,
            GadgetActivation activation = GadgetActivation.Immediate | GadgetActivation.RelVerify,
            string? text = null,
            bool disabled = false,
            int gadgetId = -1)
        {
            if (window is Window w)
            {
                if (topEdge <= 0) { flags |= GadgetFlags.RelBottom; }
                if (leftEdge <= 0) { flags |= GadgetFlags.RelRight; }
                if (width <= 0) { flags |= GadgetFlags.RelWidth; }
                if (height <= 0) { flags |= GadgetFlags.RelHeight; }
                if (disabled) { flags |= GadgetFlags.Disabled; }

                if (gadgetId < 0) { gadgetId = nextGadgetID++; }
                else { nextGadgetID = Math.Max(nextGadgetID, gadgetId); }

                Gadget gadget = new Gadget(this, w);
                gadget.LeftEdge = leftEdge;
                gadget.TopEdge = topEdge;
                gadget.Width = width;
                gadget.Height = height;
                gadget.Flags = flags;
                gadget.Activation = activation;
                gadget.GadgetId = gadgetId;
                gadget.Text = text;
                return gadget;
            }
            else
            {
                throw new InvalidOperationException("Invalid Window");
            }
        }

        public void RemoveGadget(IGUIWindow window, IGUIGadget gadget)
        {
            if (window is Window w)
            {
                if (gadget is Gadget g)
                {

                }
                else
                {
                    throw new InvalidOperationException("Invalid Gadget");
                }
            }
            else
            {
                throw new InvalidOperationException("Invalid Window");
            }
        }
    }
}
