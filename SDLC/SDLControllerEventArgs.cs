namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLControllerEventArgs : EventArgs
    {
        private readonly SDLController controller;

        public SDLControllerEventArgs(SDLController controller)
        {
            this.controller = controller;
        }

        public SDLController Controller => controller;
    }
}
