namespace SDLC.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGUIScreen : IBox
    {
        IEnumerable<IGUIWindow> Windows { get; }
        string? Title { get; set; }
    }
}
