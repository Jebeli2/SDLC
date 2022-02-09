namespace SDLC.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGUISystem
    {
        IGUIScreen OpenScreen();
        void CloseScreen(IGUIScreen screen);


        IGUIWindow OpenWindow(IGUIScreen screen,
            int leftEdge = 0,
            int topEdge = 0,
            int width = 256,
            int height = 256,
            string title = "",
            int minWidth = 0,
            int minHeight = 0);
        void CloseWindow(IGUIWindow window);

        IGUIGadget AddGadget(IGUIWindow window,
            int leftEdge = 0,
            int topEdge = 0,
            int width = 100,
            int height = 50,
            GadgetFlags flags = GadgetFlags.None,
            GadgetActivation activation = GadgetActivation.Immediate | GadgetActivation.RelVerify,
            string? text = null,
            bool disabled = false,
            int gadgetId = -1);

        void RemoveGadget(IGUIWindow window, IGUIGadget gadget);
    }
}
