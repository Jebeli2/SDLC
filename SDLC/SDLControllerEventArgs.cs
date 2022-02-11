// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
public class SDLControllerEventArgs : SDLHandledEventArgs
{
    private readonly SDLController controller;

    public SDLControllerEventArgs(SDLController controller)
    {
        this.controller = controller;
    }

    public SDLController Controller => controller;
}
