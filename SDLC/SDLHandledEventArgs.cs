namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLHandledEventArgs : EventArgs
    {
        private bool handled;

        public bool Handled
        {
            get => handled;
            set => handled = value;
        }

    }
}
