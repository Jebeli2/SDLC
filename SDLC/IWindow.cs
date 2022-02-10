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
        IContentManager ContentManager { get; }
        IScreen Screen { get; set; }
        int Width { get; }
        int Height { get; }
        bool FullScreen { get; set; }

        T GetApplet<T>() where T : SDLApplet, new();
        void AddApplet(SDLApplet applet);
        void RemoveApplet(SDLApplet applet);
        void ChangeApplet(SDLApplet applet);
    }
}
