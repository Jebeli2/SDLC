namespace SDLC.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class PropInfo
    {
        public const int KNOBHMIN = 6;
        public const int KNOBVMIN = 4;
        public const int MAXBODY = 0xFFFF;
        public const int MAXPOT = 0xFFFFF;

        private PropFlags flags;
        private int horizBody;
        private int vertBody;
        private int horizPot;
        private int vertPot;
        private int hpotRes;
        private int vpotRes;
        private int cWidth;
        private int cHeight;
        private int leftBorder;
        private int topBorder;

        private Rectangle knob;
        private bool knobValid;
        private int knobStartX;
        private int knobStartY;
        private bool knobHover;

        private readonly Gadget gadget;

        public PropInfo(Gadget gadget)
        {
            this.gadget = gadget;
        }
        public PropFlags Flags
        {
            get { return flags; }
            set
            {
                if (flags != value)
                {
                    flags = value;
                    knobValid = false;
                }
            }
        }
        public int HorizPot
        {
            get { return horizPot; }
            set
            {
                if (horizPot != value)
                {
                    horizPot = value;
                    knobValid = false;
                }
            }
        }
        public int VertPot
        {
            get { return vertPot; }
            set
            {
                if (vertPot != value)
                {
                    vertPot = value;
                    knobValid = false;
                }
            }
        }
        public int HorizBody
        {
            get { return horizBody; }
            set
            {
                if (horizBody != value)
                {
                    horizBody = value;
                    knobValid = false;
                }
            }
        }
        public int VertBody
        {
            get { return vertBody; }
            set
            {
                if (vertBody != value)
                {
                    vertBody = value;
                    knobValid = false;
                }
            }
        }

        public int HPotRes
        {
            get { return hpotRes; }
            //set { hpotRes = value; }
        }
        public int VPotRes
        {
            get { return vpotRes; }
            //set { vpotRes = value; }
        }

        public bool Borderless
        {
            get { return (flags & PropFlags.PropBorderless) != 0; }
        }

        public bool FreeHoriz
        {
            get { return (flags & PropFlags.FreeHoriz) != 0; }
        }
        public bool FreeVert
        {
            get { return (flags & PropFlags.FreeVert) != 0; }
        }
        public bool KnobHit
        {
            get { return (flags & PropFlags.KnobHit) != 0; }
        }

        public bool KnobHover => knobHover;
        internal void Invalidate()
        {
            knobValid = false;
        }

        internal bool HandleDeselection()
        {
            if (KnobHit)
            {
                flags &= ~PropFlags.KnobHit;
                knobValid = false;
                return true;
            }
            return false;
        }
        internal bool HandlePropMouseDown(Rectangle bounds, int x, int y)
        {
            Rectangle knob = GetKnob(bounds);
            if (knob.Contains(x, y))
            {                
                knobStartX = x - (knob.Left - bounds.X);
                knobStartY = y - (knob.Top - bounds.Y);
                flags |= PropFlags.KnobHit;
                knobHover = true;
                //SDLLog.Debug(LogCategory.APPLICATION, $"KnobHit: {knobStartX}x{knobStartY}");
                Invalidate();
                return true;
            }
            else
            {
                if (KnobHit)
                {
                    knobHover = false;
                }
                else
                {
                    flags &= ~PropFlags.KnobHit;
                    knobHover = false;
                    return HandleContainerHit(knob, x, y, horizPot, vertPot);
                }
            }
            return false;
        }

        internal bool HanldePropMouseMove(Rectangle bounds, int x, int y)
        {
            Rectangle knob = GetKnob(bounds);
            if (KnobHit)
            {
                int dx = x - knobStartX;
                int dy = y - knobStartY;
                //SDLLog.Debug(LogCategory.APPLICATION, $"KnobMove: {dx}x{dy}");
                if (FreeHoriz && (cWidth != knob.Width))
                {
                    dx = (dx * MAXPOT) / (cWidth - knob.Width);
                    if (dx < 0) dx = 0;
                    if (dx > MAXPOT) dx = MAXPOT;
                }
                if (FreeVert && (cHeight != knob.Height))
                {
                    dy = (dy * MAXPOT) / (cHeight - knob.Height);
                    if (dy < 0) dy = 0;
                    if (dy > MAXPOT) dy = MAXPOT;
                }
                if ((FreeHoriz && (dx != horizPot)) || (FreeVert && (dy != vertPot)))
                {
                    horizPot = dx;
                    vertPot = dy;
                    Invalidate();
                    return true;
                }
            }
            else
            {
                return CheckKnobHover(knob, x, y);
            }
            return false;

        }
        internal bool HandlePropMouseUp(Rectangle bounds, int x, int y)
        {
            Rectangle knob = GetKnob(bounds);
            flags &= ~PropFlags.KnobHit;
            knobHover = knob.Contains(x, y);
            Invalidate();
            return true;
        }

        private bool CheckKnobHover(Rectangle knob, int x, int y)
        {
            bool overKnob = knob.Contains(x, y);
            if (overKnob != knobHover)
            {
                knobHover = overKnob;
                Invalidate();
                //SDLLog.Debug(LogCategory.APPLICATION, $"KnobHover: {x}x{y} = {knobHover}");
                return true;
            }
            return false;
        }

        private bool HandleContainerHit(Rectangle knob, int mx, int my, int dx, int dy)
        {
            if (FreeHoriz)
            {
                if (mx < knob.Left)
                {
                    if (dx > hpotRes) { dx -= hpotRes; }
                    else { dx = 0; }
                }
                else if (mx > knob.Right)
                {
                    if (dx < MAXPOT - hpotRes) { dx += hpotRes; }
                    else { dx = MAXPOT; }
                }
            }
            if (FreeVert)
            {
                if (my < knob.Top)
                {
                    if (dy > vpotRes) { dy -= vpotRes; }
                    else { dy = 0; }
                }
                else if (my > knob.Bottom)
                {
                    if (dy < MAXPOT - vpotRes) { dy += vpotRes; }
                    else { dy = MAXPOT; }
                }
            }
            if ((FreeHoriz && (dx != horizPot)) || (FreeVert && (dy != vertPot)))
            {
                horizPot = dx;
                vertPot = dy;
                Invalidate();
                return true;
            }
            return false;
        }

        public Rectangle GetKnob(Rectangle bounds)
        {
            if (!knobValid)
            {
                if (!Borderless)
                {
                    bounds.X += 2;
                    bounds.Y += 2;
                    bounds.Width -= 4;
                    bounds.Height -= 4;
                }
                knob.X = bounds.X;
                knob.Y = bounds.Y;
                knob.Width = bounds.Width;
                knob.Height = bounds.Height;
                cWidth = knob.Width;
                cHeight = knob.Height;
                leftBorder = knob.X;
                topBorder = knob.Y;
                if (FreeHoriz)
                {
                    knob.Width = cWidth * horizBody / MAXBODY;
                    if (knob.Width < KNOBHMIN) knob.Width = KNOBHMIN;
                    knob.X = knob.X + (cWidth - knob.Width) * horizPot / MAXPOT;
                    if (horizBody > 0)
                    {
                        if (horizBody < MAXBODY / 2)
                        {
                            hpotRes = (int)((long)MAXPOT * 32768 / (((long)MAXBODY * 32768 / horizBody) - 32768));
                        }
                        else
                        {
                            hpotRes = MAXPOT;
                        }
                    }
                    else
                    {
                        hpotRes = 1;
                    }
                }
                if (FreeVert)
                {
                    knob.Height = cHeight * vertBody / MAXBODY;
                    if (knob.Height < KNOBVMIN) knob.Height = KNOBVMIN;
                    knob.Y = knob.Y + (cHeight - knob.Height) * vertPot / MAXPOT;
                    if (vertBody > 0)
                    {
                        if (vertBody < MAXBODY / 2)
                        {
                            vpotRes = (int)((long)MAXPOT * 32768 / (((long)MAXBODY * 32768 / VertBody) - 32768));
                        }
                        else
                        {
                            vpotRes = MAXPOT;
                        }
                    }
                    else
                    {
                        vpotRes = 1;
                    }
                }
                knobValid = true;
            }
            return knob;
        }
    }
}
