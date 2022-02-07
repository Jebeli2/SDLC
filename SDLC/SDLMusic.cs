namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLMusic : SDLObject
    {
        private readonly string? tempFile;
        private readonly MusicType musicType;
        private bool disposedValue;

        internal SDLMusic(IntPtr handle, string name, string? tempFile = null)
            : base(handle, name)
        {
            this.tempFile = tempFile;
            musicType = SDLAudio.Mix_GetMusicType(this.handle);
            SDLAudio.Track(this);
        }
        public MusicType MusicType => musicType;

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    SDLAudio.Untrack(this);
                }
                if (handle != IntPtr.Zero)
                {
                    SDLAudio.Mix_FreeMusic(handle);
                }
                if (tempFile != null)
                {
                    File.Delete(tempFile);
                }
                disposedValue = true;
            }
            base.Dispose(disposing);
        }
    }
}
