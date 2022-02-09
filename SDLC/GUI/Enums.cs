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
    public enum GadgetType
    {
        GadgetType = 0xFC00,
        ScrGadget = 0x4000,
        GzzGadget = 0x2000,
        ReqGadget = 0x1000,
        SysGadget = 0x8000,
        SysTypeMask = 0xF0,
        Sizing = 0x10,
        WDragging = 0x20,
        SDragging = 0x30,
        WDepth = 0x40,
        SDepth = 0x50,
        WZoom = 0x60,
        SUnused = 0x70,
        Close = 0x80,
        GTypeMask = 0x7,
        BoolGadget = 0x1,
        Gadget0002 = 0x2,
        PropGadget = 0x3,
        StrGadget = 0x4,
        CustomGadget = 0x5
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

    [Flags]
    public enum PropFlags
    {
        FreeHoriz = 0x0002,
        FreeVert = 0x0004,
        PropBorderless = 0x0008,
        KnobHit = 0x0100
    }
}
