namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public enum MouseButton
    {
        None = 0,
        Left = 1,
        Middle = 2,
        Right = 3,
        X1 = 4,
        X2 = 5
    }

    public enum KeyButtonState
    {
        Released = 0,
        Pressed = 1
    }

    public enum MouseWheelDirection
    {
        Normal = 0,
        Flipped = 1
    }
    [Flags]
    public enum BlendMode
    {
        None = 0x00000000,
        Blend = 0x00000001,
        Add = 0x00000002,
        Mod = 0x00000004,
        Mul = 0x00000008,
        Invalid = 0x7FFFFFFF
    }

    [Flags]
    public enum RendererFlip
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2,
        Both = Horizontal | Vertical
    }

    public enum TextureFilter
    {
        Linear,
        Nearest,
        Best
    }

    public enum RendererSizeMode
    {
        Window,
        BackBuffer
    }

    [Flags]
    public enum FontStyle
    {
        Normal = 0x00,
        Bold = 0x01,
        Italic = 0x02,
        Underline = 0x04,
        Strikethrough = 0x08
    }

    public enum FontHinting
    {
        Normal = 0,
        Light = 1,
        Mono = 2,
        None = 3,
        LightSubPixel = 4
    }

    public enum HorizontalAlignment
    {
        Left,
        Right,
        Center,
        Stretch
    }

    public enum VerticalAlignment
    {
        Top,
        Bottom,
        Center,
        Stretch
    }

    public enum WindowCloseOperation
    {
        Close,
        Exit,
        DoNothing
    }

    public enum ControllerButton
    {
        Invalid = -1,
        A = 0,
        B = 1,
        X = 2,
        Y = 3,
        Back = 4,
        Guide = 5,
        Start = 6,
        LeftStick = 7,
        RightStick = 8,
        LeftShoulder = 9,
        RightShoulder = 10,
        DPadUp = 11,
        DPadDown = 12,
        DPadLeft = 13,
        DpadRight = 14,
        Misc = 15,
        Paddle1 = 16,
        Paddle2 = 17,
        Paddle3 = 18,
        Paddle4 = 18,
        TouchPad = 20,
        Max = 21
    }

    public enum ControllerAxis
    {
        INVALID = -1,
        LEFTX,
        LEFTY,
        RIGHTX,
        RIGHTY,
        TRIGGERLEFT,
        TRIGGERRIGHT,
        MAX
    }

    public enum FullScreenMode
    {
        Desktop,
        FullSize,
        MultiMonitor
    }
    public enum MusicType
    {
        None = 0,
        Cmd = 1,
        Wav = 2,
        Mod = 3,
        Mid = 4,
        Ogg = 5,
        MP3 = 6,
        //MP3_MAD_UNUSED,
        Flac = 8,
        //MODPLUG_UNUSED,
        Opus = 10
    }

    public enum LogPriority
    {
        Verbose = 1,
        Debug,
        Info,
        Warn,
        Error,
        Critical
    }

    public enum LogCategory
    {
        APPLICATION,
        ERROR,
        ASSERT,
        SYSTEM,
        AUDIO,
        VIDEO,
        RENDER,
        INPUT,
        TEST,
        RESERVED1,
        RESERVED2,
        RESERVED3,
        RESERVED4,
        RESERVED5,
        RESERVED6,
        RESERVED7,
        RESERVED8,
        RESERVED9,
        RESERVED10,
        CUSTOM,
        FONT,
        SDLC,
    }

    public enum MusicFinishReason
    {
        Finished,
        Interrupted
    }
}
