// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
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
