// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.GUI;
using System.Collections.Generic;
using System.Drawing;

public class Requester : GUIObject
{
    private readonly Window window;
    private readonly List<Gadget> gadgets = new();
    private ReqFlags flags;
    private int relLeft;
    private int relTop;
    internal Requester(IGUISystem gui, Window window)
        : base(gui)
    {
        this.window = window;
        SetBorders(4, 4, 4, 4);
    }

    public int RelLeft
    {
        get { return relLeft; }
        set
        {
            if (relLeft != value)
            {
                relLeft = value;
            }
        }
    }

    public int RelTop
    {
        get { return relTop; }
        set
        {
            if (relTop != value)
            {
                relTop = value;
            }
        }
    }
    public ReqFlags Flags
    {
        get => flags;
        set
        {
            if (flags != value)
            {
                flags = value;
            }
        }
    }

    public Window Window => window;

    public IEnumerable<Gadget> Gadgets => gadgets;

    internal void AddGadget(Gadget gadget)
    {
        gadgets.Add(gadget);
    }

    public Gadget? FindGadget(int x, int y)
    {
        for (int i = gadgets.Count - 1; i >= 0; i--)
        {
            Gadget gad = gadgets[i];
            if (gad.Enabled && gad.Contains(x, y))
            {
                return gad;
            }
        }
        return null;
    }
    internal void InvalidateBounds()
    {
        OnInvalidate();
        foreach (Gadget gadget in gadgets)
        {
            gadget.InvalidateBounds();
        }
    }

    public override Rectangle GetBounds()
    {
        if ((flags & ReqFlags.PointRel) == ReqFlags.PointRel)
        {
            Rectangle win = window.GetInnerBounds();
            Rectangle res = new Rectangle(win.X + win.Width / 2 - Width / 2, win.Y + win.Height / 2 - Height / 2, Width, Height);
            if (res.Right > win.Right)
            {
                res.X -= res.Right - win.Right;
                if (res.X < 0) { res.X = 0; }
            }
            if (res.Bottom > win.Bottom)
            {
                res.Y -= res.Bottom - win.Bottom;
                if (res.Y < 0) { res.Y = 0; }
            }

            LeftEdge = Math.Max(0, res.X - win.Left);
            TopEdge = Math.Max(0, res.Y - win.Top);
            return res;
        }
        else
        {
            return new Rectangle(window.LeftEdge + window.BorderLeft + LeftEdge, window.TopEdge + window.BorderTop + TopEdge, Width, Height);
        }
    }

    public void Render(IRenderer gfx, IGUIRenderer ren)
    {
        if (window.Superbitmap)
        {
            ren.RenderRequester(gfx, this, -window.LeftEdge, -window.TopEdge);
        }
        else
        {
            ren.RenderRequester(gfx, this, 0, 0);
        }
        foreach (Gadget gadget in gadgets)
        {
            gadget.Render(gfx, ren);
        }

    }

}
