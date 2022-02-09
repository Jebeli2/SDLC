namespace SDLC.Applets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BackgroundImage : SDLApplet
    {
        private SDLTexture? image;
        public BackgroundImage() : base("Background Image")
        {
            RenderPrio = -1000;
            noInput = true;
        }

        public SDLTexture? Image
        {
            get => image;
            set => image = value;
        }

        protected override void OnWindowPaint(SDLWindowPaintEventArgs e)
        {
            e.Renderer.DrawTexture(image);
        }

    }
}
