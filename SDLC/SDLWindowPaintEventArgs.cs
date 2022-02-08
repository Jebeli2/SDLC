namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLWindowPaintEventArgs : EventArgs
    {
        private readonly double totalTime;
        private readonly double elapsedTime;
        private readonly IRenderer renderer;
        private readonly int width;
        private readonly int height;
        public SDLWindowPaintEventArgs(IRenderer renderer, int width, int height, double totalTime, double elapsedTime)
        {
            this.renderer = renderer;
            this.width = width;
            this.height = height;
            this.totalTime = totalTime;
            this.elapsedTime = elapsedTime;
        }
        public IRenderer Renderer => renderer;
        public int Width => width;
        public int Height => height;
        public double TotalTime => totalTime;
        public double ElapsedTime => elapsedTime;

    }

    public delegate void SDLWindowPaintEventHandler(object sender, SDLWindowPaintEventArgs e);

}
