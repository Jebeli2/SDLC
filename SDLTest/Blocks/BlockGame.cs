// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLTest.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BlockGame
{
    public const int SOUND_COLLIDED = 0;
    public const int SOUND_LINE_DELETED = 1;
    public const int SOUND_LEVEL_UP = 2;
    public const int SOUND_HOLD = 3;
    public const int SOUND_GAME_OVER = 4;


    private const int BOARDWIDTH = 10;
    private const int BOARDHEIGHT = 23;

    private bool freshStart;
    private bool blocksFlag;
    private BlockBoard board = new(BOARDWIDTH, BOARDHEIGHT);
    private BlockPiece? currentPiece;
    private BlockPiece? nextPiece;
    private BlockPiece? holdPiece;
    private BlockPiece? ghostPiece;
    private double currentSpeed;
    private double currentTime;
    private bool gameOver;
    private int points;
    private int rowsPerLevel = 10;
    private int totalRowsCleared;
    private int currentLevel;
    private int levelRowsCleared;
    private int lineClearDelay = 42;
    private int currentLineClear;
    private int keyDAS = 100;
    private int lockDelay = 30;
    private int dropLockDelay = 2;
    private int currentLockDelay;

    private bool dropped;
    private bool rotatedLeft;
    private bool rotatedRight;
    private int soundEffect;
    private readonly List<BlockMessage> messages = new();
    private readonly List<BlockScore> highScores = new();
    private BlockScore currentScore = new();
    private string currentName = "";
    private BlockScore nextBestScore = new();


    public BlockGame()
    {
        Reset();
    }

    public int BoardWidth => board.Width;
    public int BoardHeight => board.Height;
    public int KeyDAS => keyDAS;
    public int LineClearDelay => lineClearDelay;
    public int CurrentLineClear => currentLineClear;
    public string PointsStr => points.ToString();
    public string LevelStr => currentLevel.ToString();
    public string LinesToClearStr => (rowsPerLevel - levelRowsCleared).ToString();
    public int SoundEffect => soundEffect;
    public bool GameOver => gameOver;
    public BlockBoard Board => board;
    public BlockPiece? CurrentPiece => currentPiece;
    public BlockPiece? NextPiece => nextPiece;
    public BlockPiece? HoldPiece => holdPiece;
    public BlockScore NextBest => nextBestScore;
    public IList<BlockScore> HighScores => highScores;
    public BlockMessage? GetNewMessage()
    {
        BlockMessage? msg = null;
        if (messages.Count > 0)
        {
            msg = messages[0];
            messages.RemoveAt(0);
        }
        return msg;
    }

    public string CurrentName => currentName;

    public void SetCurrentName(string name)
    {
        if (name == null) name = "";
        name = name.Trim();
        currentName = name;
        if (currentScore != null)
        {
            currentScore.Name = currentName;
        }
    }

    public void SetHighScores(byte[] data)
    {
        highScores.Clear();
        MemoryStream ms = new MemoryStream(data, false);
        BinaryReader br = new BinaryReader(ms);
        int count = br.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            var hs = ReadHighScore(br);
            highScores.Add(hs);
        }
        br.Close();
        ms.Close();
        highScores.Sort();
        FindNextBest();
    }

    public byte[] GetHighScores()
    {
        MemoryStream ms = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write(highScores.Count);
        foreach (var hs in highScores)
        {
            WriteHighscore(bw, hs);
        }
        bw.Close();
        ms.Close();
        byte[] data = ms.GetBuffer();
        return data;
    }

    private static BlockScore ReadHighScore(BinaryReader br)
    {
        string name = br.ReadString();
        int points = br.ReadInt32();
        int lines = br.ReadInt32();
        int level = br.ReadInt32();
        return new BlockScore
        {
            Name = name,
            Lines = lines,
            Points = points,
            Level = level,
        };
    }

    private static void WriteHighscore(BinaryWriter bw, BlockScore score)
    {
        bw.Write(score.Name);
        bw.Write(score.Points);
        bw.Write(score.Lines);
        bw.Write(score.Level);
    }

    public BlockPiece? GhostPiece
    {
        get
        {
            if (ghostPiece == null)
            {
                if (currentPiece != null)
                {
                    ghostPiece = new BlockPiece(currentPiece);
                    ghostPiece.Drop(board);
                }
            }
            return ghostPiece;
        }
    }

    public void Update(double deltaTime)
    {
        CheckPieces();
        MovePieces(deltaTime);
        CheckLineClear();
    }

    public void Reset()
    {
        board = new BlockBoard(BOARDWIDTH, BOARDHEIGHT);
        freshStart = true;
        BlockPiece.ClearBag();
        blocksFlag = false;
        soundEffect = -1;
        currentPiece = null;
        nextPiece = null;
        holdPiece = null;
        ghostPiece = null;
        currentTime = 0;
        gameOver = false;
        totalRowsCleared = 0;
        currentLevel = 1;
        levelRowsCleared = 0;
        currentSpeed = GetSpeed(currentLevel);
        currentLineClear = 0;
        currentLockDelay = 0;
        points = 0;
        ResetHighScores();
    }

    public void ResetHighScores()
    {
        foreach (var hs in highScores) hs.IsCurrent = false;
        FindNextBest();
    }

    public void ClearSoundEffect()
    {
        soundEffect = -1;
    }

    public void SetSoundEffect(int value)
    {
        soundEffect = value;
    }

    public bool MoveLeft()
    {
        if (currentPiece != null && !dropped)
        {
            if (currentPiece.MoveLeft(board))
            {
                ResetPiece(currentPiece);
                return true;
            }
        }
        return false;
    }

    public bool MoveRight()
    {
        if (currentPiece != null && !dropped)
        {
            if (currentPiece.MoveRight(board))
            {
                ResetPiece(currentPiece);
                return true;
            }
        }
        return false;
    }

    public bool MoveDown()
    {
        if (currentPiece != null && !dropped)
        {
            if (currentPiece.MoveDown(board))
            {
                currentPiece.Points += 1;
                ResetPiece(currentPiece);
                return true;
            }
        }
        return false;
    }

    public bool Drop()
    {
        if (currentPiece != null && !dropped)
        {
            currentPiece.Drop(board);
            CheckPieceLanding(true);
            dropped = true;
            return true;
        }
        return false;
    }

    public void DoneDropped()
    {
        dropped = false;
    }


    public bool RotateLeft()
    {
        if (currentPiece != null && !dropped && !rotatedLeft)
        {
            if (currentPiece.RotateLeft(board))
            {
                ResetPiece(currentPiece);
                rotatedLeft = true;
                return true;
            }
        }
        return false;
    }

    public void DoneRotatedLeft()
    {
        rotatedLeft = false;
    }

    public bool RotateRight()
    {
        if (currentPiece != null && !dropped && !rotatedRight)
        {
            if (currentPiece.RotateRight(board))
            {
                ResetPiece(currentPiece);
                rotatedRight = true;
                return true;
            }
        }
        return false;
    }

    public void DoneRotatedRight()
    {
        rotatedRight = false;
    }
    public bool Hold()
    {
        bool ok = false;
        if (currentPiece != null)
        {
            if (holdPiece == null || !holdPiece.Hold)
            {
                BlockPiece? oldPiece = holdPiece;
                holdPiece = currentPiece;
                holdPiece.RotateToStart();
                holdPiece.Hold = true;
                currentPiece = oldPiece;
                if (currentPiece != null)
                {
                    ok = currentPiece.MoveToStartPosition(board);
                }
                else
                {
                    ok = true;
                }
                if (ok)
                {
                    ghostPiece = null;
                    SetSoundEffect(SOUND_HOLD);
                }
            }
        }
        return ok;
    }

    private void ResetPiece(BlockPiece piece)
    {
        currentTime = 0;
        currentLineClear = 0;
        currentLockDelay = 0;
        ghostPiece = null;
    }

    private void CheckPieces()
    {
        if (gameOver) return;
        if (nextPiece == null)
        {
            nextPiece = BlockPiece.GetNextPiece(freshStart);
            freshStart = false;
        }
        if (currentPiece == null && currentLineClear <= 0)
        {
            currentPiece = nextPiece;
            nextPiece = null;
            if (currentPiece != null)
            {
                if (!currentPiece.MoveToStartPosition(board))
                {
                    MakeGameOver();
                }
            }
        }
    }

    private void MakeGameOver()
    {
        CopyPieceToBoard(true);
        gameOver = true;
        FindNextBest();
        currentScore = new BlockScore()
        {
            Name = "",
            Points = points,
            Lines = totalRowsCleared,
            Level = currentLevel,
            IsCurrent = true

        };
        highScores.Add(currentScore);
        highScores.Sort();
        SetSoundEffect(SOUND_GAME_OVER);
    }

    private void MovePieces(double deltaTime)
    {
        if (gameOver) return;
        currentTime += deltaTime;
        if (currentTime > currentSpeed && currentLockDelay <= 0)
        {
            CheckPieceLanding();
            currentTime = 0;
        }
        CheckPieceLock();
    }

    private void CheckPieceLanding(bool fromDrop = false)
    {
        if (currentPiece != null)
        {
            if (!currentPiece.MoveDown(board))
            {
                currentLockDelay = fromDrop ? dropLockDelay : lockDelay;
            }
        }
    }

    private void CheckPieceLock()
    {
        if (currentLockDelay > 0)
        {
            currentLockDelay--;
            if (currentLockDelay <= 0)
            {
                currentLockDelay = 0;
                currentLineClear = 0;
                CopyPieceToBoard();
                if (board.HasFullRows)
                {
                    messages.Add(new BlockMessage { Text = board.FullRowText, StartRow = board.FirstFullRow, InitialDuration = lineClearDelay / 60.0f, Speed = 50.0f });
                    currentLineClear = lineClearDelay;
                    SetSoundEffect(SOUND_LINE_DELETED);
                }
                else
                {
                    blocksFlag = false;
                }
            }
        }
    }

    private void CheckLineClear()
    {
        if (currentLineClear > 0)
        {
            currentLineClear--;
            if (currentLineClear <= 0)
            {
                currentLineClear = 0;
                currentLockDelay = 0;
                int oldRow = board.FirstFullRow;
                int rows = board.RemoveFullRows(currentLevel, ref points);
                if (blocksFlag)
                {
                    points += 200;
                    messages.Add(new BlockMessage { Text = "Back-to-back!", StartRow = oldRow - 1, InitialDuration = 2 * lineClearDelay / 60.0f, Speed = 60.0f });

                }
                blocksFlag = rows >= 4;
                levelRowsCleared += rows;
                totalRowsCleared += rows;
                CheckLevelUp(oldRow);
            }
        }
    }

    private void CheckLevelUp(int oldRow)
    {
        while (levelRowsCleared >= rowsPerLevel)
        {
            currentLevel += 1;
            levelRowsCleared -= rowsPerLevel;
            messages.Add(new BlockMessage { Text = "Level Up!", StartRow = oldRow - 1, InitialDuration = 2 * lineClearDelay / 60.0f, Speed = 60.0f });
            currentSpeed = GetSpeed(currentLevel);
            SetSoundEffect(SOUND_LEVEL_UP);
        }
    }

    private void CopyPieceToBoard(bool noSound = false)
    {
        if (currentPiece != null)
        {
            board.CopyPiece(currentPiece);
            points += currentPiece.Points;
            FindNextBest();
        }
        currentPiece = null;
        ghostPiece = null;
        if (holdPiece != null) holdPiece.Hold = false;
        if (!noSound) SetSoundEffect(SOUND_COLLIDED);
    }

    private void FindNextBest()
    {
        BlockScore temp = new BlockScore()
        {
            Points = points,
            Lines = totalRowsCleared
        };
        List<BlockScore> tempList = new List<BlockScore>(highScores);
        tempList.Add(temp);
        tempList.Sort();
        int index = tempList.IndexOf(temp);
        if (index > 0)
        {
            nextBestScore = tempList[index - 1];
        }
        else
        {
            nextBestScore = new BlockScore()
            {
                Name = "New Highscore!",
                Points = 0,
                Lines = 0
            };
        }
    }

    private static float GetSpeed(int level)
    {
        switch (level)
        {
            case 1:
                return 1.0f;
            case 2:
                return 0.793f;
            case 3:
                return 0.61780f;
            case 4:
                return 0.4723f;
            case 5:
                return 0.35520f;
            case 6:
                return 0.26200f;
            case 7:
                return 0.18968f;
            case 8:
                return 0.13473f;
            case 9:
                return 0.09388f;
            case 10:
                return 0.06415f;
            case 11:
                return 0.04298f;
            case 12:
                return 0.02822f;
            case 13:
                return 0.01815f;
            case 14:
                return 0.01144f;
            case 15:
                return 0.00706f;
            case 16:
                return 0.00426f;
            case 17:
                return 0.00252f;
            case 18:
                return 0.00146f;
            case 19:
                return 0.00082f;
            case 20:
            default:
                return 0.00046f;
        }
    }
}
