namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLControllerTouchpadEventArgs : SDLControllerEventArgs
    {
        private readonly int touchpad;
        private readonly int finger;
        private readonly float x;
        private readonly float y;
        private readonly float pressure;
        public SDLControllerTouchpadEventArgs(SDLController controller, int touchpad, int finger, float x, float y, float pressure)
            : base(controller)
        {
            this.touchpad = touchpad;
            this.finger = finger;
            this.x = x;
            this.y = y;
            this.pressure = pressure;
        }

        public int Touchpad => touchpad;
        public int Finger => finger;
        public float X => x;
        public float Y => y;
        public float Pressure => pressure;

    }

    public delegate void SDLControllerTouchpadEventHandler(object sender, SDLControllerTouchpadEventArgs e);
}
