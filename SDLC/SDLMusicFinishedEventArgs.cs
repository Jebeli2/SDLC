// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
public class SDLMusicFinishedEventArgs : SDLMusicEventArgs
{
    private readonly MusicFinishReason reason;
    public SDLMusicFinishedEventArgs(SDLMusic music, MusicFinishReason reason)
        : base(music)
    {
        this.reason = reason;
    }

    public MusicFinishReason Reason => reason;
}

public delegate void SDLMusicFinishedEventHandler(object? sender, SDLMusicFinishedEventArgs e);
