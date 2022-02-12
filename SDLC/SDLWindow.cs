// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;

using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

internal sealed class SDLWindow : IWindow, IDisposable
{

    public const int SDL_WINDOWPOS_UNDEFINED_MASK = 0x1FFF0000;
    public const int SDL_WINDOWPOS_CENTERED_MASK = 0x2FFF0000;
    public const int SDL_WINDOWPOS_UNDEFINED = 0x1FFF0000;
    public const int SDL_WINDOWPOS_CENTERED = 0x2FFF0000;


    private IntPtr handle;
    private readonly SDLRenderer renderer;
    private readonly SDLContentManager contentManager;
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
    private readonly List<SDLApplet> paintApplets = new();
    private readonly List<SDLApplet> inputApplets = new();
    private readonly List<SDLApplet> otherApplets = new();
    private IScreen screen;
    private readonly ScreenForwarder screenForwarder;
    private bool enableEventHandlers;
    private readonly SDLWindowPaintEventArgs paintEventArgs;
    private readonly SDLWindowUpdateEventArgs updateEventArgs;

    private readonly EventHandlerList eventHandlerList = new();
    private static readonly object windowLoadEventKey = new();
    private static readonly object windowShownEventKey = new();
    private static readonly object windowHiddenEventKey = new();
    private static readonly object windowExposedEventKey = new();
    private static readonly object windowMovedEventKey = new();
    private static readonly object windowResizedEventKey = new();
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
        contentManager = new SDLContentManager(this);
        renderer = new SDLRenderer(this);
        paintEventArgs = new SDLWindowPaintEventArgs(renderer, 0, 0);
        updateEventArgs = new SDLWindowUpdateEventArgs(0, 0);
        screenForwarder = new ScreenForwarder(this);
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

    public bool EnableEventHandlers
    {
        get => enableEventHandlers;
        set
        {
            if (enableEventHandlers != value)
            {
                enableEventHandlers = value;
            }
        }
    }

    public IContentManager ContentManager => contentManager;

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
                    SDLLog.Warn(LogCategory.VIDEO, $"SkipTaskbar can only be set before the window handle is created");
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
    }

    public void Hide()
    {
        visible = false;
        if (HandleCreated)
        {
            SDL_HideWindow(handle);
        }
    }

    public void Close()
    {
        screen.Hide(this);
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

    public T GetApplet<T>() where T : SDLApplet, new()
    {
        foreach (SDLApplet applet in applets)
        {
            if (applet is T t) { return t; }
        }
        T newT = new();
        AddApplet(newT);
        return newT;
    }

    public void AddApplet(SDLApplet applet)
    {
        if (!applets.Contains(applet))
        {
            applets.Add(applet);
            InitApplet(applet);
            CacheAppletSortOrder();
        }
    }

    public void RemoveApplet(SDLApplet applet)
    {
        applets.Remove(applet);
        applet.OnUninstall(this);
        CacheAppletSortOrder();
    }
    public void ChangeApplet(SDLApplet applet)
    {
        if (applets.Contains(applet))
        {
            CacheAppletSortOrder();
        }
    }

    private void InitLateApplets()
    {
        foreach (SDLApplet applet in applets)
        {
            InitApplet(applet);
        }
        CacheAppletSortOrder();
    }
    private void InitApplet(SDLApplet applet)
    {
        if (HandleCreated && !applet.Installed)
        {
            applet.OnInstall(this);
        }
    }

    private void ClearApplets()
    {
        foreach (SDLApplet applet in applets)
        {
            applet.OnUninstall(this);
            applet.Dispose();
        }
        applets.Clear();
        CacheAppletSortOrder();
    }

    private void CacheAppletSortOrder()
    {
        paintApplets.Clear();
        paintApplets.AddRange(applets.Where(x => x.Enabled && !x.NoRender).OrderBy(x => x.RenderPrio));
        inputApplets.Clear();
        inputApplets.AddRange(applets.Where(x => x.Enabled && !x.NoInput).OrderBy(x => x.InputPrio));
        otherApplets.Clear();
        otherApplets.AddRange(applets.Where(x => x.Enabled));
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
            if (SDL_SetWindowFullscreen(Handle, 0) == 0)
            {
                SDLLog.Info(LogCategory.VIDEO, $"Entered Windowed Mode");
            }
        }
        else
        {
            SDL_SetWindowSize(handle, oldWidth, oldHeight);
            SDL_SetWindowPosition(handle, oldX, oldY);
            SDL_SetWindowBordered(handle, !borderless);
            SDL_SetWindowResizable(handle, resizeable);
            if (title != null) { SDL_SetWindowTitle(handle, title); }
            SDL_SetWindowAlwaysOnTop(handle, alwaysOnTop);
            SDLLog.Info(LogCategory.VIDEO, $"Entered Windowed Mode");
        }
    }

    private void GoFullSizeFullScreen()
    {
        SDL_GetWindowPosition(handle, out oldX, out oldY);
        SDL_GetWindowSize(handle, out oldWidth, out oldHeight);
        int index = SDL_GetWindowDisplayIndex(handle);
        if (SDL_GetDisplayBounds(index, out Rectangle bounds) == 0)
        {
            SDL_SetWindowBordered(handle, false);
            SDL_SetWindowResizable(handle, false);
            SDL_SetWindowTitle(handle, IntPtr.Zero);
            SDL_SetWindowAlwaysOnTop(handle, true);
            SDL_SetWindowSize(handle, bounds.Width, bounds.Height);
            SDL_SetWindowPosition(handle, bounds.X, bounds.Y);
            SDLLog.Info(LogCategory.VIDEO, $"Entered Full Size Full Screen Mode");
        }
    }

    private void GoMultiMonitorFullScreen()
    {
        SDL_GetWindowPosition(handle, out oldX, out oldY);
        SDL_GetWindowSize(handle, out oldWidth, out oldHeight);
        if (SDL_GetDisplayBounds(0, out Rectangle bounds) == 0)
        {
            int numDisplays = SDL_GetNumVideoDisplays();
            for (int index = 1; index < numDisplays; index++)
            {
                if (SDL_GetDisplayBounds(index, out Rectangle otherBounds) == 0)
                {
                    if (otherBounds.Height == bounds.Height)
                    {
                        bounds = Rectangle.Union(bounds, otherBounds);

                    }
                    else
                    {
                        break;
                    }
                }
            }
            SDL_SetWindowBordered(handle, false);
            SDL_SetWindowResizable(handle, false);
            SDL_SetWindowTitle(handle, IntPtr.Zero);
            SDL_SetWindowAlwaysOnTop(handle, true);
            SDL_SetWindowSize(handle, bounds.Width, bounds.Height);
            SDL_SetWindowPosition(handle, bounds.X, bounds.Y);
            SDLLog.Info(LogCategory.VIDEO, $"Entered Multi Monitor Full Screen Mode");
        }
    }

    private void GoDesktopFullScreen()
    {
        SDL_GetWindowPosition(handle, out oldX, out oldY);
        SDL_GetWindowSize(handle, out oldWidth, out oldHeight);
        if (SDL_SetWindowFullscreen(Handle, (uint)SDL_WindowFlags.FULLSCREEN_DESKTOP) == 0)
        {
            SDLLog.Info(LogCategory.VIDEO, $"Entered Desktop Full Screen Mode");
        }
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

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {

            }
            ClearApplets();
            renderer.Dispose();
            if (handle != IntPtr.Zero)
            {
                SDL_DestroyWindow(handle);
                handle = IntPtr.Zero;
                SDLLog.Info(LogCategory.VIDEO, "SDLWindow {0} destroyed", windowId);
            }
            disposedValue = true;
        }
    }

    private void DetectWindowResized()
    {
        SDL_GetWindowSize(handle, out int w, out int h);
        if (w != width || h != height)
        {
            RaiseWindowResized(w, h, WindowResizeSource.Detected);
        }
    }
    internal void RaiseWindowShown()
    {
        SDLLog.Debug(LogCategory.VIDEO, "Window {0} Shown", windowId);
        foreach (SDLApplet applet in applets) { applet.OnWindowShown(EventArgs.Empty); }        
        if (enableEventHandlers) { ((EventHandler?)eventHandlerList[windowShownEventKey])?.Invoke(this, EventArgs.Empty); }
    }
    internal void RaiseWindowHidden()
    {
        SDLLog.Debug(LogCategory.VIDEO, "Window {0} Hidden", windowId);
        foreach (SDLApplet applet in applets) { applet.OnWindowHidden(EventArgs.Empty); }
        if (enableEventHandlers) { ((EventHandler?)eventHandlerList[windowHiddenEventKey])?.Invoke(this, EventArgs.Empty); }
    }
    internal void RaiseWindowExposed()
    {
        SDLLog.Debug(LogCategory.VIDEO, "Window {0} Exposed", windowId);
        foreach (SDLApplet applet in applets) { applet.OnWindowExposed(EventArgs.Empty); }
        if (enableEventHandlers) { ((EventHandler?)eventHandlerList[windowExposedEventKey])?.Invoke(this, EventArgs.Empty); }
        DetectWindowResized();
    }
    internal void RaiseWindowMinimized()
    {
        SDLLog.Debug(LogCategory.VIDEO, "Window {0} Minimized", windowId);
        foreach (SDLApplet applet in applets) { applet.OnWindowMinimized(EventArgs.Empty); }
        if (enableEventHandlers) { ((EventHandler?)eventHandlerList[windowMinimizedEventKey])?.Invoke(this, EventArgs.Empty); }
    }
    internal void RaiseWindowMaximized()
    {
        SDLLog.Debug(LogCategory.VIDEO, "Window {0} Maximized", windowId);
        foreach (SDLApplet applet in applets) { applet.OnWindowMaximized(EventArgs.Empty); }
        if (enableEventHandlers) { ((EventHandler?)eventHandlerList[windowMaximizedEventKey])?.Invoke(this, EventArgs.Empty); }
    }
    internal void RaiseWindowRestored()
    {
        SDLLog.Debug(LogCategory.VIDEO, "Window {0} Restored", windowId);
        foreach (SDLApplet applet in applets) { applet.OnWindowRestored(EventArgs.Empty); }
        if (enableEventHandlers) { ((EventHandler?)eventHandlerList[windowRestoredEventKey])?.Invoke(this, EventArgs.Empty); }
    }
    internal void RaiseWindowEnter()
    {
        SDLLog.Debug(LogCategory.INPUT, "Window {0} Enter", windowId);
        foreach (SDLApplet applet in applets) { applet.OnWindowEnter(EventArgs.Empty); }
        if (enableEventHandlers) { ((EventHandler?)eventHandlerList[windowEnterEventKey])?.Invoke(this, EventArgs.Empty); }
    }
    internal void RaiseWindowLeave()
    {
        SDLLog.Debug(LogCategory.INPUT, "Window {0} Leave", windowId);
        foreach (SDLApplet applet in applets) { applet.OnWindowLeave(EventArgs.Empty); }
        if (enableEventHandlers) { ((EventHandler?)eventHandlerList[windowLeaveEventKey])?.Invoke(this, EventArgs.Empty); }
    }
    internal void RaiseWindowFocusGained()
    {
        SDLLog.Debug(LogCategory.INPUT, "Window {0} Focus Gained", windowId);
        foreach (SDLApplet applet in applets) { applet.OnWindowFocusGained(EventArgs.Empty); }
        if (enableEventHandlers) { ((EventHandler?)eventHandlerList[windowFocusGainedEventKey])?.Invoke(this, EventArgs.Empty); }
    }
    internal void RaiseWindowFocusLost()
    {
        SDLLog.Debug(LogCategory.INPUT, "Window {0} Focus Lost", windowId);
        foreach (SDLApplet applet in applets) { applet.OnWindowFocusLost(EventArgs.Empty); }
        if (enableEventHandlers) { ((EventHandler?)eventHandlerList[windowFocusLostEventKey])?.Invoke(this, EventArgs.Empty); }
    }
    internal void RaiseWindowTakeFocus()
    {
        SDLLog.Debug(LogCategory.INPUT, "Window {0} Take Focus", windowId);
        foreach (SDLApplet applet in applets) { applet.OnWindowTakeFocus(EventArgs.Empty); }
        if (enableEventHandlers) { ((EventHandler?)eventHandlerList[windowTakeFocusEventKey])?.Invoke(this, EventArgs.Empty); }
    }
    internal void RaiseWindowClose()
    {
        SDLLog.Debug(LogCategory.VIDEO, "Window {0} Close", windowId);
        foreach (SDLApplet applet in applets) { applet.OnWindowClose(EventArgs.Empty); }
        if (enableEventHandlers) { ((EventHandler?)eventHandlerList[windowCloseEventKey])?.Invoke(this, EventArgs.Empty); }
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
    internal void RaiseWindowMoved(int x, int y)
    {
        SDLLog.Debug(LogCategory.VIDEO, "Window {0} Moved {1} {2}", windowId, x, y);
        this.x = x;
        this.y = y;
        SDLWindowPositionEventArgs e = new(x, y);
        foreach (SDLApplet applet in applets) { applet.OnWindowMoved(e); }
        if (enableEventHandlers) { ((SDLWindowPositionEventHandler?)eventHandlerList[windowMovedEventKey])?.Invoke(this, e); }
    }
    internal void RaiseWindowResized(int width, int height, WindowResizeSource source)
    {
        SDLLog.Debug(LogCategory.VIDEO, "Window {0} Resized {1} {2} ({3})", windowId, width, height, source);
        this.width = width;
        this.height = height;
        SDLWindowSizeEventArgs e = new(width, height, source);
        renderer.WindowResized(width, height, source);
        foreach (SDLApplet applet in applets) { applet.OnWindowResized(e); }
        if (enableEventHandlers) { ((SDLWindowSizeEventHandler?)eventHandlerList[windowResizedEventKey])?.Invoke(this, e); }
    }
    internal void RaiseWindowLoad()
    {
        SDLLog.Debug(LogCategory.VIDEO, "Window {0} Load", windowId);
        SDLWindowLoadEventArgs e = new(renderer);
        if (enableEventHandlers) { ((SDLWindowLoadEventHandler?)eventHandlerList[windowLoadEventKey])?.Invoke(this, e); }
    }
    internal void RaiseWindowUpdate(double totalTime, double elapsedTime)
    {
        updateEventArgs.Update(totalTime, elapsedTime);
        foreach (SDLApplet applet in otherApplets) { applet.InternalOnUpdate(updateEventArgs); }
        if (enableEventHandlers) { ((SDLWindowUpdateEventHandler?)eventHandlerList[windowUpdateEventKey])?.Invoke(this, updateEventArgs); }
    }
    internal void RaiseWindowPaint(double totalTime, double elapsedTime)
    {
        paintEventArgs.Update(totalTime, elapsedTime);
        renderer.BeginPaint();
        foreach (SDLApplet applet in paintApplets) { applet.InternalOnPaint(paintEventArgs); }
        if (enableEventHandlers) { ((SDLWindowPaintEventHandler?)eventHandlerList[windowPaintEventKey])?.Invoke(this, paintEventArgs); }
        if (showFPS) { renderer.DrawText(null, SDLApplication.FPSText, fpsPosX, fpsPosY, Color.White); }
        renderer.EndPaint();
    }
    internal void RaiseMouseButtonDown(int which, int x, int y, MouseButton button, int clicks, KeyButtonState state)
    {
        ScaleMouse(ref x, ref y);
        SDLLog.Verbose(LogCategory.INPUT, $"Window {0} Mouse {1} {2} {3} {4} {5}", windowId, which, button, state, x, y);
        lastButton = button;
        lastState = state;
        SDLMouseEventArgs e = new(which, x, y, button, clicks, state, 0, 0);
        foreach (SDLApplet applet in inputApplets) { applet.OnMouseButtonDown(e); if (e.Handled) break; }
        if (enableEventHandlers && !e.Handled) { ((SDLMouseEventHandler?)eventHandlerList[mouseButtonDownEventKey])?.Invoke(this, e); }
    }

    internal void RaiseMouseButtonUp(int which, int x, int y, MouseButton button, int clicks, KeyButtonState state)
    {
        ScaleMouse(ref x, ref y);
        SDLLog.Verbose(LogCategory.INPUT, "Window {0} Mouse {1} {2} {3} {4} {5}", windowId, which, button, state, x, y);
        lastButton = button;
        lastState = state;
        SDLMouseEventArgs e = new(which, x, y, button, clicks, state, 0, 0);
        foreach (SDLApplet applet in inputApplets) { applet.OnMouseButtonUp(e); if (e.Handled) break; }
        if (enableEventHandlers && !e.Handled) { ((SDLMouseEventHandler?)eventHandlerList[mouseButtonUpEventKey])?.Invoke(this, e); }
    }

    internal void RaiseMouseMove(int which, int x, int y, int relX, int relY)
    {
        ScaleMouse(ref x, ref y);
        SDLLog.Verbose(LogCategory.INPUT, "Window {0} Mouse {1} Moved {2} {3} {4} {5}", windowId, which, x, y, relX, relY);
        SDLMouseEventArgs e = new(which, x, y, lastButton, 0, lastState, relX, relY);
        foreach (SDLApplet applet in inputApplets) { applet.OnMouseMove(e); if (e.Handled) break; }
        if (enableEventHandlers && !e.Handled) { ((SDLMouseEventHandler?)eventHandlerList[mouseMoveEventKey])?.Invoke(this, e); }
    }

    internal void RaiseMouseWheel(int which, int x, int y, float preciseX, float preciseY, MouseWheelDirection direction)
    {
        SDLLog.Verbose(LogCategory.INPUT, "Window {0} Mouse {1} Wheel {2} {3} {4} {5} {6}", windowId, which, x, y, preciseX, preciseY, direction);
        SDLMouseWheelEventArgs e = new(which, x, y, preciseX, preciseY, direction);
        foreach (SDLApplet applet in inputApplets) { applet.OnMouseWheel(e); if (e.Handled) break; }
        if (enableEventHandlers && !e.Handled) { ((SDLMouseWheelEventHandler?)eventHandlerList[mouseWheelEventKey])?.Invoke(this, e); }
    }

    internal void RaiseKeyDown(ScanCode scanCode, KeyCode keyCode, KeyMod keyMod, KeyButtonState state, bool repeat)
    {
        SDLLog.Verbose(LogCategory.INPUT, "Window {0} {1} {2} {3} {4}", windowId, scanCode, keyCode, keyMod, state);
        SDLKeyEventArgs e = new(scanCode, keyCode, keyMod, state, repeat);
        foreach (SDLApplet applet in inputApplets) { applet.OnKeyDown(e); if (e.Handled) break; }
        if (enableEventHandlers && !e.Handled) { ((SDLKeyEventHandler?)eventHandlerList[keyDownEventKey])?.Invoke(this, e); }
    }
    internal void RaiseKeyUp(ScanCode scanCode, KeyCode keyCode, KeyMod keyMod, KeyButtonState state, bool repeat)
    {
        SDLLog.Verbose(LogCategory.INPUT, "Window {0} {1} {2} {3} {4}", windowId, scanCode, keyCode, keyMod, state);
        SDLKeyEventArgs e = new(scanCode, keyCode, keyMod, state, repeat);
        foreach (SDLApplet applet in inputApplets) { applet.OnKeyUp(e); if (e.Handled) break; }
        if (enableEventHandlers && !e.Handled) { ((SDLKeyEventHandler?)eventHandlerList[keyUpEventKey])?.Invoke(this, e); }
    }
    internal void RaiseTextInput(string text)
    {
        SDLLog.Verbose(LogCategory.INPUT, "Window {0} Text Input '{1}'", windowId, text);
        SDLTextInputEventArgs e = new(text);
        foreach (SDLApplet applet in inputApplets) { applet.OnTextInput(e); if (e.Handled) break; }
        if (enableEventHandlers && !e.Handled) { ((SDLTextInputEventHandler?)eventHandlerList[textInputEventKey])?.Invoke(this, e); }
    }
    internal void RaiseControllerButtonDown(SDLController controller, ControllerButton button, KeyButtonState state)
    {
        SDLLog.Verbose(LogCategory.INPUT, "Window {0} Controller {1} {2} {3}", windowId, controller.Which, button, state);
        SDLControllerButtonEventArgs e = new(controller, button, state);
        foreach (SDLApplet applet in inputApplets) { applet.OnControllerButtonDown(e); if (e.Handled) break; }
        if (enableEventHandlers && !e.Handled) { ((SDLControllerButtonEventHandler?)eventHandlerList[controllerButtonDownEventKey])?.Invoke(this, e); }
    }
    internal void RaiseControllerButtonUp(SDLController controller, ControllerButton button, KeyButtonState state)
    {
        SDLLog.Verbose(LogCategory.INPUT, "Window {0} Controller {1} {2} {3}", windowId, controller.Which, button, state);
        SDLControllerButtonEventArgs e = new(controller, button, state);
        foreach (SDLApplet applet in inputApplets) { applet.OnControllerButtonUp(e); if (e.Handled) break; }
        if (enableEventHandlers && !e.Handled) { ((SDLControllerButtonEventHandler?)eventHandlerList[controllerButtonUpEventKey])?.Invoke(this, e); }
    }
    internal void RaiseControllerAxisEvent(SDLController controller, ControllerAxis axis, int axisValue, Vector2 direction)
    {
        SDLLog.Verbose(LogCategory.INPUT, "Window {0} Controller {1} {2} {3} {4}", windowId, controller.Which, axis, axisValue, direction);
        SDLControllerAxisEventArgs e = new(controller, axis, axisValue, direction);
        foreach (SDLApplet applet in inputApplets) { applet.OnControllerAxis(e); if (e.Handled) break; }
        if (enableEventHandlers && !e.Handled) { ((SDLControllerAxisEventHandler?)eventHandlerList[controllerAxisEventKey])?.Invoke(this, e); }
    }
    internal void RaiseControllerTouchpadDownEvent(SDLController controller, int touchpad, int finger, float x, float y, float pressure)
    {
        SDLLog.Verbose(LogCategory.INPUT, "Window {0} Controller {1} {2} {3} down {4} {5} {6}", windowId, controller.Which, touchpad, finger, x, y, pressure);
        SDLControllerTouchpadEventArgs e = new(controller, touchpad, finger, x, y, pressure);
        foreach (SDLApplet applet in inputApplets) { applet.OnControllerTouchpadDown(e); if (e.Handled) break; }
        if (enableEventHandlers && !e.Handled) { ((SDLControllerTouchpadEventHandler?)eventHandlerList[controllerTouchpadDownEventKey])?.Invoke(this, e); }
    }
    internal void RaiseControllerTouchpadUpEvent(SDLController controller, int touchpad, int finger, float x, float y, float pressure)
    {
        SDLLog.Verbose(LogCategory.INPUT, "Window {0} Controller {1} {2} {3} up {4} {5} {6}", windowId, controller.Which, touchpad, finger, x, y, pressure);
        SDLControllerTouchpadEventArgs e = new(controller, touchpad, finger, x, y, pressure);
        foreach (SDLApplet applet in inputApplets) { applet.OnControllerTouchpadUp(e); if (e.Handled) break; }
        if (enableEventHandlers && !e.Handled) { ((SDLControllerTouchpadEventHandler?)eventHandlerList[controllerTouchpadUpEventKey])?.Invoke(this, e); }
    }
    internal void RaiseControllerTouchpadMotionEvent(SDLController controller, int touchpad, int finger, float x, float y, float pressure)
    {
        SDLLog.Verbose(LogCategory.INPUT, "Window {0} Controller {1} {2} {3} move {4} {5} {6}", windowId, controller.Which, touchpad, finger, x, y, pressure);
        SDLControllerTouchpadEventArgs e = new(controller, touchpad, finger, x, y, pressure);
        foreach (SDLApplet applet in inputApplets) { applet.OnControllerTouchpadMove(e); if (e.Handled) break; }
        if (enableEventHandlers && !e.Handled) { ((SDLControllerTouchpadEventHandler?)eventHandlerList[controllerTouchpadMotionEventKey])?.Invoke(this, e); }
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
            SDLLog.Info(LogCategory.VIDEO, "SDLWindow {0} created", windowId);
            renderer.SetBackBufferSize(backBufferWidth, backBufferHeight);
            renderer.CreateHandle();
            if (renderer.HandleCreated)
            {
                InitLateApplets();
                AddApplet(screenForwarder);
                if (fullScreen) { GoFullScreen(); }
                RaiseWindowLoad();
                screen.Show(this);
            }
        }
        else
        {
            SDLLog.Critical(LogCategory.VIDEO, "Could not create SDLWindow: {0}", SDLApplication.GetError());
        }
    }

    private class ScreenForwarder : SDLApplet
    {
        private readonly SDLWindow window;
        public ScreenForwarder(SDLWindow window) : base("Screen Forwarder")
        {
            this.window = window;
        }

        protected override void OnWindowLoad(SDLWindowLoadEventArgs e)
        {
            window.screen.Initialize(window);
        }
        protected override void OnWindowUpdate(SDLWindowUpdateEventArgs e)
        {
            window.screen.Update(window.renderer, e.TotalTime, e.ElapsedTime);
        }
        protected override void OnWindowPaint(SDLWindowPaintEventArgs e)
        {
            window.screen.Render(e.Renderer, e.TotalTime, e.ElapsedTime);
        }

        protected internal override void OnWindowResized(SDLWindowSizeEventArgs e)
        {
            window.screen.Resized(window, e.Width, e.Height);
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
