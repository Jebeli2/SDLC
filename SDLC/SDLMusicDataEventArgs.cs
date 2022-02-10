namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
}
