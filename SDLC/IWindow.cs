﻿// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;

using System;

public interface IWindow
{
    int WindowId { get; }
    IntPtr Handle { get; }
    bool HandleCreated { get; }
    IRenderer Renderer { get; }
    IContentManager ContentManager { get; }
    IScreen Screen { get; set; }
    int Width { get; }
    int Height { get; }
    bool FullScreen { get; set; }

    T GetApplet<T>() where T : SDLApplet, new();
    void AddApplet(SDLApplet applet);
    void RemoveApplet(SDLApplet applet);
    void ChangeApplet(SDLApplet applet);
}
