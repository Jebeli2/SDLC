namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class SDLExtensions
    {
        public static bool GetTextureHandle(this SDLTexture? texture, out IntPtr handle)
        {
            if (texture != null)
            {
                handle = texture.Handle;
            }
            else
            {
                handle = IntPtr.Zero;
            }
            return handle != IntPtr.Zero;
        }

        public static T[] AsArray<T>(this IEnumerable<T> src)
        {
            if (src is T[] array) { return array; }            
            return src.ToArray();
        }

    }
}
