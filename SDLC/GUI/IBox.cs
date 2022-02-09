namespace SDLC.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IBox
    {
        int LeftEdge { get; set; }
        int TopEdge { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        int BorderTop { get; set; }
        int BorderLeft { get; set; }
        int BorderRight { get; set; }
        int BorderBottom { get; set; }
        bool Contains(int x, int y);
        Rectangle GetBounds();
        Rectangle GetInnerBounds();
    }
}
