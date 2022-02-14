// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLTest.Blocks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Block
{
    private Color color;
    private int x;
    private int y;
    private int points;
    private Tetronimo tetronimo;

    public Block(int x, int y, Color color, Tetronimo tetronimo)
    {
        this.x = x;
        this.y = y;
        this.color = color;
        this.tetronimo = tetronimo;
    }

    public Tetronimo Tetronimo
    {
        get { return tetronimo; }
        set { tetronimo = value; }
    }

    public int Points
    {
        get { return points; }
        set { points = value; }
    }

    public Color Color
    {
        get { return color; }
        set { color = value; }
    }

    public int X
    {
        get { return x; }
        set { x = value; }
    }

    public int Y
    {
        get { return y; }
        set { y = value; }
    }

    public void Move(int dx, int dy)
    {
        x += dx;
        y += dy;
    }

    public override string ToString()
    {
        return $"{tetronimo} ({x}/{y})";
    }
}
