// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.Applets;

using System;
using System.Drawing;

/// <summary>
/// C# Port of Kartik Thakore's Perl Code from here: "https://www.perl.com/pub/2011/01/visualizing-music-with-sdl-and-perl.html/"
/// Slightly changed during porting.
/// </summary>
public class MusicVisualizer : SDLApplet
{
    private short[] audioData = Array.Empty<short>();
    private readonly Color leftColor1 = Color.FromArgb(156, 40, 0, 255);
    private readonly Color leftColor2 = Color.FromArgb(10, Color.BlanchedAlmond);
    private readonly Color rightColor1 = Color.FromArgb(96, 255, 40, 128);
    private readonly Color rightColor2 = Color.FromArgb(10, Color.BlanchedAlmond);

    public MusicVisualizer() : base("Music Visualizer")
    {

    }

    protected override void OnWindowLoad(SDLWindowLoadEventArgs e)
    {
        SDLAudio.MusicDataReceived += SDLAudio_MusicDataReceived;
    }



    protected override void OnDispose()
    {
        SDLAudio.MusicDataReceived -= SDLAudio_MusicDataReceived;
    }

    protected override void OnWindowPaint(SDLWindowPaintEventArgs e)
    {
        int lines = (Width / 4) & (0xFFFF - 1);
        PaintMusicArrayI(e.Renderer, audioData, lines);
    }
    private void SDLAudio_MusicDataReceived(object? sender, SDLMusicDataEventArgs e)
    {
        audioData = e.Data;
    }

    private void PaintMusicArrayI(IRenderer gfx, short[] data, int lines)
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
            float drawWidth = lineWidth * 1.2f;
            float midY = h / 2.0f;
            float facY = h / 3.0f / 32000.0f;
            float endW = w - 2 * offsetX;
            float pX = offsetX;
            int index = 0;
            while (pX < endW && index < data.Length - 1)
            {
                float left = data[index];
                float right = data[index + 1];
                PaintSample(gfx, pX, midY, drawWidth, left * facY, leftColor1, leftColor2);
                PaintSample(gfx, pX + drawWidth / 2, midY, drawWidth, right * facY, rightColor1, rightColor2);
                pX += lineWidth;
                index += cut;
            }
        }
    }
    private void PaintMusicArray(IRenderer gfx, short[] data, int lines, bool drawLeft, bool drawRight)
    {
        if (data.Length > 0)
        {
            int countNonZero = 0;
            int w = gfx.Width;
            int h = gfx.Height;
            int len = data.Length;

            int cut = len / lines;
            float offsetX = 10;
            float dW = w - offsetX * 2;
            float lineWidth = dW / lines;
            float drawWidth = lineWidth * 1.2f;
            float midY = h / 2.0f;
            float facY = h / 3.0f / 32000.0f;
            float endW = w - 2 * offsetX;
            if (drawLeft)
            {
                float pX = offsetX;
                int index = 0;
                while (pX < endW && index < data.Length)
                {
                    if (data[index] != 0)
                    {
                        countNonZero++;
                        float sample = data[index];
                        PaintSample(gfx, pX, midY, drawWidth, sample * facY, leftColor1, leftColor2);
                    }
                    pX += lineWidth;
                    index += cut;
                }
            }
            if (drawRight)
            {
                float pX = offsetX + drawWidth;
                int index = 1;
                while (pX < endW && index < data.Length)
                {
                    if (data[index] != 0)
                    {
                        countNonZero++;
                        float sample = data[index];
                        PaintSample(gfx, pX, midY, drawWidth, sample * facY, rightColor1, rightColor2);
                    }
                    pX += lineWidth;
                    index += cut;
                }
            }
            if (countNonZero == 0)
            {
                // With MIDI Files and no TIMIDITY you get sound + the PostMix callback, but it's
                // always full of zeroes (on Windows...)
                //SDLLog.Debug(LogCategory.AUDIO, "All Music is zero...");
            }
        }
    }

    private static void PaintSample(IRenderer gfx, float xPos, float yMid, float lineWidth, float sample, Color mid, Color peak)
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
