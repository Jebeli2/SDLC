// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLTest.Blocks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BlockMessage
{
    public string Text { get; set; } = "";
    public int StartRow { get; set; }
    public float Y { get; set; }
    public float Duration { get; set; }
    public float Speed { get; set; }
    public float InitialDuration { get; set; }

    public Size TextSize { get; set; }

    public int Alpha
    {
        get
        {
            float pc = Duration / InitialDuration;
            return (int)(pc * 255);
        }
    }
}
