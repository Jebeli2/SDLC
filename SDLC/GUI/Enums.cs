namespace SDLC.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Flags]
    public enum GadgetFlags
    {
        None = 0x0000,
        RelBottom = 0x0008,
        RelRight = 0x0010,
        RelWidth = 0x0020,
        RelHeight = 0x0040,
        Selected = 0x0080,
        Disabled = 0x0100,
    }

    [Flags]
    public enum GadgetActivation
    {
        None = 0x0000,
        RelVerify = 0x0001,
        Immediate = 0x0002,
        MouseHover = 0x0008,
        RightBorder = 0x0010,
        LeftBorder = 0x0020,
        TopBorder = 0x0040,
        BottomBorder = 0x0080,
        ToggleSelect = 0x01000,
        ActiveGadget = 0x4000
    }

    [Flags]
    public enum WindowFlags
    {
        None = 0x0000,
        SizeGadget = 0x0001,
        DragBar = 0x0002,
        DepthGadget = 0x0004,
        CloseGadget = 0x0008,
        SizeBRight = 0x0010,
        SizeBBottom = 0x0020,
        BackDrop = 0x0100,
        ReportMouse = 0x0200,
        Borderless = 0x0400,
        Activate = 0x0800,
        WindowActive = 0x2000,
        MouseHover = 0x40000,
        Zoomed = 0x10000000,
        HasZoom = 0x20000000
    }

}
