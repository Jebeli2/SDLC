namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLWindowLoadEventArgs : EventArgs
    {
        private readonly SDLRenderer renderer;
        public SDLWindowLoadEventArgs(SDLRenderer renderer)
        {
            this.renderer = renderer;
        }
        public SDLRenderer Renderer => renderer;
    }

    public delegate void SDLWindowLoadEventHandler(object sender, SDLWindowLoadEventArgs e);
}
