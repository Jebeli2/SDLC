namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLScreen : IScreen
    {
        private readonly Configuration configuration = new();
        private string name;


        private int width;
        private int height;
        private IWindow? window;
        private IRenderer? renderer;

        public SDLScreen(string name = "SDLScreen")
        {
            this.name = name;
        }
        public string Name => name;

        public Configuration Configuration => configuration;

        public Configuration GetConfiguration()
        {
            return configuration;
        }

        public virtual void Hide(IWindow window)
        {
            SDLLog.Info($"Hide Screen {name}");
        }

        public virtual void Initialize(IWindow window)
        {
            SDLLog.Info($"Initialize Screen {name}");
            this.window = window;
            renderer = window.Renderer;
            width = window.Width;
            height = window.Height;
        }

        public virtual void Pause(IWindow window)
        {
            SDLLog.Info($"Pause Screen {name}");
        }

        public virtual void Render(IRenderer renderer, double totalTime, double elapsedTime)
        {
        }

        public virtual void Resized(IWindow window, int width, int height)
        {
            SDLLog.Info($"Resized Screen {name} ({width}x{height})");
            this.width = width;
            this.height = height;
        }

        public virtual void Resume(IWindow window)
        {
            SDLLog.Info($"Resume Screen {name}");
        }

        public virtual void Show(IWindow window)
        {
            SDLLog.Info($"Show Screen {name}");
        }

        public virtual void Shutdown(IWindow window)
        {
            SDLLog.Info($"Shutdown Screen {name}");
        }

        public virtual void Update(IRenderer renderer, double totalTime, double elapsedTime)
        {
        }

        #region Convenience Methods for inheritors
        protected int Width => width;
        protected int Height => height;

        protected SDLTexture? LoadTexture(string name)
        {
            return renderer?.LoadTexture(name);
        }

        protected SDLTexture? LoadTexture(string name, byte[]? data)
        {
            return renderer?.LoadTexture(name, data);
        }

        protected SDLMusic? LoadMusic(string name)
        {
            return SDLAudio.LoadMusic(name);
        }
        protected SDLMusic? LoadMusic(string name, byte[]? data)
        {
            return SDLAudio.LoadMusic(name, data);
        }

        protected void AddApplet(SDLApplet applet)
        {
            window?.AddApplet(applet);
        }
        protected void RemoveApplet(SDLApplet applet)
        {
            window?.RemoveApplet(applet);
        }

        #endregion

    }
}
