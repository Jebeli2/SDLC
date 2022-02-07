namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLObject : IDisposable
    {
        protected readonly IntPtr handle;
        private readonly string name;
        private bool disposedValue;

        protected SDLObject(IntPtr handle, string name)
        {
            this.handle = handle;
            this.name = name;
        }

        public IntPtr Handle => handle;
        public string Name => name;



        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        ~SDLObject()
        {

            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public override string ToString()
        {
            return name;
        }

    }
}
