namespace SDLC.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class GUIObject : IBox
    {
        protected readonly IGUISystem gui;
        private int leftEdge;
        private int topEdge;
        private int width;
        private int height;
        private int borderTop;
        private int borderLeft;
        private int borderRight;
        private int borderBottom;
        private int minWidth;
        private int minHeight;
        private string? text;

        protected GUIObject(IGUISystem gui)
        {
            this.gui = gui;
        }
        public string? Text
        {
            get => text;
            set
            {
                if (!string.Equals(text, value))
                {
                    SetText(value);
                    //Invalidate();
                }
            }
        }

        public virtual void SetText(string? text)
        {
            this.text = text;
        }

        public int LeftEdge
        {
            get => leftEdge;
            set
            {
                if (leftEdge != value)
                {
                    SetDimensions(value, topEdge, width, height);
                }
            }
        }

        public int TopEdge
        {
            get => topEdge;
            set
            {
                if (topEdge != value)
                {
                    SetDimensions(leftEdge, value, width, height);
                }
            }
        }

        public int Width
        {
            get => width;
            set
            {
                if (width != value)
                {
                    SetDimensions(leftEdge, topEdge, value, height);
                }
            }
        }

        public int Height
        {
            get => height;
            set
            {
                if (height != value)
                {
                    SetDimensions(leftEdge, topEdge, width, value);
                }
            }
        }

        public int BorderLeft
        {
            get => borderLeft;
            set
            {
                if (borderLeft != value)
                {
                    SetBorders(value, borderTop, borderRight, borderBottom);
                }
            }
        }
        public int BorderTop
        {
            get => borderTop;
            set
            {
                if (borderTop != value)
                {
                    SetBorders(borderLeft, value, borderRight, borderBottom);
                }
            }
        }
        public int BorderRight
        {
            get => borderRight;
            set
            {
                if (borderRight != value)
                {
                    SetBorders(borderLeft, borderTop, value, borderBottom);
                }
            }
        }
        public int BorderBottom
        {
            get => borderBottom;
            set
            {
                if (borderBottom != value)
                {
                    SetBorders(borderLeft, borderTop, borderRight, value);
                }
            }
        }

        public int MinWidth
        {
            get => minWidth;
            set
            {
                if (minWidth != value)
                {
                    SetMinSize(value, minHeight);
                }
            }
        }

        public int MinHeight
        {
            get => minHeight;
            set
            {
                if (minHeight != value)
                {
                    SetMinSize(minWidth, value);
                }
            }
        }

        public virtual Rectangle GetBounds()
        {
            return new Rectangle(leftEdge, topEdge, width, height);
        }

        public Rectangle GetInnerBounds()
        {
            Rectangle rect = GetBounds();
            rect.X += borderLeft;
            rect.Y += borderTop;
            rect.Width -= (borderLeft + borderRight);
            rect.Height -= (borderTop + borderBottom);
            return rect;
        }

        public virtual void SetDimensions(int x, int y, int w, int h)
        {
            leftEdge = x;
            topEdge = y;
            width = w;
            height = h;
        }

        public virtual void SetBorders(int left, int top, int right, int bottom)
        {
            borderLeft = left;
            borderTop = top;
            borderRight = right;
            borderBottom = bottom;
        }

        public virtual void SetMinSize(int minWidth, int minHeight)
        {
            this.minWidth = minWidth;
            this.minHeight = minHeight;
        }

        public virtual bool Contains(int x, int y)
        {
            return GetBounds().Contains(x, y);
            //return x >= leftEdge && y >= topEdge && x < leftEdge + width && y < topEdge + height;
        }

    }
}
