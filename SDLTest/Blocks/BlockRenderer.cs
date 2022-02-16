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

public class BlockRenderer
{
    private readonly BlockGame blockGame;
    private readonly BlockGfx blockGfx;
    private readonly BlockSettings settings;
    private readonly List<BlockMessage> messages = new();
    private const int BOARDOFFSET = 1;

    private int bwidth;
    private int bheight;
    private int xstart;
    private int ystart;
    private int bsize;

    private int boardX;
    private int boardY;
    private int boardWidth;
    private int boardHeight;

    private int nextX;
    private int nextY;
    private int nextWidth;
    private int nextHeight;

    private int holdX;
    private int holdY;
    private int holdWidth;
    private int holdHeight;

    private int levelX;
    private int levelY;
    private int levelWidth;
    private int levelHeight;

    private int scoreX;
    private int scoreY;
    private int scoreWidth;
    private int scoreHeight;

    private int nextBestX;
    private int nextBestY;
    private int nextBestWidth;
    private int nextBestHeight;

    private Color[] lineClearColors;
    private const int TetTitleHeight = 40;
    private const int TetHeight = 22;
    private const int TetWidth = 10;

    private float spaceOffsetX;
    private float spaceOffsetY;
    private float fogOffsetX;
    private float fogOffsetY;
    private float spaceSpeedX = 0.1f;
    private float spaceSpeedY = 0.05f;
    private float fogSpeedX = 0.2f;
    private float fogSpeedY = -0.3f;
    private int maxSpaceX;
    private int maxSpaceY;
    private int maxFogX;
    private int maxFogY;

    private int highScoresX = 10;
    private int highScoresY = 66;
    private float scoreAlpha = 0.5f;
    private float scoreDir = 1.0f;
    private float scoreSpeed = 0.02f;

    private Color gridColor;
    private Color borderColor;
    private Color innerBorderColor;
    private Color shadowColor;
    private Color tetColor;
    private Color tetBorderColor;
    private Color textColor;
    private Color lightTextColor;
    private Color varTextColor;

    private int linesToClear;
    private string linesToClearText = "";
    private int currentLevel;
    private string currentLevelText = "";
    private int points;
    private string pointsText = "";
    private int nextBestPoints;
    private string nextBestPointsText = "";

    public BlockRenderer(BlockGame blockGame, BlockGfx blockGfx, BlockSettings settings)
    {
        this.blockGame = blockGame;
        this.blockGfx = blockGfx;
        this.settings = settings;
        gridColor = Color.FromArgb(64, Color.DarkGray);
        borderColor = Color.FromArgb(64, Color.White);
        shadowColor = Color.FromArgb(64, Color.Black);
        innerBorderColor = Color.FromArgb(96, Color.White);
        tetColor = Color.FromArgb(128, Color.DarkBlue);
        tetBorderColor = Color.FromArgb(204, Color.LightBlue);
        lightTextColor = Color.FromArgb(204, Color.AliceBlue);
        varTextColor = Color.FromArgb(229, Color.White);
        textColor = Color.White;
        lineClearColors = BuildLineClearColors(Color.Black, blockGame.LineClearDelay);
        linesToClear = -1;
        currentLevel = -1;
        points = -1;
        nextBestPoints = -1;
    }

    public void InitGfx()
    {
        if (blockGfx.Space != null)
        {
            blockGfx.Space.AlphaMod = 132;
            maxSpaceX = blockGfx.Space.Width;
            maxSpaceY = blockGfx.Space.Height;
        }
        if (blockGfx.Fog != null)
        {
            blockGfx.Fog.AlphaMod = 144;
            maxFogX = blockGfx.Fog.Width;
            maxFogY = blockGfx.Fog.Height;
        }
        if (blockGfx.Blur != null)
        {
            blockGfx.Blur.AlphaMod = 229;
        }
    }

    public void Resize(int width, int height)
    {
        bsize = CalcBlockSize(width, height);
    }
    public void Update(double totalTime, double elapsedTime)
    {
        UpdateBackground(elapsedTime);
        if (!blockGame.Paused)
        {
            UpdateMessages(elapsedTime / 1000.0);
            UpdateTexts();
        }
    }

    private void UpdateTexts()
    {
        if (linesToClear != blockGame.LinesToClear)
        {
            linesToClear = blockGame.LinesToClear;
            linesToClearText = linesToClear.ToString();
        }
        if (currentLevel != blockGame.CurrentLevel)
        {
            currentLevel = blockGame.CurrentLevel;
            currentLevelText = currentLevel.ToString();
        }
        if (points != blockGame.Points)
        {
            points = blockGame.Points;
            pointsText = points.ToString();
        }
        int np = blockGame.NextBest?.Points ?? -1;
        if (np != nextBestPoints)
        {
            nextBestPoints = np;
            if (nextBestPoints > 0)
            {
                nextBestPointsText = nextBestPoints.ToString();
            }
            else
            {
                nextBestPointsText = "";
            }
        }
    }
    private void UpdateBackground(double deltaTime)
    {
        if (settings.UseBackground)
        {
            float dT = (float)deltaTime;
            spaceOffsetX += spaceSpeedX * dT;
            spaceOffsetY += spaceSpeedY * dT;
            fogOffsetX += fogSpeedX * dT;
            fogOffsetY += fogSpeedY * dT;
            if (spaceOffsetX <= -maxSpaceX) { spaceOffsetX = 0; }
            else if (spaceOffsetX > maxSpaceX) { spaceOffsetX = 0; }
            if (spaceOffsetY <= -maxSpaceY) { spaceOffsetY = 0; }
            else if (spaceOffsetY > maxSpaceY) { spaceOffsetY = 0; }
            if (fogOffsetX <= -maxFogX) { fogOffsetX = 0; }
            else if (fogOffsetX > maxFogX) { fogOffsetX = 0; }
            if (fogOffsetY <= -maxFogY) { fogOffsetY = 0; }
            else if (fogOffsetY > maxFogY) { fogOffsetY = 0; }
        }
    }

    private void UpdateMessages(double deltaTime)
    {
        BlockMessage? msg = blockGame.GetNewMessage();
        if (msg != null)
        {
            msg.Y = (msg.StartRow - BOARDOFFSET) * bsize + ystart;
            msg.Duration = msg.InitialDuration;
            messages.Add(msg);
        }
        int pos = 0;
        while (pos < messages.Count)
        {
            msg = messages[pos];
            msg.Duration -= (float)deltaTime;
            if (msg.Duration <= 0)
            {
                messages.RemoveAt(pos);
            }
            else
            {
                msg.Y -= msg.Speed * (float)deltaTime;
                pos++;
            }
        }
    }

    public void Render(IRenderer gfx)
    {
        gfx.BlendMode = BlendMode.Blend;
        RenderBackground(gfx);
        RenderBoard(gfx, blockGame.Board);
        RenderPiece(gfx, blockGame.CurrentPiece);
        RenderGhostPiece(gfx, blockGame.GhostPiece);
        RenderNextPiece(gfx, blockGame.NextPiece);
        RenderHoldPiece(gfx, blockGame.HoldPiece);
        RenderLevel(gfx);
        RenderScore(gfx);
        RenderNextBest(gfx);
        RenderMessages(gfx);
        RenderGameOver(gfx);
        RenderPause(gfx);
    }

    private void RenderBackground(IRenderer gfx)
    {
        if (settings.UseBackground)
        {
            gfx.DrawTextureFill(blockGfx.Space, (int)spaceOffsetX, (int)spaceOffsetY);
            gfx.DrawTextureFill(blockGfx.Fog, (int)fogOffsetX, (int)fogOffsetY);
        }
    }

    private void RenderLevel(IRenderer gfx)
    {
        var font = blockGfx.SmallFont;
        DrawTetrion(gfx, levelX, levelY, levelWidth, levelHeight, TetTitleHeight, 3, 3, 3, "LEVEL");
        gfx.DrawText(blockGfx.BigFont, currentLevelText, levelX + 10, levelY + 10, textColor);
        gfx.DrawText(font, "LINES TO CLEAR", levelX + 3, levelY + levelHeight - 22 * 2, lightTextColor);
        var ltcSize = gfx.MeasureText(font, linesToClearText);
        gfx.DrawText(font, linesToClearText, levelX + levelWidth - 3 - ltcSize.Width, levelY + levelHeight - 22 * 2, varTextColor);
    }

    private void RenderScore(IRenderer gfx)
    {
        DrawTetrion(gfx, scoreX, scoreY, scoreWidth, scoreHeight, TetTitleHeight, 3, 3, 3, "SCORE");
        gfx.DrawText(blockGfx.BigFont, pointsText, scoreX + 10, scoreY + 10, textColor);
    }

    private void RenderNextBest(IRenderer gfx)
    {
        DrawTetrion(gfx, nextBestX, nextBestY, nextBestWidth, nextBestHeight, TetTitleHeight, 3, 3, 3, "NEXT BEST");
        BlockScore nextBest = blockGame.NextBest;
        if (nextBest != null)
        {
            var font = blockGfx.SmallFont;
            gfx.DrawText(font, nextBest.Name, nextBestX + 3, nextBestY + 22, lightTextColor);
            gfx.DrawText(font, nextBestPointsText, nextBestX + 10, nextBestY + nextBestHeight - 22, varTextColor);
        }
    }

    private void RenderGameOver(IRenderer gfx)
    {
        if (blockGame.GameOver)
        {
            var font = blockGfx.SoupFontBig;
            var goSize = gfx.MeasureText(font, "GAME OVER");
            int goPosX = boardX + boardWidth / 2 - goSize.Width / 2;
            int goPosY = boardY + boardHeight / 2 - goSize.Height / 2;
            gfx.DrawText(font, "GAME OVER", goPosX + 1, goPosY + 1, Color.Black);
            gfx.DrawText(font, "GAME OVER", goPosX - 1, goPosY - 1, Color.Black);
            gfx.DrawText(font, "GAME OVER", goPosX, goPosY, Color.White);
        }
    }
    private void RenderPause(IRenderer gfx)
    {
        if (blockGame.Paused)
        {
            gfx.DrawTextureFill(blockGfx.Blur);
        }
    }

    private void RenderMessages(IRenderer gfx)
    {
        foreach (var msg in messages)
        {
            RenderMessage(gfx, msg);
        }
    }

    private void RenderMessage(IRenderer gfx, BlockMessage msg)
    {
        int txtPosX = boardX;
        int txtPosY = (int)msg.Y;
        int txtWidth = boardWidth;
        int txtHeight = 32;
        gfx.DrawText(blockGfx.MsgFont, msg.Text, txtPosX + 1, txtPosY + 1, txtWidth, txtHeight, Color.FromArgb(msg.Alpha, Color.Black));
        gfx.DrawText(blockGfx.MsgFont, msg.Text, txtPosX - 1, txtPosY - 1, txtWidth, txtHeight, Color.FromArgb(msg.Alpha, Color.Black));
        gfx.DrawText(blockGfx.MsgFont, msg.Text, txtPosX, txtPosY, txtWidth, txtHeight, Color.FromArgb(msg.Alpha, Color.White));
    }

    private void RenderBoard(IRenderer gfx, BlockBoard board)
    {
        bool useGrid = settings.UseGrid;
        int py = ystart;
        for (int y = 0; y < bheight; y++)
        {
            int px = xstart;
            for (int x = 0; x < bwidth; x++)
            {
                if (useGrid)
                {

                    gfx.DrawRect(px, py, bsize, bsize, gridColor);
                }
                Block block = board[x, y + BOARDOFFSET];
                if (block != null)
                {
                    RenderBlock(gfx, block, px, py, bsize);
                }
                px += bsize;
            }
            if (board.IsFullRow(y + BOARDOFFSET))
            {
                RenderLineClear(gfx, xstart, py, bsize * bwidth, bsize, blockGame.CurrentLineClear);
            }
            py += bsize;
        }
        DrawTetrion(gfx, boardX, boardY, boardWidth, boardHeight, TetTitleHeight, TetHeight, TetWidth, TetWidth);
    }

    private void RenderPiece(IRenderer gfx, BlockPiece? piece)
    {
        if (piece == null) return;
        int sx = xstart + piece.X * bsize;
        int sy = ystart + piece.Y * bsize;
        foreach (var block in piece.Blocks)
        {
            int px = sx + bsize * block.X;
            int py = sy + bsize * (block.Y - BOARDOFFSET);
            RenderBlock(gfx, block, px, py, bsize);
        }
    }

    private void RenderGhostPiece(IRenderer gfx, BlockPiece? ghost)
    {
        if (ghost == null) return;
        if (!settings.UseGhostPiece) return;
        bool useGhostOutline = settings.UseGhostOutline;
        int ghostAlpha = (int)(settings.GhostAlpha * 255);
        int sx = xstart + ghost.X * bsize;
        int sy = ystart + ghost.Y * bsize;
        foreach (var block in ghost.Blocks)
        {
            int px = sx + bsize * block.X;
            int py = sy + bsize * (block.Y - BOARDOFFSET);
            if (useGhostOutline)
            {
                RenderGhostOutline(gfx, block, px, py, bsize, ghostAlpha);
            }
            else
            {
                RenderGhostBlock(gfx, block, px, py, bsize, ghostAlpha);
            }
        }
    }
    private void RenderGhostOutline(IRenderer gfx, Block block, int x, int y, int size, int ghostAlpha)
    {
        if (y < ystart) return;
        gfx.DrawRect(x + 1, y + 1, size - 2, size - 2, Color.FromArgb(ghostAlpha, block.Color));
        gfx.DrawLine(x, y, x + size - 1, y, Color.FromArgb(ghostAlpha, borderColor));
        gfx.DrawLine(x, y + 1, x, y + size - 1, Color.FromArgb(ghostAlpha, borderColor));
        gfx.DrawLine(x, y + size - 1, x + size - 1, y + size - 1, Color.FromArgb(ghostAlpha, shadowColor));
        gfx.DrawLine(x + size - 1, y + 1, x + size - 1, y + size - 1, Color.FromArgb(ghostAlpha, shadowColor));
        gfx.DrawRect(x + 2, y + 2, size - 4, size - 4, Color.FromArgb(ghostAlpha, innerBorderColor));
    }
    private void RenderGhostBlock(IRenderer gfx, Block block, int x, int y, int size, int ghostAlpha)
    {
        if (y < ystart) return;
        OldRenderGhostBlock(gfx, block, x, y, size, ghostAlpha);
    }

    private void OldRenderGhostBlock(IRenderer gfx, Block block, int x, int y, int size, int ghostAlpha)
    {
        if (y < ystart) return;
        gfx.FillRect(x, y, size, size, Color.FromArgb(ghostAlpha, block.Color));
        gfx.DrawLine(x, y, x + size - 1, y, Color.FromArgb(ghostAlpha, borderColor));
        gfx.DrawLine(x, y + 1, x, y + size - 1, Color.FromArgb(ghostAlpha, borderColor));
        gfx.DrawLine(x, y + size - 1, x + size - 1, y + size - 1, Color.FromArgb(ghostAlpha, shadowColor));
        gfx.DrawLine(x + size - 1, y + 1, x + size - 1, y + size - 1, Color.FromArgb(ghostAlpha, shadowColor));
        gfx.DrawRect(x + 1, y + 1, size - 2, size - 2, Color.FromArgb(ghostAlpha, innerBorderColor));
    }
    private void RenderNextPiece(IRenderer gfx, BlockPiece? piece)
    {
        RenderNHPiece(gfx, piece, nextX, nextY, nextWidth, nextHeight, "NEXT");
    }

    private void RenderHoldPiece(IRenderer gfx, BlockPiece? piece)
    {
        RenderNHPiece(gfx, piece, holdX, holdY, holdWidth, holdHeight, "HOLD");
    }

    private void RenderNHPiece(IRenderer gfx, BlockPiece? piece, int x, int y, int width, int height, string title)
    {
        if (piece != null)
        {
            int sx = x + (width - piece.Width * bsize) / 2;
            int sy = y + (height - piece.Height * bsize) / 2 + bsize / 2;
            foreach (var block in piece.Blocks)
            {
                int px = sx + bsize * block.X;
                int py = sy + bsize * block.Y;
                RenderBlock(gfx, block, px, py, bsize);
            }
        }
        DrawTetrion(gfx, x, y, width, height, TetTitleHeight, 3, 3, 3, title);
    }


    private void RenderBlock(IRenderer gfx, Block block, int x, int y, int size)
    {
        if (y < ystart) return;
        OldRenderBlock(gfx, block, x, y, size);
    }

    private void OldRenderBlock(IRenderer gfx, Block block, int x, int y, int size)
    {
        if (y < ystart) return;
        Color o = block.Color;
        Color d1 = o.ChangBrightness(-0.3f);
        Color b1 = o.ChangBrightness(0.2f);
        Color b2 = o.ChangBrightness(0.4f);

        gfx.FillColorRect(x, y, size, size, b1, o, b2, d1);
        gfx.DrawLine(x, y, x + size - 1, y, borderColor);
        gfx.DrawLine(x, y + 1, x, y + size - 1, borderColor);
        gfx.DrawLine(x, y + size - 1, x + size - 1, y + size - 1, shadowColor);
        gfx.DrawLine(x + size - 1, y + 1, x + size - 1, y + size - 1, shadowColor);
        gfx.DrawRect(x + 1, y + 1, size - 2, size - 2, innerBorderColor);
    }
    private void VeryOldRenderBlock(IRenderer gfx, Block block, int x, int y, int size)
    {
        if (y < ystart) return;
        gfx.FillRect(x, y, size, size, block.Color);
        gfx.DrawLine(x, y, x + size - 1, y, borderColor);
        gfx.DrawLine(x, y + 1, x, y + size - 1, borderColor);
        gfx.DrawLine(x, y + size - 1, x + size - 1, y + size - 1, shadowColor);
        gfx.DrawLine(x + size - 1, y + 1, x + size - 1, y + size - 1, shadowColor);
        gfx.DrawRect(x + 1, y + 1, size - 2, size - 2, innerBorderColor);
    }

    private void RenderLineClear(IRenderer gfx, int x, int y, int width, int height, int lineClear)
    {
        Color c = lineClearColors[lineClear];
        gfx.FillRect(x, y, width, height, c);
    }

    private void DrawTetrion(IRenderer gfx, int x, int y, int width, int height, int top, int bottom, int left, int right, string title = "")
    {
        DrawTetrionRect(gfx, new Rectangle(x - left, y - top, width + left + right, top)); // top row
        DrawTetrionRect(gfx, new Rectangle(x - left, y + height, width + left + right, bottom)); // bottom row
        DrawTetrionRect(gfx, new Rectangle(x - left, y - top, left, height + top + bottom)); // left col
        DrawTetrionRect(gfx, new Rectangle(x + width, y - top, right, height + top + bottom)); // right col
        if (!string.IsNullOrEmpty(title))
        {
            int textPosX = x + 4;
            int textPosY = y - top + 2;
            gfx.DrawText(blockGfx.BigFont, title, textPosX, textPosY, varTextColor);
        }
    }

    private void DrawTetrionRect(IRenderer gfx, Rectangle rect)
    {
        Color o = tetColor;
        Color d1 = o.ChangBrightness(-0.3f);
        Color b1 = o.ChangBrightness(0.2f);
        Color b2 = o.ChangBrightness(0.4f);

        gfx.FillColorRect(rect, b1, o, b2, d1);
        gfx.DrawRect(rect.X, rect.Y, rect.Width, rect.Height, tetBorderColor);
    }

    private int CalcBlockSize(int swidth, int sheight)
    {
        bwidth = blockGame.BoardWidth;
        bheight = blockGame.BoardHeight - BOARDOFFSET;
        bsize = (sheight - TetHeight * 2) / (bheight + 2);
        xstart = swidth / 2 - bwidth * bsize / 2;
        ystart = sheight / 2 - bheight * bsize / 2;
        boardX = xstart;
        boardY = ystart;
        boardWidth = bwidth * bsize;
        boardHeight = bheight * bsize;

        nextX = xstart + boardWidth + bsize;
        nextY = ystart;
        nextWidth = 6 * bsize;
        nextHeight = 3 * bsize;

        holdX = xstart - 7 * bsize;
        holdY = ystart;
        holdWidth = 6 * bsize;
        holdHeight = 3 * bsize;

        levelX = holdX;
        levelWidth = holdWidth;
        levelHeight = 6 * bsize;
        levelY = ystart + boardHeight - levelHeight + TetHeight - 3;

        scoreX = nextX;
        scoreWidth = nextWidth;
        scoreHeight = 2 * bsize;
        scoreY = ystart + boardHeight - levelHeight + TetHeight - 3;

        nextBestX = scoreX;
        nextBestWidth = scoreWidth;
        nextBestHeight = 2 * bsize;
        nextBestY = ystart + boardHeight - nextBestHeight + TetHeight - 3; ;
        return bsize;
    }

    private static Color[] BuildLineClearColors(Color baseColor, int steps)
    {
        List<Color> colors = new List<Color>();
        for (int i = 0; i <= steps; i++)
        {
            int alpha = GetAlphaForLineClear(i, steps);
            Color c = Color.FromArgb(alpha, baseColor);
            colors.Add(c);
        }
        colors[steps - 1] = Color.FromArgb(229, Color.White);
        colors[steps - 2] = Color.FromArgb(229, Color.White);
        return colors.ToArray();
    }

    private static int GetAlphaForLineClear(int lineClear, int steps)
    {
        int max = steps;
        int min = 0;
        float a = 0.0f;
        float b = 1.0f;
        int x = steps - lineClear;
        float fx = ((b - a) * (x - min) / (max - min)) + a;
        return (int)(fx * 255.0);
    }

}
