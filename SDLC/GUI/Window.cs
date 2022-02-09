namespace SDLC.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class Window : GUIObject, IGUIWindow
    {
        private readonly Screen screen;
        private readonly List<Gadget> gadgets = new();
        private WindowFlags windowFlags;

        public Window(IGUISystem gui, Screen screen)
            : base(gui)
        {
            this.screen = screen;
            windowFlags = WindowFlags.SizeGadget | WindowFlags.DragBar | WindowFlags.DepthGadget | WindowFlags.HasZoom | WindowFlags.CloseGadget | WindowFlags.SizeBBottom | WindowFlags.Activate;
            SetBorders(4, 28, 4, 4);
            screen.AddWindow(this);
        }
        public string? Title { get => Text; set => Text = value; }
        public IGUIScreen Screen => screen;
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

        public IEnumerable<IGUIGadget> Gadgets
        {
            get
            {
                foreach (Gadget gadget in gadgets)
                {
                    yield return gadget;
                }
            }
        }

        public void AddGadget(Gadget gadget)
        {
            gadgets.Add(gadget);
        }

        public void MoveWindow(int dX, int dY)
        {
            int newX = LeftEdge + dX;
            int newY = TopEdge + dY;
            if (newX < 0) { newX = 0; }
            if (newY < 0) { newY = 0; }
            SetDimensions(newX, newY, Width, Height);
            windowFlags |= WindowFlags.MouseHover;
            InvalidateBounds();
        }

        public void SizeWindow(int dX, int dY)
        {
            int newWidth = (Width + dX);
            int newHeight = (Height + dY);
            if (newWidth < MinWidth) { newWidth = MinWidth; }
            if (newHeight < MinHeight) { newHeight = MinHeight; }
            SetDimensions(LeftEdge, TopEdge, newWidth, newHeight);
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

        public void Invalidate()
        {

        }

        private void InvalidateBounds()
        {
            Invalidate();
            foreach (Gadget gadget in gadgets)
            {
                gadget.InvalidateBounds();
            }
        }

        public override string ToString()
        {
            return $"Window '{Title}'";
        }

    }
}
