// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class HotKeyCommand
{
    public HotKeyCommand(string name, HotKey hotKey, Action? action = null)
    {
        Name = name;
        HotKey = hotKey;
        Action = action;
    }
    public string Name { get; set; }
    public HotKey HotKey { get; set; }
    public Action? Action { get; set; }

    public override string ToString()
    {
        return HotKey.ToString() + "=>" + Name;
    }
}
