// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;

using System;

public class SDLHandledEventArgs : EventArgs
{
    private bool handled;

    public bool Handled
    {
        get => handled;
        set => handled = value;
    }

}
