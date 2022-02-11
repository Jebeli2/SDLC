namespace SDLC.Applets
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MusicVisualizer : SDLApplet
    {
        private int lines = 400;
        private short[] audioData = Array.Empty<short>();
        private bool musicPaused;
        private readonly Color leftColor1 = Color.FromArgb(128, Color.AliceBlue);
        private readonly Color rightColor1 = Color.FromArgb(128, Color.Azure);
        private readonly Color leftColor2 = Color.FromArgb(128, Color.Red);
        private readonly Color rightColor2 = Color.FromArgb(128, Color.Orange);

        public MusicVisualizer() : base("Music Visualizer")
        {

        }

        protected override void OnWindowLoad(SDLWindowLoadEventArgs e)
        {
            lines = (Width / 2) & (0xFFFF - 1);
            SDLAudio.MusicDataReceived += SDLAudio_MusicDataReceived;
        }

        protected internal override void OnWindowResized(SDLWindowSizeEventArgs e)
        {
            lines = (Width / 2) & (0xFFFF - 1);
        }


        protected override void OnDispose()
        {
            SDLAudio.MusicDataReceived -= SDLAudio_MusicDataReceived;
        }

        protected override void OnWindowPaint(SDLWindowPaintEventArgs e)
        {
            PaintMusicArray(e.Renderer, audioData, lines, true, true);
        }
        private void SDLAudio_MusicDataReceived(object? sender, SDLMusicDataEventArgs e)
        {
            audioData = e.Data;
        }

        private void PaintMusicArray(IRenderer gfx, short[] data, int lines, bool drawLeft, bool drawRight)
        {
            if (data.Length > 0)
            {
                int w = gfx.Width;
                int h = gfx.Height;
                int len = data.Length;

                int cut = len / lines;
                float offsetX = 10;
                float dW = w - offsetX * 2;
                float lineWidth = dW / lines;
                float midY = h / 2.0f;
                float facY = h / 3.0f / 32000.0f;
                float endW = w - 2 * offsetX;
                if (drawLeft)
                {
                    float pX = offsetX;
                    int index = 0;
                    while (pX < endW && index < data.Length)
                    {
                        float sample = data[index];
                        PaintSample(gfx, pX, midY, lineWidth, sample * facY, leftColor1, leftColor2);
                        pX += lineWidth;
                        index += cut;
                    }
                }
                if (drawRight)
                {
                    float pX = offsetX + lineWidth / 2;
                    int index = 1;
                    while (pX < endW && index < data.Length)
                    {
                        float sample = data[index];
                        PaintSample(gfx, pX, midY, lineWidth, sample * facY, rightColor1, rightColor2);
                        pX += lineWidth;
                        index += cut;
                    }
                }
            }
        }

        private void PaintSample(IRenderer gfx, float xPos, float yMid, float lineWidth, float sample, Color mid, Color peak)
        {
            RectangleF dst = new RectangleF(xPos, yMid, lineWidth, sample);
            if (sample < 0)
            {
                dst.Y += sample;
                dst.Height *= -1;
                gfx.FillVertGradient(dst, peak, mid);
            }
            else
            {
                gfx.FillVertGradient(dst, mid, peak);
            }
        }

    }
}
