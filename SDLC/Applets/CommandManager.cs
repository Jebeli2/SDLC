// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.Applets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CommandManager : SDLApplet
{
    private readonly List<Command> commands = new();
    private readonly Dictionary<CommandInput, Command> commandMap = new();
    private bool executeCommandsDirectly = true;
    private readonly CommandInput finder = new();
    public CommandManager() : base("Command Manager")
    {
        noRender = true;
    }

    public Command AddCommand(Action action)
    {
        string name = action.Method.Name;
        return AddCommand(new Command(name, action));
    }

    public Command AddCommand(Command cmd)
    {
        commands.Add(cmd);
        Invalidate();
        return cmd;
    }

    private void Invalidate()
    {
        commandMap.Clear();
    }
    private void MapCommands()
    {
        commandMap.Clear();
        foreach (var cmd in commands)
        {
            foreach (var cmdInput in cmd.Inputs)
            {
                commandMap[cmdInput] = cmd;
            }
        }
    }

    private Command? FindCommand(CommandInput input)
    {
        if (commandMap.Count == 0)
        {
            MapCommands();
        }
        if (commandMap.TryGetValue(input, out var cmd))
        {
            return cmd;
        }
        return null;
    }

    private Command? FindCommand(SDLControllerButtonEventArgs e)
    {
        finder.KeyButtonState = e.State;
        finder.Button = e.Button;
        finder.ScanCode = ScanCode.SCANCODE_UNKNOWN;
        finder.Axis = ControllerAxis.INVALID;
        finder.AxisValue = 0;
        return FindCommand(finder);
    }

    private Command? FindCommand(SDLKeyEventArgs e)
    {
        finder.KeyButtonState = e.State;
        finder.Button = ControllerButton.Invalid;
        finder.ScanCode = e.ScanCode;
        finder.Axis = ControllerAxis.INVALID;
        finder.AxisValue = 0;
        return FindCommand(finder);
    }
    private Command? FindCommand(SDLControllerAxisEventArgs e)
    {
        finder.KeyButtonState = e.State;
        finder.Axis = e.Axis;
        finder.AxisValue = e.AxisTriggerValue;
        finder.Button = ControllerButton.Invalid;
        finder.ScanCode = ScanCode.SCANCODE_UNKNOWN;
        return FindCommand(finder);
    }

    private bool MaybeExceuteCommand(Command? cmd)
    {
        if (cmd != null && executeCommandsDirectly)
        {
            cmd.Action();
            return true;
        }
        return false;
    }

    protected internal override void OnControllerAxis(SDLControllerAxisEventArgs e)
    {
        Command? cmd = FindCommand(e);
        e.Handled = MaybeExceuteCommand(cmd);
    }

    protected internal override void OnControllerButtonDown(SDLControllerButtonEventArgs e)
    {
        Command? cmd = FindCommand(e);
        e.Handled = MaybeExceuteCommand(cmd);
    }

    protected internal override void OnControllerButtonUp(SDLControllerButtonEventArgs e)
    {
        Command? cmd = FindCommand(e);
        e.Handled = MaybeExceuteCommand(cmd);

    }

    protected internal override void OnKeyDown(SDLKeyEventArgs e)
    {
        Command? cmd = FindCommand(e);
        e.Handled = MaybeExceuteCommand(cmd);

    }

    protected internal override void OnKeyUp(SDLKeyEventArgs e)
    {
        Command? cmd = FindCommand(e);
        e.Handled = MaybeExceuteCommand(cmd);
    }

}
