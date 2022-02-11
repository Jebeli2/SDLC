// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;

using System;

public class SDLWindowSizeEventArgs : EventArgs
{
    private readonly int width;
    private readonly int height;
    private readonly WindowResizeSource source;
    public SDLWindowSizeEventArgs(int width, int height, WindowResizeSource source)
    {
        this.width = width;
        this.height = height;
        this.source = source;
    }

    public int Width => width;
    public int Height => height;
    public WindowResizeSource Source => source;

}

public delegate void SDLWindowSizeEventHandler(object sender, SDLWindowSizeEventArgs e);
