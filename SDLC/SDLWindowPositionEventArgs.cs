// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;

using System;

public class SDLWindowPositionEventArgs : EventArgs
{
    private readonly int x;
    private readonly int y;

    public SDLWindowPositionEventArgs(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public int X => x;
    public int Y => y;
}

public delegate void SDLWindowPositionEventHandler(object sender, SDLWindowPositionEventArgs e);
