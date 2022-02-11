// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.GUI;
using System.Collections.Generic;

public class Requester : GUIObject
{
    private readonly Window window;
    private readonly List<Gadget> gadgets = new();
    internal Requester(IGUISystem gui, Window window)
        : base(gui)
    {
        this.window = window;
    }

    public Window Window => window;

    public IEnumerable<Gadget> Gadgets => gadgets;
}
