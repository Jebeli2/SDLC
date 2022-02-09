namespace SDLC.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGUIRenderer
    {

        void RenderScreen(IRenderer gfx, IGUIScreen screen);
        void RenderWindow(IRenderer gfx, IGUIWindow window);
        void RenderGadget(IRenderer gfx, IGUIGadget gadget);
    }
}
