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
        private int renderPrio;
        private int inputPrio;
        protected bool noInput;
        protected bool noRender;

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
            set => renderPrio = value;
        }

        public int InputPrio
        {
            get => inputPrio;
            set => inputPrio = value;
        }

        public bool NoInput => noInput;
        public bool NoRender => noRender;

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
        internal protected virtual void OnWindowSizeChanged(SDLWindowSizeEventArgs e) { }
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
            if (!Initialized)
            {
                OnWindowLoad(e);
            }
        }

    }
}
