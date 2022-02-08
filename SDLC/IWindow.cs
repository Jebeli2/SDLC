namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IWindow
    {
        int WindowId { get; }
        IntPtr Handle { get; }
        bool HandleCreated { get; }
        IRenderer Renderer { get; }
        IScreen Screen { get; set; }
        int Width { get; }
        int Height { get; }

        void AddApplet(SDLApplet applet);
        void RemoveApplet(SDLApplet applet);
    }
}
