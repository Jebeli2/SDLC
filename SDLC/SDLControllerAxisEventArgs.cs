// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
using System.Numerics;

public class SDLControllerAxisEventArgs : SDLControllerEventArgs
{
    private readonly KeyButtonState state;
    private readonly ControllerAxis axis;
    private readonly int axisValue;
    private readonly Vector2 direction;
    private readonly int axisTriggerValue;
    public SDLControllerAxisEventArgs(SDLController controller, KeyButtonState state, ControllerAxis axis, int axisValue, int axisTriggerValue, Vector2 direction)
        : base(controller)
    {
        this.state = state;
        this.axis = axis;
        this.axisValue = axisValue;
        this.axisTriggerValue = axisTriggerValue;
        this.direction = direction;
    }

    public KeyButtonState State => state;
    public ControllerAxis Axis => axis;

    public int AxisValue => axisValue;
    public int AxisTriggerValue => axisTriggerValue;
    public Vector2 Direction => direction;
}

public delegate void SDLControllerAxisEventHandler(object sender, SDLControllerAxisEventArgs e);
