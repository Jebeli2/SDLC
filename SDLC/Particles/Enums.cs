// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLC.Particles;

public enum SpriteMode
{
    Single,
    Animation,
    Random
}

public enum SpawnShape
{
    Point,
    Line,
    Square,
    Ellipse
}

public enum SpawnEllipseSide
{
    Both,
    Top,
    Bottom,
}

public enum Mode
{
    Gravity,
    Radius
}

public enum Style
{
    None,
    Fire,
    FireWork,
    Sun,
    Galaxy,
    Flower,
    Meteor,
    Spiral,
    Explosion,
    Smoke,
    Snow,
    Rain,
    Blocks,
    Max
}
