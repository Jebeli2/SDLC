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


}
