// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.GUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class GadTools
{
    private static IGUISystem? guiContext;
    private static Window? windowContext;

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
        Icons icon = Icons.NONE,
        Color? bgColor = null,
        string? buffer = null,
        bool disabled = false,
        bool selected = false,
        bool toggleSelect = false,
        bool endGadget = false,
        Action? clickAction = null,
        Action<int>? valueChangedAction = null,
        int min = 0,
        int max = 15,
        int level = 0,
        int top = 0,
        int total = 0,
        int visible = 2,
        PropFreedom freedom = PropFreedom.Horizontal,
        int gadgetId = -1)
    {
        if (gui == null) { gui = guiContext; }
        if (window == null) { window = windowContext; }
        if (gui == null || window == null) throw new InvalidOperationException($"Cannot create {kind} gadget without context");
        switch (kind)
        {
            case GadgetKind.Button: return CreateButton(gui, window, requester, leftEdge, topEdge, width, height, text, icon, bgColor, disabled, selected, toggleSelect, endGadget, clickAction, gadgetId);
            case GadgetKind.String: return CreateString(gui, window, requester, leftEdge, topEdge, width, height, buffer, disabled, clickAction, gadgetId);
            case GadgetKind.Text: return CreateText(gui, window, requester, leftEdge, topEdge, width, height, text, disabled, gadgetId);
            case GadgetKind.Slider: return CreateSlider(gui, window, requester, leftEdge, topEdge, width, height, min, max, level, freedom, valueChangedAction, gadgetId);
            case GadgetKind.Scroller: return CreateScroller(gui, window, requester, leftEdge, topEdge, width, height, top, total, visible, freedom, valueChangedAction, gadgetId);
        }
        throw new NotSupportedException($"GadgetKind {kind} not supported");
    }

    private static Gadget CreateButton(IGUISystem gui, Window window, Requester? req, int leftEdge, int topEdge, int width, int height, string? text, Icons icon,
        Color? bgColor,
        bool disabled,
        bool selected,
        bool toggleSelect,
        bool endGadget,
        Action? clickAction,
        int gadgetId)
    {
        Gadget gad = gui.AddGadget(window, req, leftEdge, topEdge, width, height, text: text, icon: icon, type: GadgetType.BoolGadget,
            bgColor: bgColor,
            disabled: disabled,
            selected: selected,
            toggleSelect: toggleSelect,
            endGadget: endGadget,
            clickAction: clickAction, gadgetId: gadgetId);
        return gad;
    }
    private static Gadget CreateString(IGUISystem gui, Window window, Requester? req, int leftEdge, int topEdge, int width, int height,
        string? buffer,
        bool disabled,
        Action? clickAction,
        int gadgetId)
    {
        Gadget gad = gui.AddGadget(window, req, leftEdge, topEdge, width, height, type: GadgetType.StrGadget,
            disabled: disabled,
            buffer: buffer,
            clickAction: clickAction,
            gadgetId: gadgetId);
        return gad;
    }

    private static Gadget CreateText(IGUISystem gui, Window window, Requester? req, int leftEdge, int topEdge, int width, int height, string? text,
        bool disabled,
        int gadgetId)
    {
        Gadget gad = gui.AddGadget(window, req, leftEdge, topEdge, width, height, text: text, type: GadgetType.BoolGadget,
            flags: GadgetFlags.HNone,
            activation: GadgetActivation.None,
            gadgetId: gadgetId);
        gad.TransparentBackground = true;
        return gad;
    }

    private static Gadget CreateSlider(IGUISystem gui, Window window, Requester? req, int leftEdge, int topEdge, int width, int height,
        int min, int max, int level, PropFreedom freedom,
        Action<int>? valueChangedAction,
        int gadgetId)
    {
        Gadget gad = gui.AddGadget(window, req, leftEdge, topEdge, width, height, type: GadgetType.PropGadget,
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
        gad.SliderMin = min;
        gad.SliderMax = max;
        gad.SliderLevel = level;
        gad.ValueChangedAction = valueChangedAction;
        FindSliderValues(max + 1 - min, level - min, out int body, out int pot);
        gui.ModifyProp(gad, flags, pot, pot, body, body);

        gad.GadgetDown += SliderGadgetDown;
        gad.GadgetUp += SliderGadgetUp;
        return gad;
    }


    private static Gadget CreateScroller(IGUISystem gui, Window window, Requester? req, int leftEdge, int topEdge, int width, int height,
        int top, int total, int visible, PropFreedom freedom,
        Action<int>? valueChangedAction,
        int gadgetId)
    {
        Gadget gad = gui.AddGadget(window, req, leftEdge, topEdge, width, height, type: GadgetType.PropGadget,
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
        gad.ScrollerTop = top;
        gad.ScrollerTotal = total;
        gad.ScrollerVisible = visible;
        gad.ValueChangedAction = valueChangedAction;
        gui.ModifyProp(gad, flags, pot, pot, body, body);
        gad.GadgetDown += ScrollerGadgetDown;
        gad.GadgetUp += ScrollerGadgetUp;
        return gad;
    }

    private static void SliderPotChanged(Gadget? gadget)
    {
        if (gadget != null && GetPot(gadget, out int pot))
        {
            int level = FindSliderLevel(gadget.SliderMax + 1 - gadget.SliderMin, pot) + gadget.SliderMin;
            if (level != gadget.SliderLevel)
            {
                gadget.SliderLevel = level;
                gadget.ValueChangedAction?.Invoke(level);
            }
        }
    }

    private static void ScrollerPotChanged(Gadget? gadget)
    {
        if (gadget != null && GetPot(gadget, out int pot))
        {
            int top = FindScrollerTop(gadget.ScrollerTotal, gadget.ScrollerVisible, pot);
            if (top != gadget.ScrollerTop)
            {
                gadget.ScrollerTop = top;
                gadget.ValueChangedAction?.Invoke(top);
            }
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
