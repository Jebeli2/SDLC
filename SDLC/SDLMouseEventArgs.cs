// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
public class SDLMouseEventArgs : SDLHandledEventArgs
{
    private readonly int which;
    private readonly int x;
    private readonly int y;
    private readonly MouseButton button;
    private readonly int clicks;
    private readonly KeyButtonState state;
    private readonly int relX;
    private readonly int relY;

    public SDLMouseEventArgs(int which, int x, int y, MouseButton button, int clicks, KeyButtonState state, int relX, int relY)
    {
        this.which = which;
        this.x = x;
        this.y = y;
        this.button = button;
        this.clicks = clicks;
        this.state = state;
        this.relX = relX;
        this.relY = relY;
    }

    public int Which => which;
    public int X => x;
    public int Y => y;
    public MouseButton Button => button;
    public int Clicks => clicks;
    public KeyButtonState State => state;
    public int RelX => relX;
    public int RelY => relY;
}

public delegate void SDLMouseEventHandler(object sender, SDLMouseEventArgs e);
