// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;

using System;

public class SDLWindowPaintEventArgs : EventArgs
{
    private readonly IRenderer renderer;
    private double totalTime;
    private double elapsedTime;
    public SDLWindowPaintEventArgs(IRenderer renderer, double totalTime, double elapsedTime)
    {
        this.renderer = renderer;
        this.totalTime = totalTime;
        this.elapsedTime = elapsedTime;
    }
    public IRenderer Renderer
    {
        get => renderer;
    }
    public double TotalTime => totalTime;
    public double ElapsedTime => elapsedTime;
    internal void Update(double totalTime, double elapsedTime)
    {
        this.totalTime = totalTime;
        this.elapsedTime = elapsedTime;
    }

}

public delegate void SDLWindowPaintEventHandler(object sender, SDLWindowPaintEventArgs e);
