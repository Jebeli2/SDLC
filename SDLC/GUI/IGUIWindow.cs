namespace SDLC.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGUIWindow : IBox
    {
        IGUIScreen Screen { get; }
        IEnumerable<IGUIGadget> Gadgets { get; }
        WindowFlags WindowFlags { get; set; }
        bool Borderless { get; set; }
        bool Active { get; set; }
        bool MouseHover { get; set; }
        string? Title { get; set; }

    }
}
