// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.GUI;

using SDLC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

public class Window : GUIObject
{
    private readonly Screen screen;
    private readonly List<Gadget> gadgets = new();
    private readonly List<Requester> requests = new();
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
    private byte alpha;

    public const WindowFlags DefaultFlags = WindowFlags.SizeGadget | WindowFlags.DragBar | WindowFlags.DepthGadget |
        WindowFlags.HasZoom | WindowFlags.CloseGadget | WindowFlags.Activate | WindowFlags.SizeBBottom |
        WindowFlags.SuperBitmap;
    internal Window(IGUISystem gui, Screen screen, WindowFlags flags, string? title)
        : base(gui)
    {
        this.screen = screen;
        windowFlags = flags;
        Title = title;
        if (Borderless)
        {
            SetBorders(0, 0, 0, 0);
        }
        else
        {
            SetBorders(4, HasTitleBar ? 28 : 4, 4, 4);
        }
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
    public WindowFlags WindowFlags
    {
        get => windowFlags;
        set
        {
            if (windowFlags != value)
            {
                windowFlags = value;
                OnInvalidate();
            }
        }
    }
    public bool Superbitmap
    {
        get => (windowFlags & WindowFlags.SuperBitmap) == WindowFlags.SuperBitmap;
        set
        {
            if (value)
            {
                WindowFlags |= WindowFlags.SuperBitmap;
            }
            else
            {
                WindowFlags &= ~WindowFlags.SuperBitmap;
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

    public bool BackDrop
    {
        get => (windowFlags & WindowFlags.BackDrop) == WindowFlags.BackDrop;
        set
        {
            if (value)
            {
                WindowFlags |= WindowFlags.BackDrop;
            }
            else
            {
                WindowFlags &= ~WindowFlags.BackDrop;
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
    public bool InRequest
    {
        get => (windowFlags & WindowFlags.InRequest) == WindowFlags.InRequest;
    }

    public bool HasTitleBar
    {
        get => !string.IsNullOrEmpty(Title) || (windowFlags & WindowFlags.DragBar) == WindowFlags.DragBar;
    }

    public IEnumerable<Gadget> Gadgets => gadgets;

    protected override SDLFont? GetFont()
    {
        return font ?? screen.Font;
    }
    internal void AddGadget(Gadget gadget)
    {
        gadgets.Add(gadget);
    }

    internal bool Request(Requester req)
    {
        if (req.Window == this)
        {
            requests.Add(req);
            windowFlags |= WindowFlags.InRequest;
            OnInvalidate();
            return true;
        }
        return false;
    }

    internal void EndRequest(Requester req)
    {
        if (req.Window == this && requests.Remove(req))
        {
            if (requests.Count == 0)
            {
                windowFlags &= ~WindowFlags.InRequest;
            }
            OnInvalidate();
        }
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

    internal void ToggleDepth()
    {
        if (IsFrontWindow)
        {
            ToBack();
        }
        else
        {
            ToFront();
        }
    }

    internal void ChangeWindowBox(int left, int top, int width, int height)
    {
        Rectangle newDim = CheckDimensions(left, top, width, height);
        SetDimensions(newDim);
        InvalidateBounds();
    }

    internal Gadget? FindNextGadget(Gadget gadget)
    {
        int index = gadgets.IndexOf(gadget);
        while (true)
        {
            index++;
            if (index >= gadgets.Count) { index = 0; }
            Gadget gad = gadgets[index];
            if (gad == gadget) return null;
            if (gad.TabCycle) return gad;
        }
    }
    internal Gadget? FindPrevGadget(Gadget gadget)
    {
        int index = gadgets.IndexOf(gadget);
        while (true)
        {
            index--;
            if (index < 0) { index = gadgets.Count - 1; }
            Gadget gad = gadgets[index];
            if (gad == gadget) return null;
            if (gad.TabCycle) return gad;
        }
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

    internal void MoveWindow(int dX, int dY, bool dragging = false)
    {
        Rectangle newDim = CheckDimensions(LeftEdge + dX, TopEdge + dY, Width, Height);
        SetDimensions(newDim);
        if (dragging) { windowFlags |= WindowFlags.MouseHover; }
        InvalidateBounds();
    }

    internal void SizeWindow(int dX, int dY, bool sizing = false)
    {
        Rectangle newDim = CheckDimensions(LeftEdge, TopEdge, Width + dX, Height + dY);
        SetDimensions(newDim);
        if (sizing) { windowFlags |= WindowFlags.MouseHover; }
        InvalidateBounds();
    }

    public Gadget? FindGadget(int x, int y)
    {
        if (InRequest && requests.Count > 0)
        {
            Requester req = requests[^1];
            if (req.Contains(x, y))
            {
                return req.FindGadget(x, y);
            }
            return InReqFindGadget(x, y);
        }
        else
        {
            return NormalFindGadget(x, y);
        }
    }
    private Gadget? InReqFindGadget(int x, int y)
    {
        for (int i = gadgets.Count - 1; i >= 0; i--)
        {
            Gadget gad = gadgets[i];
            if (gad.Enabled && gad.IsBorderGadget && gad.IsSysGadget && gad.Contains(x, y))
            {
                return gad;
            }
        }
        return null;
    }
    private Gadget? NormalFindGadget(int x, int y)
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


    public override Rectangle GetBounds()
    {
        return new Rectangle(screen.LeftEdge + LeftEdge, screen.TopEdge + TopEdge, Width, Height);
    }

    internal void InvalidateFromGadget()
    {
        OnInvalidate();
    }

    protected override void OnInvalidate()
    {
        valid = false;
    }

    protected internal override void InvalidateBounds()
    {
        OnInvalidate();
        foreach (Gadget gadget in gadgets)
        {
            gadget.InvalidateBounds();
        }
        foreach (Requester req in requests)
        {
            req.InvalidateBounds();
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
                RenderWindow(gfx, ren, -LeftEdge, -TopEdge);
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
            RenderWindow(gfx, ren, 0, 0);
        }
    }

    private void RenderWindow(IRenderer gfx, IGUIRenderer ren, int offsetX, int offsetY)
    {
        ren.RenderWindow(gfx, this, offsetX, offsetY);
        foreach (var gad in gadgets.Where(g => g.IsBorderGadget))
        {
            gad.Render(gfx, ren);
        }
        Rectangle inner = GetInnerBounds();
        inner.Offset(offsetX, offsetY);
        gfx.PushClip(inner);
        foreach (var gad in gadgets.Where(g => !g.IsBorderGadget))
        {
            gad.Render(gfx, ren);
        }
        foreach (Requester req in requests)
        {
            req.Render(gfx, ren);
        }
        gfx.PopClip();
    }

    private string GetWindowName()
    {
        StringBuilder sb = new();
        sb.Append("_GUI_Window_");
        sb.Append(WindowId);
        sb.Append('_');
        return sb.ToString();
    }
    public override string ToString()
    {
        return GetWindowName();
    }

}
