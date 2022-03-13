// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Gfx
{
    public static void DrawSprite(this IRenderer renderer, Sprite sprite, int x, int y)
    {
        Rectangle src = new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Height);
        Rectangle dst = new Rectangle(x - sprite.OffsetX, y - sprite.OffsetY, sprite.Width, sprite.Height);
        renderer.DrawTexture(sprite.Texture, src, dst);
    }

}
