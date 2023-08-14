// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
public class Configuration
{
    public Configuration()
    {
        //Driver = "direct3d11";
        Driver = "opengl";
        WindowTitle = "SDL";
        WindowWidth = 1440;
        WindowHeight = 900;
        BackbufferWidth = 1440;
        BackbufferHeight = 900;
        SizeMode = RendererSizeMode.Window;
        FullScreen = false;
        Resizeable = true;
        AlwaysOnTop = false;
        Borderless = false;
        SkipTaskbar = false;
        MaxFramesPerSecond = 60;
        ShowFPS = false;
        FPSPosX = 10;
        FPSPosY = 10;
    }
    public string Driver { get; set; }
    public int WindowWidth { get; set; }
    public int WindowHeight { get; set; }
    public int BackbufferWidth { get; set; }
    public int BackbufferHeight { get; set; }
    public RendererSizeMode SizeMode { get; set; }
    public string? WindowTitle { get; set; }
    public bool FullScreen { get; set; }
    public bool Resizeable { get; set; }
    public bool AlwaysOnTop { get; set; }
    public bool Borderless { get; set; }
    public bool SkipTaskbar { get; set; }
    public int MaxFramesPerSecond { get; set; }
    public bool ShowFPS { get; set; }
    public int FPSPosX { get; set; }
    public int FPSPosY { get; set; }
}
