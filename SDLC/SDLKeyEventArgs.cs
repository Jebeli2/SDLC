namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLKeyEventArgs : SDLHandledEventArgs
    {
        private readonly ScanCode scanCode;
        private readonly KeyCode keyCode;
        private readonly KeyMod keyMod;
        private readonly KeyButtonState state;
        private readonly bool repeat;

        public SDLKeyEventArgs(ScanCode scanCode, KeyCode keyCode, KeyMod keyMod, KeyButtonState state, bool repeat)
        {
            this.scanCode = scanCode;
            this.keyCode = keyCode;
            this.keyMod = keyMod;
            this.state = state;
            this.repeat = repeat;
        }

        public ScanCode ScanCode => scanCode;
        public KeyCode KeyCode => keyCode;
        public KeyMod KeyMod => keyMod;
        public KeyButtonState State => state;
        public bool Repeat => repeat;

    }

    public delegate void SDLKeyEventHandler(object sender, SDLKeyEventArgs e);
}
