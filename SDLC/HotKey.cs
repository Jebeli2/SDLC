// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class HotKey : IEquatable<HotKey>
{
    public const KeyMod IgnoredKeyMods = KeyMod.NUM | KeyMod.CAPS;

    public HotKey()
    {
        ControllerButton = ControllerButton.Invalid;
        ScanCode = ScanCode.SCANCODE_UNKNOWN;
        KeyMod = KeyMod.NONE;
        State = KeyButtonState.Invalid;
    }

    public HotKey(ScanCode scanCode)
    {
        ControllerButton = ControllerButton.Invalid;
        ScanCode = scanCode;
        KeyMod = KeyMod.NONE;
        State = KeyButtonState.Pressed;
    }
    public HotKey(ScanCode scanCode, KeyButtonState state)
    {
        ControllerButton = ControllerButton.Invalid;
        ScanCode = scanCode;
        KeyMod = KeyMod.NONE;
        State = state;
    }
    public HotKey(ControllerButton button, KeyButtonState state)
    {
        ControllerButton = button;
        ScanCode = ScanCode.SCANCODE_UNKNOWN;
        KeyMod = KeyMod.NONE;
        State = state;
    }

    public ControllerButton ControllerButton { get; set; }
    public ScanCode ScanCode { get; set; }
    public KeyMod KeyMod { get; set; }
    public KeyButtonState State { get; set; }

    public bool Matches(ControllerButton controllerButton, ScanCode scanCode, KeyMod keyMod, KeyButtonState state)
    {
        if ((controllerButton == ControllerButton) && (scanCode == ScanCode))
        {
            if (State == KeyButtonState.Invalid || State == state)
            {
                if (KeyMod == KeyMod.NONE || (keyMod & KeyMod) != 0)
                {
                    return true;
                }

            }

        }
        return false;
    }
    public bool Matches(SDLKeyEventArgs e)
    {
        return Matches(ControllerButton.Invalid, e.ScanCode, e.KeyMod & ~IgnoredKeyMods, e.State);
    }

    public bool Matches(SDLControllerButtonEventArgs e)
    {
        return Matches(e.Button, ScanCode.SCANCODE_UNKNOWN, KeyMod.NONE, e.State);
    }

    public bool Equals(HotKey? other)
    {
        return other != null && other.ControllerButton == ControllerButton && other.ScanCode == ScanCode && other.KeyMod == KeyMod && other.State == State;
    }

    public override bool Equals(object? obj)
    {
        if (obj is HotKey other) { return Equals(other); }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ControllerButton, ScanCode, KeyMod, State);
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        if (ControllerButton != ControllerButton.Invalid)
        {
            sb.Append("ControllerButton ");
            sb.Append(ControllerButton);
        }
        else if (ScanCode != ScanCode.SCANCODE_UNKNOWN)
        {
            sb.Append("Key ");
            sb.Append(ScanCode);
        }
        if (State != KeyButtonState.Invalid)
        {
            sb.Append(" ");
            sb.Append(State);
        }
        return sb.ToString();
    }
}
