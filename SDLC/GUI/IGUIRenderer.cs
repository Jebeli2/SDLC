namespace SDLC.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGUIRenderer
    {

        void RenderScreen(IRenderer gfx, Screen screen, int offsetX, int offsetY);
        void RenderWindow(IRenderer gfx, Window window, int offsetX, int offsetY);
        void RenderGadget(IRenderer gfx, Gadget gadget, int offsetX, int offsetY);
    }
}
