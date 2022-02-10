namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLWindowUpdateEventArgs : EventArgs
    {
        private double totalTime;
        private double elapsedTime;

        public SDLWindowUpdateEventArgs(double totalTime, double elapsedTime)
        {
            this.totalTime = totalTime;
            this.elapsedTime = elapsedTime;
        }

        public double TotalTime => totalTime;
        public double ElapsedTime => elapsedTime;

        internal void Update(double totalTime, double elapsedTime)
        {
            this.totalTime = totalTime;
            this.elapsedTime = elapsedTime;
        }
    }

    public delegate void SDLWindowUpdateEventHandler(object sender, SDLWindowUpdateEventArgs e);

}
