namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class NoScreen : IScreen
    {
        private readonly Configuration configuration = new();
        public Configuration GetConfiguration() { return configuration; }
        public void Hide(IWindow window)
        {
        }

        public void Initialize(IWindow window)
        {
        }

        public void Pause(IWindow window)
        {
        }

        public void Render(IRenderer renderer, double totalTime, double elapsedTime)
        {
        }

        public void Resized(IWindow window, int width, int height)
        {
        }

        public void Resume(IWindow window)
        {
        }

        public void Show(IWindow window)
        {
        }

        public void Shutdown(IWindow window)
        {
        }

        public void Update(IRenderer renderer, double totalTime, double elapsedTime)
        {
        }
    }
}
