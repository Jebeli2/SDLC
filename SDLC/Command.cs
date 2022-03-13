// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Command
{
    private string name;
    private Action action;
    private readonly List<CommandInput> inputs = new();

    public Command(string name, Action action)
    {
        this.name = name;
        this.action = action;
    }

    public string Name => name;
    public Action Action => action;
    public IEnumerable<CommandInput> Inputs => inputs;

    public Command AddKeyPress(params ScanCode[] scanCodes)
    {
        foreach (ScanCode scanCode in scanCodes)
        {
            inputs.Add(CommandInput.GetKeyPressedCommandInput(scanCode));
        }
        return this;
    }
    public Command AddKeyRelease(params ScanCode[] scanCodes)
    {
        foreach (ScanCode scanCode in scanCodes)
        {
            inputs.Add(CommandInput.GetKeyReleasedCommandInput(scanCode));
        }
        return this;
    }

    public Command AddControllerButtonPress(params ControllerButton[] buttons)
    {
        foreach (ControllerButton button in buttons)
        {
            inputs.Add(CommandInput.GetControllerButtonPressedCommandInput(button));
        }
        return this;
    }
    public Command AddControllerButtonRelease(params ControllerButton[] buttons)
    {
        foreach (ControllerButton button in buttons)
        {
            inputs.Add(CommandInput.GetControllerButtonReleasedCommandInput(button));
        }
        return this;
    }

    public Command AddControllerAxisPress(ControllerAxis axis, int value)
    {
        inputs.Add(CommandInput.GetControllerAxisPressedCommandInput(axis, value));
        return this;
    }
    public Command AddControllerAxisRelease(ControllerAxis axis, int value)
    {
        inputs.Add(CommandInput.GetControllerAxisReleasedCommandInput(axis, value));
        return this;
    }

    public bool Matches(SDLKeyEventArgs args)
    {
        foreach (var input in inputs)
        {
            if (input.Matches(args))
            {
                return true;
            }
        }
        return false;
    }

    public bool Matches(SDLControllerButtonEventArgs args)
    {
        foreach (var input in inputs)
        {
            if (input.Matches(args))
            {
                return true;
            }
        }
        return false;
    }

    public virtual bool Matches(SDLControllerAxisEventArgs args)
    {
        foreach (var input in inputs)
        {
            if (input.Matches(args))
            {
                return true;
            }
        }
        return false;
    }


}
