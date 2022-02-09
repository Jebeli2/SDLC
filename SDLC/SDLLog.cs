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
        public static void Log(LogCategory cat, LogPriority prio, string msg)
        {
            SDL_LogMessage((int)cat, prio, msg);
        }
        public static void Log(LogPriority prio, string msg)
        {
            SDL_LogMessage((int)LogCategory.SDLC, prio, msg);
        }
        public static void Verbose(LogCategory cat, string msg)
        {
            SDL_LogVerbose((int)cat, msg);
        }
        public static void Debug(LogCategory cat, string msg)
        {
            SDL_LogDebug((int)cat, msg);
        }
        public static void Info(LogCategory cat, string msg)
        {
            SDL_LogInfo((int)cat, msg);
        }
        public static void Warn(LogCategory cat, string msg)
        {
            SDL_LogWarn((int)cat, msg);
        }
        public static void Error(LogCategory cat, string msg)
        {
            SDL_LogError((int)cat, msg);
        }
        public static void Critical(LogCategory cat, string msg)
        {
            SDL_LogCritical((int)cat, msg);
        }
        private static void LogOutputFunc(IntPtr userData, int category, LogPriority priority, IntPtr message)
        {
            if (logToConsole)
            {
                ClearConsoleColor();
                Console.Write(DateTime.Now);
                SetConsoleColor(priority);
                Console.Write("{0,8} ", priority.ToString().ToUpperInvariant());
                Console.Write(" {0,11} ", (LogCategory)category);
                ClearConsoleColor();
                Console.WriteLine(Marshal.PtrToStringUTF8(message));
            }
        }

        private static void ClearConsoleColor()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }
        private static void SetConsoleColor(LogPriority level)
        {
            switch (level)
            {
                case LogPriority.Error:
                case LogPriority.Critical:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogPriority.Warn:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case LogPriority.Debug:
                case LogPriority.Verbose:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogPriority.Info:
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }


        private const string LibName = "SDL2";


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
