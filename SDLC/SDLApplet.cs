namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLApplet : IDisposable
    {
        private readonly string name;
        private bool installed;
        private bool enabled;
        private int width;
        private int height;
        private IWindow? window;
        private IRenderer? renderer;
        private IContentManager? contentManager;
        private int renderPrio;
        private int inputPrio;
        protected bool noInput;
        protected bool noRender;
        private bool disposedValue;

        public SDLApplet(string name)
        {
            this.name = name;
            enabled = true;
        }

        public string Name => name;
        public int Width => width;
        public int Height => height;

        public int RenderPrio
        {
            get => renderPrio;
            set
            {
                if (renderPrio != value)
                {
                    renderPrio = value;
                    AppletChanged();
                }
            }
        }

        public int InputPrio
        {
            get => inputPrio;
            set
            {
                if (inputPrio != value)
                {
                    inputPrio = value;
                    AppletChanged();
                }
            }
        }

        public bool NoInput => noInput;
        public bool NoRender => noRender;

        public bool Installed
        {
            get => installed;
        }
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    AppletChanged();
                }
            }
        }

        protected void AppletChanged()
        {
            window?.ChangeApplet(this);
        }

        protected SDLMusic? LoadMusic(string name)
        {
            byte[]? data = contentManager?.FindContent(name);
            return SDLAudio.LoadMusic(name, data);
        }
        protected SDLMusic? LoadMusic(string name, byte[]? data)
        {
            return SDLAudio.LoadMusic(name, data);
        }

        protected SDLTexture? LoadTexture(string name)
        {
            byte[]? data = contentManager?.FindContent(name);
            return renderer?.LoadTexture(name, data);
        }
        protected SDLTexture? LoadTexture(string name, byte[]? data)
        {
            return renderer?.LoadTexture(name, data);
        }

        internal void OnInstall(IWindow window)
        {
            this.window = window;
            renderer = this.window.Renderer;
            contentManager = this.window.ContentManager;
            installed = true;
            InternalOnLoad(new SDLWindowLoadEventArgs(renderer));
        }

        internal void OnUninstall(IWindow window)
        {
            installed = false;
        }
        internal bool Shown { get; set; }
        internal protected virtual void OnWindowShown(EventArgs e) { }
        internal protected virtual void OnWindowHidden(EventArgs e) { }
        internal protected virtual void OnWindowExposed(EventArgs e) { }
        internal protected virtual void OnWindowMinimized(EventArgs e) { }
        internal protected virtual void OnWindowMaximized(EventArgs e) { }
        internal protected virtual void OnWindowRestored(EventArgs e) { }
        internal protected virtual void OnWindowClose(EventArgs e) { }
        internal protected virtual void OnWindowEnter(EventArgs e) { }
        internal protected virtual void OnWindowLeave(EventArgs e) { }
        internal protected virtual void OnWindowFocusGained(EventArgs e) { }
        internal protected virtual void OnWindowFocusLost(EventArgs e) { }
        internal protected virtual void OnWindowTakeFocus(EventArgs e) { }
        protected virtual void OnWindowUpdate(SDLWindowUpdateEventArgs e) { }
        protected virtual void OnWindowPaint(SDLWindowPaintEventArgs e) { }
        protected virtual void OnWindowLoad(SDLWindowLoadEventArgs e) { }
        internal protected virtual void OnWindowMoved(SDLWindowPositionEventArgs e) { }
        internal protected virtual void OnWindowResized(SDLWindowSizeEventArgs e) { }
        internal protected virtual void OnMouseButtonDown(SDLMouseEventArgs e) { }
        internal protected virtual void OnMouseButtonUp(SDLMouseEventArgs e) { }
        internal protected virtual void OnMouseMove(SDLMouseEventArgs e) { }
        internal protected virtual void OnMouseWheel(SDLMouseWheelEventArgs e) { }
        internal protected virtual void OnKeyDown(SDLKeyEventArgs e) { }
        internal protected virtual void OnKeyUp(SDLKeyEventArgs e) { }
        internal protected virtual void OnTextInput(SDLTextInputEventArgs e) { }
        internal protected virtual void OnControllerButtonDown(SDLControllerButtonEventArgs e) { }
        internal protected virtual void OnControllerButtonUp(SDLControllerButtonEventArgs e) { }
        internal protected virtual void OnControllerAxis(SDLControllerAxisEventArgs e) { }
        internal protected virtual void OnControllerTouchpadDown(SDLControllerTouchpadEventArgs e) { }
        internal protected virtual void OnControllerTouchpadUp(SDLControllerTouchpadEventArgs e) { }
        internal protected virtual void OnControllerTouchpadMove(SDLControllerTouchpadEventArgs e) { }
        protected virtual void OnDispose() { }

        internal void InternalOnUpdate(SDLWindowUpdateEventArgs e)
        {
            OnWindowUpdate(e);
        }
        internal void InternalOnPaint(SDLWindowPaintEventArgs e)
        {
            renderer = e.Renderer;
            width = e.Renderer.Width;
            height = e.Renderer.Height;
            OnWindowPaint(e);
        }
        internal void InternalOnLoad(SDLWindowLoadEventArgs e)
        {
            renderer = e.Renderer;
            width = e.Renderer.Width;
            height = e.Renderer.Height;
            OnWindowLoad(e);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OnDispose();
                }

                disposedValue = true;
            }
        }

        ~SDLApplet()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
