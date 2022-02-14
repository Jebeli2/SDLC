// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLTest.Blocks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDLC;

public class BlockGfx
{
    private readonly BlocksScreen screen;

    private int scoreX = 10;
    private int scoreY = 66;
    private float scoreAlpha = 0.5f;
    private float scoreDir = 1.0f;
    private float scoreSpeed = 0.02f;
    public BlockGfx(BlocksScreen screen)
    {
        this.screen = screen;
    }

    public void LoadContent()
    {
        //soundEffects.Add(cm.GetSound("collided"));
        //soundEffects.Add(cm.GetSound("deleteLine"));
        //soundEffects.Add(cm.GetSound("glassbell"));
        //soundEffects.Add(cm.GetSound("swish-11"));
        //soundEffects.Add(cm.GetSound("KL Peach Game Over III"));
    }

    public SDLFont? BigFont { get; set; }
    public SDLFont? MsgFont { get; set; }
    public SDLFont? SmallFont { get; set; }
    public SDLFont? SoupFontBig { get; set; }
    public SDLTexture? Space { get; set; }
    public SDLTexture? Fog { get; set; }
    public SDLTexture? Blur { get; set; }

    public void Update(double totalTime, double elapsedTime)
    {
        //if (game.Settings.UseBackground)
        //{
        //    spaceP.Update(gameTime);
        //    fogP.Update(gameTime);
        //}
        //if (game.ShowScore)
        //{
        //    UpdateHighScore(gameTime);
        //}
    }

    private void UpdateHighScore(double elapsedTime)
    {
        //float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (scoreDir > 0.0f)
        {
            scoreAlpha += scoreSpeed;
            if (scoreAlpha > 1.0f)
            {
                scoreAlpha = 1.0f;
                scoreDir = -1.0f;
            }
        }
        else
        {
            scoreAlpha -= scoreSpeed;
            if (scoreAlpha < 0.0f)
            {
                scoreAlpha = 0.0f;
                scoreDir = 1.0f;
            }
        }
    }

    private void RenderHighScores(IRenderer gfx, int x, int y)
    {
        Point pos = new Point(x, y);
        gfx.DrawText(SmallFont, "#", pos.X, pos.Y, Color.LightBlue);
        pos.X += 30;
        gfx.DrawText(SmallFont, "Player", pos.X, pos.Y, Color.White);
        pos.X += 140;
        var scSize = gfx.MeasureText(SmallFont, "Score");
        gfx.DrawText(SmallFont, "Score", pos.X - scSize.Width, pos.Y, Color.White);
        pos.X += 100;
        var liSize = gfx.MeasureText(SmallFont, "Lines");
        gfx.DrawText(SmallFont, "Lines", pos.X - liSize.Width, pos.Y, Color.White);
        pos.X += 70;
        var leSize = gfx.MeasureText(SmallFont, "Level");
        gfx.DrawText(SmallFont, "Level", pos.X - leSize.Width, pos.Y, Color.White);
        gfx.DrawLine(x, y + 20, pos.X, y + 20, Color.LightBlue);
        int count = 0;
        foreach (var hs in screen.BlockGame.HighScores)
        {
            y += 20;
            if (hs.IsCurrent)
            {
                gfx.DrawRect(x - 5, y, 30 + 140 + 100 + 70 + 10, 20, Color.FromArgb((int)(scoreAlpha * 255), Color.LightBlue));
            }

            pos = new Point(x, y);
            gfx.DrawText(SmallFont, (count + 1).ToString(), pos.X, pos.Y, Color.LightBlue);
            pos.X += 30;
            gfx.DrawText(SmallFont, hs.Name, pos.X, pos.Y, Color.White);
            pos.X += 140;
            string ps = hs.Points.ToString();
            scSize = gfx.MeasureText(SmallFont, ps);
            gfx.DrawText(SmallFont, ps, pos.X - scSize.Width, pos.Y, Color.White);
            pos.X += 100;
            string li = hs.Lines.ToString();
            liSize = gfx.MeasureText(SmallFont, li);
            gfx.DrawText(SmallFont, li, pos.X - liSize.Width, pos.Y, Color.White);
            pos.X += 70;
            string le = hs.Level.ToString();
            leSize = gfx.MeasureText(SmallFont, le);
            gfx.DrawText(SmallFont, le, pos.X - leSize.Width, pos.Y, Color.White);
            count++;
        }
    }
}
