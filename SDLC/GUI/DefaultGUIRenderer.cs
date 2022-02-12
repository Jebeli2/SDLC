// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.GUI;
using System.Drawing;

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

        BorderDark = MkColor(29, 255);
        BorderLight = MkColor(92, 255);
        BorderMedium = MkColor(35, 255);
        TextColor = MkColor(255, 160);
        DisabledTextColor = MkColor(255, 80);
        TextColorShadow = MkColor(0, 160);
        IconColor = TextColor;

        ButtonGradientTopFocused = MkColor(64, 255);
        ButtonGradientBotFocused = MkColor(48, 255);
        ButtonGradientTopUnFocused = MkColor(74, 255);
        ButtonGradientBotUnFocused = MkColor(58, 255);
        ButtonGradientTopPushed = MkColor(41, 255);
        ButtonGradientBotPushed = MkColor(28, 255);
        ButtonGradientTopHover = MkColor(84, 255);
        ButtonGradientBotHover = MkColor(68, 255);

        WindowFillUnFocused = MkColor(43, 230);
        WindowFillFocused = MkColor(45, 230);
        WindowTitleUnFocused = MkColor(220, 160);
        WindowTitleFocused = MkColor(225, 190);

        WindowHeaderGradientTop = ButtonGradientTopUnFocused;
        WindowHeaderGradientBot = ButtonGradientBotUnFocused;
        WindowHeaderGradientTopActive = Color.FromArgb(200, 62 + 10, 92 + 10, 154 + 10);
        WindowHeaderGradientBotActive = Color.FromArgb(130, 62, 92, 154);

        PropGradientTop = MkColor(0, 32);
        PropGradientBot = MkColor(0, 92);
        KnobGradientTop = MkColor(100, 100);
        KnobGradientBot = MkColor(128, 100);
        KnobGradientTopHover = MkColor(220, 100);
        KnobGradientBotHover = MkColor(128, 100);
        StrGradientTop = MkColor(255, 32);
        StrGradientBot = MkColor(32, 32);

        DisabledGhost = MkColor(255, 22);
    }

    private static Color MkColor(int gray, int alpha)
    {
        return Color.FromArgb(alpha, gray, gray, gray);
    }

    public Color BorderDark { get; set; }
    public Color BorderLight { get; set; }
    public Color BorderMedium { get; set; }
    public Color TextColor { get; set; }
    public Color DisabledTextColor { get; set; }
    public Color TextColorShadow { get; set; }
    public Color IconColor { get; set; }
    public Color ButtonGradientTopFocused { get; set; }
    public Color ButtonGradientBotFocused { get; set; }
    public Color ButtonGradientTopUnFocused { get; set; }
    public Color ButtonGradientBotUnFocused { get; set; }
    public Color ButtonGradientTopPushed { get; set; }
    public Color ButtonGradientBotPushed { get; set; }
    public Color ButtonGradientTopHover { get; set; }
    public Color ButtonGradientBotHover { get; set; }

    public Color WindowFillUnFocused { get; set; }
    public Color WindowFillFocused { get; set; }
    public Color WindowTitleUnFocused { get; set; }
    public Color WindowTitleFocused { get; set; }
    public Color WindowHeaderGradientTop { get; set; }
    public Color WindowHeaderGradientBot { get; set; }
    public Color WindowHeaderGradientTopActive { get; set; }
    public Color WindowHeaderGradientBotActive { get; set; }

    public Color PropGradientTop { get; set; }
    public Color PropGradientBot { get; set; }
    public Color KnobGradientTop { get; set; }
    public Color KnobGradientBot { get; set; }
    public Color KnobGradientTopHover { get; set; }
    public Color KnobGradientBotHover { get; set; }
    public Color StrGradientTop { get; set; }
    public Color StrGradientBot { get; set; }

    public Color DisabledGhost { get; set; }


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

    public void RenderScreen(IRenderer gfx, Screen screen, int offsetX, int offsetY)
    {
        DrawScreen(gfx, screen, offsetX, offsetY);
    }

    public void RenderWindow(IRenderer gfx, Window window, int offsetX, int offsetY)
    {
        DrawWindow(gfx, window, offsetX, offsetY);
    }
    public void RenderGadget(IRenderer gfx, Gadget gadget, int offsetX, int offsetY)
    {
        DrawGadget(gfx, gadget, offsetX, offsetY);
    }
    public void RenderRequester(IRenderer gfx, Requester req, int offsetX, int offsetY)
    {
        DrawRequester(gfx, req, offsetX, offsetY);
    }

    private void DrawScreen(IRenderer gfx, Screen screen, int offsetX, int offsetY)
    {
        Rectangle bounds = screen.GetBounds();

    }

    private void DrawWindow(IRenderer gfx, Window window, int offsetX, int offsetY)
    {
        if (window.BackDrop) return;
        Rectangle bounds = window.GetBounds();
        Rectangle inner = window.GetInnerBounds();
        bounds.Offset(offsetX, offsetY);
        inner.Offset(offsetX, offsetY);
        bool active = window.Active;
        bool hover = window.MouseHover;
        Color bg = WindowFillUnFocused;
        Color bt = WindowHeaderGradientTop;
        Color bb = WindowHeaderGradientBot;
        if (active)
        {
            bg = WindowFillFocused;
            bt = WindowHeaderGradientTopActive;
            bb = WindowHeaderGradientBotActive;
        }

        gfx.FillRect(bounds, bg);
        if (!window.Borderless)
        {
            if (window.BorderTop > 2) { gfx.FillVertGradient(bounds.Left, bounds.Top, bounds.Width, window.BorderTop, bt, bb); }
            if (window.BorderLeft > 2) gfx.FillRect(bounds.Left, inner.Top, window.BorderLeft, inner.Height, bb);
            if (window.BorderRight > 2) gfx.FillRect(bounds.Right - window.BorderRight - 1, inner.Top, window.BorderRight, inner.Height, bb);
            if (window.BorderBottom > 2) gfx.FillVertGradient(bounds.Left, bounds.Bottom - window.BorderBottom - 1, bounds.Width, window.BorderBottom, bb, bt);

            DrawBox(gfx, bounds, BorderLight, BorderDark);
            if (!string.IsNullOrEmpty(window.Title))
            {
                gfx.DrawText(null, window.Title, inner.X, bounds.Y, inner.Width, window.BorderTop, active ? WindowTitleFocused : WindowTitleUnFocused);
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
        else if (gadget.IsStrGadget && gadget.StrInfo != null)
        {
            DrawStrGadget(gfx, gadget, gadget.StrInfo, offsetX, offsetY);
        }
    }

    private void DrawRequester(IRenderer gfx, Requester req, int offsetX, int offsetY)
    {
        Rectangle bounds = req.GetBounds();
        Rectangle inner = req.GetInnerBounds();
        bounds.Offset(offsetX, offsetY);
        inner.Offset(offsetX, offsetY);
        bool selected = true;
        bool active = req.Window.Active;
        bool hover = req.Window.MouseHover;
        Color gradTop = ButtonGradientTopUnFocused;
        Color gradBottom = ButtonGradientBotUnFocused;
        if (selected)
        {
            gradTop = ButtonGradientTopPushed;
            gradBottom = ButtonGradientBotPushed;
        }
        else if (active)
        {
            gradTop = ButtonGradientTopFocused;
            gradBottom = ButtonGradientBotFocused;
        }
        else if (hover)
        {
            gradTop = ButtonGradientTopHover;
            gradBottom = ButtonGradientBotHover;
        }
        gfx.FillVertGradient(bounds, gradTop, gradBottom);
        DrawBox(gfx, bounds, BorderLight, BorderDark);
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
        if (gadget.NoHighlight)
        {
            active = false;
            hover = false;
            selected = false;
        }
        if (!gadget.TransparentBackground)
        {
            Color gradTop = ButtonGradientTopUnFocused;
            Color gradBottom = ButtonGradientBotUnFocused;
            if (selected)
            {
                gradTop = ButtonGradientTopPushed;
                gradBottom = ButtonGradientBotPushed;
            }
            else if (active)
            {
                gradTop = ButtonGradientTopFocused;
                gradBottom = ButtonGradientBotFocused;
            }
            else if (hover)
            {
                gradTop = ButtonGradientTopHover;
                gradBottom = ButtonGradientBotHover;
            }
            gfx.FillVertGradient(bounds, gradTop, gradBottom);
            if (!gadget.BackgroundColor.IsEmpty)
            {
                gfx.FillRect(bounds, Color.FromArgb(64, gadget.BackgroundColor));
            }

        }
        if (gadget.IsBorderGadget)
        {
            if (hover)
            {
                DrawBox(gfx, bounds, BorderLight, BorderDark);
            }
        }
        else if (!gadget.NoHighlight)
        {
            DrawBox(gfx, bounds, BorderLight, BorderDark);
        }
        int offset = selected ? 1 : 0;
        bool hasIcon = gadget.Icon != Icons.NONE;
        bool hasText = !string.IsNullOrEmpty(gadget.Text);
        Color tc = gadget.Enabled ? TextColor : DisabledTextColor;

        if (hasIcon && hasText)
        {
            Size textSize = gfx.MeasureText(null, gadget.Text);

            gfx.DrawIcon(gadget.Icon, inner.X, inner.Y, inner.Width / 2 - textSize.Width / 2 - 10, inner.Height, tc, HorizontalAlignment.Right, VerticalAlignment.Center, offset, offset);
            gfx.DrawText(null, gadget.Text, inner.X, inner.Y, inner.Width, inner.Height, tc, HorizontalAlignment.Center, VerticalAlignment.Center, offset, offset);
        }
        else if (hasIcon)
        {
            gfx.DrawIcon(gadget.Icon, inner.X, inner.Y, inner.Width, inner.Height, tc, gadget.HorizontalTextAlignment, gadget.VerticalTextAlignment, offset, offset);
        }
        else if (hasText)
        {
            gfx.DrawText(null, gadget.Text, inner.X, inner.Y, inner.Width, inner.Height, tc, gadget.HorizontalTextAlignment, gadget.VerticalTextAlignment, offset, offset);
        }
        if (!gadget.Enabled && !gadget.NoHighlight)
        {
            gfx.FillRect(bounds, DisabledGhost);
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
        gfx.FillVertGradient(bounds, PropGradientTop, PropGradientBot);
        if (!prop.Borderless)
        {
            DrawBox(gfx, bounds, BorderLight, BorderDark);
        }
        if ((knobHover && hover) || (knobHover && selected) || (knobHit))
        {
            gfx.FillVertGradient(knob, KnobGradientTopHover, KnobGradientBotHover);
        }
        else
        {
            gfx.FillVertGradient(knob, KnobGradientTop, KnobGradientBot);
        }
    }

    private void DrawStrGadget(IRenderer gfx, Gadget gadget, StringInfo strInfo, int offsetX, int offsetY)
    {
        Rectangle bounds = gadget.GetBounds();
        Rectangle inner = gadget.GetInnerBounds();
        bounds.Offset(offsetX, offsetY);
        inner.Offset(offsetX, offsetY);
        gfx.FillVertGradient(bounds, StrGradientTop, StrGradientBot);
        DrawBox(gfx, bounds, BorderDark, BorderLight);
        string buffer = strInfo.Buffer;
        int x = inner.X;
        int y = inner.Y;
        int last = buffer.Length;
        gfx.PushClip(inner);
        int dispPos = strInfo.DispPos;
        int minx;
        int maxx;
        int miny;
        int maxy;
        int advance;
        for (int i = dispPos; i < last + 1; i++)
        {
            char c = ' ';
            if (i < last) { c = buffer[i]; }
            bool selected = (i >= strInfo.BufferSelStart && i < strInfo.BufferSelEnd);
            gfx.GetGlyphMetrics(null, c, out minx, out maxx, out miny, out maxy, out advance);
            string txt = "" + c;
            //Size size = gfx.MeasureText(null, "" + c);
            if (selected)
            {
                gfx.FillRect(x, y, advance, inner.Height, Color.LightBlue);
                gfx.DrawText(null, txt, x, y, SelectedTextColor);
            }
            else
            {
                gfx.DrawText(null, txt, x, y, TextColor);
            }
            if (i == strInfo.BufferPos)
            {
                if (gadget.Active)
                {
                    gfx.DrawLine(x, y, x, y + inner.Height, TextColor);
                }
            }
            x += advance;
        }
        gfx.PopClip();
    }

    private static void DrawBox(IRenderer gfx, Rectangle rect, Color shinePen, Color shadowPen)
    {
        gfx.DrawRect(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2, shinePen);
        gfx.DrawRect(rect.X, rect.Y, rect.Width - 1, rect.Height - 1, shadowPen);
    }

}
