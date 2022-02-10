namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLWindowPaintEventArgs : EventArgs
    {
        private readonly IRenderer renderer;
        private double totalTime;
        private double elapsedTime;
        public SDLWindowPaintEventArgs(IRenderer renderer, double totalTime, double elapsedTime)
        {
            this.renderer = renderer;
            this.totalTime = totalTime;
            this.elapsedTime = elapsedTime;
        }
        public IRenderer Renderer
        {
            get => renderer;
        }
        public double TotalTime => totalTime;
        public double ElapsedTime => elapsedTime;
        internal void Update(double totalTime, double elapsedTime)
        {
            this.totalTime = totalTime;
            this.elapsedTime = elapsedTime;
        }

    }

    public delegate void SDLWindowPaintEventHandler(object sender, SDLWindowPaintEventArgs e);

}
