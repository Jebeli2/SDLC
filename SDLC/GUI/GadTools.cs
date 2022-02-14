// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.GUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class GadTools
{
    private static IGUISystem? guiContext;
    private static Window? windowContext;
    private const int CHECKBOX_WIDTH = 28;
    private const int CHECKBOX_HEIGHT = 22;
    private const int MX_WIDTH = 20;
    private const int MX_HEIGHT = 20;
    private const int INTERWIDTH = 8;
    private const int INTERHEIGHT = 4;
    public static void CreateContext(IGUISystem gui, Window window)
    {
        guiContext = gui;
        windowContext = window;
    }
    public static Gadget CreateGadget(GadgetKind kind, IGUISystem? gui = null, Window? window = null,
        Requester? requester = null,
        int leftEdge = 0,
        int topEdge = 0,
        int width = 100,
        int height = 50,
        string? text = null,
        IList? options = null,
        Icons icon = Icons.NONE,
        Color? bgColor = null,
        string? tooltip = null,
        string? buffer = null,
        long intValue = 0,
        int selectedIndex = 0,
        bool disabled = false,
        bool selected = false,
        bool toggleSelect = false,
        bool endGadget = false,
        bool _checked = false,
        Action? clickAction = null,
        Action<int>? valueChangedAction = null,
        Action<bool>? checkedStateChangedAction = null,
        int min = 0,
        int max = 15,
        int level = 0,
        int top = 0,
        int total = 0,
        int visible = 2,
        PropFreedom freedom = PropFreedom.Horizontal,
        bool scaled = false,
        HorizontalAlignment htAlign = HorizontalAlignment.Center,
        VerticalAlignment vtAlign = VerticalAlignment.Center,
        PlaceText placeText = 0,
        int gadgetId = -1)
    {
        if (gui == null) { gui = guiContext; }
        if (window == null) { window = windowContext; }
        if (gui == null || window == null) throw new InvalidOperationException($"Cannot create {kind} gadget without context");
        switch (kind)
        {
            case GadgetKind.Button: return CreateButton(gui, window, requester, leftEdge, topEdge, width, height, text, icon, bgColor, tooltip, disabled, selected, toggleSelect, endGadget, clickAction, gadgetId);
            case GadgetKind.Checkbox: return CreateCheckbox(gui, window, requester, leftEdge, topEdge, width, height, text, tooltip, _checked, disabled, scaled, checkedStateChangedAction, gadgetId);
            case GadgetKind.Mx: return CreateMx(gui, window, requester, leftEdge, topEdge, width, height, options, selectedIndex, tooltip, valueChangedAction, scaled, gadgetId);
            case GadgetKind.String: return CreateString(gui, window, requester, leftEdge, topEdge, width, height, buffer, tooltip, disabled, clickAction, gadgetId);
            case GadgetKind.Integer: return CreateInteger(gui, window, requester, leftEdge, topEdge, width, height, intValue, tooltip, disabled, clickAction, gadgetId);
            case GadgetKind.Text: return CreateText(gui, window, requester, leftEdge, topEdge, width, height, text, tooltip, htAlign, vtAlign, disabled, gadgetId);
            case GadgetKind.Number: return CreateNumber(gui, window, requester, leftEdge, topEdge, width, height, intValue, text ?? "{0}", tooltip, htAlign, vtAlign, disabled, gadgetId);
            case GadgetKind.Slider: return CreateSlider(gui, window, requester, leftEdge, topEdge, width, height, min, max, level, freedom, placeText, tooltip, valueChangedAction, gadgetId);
            case GadgetKind.Scroller: return CreateScroller(gui, window, requester, leftEdge, topEdge, width, height, top, total, visible, freedom, tooltip, valueChangedAction, gadgetId);
        }
        throw new NotSupportedException($"GadgetKind {kind} not supported");
    }

    public static void SetAttrs(Gadget gadget, int? intValue = null)
    {
        switch (GetGadgetKind(gadget))
        {
            case GadgetKind.Number: SetNumberAttrs(gadget, intValue); break;
        }
    }
    private static GadgetKind GetGadgetKind(Gadget? gadget)
    {
        return gadget?.GadInfo?.Kind ?? GadgetKind.None;
    }

    private static bool IsValid(Gadget? gadget, GadgetKind expectedKind, out GadToolsInfo? info)
    {
        info = null;
        if (gadget != null)
        {
            info = gadget.GadInfo;
            if (info != null)
            {
                return info.Kind == expectedKind;
            }
        }
        return false;
    }

    private static void SetNumberAttrs(Gadget gadget, int? intValue = null)
    {
        if (IsValid(gadget, GadgetKind.Number, out GadToolsInfo? info))
        {
            if (intValue != null)
            {
                string format = info?.Format ?? "{0}";
                gadget.Text = string.Format(format, intValue.Value);
            }
        }
    }
    private static Gadget CreateButton(IGUISystem gui, Window window, Requester? req, int leftEdge, int topEdge, int width, int height,
    string? text,
    Icons icon,
    Color? bgColor,
    string? tooltip,
    bool disabled,
    bool selected,
    bool toggleSelect,
    bool endGadget,
    Action? clickAction,
    int gadgetId)
    {
        Gadget gad = gui.AddGadget(window, req, leftEdge, topEdge, width, height, text: text, icon: icon, type: GadgetType.BoolGadget | GadgetType.GadToolsGadget,
            bgColor: bgColor,
            tooltip: tooltip,
            disabled: disabled,
            selected: selected,
            toggleSelect: toggleSelect,
            endGadget: endGadget,
            clickAction: clickAction, gadgetId: gadgetId);
        if (gad.GadInfo != null)
        {
            gad.GadInfo.Kind = GadgetKind.Button;
        }
        return gad;
    }

    private static Gadget CreateCheckbox(IGUISystem gui, Window window, Requester? req, int leftEdge, int topEdge, int width, int height,
        string? text,
        string? tooltip,
        bool _checked,
        bool disabled,
        bool scaled,
        Action<bool>? checkedStateChangedAction,
        int gadgetId)
    {
        int boxWidth = scaled ? width : CHECKBOX_WIDTH;
        int boxHeight = scaled ? height : CHECKBOX_HEIGHT;
        Icons icon = _checked ? Icons.ENTYPO_ICON_CHECK : Icons.NONE;
        Gadget gad = gui.AddGadget(window, req, leftEdge, topEdge, boxWidth, boxHeight, type: GadgetType.BoolGadget | GadgetType.GadToolsGadget,
            icon: icon,
            tooltip: tooltip,
            disabled: disabled,
            gadgetId: gadgetId);
        if (gad.GadInfo != null)
        {
            gad.GadInfo.Kind = GadgetKind.Checkbox;
            gad.GadInfo.CheckboxChecked = _checked;
            gad.GadInfo.CheckedStateChangedAction = checkedStateChangedAction;
            Gadget textGad = CreateText(gui, window, req, leftEdge + boxWidth, topEdge, width - boxWidth, boxHeight, text,
                tooltip, HorizontalAlignment.Left, VerticalAlignment.Center, disabled, gadgetId);
            textGad.TextOffsetX = INTERWIDTH;
            gad.GadInfo.TextGadget = textGad;
            LinkTextGadget(gad, textGad);
        }
        gad.GadgetUp += CheckboxGadgetUp;
        return gad;
    }

    private static Gadget CreateMx(IGUISystem gui, Window window, Requester? req, int leftEdge, int topEdge, int width, int height,
        IList? options,
        int selectedIndex,
        string? tooltip,
        Action<int>? valueChangedAction,
        bool scaled,
        int gadgetId)
    {
        if (options != null && options.Count > 0)
        {
            int boxWidth = scaled ? width : MX_WIDTH;
            int boxHeight = scaled ? height : MX_HEIGHT;
            int numOptions = options.Count;
            if (selectedIndex >= numOptions) { selectedIndex = numOptions - 1; }
            if (selectedIndex < 0) { selectedIndex = 0; }
            Gadget? firstGad = null;
            List<Gadget> mxButtons = new List<Gadget>();
            int y = topEdge;
            for (int index = 0; index < numOptions; index++)
            {
                object? obj = options[index];
                string text = obj?.ToString() ?? "";
                Icons icon = Icons.NONE;
                if (index == selectedIndex) { icon = Icons.ENTYPO_ICON_CHECK; }
                Gadget gad = gui.AddGadget(window, req, leftEdge, y, boxWidth, boxHeight, type: GadgetType.BoolGadget | GadgetType.GadToolsGadget,
                    tooltip: tooltip,
                    icon: icon,
                    gadgetId: gadgetId);
                gad.TransparentBackground = true;
                mxButtons.Add(gad);
                gad.GadgetUp += MxGadgetUp;
                if (firstGad == null) { firstGad = gad; }
                if (gad.GadInfo != null)
                {
                    gad.GadInfo.Kind = GadgetKind.Mx;
                    gad.GadInfo.MxGadgets = mxButtons;
                    gad.GadInfo.SelectedIndex = selectedIndex;
                    gad.GadInfo.ValueChangedAction = valueChangedAction;
                    Gadget textGad = CreateText(gui, window, req, leftEdge + boxWidth, y, width - boxWidth, boxHeight, text,
                        tooltip, HorizontalAlignment.Left, VerticalAlignment.Center, false, gadgetId);
                    textGad.TextOffsetX = INTERWIDTH;
                    gad.GadInfo.TextGadget = textGad;
                    LinkTextGadget(gad, textGad);
                }
                y += boxHeight;
                y += 1;
            }
            if (firstGad != null)
            {
                return firstGad;
            }
        }
        throw new InvalidOperationException("Mx Gadget needs at least one option");

    }


    private static Gadget CreateString(IGUISystem gui, Window window, Requester? req, int leftEdge, int topEdge, int width, int height,
        string? buffer,
        string? tooltip,
        bool disabled,
        Action? clickAction,
        int gadgetId)
    {
        Gadget gad = gui.AddGadget(window, req, leftEdge, topEdge, width, height, type: GadgetType.StrGadget | GadgetType.GadToolsGadget,
            disabled: disabled,
            buffer: buffer,
            tooltip: tooltip,
            clickAction: clickAction,
            gadgetId: gadgetId);
        if (gad.GadInfo != null)
        {
            gad.GadInfo.Kind = GadgetKind.String;
        }
        return gad;
    }
    private static Gadget CreateInteger(IGUISystem gui, Window window, Requester? req, int leftEdge, int topEdge, int width, int height,
        long intValue,
        string? tooltip,
        bool disabled,
        Action? clickAction,
        int gadgetId)
    {
        Gadget gad = gui.AddGadget(window, req, leftEdge, topEdge, width, height, type: GadgetType.StrGadget | GadgetType.GadToolsGadget,
            activation: GadgetActivation.Immediate | GadgetActivation.RelVerify | GadgetActivation.LongInt,
            disabled: disabled,
            buffer: intValue.ToString(),
            tooltip: tooltip,
            clickAction: clickAction,
            gadgetId: gadgetId);
        if (gad.GadInfo != null)
        {
            gad.GadInfo.Kind = GadgetKind.Integer;
        }
        return gad;
    }

    private static Gadget CreateText(IGUISystem gui, Window window, Requester? req, int leftEdge, int topEdge, int width, int height, string? text,
        string? tooltip,
        HorizontalAlignment htAlign,
        VerticalAlignment vtAlign,
        bool disabled,
        int gadgetId)
    {
        Gadget gad = gui.AddGadget(window, req, leftEdge, topEdge, width, height, text: text, type: GadgetType.BoolGadget | GadgetType.GadToolsGadget,
            flags: GadgetFlags.HNone,
            activation: GadgetActivation.None,
            tooltip: tooltip,
            disabled: disabled,
            gadgetId: gadgetId);
        gad.VerticalTextAlignment = vtAlign;
        gad.HorizontalTextAlignment = htAlign;
        gad.TransparentBackground = true;
        if (gad.GadInfo != null)
        {
            gad.GadInfo.Kind = GadgetKind.Text;
        }
        return gad;
    }
    private static Gadget CreateNumber(IGUISystem gui, Window window, Requester? req, int leftEdge, int topEdge, int width, int height, long intValue,
        string format,
        string? tooltip,
        HorizontalAlignment htAlign,
        VerticalAlignment vtAlign,
        bool disabled,
        int gadgetId)
    {
        Gadget gad = gui.AddGadget(window, req, leftEdge, topEdge, width, height, text: string.Format(format, intValue), type: GadgetType.BoolGadget | GadgetType.GadToolsGadget,
            flags: GadgetFlags.HNone,
            activation: GadgetActivation.None,
            tooltip: tooltip,
            disabled: disabled,
            gadgetId: gadgetId);
        gad.VerticalTextAlignment = vtAlign;
        gad.HorizontalTextAlignment = htAlign;
        gad.TransparentBackground = true;
        if (gad.GadInfo != null)
        {
            gad.GadInfo.Kind = GadgetKind.Number;
            gad.GadInfo.Format = format;
        }
        return gad;
    }

    private static Gadget CreateSlider(IGUISystem gui, Window window, Requester? req, int leftEdge, int topEdge, int width, int height,
        int min, int max, int level, PropFreedom freedom,
        PlaceText levelPlace,
        string? tooltip,
        Action<int>? valueChangedAction,
        int gadgetId)
    {
        if (levelPlace == 0) { levelPlace = PlaceText.Left; }
        Gadget gad = gui.AddGadget(window, req, leftEdge, topEdge, width, height, type: GadgetType.PropGadget | GadgetType.GadToolsGadget,
            tooltip: tooltip,
            gadgetId: gadgetId);
        PropFlags flags = 0;
        if (freedom == PropFreedom.Horizontal)
        {
            flags |= PropFlags.FreeHoriz;
        }
        else if (freedom == PropFreedom.Vertical)
        {
            flags |= PropFlags.FreeVert;
        }
        if (level > max) { level = max; }
        if (level < min) { level = min; }
        if (gad.GadInfo != null)
        {
            gad.GadInfo.Kind = GadgetKind.Slider;
            gad.GadInfo.SliderMin = min;
            gad.GadInfo.SliderMax = max;
            gad.GadInfo.SliderLevel = level;
            gad.GadInfo.ValueChangedAction = valueChangedAction;
        }
        FindSliderValues(max + 1 - min, level - min, out int body, out int pot);
        gui.ModifyProp(gad, flags, pot, pot, body, body);

        gad.GadgetDown += SliderGadgetDown;
        gad.GadgetUp += SliderGadgetUp;
        return gad;
    }


    private static Gadget CreateScroller(IGUISystem gui, Window window, Requester? req, int leftEdge, int topEdge, int width, int height,
        int top, int total, int visible, PropFreedom freedom,
        string? tooltip,
        Action<int>? valueChangedAction,
        int gadgetId)
    {
        Gadget gad = gui.AddGadget(window, req, leftEdge, topEdge, width, height, type: GadgetType.PropGadget | GadgetType.GadToolsGadget,
            tooltip: tooltip,
            gadgetId: gadgetId);
        PropFlags flags = 0;
        if (freedom == PropFreedom.Horizontal)
        {
            flags |= PropFlags.FreeHoriz;
        }
        else if (freedom == PropFreedom.Vertical)
        {
            flags |= PropFlags.FreeVert;
        }
        FindScrollerValues(total, visible, top, 0, out int body, out int pot);
        if (gad.GadInfo != null)
        {
            gad.GadInfo.Kind = GadgetKind.Scroller;
            gad.GadInfo.ScrollerTop = top;
            gad.GadInfo.ScrollerTotal = total;
            gad.GadInfo.ScrollerVisible = visible;
            gad.GadInfo.ValueChangedAction = valueChangedAction;
        }
        gui.ModifyProp(gad, flags, pot, pot, body, body);
        gad.GadgetDown += ScrollerGadgetDown;
        gad.GadgetUp += ScrollerGadgetUp;
        return gad;
    }

    private static void LinkTextGadget(Gadget gadget, Gadget textGadget)
    {
        if (gadget.GadInfo != null && textGadget.GadInfo != null)
        {
            textGadget.GadInfo.LinkedGadget = gadget;
            textGadget.Activation |= GadgetActivation.RelVerify;
            textGadget.GadgetUp += TextGadgetGadgetUp;
        }
    }

    private static void TextGadgetGadgetUp(object? sender, EventArgs e)
    {
        if (sender is Gadget gadget)
        {
            gadget.GadInfo?.LinkedGadget?.RaiseGadgetUp();
        }
    }

    private static void SliderGadgetUp(object? sender, EventArgs e)
    {
        SliderPotChanged(sender as Gadget);
    }

    private static void SliderGadgetDown(object? sender, EventArgs e)
    {
        SliderPotChanged(sender as Gadget);
    }

    private static void ScrollerGadgetUp(object? sender, EventArgs e)
    {
        ScrollerPotChanged(sender as Gadget);
    }

    private static void ScrollerGadgetDown(object? sender, EventArgs e)
    {
        ScrollerPotChanged(sender as Gadget);
    }
    private static void CheckboxGadgetUp(object? sender, EventArgs e)
    {
        if (sender is Gadget gadget && gadget.GadInfo != null)
        {
            bool check = !gadget.GadInfo.CheckboxChecked;
            gadget.GadInfo.CheckboxChecked = check;
            gadget.Icon = check ? Icons.ENTYPO_ICON_CHECK : Icons.NONE;
            gadget.GadInfo.CheckedStateChangedAction?.Invoke(check);
        }
    }
    private static void MxGadgetUp(object? sender, EventArgs e)
    {
        if (sender is Gadget gadget && gadget.GadInfo != null)
        {
            List<Gadget>? mxButtons = gadget.GadInfo.MxGadgets;
            if (mxButtons != null)
            {
                int index = mxButtons.IndexOf(gadget);
                if (index >= 0)
                {
                    foreach (Gadget gad in mxButtons)
                    {
                        if (gad == gadget)
                        {
                            gad.Icon = Icons.ENTYPO_ICON_CHECK;
                            gad.Selected = true;
                        }
                        else
                        {
                            gad.Icon = Icons.NONE;
                            gad.Selected = false;
                        }
                    }
                }
                gadget.GadInfo.SelectedIndex = index;
                gadget.GadInfo.ValueChangedAction?.Invoke(index);
            }
        }
    }
    private static void SliderPotChanged(Gadget? gadget)
    {
        if (gadget != null && GetPot(gadget, out int pot))
        {
            if (gadget.GadInfo != null)
            {
                int level = FindSliderLevel(gadget.GadInfo.SliderMax + 1 - gadget.GadInfo.SliderMin, pot) + gadget.GadInfo.SliderMin;
                if (level != gadget.GadInfo.SliderLevel)
                {
                    gadget.GadInfo.SliderLevel = level;
                    gadget.GadInfo.ValueChangedAction?.Invoke(level);
                }
            }
        }
    }

    private static void ScrollerPotChanged(Gadget? gadget)
    {
        if (gadget != null && GetPot(gadget, out int pot))
        {
            if (gadget.GadInfo != null)
            {
                int top = FindScrollerTop(gadget.GadInfo.ScrollerTotal, gadget.GadInfo.ScrollerVisible, pot);
                if (top != gadget.GadInfo.ScrollerTop)
                {
                    gadget.GadInfo.ScrollerTop = top;
                    gadget.GadInfo.ValueChangedAction?.Invoke(top);
                }
            }
        }
    }


    private static bool GetPot(Gadget gadget, out int pot)
    {
        pot = 0;
        if (gadget.IsPropGadget && gadget.PropInfo != null)
        {
            if (gadget.PropInfo.FreeVert)
            {
                pot = gadget.PropInfo.VertPot;
                return true;
            }
            else if (gadget.PropInfo.FreeHoriz)
            {
                pot = gadget.PropInfo.HorizPot;
                return true;
            }
        }
        return false;
    }

    private static void FindSliderValues(int numLevels, int level, out int body, out int pot)
    {
        if (numLevels > 0)
        {
            body = PropInfo.MAXBODY / numLevels;
        }
        else
        {
            body = PropInfo.MAXBODY;
        }
        if (numLevels > 1)
        {
            pot = (PropInfo.MAXPOT * level) / (numLevels - 1);
        }
        else
        {
            pot = 0;
        }
    }

    private static int FindSliderLevel(int numLevels, int pot)
    {
        if (numLevels > 1)
        {
            return (pot * (numLevels - 1) + PropInfo.MAXPOT / 2) / PropInfo.MAXPOT;
        }
        else
        {
            return 0;
        }
    }

    private static void FindScrollerValues(int total, int displayable, int top, int overlap, out int body, out int pot)
    {
        int hidden = Math.Max(total - displayable, 0);
        if (top > hidden) { top = hidden; }
        body = (hidden > 0) ? ((displayable - overlap) * PropInfo.MAXBODY) / (total - overlap) : PropInfo.MAXBODY;
        pot = (hidden > 0) ? (top * PropInfo.MAXBODY) / hidden : 0;
    }

    private static int FindScrollerTop(int total, int displayable, int pot)
    {
        int hidden = Math.Max(total - displayable, 0);
        return ((hidden * pot) + (PropInfo.MAXPOT / 2)) / PropInfo.MAXPOT;
    }

}
