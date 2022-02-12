// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.Applets;

using SDLC.GUI;
using System;
using System.Collections.Generic;
using System.Drawing;

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
    private readonly List<Window> fadingInWindows = new();
    private readonly List<Window> fadingOutWindows = new();
    private const int fadeOutSpeed = 22;
    private const int fadeInSpeed = 27;
    private bool useFadeOut = true;
    private bool useFadeIn = true;
    private bool moveWindowToFrontOnActivate = true;
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
    protected internal override void OnWindowResized(SDLWindowSizeEventArgs e)
    {
        UpdateScreenSize(e.Width, e.Height);
    }

    protected override void OnWindowUpdate(SDLWindowUpdateEventArgs e)
    {
        CheckWindowActivationQueue();
        CheckFadingOutWindows();
        CheckFadingInWindows();
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
                SDLLog.Verbose(LogCategory.APPLICATION, "Screen deactivated: {0}", activeScreen);
            }
            activeScreen = scr;
            if (activeScreen != null)
            {
                SDLLog.Verbose(LogCategory.APPLICATION, "Screen activated: {0}", activeScreen);
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
                SDLLog.Verbose(LogCategory.APPLICATION, "Window deactivated: {0}", activeWindow);
            }
            activeWindow = win;
            if (activeWindow != null)
            {
                activeWindow.Active = true;
                SDLLog.Verbose(LogCategory.APPLICATION, "Window activated: {0}", activeWindow);
                if (moveWindowToFrontOnActivate)
                {
                    activeWindow.ToFront();
                }
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
                SDLLog.Verbose(LogCategory.APPLICATION, "Gadget deactivated: {0}", activeGadget);
            }
            activeGadget = gad;
            if (activeGadget != null)
            {
                activeGadget.Active = true;
                SDLLog.Verbose(LogCategory.APPLICATION, "Gadget activated: {0}", activeGadget);
            }
        }
    }
    private void SetSelectedGadget(Gadget? gad)
    {
        if (selectedGadget != gad)
        {
            if (selectedGadget != null)
            {
                selectedGadget.HandleDeselection();
                SDLLog.Verbose(LogCategory.APPLICATION, "Gadget deselected: {0}", selectedGadget);
            }
            selectedGadget = gad;
            if (selectedGadget != null)
            {
                selectedGadget.HandleSelection();
                SDLLog.Verbose(LogCategory.APPLICATION, "Gadget selected: {0}", selectedGadget);
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

    private static void CheckAndClear(ref Gadget? gadget, Window test)
    {
        if (gadget != null && gadget.Window == test)
        {
            gadget = null;
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
            activeWindow.MoveWindow(diffMouseX, diffMouseY, true);
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
            activeWindow.SizeWindow(diffMouseX, diffMouseY, true);
            return true;
        }
        return false;
    }
    private bool CheckGadgetMove(SDLMouseEventArgs e)
    {
        bool result = false;
        if (downGadget == mouseGadget && downGadget != null)
        {
            result |= downGadget.HandleMouseMove(e.X, e.Y);
        }
        else
        {
            if (downGadget != null)
            {
                result |= downGadget.HandleMouseMove(e.X, e.Y);
            }
            if (mouseGadget != null)
            {
                result |= mouseGadget.HandleMouseMove(e.X, e.Y);
            }
        }
        return result;
    }
    private bool CheckGadgetDown(SDLMouseEventArgs e)
    {
        bool result = false;
        if (e.Button == MouseButton.Left)
        {
            downGadget = mouseGadget;
            if (downGadget != null)
            {
                SetSelectedGadget(downGadget);
                result |= downGadget.HandleMouseDown(e.X, e.Y, isTimerRepeat: false);
            }
        }
        return result;
    }
    private bool CheckGadgetUp(SDLMouseEventArgs e)
    {
        bool result = false;
        if (e.Button == MouseButton.Left)
        {
            upGadget = mouseGadget;
            if (upGadget != null && upGadget == downGadget)
            {
                result |= upGadget.HandleMouseUp(e.X, e.Y);
                if (upGadget.IsEndGadget && upGadget.IsReqGadget && upGadget.Requester != null)
                {
                    EndRequest(upGadget.Requester, upGadget.Window);
                }
            }
            SetSelectedGadget(null);
        }
        return result;
    }

    private bool CheckGadgetTimer(double time)
    {
        if (selectedGadget != null && downGadget != null && selectMouseDown)
        {
            if (downGadget.HandleMouseDown(mouseX, mouseY, isTimerRepeat: true))
            {
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

    private bool CheckGadgetKeyDown(SDLKeyEventArgs e)
    {
        ActionResult result = ActionResult.None;
        if (activeGadget != null)
        {
            result |= activeGadget.HandleKeyDown(e);
            if (result == ActionResult.NavigateNext)
            {
                Gadget? next = activeGadget.FindNextGadget();
                if (next != null) { ActivateGadget(next); }
            }
            else if (result == ActionResult.NavigatePrevious)
            {
                Gadget? prev = activeGadget.FindPrevGadget();
                if (prev != null) { ActivateGadget(prev); }
            }
            else if (result == ActionResult.GadgetUp)
            {

            }
        }
        return result != ActionResult.None;
    }
    private bool CheckGadgetKeyUp(SDLKeyEventArgs e)
    {
        ActionResult result = ActionResult.None;
        if (activeGadget != null)
        {
            result |= activeGadget.HandleKeyUp(e);
        }
        return result != ActionResult.None;
    }
    private bool CheckGadgetTextInput(SDLTextInputEventArgs e)
    {
        if (activeGadget != null)
        {
            return activeGadget.HandleTextInput(e);
        }
        return false;
    }

    protected internal override void OnKeyDown(SDLKeyEventArgs e)
    {
        if (CheckGadgetKeyDown(e)) { e.Handled = true; }
    }

    protected internal override void OnKeyUp(SDLKeyEventArgs e)
    {
        if (CheckGadgetKeyUp(e)) { e.Handled = true; }
    }

    protected internal override void OnTextInput(SDLTextInputEventArgs e)
    {
        if (CheckGadgetTextInput(e)) { e.Handled = true; }
    }

    private bool CheckHandled(Screen? screen, Window? window, Gadget? gadget)
    {
        if (screen == null) return false;
        if (gadget != null) return true;
        if (window != null && !window.BackDrop) return true;
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
        e.Handled = CheckHandled(screen, window, gadget);
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
        e.Handled = CheckHandled(screen, window, gadget);
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
        e.Handled = CheckHandled(screen, window, gadget);
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
        int minHeight = 0,
        bool borderless = false,
        bool backdrop = false,
        bool sizing = true,
        bool dragging = true,
        bool zooming = true,
        bool closing = true,
        bool depth = true)
    {
        WindowFlags flags = Window.DefaultFlags;
        if (borderless)
        {
            flags |= WindowFlags.Borderless;
        }
        if (!sizing)
        {
            flags &= ~WindowFlags.SizeGadget;
            flags &= ~WindowFlags.SizeBBottom;
            flags &= ~WindowFlags.SizeBRight;
        }
        if (!dragging)
        {
            flags &= ~WindowFlags.DragBar;
        }
        if (!zooming)
        {
            flags &= ~WindowFlags.HasZoom;
        }
        if (!closing)
        {
            flags &= ~WindowFlags.CloseGadget;
        }
        if (!depth)
        {
            flags &= ~WindowFlags.DepthGadget;
        }
        if (backdrop)
        {
            flags |= WindowFlags.BackDrop;
        }
        Window window = new Window(this, s, flags, title);
        window.LeftEdge = leftEdge;
        window.TopEdge = topEdge;
        window.Width = width;
        window.Height = height;
        window.MinWidth = minWidth;
        window.MinHeight = minHeight;
        window.WindowId = nextWindowID++;
        window.SetAlpha(0);
        AddSystemGadgets(window);
        if ((window.WindowFlags & WindowFlags.Activate) != 0)
        {
            activationWindows.Enqueue(window);
        }
        if (useFadeIn) { fadingInWindows.Add(window); }
        return window;
    }

    public void CloseWindow(Window window)
    {
        if (useFadeOut)
        {
            FadeOutWindow(window);
        }
        else
        {
            ReallyCloseWindow(window);
        }
    }

    private void ReallyCloseWindow(Window window)
    {
        Screen s = window.Screen;
        Window? next = s.NextWindow(window);
        s.RemoveWindow(window);
        CheckAndClear(ref activeWindow, window);
        CheckAndClear(ref mouseWindow, window);
        CheckAndClear(ref activeGadget, window);
        CheckAndClear(ref mouseGadget, window);
        CheckAndClear(ref downGadget, window);
        CheckAndClear(ref upGadget, window);
        CheckAndClear(ref selectedGadget, window);
        window.Close();
        if (next != null) { ActivateWindow(next); }
    }

    private void FadeOutWindow(Window window)
    {
        if (!fadingOutWindows.Contains(window))
        {
            fadingOutWindows.Add(window);
        }
    }

    private void CheckFadingOutWindows()
    {
        foreach (Window window in fadingOutWindows)
        {
            window.DecreaseAlpha(fadeOutSpeed);
        }
        int i = 0;
        while (i < fadingOutWindows.Count)
        {
            Window window = fadingOutWindows[i];
            if (window.Alpha > 0)
            {
                i++;
            }
            else
            {
                fadingOutWindows.RemoveAt(i);
                ReallyCloseWindow(window);
            }
        }
    }

    private void CheckFadingInWindows()
    {
        foreach (Window window in fadingInWindows)
        {
            window.IncreaseAlpha(fadeInSpeed);
        }
        int i = 0;
        while (i < fadingInWindows.Count)
        {
            Window window = fadingInWindows[i];
            if (window.Alpha < 255)
            {
                i++;
            }
            else
            {
                fadingInWindows.RemoveAt(i);
            }
        }
    }
    public Gadget AddGadget(Window w,
        Requester? requester = null,
        int leftEdge = 0,
        int topEdge = 0,
        int width = 100,
        int height = 50,
        GadgetFlags flags = GadgetFlags.None,
        GadgetActivation activation = GadgetActivation.Immediate | GadgetActivation.RelVerify,
        GadgetType type = GadgetType.BoolGadget,
        string? text = null,
        Icons icon = Icons.NONE,
        Color? bgColor = null,
        bool disabled = false,
        bool selected = false,
        bool toggleSelect = false,
        bool endGadget = false,
        Action? clickAction = null,
        string? buffer = null,
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
        if (endGadget) { activation |= GadgetActivation.EndGadget; }
        if (selected) { flags |= GadgetFlags.Selected; }

        Gadget gadget = new(this, w, requester);
        gadget.LeftEdge = leftEdge;
        gadget.TopEdge = topEdge;
        gadget.Width = width;
        gadget.Height = height;
        gadget.Flags = flags;
        gadget.Activation = activation;
        gadget.GadgetType |= type;
        gadget.GadgetId = gadgetId;
        gadget.Text = text;
        gadget.Icon = icon;
        if (bgColor != null) { gadget.BackgroundColor = bgColor.Value; }
        if (clickAction != null) { gadget.GadgetUp += (s, e) => { clickAction(); }; }
        if (buffer != null && gadget.IsStrGadget && gadget.StrInfo != null)
        {
            gadget.StrInfo.Buffer = buffer;
        }
        return gadget;
    }

    public void RemoveGadget(Window window, Gadget gadget)
    {
    }
    public Requester InitRequester(Window window)
    {
        Requester req = new Requester(this, window);
        return req;
    }
    public bool Request(Requester req, Window window)
    {
        if (window.Request(req))
        {
            if (window == activeGadget?.Window)
            {
                SetActiveGadget(null);
            }
            if (window == selectedGadget?.Window)
            {
                SetSelectedGadget(null);
            }
            if (window == mouseGadget?.Window)
            {
                SetMouseGadget(null);
            }
            return true;
        }
        return false;
    }
    public void EndRequest(Requester req, Window window)
    {
        window.EndRequest(req);
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
    public void ChangeWindowBox(Window window, int left, int top, int width, int height)
    {
        window.ChangeWindowBox(left, top, width, height);
    }

    public void MoveWindow(Window window, int deltaX, int deltaY)
    {
        window.MoveWindow(deltaX, deltaY);
    }

    public void SizeWindow(Window window, int deltaX, int deltaY)
    {
        window.SizeWindow(deltaX, deltaY);
    }
    public void ZipWindow(Window window)
    {
        window.Zip();
    }
    public void OffGadget(Gadget gadget)
    {
        gadget.Enabled = false;
    }
    public void OnGadget(Gadget gadget)
    {
        gadget.Enabled = true;
    }

    public void ModifyProp(Gadget gadget, PropFlags flags, int horizPot, int vertPot, int horizBody, int vertBody)
    {
        gadget.ModifyProp(flags, horizPot, vertPot, horizBody, vertBody);
    }
}
