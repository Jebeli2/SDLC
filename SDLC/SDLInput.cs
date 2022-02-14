// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

public static class SDLInput
{
    private static IntPtr evtMem;
    private static readonly List<SDLController> controllers = new();
    private static readonly List<SDLJoystick> joysticks = new();
    private static float controllerDeadZone = 8000.0f;
    private static float controllerMaxValue = 30000.0f;
    private static bool useController = true;

    internal static void Initialize()
    {
        evtMem = Marshal.AllocHGlobal(64);
    }

    internal static void Shutdown()
    {
        List<SDLController> clist = new List<SDLController>(controllers);
        foreach (SDLController c in clist) { RemoveController(c.Which); }
        List<SDLJoystick> jlist = new List<SDLJoystick>(joysticks);
        foreach (SDLJoystick j in jlist) { RemoveJoystick(j.Which); }
        Marshal.FreeHGlobal(evtMem);
    }

    internal static void MessageLoop()
    {
        while (SDL_PollEvent(out SDL_Event evt) != 0 && !SDLApplication.QuitRequested)
        {
            switch (evt.type)
            {
                case SDL_EventType.QUIT:
                    SDLApplication.QuitRequested = true;
                    break;
                case SDL_EventType.WINDOWEVENT:
                    HandleWindowEvent(SDLApplication.GetWindowFromId(evt.window.windowID), ref evt.window);
                    break;
                case SDL_EventType.MOUSEBUTTONDOWN:
                    HandleMouseButtonDownEvent(SDLApplication.GetWindowFromId(evt.button.windowID), ref evt.button);
                    break;
                case SDL_EventType.MOUSEBUTTONUP:
                    HandleMouseButtonUpEvent(SDLApplication.GetWindowFromId(evt.button.windowID), ref evt.button);
                    break;
                case SDL_EventType.MOUSEMOTION:
                    HandleMouseMotionEvent(SDLApplication.GetWindowFromId(evt.button.windowID), ref evt.motion);
                    break;
                case SDL_EventType.MOUSEWHEEL:
                    HandleMouseWheelEvent(SDLApplication.GetWindowFromId(evt.wheel.windowID), ref evt.wheel);
                    break;
                case SDL_EventType.KEYDOWN:
                    HandleKeyDownEvent(SDLApplication.GetWindowFromId(evt.key.windowID), ref evt.key);
                    break;
                case SDL_EventType.KEYUP:
                    HandleKeyUpEvent(SDLApplication.GetWindowFromId(evt.key.windowID), ref evt.key);
                    break;
                case SDL_EventType.TEXTINPUT:
                    Marshal.StructureToPtr(evt, evtMem, false);
                    HandleTextInput(SDLApplication.GetWindowFromId(evt.text.windowID), ref evt.text);
                    break;
                case SDL_EventType.CONTROLLERDEVICEADDED:
                    AddController(evt.cdevice.which);
                    break;
                case SDL_EventType.CONTROLLERDEVICEREMOVED:
                    RemoveController(evt.cdevice.which);
                    break;
                case SDL_EventType.JOYDEVICEADDED:
                    AddJoystick(evt.jdevice.which);
                    break;
                case SDL_EventType.JOYDEVICEREMOVED:
                    RemoveJoystick(evt.jdevice.which);
                    break;
                case SDL_EventType.CONTROLLERBUTTONDOWN:
                    HandleControllerButtonDownEvent(GetController(evt.cbutton.which), ref evt.cbutton);
                    break;
                case SDL_EventType.CONTROLLERBUTTONUP:
                    HandleControllerButtonUpEvent(GetController(evt.cbutton.which), ref evt.cbutton);
                    break;
                case SDL_EventType.CONTROLLERAXISMOTION:
                    HandleControllerAxisEvent(GetController(evt.caxis.which), ref evt.caxis);
                    break;
                case SDL_EventType.CONTROLLERTOUCHPADDOWN:
                    HandleControllerTouchpadDownEvent(GetController(evt.ctouchpad.which), ref evt.ctouchpad);
                    break;
                case SDL_EventType.CONTROLLERTOUCHPADUP:
                    HandleControllerTouchpadUpEvent(GetController(evt.ctouchpad.which), ref evt.ctouchpad);
                    break;
                case SDL_EventType.CONTROLLERTOUCHPADMOTION:
                    HandleControllerTouchpadMotionEvent(GetController(evt.ctouchpad.which), ref evt.ctouchpad);
                    break;
                case SDL_EventType.JOYBUTTONDOWN:
                    HandleJoystickButtonDownEvent(GetJoystick(evt.jbutton.which), ref evt.jbutton);
                    break;
                case SDL_EventType.JOYBUTTONUP:
                    HandleJoystickButtonUpEvent(GetJoystick(evt.jbutton.which), ref evt.jbutton);
                    break;
            }
        }
        CheckControllerButtonRepeats();
    }

    private static void CheckControllerButtonRepeats()
    {
        foreach (SDLController c in controllers)
        {
            c.HandleButtonRepeats(SDL_GetTicks());
        }
    }

    public static void CheckJoystick()
    {
        int state = SDL_JoystickEventState(SDL_QUERY);
        SDLLog.Debug(LogCategory.INPUT, "Joy State = {0}", state);
        state = SDL_GameControllerEventState(SDL_QUERY);
        SDLLog.Debug(LogCategory.INPUT, "Controller State = {0}", state);
    }

    private static void HandleWindowEvent(SDLWindow? window, ref SDL_WindowEvent evt)
    {
        if (window == null) return;
        switch (evt.windowEvent)
        {
            case SDL_WindowEventID.CLOSE: window.RaiseWindowClose(); break;
            case SDL_WindowEventID.DISPLAY_CHANGED:
                break;
            case SDL_WindowEventID.ENTER: window.RaiseWindowEnter(); break;
            case SDL_WindowEventID.EXPOSED: window.RaiseWindowExposed(); break;
            case SDL_WindowEventID.FOCUS_GAINED: window.RaiseWindowFocusGained(); break;
            case SDL_WindowEventID.FOCUS_LOST: window.RaiseWindowFocusLost(); break;
            case SDL_WindowEventID.HIDDEN: window.RaiseWindowHidden(); break;
            case SDL_WindowEventID.HIT_TEST:
                break;
            case SDL_WindowEventID.ICCPROF_CHANGED:
                break;
            case SDL_WindowEventID.LEAVE: window.RaiseWindowLeave(); break;
            case SDL_WindowEventID.MAXIMIZED: window.RaiseWindowMaximized(); break;
            case SDL_WindowEventID.MINIMIZED: window.RaiseWindowMinimized(); break;
            case SDL_WindowEventID.MOVED: window.RaiseWindowMoved(evt.data1, evt.data2); break;
            case SDL_WindowEventID.RESIZED: window.RaiseWindowResized(evt.data1, evt.data2, WindowResizeSource.Resized); break;
            case SDL_WindowEventID.RESTORED: window.RaiseWindowRestored(); break;
            case SDL_WindowEventID.SHOWN: window.RaiseWindowShown(); break;
            case SDL_WindowEventID.SIZE_CHANGED: window.RaiseWindowResized(evt.data1, evt.data2, WindowResizeSource.SizeChanged); break;
            case SDL_WindowEventID.TAKE_FOCUS: window.RaiseWindowTakeFocus(); break;
        }
    }
    private static void HandleMouseButtonDownEvent(SDLWindow? window, ref SDL_MouseButtonEvent evt)
    {
        if (window == null) return;
        window.RaiseMouseButtonDown(evt.which, evt.x, evt.y, (MouseButton)evt.button, evt.clicks, (KeyButtonState)evt.state);
    }
    private static void HandleMouseButtonUpEvent(SDLWindow? window, ref SDL_MouseButtonEvent evt)
    {
        if (window == null) return;
        window.RaiseMouseButtonUp(evt.which, evt.x, evt.y, (MouseButton)evt.button, evt.clicks, (KeyButtonState)evt.state);
    }
    private static void HandleMouseMotionEvent(SDLWindow? window, ref SDL_MouseMotionEvent evt)
    {
        if (window == null) return;
        window.RaiseMouseMove(evt.which, evt.x, evt.y, evt.xrel, evt.yrel);
    }
    private static void HandleMouseWheelEvent(SDLWindow? window, ref SDL_MouseWheelEvent evt)
    {
        if (window == null) return;
        window.RaiseMouseWheel(evt.which, evt.x, evt.y, evt.preciseX, evt.preciseY, (MouseWheelDirection)evt.direction);
    }

    private static void HandleKeyDownEvent(SDLWindow? window, ref SDL_KeyboardEvent evt)
    {
        if (window == null) return;
        window.RaiseKeyDown(evt.keysym.scancode, evt.keysym.sym, evt.keysym.mod, (KeyButtonState)evt.state, evt.repeat != 0);
    }
    private static void HandleKeyUpEvent(SDLWindow? window, ref SDL_KeyboardEvent evt)
    {
        if (window == null) return;
        window.RaiseKeyUp(evt.keysym.scancode, evt.keysym.sym, evt.keysym.mod, (KeyButtonState)evt.state, evt.repeat != 0);
    }
    private static void HandleTextInput(SDLWindow? window, ref SDL_TextInputEvent evt)
    {
        if (window == null) return;

        byte[] data = new byte[64];
        Marshal.Copy(evtMem, data, 0, 64);
        int length = 0;
        while (data[length + 12] != 0 && length < SDL_TEXTINPUTEVENT_TEXT_SIZE)
        {
            length++;
        }
        if (length > 0)
        {
            string str = Encoding.UTF8.GetString(data, 12, length);
            if (!string.IsNullOrEmpty(str))
            {
                window.RaiseTextInput(str);
            }
        }
    }

    private static void HandleJoystickButtonDownEvent(SDLJoystick? joystick, ref SDL_JoyButtonEvent evt)
    {
        if (joystick == null) return;
        if (joystick.Window == null) return;
        SDLLog.Debug(LogCategory.INPUT, "Joy Button {0} down", evt.button);
    }
    private static void HandleJoystickButtonUpEvent(SDLJoystick? joystick, ref SDL_JoyButtonEvent evt)
    {
        if (joystick == null) return;
        if (joystick.Window == null) return;
        SDLLog.Debug(LogCategory.INPUT, "Joy Button {0} up", evt.button);
    }

    private static void HandleControllerButtonDownEvent(SDLController? controller, ref SDL_ControllerButtonEvent evt)
    {
        if (controller == null) return;
        if (controller.Window == null) return;
        controller.HandleButtonEvent(evt.button, evt.state, evt.timestamp);
        //controller.Window.RaiseControllerButtonDown(controller, (ControllerButton)evt.button, (KeyButtonState)evt.state);
    }
    private static void HandleControllerButtonUpEvent(SDLController? controller, ref SDL_ControllerButtonEvent evt)
    {
        if (controller == null) return;
        if (controller.Window == null) return;
        controller.HandleButtonEvent(evt.button, evt.state, evt.timestamp);

        //controller.Window.RaiseControllerButtonUp(controller, (ControllerButton)evt.button, (KeyButtonState)evt.state);
    }

    private static void HandleControllerAxisEvent(SDLController? controller, ref SDL_ControllerAxisEvent evt)
    {
        if (controller == null) return;
        if (controller.Window == null) return;
        Vector2 dir = GetAxisDirection(controller.Handle, ref evt);
        if (dir == Vector2.Zero) return;
        controller.Window.RaiseControllerAxisEvent(controller, (ControllerAxis)evt.axis, evt.axisValue, dir);
    }

    private static void HandleControllerTouchpadDownEvent(SDLController? controller, ref SDL_ControllerTouchpadEvent evt)
    {
        if (controller == null) return;
        if (controller.Window == null) return;
        controller.Window.RaiseControllerTouchpadDownEvent(controller, evt.touchpad, evt.finger, evt.y, evt.y, evt.pressure);
    }
    private static void HandleControllerTouchpadUpEvent(SDLController? controller, ref SDL_ControllerTouchpadEvent evt)
    {
        if (controller == null) return;
        if (controller.Window == null) return;
        controller.Window.RaiseControllerTouchpadUpEvent(controller, evt.touchpad, evt.finger, evt.y, evt.y, evt.pressure);
    }
    private static void HandleControllerTouchpadMotionEvent(SDLController? controller, ref SDL_ControllerTouchpadEvent evt)
    {
        if (controller == null) return;
        if (controller.Window == null) return;
        controller.Window.RaiseControllerTouchpadMotionEvent(controller, evt.touchpad, evt.finger, evt.y, evt.y, evt.pressure);
    }

    private static void AddController(int which)
    {
        if (SDL_IsGameController(which) && useController)
        {
            IntPtr handle = SDL_GameControllerOpen(which);
            if (handle != IntPtr.Zero)
            {
                SDLController controller = new SDLController(which, handle);
                controller.Window = SDLApplication.MainWindow;
                controller.Name = Marshal.PtrToStringUTF8(SDL_GameControllerName(handle));
                controller.Mapping = Marshal.PtrToStringUTF8(SDL_GameControllerMapping(handle));
                controllers.Add(controller);
                SDLLog.Info(LogCategory.INPUT, "SDLController {0} ({1}) added", which, controller.Name);
            }
        }
    }

    private static void RemoveController(int which)
    {
        if (SDL_IsGameController(which) && useController)
        {
            SDLController? controller = GetController(which);
            if (controller != null)
            {
                controllers.Remove(controller);
                if (controller.Handle != IntPtr.Zero)
                {
                    SDL_GameControllerClose(controller.Handle);
                    SDLLog.Info(LogCategory.INPUT, "SDLController {0} removed", which);
                    return;
                }
            }
            SDLLog.Warn(LogCategory.INPUT, "Previously unknown SDLController {0} removed", which);
        }
    }

    private static void AddJoystick(int which)
    {
        IntPtr handle = SDL_JoystickOpen(which);
        if (handle != IntPtr.Zero)
        {
            SDLJoystick joystick = new SDLJoystick(which, handle);
            joystick.Window = SDLApplication.MainWindow;
            joystick.Name = Marshal.PtrToStringUTF8(SDL_JoystickName(handle));
            joysticks.Add(joystick);
            SDLLog.Info(LogCategory.INPUT, "SDLJoystick {0} ({1}) added", which, joystick.Name);

        }
    }

    private static void RemoveJoystick(int which)
    {
        SDLJoystick? joystick = GetJoystick(which);
        if (joystick != null)
        {
            joysticks.Remove(joystick);
            if (joystick.Handle != IntPtr.Zero)
            {
                SDL_JoystickClose(joystick.Handle);
                SDLLog.Info(LogCategory.INPUT, "SDLJoystick {0} removed", which);
                return;
            }
        }
        SDLLog.Warn(LogCategory.INPUT, "Previously unknown SDLJoystick {0} removed", which);
    }

    private static SDLController? GetController(int which)
    {
        return controllers.FirstOrDefault(x => x.Which == which);
    }

    private static SDLJoystick? GetJoystick(int which)
    {
        return joysticks.FirstOrDefault(x => x.Which == which);
    }

    private static Vector2 GetAxisDirection(IntPtr controller, ref SDL_ControllerAxisEvent evt)
    {
        Vector2 dir = Vector2.Zero;
        switch ((ControllerAxis)evt.axis)
        {
            case ControllerAxis.LEFTX:
                dir.X = evt.axisValue;
                dir.Y = SDL_GameControllerGetAxis(controller, ControllerAxis.LEFTY);
                break;
            case ControllerAxis.LEFTY:
                dir.X = SDL_GameControllerGetAxis(controller, ControllerAxis.LEFTX);
                dir.Y = evt.axisValue;
                break;
            case ControllerAxis.RIGHTX:
                dir.X = evt.axisValue;
                dir.Y = SDL_GameControllerGetAxis(controller, ControllerAxis.RIGHTY);
                break;
            case ControllerAxis.RIGHTY:
                dir.X = SDL_GameControllerGetAxis(controller, ControllerAxis.RIGHTX);
                dir.Y = evt.axisValue;
                break;
        }
        float length = dir.Length();
        if (length < controllerDeadZone)
        {
            dir = Vector2.Zero;
        }
        else
        {
            float f = (length - controllerDeadZone) / (controllerMaxValue - controllerDeadZone);
            if (f > 1.0f) { f = 1.0f; }
            if (f < 0.0f) { f = 0.0f; }
            dir *= f;
            dir = Vector2.Normalize(dir);
        }
        return dir;
    }


    private const string LibName = "SDL2";
    private enum SDL_EventType : uint
    {
        QUIT = 0x100,
        APP_TERMINATING,
        APP_LOWMEMORY,
        APP_WILLENTERBACKGROUND,
        APP_DIDENTERBACKGROUND,
        APP_WILLENTERFOREGROUND,
        APP_DIDENTERFOREGROUND,
        LOCALECHANGED,
        DISPLAYEVENT = 0x150,
        WINDOWEVENT = 0x200,
        SYSWMEVENT,
        KEYDOWN = 0x300,
        KEYUP,
        TEXTEDITING,
        TEXTINPUT,
        KEYMAPCHANGED,
        MOUSEMOTION = 0x400,
        MOUSEBUTTONDOWN,
        MOUSEBUTTONUP,
        MOUSEWHEEL,
        JOYAXISMOTION = 0x600,
        JOYBALLMOTION,
        JOYHATMOTION,
        JOYBUTTONDOWN,
        JOYBUTTONUP,
        JOYDEVICEADDED,
        JOYDEVICEREMOVED,
        CONTROLLERAXISMOTION = 0x650,
        CONTROLLERBUTTONDOWN,
        CONTROLLERBUTTONUP,
        CONTROLLERDEVICEADDED,
        CONTROLLERDEVICEREMOVED,
        CONTROLLERDEVICEREMAPPED,
        CONTROLLERTOUCHPADDOWN,
        CONTROLLERTOUCHPADMOTION,
        CONTROLLERTOUCHPADUP,
        CONTROLLERSENSORUPDATE,
        FINGERDOWN = 0x700,
        FINGERUP,
        FINGERMOTION,
        DOLLARGESTURE = 0x800,
        DOLLARRECORD,
        MULTIGESTURE,
        CLIPBOARDUPDATE = 0x900,
        DROPFILE = 0x1000,
        DROPTEXT,
        DROPBEGIN,
        DROPCOMPLETE,
        AUDIODEVICEADDED = 0x1100,
        AUDIODEVICEREMOVED,
        SENSORUPDATE = 0x1200,
        RENDER_TARGETS_RESET = 0x2000,
        RENDER_DEVICE_RESET,
        POLLSENTINEL = 0x7F00,
        USEREVENT = 0x8000
    }
    private enum SDL_WindowEventID : byte
    {
        NONE,
        SHOWN,
        HIDDEN,
        EXPOSED,
        MOVED,
        RESIZED,
        SIZE_CHANGED,
        MINIMIZED,
        MAXIMIZED,
        RESTORED,
        ENTER,
        LEAVE,
        FOCUS_GAINED,
        FOCUS_LOST,
        CLOSE,
        TAKE_FOCUS,
        HIT_TEST,
        ICCPROF_CHANGED,
        DISPLAY_CHANGED
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_QuitEvent
    {
        public SDL_EventType type;
        public uint timestamp;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_WindowEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int windowID;
        public SDL_WindowEventID windowEvent;
        private byte padding1;
        private byte padding2;
        private byte padding3;
        public int data1;
        public int data2;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_MouseMotionEvent
    {
        public SDL_EventType type;
        public int timestamp;
        public uint windowID;
        public int which;
        public byte state;
        private byte padding1;
        private byte padding2;
        private byte padding3;
        public int x;
        public int y;
        public int xrel;
        public int yrel;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_MouseButtonEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int windowID;
        public int which;
        public byte button;
        public byte state;
        public byte clicks;
        private byte padding1;
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_MouseWheelEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int windowID;
        public int which;
        public int x;
        public int y;
        public uint direction;
        public float preciseX;
        public float preciseY;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_Keysym
    {
        public ScanCode scancode;
        public KeyCode sym;
        public KeyMod mod;
        public uint unicode;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_KeyboardEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int windowID;
        public byte state;
        public byte repeat;
        private byte padding2;
        private byte padding3;
        public SDL_Keysym keysym;
    }

    private const int SDL_TEXTINPUTEVENT_TEXT_SIZE = 32;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct SDL_TextInputEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int windowID;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = SDL_TEXTINPUTEVENT_TEXT_SIZE)]
        //public byte[] text;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_ControllerAxisEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int which;
        public byte axis;
        private byte padding1;
        private byte padding2;
        private byte padding3;
        public short axisValue;
        private uint padding4;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_ControllerButtonEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int which;
        public byte button;
        public byte state;
        private byte padding1;
        private byte padding2;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_ControllerDeviceEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int which;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_ControllerTouchpadEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int which;
        public int touchpad;
        public int finger;
        public float x;
        public float y;
        public float pressure;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_ControllerSensorEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int which;
        public int sensor;
        public float data1;
        public float data2;
        public float data3;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_TouchFingerEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public long touchId;
        public long fingerId;
        public float x;
        public float y;
        public float dx;
        public float dy;
        public float pressure;
        public int windowID;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_MultiGestureEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public long touchId;
        public float dTheta;
        public float dDist;
        public float x;
        public float y;
        public ushort numFingers;
        public ushort padding;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_DollarGestureEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public long touchId;
        public long gestureId;
        public uint numFingers;
        public float error;
        public float x;
        public float y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_JoyAxisEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int which;
        public byte axis;
        private byte padding1;
        private byte padding2;
        private byte padding3;
        public short axisValue;
        public ushort padding4;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_JoyBallEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int which;
        public byte ball;
        private byte padding1;
        private byte padding2;
        private byte padding3;
        public short xrel;
        public short yrel;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_JoyHatEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int which;
        public byte hat;
        public byte hatValue;
        private byte padding1;
        private byte padding2;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_JoyButtonEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int which;
        public byte button;
        public byte state;
        private byte padding1;
        private byte padding2;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SDL_JoyDeviceEvent
    {
        public SDL_EventType type;
        public uint timestamp;
        public int which;
    }


    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 64)]
    private struct SDL_Event
    {
        [FieldOffset(0)]
        public SDL_EventType type;
        [FieldOffset(0)]
        public SDL_QuitEvent quit;
        [FieldOffset(0)]
        public SDL_WindowEvent window;
        [FieldOffset(0)]
        public SDL_MouseMotionEvent motion;
        [FieldOffset(0)]
        public SDL_MouseButtonEvent button;
        [FieldOffset(0)]
        public SDL_MouseWheelEvent wheel;
        [FieldOffset(0)]
        public SDL_KeyboardEvent key;
        [FieldOffset(0)]
        public SDL_TextInputEvent text;
        [FieldOffset(0)]
        public SDL_ControllerAxisEvent caxis;
        [FieldOffset(0)]
        public SDL_ControllerButtonEvent cbutton;
        [FieldOffset(0)]
        public SDL_ControllerDeviceEvent cdevice;
        [FieldOffset(0)]
        public SDL_ControllerTouchpadEvent ctouchpad;
        [FieldOffset(0)]
        public SDL_ControllerSensorEvent csensor;
        [FieldOffset(0)]
        public SDL_TouchFingerEvent tfinger;
        [FieldOffset(0)]
        public SDL_MultiGestureEvent mgesture;
        [FieldOffset(0)]
        public SDL_DollarGestureEvent dgesture;
        [FieldOffset(0)]
        public SDL_JoyAxisEvent jaxis;
        [FieldOffset(0)]
        public SDL_JoyBallEvent jball;
        [FieldOffset(0)]
        public SDL_JoyHatEvent jhat;
        [FieldOffset(0)]
        public SDL_JoyButtonEvent jbutton;
        [FieldOffset(0)]
        public SDL_JoyDeviceEvent jdevice;

    }

    private const int SDL_QUERY = -1;
    private const int SDL_IGNORE = 0;
    private const int SDL_DISABLE = 0;
    private const int SDL_ENABLE = 1;


    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int SDL_PollEvent(out SDL_Event _event);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SDL_IsGameController(int joystick_index);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_GameControllerClose(IntPtr gamecontroller);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr SDL_GameControllerOpen(int joystick_index);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern short SDL_GameControllerGetAxis(IntPtr gamecontroller, ControllerAxis axis);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr SDL_GameControllerName(IntPtr gamecontroller);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint SDL_GetTicks();
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int SDL_GameControllerAddMapping([MarshalAs(UnmanagedType.LPUTF8Str)] string mappingString);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int SDL_GameControllerNumMappings();
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr INTERNAL_SDL_GameControllerMappingForIndex(int mapping_index);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int SDL_GameControllerAddMappingsFromRW(IntPtr rw, int freerw);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int SDL_GameControllerAddMappingsFromFile([MarshalAs(UnmanagedType.LPUTF8Str)] string filename);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr SDL_GameControllerMappingForGUID(Guid guid);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr SDL_GameControllerMapping(IntPtr gamecontroller);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr SDL_GameControllerNameForIndex(int joystick_index);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr SDL_GameControllerMappingForDeviceIndex(int joystick_index);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern ushort SDL_GameControllerGetVendor(IntPtr gamecontroller);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern ushort SDL_GameControllerGetProduct(IntPtr gamecontroller);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern ushort SDL_GameControllerGetProductVersion(IntPtr gamecontroller);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr SDL_GameControllerGetSerial(IntPtr gamecontroller);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SDL_GameControllerGetAttached(IntPtr gamecontroller);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr SDL_JoystickOpen(int device_index);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SDL_JoystickClose(IntPtr joystick);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr SDL_JoystickName(IntPtr joystick);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int SDL_GameControllerEventState(int state);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SDL_GameControllerUpdate();
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int SDL_JoystickEventState(int state);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SDL_JoystickUpdate();


}
