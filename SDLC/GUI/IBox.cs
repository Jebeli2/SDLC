// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.GUI;
using System.Drawing;

public interface IBox
{
    int LeftEdge { get; set; }
    int TopEdge { get; set; }
    int Width { get; set; }
    int Height { get; set; }
    int BorderTop { get; set; }
    int BorderLeft { get; set; }
    int BorderRight { get; set; }
    int BorderBottom { get; set; }
    bool Contains(int x, int y);
    Rectangle GetBounds();
    Rectangle GetInnerBounds();
}
