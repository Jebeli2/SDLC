namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLApplet
    {
        private readonly string name;
        private bool installed;
        private bool enabled;
        private int width;
        private int height;
        private IRenderer? renderer;

        public SDLApplet(string name)
        {
            this.name = name;
            enabled = true;
        }

        public string Name => name;
        public int Width => width;
        public int Height => height;

        public bool Installed
        {
            get => installed;
            internal set => installed = value;
        }
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        protected SDLTexture? LoadTexture(string name)
        {
            return renderer?.LoadTexture(name);
        }
        protected SDLTexture? LoadTexture(string name, byte[]? data)
        {
            return renderer?.LoadTexture(name, data);
        }
        internal bool Initialized { get; set; }
        internal bool Shown { get; set; }
        protected virtual void OnWindowShown(EventArgs e) { }
        protected virtual void OnWindowHidden(EventArgs e) { }
        protected virtual void OnWindowClose(EventArgs e) { }

        protected virtual void OnWindowUpdate(SDLWindowUpdateEventArgs e) { }
        protected virtual void OnWindowPaint(SDLWindowPaintEventArgs e) { }
        protected virtual void OnWindowLoad(SDLWindowLoadEventArgs e) { }
        protected virtual void OnWindowSizeChanged(SDLWindowSizeEventArgs e) { }

        protected virtual void OnMouseButtonDown(SDLMouseEventArgs e) { }
        protected virtual void OnMouseButtonUp(SDLMouseEventArgs e) { }
        protected virtual void OnMouseMove(SDLMouseEventArgs e) { }
        protected virtual void OnMouseWheel(SDLMouseWheelEventArgs e) { }

        protected virtual void OnKeyDown(SDLKeyEventArgs e) { }
        protected virtual void OnKeyUp(SDLKeyEventArgs e) { }
        protected virtual void OnTextInput(SDLTextInputEventArgs e) { }

        internal void InternalOnUpdate(SDLWindowUpdateEventArgs e) { OnWindowUpdate(e); }
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

        internal void InternalOnShown(EventArgs e) { OnWindowShown(e); }
        internal void InternalOnHidden(EventArgs e) { OnWindowHidden(e); }
        internal void InternalOnClose(EventArgs e) { OnWindowClose(e); }
        internal void InternalOnSizeChanged(SDLWindowSizeEventArgs e) { OnWindowSizeChanged(e); }

        internal void InternalOnMouseButtonDown(SDLMouseEventArgs e) { OnMouseButtonDown(e); }
        internal virtual void InternalOnMouseButtonUp(SDLMouseEventArgs e) { OnMouseButtonUp(e); }
        internal virtual void InternalOnMouseMove(SDLMouseEventArgs e) { OnMouseMove(e); }
        internal virtual void InternalOnMouseWheel(SDLMouseWheelEventArgs e) { OnMouseWheel(e); }
        internal virtual void InternalOnKeyDown(SDLKeyEventArgs e) { OnKeyDown(e); }
        internal virtual void InternalOnKeyUp(SDLKeyEventArgs e) { OnKeyUp(e); }
        internal virtual void InternalOnTextInput(SDLTextInputEventArgs e) { OnTextInput(e); }

    }
}
