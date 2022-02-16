// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLTest.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Tetronimo
{
    I,
    J,
    L,
    S,
    T,
    Z,
    O
}

public enum Rotation
{
    R0,
    R90,
    R180,
    R270
}

public enum GameState
{
    Game,
    Pause,
    Settings,
    EnterName
}
