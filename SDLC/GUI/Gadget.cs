﻿// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.GUI;

using SDLC;
using System;
using System.Drawing;

public class Gadget : GUIObject
{
    private readonly Window window;
    private readonly Requester? requester;
    private GadgetFlags flags;
    private GadgetActivation activation;
    private GadgetType gadgetType;
    private Rectangle? bounds;
    private bool transparentBackground;
    private Color bgColor = Color.Empty;
    private Icons icon;
    private PropInfo? propInfo;
    private StringInfo? strInfo;
    private GadToolsInfo? gadInfo;
    private VerticalAlignment verticalTextAlignment;
    private HorizontalAlignment horizontalTextAlignment;
    private int textOffsetX;
    private int textOffsetY;
    private string? tooltipText;
    internal Gadget(IGUISystem gui, Window window, Requester? req = null)
        : base(gui)
    {
        this.window = window;
        requester = req;
        SetBorders(1, 1, 1, 1);
        verticalTextAlignment = VerticalAlignment.Center;
        horizontalTextAlignment = HorizontalAlignment.Center;
        activation = GadgetActivation.RelVerify | GadgetActivation.Immediate;
        if (req != null)
        {
            gadgetType |= GadgetType.ReqGadget;
            req.AddGadget(this);
        }
        else
        {
            window.AddGadget(this);
        }
    }

    public event EventHandler<EventArgs>? GadgetDown;

    public event EventHandler<EventArgs>? GadgetUp;

    public Window Window => window;
    public int GadgetId { get; set; }
    public Icons Icon
    {
        get => icon;
        set
        {
            if (icon != value)
            {
                icon = value;
                OnInvalidate();
            }
        }
    }

    public Color BackgroundColor
    {
        get => bgColor;
        set
        {
            if (bgColor != value)
            {
                bgColor = value;
                OnInvalidate();
            }
        }
    }

    public VerticalAlignment VerticalTextAlignment
    {
        get => verticalTextAlignment;
        set
        {
            if (verticalTextAlignment != value)
            {
                verticalTextAlignment = value;
                OnInvalidate();
            }
        }
    }

    public HorizontalAlignment HorizontalTextAlignment
    {
        get => horizontalTextAlignment;
        set
        {
            if (horizontalTextAlignment != value)
            {
                horizontalTextAlignment = value;
                OnInvalidate();
            }
        }
    }

    public int TextOffsetX
    {
        get => textOffsetX;
        set
        {
            if (textOffsetX != value)
            {
                textOffsetX = value;
                OnInvalidate();
            }
        }
    }
    public int TextOffsetY
    {
        get => textOffsetY;
        set
        {
            if (textOffsetY != value)
            {
                textOffsetY = value;
                OnInvalidate();
            }
        }
    }

    public string? TooltipText
    {
        get => tooltipText;
        set
        {
            if (!string.Equals(tooltipText, value))
            {
                tooltipText = value;
                //window.InvalidateFromGadget();
            }
        }
    }

    internal Requester? Requester => requester;
    internal PropInfo? PropInfo => propInfo;
    internal StringInfo? StrInfo => strInfo;
    internal GadToolsInfo? GadInfo => gadInfo;

    public bool Selected
    {
        get => (flags & GadgetFlags.Selected) == GadgetFlags.Selected;
        set
        {
            if (value)
            {
                Flags |= GadgetFlags.Selected;
            }
            else
            {
                Flags &= ~GadgetFlags.Selected;
            }
        }
    }

    public bool NoHighlight
    {
        get => (flags & GadgetFlags.HNone) == GadgetFlags.HNone;
        set
        {
            if (value)
            {
                Flags |= GadgetFlags.HNone;
            }
            else
            {
                Flags &= ~GadgetFlags.HNone;
            }
        }
    }
    public bool Active
    {
        get => (activation & GadgetActivation.ActiveGadget) == GadgetActivation.ActiveGadget;
        set
        {

            if (value)
            {
                Activation |= GadgetActivation.ActiveGadget;
            }
            else
            {
                Activation &= ~GadgetActivation.ActiveGadget;
            }
        }
    }
    public bool ToggleSelect
    {
        get => (activation & GadgetActivation.ToggleSelect) == GadgetActivation.ToggleSelect;
        set
        {

            if (value)
            {
                Activation |= GadgetActivation.ToggleSelect;
            }
            else
            {
                Activation &= ~GadgetActivation.ToggleSelect;
            }
        }
    }
    public bool TabCycle
    {
        get => (flags & GadgetFlags.TabCycle) != 0;
        set
        {
            if (value)
            {
                Flags |= GadgetFlags.TabCycle;
            }
            else
            {
                Flags &= ~GadgetFlags.TabCycle;
            }
        }
    }

    public bool Enabled
    {
        get => (flags & GadgetFlags.Disabled) == 0;
        set
        {
            if (value)
            {
                Flags &= ~GadgetFlags.Disabled;
            }
            else
            {
                Flags |= GadgetFlags.Disabled;
            }
        }
    }

    public bool MouseHover
    {
        get => (activation & GadgetActivation.MouseHover) == GadgetActivation.MouseHover;
        set
        {
            if (value)
            {
                Activation |= GadgetActivation.MouseHover;
            }
            else
            {
                Activation &= ~GadgetActivation.MouseHover;
            }
        }
    }

    public bool IsIntegerGadget
    {
        get => (activation & GadgetActivation.LongInt) == GadgetActivation.LongInt;
        set
        {
            if (value)
            {
                Activation |= GadgetActivation.LongInt;
            }
            else
            {
                Activation &= ~GadgetActivation.LongInt;
            }
        }
    }

    public GadgetFlags Flags
    {
        get => flags;
        set
        {
            if (flags != value)
            {
                flags = value;
                InvalidateBounds();
            }
        }
    }

    public GadgetActivation Activation
    {
        get => activation;
        set
        {
            if (activation != value)
            {
                activation = value;
                InvalidateBounds();
            }
        }
    }
    public GadgetType GadgetType
    {
        get => gadgetType;
        set
        {
            if (gadgetType != value)
            {
                gadgetType = value;
                if (GType == GadgetType.PropGadget)
                {
                    propInfo = new PropInfo(this);
                }
                else if (GType == GadgetType.StrGadget)
                {
                    flags |= GadgetFlags.TabCycle;
                    strInfo = new StringInfo(this);
                }
                if ((gadgetType & GadgetType.GadToolsGadget) == GadgetType.GadToolsGadget)
                {
                    gadInfo = new GadToolsInfo(this);
                }
            }
        }
    }

    public GadgetType GType => gadgetType & GadgetType.GTypeMask;
    public bool IsPropGadget
    {
        get => GType == GadgetType.PropGadget && propInfo != null;
    }

    public bool IsStrGadget
    {
        get => GType == GadgetType.StrGadget && strInfo != null;
    }

    public bool IsBoolGadget
    {
        get => GType == GadgetType.BoolGadget;
    }
    public bool IsSysGadget
    {
        get => (gadgetType & GadgetType.SysGadget) != 0;
    }
    public bool IsReqGadget
    {
        get => (gadgetType & GadgetType.ReqGadget) != 0;
    }

    public GadgetType SysGadgetType
    {
        get => gadgetType & GadgetType.SysTypeMask;
    }
    public bool RelVeriy
    {
        get => (activation & GadgetActivation.RelVerify) == GadgetActivation.RelVerify;
    }

    public bool Immediate
    {
        get => (activation & GadgetActivation.Immediate) == GadgetActivation.Immediate;
    }

    public bool IsEndGadget
    {
        get => (activation & GadgetActivation.EndGadget) == GadgetActivation.EndGadget;
    }

    public bool IsBorderGadget
    {
        get => (activation & (GadgetActivation.LeftBorder | GadgetActivation.RightBorder | GadgetActivation.TopBorder | GadgetActivation.BottomBorder)) != 0;
    }
    public bool TransparentBackground
    {
        get => transparentBackground;
        set => transparentBackground = value;
    }

    public Gadget? FindNextGadget()
    {
        return window.FindNextGadget(this);
    }

    public Gadget? FindPrevGadget()
    {
        return window.FindPrevGadget(this);
    }

    protected override SDLFont? GetFont()
    {
        return font ?? window.Font;
    }
    internal void RaiseGadgetDown()
    {
        EventHelper.Raise(this, GadgetDown, EventArgs.Empty);
    }
    internal void RaiseGadgetUp()
    {
        EventHelper.Raise(this, GadgetUp, EventArgs.Empty);
    }

    private ActionResult CheckNavKeys(SDLKeyEventArgs e)
    {
        if (e.ScanCode == ScanCode.SCANCODE_TAB && e.State == KeyButtonState.Pressed)
        {
            if ((e.KeyMod & KeyMod.SHIFT) != 0)
            {
                return ActionResult.NavigatePrevious;
            }
            else
            {
                return ActionResult.NavigateNext;
            }
        }
        else if (e.ScanCode == ScanCode.SCANCODE_RETURN && e.State == KeyButtonState.Pressed)
        {
            RaiseGadgetUp();
            return ActionResult.GadgetUp;
        }
        return ActionResult.None;
    }
    internal void HandleDeselection()
    {
        if (!ToggleSelect)
        {
            Selected = false;
        }
        else
        {
            Selected = !Selected;
        }
        if (propInfo?.HandleDeselection() ?? false)
        {
            window.InvalidateFromGadget();
        }
    }

    internal void HandleSelection()
    {
        if (!ToggleSelect)
        {
            Selected = true;
        }
        strInfo?.HandleSelected();
    }

    internal bool HandleMouseDown(int x, int y, bool isTimerRepeat = false)
    {
        bool result = false;
        Rectangle bounds = GetBounds();
        result |= propInfo?.HandleMouseDown(bounds, x, y, isTimerRepeat) ?? false;
        result |= strInfo?.HandleMouseDown(bounds, x, y, isTimerRepeat) ?? false;
        if (Immediate) { RaiseGadgetDown(); result |= true; }
        if (result) { window.InvalidateFromGadget(); }
        return result;
    }
    internal bool HandleMouseUp(int x, int y)
    {
        bool result = false;
        Rectangle bounds = GetBounds();
        result |= propInfo?.HandleMouseUp(bounds, x, y) ?? false;
        result |= strInfo?.HandleMouseUp(bounds, x, y) ?? false;
        if (RelVeriy) { RaiseGadgetUp(); result |= true; }
        if (result) { window.InvalidateFromGadget(); }
        return result;
    }

    internal bool HandleMouseMove(int x, int y)
    {
        bool result = false;
        Rectangle bounds = GetBounds();
        result |= propInfo?.HandleMouseMove(bounds, x, y) ?? false;
        result |= strInfo?.HandleMouseMove(bounds, x, y) ?? false;
        if (result) { window.InvalidateFromGadget(); }
        return result;
    }
    internal ActionResult HandleKeyDown(SDLKeyEventArgs e)
    {
        ActionResult result = CheckNavKeys(e);
        if (result == ActionResult.None)
        {
            result |= strInfo?.HandleKeyDown(e) ?? ActionResult.None;
            if (result != ActionResult.None) { window.InvalidateFromGadget(); }
        }
        return result;
    }
    internal ActionResult HandleKeyUp(SDLKeyEventArgs e)
    {
        ActionResult result = CheckNavKeys(e);
        if (result == ActionResult.None)
        {
            result |= strInfo?.HandleKeyUp(e) ?? ActionResult.None;
            if (result != ActionResult.None) { window.InvalidateFromGadget(); }
        }
        return result;
    }
    internal bool HandleTextInput(SDLTextInputEventArgs e)
    {
        bool result = false;
        result |= strInfo?.HandleTextInput(e) ?? false;
        if (result) { window.InvalidateFromGadget(); }
        return result;
    }
    internal void ModifyProp(PropFlags flags, int horizPot, int vertPot, int horizBody, int vertBody)
    {
        if (propInfo != null)
        {
            propInfo.Flags = flags;
            propInfo.HorizPot = horizPot;
            propInfo.VertPot = vertPot;
            propInfo.HorizBody = horizBody;
            propInfo.VertBody = vertBody;
            window.InvalidateFromGadget();
        }
    }

    protected override void OnInvalidate()
    {
        propInfo?.Invalidate();
        strInfo?.Invalidate();
        gadInfo?.Invalidate();
        window.InvalidateFromGadget();
    }


    protected internal override void InvalidateBounds()
    {
        bounds = null;
        OnInvalidate();
    }

    public override Rectangle GetBounds()
    {
        if (bounds == null)
        {
            bounds = CalculateBounds();
        }
        return bounds.Value;
    }

    private int GetWindowX()
    {
        int x = requester?.LeftEdge ?? window.LeftEdge;
        if ((activation & GadgetActivation.LeftBorder) == 0)
        {
            x += requester?.BorderLeft ?? window.BorderLeft;
        }
        return x;
    }

    private int GetWindowY()
    {
        int y = requester?.TopEdge ?? window.TopEdge;
        if ((activation & GadgetActivation.TopBorder) == 0)
        {
            y += requester?.BorderTop ?? window.BorderTop;
        }
        return y;
    }

    private int GetWindowW()
    {
        int w = requester?.Width ?? window.Width;
        if ((activation & GadgetActivation.LeftBorder) == 0)
        {
            w -= requester?.BorderLeft ?? window.BorderLeft;
        }
        if ((activation & GadgetActivation.RightBorder) == 0)
        {
            w -= requester?.BorderRight ?? window.BorderRight;
        }
        return w;
    }
    private int GetWindowH()
    {
        int h = requester?.Height ?? window.Height;
        if ((activation & GadgetActivation.TopBorder) == 0)
        {
            h -= requester?.BorderTop ?? window.BorderTop;
        }
        if ((activation & GadgetActivation.BottomBorder) == 0)
        {
            h -= requester?.BorderBottom ?? window.BorderBottom;
        }
        return h;
    }

    private Rectangle GetParentBounds()
    {
        Rectangle bounds = window.GetBounds();
        if ((activation & GadgetActivation.LeftBorder) == 0)
        {
            bounds.X += window.BorderLeft;
            bounds.Width -= window.BorderLeft;
        }
        if ((activation & GadgetActivation.TopBorder) == 0)
        {
            bounds.Y += window.BorderTop;
            bounds.Height -= window.BorderTop;
        }
        if ((activation & GadgetActivation.RightBorder) == 0)
        {
            bounds.Width -= window.BorderRight;
        }
        if ((activation & GadgetActivation.BottomBorder) == 0)
        {
            bounds.Height -= window.BorderBottom;
        }
        if (requester != null)
        {
            bounds.X += requester.LeftEdge;
            bounds.Y += requester.TopEdge;
            bounds.Width = requester.Width;
            bounds.Height = requester.Height;
        }
        return bounds;
    }

    private Rectangle CalculateBounds()
    {
        Rectangle pBounds = GetParentBounds();
        int x = pBounds.X;
        int y = pBounds.Y;
        int w = pBounds.Width;
        int h = pBounds.Height;
        int bx = AddRel(GadgetFlags.RelRight, w) + LeftEdge;
        int by = AddRel(GadgetFlags.RelBottom, h) + TopEdge;
        int bw = AddRel(GadgetFlags.RelWidth, w) + Width;
        int bh = AddRel(GadgetFlags.RelHeight, h) + Height;
        bx += x;
        by += y;
        return new Rectangle(bx, by, bw, bh);
    }

    private int AddRel(GadgetFlags flag, int value)
    {
        return (flags & flag) == flag ? value : 0;
    }

    public void Render(IRenderer gfx, IGUIRenderer ren)
    {
        if (window.Superbitmap)
        {
            ren.RenderGadget(gfx, this, -window.LeftEdge, -window.TopEdge);
        }
        else
        {
            ren.RenderGadget(gfx, this, 0, 0);
        }
    }

    public override string ToString()
    {
        if (IsSysGadget) { return $"SysGadget '{SysGadgetType}'"; }
        if (Text != null) { return $"Gadget '{Text}'"; }
        if (Icon != Icons.NONE) { return $"Gadget '{Icon.ToString().Replace("ENTYPO_ICON_", "")}'"; }
        return $"Gadget '{GadgetId}'";
    }
}
