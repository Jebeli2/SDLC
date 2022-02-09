namespace SDLC.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGUIRenderer
    {

        void RenderScreen(IRenderer gfx, Screen screen);
        void RenderWindow(IRenderer gfx, Window window);
        void RenderGadget(IRenderer gfx, Gadget gadget);
    }
}
