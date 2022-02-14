// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class GadToolsInfo
{
    private readonly Gadget gadget;
    public GadToolsInfo(Gadget gadget)
    {
        this.gadget = gadget;
    }
    internal GadgetKind Kind { get; set; }
    internal Action<int>? ValueChangedAction { get; set; }
    internal Action<bool>? CheckedStateChangedAction { get; set; }
    internal int SliderMin { get; set; }
    internal int SliderMax { get; set; }
    internal int SliderLevel { get; set; }

    internal int ScrollerTotal { get; set; }
    internal int ScrollerVisible { get; set; }
    internal int ScrollerTop { get; set; }
    internal bool CheckboxChecked { get; set; }
    internal int SelectedIndex { get; set; } = -1;
    internal string? Format { get; set; }
    internal Gadget? LinkedGadget { get; set; }
    internal Gadget? TextGadget { get; set; }
    internal List<Gadget>? MxGadgets { get; set; }

    internal void Invalidate()
    {
        if (TextGadget != null)
        {
            TextGadget.Enabled = gadget.Enabled;
        }
        if (MxGadgets != null && MxGadgets.Count > 1)
        {
            Gadget first = MxGadgets[0];
            Gadget second = MxGadgets[1];
            if (first.Enabled != second.Enabled)
            {
                for (int i = 1; i < MxGadgets.Count; i++)
                {
                    MxGadgets[i].Enabled = first.Enabled;
                }
            }
        }
    }

}
