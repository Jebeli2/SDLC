namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
}
