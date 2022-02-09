namespace SDLC.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGUIGadget : IBox
    {
        IGUIWindow Window { get; }
        GadgetFlags Flags { get; set; }
        GadgetActivation Activation { get; set; }
        bool Active { get; set; }
        bool MouseHover { get; set; }
        bool IsBorderGadget { get; }
        bool RelVeriy { get; }
        bool Immediate { get; }
        bool TransparentBackground { get; set; }
        string? Text { get; set; }
        Icons Icon { get; set; }
    }
}
