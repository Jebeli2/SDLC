// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.Applets;
public class BackgroundImage : SDLApplet
{
    private SDLTexture? image;
    public BackgroundImage() : base("Background Image")
    {
        RenderPrio = -1000;
        noInput = true;
    }

    public SDLTexture? Image
    {
        get => image;
        set
        {
            if (image != value)
            {
                image?.Dispose();
                image = value;
            }
        }
    }

    protected override void OnWindowPaint(SDLWindowPaintEventArgs e)
    {
        e.Renderer.DrawTexture(image);
    }

    protected override void OnDispose()
    {
        image?.Dispose();
    }
}
