namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLController
    {
        private readonly int which;
        private readonly IntPtr handle;
        private SDLWindow? window;
        private string? name;

        public SDLController(int which, IntPtr handle)
        {
            this.which = which;
            this.handle = handle;
        }

        public int Which => which;
        public IntPtr Handle => handle;
        internal SDLWindow? Window
        {
            get => window;
            set => window = value;
        }

        public string? Name
        {
            get => name;
            set => name = value;
        }



    }
}
