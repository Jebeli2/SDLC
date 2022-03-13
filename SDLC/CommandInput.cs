// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CommandInput : IEquatable<CommandInput>
{
    private KeyButtonState keyButtonState;
    private ScanCode scanCode;
    private ControllerButton button;
    private ControllerAxis axis;
    private int axisValue;


    public CommandInput(KeyButtonState keyButtonState = KeyButtonState.Released,
        ScanCode scanCode = ScanCode.SCANCODE_UNKNOWN,
        ControllerButton button = ControllerButton.Invalid,
        ControllerAxis axis = ControllerAxis.INVALID,
        int axisValue = 0)
    {
        this.keyButtonState = keyButtonState;
        this.scanCode = scanCode;
        this.button = button;
        this.axis = axis;
        this.axisValue = axisValue;
    }

    public KeyButtonState KeyButtonState
    {
        get => keyButtonState;
        internal set => keyButtonState = value;
    }

    public ScanCode ScanCode
    {
        get => scanCode;
        internal set => scanCode = value;
    }

    public ControllerButton Button
    {
        get => button;
        internal set => button = value;
    }

    public ControllerAxis Axis
    {
        get => axis;
        internal set => axis = value;
    }

    public int AxisValue
    {
        get => axisValue;
        internal set => axisValue = value;
    }

    public bool Matches(SDLKeyEventArgs args)
    {
        if (args.State == keyButtonState)
        {
            if (args.ScanCode == scanCode)
            {
                return true;
            }
        }
        return false;
    }

    public bool Matches(SDLControllerButtonEventArgs args)
    {
        if (args.State == keyButtonState)
        {
            if (args.Button == button)
            {
                return true;
            }
        }
        return false;
    }

    public bool Matches(SDLControllerAxisEventArgs args)
    {
        return false;
    }

    public bool Equals(CommandInput? other)
    {
        return other != null && other.axisValue == axisValue && other.axis == axis && other.button == button && other.scanCode == scanCode && other.keyButtonState == keyButtonState;
    }

    public override bool Equals(object? obj)
    {
        if (obj is HotKey other) { return Equals(other); }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(keyButtonState, scanCode, button, axis, axisValue);
    }

    public static CommandInput GetKeyPressedCommandInput(ScanCode scanCode)
    {
        return new CommandInput(KeyButtonState.Pressed, scanCode: scanCode);
    }

    public static CommandInput GetKeyReleasedCommandInput(ScanCode scanCode)
    {
        return new CommandInput(KeyButtonState.Released, scanCode: scanCode);
    }
    public static CommandInput GetControllerButtonPressedCommandInput(ControllerButton button)
    {
        return new CommandInput(KeyButtonState.Pressed, button: button);
    }
    public static CommandInput GetControllerButtonReleasedCommandInput(ControllerButton button)
    {
        return new CommandInput(KeyButtonState.Released, button: button);
    }

    public static CommandInput GetControllerAxisPressedCommandInput(ControllerAxis axis, int axisValue)
    {
        return new CommandInput(KeyButtonState.Pressed, axis: axis, axisValue: axisValue);
    }

    public static CommandInput GetControllerAxisReleasedCommandInput(ControllerAxis axis, int axisValue)
    {
        return new CommandInput(KeyButtonState.Released, axis: axis, axisValue: axisValue);
    }

}

