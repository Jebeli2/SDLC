// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLTest.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BlockBoard
{
    private int width;
    private int height;
    private Block[,] blocks;
    private List<int> fullRows;
    private string fullRowText = "";

    public BlockBoard(int width, int height)
    {
        fullRows = new List<int>();
        this.width = width;
        this.height = height;
        blocks = new Block[width, height];
    }

    public int Width => width;

    public int Height => height;

    public Block this[int x, int y]
    {
        get
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                return blocks[x, y];
            }
            return default;
        }
        set
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                blocks[x, y] = value;
            }
        }
    }

    public bool HasCollision(int x, int y)
    {
        if (x < 0) return true;
        if (y < 0) return true;
        if (x >= width) return true;
        if (y >= height) return true;
        return blocks[x, y] != null;
    }

    public string FullRowText
    {
        get { return fullRowText; }
    }

    public int FirstFullRow
    {
        get { return fullRows.Count > 0 ? fullRows[0] : -1; }
    }

    public bool HasFullRows
    {
        get { return fullRows.Count > 0; }
    }

    public bool IsFullRow(int row)
    {
        return fullRows.Contains(row);
    }

    public void CopyPiece(BlockPiece piece)
    {
        if (piece != null)
        {
            foreach (var b in piece.Blocks)
            {
                int x = piece.X + b.X;
                int y = piece.Y + b.Y;
                this[x, y] = b;
            }
            FindFullRows();
        }
    }

    public int RemoveFullRows(int level, ref int points)
    {
        int count = 0;
        foreach (int row in fullRows)
        {
            RemoveRow(row);
            count++;
        }
        fullRows.Clear();
        points += CalcPoints(count, level);
        return count;
    }

    private int CalcPoints(int count, int level)
    {
        int points = 0;
        switch (count)
        {
            case 1:
                points = 100 * level;
                break;
            case 2:
                points = 300 * level;
                break;
            case 3:
                points = 500 * level;
                break;
            case 4:
                points = 800 * level;
                break;
        }
        return points;
    }

    private void RemoveRow(int row)
    {
        for (int y = row - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                blocks[x, y + 1] = blocks[x, y];
            }
        }
    }

    private void FindFullRows()
    {
        fullRowText = "";
        fullRows.Clear();
        for (int y = 0; y < height; y++)
        {
            if (CheckFullRow(y))
            {
                fullRows.Add(y);
            }
        }
        switch (fullRows.Count)
        {
            case 1:
                fullRowText = "Single!";
                break;
            case 2:
                fullRowText = "Double!";
                break;
            case 3:
                fullRowText = "Triple!";
                break;
            case 4:
                fullRowText = "Blocks!";
                break;
        }
    }

    private bool CheckFullRow(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (blocks[x, y] == null)
            {
                return false;
            }
        }
        return true;
    }
}
