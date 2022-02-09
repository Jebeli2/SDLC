namespace SDLC.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DefaultGUIRenderer : IGUIRenderer
    {
        public DefaultGUIRenderer()
        {
            WindowBackActive = Color.FromArgb(228, 45, 45, 45);
            WindowBackInactive = Color.FromArgb(128, 43, 43, 43);
            WindowBackActiveHover = Color.FromArgb(228, 45 + 20, 45 + 20, 45 + 20);
            WindowBackInactiveHover = Color.FromArgb(128, 43 + 20, 43 + 20, 43 + 20);

            WindowBorderActive = Color.FromArgb(130, 62, 92, 154);
            WindowBorderInactive = Color.FromArgb(130, 50, 50, 50);
            WindowBorderActiveHover = Color.FromArgb(130, 62 + 20, 92 + 20, 154 + 20);
            WindowBorderInactiveHover = Color.FromArgb(130, 50 + 20, 50 + 20, 50 + 20);

            ButtonActive = Color.FromArgb(128, 64, 64, 64);
            ButtonInactive = Color.FromArgb(128, 74, 74, 74);
            ButtonActiveHover = Color.FromArgb(128, 64 + 20, 64 + 20, 64 + 20);
            ButtonInactiveHover = Color.FromArgb(128, 74 + 20, 74 + 20, 74 + 20);

            TextColor = Color.FromArgb(238, 238, 238);
            ActiveTextColor = Color.FromArgb(238, 238, 238);
            InactiveTextColor = Color.FromArgb(200, 200, 200);
            SelectedTextColor = Color.FromArgb(255, 255, 255, 255);

            ShineColor = Color.FromArgb(128, 250, 250, 250);
            ShadowColor = Color.FromArgb(128, 40, 40, 40);
            DarkBackColor = Color.FromArgb(230, 55, 55, 55);
            PropKnobColor = Color.FromArgb(200, 120, 120, 120);
        }
        public Color TextColor { get; set; }
        public Color ActiveTextColor { get; set; }
        public Color InactiveTextColor { get; set; }
        public Color SelectedTextColor { get; set; }

        public Color WindowBackActive { get; set; }
        public Color WindowBackInactive { get; set; }
        public Color WindowBackActiveHover { get; set; }
        public Color WindowBackInactiveHover { get; set; }
        public Color WindowBorderActive { get; set; }
        public Color WindowBorderInactive { get; set; }
        public Color WindowBorderActiveHover { get; set; }
        public Color WindowBorderInactiveHover { get; set; }

        public Color ButtonActive { get; set; }
        public Color ButtonInactive { get; set; }
        public Color ButtonActiveHover { get; set; }
        public Color ButtonInactiveHover { get; set; }
        public Color ShineColor { get; set; }
        public Color ShadowColor { get; set; }
        public Color DarkBackColor { get; set; }
        public Color PropKnobColor { get; set; }

        public void RenderScreen(IRenderer gfx, Screen screen)
        {
            DrawScreen(gfx, screen);
        }

        public void RenderWindow(IRenderer gfx, Window window)
        {
            if (window.Superbitmap)
            {
                DrawWindow(gfx, window, -window.LeftEdge, -window.TopEdge);
            }
            else
            {
                DrawWindow(gfx, window, 0, 0);
            }
        }
        public void RenderGadget(IRenderer gfx, Gadget gadget)
        {
            if (gadget.Window.Superbitmap)
            {
                DrawGadget(gfx, gadget, -gadget.Window.LeftEdge, -gadget.Window.TopEdge);

            }
            else
            {
                DrawGadget(gfx, gadget, 0, 0);
            }
        }

        private void DrawScreen(IRenderer gfx, Screen screen)
        {
            Rectangle bounds = screen.GetBounds();

        }

        private void DrawWindow(IRenderer gfx, Window window, int offsetX, int offsetY)
        {
            Rectangle bounds = window.GetBounds();
            Rectangle inner = window.GetInnerBounds();
            bounds.Offset(offsetX, offsetY);
            inner.Offset(offsetX, offsetY);
            bool active = window.Active;
            bool hover = window.MouseHover;
            Color c1;
            if (hover)
            {
                c1 = active ? WindowBackActiveHover : WindowBackInactiveHover;
            }
            else
            {
                c1 = active ? WindowBackActive : WindowBackInactive;
            }
            Color c2 = Color.FromArgb(c1.A, c1.R + 20, c1.G + 30, c1.B + 40);
            gfx.FillColorRect(bounds, c2, c2, c1, c1);
            Color fc = window.Active ? WindowBorderActive : WindowBorderInactive;
            Color tc = window.Active ? ActiveTextColor : InactiveTextColor;

            if (!window.Borderless)
            {
                gfx.Color = fc;
                if (window.BorderTop > 2) gfx.FillRect(bounds.Left, bounds.Top, bounds.Width, window.BorderTop);
                if (window.BorderLeft > 2) gfx.FillRect(bounds.Left, inner.Top, window.BorderLeft, inner.Height);
                if (window.BorderRight > 2) gfx.FillRect(bounds.Right - window.BorderRight - 1, inner.Top, window.BorderRight, inner.Height);
                if (window.BorderBottom > 2) gfx.FillRect(bounds.Left, bounds.Bottom - window.BorderBottom - 1, bounds.Width, window.BorderBottom);

                DrawBox(gfx, bounds, ShineColor, ShadowColor);
                DrawBox(gfx, inner, ShadowColor, ShineColor);
                if (!string.IsNullOrEmpty(window.Title))
                {
                    gfx.DrawText(null, window.Title, inner.X, bounds.Y, inner.Width, window.BorderTop, tc);
                }
            }
        }

        private void DrawGadget(IRenderer gfx, Gadget gadget, int offsetX, int offsetY)
        {
            if (gadget.IsBoolGadget)
            {
                DrawBoolGadget(gfx, gadget, offsetX, offsetY);
            }
            else if (gadget.IsPropGadget && gadget.PropInfo != null)
            {
                DrawPropGadget(gfx, gadget, gadget.PropInfo, offsetX, offsetY);
            }
        }
        private void DrawBoolGadget(IRenderer gfx, Gadget gadget, int offsetX, int offsetY)
        {
            Rectangle bounds = gadget.GetBounds();
            Rectangle inner = gadget.GetInnerBounds();
            bounds.Offset(offsetX, offsetY);
            inner.Offset(offsetX, offsetY);
            bool active = gadget.Active;
            bool hover = gadget.MouseHover;
            bool selected = gadget.Selected;
            if (!gadget.TransparentBackground)
            {
                Color c1;
                if (hover)
                {
                    c1 = active ? ButtonActiveHover : ButtonInactiveHover;
                }
                else
                {
                    c1 = active ? ButtonActive : ButtonInactive;
                }
                Color c2 = Color.FromArgb(c1.A, c1.R + 20, c1.G + 30, c1.B + 40);

                gfx.FillColorRect(bounds, c2, c2, c1, c1);
            }
            if (gadget.IsBorderGadget)
            {
                if (hover)
                {
                    gfx.DrawRect(bounds, ShineColor);
                }
            }
            else
            {
                if (selected)
                {
                    DrawBox(gfx, bounds, ShadowColor, ShineColor);
                    DrawBox(gfx, inner, ShineColor, ShadowColor);
                }
                else
                {
                    DrawBox(gfx, bounds, ShineColor, ShadowColor);
                    DrawBox(gfx, inner, ShadowColor, ShineColor);
                }
            }
            int offset = selected ? 1 : 0;
            if (gadget.Icon != Icons.NONE)
            {
                Color tc = (active || selected) ? ActiveTextColor : InactiveTextColor;
                gfx.DrawIcon(gadget.Icon, inner.X, inner.Y, inner.Width, inner.Height, tc, HorizontalAlignment.Center, VerticalAlignment.Center, offset, offset);
            }
            if (!string.IsNullOrEmpty(gadget.Text))
            {
                Color tc = (active || selected) ? ActiveTextColor : InactiveTextColor;
                gfx.DrawText(null, gadget.Text, inner.X, inner.Y, inner.Width, inner.Height, tc, HorizontalAlignment.Center, VerticalAlignment.Center, offset, offset);
            }

        }

        private void DrawPropGadget(IRenderer gfx, Gadget gadget, PropInfo prop, int offsetX, int offsetY)
        {
            Rectangle bounds = gadget.GetBounds();
            Rectangle inner = gadget.GetInnerBounds();
            bool active = gadget.Active;
            bool hover = gadget.MouseHover;
            bool selected = gadget.Selected;
            bool knobHit = prop.KnobHit;
            bool knobHover = prop.KnobHover;
            Rectangle knob = prop.GetKnob(bounds);
            bounds.Offset(offsetX, offsetY);
            inner.Offset(offsetX, offsetY);
            knob.Offset(offsetX, offsetY);
            Rectangle innerKnob = knob;
            innerKnob.X += 1;
            innerKnob.Y += 1;
            innerKnob.Width -= 2;
            innerKnob.Height -= 2;
            gfx.FillRect(bounds, DarkBackColor);
            if (!prop.Borderless)
            {
                DrawBox(gfx, bounds, ShineColor, ShadowColor);
            }
            gfx.FillRect(knob, PropKnobColor);
            gfx.DrawRect(knob, ShadowColor);
            if ((knobHover && hover) || (knobHover && selected))
            {
                gfx.DrawRect(innerKnob, ShineColor);
            }
        }

        private static void DrawBox(IRenderer gfx, Rectangle rect, Color shinePen, Color shadowPen)
        {
            gfx.Color = shinePen;
            gfx.DrawLine(rect.Left, rect.Top, rect.Right - 2, rect.Top);
            gfx.DrawLine(rect.Left, rect.Top, rect.Left, rect.Bottom - 2);
            gfx.Color = shadowPen;
            gfx.DrawLine(rect.Left, rect.Bottom - 1, rect.Right - 1, rect.Bottom - 1);
            gfx.DrawLine(rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom - 1);
        }

    }
}
