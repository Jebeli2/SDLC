// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;

using System;

public class SDLWindowLoadEventArgs : EventArgs
{
    private readonly IRenderer renderer;
    public SDLWindowLoadEventArgs(IRenderer renderer)
    {
        this.renderer = renderer;
    }
    public IRenderer Renderer => renderer;
}

public delegate void SDLWindowLoadEventHandler(object sender, SDLWindowLoadEventArgs e);
