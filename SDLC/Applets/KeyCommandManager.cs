// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.Applets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class KeyCommandManager : SDLApplet
{
    private readonly Dictionary<HotKey, HotKeyCommand> commands = new();
    private readonly HotKey finder = new();

    public KeyCommandManager() : base("Key Command Manager")
    {
        noRender = true;
    }

    public void Clear()
    {
        commands.Clear();
    }

    public void AddHotKeyCommand(HotKeyCommand cmd)
    {
        commands[cmd.HotKey] = cmd;
    }

    public void AddKeyCommand(ScanCode scanCode, Action action)
    {
        string name = action.Method.Name;
        AddHotKeyCommand(new HotKeyCommand(name, new HotKey(scanCode), action));
    }

    public void AddKeyCommand(ScanCode scanCode, KeyButtonState state, Action action)
    {
        string name = action.Method.Name;
        AddHotKeyCommand(new HotKeyCommand(name, new HotKey(scanCode, state), action));
    }
    public void AddKeyCommand(ControllerButton button, KeyButtonState state, Action action)
    {
        string name = action.Method.Name;
        AddHotKeyCommand(new HotKeyCommand(name, new HotKey(button, state), action));
    }

    public HotKeyCommand? FindHotKeyAction(HotKey hotKey)
    {
        if (commands.TryGetValue(hotKey, out HotKeyCommand? hotKeyAction))
        {
            return hotKeyAction;
        }
        return null;
    }

    public HotKeyCommand? FindHotKeyAction(ScanCode scanCode, KeyMod keyMod, KeyButtonState state)
    {
        finder.ControllerButton = ControllerButton.Invalid;
        finder.ScanCode = scanCode;
        finder.KeyMod = keyMod & ~HotKey.IgnoredKeyMods;
        finder.State = state;
        return FindHotKeyAction(finder);
    }
    public HotKeyCommand? FindHotKeyAction(ControllerButton button, KeyButtonState state)
    {
        finder.ControllerButton = button;
        finder.ScanCode = ScanCode.SCANCODE_UNKNOWN;
        finder.KeyMod = KeyMod.NONE;
        finder.State = state;
        return FindHotKeyAction(finder);
    }

    public HotKeyCommand? FindHotKeyAction(SDLKeyEventArgs e)
    {
        return FindHotKeyAction(e.ScanCode, e.KeyMod, e.State);
    }
    public HotKeyCommand? FindHotKeyAction(SDLControllerButtonEventArgs e)
    {
        return FindHotKeyAction(e.Button, e.State);
    }

    protected internal override void OnKeyDown(SDLKeyEventArgs e)
    {
        ExecuteHotKeyCommand(e);
    }

    protected internal override void OnKeyUp(SDLKeyEventArgs e)
    {
        ExecuteHotKeyCommand(e);
    }

    protected internal override void OnControllerButtonDown(SDLControllerButtonEventArgs e)
    {
        ExecuteHotKeyCommand(e);
    }

    protected internal override void OnControllerButtonUp(SDLControllerButtonEventArgs e)
    {
        ExecuteHotKeyCommand(e);
    }

    public bool ExecuteHotKeyCommand(HotKeyCommand? cmd)
    {
        if (cmd != null && cmd.Action != null)
        {
            SDLLog.Verbose(LogCategory.APPLICATION, "Executing HotKeyCommand {0}", cmd);
            cmd.Action();
            return true;
        }
        return false;
    }

    public bool ExecuteHotKeyCommand(SDLKeyEventArgs e)
    {
        return ExecuteHotKeyCommand(FindHotKeyAction(e));
    }
    public bool ExecuteHotKeyCommand(SDLControllerButtonEventArgs e)
    {
        return ExecuteHotKeyCommand(FindHotKeyAction(e));
    }

}
