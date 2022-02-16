// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;

using System;
using System.Runtime.InteropServices;

/// <summary>
/// Log to SDL log facility and back to app again.
/// Has a lot of generic overloads to delay boxing of values for formatted messages
/// until they are actually formatted. Values that are not logged (because of their
/// LogPriority) will never be boxed at all. If they are logged, boxing will currently
/// still happen, because the standard string.Format method is used.
/// Looking into allocation free string formatters...
/// </summary>
public static class SDLLog
{
    private static readonly SDL_LogOutputFunction logFunc = LogOutputFunc;
    private static bool logToConsole = true;
    private static LogPriority logPriority;
    private static readonly string[] prioText = new string[(int)LogPriority.Max];
    private static readonly string[] catText = new string[(int)LogCategory.MAX];

    public static bool LogToConsole
    {
        get => logToConsole;
        set => logToConsole = value;
    }

    public static void InitializeLog(LogPriority prio)
    {
        logPriority = prio;
        for (int i = 0; i < prioText.Length; i++) { prioText[i] = string.Format("{0,-8}", ((LogPriority)i).ToString().ToUpperInvariant()); }
        for (int i = 0; i < catText.Length; i++) { catText[i] = string.Format("{0,-11}", ((LogCategory)i).ToString().ToUpperInvariant()); }
        SDL_LogSetOutputFunction(logFunc, IntPtr.Zero);
        SDL_LogSetAllPriority(prio);
    }

    public static void ShutdownLog()
    {
        SDL_LogSetOutputFunction(IntPtr.Zero, IntPtr.Zero);
        SDL_LogResetPriorities();
    }

    public static bool WillLog(LogCategory cat, LogPriority prio)
    {
        if (prio >= logPriority)
        {
            return true;
        }
        return false;
    }
    public static void Log(LogCategory cat, LogPriority prio, string msg)
    {
        if (!WillLog(cat, prio)) return;
        SDL_LogMessage((int)cat, prio, msg);
    }
    public static void Log<T0>(LogCategory cat, LogPriority prio, string format, T0 arg0)
    {
        if (!WillLog(cat, prio)) return;
        SDL_LogMessage((int)cat, prio, string.Format(format, arg0));
    }
    public static void Log<T0, T1>(LogCategory cat, LogPriority prio, string format, T0 arg0, T1 arg1)
    {
        if (!WillLog(cat, prio)) return;
        SDL_LogMessage((int)cat, prio, string.Format(format, arg0, arg1));
    }
    public static void Log<T0, T1, T2>(LogCategory cat, LogPriority prio, string format, T0 arg0, T1 arg1, T2 arg2)
    {
        if (!WillLog(cat, prio)) return;
        SDL_LogMessage((int)cat, prio, string.Format(format, arg0, arg1, arg2));
    }
    public static void Log<T0, T1, T2, T3>(LogCategory cat, LogPriority prio, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
    {
        if (!WillLog(cat, prio)) return;
        SDL_LogMessage((int)cat, prio, string.Format(format, arg0, arg1, arg2, arg3));
    }
    public static void Log<T0, T1, T2, T3, T4>(LogCategory cat, LogPriority prio, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (!WillLog(cat, prio)) return;
        SDL_LogMessage((int)cat, prio, string.Format(format, arg0, arg1, arg2, arg3, arg4));
    }
    public static void Log<T0, T1, T2, T3, T4, T5>(LogCategory cat, LogPriority prio, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        if (!WillLog(cat, prio)) return;
        SDL_LogMessage((int)cat, prio, string.Format(format, arg0, arg1, arg2, arg3, arg4, arg5));
    }
    public static void Log<T0, T1, T2, T3, T4, T5, T6>(LogCategory cat, LogPriority prio, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
    {
        if (!WillLog(cat, prio)) return;
        SDL_LogMessage((int)cat, prio, string.Format(format, arg0, arg1, arg2, arg3, arg4, arg5, arg6));
    }
    public static void Log(LogCategory cat, LogPriority prio, string format, params object?[] args)
    {
        if (!WillLog(cat, prio)) return;
        SDL_LogMessage((int)cat, prio, string.Format(format, args));
    }
    public static void Verbose(LogCategory cat, string msg)
    {
        Log(cat, LogPriority.Verbose, msg);
    }
    public static void Verbose<T0>(LogCategory cat, string format, T0 arg0)
    {
        Log(cat, LogPriority.Verbose, format, arg0);
    }
    public static void Verbose<T0, T1>(LogCategory cat, string format, T0 arg0, T1 arg1)
    {
        Log(cat, LogPriority.Verbose, format, arg0, arg1);
    }
    public static void Verbose<T0, T1, T2>(LogCategory cat, string format, T0 arg0, T1 arg1, T2 arg2)
    {
        Log(cat, LogPriority.Verbose, format, arg0, arg1, arg2);
    }
    public static void Verbose<T0, T1, T2, T3>(LogCategory cat, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
    {
        Log(cat, LogPriority.Verbose, format, arg0, arg1, arg2, arg3);
    }
    public static void Verbose<T0, T1, T2, T3, T4>(LogCategory cat, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        Log(cat, LogPriority.Verbose, format, arg0, arg1, arg2, arg3, arg4);
    }
    public static void Verbose<T0, T1, T2, T3, T4, T5>(LogCategory cat, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        Log(cat, LogPriority.Verbose, format, arg0, arg1, arg2, arg3, arg4, arg5);
    }
    public static void Verbose<T0, T1, T2, T3, T4, T5, T6>(LogCategory cat, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
    {
        Log(cat, LogPriority.Verbose, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
    }
    public static void Verbose(LogCategory cat, string format, params object?[] args)
    {
        Log(cat, LogPriority.Verbose, format, args);
    }

    public static void Debug(LogCategory cat, string msg)
    {
        Log(cat, LogPriority.Debug, msg);
    }
    public static void Debug<T0>(LogCategory cat, string format, T0 arg0)
    {
        Log(cat, LogPriority.Debug, format, arg0);
    }
    public static void Debug<T0, T1>(LogCategory cat, string format, T0 arg0, T1 arg1)
    {
        Log(cat, LogPriority.Debug, format, arg0, arg1);
    }
    public static void Debug<T0, T1, T2>(LogCategory cat, string format, T0 arg0, T1 arg1, T2 arg2)
    {
        Log(cat, LogPriority.Debug, format, arg0, arg1, arg2);
    }
    public static void Debug<T0, T1, T2, T3>(LogCategory cat, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
    {
        Log(cat, LogPriority.Debug, format, arg0, arg1, arg2, arg3);
    }
    public static void Debug(LogCategory cat, string format, params object?[] args)
    {
        Log(cat, LogPriority.Debug, format, args);
    }

    public static void Info(LogCategory cat, string msg)
    {
        Log(cat, LogPriority.Info, msg);
    }
    public static void Info<T0>(LogCategory cat, string format, T0 arg0)
    {
        Log(cat, LogPriority.Info, format, arg0);
    }
    public static void Info<T0, T1>(LogCategory cat, string format, T0 arg0, T1 arg1)
    {
        Log(cat, LogPriority.Info, format, arg0, arg1);
    }
    public static void Info<T0, T1, T2>(LogCategory cat, string format, T0 arg0, T1 arg1, T2 arg2)
    {
        Log(cat, LogPriority.Info, format, arg0, arg1, arg2);
    }
    public static void Info<T0, T1, T2, T3>(LogCategory cat, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
    {
        Log(cat, LogPriority.Info, format, arg0, arg1, arg2, arg3);
    }
    public static void Info(LogCategory cat, string format, params object?[] args)
    {
        Log(cat, LogPriority.Info, format, args);
    }

    public static void Warn(LogCategory cat, string msg)
    {
        Log(cat, LogPriority.Warn, msg);
    }
    public static void Warn<T0>(LogCategory cat, string format, T0 arg0)
    {
        Log(cat, LogPriority.Warn, format, arg0);
    }
    public static void Warn<T0, T1>(LogCategory cat, string format, T0 arg0, T1 arg1)
    {
        Log(cat, LogPriority.Warn, format, arg0, arg1);
    }
    public static void Warn<T0, T1, T2>(LogCategory cat, string format, T0 arg0, T1 arg1, T2 arg2)
    {
        Log(cat, LogPriority.Warn, format, arg0, arg1, arg2);
    }
    public static void Warn(LogCategory cat, string format, params object?[] args)
    {
        Log(cat, LogPriority.Warn, format, args);
    }

    public static void Error(LogCategory cat, string msg)
    {
        Log(cat, LogPriority.Error, msg);
    }
    public static void Error<T0>(LogCategory cat, string format, T0 arg0)
    {
        Log(cat, LogPriority.Error, format, arg0);
    }
    public static void Error<T0, T1>(LogCategory cat, string format, T0 arg0, T1 arg1)
    {
        Log(cat, LogPriority.Error, format, arg0, arg1);
    }
    public static void Error<T0, T1, T2>(LogCategory cat, string format, T0 arg0, T1 arg1, T2 arg2)
    {
        Log(cat, LogPriority.Error, format, arg0, arg1, arg2);
    }
    public static void Error(LogCategory cat, string format, params object?[] args)
    {
        Log(cat, LogPriority.Error, format, args);
    }

    public static void Critical(LogCategory cat, string msg)
    {
        Log(cat, LogPriority.Critical, msg);
    }
    public static void Critical(LogCategory cat, string format, object? arg0)
    {
        Log(cat, LogPriority.Critical, format, arg0);
    }
    public static void Critical(LogCategory cat, string format, object? arg0, object? arg1)
    {
        Log(cat, LogPriority.Critical, format, arg0, arg1);
    }
    public static void Critical(LogCategory cat, string format, object? arg0, object? arg1, object? arg2)
    {
        Log(cat, LogPriority.Critical, format, arg0, arg1, arg2);
    }
    public static void Critical(LogCategory cat, string format, params object?[] args)
    {
        Log(cat, LogPriority.Critical, format, args);
    }

    private static void LogOutputFunc(IntPtr userData, int category, LogPriority priority, IntPtr message)
    {
        if (logToConsole)
        {
            ClearConsoleColor();
            Console.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            Console.Write(' ');
            SetConsoleColor(priority);
            Console.Write(prioText[(int)priority]);
            Console.Write(' ');
            Console.Write(catText[category]);
            Console.Write(' ');
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
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private static extern void SDL_Log([MarshalAs(UnmanagedType.LPUTF8Str)] string msg);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private static extern void SDL_LogVerbose(int category, [MarshalAs(UnmanagedType.LPUTF8Str)] string msg);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private static extern void SDL_LogDebug(int category, [MarshalAs(UnmanagedType.LPUTF8Str)] string msg);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private static extern void SDL_LogInfo(int category, [MarshalAs(UnmanagedType.LPUTF8Str)] string msg);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private static extern void SDL_LogWarn(int category, [MarshalAs(UnmanagedType.LPUTF8Str)] string msg);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private static extern void SDL_LogError(int category, [MarshalAs(UnmanagedType.LPUTF8Str)] string msg);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private static extern void SDL_LogCritical(int category, [MarshalAs(UnmanagedType.LPUTF8Str)] string msg);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private static extern void SDL_LogMessage(int category, LogPriority priority, [MarshalAs(UnmanagedType.LPUTF8Str)] string msg);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private static extern void SDL_LogMessageV(int category, LogPriority priority, [MarshalAs(UnmanagedType.LPUTF8Str)] string msg);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
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
