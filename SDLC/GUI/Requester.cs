namespace SDLC.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Requester : GUIObject
    {
        private readonly Window window;
        private readonly List<Gadget> gadgets = new();
        internal Requester(IGUISystem gui, Window window)
            : base(gui)
        {
            this.window = window;
        }

        public Window Window => window;

        public IEnumerable<Gadget> Gadgets => gadgets;
    }
}
