namespace SDLC.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class StringInfo
    {
        private string buffer = "";
        private string undoBuffer = "";
        private int bufferPos;
        private int bufferSelStart;
        private int bufferSelEnd;

        private int maxChars;
        private int dispPos;
        private int undoPos;
        private int numChars;
        private int dispCount;
        private int cLeft;
        private int cTop;
        private long longInt;
        private readonly Gadget gadget;
        public StringInfo(Gadget gadget)
        {
            this.gadget = gadget;
        }
        public string Buffer
        {
            get => buffer;
            set
            {
                if (buffer != value)
                {
                    buffer = value;
                    Invalidate();
                }
            }
        }

        public int BufferPos
        {
            get => bufferPos;
            set
            {
                if (value == -1) { value = buffer.Length; }
                if (value > buffer.Length) { value = buffer.Length; }
                if (value < 0) { value = 0; }
                if (value != bufferPos)
                {
                    bufferSelStart = 0;
                    bufferSelEnd = 0;
                    bufferPos = value;
                    AdjustDispPos();
                    Invalidate();
                }
            }
        }
        public int BufferSelStart
        {
            get { return bufferSelStart; }
        }

        public int BufferSelEnd
        {
            get { return bufferSelEnd; }
        }

        public int DispPos => dispPos;
        private void NormSelection()
        {
            if (bufferSelStart > bufferSelEnd)
            {
                int temp = bufferSelEnd;
                bufferSelEnd = bufferSelStart;
                bufferSelStart = temp;
            }
        }
        private void SetBufferSel(int start, int end)
        {
            bufferSelStart = start;
            bufferSelEnd = end;
            NormSelection();
            Invalidate();
        }
        private void SetBufferSel(int pos)
        {
            if (pos < bufferPos)
            {
                SetBufferSel(pos, bufferPos);
            }
            else if (pos > bufferPos)
            {
                SetBufferSel(bufferSelStart, pos);
            }
            else
            {
                SetBufferSel(pos, pos);
            }
        }
        internal void Invalidate()
        {

        }

        private void AdjustDispPos()
        {
            int cd = CalcBufferPosDispPos(SDLApplication.DefaultFont);
            if (cd != 0)
            {
                Rectangle bounds = gadget.GetInnerBounds();
                if (cd > bounds.Width)
                {
                    while (cd > bounds.Width)
                    {
                        dispPos++;
                        cd = CalcBufferPosDispPos(SDLApplication.DefaultFont);
                    }
                }
                else if (cd < bounds.Width)
                {
                    while (cd < bounds.Width && dispPos > 0)
                    {
                        dispPos--;
                        cd = CalcBufferPosDispPos(SDLApplication.DefaultFont);
                    }
                }
            }
            else
            {
                dispPos = 0;
            }
        }
        private int CalcBufferPosDispPos(SDLFont? font)
        {
            int x = 0;
            if (font != null)
            {
                for (int i = dispPos; i < bufferPos + 1; i++)
                {
                    char ch = ' ';
                    if (i < buffer.Length) { ch = buffer[i]; }
                    //if (ch == ' ') { ch = 'j'; }
                    Size size = font.MeasureText("" + ch);
                    x += size.Width;
                }
            }
            return x;
        }
        private bool MapPosition(SDLFont font, int mx, int my, out int pos)
        {
            pos = 0;
            int x = 0;
            int y = 0;
            int lineSkip = font.LineSkip;
            if (mx < x) return false;
            if (my < y) return false;
            for (int i = dispPos; i < buffer.Length; i++)
            {
                char ch = buffer[i];
                Size size = font.MeasureText("" + ch);
                int gx = size.Width;
                if ((my >= y && my <= (y + lineSkip)) && (mx >= x && mx <= x + gx))
                {
                    pos = i;
                    return true;
                }
                x += gx;
            }
            if ((my >= y && my <= (y + lineSkip)) && (mx >= x))
            {
                pos = buffer.Length;
                return true;
            }
            return false;
        }

        private bool GetPos(int x, int y, out int pos)
        {
            if (SDLApplication.DefaultFont != null)
            {
                if (MapPosition(SDLApplication.DefaultFont, x, y, out pos))
                {
                    return true;
                }
            }
            pos = -1;
            return false;
        }

        internal bool HandleMouseDown(Rectangle bounds, int x, int y)
        {
            if (GetPos(x - bounds.X, y - bounds.Y, out int pos))
            {
                BufferPos = pos;
                return true;
            }
            return false;
        }
        internal bool HandleMouseUp(Rectangle bounds, int x, int y)
        {
            if (GetPos(x - bounds.X, y - bounds.Y, out int pos))
            {
                SetBufferSel(pos);
                return true;
            }
            return false;
        }
        internal bool HandleMouseMove(Rectangle bounds, int x, int y)
        {
            if (GetPos(x - bounds.X, y - bounds.Y, out int pos))
            {
                SetBufferSel(pos);
                return true;
            }
            return false;
        }
        internal bool HandleKeyDown(SDLKeyEventArgs e)
        {
            switch (e.ScanCode)
            {
                case ScanCode.SCANCODE_LEFT:
                    if (bufferPos > 0)
                    {
                        BufferPos--;
                    }
                    break;
                case ScanCode.SCANCODE_RIGHT:
                    BufferPos++;
                    break;
                case ScanCode.SCANCODE_HOME:
                    BufferPos = 0;
                    break;
                case ScanCode.SCANCODE_END:
                    BufferPos = buffer.Length;
                    break;
                case ScanCode.SCANCODE_DELETE:
                    RemoveOrDelText(false);
                    break;
                case ScanCode.SCANCODE_BACKSPACE:
                    RemoveOrDelText(true);
                    break;
            }
            return true;
        }
        internal bool HandleKeyUp(SDLKeyEventArgs e)
        {
            return true;
        }
        internal bool HandleTextInput(SDLTextInputEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text))
            {
                ReplaceOrAddText(e.Text);
            }
            return true;
        }
        private void RemoveOrDelText(bool backSpace)
        {
            if (bufferSelStart >= 0 && bufferSelEnd > 0)
            {
                int start = bufferSelStart;
                int len = bufferSelEnd - start;

                Buffer = buffer.Remove(start, len);
                bufferSelStart = 0;
                bufferSelEnd = 0;
                BufferPos = start;
            }
            else if (backSpace)
            {
                if (bufferPos > 0)
                {
                    Buffer = buffer.Remove(bufferPos - 1, 1);
                    BufferPos--;
                }
            }
            else
            {
                if (bufferPos < buffer.Length)
                {
                    Buffer = buffer.Remove(bufferPos, 1);
                }
            }
        }
        private void ReplaceOrAddText(string text)
        {
            if (bufferSelStart >= 0 && bufferSelEnd > 0)
            {
                int start = bufferSelStart;
                int len = bufferSelEnd - start;
                string temp = buffer.Remove(start, len);
                temp = temp.Insert(start, text);
                Buffer = temp;
                BufferPos = start + text.Length;
            }
            else
            {
                Buffer = buffer.Insert(bufferPos, text);
                BufferPos += text.Length;
            }
        }

    }
}
