namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLTextInputEventArgs : EventArgs
    {
        private readonly string text;
        public SDLTextInputEventArgs(string text)
        {
            this.text = text;
        }

        public string Text => text;

    }

    public delegate void SDLTextInputEventHandler(object sender, SDLTextInputEventArgs e);

}
