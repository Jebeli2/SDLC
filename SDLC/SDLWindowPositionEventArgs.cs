namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLWindowPositionEventArgs : EventArgs
    {
        private readonly int x;
        private readonly int y;

        public SDLWindowPositionEventArgs(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X => x;
        public int Y => y;
    }

    public delegate void SDLWindowPositionEventHandler(object sender, SDLWindowPositionEventArgs e);

}
