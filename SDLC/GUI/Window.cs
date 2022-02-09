namespace SDLC.GUI
{
    using SDLC;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Window : GUIObject
    {
        private readonly Screen screen;
        private readonly List<Gadget> gadgets = new();
        private WindowFlags windowFlags;
        private int zoomX = -1;
        private int zoomY = -1;
        private int zoomW = -1;
        private int zoomH = -1;
        private int unzoomX = -1;
        private int unzoomY = -1;
        private int unzoomW = -1;
        private int unzoomH = -1;
        private SDLTexture? bitmap;
        private bool valid;
        private bool superbitmap;
        private byte alpha;
        internal Window(IGUISystem gui, Screen screen)
            : base(gui)
        {
            this.screen = screen;
            superbitmap = true;
            windowFlags = WindowFlags.SizeGadget | WindowFlags.DragBar | WindowFlags.DepthGadget | WindowFlags.HasZoom | WindowFlags.CloseGadget | WindowFlags.SizeBBottom | WindowFlags.Activate;
            SetBorders(4, 28, 4, 4);
            screen.AddWindow(this);
        }
        public string? Title { get => Text; set => Text = value; }
        public Screen Screen => screen;
        public byte Alpha
        {
            get => alpha;
        }
        public void SetAlpha(int v)
        {
            if (v > 255) { v = 255; }
            if (v < 0) { v = 0; }
            alpha = (byte)(v & 0xFF);
        }
        public void IncreaseAlpha(int v)
        {
            int a = alpha + v;
            if (a > 255) { alpha = 255; }
            else { alpha = (byte)(a & 0xFF); }
        }

        public void DecreaseAlpha(int v)
        {
            int a = alpha - v;
            if (a < 0) { alpha = 0; }
            else { alpha = (byte)(a & 0xFF); }
        }

        public int WindowId { get; internal set; }

        public event EventHandler<EventArgs>? WindowClose;
        public bool Superbitmap
        {
            get => superbitmap;
            set
            {
                if (superbitmap != value)
                {
                    superbitmap = value;
                    if (superbitmap)
                    {
                        valid = false;
                    }
                    else
                    {
                        bitmap?.Dispose();
                        bitmap = null;
                    }
                }
            }
        }
        public WindowFlags WindowFlags
        {
            get => windowFlags;
            set
            {
                if (windowFlags != value)
                {
                    windowFlags = value;
                    Invalidate();
                }
            }
        }
        public bool Borderless
        {
            get => (windowFlags & WindowFlags.Borderless) == WindowFlags.Borderless;
            set
            {
                if (value)
                {
                    WindowFlags |= WindowFlags.Borderless;
                }
                else
                {
                    WindowFlags &= ~WindowFlags.Borderless;
                }
            }
        }
        public bool Active
        {
            get => (windowFlags & WindowFlags.WindowActive) == WindowFlags.WindowActive;
            set
            {
                if (value)
                {
                    WindowFlags |= WindowFlags.WindowActive;
                }
                else
                {
                    WindowFlags &= ~WindowFlags.WindowActive;
                }
            }
        }
        public bool MouseHover
        {
            get => (windowFlags & WindowFlags.MouseHover) == WindowFlags.MouseHover;
            set
            {
                if (value)
                {
                    WindowFlags |= WindowFlags.MouseHover;
                }
                else
                {
                    WindowFlags &= ~WindowFlags.MouseHover;
                }
            }
        }
        public bool Zoomed
        {
            get => (windowFlags & WindowFlags.Zoomed) == WindowFlags.Zoomed;
        }

        public IEnumerable<Gadget> Gadgets => gadgets;

        internal void AddGadget(Gadget gadget)
        {
            gadgets.Add(gadget);
        }

        internal bool IsFrontWindow
        {
            get
            {
                return screen.IsFrontWindow(this);
            }
        }

        internal void Close()
        {
            bitmap?.Dispose();
            bitmap = null;
            valid = false;
        }

        internal void ToFront()
        {
            screen.WindowToFront(this);
        }

        internal void ToBack()
        {
            screen.WindowToBack(this);
        }

        internal void Zip()
        {
            if (Zoomed)
            {
                windowFlags &= ~WindowFlags.Zoomed;
                RememberZoom();
                SanitizeUnZoomValues();
                SetDimensions(unzoomX, unzoomY, unzoomW, unzoomH);
                InvalidateBounds();
            }
            else
            {
                windowFlags |= WindowFlags.Zoomed;
                RememberUnZoom();
                SanitizeZoomValues();
                SetDimensions(zoomX, zoomY, zoomW, zoomH);
                InvalidateBounds();
            }
        }

        internal void RaiseWindowClose()
        {
            EventHelper.Raise(this, WindowClose, EventArgs.Empty);
        }

        private void SanitizeZoomValues()
        {
            if (zoomW < 0 && zoomH < 0)
            {
                int sw = screen.Width;
                int sh = screen.Height;
                Rectangle r;
                if (Width < sw / 2 && Height < sh / 2)
                {
                    r = CheckDimensions(0, 0, sw, sh);
                }
                else if (Width < sw / 2)
                {
                    r = CheckDimensions(0, 10, sw, Height);
                }
                else if (Height < sh / 2)
                {
                    r = CheckDimensions(10, 0, Width, sh);
                }
                else
                {
                    r = CheckDimensions(10, 10, sw / 2, sh / 2);
                }
                zoomX = r.X;
                zoomY = r.Y;
                zoomW = r.Width;
                zoomH = r.Height;
            }
        }

        private void SanitizeUnZoomValues()
        {
            if (unzoomW < 0 && unzoomH < 0)
            {
                int sw = screen.Width;
                int sh = screen.Height;
                Rectangle r;
                if (Width > sw / 2 && Height > sh / 2)
                {
                    r = CheckDimensions(0, 0, sw / 3, sh / 3);
                }
                else if (Width > sw / 2)
                {
                    r = CheckDimensions(0, 10, sw / 3, Height);
                }
                else if (Height > sh / 2)
                {
                    r = CheckDimensions(10, 0, Width, sh / 3);
                }
                else
                {
                    r = CheckDimensions(10, 10, sw / 3, sh / 3);
                }
                unzoomX = r.X;
                unzoomY = r.Y;
                unzoomW = r.Width;
                unzoomH = r.Height;
            }
        }
        private void RememberUnZoom()
        {
            unzoomX = LeftEdge;
            unzoomY = TopEdge;
            unzoomW = Width;
            unzoomH = Height;
        }
        private void RememberZoom()
        {
            zoomX = LeftEdge;
            zoomY = TopEdge;
            zoomW = Width;
            zoomH = Height;
        }

        private Rectangle CheckDimensions(int x, int y, int w, int h)
        {
            int sw = screen.Width;
            int sh = screen.Height;
            if (MaxWidth > 0 && w > MaxWidth) { w = MaxWidth; }
            if (MaxHeight > 0 && h > MaxHeight) { h = MaxHeight; }
            if (w < MinWidth) { w = MinWidth; }
            if (h < MinHeight) { h = MinHeight; }
            if (x < 0) { x = 0; }
            if (y < 0) { y = 0; }
            if (x + w > sw) { x -= (x + w) - sw; }
            if (y + h > sh) { y -= (y + h) - sh; }
            return new Rectangle(x, y, w, h);
        }

        internal void MoveWindow(int dX, int dY)
        {
            Rectangle newDim = CheckDimensions(LeftEdge + dX, TopEdge + dY, Width, Height);
            SetDimensions(newDim);
            windowFlags |= WindowFlags.MouseHover;
            InvalidateBounds();
        }

        internal void SizeWindow(int dX, int dY)
        {
            Rectangle newDim = CheckDimensions(LeftEdge, TopEdge, Width + dX, Height + dY);
            SetDimensions(newDim);
            windowFlags |= WindowFlags.MouseHover;
            InvalidateBounds();
        }

        public Gadget? FindGadget(int x, int y)
        {
            for (int i = gadgets.Count - 1; i >= 0; i--)
            {
                Gadget gad = gadgets[i];
                if (gad.Contains(x, y))
                {
                    return gad;
                }
            }
            return null;
        }


        public override Rectangle GetBounds()
        {
            return new Rectangle(screen.LeftEdge + LeftEdge, screen.TopEdge + TopEdge, Width, Height);
        }

        internal void InvalidateFromGadget()
        {
            Invalidate();
        }

        protected override void Invalidate()
        {
            valid = false;
        }

        private void InvalidateBounds()
        {
            Invalidate();
            foreach (Gadget gadget in gadgets)
            {
                gadget.InvalidateBounds();
            }
        }

        private void InitBitmap(IRenderer gfx)
        {
            bitmap?.Dispose();
            bitmap = gfx.CreateTexture(GetWindowName(), Width, Height);
            if (bitmap != null)
            {
                bitmap.BlendMode = BlendMode.Blend;
            }
        }

        private void CheckBitmap(IRenderer gfx)
        {
            if (bitmap == null || bitmap.Width < Width || bitmap.Height < Height)
            {
                InitBitmap(gfx);
            }
        }
        public void Render(IRenderer gfx, IGUIRenderer ren)
        {
            if (Superbitmap)
            {
                if (!valid)
                {
                    CheckBitmap(gfx);
                    gfx.PushTarget(bitmap);
                    gfx.ClearScreen(Color.FromArgb(0, 0, 0, 0));
                    RenderWindow(gfx, ren);
                    gfx.PopTarget();
                    valid = true;
                }
                if (valid)
                {
                    if (bitmap != null)
                    {
                        Rectangle dst = GetBounds();
                        Rectangle src = new Rectangle(0, 0, dst.Width, dst.Height);
                        SDLGfx.DrawTexture(gfx, bitmap, src, dst, alpha);
                    }
                }
            }
            else
            {
                RenderWindow(gfx, ren);
            }
        }

        private void RenderWindow(IRenderer gfx, IGUIRenderer ren)
        {
            ren.RenderWindow(gfx, this);
            foreach (var gad in gadgets)
            {
                gad.Render(gfx, ren);
            }
        }

        private string GetWindowName()
        {
            StringBuilder sb = new();
            sb.Append("_GUI_Window_");
            sb.Append(WindowId);
            sb.Append("_");
            return sb.ToString();
        }
        public override string ToString()
        {
            return GetWindowName();
        }

    }
}
