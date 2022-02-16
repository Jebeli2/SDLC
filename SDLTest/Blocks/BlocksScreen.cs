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
    private readonly BlockSettings settings = new();
    private readonly BlockGame blockGame = new();
    private readonly BlockGfx blockGfx;
    private readonly BlockRenderer blockRenderer;

    private int leftMoveDelay;
    private int rightMoveDelay;
    private int highScoresX = 10;
    private int highScoresY = 66;
    private float scoreAlpha = 0.5f;
    private float scoreDir = 1.0f;
    private float scoreSpeed = 0.02f;
    private bool showScore;
    private bool nameEntered;
    private Gadget? nameGadget;

    public BlocksScreen() : base("Blocks")
    {
        blockGfx = new BlockGfx(this);
        blockRenderer = new BlockRenderer(blockGame, blockGfx, settings);
        blockGame.Reset();
    }

    public BlockGame BlockGame => blockGame;


    public override void Initialize(IWindow window)
    {
        base.Initialize(window);
        blockRenderer.Resize(Width, Height);
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
        blockRenderer.Resize(Width, Height);
        LoadHighScores();
        _ = MakeGameScreen();
        SDLApplication.ShowCursor = false;
    }
    public override void Resized(IWindow window, int width, int height)
    {
        base.Resized(window, width, height);
        blockRenderer.Resize(Width, Height);
    }

    public override void Update(IRenderer renderer, double totalTime, double elapsedTime)
    {
        base.Update(renderer, totalTime, elapsedTime);
        blockRenderer.Update(totalTime, elapsedTime);

        if (!blockGame.Paused)
        {
            blockGame.Update(elapsedTime / 1000.0);
            UpdateSoundEffects();
            UpdateGameOver();
        }
        if (showScore) { UpdateHighScore(elapsedTime / 1000.0); }
    }

    public override void Render(IRenderer renderer, double totalTime, double elapsedTime)
    {
        base.Render(renderer, totalTime, elapsedTime);
        blockRenderer.Render(renderer);
        if (showScore) { RenderHighScores(renderer, highScoresX, highScoresY); }
    }

    public override void Hide(IWindow window)
    {
        base.Hide(window);
        SaveHighScores();
        DisposeGfx();
        SDLApplication.ShowCursor = true;
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
        _ = GadTools.CreateGadget(GadgetKind.Text, leftEdge: 10, topEdge: 5, width: -20, height: 40, text: "Pause");
        _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 10, topEdge: 60, width: -20, height: 40, text: "Resume Game", clickAction: GoToGame);
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

    private Screen MakeSettingsScreen()
    {
        IGUISystem gui = GUI;
        Screen scr = gui.OpenScreen();
        scr.Font = blockGfx.MsgFont;
        int midX = Width / 2;
        int midY = Height / 2;
        int halfW = 400 / 2;
        int halfH = 400 / 2;
        Window win = gui.OpenWindow(scr, leftEdge: midX - halfW, topEdge: midY - halfH, width: 400, height: 410,
            closing: false, sizing: false, zooming: false, depth: false, dragging: false);
        GadTools.CreateContext(gui, win);
        Gadget? useOutline = null;
        _ = GadTools.CreateGadget(GadgetKind.Text, leftEdge: 10, topEdge: 5, width: -20, height: 40, text: "Settings");
        Gadget useGrid = GadTools.CreateGadget(GadgetKind.Checkbox, leftEdge: 10, topEdge: 60, width: -20, height: 40, _checked: settings.UseGrid, text: "Show Grid");
        Gadget useGhost = GadTools.CreateGadget(GadgetKind.Checkbox, leftEdge: 10, topEdge: 110, width: -20, height: 40, _checked: settings.UseGhostPiece, text: "Show Ghost Piece",
            checkedStateChangedAction: (b) => { useOutline!.Enabled = b; });
        useOutline = GadTools.CreateGadget(GadgetKind.Checkbox, leftEdge: 10, topEdge: 160, width: -20, height: 40, _checked: settings.UseGhostOutline, disabled: !settings.UseGhostPiece, text: "Ghost Outline");
        Gadget useBg = GadTools.CreateGadget(GadgetKind.Checkbox, leftEdge: 10, topEdge: 210, width: -20, height: 40, _checked: settings.UseBackground, text: "Show Background");
        Gadget usePart = GadTools.CreateGadget(GadgetKind.Checkbox, leftEdge: 10, topEdge: 260, width: -20, height: 40, _checked: settings.UseParticles, text: "Show Particles");
        Gadget useFS = GadTools.CreateGadget(GadgetKind.Checkbox, leftEdge: 10, topEdge: 310, width: -20, height: 40, _checked: settings.Fullscreen, text: "Fullscreen");
        _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 10, topEdge: -50, width: 170, height: 40, text: "OK", clickAction: () =>
        {
            bool flag;
            if (useGrid.GetChecked(out flag)) { settings.UseGrid = flag; }
            if (useGhost.GetChecked(out flag)) { settings.UseGhostPiece = flag; }
            if (useOutline.GetChecked(out flag)) { settings.UseGhostOutline = flag; }
            if (useBg.GetChecked(out flag)) { settings.UseBackground = flag; }
            if (usePart.GetChecked(out flag)) { settings.UseParticles = flag; }
            if (useFS.GetChecked(out flag)) { settings.Fullscreen = flag; }
            IsFullScreen = settings.Fullscreen;
            OkSettings();
        });
        _ = GadTools.CreateGadget(GadgetKind.Button, leftEdge: 210, topEdge: -50, width: 170, height: 40, text: "Cancel", clickAction: CancelSettings);

        return scr;
    }

    private void ChangeGameState(GameState state)
    {
        if (state != blockGame.GameState)
        {
            blockGame.GameState = state;
            switch (state)
            {
                case GameState.Game:
                    _ = MakeGameScreen();
                    SDLApplication.ShowCursor = false;
                    break;
                case GameState.Pause:
                    _ = MakePauseScreen();
                    SDLApplication.ShowCursor = true;
                    break;
                case GameState.Settings:
                    _ = MakeSettingsScreen();
                    SDLApplication.ShowCursor = true;
                    break;
                case GameState.EnterName:
                    _ = MakeEnterNameScreen();
                    SDLApplication.ShowCursor = true;
                    break;
            }
        }
    }
    private void GoToPause()
    {
        ChangeGameState(GameState.Pause);
    }

    private void GoToGame()
    {
        ChangeGameState(GameState.Game);
    }

    private void GoToSettings()
    {
        ChangeGameState(GameState.Settings);
    }

    private void GoToEnterName()
    {
        ChangeGameState(GameState.EnterName);
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
        blockGfx.Fog = LoadTexture("fog");
        blockGfx.Blur = LoadTexture("Blur");
        blockRenderer.InitGfx();
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

        kcm.AddKeyCommand(ScanCode.SCANCODE_ESCAPE, KeyButtonState.Released, PauseButton);
        kcm.AddKeyCommand(ControllerButton.Options, KeyButtonState.Released, PauseButton);

        kcm.AddKeyCommand(ScanCode.SCANCODE_RETURN, KeyButtonState.Pressed, RestartButton);
    }

    private void PauseButton()
    {
        switch (blockGame.GameState)
        {
            case GameState.Settings: GoToPause(); break;
            case GameState.Pause: GoToGame(); break;
            case GameState.Game: GoToPause(); break;
            case GameState.EnterName: GoToGame(); break;
        }
    }

    private void RestartButton()
    {
        switch (blockGame.GameState)
        {
            case GameState.Settings: break;
            case GameState.Pause: break;
            case GameState.Game: if (blockGame.GameOver) { NewGame(); }; break;
            case GameState.EnterName: OkName(); break;
        }
    }

    private void NewGame()
    {
        Reset();
        GoToGame();
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
        }
        GoToGame();
    }

    private void OkSettings()
    {
        GoToPause();
    }

    private void CancelSettings()
    {
        GoToPause();
    }
    private void Drop()
    {
        blockGame.Drop();
    }

    private void StopDrop()
    {
        blockGame.DoneDropped();
    }
    private void RotateLeft()
    {
        blockGame.RotateLeft();
    }
    private void StopRotateLeft()
    {
        blockGame.DoneRotatedLeft();
    }

    private void RotateRight()
    {
        blockGame.RotateRight();

    }
    private void StopRotateRight()
    {
        blockGame.DoneRotatedRight();
    }
    private void MoveDown()
    {
        blockGame.MoveDown();
    }

    private void StopMoveDown()
    {
    }
    private void MoveLeft()
    {
        if (blockGame.Paused) return;
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
        if (blockGame.Paused) return;
        leftMoveDelay = 0;
    }
    private void MoveRight()
    {
        if (blockGame.Paused) return;
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
        if (blockGame.Paused) return;
        rightMoveDelay = 0;
    }

    private void Hold()
    {
        blockGame.Hold();
    }


    private void Reset()
    {
        nameEntered = false;
        showScore = false;
        blockGame.Reset();
    }

    private void UpdateSoundEffects()
    {
        if (blockGame.SoundEffect >= 0 && blockGame.SoundEffect < blockGfx.Sounds.Count)
        {
            SDLAudio.PlaySound(blockGfx.Sounds[blockGame.SoundEffect]);
            blockGame.ClearSoundEffect();
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
            scoreAlpha += scoreSpeed * (float)elapsedTime;
            if (scoreAlpha > 1.0f)
            {
                scoreAlpha = 1.0f;
                scoreDir = -1.0f;
            }
        }
        else
        {
            scoreAlpha -= scoreSpeed * (float)elapsedTime;
            if (scoreAlpha < 0.0f)
            {
                scoreAlpha = 0.0f;
                scoreDir = 1.0f;
            }
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

}
