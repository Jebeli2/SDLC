// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLC.GUI;

public static class GUIExtensions
{
    public static bool GetBuffer(this Gadget? gadget, out string buffer)
    {
        buffer = string.Empty;
        if (gadget != null && gadget.IsStrGadget && gadget.StrInfo != null)
        {
            buffer = gadget.StrInfo.Buffer;
            return true;
        }
        return false;
    }

    public static bool GetLongInt(this Gadget? gadget, out long longInt)
    {
        longInt = 0;
        if (gadget != null && gadget.IsStrGadget && gadget.IsIntegerGadget && gadget.StrInfo != null)
        {
            longInt = gadget.StrInfo.LongInt;
            return true;
        }
        return false;
    }

    public static bool GetChecked(this Gadget? gadget, out bool checkedState)
    {
        checkedState = false;
        if (gadget != null && gadget.GadInfo != null && gadget.GadInfo.Kind == GadgetKind.Checkbox)
        {
            checkedState = gadget.GadInfo.CheckboxChecked;
            return true;
        }
        return false;
    }

    public static bool GetSelectedIndex(this Gadget? gadget, out int selectedIndex)
    {
        selectedIndex = -1;
        if (gadget != null && gadget.GadInfo != null && gadget.GadInfo.Kind == GadgetKind.Mx)
        {
            selectedIndex = gadget.GadInfo.SelectedIndex;
            return true;
        }
        return false;
    }

    public static bool GetSliderLevel(this Gadget? gadget, out int level)
    {
        level = 0;
        if (gadget != null && gadget.GadInfo != null && gadget.GadInfo.Kind == GadgetKind.Slider)
        {
            level = gadget.GadInfo.SliderLevel;
            return true;
        }
        return false;
    }

    public static bool GetScrollerTop(this Gadget? gadget, out int top)
    {
        top = 0;
        if (gadget != null && gadget.GadInfo != null && gadget.GadInfo.Kind == GadgetKind.Scroller)
        {
            top = gadget.GadInfo.ScrollerTop;
            return true;
        }
        return false;
    }

}
