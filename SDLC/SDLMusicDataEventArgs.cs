// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
public class SDLMusicDataEventArgs : SDLMusicEventArgs
{
    private readonly short[] data;
    public SDLMusicDataEventArgs(SDLMusic music, short[] data)
        : base(music)
    {
        this.data = data;
    }

    public short[] Data => data;
}

public delegate void SDLMusicDataEventHandler(object? sender, SDLMusicDataEventArgs e);
