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
using SDLC.Applets;
using SDLC.GUI;

public class BlocksScreen : SDLScreen
{
    private const string HSNAME = "blocks_highscores";
    private const int BOARDOFFSET = 1;
    private readonly BlockGame blockGame = new();
    private readonly BlockGfx blockGfx;
    private readonly List<BlockMessage> messages = new();
    private BlockSettings settings = new();
    private Color gridColor;
    private Color borderColor;
    private Color innerBorderColor;
    private Color shadowColor;
    private Color tetColor;
    private Color tetBorderColor;
    private Color textColor;
    private Color lightTextColor;
    private Color varTextColor;

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
    private const int tetTitleHeight = 40;
    private const int tetHeight = 22;
    private const int tetWidth = 10;

    private bool inSettingsMenu;
    private bool paused;
    private bool enteringName;
    private bool nameEntered;

    private int leftMoveDelay;
    private int rightMoveDelay;
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
    private bool showScore;

    private Screen? gameScreen;
    private Screen? pauseScreen;
    private Screen? enterNameScreen;
    private Gadget? nameGadget;

    public BlocksScreen() : base("Blocks")
    {
        blockGfx = new BlockGfx(this);
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
        blockGame.Reset();
    }

    public BlockGame BlockGame => blockGame;


    public override void Initialize(IWindow window)
    {
        base.Initialize(window);
        bsize = CalcBlockSize(Width, Height);
    }

    public override void Show(IWindow window)
    {
        base.Show(window);
        GetApplet<LinesApp>().Enabled = false;
        GetApplet<RainingBoxesApp>().Enabled = false;
        GetApplet<MusicVisualizer>().Enabled = false;
        GetApplet<BackgroundImage>().Enabled = false;
        GetApplet<MusicPlayer>().PlayNow(nameof(Properties.Resources.Korobeiniki));
        InitGfx();
        InitCommands(GetApplet<KeyCommandManager>());
        gameScreen = MakeGameScreen();
        bsize = CalcBlockSize(Width, Height);
        LoadHighScores();
    }
    public override void Resized(IWindow window, int width, int height)
    {
        base.Resized(window, width, height);
        bsize = CalcBlockSize(Width, Height);
    }

    public override void Update(IRenderer renderer, double totalTime, double elapsedTime)
    {
        base.Update(renderer, totalTime, elapsedTime);
        if (showScore)
        {
            UpdateHighScore(elapsedTime);
        }
        if (!paused)
        {
            blockGame.Update(elapsedTime / 1000.0);
            UpdateBackground(elapsedTime);
            UpdateSoundEffects();
            UpdateMessages(elapsedTime / 1000.0);
            UpdateGameOver();

        }
        else
        {

        }
    }

    public override void Render(IRenderer renderer, double totalTime, double elapsedTime)
    {
        base.Render(renderer, totalTime, elapsedTime);
        renderer.BlendMode = BlendMode.Blend;
        RenderBackground(renderer);
        if (showScore) { RenderHighScores(renderer, highScoresX, highScoresY); }
        RenderBoard(renderer, blockGame.Board);
        RenderPiece(renderer, blockGame.CurrentPiece);
        RenderGhostPiece(renderer, blockGame.GhostPiece);
        RenderNextPiece(renderer, blockGame.NextPiece);
        RenderHoldPiece(renderer, blockGame.HoldPiece);
        RenderMessages(renderer);
        RenderLevel(renderer);
        RenderScore(renderer);
        RenderNextBest(renderer);
        RenderGameOver(renderer);
        RenderPause(renderer);
    }

    public override void Hide(IWindow window)
    {
        base.Hide(window);
        SaveHighScores();
        DisposeGfx();
    }
    private void LoadHighScores()
    {
        blockGame.SetHighScores(LoadData(HSNAME));
    }

    private void SaveHighScores()
    {
        byte[] hs = blockGame.GetHighScores();
        SaveData(HSNAME, hs);
    }

    private Screen MakePauseScreen()
    {
        IGUISystem gui = GUI;
        Screen scr = gui.OpenScreen();
        scr.Font = blockGfx.MsgFont;
        int midX = Width / 2;
        int midY = Height / 2;
        int halfW = 400 / 2;
        int halfH = 400 / 2;
        Window win = gui.OpenWindow(scr, leftEdge: midX - halfW, topEdge: midY - halfH, width: 400, height: 260,
            closing: false, sizing: false, zooming: false, depth: false, dragging: false);
        GadTools.CreateContext(gui, win);
        _ = GadTools.CreateGadget(GadgetKind.Text, leftEdge: 10, topEdge: 10, width: -20, height: 40, text: "Pause");
        _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 10, topEdge: 60, width: -20, height: 40, text: "Resume Game", clickAction: ResumeGame);
        _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 10, topEdge: 110, width: -20, height: 40, text: "New Game", clickAction: NewGame);
        _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 10, topEdge: 160, width: -20, height: 40, text: "Settings", clickAction: GoToSettings);
        _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 10, topEdge: 210, width: -20, height: 40, text: "Exit Game", clickAction: ExitGame);
        return scr;
    }

    private Screen MakeEnterNameScreen()
    {
        IGUISystem gui = GUI;
        Screen scr = gui.OpenScreen();
        scr.Font = blockGfx.MsgFont;
        int midX = Width / 2;
        int midY = Height / 2;
        int halfW = 400 / 2;
        int halfH = 400 / 2;
        Window win = gui.OpenWindow(scr, leftEdge: midX - halfW, topEdge: midY - halfH, width: 400, height: 260,
            closing: false, sizing: false, zooming: false, depth: false, dragging: false);
        GadTools.CreateContext(gui, win);
        _ = GadTools.CreateGadget(GadgetKind.Text, leftEdge: 10, topEdge: 10, width: -20, height: 40, text: "Enter Your Name:");
        nameGadget = GadTools.CreateGadget(GadgetKind.String, leftEdge: 10, topEdge: 60, width: -20, height: 40, buffer: blockGame.CurrentName, clickAction: OkName);
        gui.ActivateGadget(nameGadget);
        return scr;
    }

    private Screen MakeGameScreen()
    {
        IGUISystem gui = GUI;
        Screen scr = gui.OpenScreen();
        return scr;
    }
    private void GoToPause()
    {
        paused = true;
        inSettingsMenu = false;
        enteringName = false;
        pauseScreen = MakePauseScreen();
    }

    private void GoToGame()
    {
        paused = false;
        inSettingsMenu = false;
        enteringName = false;
        gameScreen = MakeGameScreen();
    }

    private void GoToSettings()
    {
        paused = true;
        inSettingsMenu = true;
        enteringName = false;
        if (pauseScreen != null) { GUI.CloseScreen(pauseScreen); }
    }

    private void GoToEnterName()
    {
        paused = true;
        inSettingsMenu = false;
        enteringName = true;
        enterNameScreen = MakeEnterNameScreen();
    }

    private void InitGfx()
    {
        blockGfx.Sounds.Add(LoadSound("collided"));
        blockGfx.Sounds.Add(LoadSound("deleteLine"));
        blockGfx.Sounds.Add(LoadSound("glassbell"));
        blockGfx.Sounds.Add(LoadSound("swish-11"));
        blockGfx.Sounds.Add(LoadSound("KL Peach Game Over III"));
        blockGfx.BigFont = LoadFont("Rubik-Regular", 32);
        blockGfx.MsgFont = LoadFont("Rubik-Regular", 38);
        blockGfx.SmallFont = LoadFont("Rubik-Regular", 16);
        blockGfx.SoupFontBig = LoadFont("Rubik-Regular", 60);
        blockGfx.Space = LoadTexture("space");
        if (blockGfx.Space != null)
        {
            blockGfx.Space.AlphaMod = 132;
            maxSpaceX = blockGfx.Space.Width;
            maxSpaceY = blockGfx.Space.Height;
        }
        blockGfx.Fog = LoadTexture("fog");
        if (blockGfx.Fog != null)
        {
            blockGfx.Fog.AlphaMod = 144;
            maxFogX = blockGfx.Fog.Width;
            maxFogY = blockGfx.Fog.Height;
        }
        blockGfx.Blur = LoadTexture("Blur");
        if (blockGfx.Blur != null)
        {
            blockGfx.Blur.AlphaMod = 229;
        }
    }

    private void DisposeGfx()
    {
        foreach (SDLSound? sound in blockGfx.Sounds)
        {
            sound?.Dispose();
        }
        blockGfx.Sounds.Clear();
        blockGfx.Blur?.Dispose();
        blockGfx.Fog?.Dispose();
        blockGfx.Space?.Dispose();
        blockGfx.BigFont?.Dispose();
        blockGfx.MsgFont?.Dispose();
        blockGfx.SmallFont?.Dispose();
        blockGfx.SoupFontBig?.Dispose();
    }
    private void InitCommands(KeyCommandManager kcm)
    {
        kcm.Clear();
        kcm.AddKeyCommand(ScanCode.SCANCODE_LEFT, KeyButtonState.Pressed, MoveLeft);
        kcm.AddKeyCommand(ScanCode.SCANCODE_KP_4, KeyButtonState.Pressed, MoveLeft);
        kcm.AddKeyCommand(ControllerButton.DPadLeft, KeyButtonState.Pressed, MoveLeft);

        kcm.AddKeyCommand(ScanCode.SCANCODE_RIGHT, KeyButtonState.Pressed, MoveRight);
        kcm.AddKeyCommand(ScanCode.SCANCODE_KP_6, KeyButtonState.Pressed, MoveRight);
        kcm.AddKeyCommand(ControllerButton.DpadRight, KeyButtonState.Pressed, MoveRight);

        kcm.AddKeyCommand(ScanCode.SCANCODE_DOWN, KeyButtonState.Pressed, MoveDown);
        kcm.AddKeyCommand(ScanCode.SCANCODE_KP_2, KeyButtonState.Pressed, MoveDown);
        kcm.AddKeyCommand(ControllerButton.DPadDown, KeyButtonState.Pressed, MoveDown);

        kcm.AddKeyCommand(ScanCode.SCANCODE_UP, KeyButtonState.Pressed, RotateLeft);
        kcm.AddKeyCommand(ScanCode.SCANCODE_KP_1, KeyButtonState.Pressed, RotateLeft);
        kcm.AddKeyCommand(ScanCode.SCANCODE_KP_5, KeyButtonState.Pressed, RotateLeft);
        kcm.AddKeyCommand(ScanCode.SCANCODE_KP_9, KeyButtonState.Pressed, RotateLeft);
        kcm.AddKeyCommand(ScanCode.SCANCODE_X, KeyButtonState.Pressed, RotateLeft);
        kcm.AddKeyCommand(ControllerButton.Circle, KeyButtonState.Pressed, RotateLeft);

        kcm.AddKeyCommand(ScanCode.SCANCODE_KP_3, KeyButtonState.Pressed, RotateRight);
        kcm.AddKeyCommand(ScanCode.SCANCODE_KP_7, KeyButtonState.Pressed, RotateRight);
        kcm.AddKeyCommand(ScanCode.SCANCODE_Z, KeyButtonState.Pressed, RotateRight);
        kcm.AddKeyCommand(ScanCode.SCANCODE_LCTRL, KeyButtonState.Pressed, RotateRight);
        kcm.AddKeyCommand(ScanCode.SCANCODE_RCTRL, KeyButtonState.Pressed, RotateRight);
        kcm.AddKeyCommand(ControllerButton.Cross, KeyButtonState.Pressed, RotateRight);

        kcm.AddKeyCommand(ScanCode.SCANCODE_SPACE, KeyButtonState.Pressed, Drop);
        kcm.AddKeyCommand(ScanCode.SCANCODE_KP_8, KeyButtonState.Pressed, Drop);
        kcm.AddKeyCommand(ControllerButton.DPadUp, KeyButtonState.Pressed, Drop);
        kcm.AddKeyCommand(ControllerButton.Triangle, KeyButtonState.Pressed, Drop);


        kcm.AddKeyCommand(ScanCode.SCANCODE_LEFT, KeyButtonState.Released, StopMoveLeft);
        kcm.AddKeyCommand(ScanCode.SCANCODE_KP_4, KeyButtonState.Released, StopMoveLeft);
        kcm.AddKeyCommand(ControllerButton.DPadLeft, KeyButtonState.Released, StopMoveLeft);

        kcm.AddKeyCommand(ScanCode.SCANCODE_RIGHT, KeyButtonState.Released, StopMoveRight);
        kcm.AddKeyCommand(ScanCode.SCANCODE_KP_6, KeyButtonState.Released, StopMoveRight);
        kcm.AddKeyCommand(ControllerButton.DpadRight, KeyButtonState.Released, StopMoveRight);

        kcm.AddKeyCommand(ScanCode.SCANCODE_DOWN, KeyButtonState.Released, StopMoveDown);
        kcm.AddKeyCommand(ScanCode.SCANCODE_KP_2, KeyButtonState.Released, StopMoveDown);
        kcm.AddKeyCommand(ControllerButton.DPadDown, KeyButtonState.Released, StopMoveDown);

        kcm.AddKeyCommand(ScanCode.SCANCODE_UP, KeyButtonState.Released, StopRotateLeft);
        kcm.AddKeyCommand(ScanCode.SCANCODE_KP_1, KeyButtonState.Released, StopRotateLeft);
        kcm.AddKeyCommand(ScanCode.SCANCODE_KP_5, KeyButtonState.Released, StopRotateLeft);
        kcm.AddKeyCommand(ScanCode.SCANCODE_KP_9, KeyButtonState.Released, StopRotateLeft);
        kcm.AddKeyCommand(ScanCode.SCANCODE_X, KeyButtonState.Released, StopRotateLeft);
        kcm.AddKeyCommand(ControllerButton.Circle, KeyButtonState.Released, StopRotateLeft);

        kcm.AddKeyCommand(ScanCode.SCANCODE_KP_3, KeyButtonState.Released, StopRotateRight);
        kcm.AddKeyCommand(ScanCode.SCANCODE_KP_7, KeyButtonState.Released, StopRotateRight);
        kcm.AddKeyCommand(ScanCode.SCANCODE_Z, KeyButtonState.Released, StopRotateRight);
        kcm.AddKeyCommand(ScanCode.SCANCODE_LCTRL, KeyButtonState.Released, StopRotateRight);
        kcm.AddKeyCommand(ScanCode.SCANCODE_RCTRL, KeyButtonState.Released, StopRotateRight);
        kcm.AddKeyCommand(ControllerButton.Cross, KeyButtonState.Released, StopRotateRight);

        kcm.AddKeyCommand(ScanCode.SCANCODE_SPACE, KeyButtonState.Released, StopDrop);
        kcm.AddKeyCommand(ScanCode.SCANCODE_KP_8, KeyButtonState.Released, StopDrop);
        kcm.AddKeyCommand(ControllerButton.DPadUp, KeyButtonState.Released, StopDrop);
        kcm.AddKeyCommand(ControllerButton.Triangle, KeyButtonState.Released, StopDrop);

        kcm.AddKeyCommand(ScanCode.SCANCODE_KP_0, KeyButtonState.Released, Hold);
        kcm.AddKeyCommand(ScanCode.SCANCODE_C, KeyButtonState.Released, Hold);
        kcm.AddKeyCommand(ControllerButton.LeftShoulder, KeyButtonState.Released, Hold);
        kcm.AddKeyCommand(ControllerButton.RightShoulder, KeyButtonState.Released, Hold);

        kcm.AddKeyCommand(ScanCode.SCANCODE_ESCAPE, KeyButtonState.Released, Pause);
        kcm.AddKeyCommand(ControllerButton.Options, KeyButtonState.Released, Pause);

        kcm.AddKeyCommand(ScanCode.SCANCODE_RETURN, KeyButtonState.Pressed, RestartGame);
    }

    private void Pause()
    {
        if (inSettingsMenu)
        {
            GoToPause();
        }
        else if (paused)
        {
            GoToGame();
        }
        else
        {
            GoToPause();
        }
    }

    private void RestartGame()
    {
        if (paused) return;
        if (enteringName)
        {
            OkName();
            return;
        }
        if (!blockGame.GameOver) return;
        NewGame();
    }


    private void ResumeGame()
    {
        GoToGame();
    }

    private void NewGame()
    {
        Reset();
        ResumeGame();
    }

    private void ExitGame()
    {
        ChangeScreen(new TestScreen());
    }

    private void OkName()
    {
        nameEntered = true;
        if (nameGadget.GetBuffer(out string name))
        {
            blockGame.SetCurrentName(name);
            blockGame.ResetHighScores();
        }
        ResumeGame();
    }
    private void Drop()
    {
        if (paused) return;
        blockGame.Drop();
    }

    private void StopDrop()
    {
        if (paused) return;
        blockGame.DoneDropped();
    }
    private void RotateLeft()
    {
        if (paused) return;
        blockGame.RotateLeft();
    }
    private void StopRotateLeft()
    {
        if (paused) return;
        blockGame.DoneRotatedLeft();
    }

    private void RotateRight()
    {
        if (paused) return;
        blockGame.RotateRight();

    }
    private void StopRotateRight()
    {
        if (paused) return;
        blockGame.DoneRotatedRight();
    }
    private void MoveDown()
    {
        if (paused) return;
        blockGame.MoveDown();
    }

    private void StopMoveDown()
    {
        if (paused) return;
    }
    private void MoveLeft()
    {
        if (paused) return;
        if (leftMoveDelay <= 0)
        {
            blockGame.MoveLeft();
            leftMoveDelay = 2;
        }
        else
        {
            leftMoveDelay--;
        }
    }
    private void StopMoveLeft()
    {
        if (paused) return;
        leftMoveDelay = 0;
    }
    private void MoveRight()
    {
        if (paused) return;
        if (rightMoveDelay <= 0)
        {
            blockGame.MoveRight();
            rightMoveDelay = 2;
        }
        else
        {
            rightMoveDelay--;
        }
    }
    private void StopMoveRight()
    {
        if (paused) return;
        rightMoveDelay = 0;
    }

    private void Hold()
    {
        if (paused) return;
        blockGame.Hold();
    }


    private void Reset()
    {
        nameEntered = false;
        showScore = false;
        blockGame.Reset();
        lineClearColors = BuildLineClearColors(Color.Black, blockGame.LineClearDelay);
    }
    public int CalcBlockSize()
    {
        return CalcBlockSize(Width, Height);
    }

    public int CalcBlockSize(int swidth, int sheight)
    {
        bwidth = blockGame.BoardWidth;
        bheight = blockGame.BoardHeight - BOARDOFFSET;
        bsize = (sheight - tetHeight * 2) / (bheight + 2);
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
        levelY = ystart + boardHeight - levelHeight + tetHeight - 3;

        scoreX = nextX;
        scoreWidth = nextWidth;
        scoreHeight = 2 * bsize;
        scoreY = ystart + boardHeight - levelHeight + tetHeight - 3;

        nextBestX = scoreX;
        nextBestWidth = scoreWidth;
        nextBestHeight = 2 * bsize;
        nextBestY = ystart + boardHeight - nextBestHeight + tetHeight - 3; ;
        return bsize;
    }

    private void UpdateBackground(double deltaTime)
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
    private void UpdateSoundEffects()
    {
        if (blockGame.SoundEffect >= 0 && blockGame.SoundEffect < blockGfx.Sounds.Count)
        {
            SDLSound? snd = blockGfx.Sounds[blockGame.SoundEffect];
            if (snd != null)
            {
                SDLAudio.PlaySound(snd);
            }
            blockGame.ClearSoundEffect();
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

    private void UpdateGameOver()
    {
        if (blockGame.GameOver)
        {
            showScore = true;
            if (!nameEntered)
            {
                GoToEnterName();
            }
        }
    }

    private void UpdateHighScore(double elapsedTime)
    {
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

    private void RenderLevel(IRenderer gfx)
    {
        var font = blockGfx.SmallFont;
        DrawTetrion(gfx, levelX, levelY, levelWidth, levelHeight, tetTitleHeight, 3, 3, 3, "LEVEL");
        gfx.DrawText(blockGfx.BigFont, blockGame.LevelStr, levelX + 10, levelY + 10, textColor);
        gfx.DrawText(font, "LINES TO CLEAR", levelX + 3, levelY + levelHeight - 22 * 2, lightTextColor);
        string ltcTxt = blockGame.LinesToClearStr;
        var ltcSize = gfx.MeasureText(font, ltcTxt);
        gfx.DrawText(font, ltcTxt, levelX + levelWidth - 3 - ltcSize.Width, levelY + levelHeight - 22 * 2, varTextColor);
    }

    private void RenderScore(IRenderer gfx)
    {
        DrawTetrion(gfx, scoreX, scoreY, scoreWidth, scoreHeight, tetTitleHeight, 3, 3, 3, "SCORE");
        gfx.DrawText(blockGfx.BigFont, blockGame.PointsStr, scoreX + 10, scoreY + 10, textColor);
    }

    private void RenderNextBest(IRenderer gfx)
    {
        DrawTetrion(gfx, nextBestX, nextBestY, nextBestWidth, nextBestHeight, tetTitleHeight, 3, 3, 3, "NEXT BEST");
        BlockScore nextBest = blockGame.NextBest;
        if (nextBest != null)
        {
            var font = blockGfx.SmallFont;
            gfx.DrawText(font, nextBest.Name, nextBestX + 3, nextBestY + 22, lightTextColor);
            if (nextBest.Points > 0)
            {
                gfx.DrawText(font, nextBest.Points.ToString(), nextBestX + 10, nextBestY + nextBestHeight - 22, varTextColor);
            }
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
        if (paused)
        {
            gfx.DrawTextureFill(blockGfx.Blur);
        }
    }

    private void RenderHighScores(IRenderer gfx, int x, int y)
    {
        SDLFont? smallFont = blockGfx.SmallFont;
        Point pos = new Point(x, y);
        gfx.DrawText(smallFont, "#", pos.X, pos.Y, Color.LightBlue);
        pos.X += 30;
        gfx.DrawText(smallFont, "Player", pos.X, pos.Y, Color.White);
        pos.X += 140;
        var scSize = gfx.MeasureText(smallFont, "Score");
        gfx.DrawText(smallFont, "Score", pos.X - scSize.Width, pos.Y, Color.White);
        pos.X += 100;
        var liSize = gfx.MeasureText(smallFont, "Lines");
        gfx.DrawText(smallFont, "Lines", pos.X - liSize.Width, pos.Y, Color.White);
        pos.X += 70;
        var leSize = gfx.MeasureText(smallFont, "Level");
        gfx.DrawText(smallFont, "Level", pos.X - leSize.Width, pos.Y, Color.White);
        gfx.DrawLine(x, y + 20, pos.X, y + 20, Color.LightBlue);
        int count = 0;
        foreach (var hs in blockGame.HighScores)
        {
            y += 20;
            if (hs.IsCurrent)
            {
                gfx.DrawRect(x - 5, y, 30 + 140 + 100 + 70 + 10, 20, Color.FromArgb((int)(scoreAlpha * 255), Color.LightBlue));
            }

            pos = new Point(x, y);
            gfx.DrawText(smallFont, (count + 1).ToString(), pos.X, pos.Y, Color.LightBlue);
            pos.X += 30;
            gfx.DrawText(smallFont, hs.Name, pos.X, pos.Y, Color.White);
            pos.X += 140;
            string ps = hs.Points.ToString();
            scSize = gfx.MeasureText(smallFont, ps);
            gfx.DrawText(smallFont, ps, pos.X - scSize.Width, pos.Y, Color.White);
            pos.X += 100;
            string li = hs.Lines.ToString();
            liSize = gfx.MeasureText(smallFont, li);
            gfx.DrawText(smallFont, li, pos.X - liSize.Width, pos.Y, Color.White);
            pos.X += 70;
            string le = hs.Level.ToString();
            leSize = gfx.MeasureText(smallFont, le);
            gfx.DrawText(smallFont, le, pos.X - leSize.Width, pos.Y, Color.White);
            count++;
        }
    }

    private void RenderBackground(IRenderer gfx)
    {
        if (settings.UseBackground)
        {
            gfx.DrawTextureFill(blockGfx.Space, (int)spaceOffsetX, (int)spaceOffsetY);
            gfx.DrawTextureFill(blockGfx.Fog, (int)fogOffsetX, (int)fogOffsetY);
        }
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
        DrawTetrion(gfx, boardX, boardY, boardWidth, boardHeight, tetTitleHeight, tetHeight, tetWidth, tetWidth);
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
        DrawTetrion(gfx, x, y, width, height, tetTitleHeight, 3, 3, 3, title);
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
        //gfx.FillRect(rect.X, rect.Y, rect.Width, rect.Height, tetColor);
        gfx.DrawRect(rect.X, rect.Y, rect.Width, rect.Height, tetBorderColor);
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
        //int x = lineClear;
        float fx = ((b - a) * (x - min) / (max - min)) + a;
        return (int)(fx * 255.0);
    }

}
