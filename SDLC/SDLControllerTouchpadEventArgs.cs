﻿// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
public class SDLControllerTouchpadEventArgs : SDLControllerEventArgs
{
    private readonly int touchpad;
    private readonly int finger;
    private readonly float x;
    private readonly float y;
    private readonly float pressure;
    public SDLControllerTouchpadEventArgs(SDLController controller, int touchpad, int finger, float x, float y, float pressure)
        : base(controller)
    {
        this.touchpad = touchpad;
        this.finger = finger;
        this.x = x;
        this.y = y;
        this.pressure = pressure;
    }

    public int Touchpad => touchpad;
    public int Finger => finger;
    public float X => x;
    public float Y => y;
    public float Pressure => pressure;

}

public delegate void SDLControllerTouchpadEventHandler(object sender, SDLControllerTouchpadEventArgs e);
