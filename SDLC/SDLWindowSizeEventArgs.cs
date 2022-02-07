namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLWindowSizeEventArgs : EventArgs
    {
        private readonly int width;
        private readonly int height;
        public SDLWindowSizeEventArgs(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public int Width => width;
        public int Height => height;

    }

    public delegate void SDLWindowSizeEventHandler(object sender, SDLWindowSizeEventArgs e);

}
