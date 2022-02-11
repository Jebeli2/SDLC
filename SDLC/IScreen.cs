// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
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
