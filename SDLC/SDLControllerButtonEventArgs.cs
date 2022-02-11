// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
public class SDLControllerButtonEventArgs : SDLControllerEventArgs
{
    private readonly ControllerButton button;
    private readonly KeyButtonState state;
    public SDLControllerButtonEventArgs(SDLController controller, ControllerButton button, KeyButtonState state)
        : base(controller)
    {
        this.button = button;
        this.state = state;
    }

    public ControllerButton Button => button;
    public KeyButtonState State => state;
}

public delegate void SDLControllerButtonEventHandler(object sender, SDLControllerButtonEventArgs e);
