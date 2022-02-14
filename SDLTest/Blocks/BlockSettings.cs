// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLTest.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BlockSettings
{
    public bool UseGrid { get; set; }
    public bool UseGhostPiece { get; set; }
    public bool UseGhostOutline { get; set; }
    public float GhostAlpha { get; set; }
    public bool UseParticles { get; set; }
    public bool UseBackground { get; set; }
    public bool Fullscreen { get; set; }

    public BlockSettings()
    {
        UseGrid = true;
        UseGhostPiece = true;
        UseGhostOutline = true;
        GhostAlpha = 0.3f;
        UseParticles = true;
        UseBackground = true;
        Fullscreen = false;
    }

    public BlockSettings(BlockSettings other)
    {
        UseGrid = other.UseGrid;
        UseGhostPiece = other.UseGhostPiece;
        UseGhostOutline = other.UseGhostOutline;
        GhostAlpha = other.GhostAlpha;
        UseParticles = other.UseParticles;
        UseBackground = other.UseBackground;
        Fullscreen = other.Fullscreen;
    }

}
