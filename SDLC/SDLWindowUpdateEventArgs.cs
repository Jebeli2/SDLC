// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;

using System;

public class SDLWindowUpdateEventArgs : EventArgs
{
    private double totalTime;
    private double elapsedTime;

    public SDLWindowUpdateEventArgs(double totalTime, double elapsedTime)
    {
        this.totalTime = totalTime;
        this.elapsedTime = elapsedTime;
    }

    public double TotalTime => totalTime;
    public double ElapsedTime => elapsedTime;

    internal void Update(double totalTime, double elapsedTime)
    {
        this.totalTime = totalTime;
        this.elapsedTime = elapsedTime;
    }
}

public delegate void SDLWindowUpdateEventHandler(object sender, SDLWindowUpdateEventArgs e);
