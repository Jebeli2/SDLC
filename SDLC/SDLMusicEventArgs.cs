namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLMusicEventArgs : EventArgs
    {
        private readonly SDLMusic music;
        public SDLMusicEventArgs(SDLMusic music)
        {
            this.music = music;
        }

        public SDLMusic Music => music;
    }


}
