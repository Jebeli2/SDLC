// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.GUI;
using System.Drawing;

public class GUIObject : IBox
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
    private int maxWidth;
    private int maxHeight;
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
                Invalidate();
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

    public int MaxWidth
    {
        get => maxWidth;
        set
        {
            if (maxWidth != value)
            {
                SetMaxSize(value, maxHeight);
            }
        }
    }

    public int MaxHeight
    {
        get => maxHeight;
        set
        {
            if (maxHeight != value)
            {
                maxHeight = value;
            }
        }
    }

    protected virtual void Invalidate()
    {

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

    protected void SetDimensions(Rectangle rect)
    {
        SetDimensions(rect.X, rect.Y, rect.Width, rect.Height);
    }

    protected virtual void SetDimensions(int x, int y, int w, int h)
    {
        leftEdge = x;
        topEdge = y;
        width = w;
        height = h;
    }

    protected virtual void SetBorders(int left, int top, int right, int bottom)
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

    public virtual void SetMaxSize(int maxWidth, int maxHeight)
    {
        this.maxWidth = maxWidth;
        this.maxHeight = maxHeight;
    }

    public bool Contains(int x, int y)
    {
        return GetBounds().Contains(x, y);
    }

}
