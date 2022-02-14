// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;

using System;

public class SDLController
{
    private readonly int which;
    private readonly IntPtr handle;
    private SDLWindow? window;
    private string? name;
    private KeyButtonState[] buttonStates = new KeyButtonState[(int)ControllerButton.Max];
    private uint[] buttonRepeatDownTime = new uint[(int)ControllerButton.Max];
    private uint[] buttonRateDownTime = new uint[(int)ControllerButton.Max];
    private uint repeatDelay = 64;
    private uint repeatRate = 31;

    public SDLController(int which, IntPtr handle)
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



}
