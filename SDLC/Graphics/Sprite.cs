// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Sprite : TextureRegion
{
    private int offsetX;
    private int offsetY;
    public Sprite(SDLTexture texture)
        : base(texture)
    {

    }

    public Sprite(SDLTexture texture, int x, int y, int w, int h, int offestX = 0, int offsetY = 0)
        : base(texture, x, y, w, h)
    {
        this.offsetX = offestX;
        this.offsetY = offsetY;
    }

    public Sprite(TextureRegion textureRegion)
        : base(textureRegion)
    {

    }

    public Sprite(Sprite sprite)
        : base(sprite)
    {

    }

    public int OffsetX
    {
        get => offsetX;
        set => offsetX = value;
    }

    public int OffsetY
    {
        get => offsetY;
        set => offsetY = value;
    }
}
