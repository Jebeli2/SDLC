// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.GUI;
public interface IGUIRenderer
{
    bool ShowDebugBounds { get; set; }
    void RenderScreen(IRenderer gfx, Screen screen, int offsetX, int offsetY);
    void RenderWindow(IRenderer gfx, Window window, int offsetX, int offsetY);
    void RenderGadget(IRenderer gfx, Gadget gadget, int offsetX, int offsetY);
    void RenderRequester(IRenderer gfx, Requester req, int offsetX, int offsetY);
}
