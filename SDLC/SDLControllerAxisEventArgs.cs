// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
using System.Numerics;

public class SDLControllerAxisEventArgs : SDLControllerEventArgs
{
    private readonly ControllerAxis axis;
    private readonly int axisValue;
    private readonly Vector2 direction;
    public SDLControllerAxisEventArgs(SDLController controller, ControllerAxis axis, int axisValue, Vector2 direction)
        : base(controller)
    {
        this.axis = axis;
        this.axisValue = axisValue;
        this.direction = direction;
    }

    public ControllerAxis Axis => axis;
    public int AxisValue => axisValue;
    public Vector2 Direction => direction;
}

public delegate void SDLControllerAxisEventHandler(object sender, SDLControllerAxisEventArgs e);
