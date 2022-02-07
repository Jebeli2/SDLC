namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLMouseWheelEventArgs : EventArgs
    {
        private readonly int which;
        private readonly int x;
        private readonly int y;
        private readonly float preciseX;
        private readonly float preciseY;
        private readonly MouseWheelDirection direction;

        public SDLMouseWheelEventArgs(int which, int x, int y, float preciseX, float preciseY, MouseWheelDirection direction)
        {
            this.which = which;
            this.x = x;
            this.y = y;
            this.preciseX = preciseX;
            this.preciseY = preciseY;
            this.direction = direction;
        }

        public int Which => which;
        public int X => x;
        public int Y => y;
        public float PreciseX => preciseX;
        public float PreciseY => preciseY;
        public MouseWheelDirection Direction => direction;

    }

    public delegate void SDLMouseWheelEventHandler(object sender, SDLMouseWheelEventArgs e);
}
