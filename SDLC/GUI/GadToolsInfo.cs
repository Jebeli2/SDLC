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
    internal Action<int>? ValueChangedAction { get; set; }
    internal Action<bool>? CheckedStateChangedAction { get; set; }
    internal int SliderMin { get; set; }
    internal int SliderMax { get; set; }
    internal int SliderLevel { get; set; }

    internal int ScrollerTotal { get; set; }
    internal int ScrollerVisible { get; set; }
    internal int ScrollerTop { get; set; }
    internal bool CheckboxChecked { get; set; }
    internal Gadget? TextGadget { get; set; }

    internal void Invalidate()
    {
        if (TextGadget != null)
        {
            TextGadget.Enabled = gadget.Enabled;
        }
    }

}
