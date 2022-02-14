// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLTest.Blocks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BlockPiece
{
    private static Random rnd = new Random();
    private static IList<Tetronimo>? bag;
    private int[,] blocks;
    private Tetronimo tetronimo;
    private Rotation rotation;
    private Color color;
    private List<Block> blockList;
    private int x;
    private int y;
    private int width;
    private int height;
    private int points;
    private bool hold;

    private static Color colorZ = Color.FromArgb(0xF0, 0, 0);
    private static Color colorS = Color.FromArgb(0, 0xF0, 0);
    private static Color colorO = Color.FromArgb(0xF0, 0xF0, 0);
    private static Color colorJ = Color.FromArgb(0, 0, 0xF0);
    private static Color colorI = Color.FromArgb(0, 0xF0, 0xF0);
    private static Color colorT = Color.FromArgb(0xA0, 0, 0xF0);
    private static Color colorL = Color.FromArgb(0xF0, 0xA0, 0);

    private BlockPiece(Tetronimo t, Rotation r, int[,] blocks)
    {
        this.blocks = blocks;
        tetronimo = t;
        rotation = r;
        color = GetColor(t);
        blockList = BuildBlocks();
    }

    public BlockPiece(BlockPiece other)
    {
        x = other.x;
        y = other.y;
        blocks = other.blocks;
        tetronimo = other.tetronimo;
        rotation = other.rotation;
        color = other.color;
        blockList = other.blockList;
        points = other.points;
        hold = other.hold;
    }

    public bool Hold
    {
        get { return hold; }
        set { hold = value; }
    }

    public static Color GetColor(Tetronimo t)
    {
        switch (t)
        {
            case Tetronimo.I: return colorI;
            case Tetronimo.J: return colorJ;
            case Tetronimo.L: return colorL;
            case Tetronimo.O: return colorO;
            case Tetronimo.S: return colorS;
            case Tetronimo.T: return colorT;
            case Tetronimo.Z: return colorZ;
            default: return Color.Black;
        }
    }

    public bool HasLeftNeightbor(Block block)
    {
        return HasNeightbor(block, -1, 0);
    }
    public bool HasRightNeightbor(Block block)
    {
        return HasNeightbor(block, 1, 0);
    }

    public bool HasTopNeightbor(Block block)
    {
        return HasNeightbor(block, 0, -1);
    }

    public bool HasBottomNeightbor(Block block)
    {
        return HasNeightbor(block, 0, 1);
    }

    public bool HasNeightbor(Block block, int dx, int dy)
    {
        int x = block.X + dx;
        int y = block.Y + dy;
        if (x < 0) return false;
        if (y < 0) return false;
        if (x >= width) return false;
        if (y >= height) return false;
        return blocks[y, x] > 0;
    }

    public int[] GetPoints()
    {
        List<int> list = new List<int>();

        return list.ToArray();
    }

    private List<Block> BuildBlocks()
    {
        List<Block> list = new List<Block>();
        height = blocks.GetUpperBound(0) + 1;
        width = blocks.GetUpperBound(1) + 1;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (blocks[y, x] != 0)
                {
                    Block block = new Block(x, y, color, tetronimo);
                    list.Add(block);
                }
            }
        }
        return list;
    }

    public IList<Block> Blocks
    {
        get { return blockList; }
    }

    public int X
    {
        get { return x; }
        set { x = value; }
    }

    public int Y
    {
        get { return y; }
        set { y = value; }
    }

    public int Width
    {
        get { return width; }
    }

    public int Height
    {
        get { return height; }
    }

    public int Points
    {
        get { return points; }
        set { points = value; }
    }

    public bool MoveLeft(BlockBoard board)
    {
        return Move(-1, 0, board);
    }

    public bool MoveRight(BlockBoard board)
    {
        return Move(1, 0, board);
    }

    public bool MoveDown(BlockBoard board)
    {
        return Move(0, 1, board);
    }

    public bool MoveUp(BlockBoard board)
    {
        return Move(0, -1, board);
    }

    public bool Move(int dx, int dy, BlockBoard board)
    {
        return MoveTo(x + dx, y + dy, board);
    }

    public bool MoveTo(int x, int y, BlockBoard board)
    {
        BlockPiece temp = new BlockPiece(this);
        temp.x = x;
        temp.y = y;
        if (!temp.HasColision(board))
        {
            this.x = temp.x;
            this.y = temp.y;
            return true;
        }
        return false;
    }

    public bool Drop(BlockBoard board)
    {
        while (MoveDown(board))
        {
            points += 2;
        }
        return true;
    }

    public bool HasColision(BlockBoard board)
    {
        foreach (var b in blockList)
        {
            int bx = x + b.X;
            int by = y + b.Y;
            if (board.HasCollision(bx, by))
            {
                return true;
            }
        }
        return false;
    }

    public bool MoveToStartPosition(BlockBoard board)
    {

        int midX = (int)((board.Width - width) / 2.0f);
        x = midX;
        y = 0;
        return MoveTo(midX, 0, board);
    }

    public bool RotateToStart()
    {
        BlockPiece temp = GetPiece(tetronimo, Rotation.R0);
        blocks = temp.blocks;
        blockList = temp.blockList;
        rotation = temp.rotation;
        x = 0;
        y = 0;
        return true;
    }

    public bool RotateLeft(BlockBoard board)
    {
        return Rotate(1, board);
    }

    public bool RotateRight(BlockBoard board)
    {
        return Rotate(-1, board);
    }

    public bool Rotate(int d, BlockBoard board)
    {
        int r = (int)rotation;
        r += d;
        if (r >= 4) r = 0;
        else if (r < 0) r = 3;
        Rotation dest = (Rotation)r;
        BlockPiece temp = GetPiece(tetronimo, dest);
        var wallkicks = BuildWallKickData(tetronimo, rotation, dest);
        foreach (var p in wallkicks)
        {
            temp.x = x + p.X;
            temp.y = y + p.Y;
            if (!temp.HasColision(board))
            {
                blocks = temp.blocks;
                blockList = temp.blockList;
                rotation = temp.rotation;
                x = temp.x;
                y = temp.y;
                return true;
            }
        }
        return false;
    }

    public Color Color
    {
        get
        {
            return color;
        }
        set
        {
            color = value;
        }
    }

    private static void Shuffle(IList<Tetronimo> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int n = rnd.Next(i + 1);
            Tetronimo t = list[i];
            list[i] = list[n];
            list[n] = t;
        }
        //list.Shuffle(rnd);
        //for (int i = list.Count - 1; i >= 0; i--)
        //{
        //    int j = rnd.Next(i);
        //    Tetronimo t = list[i];
        //    list[i] = list[j];
        //    list[j] = t;
        //}
    }


    private static Point[] BuildWallKickData(Tetronimo t, Rotation rotation, Rotation dest)
    {
        switch (t)
        {
            case Tetronimo.J:
            case Tetronimo.L:
            case Tetronimo.S:
            case Tetronimo.T:
            case Tetronimo.Z:
                if (rotation == Rotation.R0 && dest == Rotation.R90) return new Point[] { new Point(0, 0), new Point(-1, 0), new Point(-1, 1), new Point(0, -2), new Point(-1, -2) };
                if (rotation == Rotation.R90 && dest == Rotation.R0) return new Point[] { new Point(0, 0), new Point(1, 0), new Point(1, -1), new Point(0, 2), new Point(1, 2) };
                if (rotation == Rotation.R90 && dest == Rotation.R180) return new Point[] { new Point(0, 0), new Point(1, 0), new Point(1, -1), new Point(0, 2), new Point(1, 2) };
                if (rotation == Rotation.R180 && dest == Rotation.R90) return new Point[] { new Point(0, 0), new Point(-1, 0), new Point(-1, 1), new Point(0, -2), new Point(-1, -2) };
                if (rotation == Rotation.R180 && dest == Rotation.R270) return new Point[] { new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(0, -2), new Point(1, -2) };
                if (rotation == Rotation.R270 && dest == Rotation.R180) return new Point[] { new Point(0, 0), new Point(-1, 0), new Point(-1, -1), new Point(0, 2), new Point(-1, 2) };
                if (rotation == Rotation.R270 && dest == Rotation.R0) return new Point[] { new Point(0, 0), new Point(-1, 0), new Point(-1, -1), new Point(0, 2), new Point(-1, -2) };
                if (rotation == Rotation.R0 && dest == Rotation.R270) return new Point[] { new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(0, -2), new Point(-1, -2) };
                break;
            case Tetronimo.I:
                if (rotation == Rotation.R0 && dest == Rotation.R90) return new Point[] { new Point(0, 0), new Point(-2, 0), new Point(1, 0), new Point(-2, -1), new Point(1, 2) };
                if (rotation == Rotation.R90 && dest == Rotation.R0) return new Point[] { new Point(0, 0), new Point(2, 0), new Point(-1, 0), new Point(2, 1), new Point(-1, -2) };
                if (rotation == Rotation.R90 && dest == Rotation.R180) return new Point[] { new Point(0, 0), new Point(-1, 0), new Point(2, 0), new Point(-1, 2), new Point(2, -1) };
                if (rotation == Rotation.R180 && dest == Rotation.R90) return new Point[] { new Point(0, 0), new Point(1, 0), new Point(-2, 0), new Point(1, -2), new Point(-2, 1) };
                if (rotation == Rotation.R180 && dest == Rotation.R270) return new Point[] { new Point(0, 0), new Point(2, 0), new Point(-1, 0), new Point(2, 1), new Point(-1, -2) };
                if (rotation == Rotation.R270 && dest == Rotation.R180) return new Point[] { new Point(0, 0), new Point(2, 0), new Point(1, 0), new Point(-2, -1), new Point(1, 2) };
                if (rotation == Rotation.R270 && dest == Rotation.R0) return new Point[] { new Point(0, 0), new Point(1, 0), new Point(-2, 0), new Point(1, -2), new Point(-2, 1) };
                if (rotation == Rotation.R0 && dest == Rotation.R270) return new Point[] { new Point(0, 0), new Point(-1, 0), new Point(2, 0), new Point(-1, 2), new Point(2, -1) };
                break;
            default:
                return new Point[] { new Point(0, 0) };
        }
        return new Point[] { new Point(0, 0) };
    }

    public static void ClearBag()
    {
        bag = null;
    }

    private static IList<Tetronimo> BuildInitialBag()
    {
        var bag = BuildBag();
        while (bag[0] == Tetronimo.S || bag[0] == Tetronimo.Z || bag[0] == Tetronimo.O)
        {
            bag = BuildBag();
        }
        return bag;
    }

    private static IList<Tetronimo> BuildBag()
    {
        List<Tetronimo> bag = new List<Tetronimo>
            {
                Tetronimo.I,
                Tetronimo.J,
                Tetronimo.L,
                Tetronimo.O,
                Tetronimo.S,
                Tetronimo.T,
                Tetronimo.Z,
                Tetronimo.I,
                Tetronimo.J,
                Tetronimo.L,
                Tetronimo.O,
                Tetronimo.S,
                Tetronimo.T,
                Tetronimo.Z,
                Tetronimo.I,
                Tetronimo.J,
                Tetronimo.L,
                Tetronimo.O,
                Tetronimo.S,
                Tetronimo.T,
                Tetronimo.Z,
                Tetronimo.I,
                Tetronimo.J,
                Tetronimo.L,
                Tetronimo.O,
                Tetronimo.S,
                Tetronimo.T,
                Tetronimo.Z,
                Tetronimo.I,
                Tetronimo.J,
                Tetronimo.L,
                Tetronimo.O,
                Tetronimo.S,
                Tetronimo.T,
                Tetronimo.Z
            };

        Shuffle(bag);
        return bag;
    }

    public static BlockPiece GetNextPiece(bool initial = false)
    {
        if (bag == null || bag.Count == 0)
        {
            if (initial)
            {
                bag = BuildInitialBag();
            }
            else
            {
                bag = BuildBag();
            }
        }
        Tetronimo t = bag[0];
        bag.RemoveAt(0);
        return GetPiece(t, Rotation.R0);
        //int r = rnd.Next(7);
        //return (GetPiece((Tetronimo)r, Rotation.R0));
    }

    public static BlockPiece GetPiece(Tetronimo t, Rotation r)
    {
        switch (t)
        {
            case Tetronimo.I:
                switch (r)
                {
                    case Rotation.R0:
                        return new BlockPiece(t, r, new int[4, 4] { { 0, 0, 0, 0 }, { 1, 1, 1, 1 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } });
                    case Rotation.R90:
                        return new BlockPiece(t, r, new int[4, 4] { { 0, 0, 1, 0 }, { 0, 0, 1, 0 }, { 0, 0, 1, 0 }, { 0, 0, 1, 0 } });
                    case Rotation.R180:
                        return new BlockPiece(t, r, new int[4, 4] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 1, 1, 1, 1 }, { 0, 0, 0, 0 } });
                    case Rotation.R270:
                        return new BlockPiece(t, r, new int[4, 4] { { 0, 1, 0, 0 }, { 0, 1, 0, 0 }, { 0, 1, 0, 0 }, { 0, 1, 0, 0 } });
                }
                break;
            case Tetronimo.J:
                switch (r)
                {
                    case Rotation.R0:
                        return new BlockPiece(t, r, new int[3, 3] { { 1, 0, 0 }, { 1, 1, 1 }, { 0, 0, 0 } });
                    case Rotation.R90:
                        return new BlockPiece(t, r, new int[3, 3] { { 0, 1, 1 }, { 0, 1, 0 }, { 0, 1, 0 } });
                    case Rotation.R180:
                        return new BlockPiece(t, r, new int[3, 3] { { 0, 0, 0 }, { 1, 1, 1 }, { 0, 0, 1 } });
                    case Rotation.R270:
                        return new BlockPiece(t, r, new int[3, 3] { { 0, 1, 0 }, { 0, 1, 0 }, { 1, 1, 0 } });
                }
                break;
            case Tetronimo.L:
                switch (r)
                {
                    case Rotation.R0:
                        return new BlockPiece(t, r, new int[3, 3] { { 0, 0, 1 }, { 1, 1, 1 }, { 0, 0, 0 } });
                    case Rotation.R90:
                        return new BlockPiece(t, r, new int[3, 3] { { 0, 1, 0 }, { 0, 1, 0 }, { 0, 1, 1 } });
                    case Rotation.R180:
                        return new BlockPiece(t, r, new int[3, 3] { { 0, 0, 0 }, { 1, 1, 1 }, { 1, 0, 0 } });
                    case Rotation.R270:
                        return new BlockPiece(t, r, new int[3, 3] { { 1, 1, 0 }, { 0, 1, 0 }, { 0, 1, 0 } });
                }
                break;
            case Tetronimo.O:
                return new BlockPiece(t, r, new int[3, 4] { { 0, 1, 1, 0 }, { 0, 1, 1, 0 }, { 0, 0, 0, 0 } });
            case Tetronimo.S:
                switch (r)
                {
                    case Rotation.R0:
                        return new BlockPiece(t, r, new int[3, 3] { { 0, 1, 1 }, { 1, 1, 0 }, { 0, 0, 0 } });
                    case Rotation.R90:
                        return new BlockPiece(t, r, new int[3, 3] { { 0, 1, 0 }, { 0, 1, 1 }, { 0, 0, 1 } });
                    case Rotation.R180:
                        return new BlockPiece(t, r, new int[3, 3] { { 0, 0, 0 }, { 0, 1, 1 }, { 1, 1, 0 } });
                    case Rotation.R270:
                        return new BlockPiece(t, r, new int[3, 3] { { 1, 0, 0 }, { 1, 1, 0 }, { 0, 1, 0 } });
                }
                break;
            case Tetronimo.T:
                switch (r)
                {
                    case Rotation.R0:
                        return new BlockPiece(t, r, new int[3, 3] { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 0, 0 } });
                    case Rotation.R90:
                        return new BlockPiece(t, r, new int[3, 3] { { 0, 1, 0 }, { 0, 1, 1 }, { 0, 1, 0 } });
                    case Rotation.R180:
                        return new BlockPiece(t, r, new int[3, 3] { { 0, 0, 0 }, { 1, 1, 1 }, { 0, 1, 0 } });
                    case Rotation.R270:
                        return new BlockPiece(t, r, new int[3, 3] { { 0, 1, 0 }, { 1, 1, 0 }, { 0, 1, 0 } });
                }
                break;
            case Tetronimo.Z:
                switch (r)
                {
                    case Rotation.R0:
                        return new BlockPiece(t, r, new int[3, 3] { { 1, 1, 0 }, { 0, 1, 1 }, { 0, 0, 0 } });
                    case Rotation.R90:
                        return new BlockPiece(t, r, new int[3, 3] { { 0, 0, 1 }, { 0, 1, 1 }, { 0, 1, 0 } });
                    case Rotation.R180:
                        return new BlockPiece(t, r, new int[3, 3] { { 0, 0, 0 }, { 1, 1, 0 }, { 0, 1, 1 } });
                    case Rotation.R270:
                        return new BlockPiece(t, r, new int[3, 3] { { 0, 1, 0 }, { 1, 1, 0 }, { 1, 0, 0 } });
                }
                break;
        }
        throw new ArgumentOutOfRangeException($"No Piece for {t}-{r}");
    }
}

