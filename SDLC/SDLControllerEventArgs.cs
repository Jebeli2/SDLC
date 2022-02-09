namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLControllerEventArgs : SDLHandledEventArgs
    {
        private readonly SDLController controller;

        public SDLControllerEventArgs(SDLController controller)
        {
            this.controller = controller;
        }

        public SDLController Controller => controller;
    }
}
