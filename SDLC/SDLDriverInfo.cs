// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
using System.Collections.Generic;

public class SDLDriverInfo
{
    public string Name { get; set; } = "";
    public uint Flags { get; set; }
    public IList<uint> TextureFormats { get; set; } = new List<uint>();
    public IList<string> TextureFormatNames { get; set; } = new List<string>();
    public int MaxTextureWidth { get; set; }
    public int MaxTextureHeight { get; set; }

    public override string ToString()
    {
        return $"{Name} ({MaxTextureWidth}x{MaxTextureHeight} max texture size)";
    }

}
