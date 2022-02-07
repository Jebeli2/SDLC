namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLControllerAxisEventArgs : SDLControllerEventArgs
    {
        private readonly ControllerAxis axis;
        private readonly int axisValue;
        private readonly Vector2 direction;
        public SDLControllerAxisEventArgs(SDLController controller, ControllerAxis axis, int axisValue, Vector2 direction)
            : base(controller)
        {
            this.axis = axis;
            this.axisValue = axisValue;
            this.direction = direction;
        }

        public ControllerAxis Axis => axis;
        public int AxisValue => axisValue;
        public Vector2 Direction => direction;
    }

    public delegate void SDLControllerAxisEventHandler(object sender, SDLControllerAxisEventArgs e);
}
