namespace SDLC.GUI
{
    using SDLC;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Gadget : GUIObject
    {
        private readonly Window window;
        private GadgetFlags flags;
        private GadgetActivation activation;
        private GadgetType gadgetType;
        private Rectangle? bounds;
        private bool transparentBackground;
        private Icons icon;
        private PropInfo? propInfo;
        private StringInfo? strInfo;
        internal Gadget(IGUISystem gui, Window window)
            : base(gui)
        {
            this.window = window;
            SetBorders(1, 1, 1, 1);
            gadgetType = GadgetType.BoolGadget;
            activation = GadgetActivation.RelVerify | GadgetActivation.Immediate;
            window.AddGadget(this);
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
                    Invalidate();
                }
            }
        }

        internal PropInfo? PropInfo => propInfo;
        internal StringInfo? StrInfo => strInfo;

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
                        strInfo = new StringInfo(this);
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

        public bool IsBorderGadget
        {
            get => (activation & (GadgetActivation.LeftBorder | GadgetActivation.RightBorder | GadgetActivation.TopBorder | GadgetActivation.BottomBorder)) != 0;
        }
        public bool TransparentBackground
        {
            get => transparentBackground;
            set => transparentBackground = value;
        }
        internal void RaiseGadgetDown()
        {
            EventHelper.Raise(this, GadgetDown, EventArgs.Empty);
        }

        internal void RaiseGadgetUp()
        {
            EventHelper.Raise(this, GadgetUp, EventArgs.Empty);
        }

        internal void HandleDeselection()
        {
            if (propInfo?.HandleDeselection() ?? false)
            {
                window.InvalidateFromGadget();
            }
        }
        internal void HandlePropMouseDown(int x, int y)
        {
            if (propInfo?.HandlePropMouseDown(GetBounds(), x, y) ?? false)
            {
                window.InvalidateFromGadget();
            }
        }
        internal bool HandleStrMouseDown(int x, int y)
        {
            if (strInfo?.HandleMouseDown(GetBounds(), x, y) ?? false)
            {
                window.InvalidateFromGadget();
                return true;
            }
            return false;
        }
        internal bool HandleStrMouseUp(int x, int y)
        {
            if (strInfo?.HandleMouseUp(GetBounds(), x, y) ?? false)
            {
                window.InvalidateFromGadget();
                return true;
            }
            return false;
        }
        internal bool HandleStrMouseMove(int x, int y)
        {
            if (Active && Selected)
            {
                if (strInfo?.HandleMouseMove(GetBounds(), x, y) ?? false)
                {
                    window.InvalidateFromGadget();
                    return true;
                }
            }
            return false;
        }
        internal void HanldePropMouseMove(int x, int y)
        {
            if (propInfo?.HanldePropMouseMove(GetBounds(), x, y) ?? false)
            {
                window.InvalidateFromGadget();
            }
        }
        internal void HandlePropMouseUp(int x, int y)
        {
            if (propInfo?.HandlePropMouseUp(GetBounds(), x, y) ?? false)
            {
                window.InvalidateFromGadget();
            }
        }

        internal bool HandleKeyDown(SDLKeyEventArgs e)
        {
            if (strInfo?.HandleKeyDown(e) ?? false)
            {
                window.InvalidateFromGadget();
                return true;
            }
            return false;
        }
        internal bool HandleKeyUp(SDLKeyEventArgs e)
        {
            if (strInfo?.HandleKeyUp(e) ?? false)
            {
                window.InvalidateFromGadget();
                return true;
            }
            return false;
        }
        internal bool HandleTextInput(SDLTextInputEventArgs e)
        {
            if (strInfo?.HandleTextInput(e) ?? false)
            {
                window.InvalidateFromGadget();
                return true;
            }
            return false;
        }
        internal void ModifyProp(PropFlags flags, int horizPot, int vertPot, int horizBody, int vertBody)
        {
            if (IsPropGadget && propInfo != null)
            {
                propInfo.Flags = flags;
                propInfo.HorizPot = horizPot;
                propInfo.VertPot = vertPot;
                propInfo.HorizBody = horizBody;
                propInfo.VertBody = vertBody;
                window.InvalidateFromGadget();
            }
        }

        protected override void Invalidate()
        {
            propInfo?.Invalidate();
            strInfo?.Invalidate();
            window.InvalidateFromGadget();
        }

        internal void InvalidateBounds()
        {
            bounds = null;
            propInfo?.Invalidate();
            strInfo?.Invalidate();
            window.InvalidateFromGadget();
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
            int x = window.LeftEdge;
            if ((activation & GadgetActivation.LeftBorder) == 0)
            {
                x += window.BorderLeft;
            }
            return x;
        }

        private int GetWindowY()
        {
            int y = window.TopEdge;
            if ((activation & GadgetActivation.TopBorder) == 0)
            {
                y += window.BorderTop;
            }
            return y;
        }

        private int GetWindowW()
        {
            int w = window.Width;
            if ((activation & GadgetActivation.LeftBorder) == 0)
            {
                w -= window.BorderLeft;
            }
            if ((activation & GadgetActivation.RightBorder) == 0)
            {
                w -= window.BorderRight;
            }
            return w;
        }
        private int GetWindowH()
        {
            int h = window.Height;
            if ((activation & GadgetActivation.TopBorder) == 0)
            {
                h -= window.BorderTop;
            }
            if ((activation & GadgetActivation.BottomBorder) == 0)
            {
                h -= window.BorderBottom;
            }
            return h;
        }

        private Rectangle CalculateBounds()
        {
            int x = GetWindowX();
            int y = GetWindowY();
            int w = GetWindowW();
            int h = GetWindowH();
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
            ren.RenderGadget(gfx, this);
        }

        public override string ToString()
        {
            if (IsSysGadget) { return $"SysGadget '{SysGadgetType}'"; }
            if (Text != null) { return $"Gadget '{Text}'"; }
            return $"Gadget '{GadgetId}'";
        }
    }
}
