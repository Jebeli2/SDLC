namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    public static class SDLLog
    {
        private static readonly SDL_LogOutputFunction logFunc = LogOutputFunc;
        private static bool logToConsole = true;
        public enum LogPriority
        {
            Verbose = 1,
            Debug,
            Info,
            Warn,
            Error,
            Critical
        }

        public static bool LogToConsole
        {
            get => logToConsole;
            set => logToConsole = value;
        }

        public static void InitializeLog(LogPriority prio)
        {
            SDL_LogSetOutputFunction(logFunc, IntPtr.Zero);
            SDL_LogSetAllPriority(prio);
        }

        public static void ShutdownLog()
        {
            SDL_LogSetOutputFunction(IntPtr.Zero, IntPtr.Zero);
            SDL_LogResetPriorities();
        }
        public static void Log(LogPriority prio, string msg)
        {
            SDL_LogMessage(LOG_CAT_SDLC, prio, msg);
        }
        public static void Verbose(string msg)
        {
            SDL_LogVerbose(LOG_CAT_SDLC, msg);
        }
        public static void Debug(string msg)
        {
            SDL_LogDebug(LOG_CAT_SDLC, msg);
        }
        public static void Info(string msg)
        {
            SDL_LogInfo(LOG_CAT_SDLC, msg);
        }
        public static void Warn(string msg)
        {
            SDL_LogWarn(LOG_CAT_SDLC, msg);
        }
        public static void Error(string msg)
        {
            SDL_LogError(LOG_CAT_SDLC, msg);
        }
        public static void Critical(string msg)
        {
            SDL_LogCritical(LOG_CAT_SDLC, msg);
        }

        private static void LogOutputFunc(IntPtr userData, int category, LogPriority priority, IntPtr message)
        {
            if (logToConsole)
            {
                Console.Write(DateTime.Now);
                Console.Write(" ");
                Console.Write(Cat2String(category));
                Console.Write(" ");
                Console.Write(priority);
                Console.Write(": ");
                Console.WriteLine(Marshal.PtrToStringUTF8(message));
            }
        }

        private static string Cat2String(int category)
        {
            if (category == LOG_CAT_SDLC) return "SDLC";
            switch (category)
            {
                case (int)SDL_LogCategory.SDL_LOG_CATEGORY_APPLICATION: return "APPLICATION";
                case (int)SDL_LogCategory.SDL_LOG_CATEGORY_ERROR: return "ERROR";
                case (int)SDL_LogCategory.SDL_LOG_CATEGORY_ASSERT: return "ASSERT";
                case (int)SDL_LogCategory.SDL_LOG_CATEGORY_SYSTEM: return "SYSTEM";
                case (int)SDL_LogCategory.SDL_LOG_CATEGORY_AUDIO: return "AUDIO";
                case (int)SDL_LogCategory.SDL_LOG_CATEGORY_RENDER: return "RENDER";
                case (int)SDL_LogCategory.SDL_LOG_CATEGORY_INPUT: return "INPUT";
                case (int)SDL_LogCategory.SDL_LOG_CATEGORY_TEST: return "TEST";
            }
            return "CAT" + category;
        }

        private const string LibName = "SDL2";

        private const int LOG_CAT_SDLC = (int)SDL_LogCategory.SDL_LOG_CATEGORY_CUSTOM + 1;

        private enum SDL_LogCategory
        {
            SDL_LOG_CATEGORY_APPLICATION,
            SDL_LOG_CATEGORY_ERROR,
            SDL_LOG_CATEGORY_ASSERT,
            SDL_LOG_CATEGORY_SYSTEM,
            SDL_LOG_CATEGORY_AUDIO,
            SDL_LOG_CATEGORY_VIDEO,
            SDL_LOG_CATEGORY_RENDER,
            SDL_LOG_CATEGORY_INPUT,
            SDL_LOG_CATEGORY_TEST,
            SDL_LOG_CATEGORY_RESERVED1,
            SDL_LOG_CATEGORY_RESERVED2,
            SDL_LOG_CATEGORY_RESERVED3,
            SDL_LOG_CATEGORY_RESERVED4,
            SDL_LOG_CATEGORY_RESERVED5,
            SDL_LOG_CATEGORY_RESERVED6,
            SDL_LOG_CATEGORY_RESERVED7,
            SDL_LOG_CATEGORY_RESERVED8,
            SDL_LOG_CATEGORY_RESERVED9,
            SDL_LOG_CATEGORY_RESERVED10,
            SDL_LOG_CATEGORY_CUSTOM
        }


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SDL_LogOutputFunction(IntPtr userdata, int category, LogPriority priority, IntPtr message);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_Log([MarshalAs(UnmanagedType.LPUTF8Str)] string msg);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_LogVerbose(int category, [MarshalAs(UnmanagedType.LPUTF8Str)] string msg);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_LogDebug(int category, [MarshalAs(UnmanagedType.LPUTF8Str)] string msg);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_LogInfo(int category, [MarshalAs(UnmanagedType.LPUTF8Str)] string msg);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_LogWarn(int category, [MarshalAs(UnmanagedType.LPUTF8Str)] string msg);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_LogError(int category, [MarshalAs(UnmanagedType.LPUTF8Str)] string msg);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_LogCritical(int category, [MarshalAs(UnmanagedType.LPUTF8Str)] string msg);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_LogMessage(int category, LogPriority priority, [MarshalAs(UnmanagedType.LPUTF8Str)] string msg);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_LogMessageV(int category, LogPriority priority, [MarshalAs(UnmanagedType.LPUTF8Str)] string msg);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern LogPriority SDL_LogGetPriority(int category);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_LogSetPriority(int category, LogPriority priority);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_LogSetAllPriority(LogPriority priority);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_LogResetPriorities();
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_LogSetOutputFunction(SDL_LogOutputFunction callback, IntPtr userdata);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_LogSetOutputFunction(IntPtr callback, IntPtr userdata);

    }
}
