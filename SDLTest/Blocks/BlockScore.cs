// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLTest.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BlockScore : IComparable<BlockScore>
{
    public string Name { get; set; } = "";
    public int Points { get; set; }
    public int Lines { get; set; }
    public int Level { get; set; }
    public bool IsCurrent { get; set; }

    public int CompareTo(BlockScore? other)
    {
        int result = -1;
        if (other != null)
        {
            result = Points.CompareTo(other.Points);
            if (result == 0)
            {
                result = Lines.CompareTo(other.Lines);
                if (result == 0)
                {
                    result = Level.CompareTo(other.Level);
                }
            }
        }
        return result * -1;
    }
}
