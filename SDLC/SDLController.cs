// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;

using System;
using System.Numerics;

public class SDLController
{
    private readonly int which;
    private readonly IntPtr handle;
    private SDLWindow? window;
    private string? name;
    private KeyButtonState[] buttonStates = new KeyButtonState[(int)ControllerButton.Max];
    private KeyButtonState[] axisStates = new KeyButtonState[(int)ControllerAxis.MAX];
    private int[] axisValues = new int[(int)ControllerAxis.MAX];
    private uint[] buttonRepeatDownTime = new uint[(int)ControllerButton.Max];
    private uint[] buttonRateDownTime = new uint[(int)ControllerButton.Max];
    private uint[] axisRepeatDownTime = new uint[(int)ControllerAxis.MAX];
    private uint[] axisRateDownTime = new uint[(int)ControllerAxis.MAX];
    private uint repeatDelay = 100;
    private uint repeatRate = 50;
    private int controllerSensitivity = 1000;

    internal SDLController(int which, IntPtr handle)
    {
        this.which = which;
        this.handle = handle;
    }

    public int Which => which;
    public IntPtr Handle => handle;
    internal SDLWindow? Window
    {
        get => window;
        set => window = value;
    }

    public string? Name
    {
        get => name;
        set => name = value;
    }

    public string? Mapping { get; set; }
    internal void HandleButtonRepeats(uint timeStamp)
    {
        for (int i = 0; i < buttonRepeatDownTime.Length; i++)
        {
            if (buttonRepeatDownTime[i] > 0)
            {
                uint diffRep = timeStamp - buttonRepeatDownTime[i];
                if (diffRep > repeatDelay)
                {
                    buttonRateDownTime[i] = timeStamp;
                    buttonRepeatDownTime[i] = 0;
                    window?.RaiseControllerButtonDown(this, (ControllerButton)i, KeyButtonState.Pressed);
                }
            }
            else if (buttonRateDownTime[i] > 0)
            {
                uint diffRat = timeStamp - buttonRateDownTime[i];
                if (diffRat > repeatRate)
                {
                    buttonRateDownTime[i] = timeStamp;
                    window?.RaiseControllerButtonDown(this, (ControllerButton)i, KeyButtonState.Pressed);
                }
            }
        }
    }
    internal void HandleButtonEvent(byte button, byte state, uint timeStamp)
    {
        if (button < buttonStates.Length)
        {
            switch (state)
            {
                case 0:
                    buttonStates[button] = KeyButtonState.Released;
                    buttonRepeatDownTime[button] = 0;
                    buttonRateDownTime[button] = 0;
                    window?.RaiseControllerButtonUp(this, (ControllerButton)button, KeyButtonState.Released);
                    break;
                case 1:
                    buttonStates[button] = KeyButtonState.Pressed;
                    buttonRepeatDownTime[button] = timeStamp;
                    window?.RaiseControllerButtonDown(this, (ControllerButton)button, KeyButtonState.Pressed);
                    break;
            }
        }
    }

    internal void HandleAxisEvent(byte axis, short axisValue, Vector2 direction, uint timeStamp)
    {
        if (axis < axisStates.Length)
        {
            KeyButtonState oldState = axisStates[axis];
            int axisTriggerValue = 0;
            if (direction == Vector2.Zero && oldState == KeyButtonState.Pressed)
            {
                axisStates[axis] = KeyButtonState.Released;
                axisRepeatDownTime[axis] = 0;
                axisRateDownTime[axis] = 0;
                axisTriggerValue = axisValues[axis];
                window?.RaiseControllerAxisEvent(this, KeyButtonState.Released, (ControllerAxis)axis, axisValue, axisTriggerValue, direction);
            }
            else if (direction != Vector2.Zero && oldState == KeyButtonState.Released)
            {
                axisStates[axis] = KeyButtonState.Pressed;
                axisRepeatDownTime[axis] = timeStamp;
                if (axisValue < -controllerSensitivity)
                {
                    axisTriggerValue = -1;
                }
                else if (axisValue > controllerSensitivity)
                {
                    axisTriggerValue = 1;
                }
                else
                {
                    axisTriggerValue = 0;
                }
                axisValues[axis] = axisTriggerValue;
                window?.RaiseControllerAxisEvent(this, KeyButtonState.Pressed, (ControllerAxis)axis, axisValue, axisTriggerValue, direction);
            }
            else if (direction != Vector2.Zero && oldState == KeyButtonState.Pressed)
            {
                uint diffRep = timeStamp - axisRepeatDownTime[axis];
                if (diffRep > repeatDelay)
                {
                    axisRateDownTime[axis] = timeStamp;
                    axisRepeatDownTime[axis] = 0;
                    axisTriggerValue = axisValues[axis];
                    window?.RaiseControllerAxisEvent(this, KeyButtonState.Pressed, (ControllerAxis)axis, axisValue, axisTriggerValue, direction);
                }
                else if (axisRateDownTime[axis] > 0)
                {
                    uint diffRat = timeStamp - axisRateDownTime[axis];
                    if (diffRat > repeatRate)
                    {
                        axisRateDownTime[axis] = timeStamp;
                        axisTriggerValue = axisValues[axis];
                        window?.RaiseControllerAxisEvent(this, KeyButtonState.Pressed, (ControllerAxis)axis, axisValue, axisTriggerValue, direction);
                    }
                }
            }
        }
    }

}
