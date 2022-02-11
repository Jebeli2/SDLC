// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;

using System;

public class SDLMusicEventArgs : EventArgs
{
    private readonly SDLMusic music;
    public SDLMusicEventArgs(SDLMusic music)
    {
        this.music = music;
    }

    public SDLMusic Music => music;
}
