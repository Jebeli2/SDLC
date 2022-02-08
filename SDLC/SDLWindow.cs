namespace SDLC
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Numerics;
    using System.Runtime.InteropServices;

    public class SDLWindow : IWindow, IDisposable
    {

        public const int SDL_WINDOWPOS_UNDEFINED_MASK = 0x1FFF0000;
        public const int SDL_WINDOWPOS_CENTERED_MASK = 0x2FFF0000;
        public const int SDL_WINDOWPOS_UNDEFINED = 0x1FFF0000;
        public const int SDL_WINDOWPOS_CENTERED = 0x2FFF0000;


        private IntPtr handle;
        private readonly SDLRenderer renderer;
        private bool disposedValue;
        private int windowId;
        private string? title;
        private bool visible;
        private bool resizeable;
        private bool alwaysOnTop;
        private bool borderless;
        private bool skipTaskbar;
        private bool fullScreen;
        private int x;
        private int y;
        private int width;
        private int height;
        private int backBufferWidth;
        private int backBufferHeight;
        private RendererSizeMode sizeMode;
        private MouseButton lastButton;
        private KeyButtonState lastState;
        private bool showFPS;
        private float fpsPosX;
        private float fpsPosY;
        private string driver;
        private bool mouseGrab;
        private bool keyboardGrab;
        private WindowCloseOperation closeOperation;
        private FullScreenMode fullScreenMode;
        private int oldX;
        private int oldY;
        private int oldWidth;
        private int oldHeight;
        private readonly List<SDLApplet> applets = new();
        private IScreen screen;

        private readonly EventHandlerList eventHandlerList = new();
        private static readonly object windowLoadEventKey = new();
        private static readonly object windowShownEventKey = new();
        private static readonly object windowHiddenEventKey = new();
        private static readonly object windowExposedEventKey = new();
        private static readonly object windowMovedEventKey = new();
        private static readonly object windowResizedEventKey = new();
        private static readonly object windowSizeChangedEventKey = new();
        private static readonly object windowMinimizedEventKey = new();
        private static readonly object windowMaximizedEventKey = new();
        private static readonly object windowRestoredEventKey = new();
        private static readonly object windowEnterEventKey = new();
        private static readonly object windowLeaveEventKey = new();
        private static readonly object windowFocusGainedEventKey = new();
        private static readonly object windowFocusLostEventKey = new();
        private static readonly object windowCloseEventKey = new();
        private static readonly object windowTakeFocusEventKey = new();
        private static readonly object mouseButtonDownEventKey = new();
        private static readonly object mouseButtonUpEventKey = new();
        private static readonly object mouseMoveEventKey = new();
        private static readonly object mouseWheelEventKey = new();

        private static readonly object windowDisplayChangedEventKey = new();
        private static readonly object keyDownEventKey = new();
        private static readonly object keyUpEventKey = new();
        private static readonly object textInputEventKey = new();

        private static readonly object controllerButtonUpEventKey = new();
        private static readonly object controllerButtonDownEventKey = new();
        private static readonly object controllerAxisEventKey = new();
        private static readonly object controllerTouchpadDownEventKey = new();
        private static readonly object controllerTouchpadUpEventKey = new();
        private static readonly object controllerTouchpadMotionEventKey = new();

        private static readonly object touchFingerUpEventKey = new();
        private static readonly object touchFingerDownEventKey = new();
        private static readonly object touchFingerMotionEventKey = new();

        private static readonly object windowPaintEventKey = new();
        private static readonly object windowUpdateEventKey = new();

        public SDLWindow(string? title = null)
            : this(new NoScreen(), title)
        {

        }
        public SDLWindow(IScreen screen, string? title = null)
        {
            this.screen = screen;
            this.title = title;
            driver = "direct3d11";
            visible = true;
            x = SDL_WINDOWPOS_UNDEFINED;
            y = SDL_WINDOWPOS_UNDEFINED;
            SetConfiguration(this.screen.GetConfiguration());
            backBufferWidth = width;
            backBufferHeight = height;
            windowId = -1;
            renderer = new SDLRenderer(this);
        }
        ~SDLWindow()
        {
            Dispose(disposing: false);
        }

        private void SetConfiguration(Configuration config)
        {
            driver = config.Driver;
            title = config.WindowTitle;
            width = config.WindowWidth;
            height = config.WindowHeight;
            resizeable = config.Resizeable;
            alwaysOnTop = config.AlwaysOnTop;
            borderless = config.Borderless;
            skipTaskbar = config.SkipTaskbar;
            fullScreen = config.FullScreen;
            showFPS = config.ShowFPS;
            fpsPosX = config.FPSPosX;
            fpsPosY = config.FPSPosY;
            SDLApplication.MaxFramesPerSecond = config.MaxFramesPerSecond;
        }

        private void ApplyConfiguration(Configuration config)
        {

        }

        public event EventHandler WindowShown
        {
            add => eventHandlerList.AddHandler(windowShownEventKey, value); remove => eventHandlerList.RemoveHandler(windowShownEventKey, value);
        }

        public event EventHandler WindowHidden
        {
            add => eventHandlerList.AddHandler(windowHiddenEventKey, value); remove => eventHandlerList.RemoveHandler(windowHiddenEventKey, value);
        }

        public event EventHandler WindowExposed
        {
            add => eventHandlerList.AddHandler(windowExposedEventKey, value); remove => eventHandlerList.RemoveHandler(windowExposedEventKey, value);
        }
        public event EventHandler WindowMinimized
        {
            add => eventHandlerList.AddHandler(windowMinimizedEventKey, value); remove => eventHandlerList.RemoveHandler(windowMinimizedEventKey, value);
        }

        public event EventHandler WindowMaxmimized
        {
            add => eventHandlerList.AddHandler(windowMaximizedEventKey, value); remove => eventHandlerList.RemoveHandler(windowMaximizedEventKey, value);
        }

        public event EventHandler WindowRestored
        {
            add => eventHandlerList.AddHandler(windowRestoredEventKey, value); remove => eventHandlerList.RemoveHandler(windowRestoredEventKey, value);
        }

        public event EventHandler WindowEnter
        {
            add => eventHandlerList.AddHandler(windowEnterEventKey, value); remove => eventHandlerList.RemoveHandler(windowEnterEventKey, value);
        }

        public event EventHandler WindowLeave
        {
            add => eventHandlerList.AddHandler(windowLeaveEventKey, value); remove => eventHandlerList.RemoveHandler(windowLeaveEventKey, value);
        }

        public event EventHandler WindowFocusGained
        {
            add => eventHandlerList.AddHandler(windowFocusGainedEventKey, value); remove => eventHandlerList.RemoveHandler(windowFocusGainedEventKey, value);
        }

        public event EventHandler WindowFocusLost
        {
            add => eventHandlerList.AddHandler(windowFocusLostEventKey, value); remove => eventHandlerList.RemoveHandler(windowFocusLostEventKey, value);
        }

        public event EventHandler WindowClose
        {
            add => eventHandlerList.AddHandler(windowCloseEventKey, value); remove => eventHandlerList.RemoveHandler(windowCloseEventKey, value);
        }

        public event EventHandler WindowTakeFocus
        {
            add => eventHandlerList.AddHandler(windowTakeFocusEventKey, value); remove => eventHandlerList.RemoveHandler(windowTakeFocusEventKey, value);
        }

        public event SDLWindowPositionEventHandler WindowMoved
        {
            add => eventHandlerList.AddHandler(windowMovedEventKey, value); remove => eventHandlerList.RemoveHandler(windowMovedEventKey, value);
        }

        public event SDLWindowSizeEventHandler WindowResized
        {
            add => eventHandlerList.AddHandler(windowResizedEventKey, value); remove => eventHandlerList.RemoveHandler(windowResizedEventKey, value);
        }

        public event SDLWindowSizeEventHandler WindowSizeChanged
        {
            add => eventHandlerList.AddHandler(windowSizeChangedEventKey, value); remove => eventHandlerList.RemoveHandler(windowSizeChangedEventKey, value);
        }
        public event SDLWindowUpdateEventHandler WindowUpdate
        {
            add => eventHandlerList.AddHandler(windowUpdateEventKey, value); remove => eventHandlerList.RemoveHandler(windowUpdateEventKey, value);
        }
        public event SDLWindowPaintEventHandler WindowPaint
        {
            add => eventHandlerList.AddHandler(windowPaintEventKey, value); remove => eventHandlerList.RemoveHandler(windowPaintEventKey, value);
        }
        public event SDLWindowLoadEventHandler WindowLoad
        {
            add => eventHandlerList.AddHandler(windowLoadEventKey, value); remove => eventHandlerList.RemoveHandler(windowLoadEventKey, value);
        }
        public event SDLMouseEventHandler MouseButtonDown
        {
            add => eventHandlerList.AddHandler(mouseButtonDownEventKey, value); remove => eventHandlerList.RemoveHandler(mouseButtonDownEventKey, value);
        }
        public event SDLMouseEventHandler MouseButtonUp
        {
            add => eventHandlerList.AddHandler(mouseButtonUpEventKey, value); remove => eventHandlerList.RemoveHandler(mouseButtonUpEventKey, value);
        }
        public event SDLMouseEventHandler MouseMove
        {
            add => eventHandlerList.AddHandler(mouseMoveEventKey, value); remove => eventHandlerList.RemoveHandler(mouseMoveEventKey, value);
        }
        public event SDLMouseWheelEventHandler MouseWheel
        {
            add => eventHandlerList.AddHandler(mouseWheelEventKey, value); remove => eventHandlerList.RemoveHandler(mouseWheelEventKey, value);
        }
        public event SDLKeyEventHandler KeyDown
        {
            add => eventHandlerList.AddHandler(keyDownEventKey, value); remove => eventHandlerList.RemoveHandler(keyDownEventKey, value);
        }
        public event SDLKeyEventHandler KeyUp
        {
            add => eventHandlerList.AddHandler(keyUpEventKey, value); remove => eventHandlerList.RemoveHandler(keyUpEventKey, value);
        }
        public event SDLTextInputEventHandler TextInput
        {
            add => eventHandlerList.AddHandler(textInputEventKey, value); remove => eventHandlerList.RemoveHandler(textInputEventKey, value);
        }
        public event SDLControllerButtonEventHandler ControllerButtonDown
        {
            add => eventHandlerList.AddHandler(controllerButtonDownEventKey, value); remove => eventHandlerList.RemoveHandler(controllerButtonDownEventKey, value);
        }
        public event SDLControllerButtonEventHandler ControllerButtonUp
        {
            add => eventHandlerList.AddHandler(controllerButtonUpEventKey, value); remove => eventHandlerList.RemoveHandler(controllerButtonUpEventKey, value);
        }
        public event SDLControllerAxisEventHandler ControllerAxis
        {
            add => eventHandlerList.AddHandler(controllerAxisEventKey, value); remove => eventHandlerList.RemoveHandler(controllerAxisEventKey, value);
        }
        public event SDLControllerTouchpadEventHandler ControllerTouchpadDown
        {
            add => eventHandlerList.AddHandler(controllerTouchpadDownEventKey, value); remove => eventHandlerList.RemoveHandler(controllerTouchpadDownEventKey, value);
        }
        public event SDLControllerTouchpadEventHandler ControllerTouchpadUp
        {
            add => eventHandlerList.AddHandler(controllerTouchpadUpEventKey, value); remove => eventHandlerList.RemoveHandler(controllerTouchpadUpEventKey, value);
        }
        public event SDLControllerTouchpadEventHandler ControllerTouchpadMove
        {
            add => eventHandlerList.AddHandler(controllerTouchpadMotionEventKey, value); remove => eventHandlerList.RemoveHandler(controllerTouchpadMotionEventKey, value);
        }


        public int WindowId => windowId;
        public IntPtr Handle => handle;
        public IRenderer Renderer => renderer;
        public IScreen Screen
        {
            get => screen;
            set
            {
                if (screen != value)
                {
                    screen.Hide(this);
                    screen = value;
                    screen.Show(this);
                }
            }
        }

        public WindowCloseOperation CloseOperation
        {
            get => closeOperation;
            set
            {
                if (closeOperation != value)
                {
                    closeOperation = value;
                }
            }
        }
        public string Driver
        {
            get => driver;
            set
            {
                if (driver != value)
                {
                    driver = value;
                }
            }
        }

        public bool ShowFPS
        {
            get => showFPS;
            set
            {
                if (showFPS != value)
                {
                    showFPS = value;
                }
            }
        }

        public float FPSPosX
        {
            get => fpsPosX;
            set
            {
                if (fpsPosX != value)
                {
                    fpsPosX = value;
                }
            }
        }

        public float FPSPosY
        {
            get => fpsPosY;
            set
            {
                if (fpsPosY != value)
                {
                    fpsPosY = value;
                }
            }
        }

        public bool MouseGrab
        {
            get => mouseGrab;
            set
            {
                if (mouseGrab != value)
                {
                    mouseGrab = value;
                    if (HandleCreated)
                    {
                        SDL_SetWindowMouseGrab(handle, mouseGrab);
                    }
                }
            }
        }

        public bool KeyboardGrab
        {
            get => keyboardGrab;
            set
            {
                if (keyboardGrab != value)
                {
                    keyboardGrab = value;
                    if (HandleCreated)
                    {
                        SDL_SetWindowKeyboardGrab(handle, keyboardGrab);
                    }
                }
            }
        }

        public string? Title
        {
            get => title;
            set
            {
                if (title != value)
                {
                    title = value;
                    if (HandleCreated)
                    {
                        if (title != null)
                        {
                            SDL_SetWindowTitle(handle, title);
                        }
                        else
                        {
                            SDL_SetWindowTitle(handle, IntPtr.Zero);
                        }
                    }
                }
            }
        }

        public bool Visible
        {
            get => visible;
            set
            {
                if (visible != value)
                {
                    visible = value;
                    if (HandleCreated)
                    {
                        if (visible)
                        {
                            Show();
                        }
                        else
                        {
                            Hide();
                        }
                    }
                }
            }
        }

        public bool Resizeable
        {
            get => resizeable;
            set
            {
                if (resizeable != value)
                {
                    resizeable = value;
                    if (HandleCreated)
                    {
                        SDL_SetWindowResizable(handle, resizeable);
                    }
                }
            }
        }

        public bool AlwaysOnTop
        {
            get => alwaysOnTop;
            set
            {
                if (alwaysOnTop != value)
                {
                    alwaysOnTop = value;
                    if (HandleCreated)
                    {
                        SDL_SetWindowAlwaysOnTop(handle, alwaysOnTop);
                    }
                }
            }
        }

        public bool Borderless
        {
            get => borderless;
            set
            {
                if (borderless != value)
                {
                    borderless = value;
                    if (HandleCreated)
                    {
                        SDL_SetWindowBordered(handle, !borderless);
                    }
                }
            }
        }

        public bool SkipTaskbar
        {
            get => skipTaskbar;
            set
            {
                if (skipTaskbar != value)
                {
                    skipTaskbar = value;
                    if (HandleCreated)
                    {
                        SDLLog.Warn($"SkipTaskbar can only be set before the window handle is created");
                    }
                }
            }
        }

        public bool FullScreen
        {
            get => fullScreen;
            set
            {
                if (fullScreen != value)
                {
                    fullScreen = value;
                    if (HandleCreated)
                    {
                        if (fullScreen)
                        {
                            GoFullScreen();
                        }
                        else
                        {
                            GoWindowed();
                        }
                    }
                }
            }
        }

        public FullScreenMode FullScreenMode
        {
            get => fullScreenMode;
            set
            {
                if (fullScreenMode != value)
                {
                    fullScreenMode = value;
                }
            }
        }
        public int X
        {
            get => x;
            set
            {
                if (x != value)
                {
                    SetPosition(value, y);
                }
            }
        }

        public int Y
        {
            get => y;
            set
            {
                if (y != value)
                {
                    SetPosition(x, value);
                }
            }
        }

        public int Width
        {
            get => width;
            set
            {
                if (width != value)
                {
                    SetSize(value, height);
                }
            }
        }

        public int Height
        {
            get => height;
            set
            {
                if (height != value)
                {
                    SetSize(width, value);
                }
            }
        }

        public int BackBufferWidth
        {
            get => backBufferWidth;
            set
            {
                if (backBufferWidth != value)
                {
                    SetBackBufferSize(value, backBufferHeight);
                }
            }
        }

        public int BackBufferHeight
        {
            get => backBufferHeight;
            set
            {
                if (backBufferHeight != value)
                {
                    SetBackBufferSize(backBufferWidth, value);
                }
            }
        }

        public RendererSizeMode SizeMode
        {
            get => sizeMode;
            set
            {
                if (sizeMode != value)
                {
                    sizeMode = value;
                    renderer.SizeMode = value;
                }
            }
        }

        public bool HandleCreated => handle != IntPtr.Zero;
        public bool Disposed => disposedValue;

        public void Show()
        {
            visible = true;
            if (HandleCreated)
            {
                SDL_ShowWindow(handle);
            }
            else
            {
                CreateHandle();
            }
            screen.Show(this);
        }

        public void Hide()
        {
            visible = false;
            if (HandleCreated)
            {
                SDL_HideWindow(handle);
            }
            screen.Hide(this);
        }

        public void Close()
        {
            screen.Shutdown(this);
            Dispose();
        }

        public void SetPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
            if (HandleCreated)
            {
                SDL_SetWindowPosition(handle, x, y);
            }
        }

        public void SetSize(int width, int height)
        {
            this.width = width;
            this.height = height;
            if (HandleCreated)
            {
                SDL_SetWindowSize(handle, width, height);
            }
        }

        public void SetBackBufferSize(int w, int h)
        {
            backBufferWidth = w;
            backBufferHeight = h;
            renderer.SetBackBufferSize(backBufferWidth, backBufferHeight);
        }

        public void AddApplet(SDLApplet applet)
        {
            if (!applets.Contains(applet))
            {
                applets.Add(applet);
                InitApplet(applet);
                applet.Installed = true;
            }
        }

        public void RemoveApplet(SDLApplet applet)
        {
            applets.Remove(applet);
            applet.Installed = false;
        }

        private void InitApplet(SDLApplet applet)
        {
            if (!applet.Initialized)
            {
                applet.InternalOnLoad(new SDLWindowLoadEventArgs(renderer));
                applet.Initialized = true;
                applet.InternalOnShown(EventArgs.Empty);
            }
        }
        private void ForEachEnabledApplet(Action<SDLApplet> action)
        {
            foreach (SDLApplet applet in applets)
            {
                if (applet.Enabled)
                {
                    action(applet);
                }
            }
        }

        private void GoFullScreen()
        {
            switch (fullScreenMode)
            {
                case FullScreenMode.Desktop:
                    GoDesktopFullScreen();
                    break;
                case FullScreenMode.FullSize:
                    GoFullSizeFullScreen();
                    break;
                case FullScreenMode.MultiMonitor:
                    GoMultiMonitorFullScreen();
                    break;
            }
        }

        private void GoWindowed()
        {
            if (fullScreenMode == FullScreenMode.Desktop)
            {
                _ = SDL_SetWindowFullscreen(Handle, 0);
                SDLLog.Info($"Entered Windowed Mode");
            }
            else
            {
                SDL_SetWindowSize(handle, oldWidth, oldHeight);
                SDL_SetWindowPosition(handle, oldX, oldY);
                SDL_SetWindowBordered(handle, !borderless);
                SDL_SetWindowResizable(handle, resizeable);
                if (title != null) { SDL_SetWindowTitle(handle, title); }
                SDL_SetWindowAlwaysOnTop(handle, alwaysOnTop);
                SDLLog.Info($"Entered Windowed Mode");
            }
        }

        private void GoFullSizeFullScreen()
        {
            SDL_GetWindowPosition(handle, out oldX, out oldY);
            SDL_GetWindowSize(handle, out oldWidth, out oldHeight);
            int index = SDL_GetWindowDisplayIndex(handle);
            _ = SDL_GetDisplayBounds(index, out Rectangle bounds);
            SDL_SetWindowBordered(handle, false);
            SDL_SetWindowResizable(handle, false);
            SDL_SetWindowTitle(handle, IntPtr.Zero);
            SDL_SetWindowAlwaysOnTop(handle, true);
            SDL_SetWindowSize(handle, bounds.Width, bounds.Height);
            SDL_SetWindowPosition(handle, bounds.X, bounds.Y);
            SDLLog.Info($"Entered Full Size Full Screen Mode");
        }

        private void GoMultiMonitorFullScreen()
        {
            SDL_GetWindowPosition(handle, out oldX, out oldY);
            SDL_GetWindowSize(handle, out oldWidth, out oldHeight);
            _ = SDL_GetDisplayBounds(0, out Rectangle bounds);
            int numDisplays = SDL_GetNumVideoDisplays();
            for (int index = 1; index < numDisplays; index++)
            {
                _ = SDL_GetDisplayBounds(index, out Rectangle otherBounds);
                if (otherBounds.Height == bounds.Height)
                {
                    bounds = Rectangle.Union(bounds, otherBounds);

                }
                else
                {
                    break;
                }
            }
            SDL_SetWindowBordered(handle, false);
            SDL_SetWindowResizable(handle, false);
            SDL_SetWindowTitle(handle, IntPtr.Zero);
            SDL_SetWindowAlwaysOnTop(handle, true);
            SDL_SetWindowSize(handle, bounds.Width, bounds.Height);
            SDL_SetWindowPosition(handle, bounds.X, bounds.Y);
            SDLLog.Info($"Entered Multi Monitor Full Screen Mode");
        }

        private void GoDesktopFullScreen()
        {
            SDL_GetWindowPosition(handle, out oldX, out oldY);
            SDL_GetWindowSize(handle, out oldWidth, out oldHeight);
            _ = SDL_SetWindowFullscreen(Handle, (uint)SDL_WindowFlags.FULLSCREEN_DESKTOP);
            SDLLog.Info($"Entered Desktop Full Screen Mode");
        }

        private void ScaleMouse(ref int x, ref int y)
        {
            if (sizeMode == RendererSizeMode.Window) return;
            double scaleX = width;
            scaleX /= backBufferWidth;
            x = (int)(x / scaleX);
            double scaleY = height;
            scaleY /= backBufferHeight;
            y = (int)(y / scaleY);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }
                renderer.Dispose();
                if (handle != IntPtr.Zero)
                {
                    SDL_DestroyWindow(handle);
                    handle = IntPtr.Zero;
                    SDLLog.Info($"SDLWindow {windowId} destroyed");
                }
                disposedValue = true;
            }
        }

        protected virtual void OnWindowShown(EventArgs e)
        {
            ForEachEnabledApplet(a => a.InternalOnShown(e));
            ((EventHandler?)eventHandlerList[windowShownEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowHidden(EventArgs e)
        {
            ForEachEnabledApplet(a => a.InternalOnHidden(e));
            ((EventHandler?)eventHandlerList[windowHiddenEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowExposed(EventArgs e) { ((EventHandler?)eventHandlerList[windowExposedEventKey])?.Invoke(this, e); }
        protected virtual void OnWindowMinimized(EventArgs e) { ((EventHandler?)eventHandlerList[windowMinimizedEventKey])?.Invoke(this, e); }
        protected virtual void OnWindowMaximized(EventArgs e) { ((EventHandler?)eventHandlerList[windowMaximizedEventKey])?.Invoke(this, e); }
        protected virtual void OnWindowRestored(EventArgs e) { ((EventHandler?)eventHandlerList[windowRestoredEventKey])?.Invoke(this, e); }
        protected virtual void OnWindowEnter(EventArgs e) { ((EventHandler?)eventHandlerList[windowEnterEventKey])?.Invoke(this, e); }
        protected virtual void OnWindowLeave(EventArgs e) { ((EventHandler?)eventHandlerList[windowLeaveEventKey])?.Invoke(this, e); }
        protected virtual void OnWindowFocusGained(EventArgs e) { ((EventHandler?)eventHandlerList[windowFocusGainedEventKey])?.Invoke(this, e); }
        protected virtual void OnWindowFocusLost(EventArgs e) { ((EventHandler?)eventHandlerList[windowFocusLostEventKey])?.Invoke(this, e); }
        protected virtual void OnWindowClose(EventArgs e)
        {
            ForEachEnabledApplet(a => a.InternalOnClose(e));
            ((EventHandler?)eventHandlerList[windowCloseEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowTakeFocus(EventArgs e) { ((EventHandler?)eventHandlerList[windowTakeFocusEventKey])?.Invoke(this, e); }
        protected virtual void OnWindowMoved(SDLWindowPositionEventArgs e) { ((SDLWindowPositionEventHandler?)eventHandlerList[windowMovedEventKey])?.Invoke(this, e); }
        protected virtual void OnWindowResized(SDLWindowSizeEventArgs e)
        {
            ForEachEnabledApplet(a => a.InternalOnSizeChanged(e));
            ((SDLWindowSizeEventHandler?)eventHandlerList[windowResizedEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowSizeChanged(SDLWindowSizeEventArgs e)
        {
            ForEachEnabledApplet(a => a.InternalOnSizeChanged(e));
            ((SDLWindowSizeEventHandler?)eventHandlerList[windowSizeChangedEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowUpdate(SDLWindowUpdateEventArgs e)
        {
            ForEachEnabledApplet(a => a.InternalOnUpdate(e));
            ((SDLWindowUpdateEventHandler?)eventHandlerList[windowUpdateEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowPaint(SDLWindowPaintEventArgs e)
        {
            ForEachEnabledApplet(a => a.InternalOnPaint(e));
            ((SDLWindowPaintEventHandler?)eventHandlerList[windowPaintEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowLoad(SDLWindowLoadEventArgs e)
        {
            ForEachEnabledApplet(a => a.InternalOnLoad(e));
            ((SDLWindowLoadEventHandler?)eventHandlerList[windowLoadEventKey])?.Invoke(this, e);
        }
        protected virtual void OnMouseButtonDown(SDLMouseEventArgs e)
        {
            ForEachEnabledApplet(a => a.InternalOnMouseButtonDown(e));
            ((SDLMouseEventHandler?)eventHandlerList[mouseButtonDownEventKey])?.Invoke(this, e);
        }
        protected virtual void OnMouseButtonUp(SDLMouseEventArgs e)
        {
            ForEachEnabledApplet(a => a.InternalOnMouseButtonUp(e));
            ((SDLMouseEventHandler?)eventHandlerList[mouseButtonUpEventKey])?.Invoke(this, e);
        }
        protected virtual void OnMouseMove(SDLMouseEventArgs e)
        {
            ForEachEnabledApplet(a => a.InternalOnMouseMove(e));
            ((SDLMouseEventHandler?)eventHandlerList[mouseMoveEventKey])?.Invoke(this, e);
        }
        protected virtual void OnMouseWheel(SDLMouseWheelEventArgs e)
        {
            ForEachEnabledApplet(a => a.InternalOnMouseWheel(e));
            ((SDLMouseWheelEventHandler?)eventHandlerList[mouseWheelEventKey])?.Invoke(this, e);
        }
        protected virtual void OnKeyDown(SDLKeyEventArgs e)
        {
            ForEachEnabledApplet(a => a.InternalOnKeyDown(e));
            ((SDLKeyEventHandler?)eventHandlerList[keyDownEventKey])?.Invoke(this, e);
        }
        protected virtual void OnKeyUp(SDLKeyEventArgs e)
        {
            ForEachEnabledApplet(a => a.InternalOnKeyUp(e));
            ((SDLKeyEventHandler?)eventHandlerList[keyUpEventKey])?.Invoke(this, e);
        }
        protected virtual void OnTextInput(SDLTextInputEventArgs e)
        {
            ForEachEnabledApplet(a => a.InternalOnTextInput(e));
            ((SDLTextInputEventHandler?)eventHandlerList[textInputEventKey])?.Invoke(this, e);
        }
        protected virtual void OnControllerButtonDown(SDLControllerButtonEventArgs e) { ((SDLControllerButtonEventHandler?)eventHandlerList[controllerButtonDownEventKey])?.Invoke(this, e); }
        protected virtual void OnControllerButtonUp(SDLControllerButtonEventArgs e) { ((SDLControllerButtonEventHandler?)eventHandlerList[controllerButtonUpEventKey])?.Invoke(this, e); }
        protected virtual void OnControllerAxis(SDLControllerAxisEventArgs e) { ((SDLControllerAxisEventHandler?)eventHandlerList[controllerAxisEventKey])?.Invoke(this, e); }
        protected virtual void OnControllerTouchpadDown(SDLControllerTouchpadEventArgs e) { ((SDLControllerTouchpadEventHandler?)eventHandlerList[controllerTouchpadDownEventKey])?.Invoke(this, e); }
        protected virtual void OnControllerTouchpadUp(SDLControllerTouchpadEventArgs e) { ((SDLControllerTouchpadEventHandler?)eventHandlerList[controllerTouchpadUpEventKey])?.Invoke(this, e); }
        protected virtual void OnControllerTouchpadMove(SDLControllerTouchpadEventArgs e) { ((SDLControllerTouchpadEventHandler?)eventHandlerList[controllerTouchpadMotionEventKey])?.Invoke(this, e); }
        internal void RaiseWindowShown()
        {
            SDLLog.Debug($"Window {windowId} Shown");
            OnWindowShown(EventArgs.Empty);
        }
        internal void RaiseWindowHidden()
        {
            SDLLog.Debug($"Window {windowId} Hidden");
            OnWindowHidden(EventArgs.Empty);
        }
        internal void RaiseWindowExposed()
        {
            SDLLog.Debug($"Window {windowId} Exposed");
            OnWindowExposed(EventArgs.Empty);
        }
        internal void RaiseWindowMinimized()
        {
            SDLLog.Debug($"Window {windowId} Minimized");
            OnWindowMinimized(EventArgs.Empty);
        }
        internal void RaiseWindowMaximized()
        {
            SDLLog.Debug($"Window {windowId} Maximized");
            OnWindowMaximized(EventArgs.Empty);
        }
        internal void RaiseWindowRestored()
        {
            SDLLog.Debug($"Window {windowId} Restored");
            OnWindowRestored(EventArgs.Empty);
        }
        internal void RaiseWindowEnter()
        {
            SDLLog.Debug($"Window {windowId} Enter");
            OnWindowEnter(EventArgs.Empty);
        }
        internal void RaiseWindowLeave()
        {
            SDLLog.Debug($"Window {windowId} Leave");
            OnWindowLeave(EventArgs.Empty);
        }
        internal void RaiseWindowFocusGained()
        {
            SDLLog.Debug($"Window {windowId} Focus Gained");
            OnWindowFocusGained(EventArgs.Empty);
        }
        internal void RaiseWindowFocusLost()
        {
            SDLLog.Debug($"Window {windowId} Focus Lost");
            OnWindowFocusLost(EventArgs.Empty);
        }
        internal void RaiseWindowClose()
        {
            SDLLog.Debug($"Window {windowId} Close");
            OnWindowClose(EventArgs.Empty);
            switch (closeOperation)
            {
                case WindowCloseOperation.Close:
                    Close();
                    break;
                case WindowCloseOperation.Exit:
                    SDLApplication.Exit();
                    break;
                case WindowCloseOperation.DoNothing:
                    break;
            }
        }
        internal void RaiseWindowTakeFocus()
        {
            SDLLog.Debug($"Window {windowId} Take Focus");
            OnWindowTakeFocus(EventArgs.Empty);
        }
        internal void RaiseWindowMoved(int x, int y)
        {
            SDLLog.Debug($"Window {windowId} Moved {x} {y}");
            this.x = x;
            this.y = y;
            OnWindowMoved(new SDLWindowPositionEventArgs(x, y));
        }

        internal void RaiseWindowResized(int width, int height)
        {
            SDLLog.Debug($"Window {windowId} Resized {width} {height}");
            this.width = width;
            this.height = height;
            renderer.WindowResized(width, height);
            screen.Resized(this, width, height);
            OnWindowResized(new SDLWindowSizeEventArgs(width, height));
        }
        internal void RaiseWindowSizeChanged(int width, int height)
        {
            SDLLog.Debug($"Window {windowId} Size Changed {width} {height}");
            this.width = width;
            this.height = height;
            renderer.WindowResized(width, height);
            screen.Resized(this, width, height);
            OnWindowSizeChanged(new SDLWindowSizeEventArgs(width, height));
        }

        internal void RaiseWindowLoad()
        {
            SDLLog.Debug($"Window {windowId} Load");
            OnWindowLoad(new SDLWindowLoadEventArgs(renderer));
        }

        internal void RaiseWindowUpdate(double totalTime, double elapsedTime)
        {
            //SDLLog.Info($"Window {windowId} Update {totalTime} {elapsedTime}");
            screen.Update(renderer, totalTime, elapsedTime);
            OnWindowUpdate(new SDLWindowUpdateEventArgs(totalTime, elapsedTime));
        }

        internal void RaiseWindowPaint(double totalTime, double elapsedTime)
        {
            //SDLLog.Info($"Window {windowId} Paint {totalTime} {elapsedTime}");
            renderer.BeginPaint();
            screen.Render(renderer, totalTime, elapsedTime);
            OnWindowPaint(new SDLWindowPaintEventArgs(renderer, renderer.Width, renderer.Height, totalTime, elapsedTime));

            if (showFPS)
            {
                renderer.DrawText(null, SDLApplication.FPSText, fpsPosX, fpsPosY, Color.White);
            }
            renderer.EndPaint();
        }

        internal void RaiseMouseButtonDown(int which, int x, int y, MouseButton button, int click, KeyButtonState state)
        {
            ScaleMouse(ref x, ref y);
            SDLLog.Verbose($"Window {windowId} Mouse {which} {button} {state} {x} {y}");
            lastButton = button;
            lastState = state;
            OnMouseButtonDown(new SDLMouseEventArgs(which, x, y, button, click, state, 0, 0));
        }
        internal void RaiseMouseButtonUp(int which, int x, int y, MouseButton button, int click, KeyButtonState state)
        {
            ScaleMouse(ref x, ref y);
            SDLLog.Verbose($"Window {windowId} Mouse {which} {button} {state} {x} {y}");
            lastButton = button;
            lastState = state;
            OnMouseButtonUp(new SDLMouseEventArgs(which, x, y, button, click, state, 0, 0));
        }
        internal void RaiseMouseMove(int which, int x, int y, int relX, int relY)
        {
            ScaleMouse(ref x, ref y);
            SDLLog.Verbose($"Window {windowId} Mouse {which} Moved {x} {y} {relX} {relY}");
            OnMouseMove(new SDLMouseEventArgs(which, x, y, lastButton, 0, lastState, relX, relY));
        }

        internal void RaiseMouseWheel(int which, int x, int y, float preciseX, float preciseY, MouseWheelDirection direction)
        {
            SDLLog.Verbose($"Window {windowId} Mouse {which} Wheel {x} {y} {preciseX} {preciseY} {direction}");
            OnMouseWheel(new SDLMouseWheelEventArgs(which, x, y, preciseX, preciseY, direction));
        }

        internal void RaiseKeyDown(ScanCode scanCode, KeyCode keyCode, KeyMod keyMod, KeyButtonState state, bool repeat)
        {
            SDLLog.Verbose($"Window {windowId} {scanCode} {keyCode} {keyMod} {state}");
            OnKeyDown(new SDLKeyEventArgs(scanCode, keyCode, keyMod, state, repeat));
        }
        internal void RaiseKeyUp(ScanCode scanCode, KeyCode keyCode, KeyMod keyMod, KeyButtonState state, bool repeat)
        {
            SDLLog.Verbose($"Window {windowId} {scanCode} {keyCode} {keyMod} {state}");
            OnKeyUp(new SDLKeyEventArgs(scanCode, keyCode, keyMod, state, repeat));
        }

        internal void RaiseTextInput(string text)
        {
            SDLLog.Verbose($"Window {windowId} Text Input '{text}'");
            OnTextInput(new SDLTextInputEventArgs(text));
        }

        internal void RaiseControllerButtonDown(SDLController controller, ControllerButton button, KeyButtonState state)
        {
            SDLLog.Verbose($"Window {windowId} Controller {controller.Which} {button} {state}");
            OnControllerButtonDown(new SDLControllerButtonEventArgs(controller, button, state));
        }
        internal void RaiseControllerButtonUp(SDLController controller, ControllerButton button, KeyButtonState state)
        {
            SDLLog.Verbose($"Window {windowId} Controller {controller.Which} {button} {state}");
            OnControllerButtonUp(new SDLControllerButtonEventArgs(controller, button, state));
        }
        internal void RaiseControllerAxisEvent(SDLController controller, ControllerAxis axis, int axisValue, Vector2 direction)
        {
            SDLLog.Verbose($"Window {windowId} Controller {controller.Which} {axis} {axisValue} {direction}");
            OnControllerAxis(new SDLControllerAxisEventArgs(controller, axis, axisValue, direction));
        }
        internal void RaiseControllerTouchpadDownEvent(SDLController controller, int touchpad, int finger, float x, float y, float pressure)
        {
            SDLLog.Verbose($"Window {windowId} Controller {controller.Which} {touchpad} down {finger} {x} {y} {pressure}");
            OnControllerTouchpadDown(new SDLControllerTouchpadEventArgs(controller, touchpad, finger, x, y, pressure));
        }
        internal void RaiseControllerTouchpadUpEvent(SDLController controller, int touchpad, int finger, float x, float y, float pressure)
        {
            SDLLog.Verbose($"Window {windowId} Controller {controller.Which} {touchpad} up {finger} {x} {y} {pressure}");
            OnControllerTouchpadUp(new SDLControllerTouchpadEventArgs(controller, touchpad, finger, x, y, pressure));
        }
        internal void RaiseControllerTouchpadMotionEvent(SDLController controller, int touchpad, int finger, float x, float y, float pressure)
        {
            SDLLog.Verbose($"Window {windowId} Controller {controller.Which} {touchpad} move {finger} {x} {y} {pressure}");
            OnControllerTouchpadMove(new SDLControllerTouchpadEventArgs(controller, touchpad, finger, x, y, pressure));
        }
        private void CreateHandle()
        {
            SDL_WindowFlags flags = SDL_WindowFlags.ALLOW_HIGHDPI;
            if (!visible) { flags |= SDL_WindowFlags.HIDDEN; }
            if (resizeable) { flags |= SDL_WindowFlags.RESIZABLE; }
            if (alwaysOnTop) { flags |= SDL_WindowFlags.ALWAYS_ON_TOP; }
            if (borderless) { flags |= SDL_WindowFlags.BORDERLESS; }
            if (skipTaskbar) { flags |= SDL_WindowFlags.SKIP_TASKBAR; }
            if (mouseGrab) { flags |= SDL_WindowFlags.MOUSE_GRABBED; }
            if (keyboardGrab) { flags |= SDL_WindowFlags.KEYBOARD_GRABBED; }
            if (title != null)
            {
                handle = SDL_CreateWindow(title, x, y, width, height, flags);
            }
            else
            {
                handle = SDL_CreateWindow(IntPtr.Zero, x, y, width, height, flags);
            }
            if (handle != IntPtr.Zero)
            {
                windowId = SDL_GetWindowID(handle);
                SDLLog.Info($"SDLWindow {windowId} created");
                renderer.SetBackBufferSize(backBufferWidth, backBufferHeight);
                renderer.CreateHandle();
                if (renderer.HandleCreated)
                {
                    if (fullScreen) { GoFullScreen(); }
                    RaiseWindowLoad();
                    screen.Initialize(this);
                }
            }
            else
            {
                SDLLog.Error($"Could not create SDLWindow: {SDLApplication.GetError()}");
            }
        }


        private const string LibName = "SDL2";

        [Flags]
        private enum SDL_WindowFlags : uint
        {
            FULLSCREEN = 0x00000001,
            OPENGL = 0x00000002,
            SHOWN = 0x00000004,
            HIDDEN = 0x00000008,
            BORDERLESS = 0x00000010,
            RESIZABLE = 0x00000020,
            MINIMIZED = 0x00000040,
            MAXIMIZED = 0x00000080,
            MOUSE_GRABBED = 0x00000100,
            INPUT_FOCUS = 0x00000200,
            MOUSE_FOCUS = 0x00000400,
            FULLSCREEN_DESKTOP = (FULLSCREEN | 0x00001000),
            FOREIGN = 0x00000800,
            ALLOW_HIGHDPI = 0x00002000,
            MOUSE_CAPTURE = 0x00004000,
            ALWAYS_ON_TOP = 0x00008000,
            SKIP_TASKBAR = 0x00010000,
            UTILITY = 0x00020000,
            TOOLTIP = 0x00040000,
            POPUP_MENU = 0x00080000,
            KEYBOARD_GRABBED = 0x00100000,
            VULKAN = 0x10000000,
            METAL = 0x2000000,
            INPUT_GRABBED = MOUSE_GRABBED
        };


        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateWindow([In()][MarshalAs(UnmanagedType.LPUTF8Str)] string title, int x, int y, int w, int h, SDL_WindowFlags flags);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateWindow(IntPtr title, int x, int y, int w, int h, SDL_WindowFlags flags);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetWindowID(IntPtr window);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_DestroyWindow(IntPtr window);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowResizable(IntPtr window, [In()][MarshalAs(UnmanagedType.Bool)] bool resizable);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowAlwaysOnTop(IntPtr window, [In()][MarshalAs(UnmanagedType.Bool)] bool onTop);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowTitle(IntPtr window, [In][MarshalAs(UnmanagedType.LPUTF8Str)] string title);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowTitle(IntPtr window, IntPtr title);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowBordered(IntPtr window, [In()][MarshalAs(UnmanagedType.Bool)] bool bordered);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_ShowWindow(IntPtr window);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_HideWindow(IntPtr window);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowPosition(IntPtr window, int x, int y);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowSize(IntPtr window, int w, int h);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_GetWindowPosition(IntPtr window, out int x, out int y);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_GetWindowSize(IntPtr window, out int w, out int h);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetWindowDisplayIndex(IntPtr window);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetDisplayBounds(int displayIndex, out Rectangle rect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetNumVideoDisplays();
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_SetWindowFullscreen(IntPtr window, uint flags);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SDL_GetWindowGrab(IntPtr window);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SDL_GetWindowKeyboardGrab(IntPtr window);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SDL_GetWindowMouseGrab(IntPtr window);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowGrab(IntPtr window, [MarshalAs(UnmanagedType.Bool)] bool grabbed);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowKeyboardGrab(IntPtr window, [MarshalAs(UnmanagedType.Bool)] bool grabbed);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowMouseGrab(IntPtr window, [MarshalAs(UnmanagedType.Bool)] bool grabbed);


    }
}