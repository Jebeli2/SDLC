namespace SDLC.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGUISystem
    {
        Screen OpenScreen(bool keepOldScreens = false);
        void CloseScreen(Screen screen);


        Window OpenWindow(Screen screen,
            int leftEdge = 0,
            int topEdge = 0,
            int width = 256,
            int height = 256,
            string title = "",
            int minWidth = 0,
            int minHeight = 0);
        void CloseWindow(Window window);

        Gadget AddGadget(Window window,
            int leftEdge = 0,
            int topEdge = 0,
            int width = 100,
            int height = 50,
            GadgetFlags flags = GadgetFlags.None,
            GadgetActivation activation = GadgetActivation.Immediate | GadgetActivation.RelVerify,
            GadgetType type = GadgetType.BoolGadget,
            string? text = null,
            bool disabled = false,
            bool selected = false,
            bool toggleSelect = false,
            Action? clickAction = null,
            int gadgetId = -1);

        void RemoveGadget(Window window, Gadget gadget);

        void WindowToFront(Window window);
        void WindowToBack(Window window);
        void ActivateWindow(Window window);

        void ModifyProp(Gadget gadget, PropFlags flags, int horizPot, int vertPot, int horizBody, int vertBody);
    }
}
