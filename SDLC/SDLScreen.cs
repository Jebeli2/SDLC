// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
using System.Collections.Generic;
using SDLC.GUI;

public class SDLScreen : IScreen
{
    private readonly Configuration configuration = new();
    private readonly string name;
    private readonly List<SDLApplet> appletsToAdd = new();


    private int width;
    private int height;
    private IWindow? window;
    private IRenderer? renderer;
    private IContentManager? contentManager;
    private IGUISystem? gui;
    private IParticleSystem? particleSystem;

    public SDLScreen(string name = "SDLScreen")
    {
        this.name = name;
    }
    public string Name => name;

    public Configuration Configuration => configuration;

    public IGUISystem GUI
    {
        get
        {
            if (gui == null)
            {
                gui = window?.GUI;
                if (gui == null)
                {
                    gui = GetApplet<Applets.GUISystem>();
                }
            }
            return gui;
        }
        set
        {
            if (gui != value)
            {
                gui = value;
                if (window != null)
                {
                    window.GUI = value;
                }
            }
        }
    }

    public IParticleSystem ParticleSystem
    {
        get
        {
            if (particleSystem == null)
            {
                particleSystem = window?.ParticleSystem;
                if (particleSystem == null)
                {
                    particleSystem = GetApplet<Particles.ParticleSystem>();
                }
            }
            return particleSystem;
        }
        set
        {
            if (particleSystem != value)
            {
                particleSystem = value;
                if (window != null)
                {
                    window.ParticleSystem = value;
                }
            }
        }
    }

    public Configuration GetConfiguration()
    {
        return configuration;
    }

    public virtual void Hide(IWindow window)
    {
        SetWindow(window);
        SDLLog.Info(LogCategory.APPLICATION, "Hide Screen {0}", name);
    }

    public virtual void Initialize(IWindow window)
    {
        SetWindow(window);
        SDLLog.Info(LogCategory.APPLICATION, "Initialize Screen {0} ({1}x{2})", name, width, height);
    }

    public virtual void Pause(IWindow window)
    {
        SetWindow(window);
        SDLLog.Info(LogCategory.APPLICATION, "Pause Screen {0}", name);
    }

    public virtual void Render(IRenderer renderer, double totalTime, double elapsedTime)
    {
    }

    public virtual void Resized(IWindow window, int width, int height)
    {
        SetWindow(window);
        SDLLog.Info(LogCategory.APPLICATION, "Resized Screen {0} ({1}x{2})", name, this.width, this.height);
    }

    public virtual void Resume(IWindow window)
    {
        SetWindow(window);
        SDLLog.Info(LogCategory.APPLICATION, "Resume Screen {0}", name);
    }

    public virtual void Show(IWindow window)
    {
        SetWindow(window);
        SDLLog.Info(LogCategory.APPLICATION, "Show Screen {0} ({1}x{2})", name, width, height);
    }

    public virtual void Shutdown(IWindow window)
    {
        SDLLog.Info(LogCategory.APPLICATION, "Shutdown Screen {0}", name);
        SetWindow(window);
    }

    public virtual void Update(IRenderer renderer, double totalTime, double elapsedTime)
    {
    }

    private void SetWindow(IWindow window)
    {
        this.window = window;
        renderer = this.window.Renderer;
        contentManager = this.window.ContentManager;
        width = renderer.Width;
        height = renderer.Height;
        while (appletsToAdd.Count > 0)
        {
            SDLApplet applet = appletsToAdd[0];
            appletsToAdd.RemoveAt(0);
            window.AddApplet(applet);
        }
    }

    #region Convenience Methods for inheritors

    protected int Width => width;
    protected int Height => height;

    protected FullScreenMode FullScreenMode
    {
        get => window?.FullScreenMode ?? 0;
        set
        {
            if (window != null)
            {
                window.FullScreenMode = value;
            }
        }
    }
    protected bool IsFullScreen
    {
        get => window?.FullScreen ?? false;
        set
        {
            if (window != null)
            {
                window.FullScreen = value;
            }
        }
    }
    protected T GetApplet<T>() where T : SDLApplet, new()
    {
        if (window != null)
        {
            return window.GetApplet<T>();
        }
        return new T();
    }
    protected void ChangeScreen(IScreen screen)
    {
        if (window != null)
        {
            window.Screen = screen;
        }
    }
    protected void AddResourceManager(System.Resources.ResourceManager resourceManager)
    {
        contentManager?.AddResourceManager(resourceManager);
    }
    protected SDLTexture? LoadTexture(string name)
    {
        byte[]? data = contentManager?.FindContent(name);
        return renderer?.LoadTexture(name, data);
    }

    protected SDLTexture? LoadTexture(string name, byte[]? data)
    {
        return renderer?.LoadTexture(name, data);
    }

    protected SDLMusic? LoadMusic(string name)
    {
        byte[]? data = contentManager?.FindContent(name);
        return SDLAudio.LoadMusic(name, data);
    }
    protected SDLMusic? LoadMusic(string name, byte[]? data)
    {
        return SDLAudio.LoadMusic(name, data);
    }

    protected SDLSound? LoadSound(string name)
    {
        byte[]? data = contentManager?.FindContent(name);
        return SDLAudio.LoadSound(name, data);
    }
    protected SDLSound? LoadSound(string name, byte[]? data)
    {
        return SDLAudio.LoadSound(name, data);
    }

    protected SDLFont? LoadFont(string name, int ySize)
    {
        byte[]? data = contentManager?.FindContent(name);
        if (data != null)
        {
            return SDLFont.LoadFont(data, name, ySize);
        }
        return null;
    }

    protected byte[]? LoadData(string name)
    {
        if (contentManager != null)
        {
            string dir = contentManager.SaveDirectory;
            string fileName = Path.Combine(dir, name);
            if (File.Exists(fileName))
            {
                return File.ReadAllBytes(fileName);
            }
        }
        return null;
    }

    protected bool SaveData(string name, byte[] data)
    {
        if (contentManager != null)
        {
            string dir = contentManager.SaveDirectory;
            DirectoryInfo info = new DirectoryInfo(dir);
            if (!info.Exists)
            {
                try
                {
                    info.Create();
                }
                catch (Exception ex)
                {
                    SDLLog.Error(LogCategory.APPLICATION, "Could not create save directory '{0}': {1}", dir, ex.Message);
                    return false;
                }
            }
            string fileName = Path.Combine(dir, name);
            try
            {
                File.WriteAllBytes(fileName, data);
                return true;
            }
            catch (Exception ex)
            {
                SDLLog.Error(LogCategory.APPLICATION, "Could not create file '{0}': {1}", dir, ex.Message);
                return false;
            }
        }
        return false;
    }

    protected void AddApplet(SDLApplet applet)
    {
        window?.AddApplet(applet);
    }
    protected void RemoveApplet(SDLApplet applet)
    {
        window?.RemoveApplet(applet);
    }

    #endregion

}
