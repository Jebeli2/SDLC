// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TextureRegion
{
    private SDLTexture texture;
    private int x;
    private int y;
    private int width;
    private int height;

    public TextureRegion(SDLTexture texture)
    {
        this.texture = texture;
        x = 0;
        y = 0;
        width = this.texture.Width;
        height = this.texture.Height;
    }

    public TextureRegion(TextureRegion textureRegion)
    {
        texture = textureRegion.texture;
        x = textureRegion.x;
        y = textureRegion.y;
        width = textureRegion.width;
        height = textureRegion.height;
    }

    public TextureRegion(SDLTexture texture, int x, int y, int w, int h)
    {
        this.texture = texture;
        this.x = x;
        this.y = y;
        width = w;
        height = h;
    }

    public SDLTexture Texture
    {
        get { return texture; }
        set { texture = value; }
    }

    public int X
    {
        get => x;
        set => x = value;
    }

    public int Y
    {
        get => y;
        set => y = value;
    }

    public int Width
    {
        get => width;
        set => width = value;
    }

    public int Height
    {
        get => height;
        set => height = value;
    }

    public Rectangle Bounds => new(x, y, width, height);

    public void Set(TextureRegion textureRegion)
    {
        texture = textureRegion.texture;
        x = textureRegion.x;
        y = textureRegion.y;
        width = textureRegion.width;
        height = textureRegion.height;
    }

    public void Render(IRenderer renderer, Rectangle dst)
    {
        renderer.DrawTexture(texture, Bounds, dst);
    }

    public void Render(IRenderer renderer, float x, float y)
    {
        renderer.DrawTexture(texture, Bounds, new RectangleF(x, y, width, height));
    }

}
