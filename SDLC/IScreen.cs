namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IScreen
    {
        Configuration GetConfiguration();

        void Initialize(IWindow window);
        void Shutdown(IWindow window);
        void Show(IWindow window);
        void Hide(IWindow window);

        void Update(IRenderer renderer, double totalTime, double elapsedTime);
        void Render(IRenderer renderer, double totalTime, double elapsedTime);
        void Pause(IWindow window);
        void Resume(IWindow window);
        void Resized(IWindow window, int width, int height);
    }
}
