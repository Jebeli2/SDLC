namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    public static class SDLApplication
    {
        private static readonly List<SDLDriverInfo> driverInfos = new();
        private static readonly List<SDLWindow> windows = new();
        private static bool quitRequested;
        private static ulong frequency = 1;
        private static bool limitFps = true;
        private static double totalElapsedTime;
        private static double elapsedTime;
        private static int framesPerSecond = 60;
        private static int prevFramesPerSecond = 60;
        private static int maxFramesPerSecond = 60;
        private static double maxElapsedTime = 500;
        private static double fpsTime;
        private static double accumulatedElapsedTime;
        private static double previousTime;
        private static double targetTime = 1000.0 / maxFramesPerSecond;
        private static int updateFrameLag;

        private static int frameCounter;
        private static bool suppressDraw;
        private static bool isRunningSlowly;
        private static SDLFont? defaultFont;
        private static SDLFont? iconFont;
        private static string? fpsText;
        private static SDLWindow? mainWindow;
        private static readonly SDLObjectTracker<SDLFont> fontTracker = new(LogCategory.FONT, "Font");

        public static void Run(IScreen screen, LogPriority logPriority = LogPriority.Info)
        {
            Run(new SDLWindow(screen), logPriority);
        }
        private static void Run(SDLWindow window, LogPriority logPriority = LogPriority.Info)
        {
            mainWindow = window;
            Initialize(logPriority);
            windows.Add(window);
            if (window.Visible) { window.Show(); }
            MainLoop();
            Shutdown();
        }

        //public static SDLWindow? MainWindow => mainWindow;

        public static int GetDriverIndex(string name)
        {
            for (int i = 0; i < driverInfos.Count; i++)
            {
                if (driverInfos[i].Name == name)
                {
                    return i;
                }
            }
            return -1;
        }

        private static void GetDriverInfos()
        {
            int numDrivers = SDL_GetNumRenderDrivers();
            for (int i = 0; i < numDrivers; i++)
            {
                _ = SDLRenderer.SDL_GetRenderDriverInfo(i, out SDLRenderer.SDL_RendererInfo info);
                SDLDriverInfo driverInfo = new()
                {
                    MaxTextureWidth = info.max_texture_width,
                    MaxTextureHeight = info.max_texture_height,
                    Flags = (uint)info.flags,
                    Name = IntPtr2String(info.name) ?? "",
                };
                for (int j = 0; j < info.num_texture_formats; j++)
                {
                    driverInfo.TextureFormats.Add(info.texture_formats[j]);
                    driverInfo.TextureFormatNames.Add(IntPtr2String(SDL_GetPixelFormatName(info.texture_formats[j])) ?? "UNKNOWN");
                }
                driverInfos.Add(driverInfo);
            }
        }


        public static string FPSText
        {
            get
            {
                if (fpsText == null)
                {
                    fpsText = BuildFPSText();
                }
                return fpsText;
            }
        }

        private static string BuildFPSText()
        {
            return framesPerSecond.ToString() + " fps";
        }

        public static int MaxFramesPerSecond
        {
            get => maxFramesPerSecond;
            set
            {
                if (value < 1) { value = 1; }
                if (maxFramesPerSecond != value)
                {
                    maxFramesPerSecond = value;
                    framesPerSecond = value;
                    frameCounter = 0;
                    fpsTime = 0;
                    prevFramesPerSecond = value;
                    targetTime = 1000.0 / maxFramesPerSecond;
                    fpsText = null;
                }
            }
        }
        internal static bool QuitRequested
        {
            get => quitRequested;
            set => quitRequested = value;
        }

        public static SDLFont? DefaultFont => defaultFont;
        internal static SDLFont? IconFont => iconFont;

        internal static SDLWindow? MainWindow => mainWindow;

        internal static SDLWindow? GetWindowFromId(int id)
        {
            return windows.FirstOrDefault(window => window.WindowId == id);
        }

        public static void Exit()
        {
            quitRequested = true;
        }
        private static void MainLoop()
        {
            previousTime = GetCurrentTime();
            UpdateLoop(0, 0);
            while (!quitRequested)
            {
                SDLInput.MessageLoop();
                Tick();
            }
        }

        private static void Tick()
        {
            RetryTick:
            double currentTime = GetCurrentTime();
            accumulatedElapsedTime += currentTime - previousTime;
            previousTime = currentTime;
            if (limitFps && accumulatedElapsedTime < targetTime)
            {
                Delay(targetTime - accumulatedElapsedTime);
                goto RetryTick;
            }
            if (accumulatedElapsedTime > maxElapsedTime) { accumulatedElapsedTime = maxElapsedTime; }
            if (limitFps)
            {
                elapsedTime = targetTime;
                int stepCount = 0;
                while (accumulatedElapsedTime >= targetTime && !quitRequested)
                {
                    totalElapsedTime += targetTime;
                    accumulatedElapsedTime -= targetTime;
                    ++stepCount;
                    UpdateLoop(totalElapsedTime, elapsedTime);
                }
                updateFrameLag += Math.Max(0, stepCount - 1);
                if (isRunningSlowly)
                {
                    if (updateFrameLag == 0)
                    {
                        isRunningSlowly = false;
                        SDLLog.Verbose(LogCategory.APPLICATION, $"Stopped Running Slowly");
                    }
                }
                else if (updateFrameLag >= 5)
                {
                    isRunningSlowly = true;
                    SDLLog.Verbose(LogCategory.APPLICATION, $"Started Running Slowly");
                }
                if (stepCount == 1 && updateFrameLag > 0) { updateFrameLag--; }
                elapsedTime = targetTime * stepCount;
            }
            else
            {
                elapsedTime += accumulatedElapsedTime;
                totalElapsedTime += accumulatedElapsedTime;
                accumulatedElapsedTime = 0;
                UpdateLoop(totalElapsedTime, elapsedTime);
            }
            if (suppressDraw)
            {
                suppressDraw = false;
            }
            else
            {
                PaintLoop(totalElapsedTime, elapsedTime);
            }
        }

        private static void UpdateLoop(double totalTime, double elapsedTime)
        {
            fpsTime += elapsedTime;
            if (fpsTime >= 1000.0)
            {
                framesPerSecond = frameCounter;
                frameCounter = 0;
                fpsTime -= 1000.0;
                if (framesPerSecond != prevFramesPerSecond)
                {
                    fpsText = null;
                    prevFramesPerSecond = framesPerSecond;
                }
            }
            foreach (SDLWindow window in windows)
            {
                if (window.HandleCreated)
                {
                    window.RaiseWindowUpdate(totalTime, elapsedTime);
                }
            }
        }

        private static void PaintLoop(double totalTime, double elapsedTime)
        {
            frameCounter++;
            foreach (SDLWindow window in windows)
            {
                if (window.HandleCreated)
                {
                    window.RaiseWindowPaint(totalTime, elapsedTime);
                }
            }
        }
        private static void Initialize(LogPriority logPriority)
        {
            string dllDir = Path.Combine(Environment.CurrentDirectory, IntPtr.Size == 4 ? "x86" : "x64");
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + dllDir);
            SDLLog.InitializeLog(logPriority);
            SDLLog.Info(LogCategory.APPLICATION, $"SDL Initialization Starting...");
            SDL_SetMainReady();
            _ = SDL_SetHint(SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");
            _ = SDL_Init(InitFlags.Everything);
            _ = SDL_SetHint(SDL_HINT_RENDER_DRIVER, "opengl");
            _ = SDL_SetHint(SDL_HINT_MOUSE_FOCUS_CLICKTHROUGH, "1");
            _ = SDL_SetHint(SDL_HINT_VIDEO_MINIMIZE_ON_FOCUS_LOSS, "0");
            _ = SDL_SetHint(SDL_HINT_RENDER_BATCHING, "1");
            _ = SDL_SetHint(SDL_HINT_GRAB_KEYBOARD, "1");
            _ = SDL_SetHint(SDL_HINT_ALLOW_ALT_TAB_WHILE_GRABBED, "1");
            _ = SDL_SetHint(SDL_BORDERLESS_WINDOWED_STYLE, "1");
            _ = SDL_SetHint(SDL_BORDERLESS_RESIZABLE_STYLE, "1");
            frequency = SDL_GetPerformanceFrequency();
            //_ = SDL_SetHint(SDL_HINT_RENDER_LOGICAL_SIZE_MODE, "0");
            //_ = SDL_SetHint(SDL_HINT_MOUSE_RELATIVE_SCALING, "1");
            GetDriverInfos();
            _ = SDLTexture.IMG_Init(SDLTexture.IMG_InitFlags.IMG_INIT_PNG);
            SDLInput.Initialize();
            SDLAudio.Initialize();
            _ = SDLFont.TTF_Init();
            defaultFont = SDLFont.LoadFont(Properties.Resources.Roboto_Regular, nameof(Properties.Resources.Roboto_Regular), 16);
            iconFont = SDLFont.LoadFont(Properties.Resources.entypo, nameof(Properties.Resources.entypo), 16);
            SDLLog.Info(LogCategory.APPLICATION, $"SDL Initialization Done...");
        }

        private static void Shutdown()
        {
            SDLLog.Info(LogCategory.APPLICATION, $"SDL Shutdown Starting...");
            defaultFont?.Dispose();
            defaultFont = null;
            iconFont?.Dispose();
            iconFont = null;
            fontTracker.Dispose();
            SDLFont.TTF_Quit();
            SDLAudio.Shutdown();
            SDLInput.Shutdown();
            SDLTexture.IMG_Quit();
            SDLLog.Info(LogCategory.APPLICATION, $"SDL Shutdown Done...");
            SDLLog.ShutdownLog();
            SDL_Quit();
        }
        public static void Delay(double ms)
        {
            if (ms > 0)
            {
                uint ums = (uint)ms;
                if (ums > 0)
                {
                    //SDLLog.Info($"Delay {ums} ms");
                    SDL_Delay(ums);
                }
            }
        }
        public static double GetCurrentTime()
        {
            ulong counter = SDL_GetPerformanceCounter();
            return 1000.0 * counter / frequency;
        }

        internal static string? IntPtr2String(IntPtr ptr, bool freePtr = false)
        {
            if (ptr == IntPtr.Zero) return null;
            string? result = Marshal.PtrToStringUTF8(ptr);
            if (freePtr) { SDL_free(ptr); }
            return result;
        }

        internal static void Track(SDLFont font)
        {
            fontTracker.Track(font);
        }

        internal static void Untrack(SDLFont font)
        {
            fontTracker.Untrack(font);
        }

        public const string SDL_HINT_FRAMEBUFFER_ACCELERATION = "SDL_FRAMEBUFFER_ACCELERATION";
        public const string SDL_HINT_RENDER_DRIVER = "SDL_RENDER_DRIVER";
        public const string SDL_HINT_RENDER_OPENGL_SHADERS = "SDL_RENDER_OPENGL_SHADERS";
        public const string SDL_HINT_RENDER_DIRECT3D_THREADSAFE = "SDL_RENDER_DIRECT3D_THREADSAFE";
        public const string SDL_HINT_RENDER_VSYNC = "SDL_RENDER_VSYNC";
        public const string SDL_HINT_VIDEO_X11_XVIDMODE = "SDL_VIDEO_X11_XVIDMODE";
        public const string SDL_HINT_VIDEO_X11_XINERAMA = "SDL_VIDEO_X11_XINERAMA";
        public const string SDL_HINT_VIDEO_X11_XRANDR = "SDL_VIDEO_X11_XRANDR";
        public const string SDL_HINT_GRAB_KEYBOARD = "SDL_GRAB_KEYBOARD";
        public const string SDL_HINT_VIDEO_MINIMIZE_ON_FOCUS_LOSS = "SDL_VIDEO_MINIMIZE_ON_FOCUS_LOSS";
        public const string SDL_HINT_IDLE_TIMER_DISABLED = "SDL_IOS_IDLE_TIMER_DISABLED";
        public const string SDL_HINT_ORIENTATIONS = "SDL_IOS_ORIENTATIONS";
        public const string SDL_HINT_XINPUT_ENABLED = "SDL_XINPUT_ENABLED";
        public const string SDL_HINT_GAMECONTROLLERCONFIG = "SDL_GAMECONTROLLERCONFIG";
        public const string SDL_HINT_JOYSTICK_ALLOW_BACKGROUND_EVENTS = "SDL_JOYSTICK_ALLOW_BACKGROUND_EVENTS";
        public const string SDL_HINT_ALLOW_TOPMOST = "SDL_ALLOW_TOPMOST";
        public const string SDL_HINT_TIMER_RESOLUTION = "SDL_TIMER_RESOLUTION";
        public const string SDL_HINT_RENDER_SCALE_QUALITY = "SDL_RENDER_SCALE_QUALITY";
        public const string SDL_HINT_VIDEO_HIGHDPI_DISABLED = "SDL_VIDEO_HIGHDPI_DISABLED";
        public const string SDL_HINT_CTRL_CLICK_EMULATE_RIGHT_CLICK = "SDL_CTRL_CLICK_EMULATE_RIGHT_CLICK";
        public const string SDL_HINT_VIDEO_WIN_D3DCOMPILER = "SDL_VIDEO_WIN_D3DCOMPILER";
        public const string SDL_HINT_MOUSE_RELATIVE_MODE_WARP = "SDL_MOUSE_RELATIVE_MODE_WARP";
        public const string SDL_HINT_VIDEO_WINDOW_SHARE_PIXEL_FORMAT = "SDL_VIDEO_WINDOW_SHARE_PIXEL_FORMAT";
        public const string SDL_HINT_VIDEO_ALLOW_SCREENSAVER = "SDL_VIDEO_ALLOW_SCREENSAVER";
        public const string SDL_HINT_ACCELEROMETER_AS_JOYSTICK = "SDL_ACCELEROMETER_AS_JOYSTICK";
        public const string SDL_HINT_VIDEO_MAC_FULLSCREEN_SPACES = "SDL_VIDEO_MAC_FULLSCREEN_SPACES";
        public const string SDL_HINT_WINRT_PRIVACY_POLICY_URL = "SDL_WINRT_PRIVACY_POLICY_URL";
        public const string SDL_HINT_WINRT_PRIVACY_POLICY_LABEL = "SDL_WINRT_PRIVACY_POLICY_LABEL";
        public const string SDL_HINT_WINRT_HANDLE_BACK_BUTTON = "SDL_WINRT_HANDLE_BACK_BUTTON";
        public const string SDL_HINT_NO_SIGNAL_HANDLERS = "SDL_NO_SIGNAL_HANDLERS";
        public const string SDL_HINT_IME_INTERNAL_EDITING = "SDL_IME_INTERNAL_EDITING";
        public const string SDL_HINT_ANDROID_SEPARATE_MOUSE_AND_TOUCH = "SDL_ANDROID_SEPARATE_MOUSE_AND_TOUCH";
        public const string SDL_HINT_EMSCRIPTEN_KEYBOARD_ELEMENT = "SDL_EMSCRIPTEN_KEYBOARD_ELEMENT";
        public const string SDL_HINT_THREAD_STACK_SIZE = "SDL_THREAD_STACK_SIZE";
        public const string SDL_HINT_WINDOW_FRAME_USABLE_WHILE_CURSOR_HIDDEN = "SDL_WINDOW_FRAME_USABLE_WHILE_CURSOR_HIDDEN";
        public const string SDL_HINT_WINDOWS_ENABLE_MESSAGELOOP = "SDL_WINDOWS_ENABLE_MESSAGELOOP";
        public const string SDL_HINT_WINDOWS_NO_CLOSE_ON_ALT_F4 = "SDL_WINDOWS_NO_CLOSE_ON_ALT_F4";
        public const string SDL_HINT_XINPUT_USE_OLD_JOYSTICK_MAPPING = "SDL_XINPUT_USE_OLD_JOYSTICK_MAPPING";
        public const string SDL_HINT_MAC_BACKGROUND_APP = "SDL_MAC_BACKGROUND_APP";
        public const string SDL_HINT_VIDEO_X11_NET_WM_PING = "SDL_VIDEO_X11_NET_WM_PING";
        public const string SDL_HINT_ANDROID_APK_EXPANSION_MAIN_FILE_VERSION = "SDL_ANDROID_APK_EXPANSION_MAIN_FILE_VERSION";
        public const string SDL_HINT_ANDROID_APK_EXPANSION_PATCH_FILE_VERSION = "SDL_ANDROID_APK_EXPANSION_PATCH_FILE_VERSION";
        public const string SDL_HINT_MOUSE_FOCUS_CLICKTHROUGH = "SDL_MOUSE_FOCUS_CLICKTHROUGH";
        public const string SDL_HINT_BMP_SAVE_LEGACY_FORMAT = "SDL_BMP_SAVE_LEGACY_FORMAT";
        public const string SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING = "SDL_WINDOWS_DISABLE_THREAD_NAMING";
        public const string SDL_HINT_APPLE_TV_REMOTE_ALLOW_ROTATION = "SDL_APPLE_TV_REMOTE_ALLOW_ROTATION";
        public const string SDL_HINT_AUDIO_RESAMPLING_MODE = "SDL_AUDIO_RESAMPLING_MODE";
        public const string SDL_HINT_RENDER_LOGICAL_SIZE_MODE = "SDL_RENDER_LOGICAL_SIZE_MODE";
        public const string SDL_HINT_MOUSE_NORMAL_SPEED_SCALE = "SDL_MOUSE_NORMAL_SPEED_SCALE";
        public const string SDL_HINT_MOUSE_RELATIVE_SPEED_SCALE = "SDL_MOUSE_RELATIVE_SPEED_SCALE";
        public const string SDL_HINT_TOUCH_MOUSE_EVENTS = "SDL_TOUCH_MOUSE_EVENTS";
        public const string SDL_HINT_WINDOWS_INTRESOURCE_ICON = "SDL_WINDOWS_INTRESOURCE_ICON";
        public const string SDL_HINT_WINDOWS_INTRESOURCE_ICON_SMALL = "SDL_WINDOWS_INTRESOURCE_ICON_SMALL";
        public const string SDL_HINT_IOS_HIDE_HOME_INDICATOR = "SDL_IOS_HIDE_HOME_INDICATOR";
        public const string SDL_HINT_TV_REMOTE_AS_JOYSTICK = "SDL_TV_REMOTE_AS_JOYSTICK";
        public const string SDL_VIDEO_X11_NET_WM_BYPASS_COMPOSITOR = "SDL_VIDEO_X11_NET_WM_BYPASS_COMPOSITOR";
        public const string SDL_HINT_MOUSE_DOUBLE_CLICK_TIME = "SDL_MOUSE_DOUBLE_CLICK_TIME";
        public const string SDL_HINT_MOUSE_DOUBLE_CLICK_RADIUS = "SDL_MOUSE_DOUBLE_CLICK_RADIUS";
        public const string SDL_HINT_JOYSTICK_HIDAPI = "SDL_JOYSTICK_HIDAPI";
        public const string SDL_HINT_JOYSTICK_HIDAPI_PS4 = "SDL_JOYSTICK_HIDAPI_PS4";
        public const string SDL_HINT_JOYSTICK_HIDAPI_PS4_RUMBLE = "SDL_JOYSTICK_HIDAPI_PS4_RUMBLE";
        public const string SDL_HINT_JOYSTICK_HIDAPI_STEAM = "SDL_JOYSTICK_HIDAPI_STEAM";
        public const string SDL_HINT_JOYSTICK_HIDAPI_SWITCH = "SDL_JOYSTICK_HIDAPI_SWITCH";
        public const string SDL_HINT_JOYSTICK_HIDAPI_XBOX = "SDL_JOYSTICK_HIDAPI_XBOX";
        public const string SDL_HINT_ENABLE_STEAM_CONTROLLERS = "SDL_ENABLE_STEAM_CONTROLLERS";
        public const string SDL_HINT_ANDROID_TRAP_BACK_BUTTON = "SDL_ANDROID_TRAP_BACK_BUTTON";
        public const string SDL_HINT_MOUSE_TOUCH_EVENTS = "SDL_MOUSE_TOUCH_EVENTS";
        public const string SDL_HINT_GAMECONTROLLERCONFIG_FILE = "SDL_GAMECONTROLLERCONFIG_FILE";
        public const string SDL_HINT_ANDROID_BLOCK_ON_PAUSE = "SDL_ANDROID_BLOCK_ON_PAUSE";
        public const string SDL_HINT_RENDER_BATCHING = "SDL_RENDER_BATCHING";
        public const string SDL_HINT_EVENT_LOGGING = "SDL_EVENT_LOGGING";
        public const string SDL_HINT_WAVE_RIFF_CHUNK_SIZE = "SDL_WAVE_RIFF_CHUNK_SIZE";
        public const string SDL_HINT_WAVE_TRUNCATION = "SDL_WAVE_TRUNCATION";
        public const string SDL_HINT_WAVE_FACT_CHUNK = "SDL_WAVE_FACT_CHUNK";
        public const string SDL_HINT_VIDO_X11_WINDOW_VISUALID = "SDL_VIDEO_X11_WINDOW_VISUALID";
        public const string SDL_HINT_GAMECONTROLLER_USE_BUTTON_LABELS = "SDL_GAMECONTROLLER_USE_BUTTON_LABELS";
        public const string SDL_HINT_VIDEO_EXTERNAL_CONTEXT = "SDL_VIDEO_EXTERNAL_CONTEXT";
        public const string SDL_HINT_JOYSTICK_HIDAPI_GAMECUBE = "SDL_JOYSTICK_HIDAPI_GAMECUBE";
        public const string SDL_HINT_DISPLAY_USABLE_BOUNDS = "SDL_DISPLAY_USABLE_BOUNDS";
        public const string SDL_HINT_VIDEO_X11_FORCE_EGL = "SDL_VIDEO_X11_FORCE_EGL";
        public const string SDL_HINT_GAMECONTROLLERTYPE = "SDL_GAMECONTROLLERTYPE";
        public const string SDL_HINT_JOYSTICK_RAWINPUT = "SDL_JOYSTICK_RAWINPUT";
        public const string SDL_HINT_AUDIO_DEVICE_APP_NAME = "SDL_AUDIO_DEVICE_APP_NAME";
        public const string SDL_HINT_AUDIO_DEVICE_STREAM_NAME = "SDL_AUDIO_DEVICE_STREAM_NAME";
        public const string SDL_HINT_PREFERRED_LOCALES = "SDL_PREFERRED_LOCALES";
        public const string SDL_HINT_THREAD_PRIORITY_POLICY = "SDL_THREAD_PRIORITY_POLICY";
        public const string SDL_HINT_EMSCRIPTEN_ASYNCIFY = "SDL_EMSCRIPTEN_ASYNCIFY";
        public const string SDL_HINT_LINUX_JOYSTICK_DEADZONES = "SDL_LINUX_JOYSTICK_DEADZONES";
        public const string SDL_HINT_ANDROID_BLOCK_ON_PAUSE_PAUSEAUDIO = "SDL_ANDROID_BLOCK_ON_PAUSE_PAUSEAUDIO";
        public const string SDL_HINT_JOYSTICK_HIDAPI_PS5 = "SDL_JOYSTICK_HIDAPI_PS5";
        public const string SDL_HINT_THREAD_FORCE_REALTIME_TIME_CRITICAL = "SDL_THREAD_FORCE_REALTIME_TIME_CRITICAL";
        public const string SDL_HINT_JOYSTICK_THREAD = "SDL_JOYSTICK_THREAD";
        public const string SDL_HINT_AUTO_UPDATE_JOYSTICKS = "SDL_AUTO_UPDATE_JOYSTICKS";
        public const string SDL_HINT_AUTO_UPDATE_SENSORS = "SDL_AUTO_UPDATE_SENSORS";
        public const string SDL_HINT_MOUSE_RELATIVE_SCALING = "SDL_MOUSE_RELATIVE_SCALING";
        public const string SDL_HINT_JOYSTICK_HIDAPI_PS5_RUMBLE = "SDL_JOYSTICK_HIDAPI_PS5_RUMBLE";
        public const string SDL_HINT_WINDOWS_FORCE_MUTEX_CRITICAL_SECTIONS = "SDL_WINDOWS_FORCE_MUTEX_CRITICAL_SECTIONS";
        public const string SDL_HINT_WINDOWS_FORCE_SEMAPHORE_KERNEL = "SDL_WINDOWS_FORCE_SEMAPHORE_KERNEL";
        public const string SDL_HINT_JOYSTICK_HIDAPI_PS5_PLAYER_LED = "SDL_JOYSTICK_HIDAPI_PS5_PLAYER_LED";
        public const string SDL_HINT_WINDOWS_USE_D3D9EX = "SDL_WINDOWS_USE_D3D9EX";
        public const string SDL_HINT_JOYSTICK_HIDAPI_JOY_CONS = "SDL_JOYSTICK_HIDAPI_JOY_CONS";
        public const string SDL_HINT_JOYSTICK_HIDAPI_STADIA = "SDL_JOYSTICK_HIDAPI_STADIA";
        public const string SDL_HINT_JOYSTICK_HIDAPI_SWITCH_HOME_LED = "SDL_JOYSTICK_HIDAPI_SWITCH_HOME_LED";
        public const string SDL_HINT_ALLOW_ALT_TAB_WHILE_GRABBED = "SDL_ALLOW_ALT_TAB_WHILE_GRABBED";
        public const string SDL_HINT_KMSDRM_REQUIRE_DRM_MASTER = "SDL_KMSDRM_REQUIRE_DRM_MASTER";
        public const string SDL_HINT_AUDIO_DEVICE_STREAM_ROLE = "SDL_AUDIO_DEVICE_STREAM_ROLE";
        public const string SDL_HINT_X11_FORCE_OVERRIDE_REDIRECT = "SDL_X11_FORCE_OVERRIDE_REDIRECT";
        public const string SDL_HINT_JOYSTICK_HIDAPI_LUNA = "SDL_JOYSTICK_HIDAPI_LUNA";
        public const string SDL_HINT_JOYSTICK_RAWINPUT_CORRELATE_XINPUT = "SDL_JOYSTICK_RAWINPUT_CORRELATE_XINPUT";
        public const string SDL_HINT_AUDIO_INCLUDE_MONITORS = "SDL_AUDIO_INCLUDE_MONITORS";
        public const string SDL_HINT_VIDEO_WAYLAND_ALLOW_LIBDECOR = "SDL_VIDEO_WAYLAND_ALLOW_LIBDECOR";
        public const string SDL_HINT_VIDEO_EGL_ALLOW_TRANSPARENCY = "SDL_VIDEO_EGL_ALLOW_TRANSPARENCY";
        public const string SDL_HINT_APP_NAME = "SDL_APP_NAME";
        public const string SDL_HINT_SCREENSAVER_INHIBIT_ACTIVITY_NAME = "SDL_SCREENSAVER_INHIBIT_ACTIVITY_NAME";
        public const string SDL_HINT_IME_SHOW_UI = "SDL_IME_SHOW_UI";
        public const string SDL_HINT_WINDOW_NO_ACTIVATION_WHEN_SHOWN = "SDL_WINDOW_NO_ACTIVATION_WHEN_SHOWN";
        public const string SDL_HINT_POLL_SENTINEL = "SDL_POLL_SENTINEL";
        public const string SDL_HINT_JOYSTICK_DEVICE = "SDL_JOYSTICK_DEVICE";
        public const string SDL_HINT_LINUX_JOYSTICK_CLASSIC = "SDL_LINUX_JOYSTICK_CLASSIC";
        public const string SDL_HINT_RENDER_LINE_METHOD = "SDL_RENDER_LINE_METHOD";
        public const string SDL_BORDERLESS_WINDOWED_STYLE = "SDL_BORDERLESS_WINDOWED_STYLE";
        public const string SDL_BORDERLESS_RESIZABLE_STYLE = "SDL_BORDERLESS_RESIZABLE_STYLE";


        private const string LibName = "SDL2";

        [Flags]
        private enum InitFlags : uint
        {
            Timer = 0x1,
            Audio = 0x10,
            Video = 0x20,
            Joystick = 0x200,
            Haptic = 0x1000,
            GameController = 0x2000,
            Events = 0x4000,
            Sensor = 0x8000,
            Everything = Timer | Audio | Video | Haptic | GameController | Events | Sensor
        };

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr SDL_malloc(IntPtr size);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SDL_free(IntPtr memblock);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr SDL_memcpy(IntPtr dst, IntPtr src, IntPtr len);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr SDL_RWFromMem(IntPtr mem, int size);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr SDL_RWFromMem([In()][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] mem, int size);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetMainReady();
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_Init(InitFlags flags);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_Quit();
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SDL_SetHint([In()][MarshalAs(UnmanagedType.LPUTF8Str)] string name, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string value);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_Delay(uint ms);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong SDL_GetPerformanceCounter();
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong SDL_GetPerformanceFrequency();
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetNumRenderDrivers();
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_GetPixelFormatName(uint format);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_GetError();
        internal static string? GetError()
        {
            return IntPtr2String(SDL_GetError());
        }


    }
}
