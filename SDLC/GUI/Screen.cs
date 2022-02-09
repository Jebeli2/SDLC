namespace SDLC.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class Screen : GUIObject, IGUIScreen
    {
        private readonly List<Window> windows = new();
        public Screen(IGUISystem gui)
            : base(gui)
        {
        }
        public string? Title { get => Text; set => Text = value; }

        public IEnumerable<IGUIWindow> Windows
        {
            get
            {
                foreach (Window window in windows)
                {
                    yield return window;
                }
            }
        }

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

        public void AddWindow(Window window)
        {
            windows.Add(window);
        }
        public override string ToString()
        {
            return $"Screen '{Title}'";
        }

    }
}
