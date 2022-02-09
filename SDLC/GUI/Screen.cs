namespace SDLC.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Screen : GUIObject
    {
        private readonly List<Window> windows = new();
        public Screen(IGUISystem gui)
            : base(gui)
        {
        }
        public string? Title { get => Text; set => Text = value; }

        public IEnumerable<Window> Windows => windows;

        public Window? FindWindow(int x, int y)
        {
            x -= LeftEdge;
            y -= TopEdge;
            for (int i = windows.Count - 1; i >= 0; i--)
            {
                Window win = windows[i];
                if (win.Contains(x, y))
                {
                    return win;
                }
            }
            return null;
        }

        internal void AddWindow(Window window)
        {
            windows.Add(window);
        }

        internal void RemoveWindow(Window window)
        {
            windows.Remove(window);
        }

        internal bool IsFrontWindow(Window window)
        {
            int index = windows.IndexOf(window);
            return index == windows.Count - 1;
        }


        internal void WindowToFront(Window window)
        {
            int index = windows.IndexOf(window);
            if (index >= 0 && index < windows.Count - 1)
            {
                windows.RemoveAt(index);
                windows.Add(window);
            }

        }
        internal void WindowToBack(Window window)
        {
            int index = windows.IndexOf(window);
            if (index > 0)
            {
                windows.RemoveAt(index);
                windows.Insert(0, window);
            }
        }

        internal void Close()
        {
            foreach (Window window in windows)
            {
                window.Close();
            }
            windows.Clear();
        }

        public void Render(IRenderer gfx, IGUIRenderer ren)
        {
            ren.RenderScreen(gfx, this);
            foreach (var win in windows)
            {
                win.Render(gfx, ren);
            }
        }

        internal Window? NextWindow(Window window)
        {
            int index = windows.IndexOf(window);
            index--;
            if (index < 0) { index = windows.Count - 1; }
            if (index < 0) { return null; }
            Window next = windows[index];
            if (next != window) return next;
            return null;
        }
        public override string ToString()
        {
            return $"Screen '{Title}'";
        }

    }
}
