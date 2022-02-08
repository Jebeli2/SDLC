namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLWindowLoadEventArgs : EventArgs
    {
        private readonly IRenderer renderer;
        public SDLWindowLoadEventArgs(IRenderer renderer)
        {
            this.renderer = renderer;
        }
        public IRenderer Renderer => renderer;
    }

    public delegate void SDLWindowLoadEventHandler(object sender, SDLWindowLoadEventArgs e);
}
