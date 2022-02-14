// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SDLJoystick
{

    private readonly int which;
    private readonly IntPtr handle;
    private SDLWindow? window;
    private string? name;

    internal SDLJoystick(int which, IntPtr handle)
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


}
