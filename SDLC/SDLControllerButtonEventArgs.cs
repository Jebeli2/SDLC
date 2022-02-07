namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLControllerButtonEventArgs : SDLControllerEventArgs
    {
        private readonly ControllerButton button;
        private readonly KeyButtonState state;
        public SDLControllerButtonEventArgs(SDLController controller, ControllerButton button, KeyButtonState state)
            : base(controller)
        {
            this.button = button;
            this.state = state;
        }

        public ControllerButton Button => button;
        public KeyButtonState State => state;
    }

    public delegate void SDLControllerButtonEventHandler(object sender, SDLControllerButtonEventArgs e);
}
