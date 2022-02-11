// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;

using System;
using System.Runtime.InteropServices;

public static class SDLLog
{
    private static readonly SDL_LogOutputFunction logFunc = LogOutputFunc;
    private static bool logToConsole = true;
    private static LogPriority logPriority;
    private static string[] prioText = new string[(int)LogPriority.Max];
    private static string[] catText = new string[(int)LogCategory.MAX];

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
    public static void Log(LogCategory cat, LogPriority prio, string format, object? arg0)
    {
        if (!WillLog(cat, prio)) return;
        SDL_LogMessage((int)cat, prio, string.Format(format, arg0));
    }
    public static void Log(LogCategory cat, LogPriority prio, string format, object? arg0, object? arg1)
    {
        if (!WillLog(cat, prio)) return;
        SDL_LogMessage((int)cat, prio, string.Format(format, arg0, arg1));
    }
    public static void Log(LogCategory cat, LogPriority prio, string format, object? arg0, object? arg1, object? arg2)
    {
        if (!WillLog(cat, prio)) return;
        SDL_LogMessage((int)cat, prio, string.Format(format, arg0, arg1, arg2));
    }
    public static void Log(LogCategory cat, LogPriority prio, string format, object? arg0, object? arg1, object? arg2, object? arg3)
    {
        if (!WillLog(cat, prio)) return;
        SDL_LogMessage((int)cat, prio, string.Format(format, arg0, arg1, arg2, arg3));
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
    public static void Verbose(LogCategory cat, string format, object? arg0)
    {
        Log(cat, LogPriority.Verbose, format, arg0);
    }
    public static void Verbose(LogCategory cat, string format, object? arg0, object? arg1)
    {
        Log(cat, LogPriority.Verbose, format, arg0, arg1);
    }
    public static void Verbose(LogCategory cat, string format, object? arg0, object? arg1, object? arg2)
    {
        Log(cat, LogPriority.Verbose, format, arg0, arg1, arg2);
    }
    public static void Verbose(LogCategory cat, string format, object? arg0, object? arg1, object? arg2, object? arg3)
    {
        Log(cat, LogPriority.Verbose, format, arg0, arg1, arg2, arg3);
    }
    public static void Verbose(LogCategory cat, string format, params object?[] args)
    {
        Log(cat, LogPriority.Verbose, format, args);
    }

    public static void Debug(LogCategory cat, string msg)
    {
        Log(cat, LogPriority.Debug, msg);
    }
    public static void Debug(LogCategory cat, string format, object? arg0)
    {
        Log(cat, LogPriority.Debug, format, arg0);
    }
    public static void Debug(LogCategory cat, string format, object? arg0, object? arg1)
    {
        Log(cat, LogPriority.Debug, format, arg0, arg1);
    }
    public static void Debug(LogCategory cat, string format, object? arg0, object? arg1, object? arg2)
    {
        Log(cat, LogPriority.Debug, format, arg0, arg1, arg2);
    }
    public static void Debug(LogCategory cat, string format, params object?[] args)
    {
        Log(cat, LogPriority.Debug, format, args);
    }

    public static void Info(LogCategory cat, string msg)
    {
        Log(cat, LogPriority.Info, msg);
    }
    public static void Info(LogCategory cat, string format, object? arg0)
    {
        Log(cat, LogPriority.Info, format, arg0);
    }
    public static void Info(LogCategory cat, string format, object? arg0, object? arg1)
    {
        Log(cat, LogPriority.Info, format, arg0, arg1);
    }
    public static void Info(LogCategory cat, string format, object? arg0, object? arg1, object? arg2)
    {
        Log(cat, LogPriority.Info, format, arg0, arg1, arg2);
    }
    public static void Info(LogCategory cat, string format, params object?[] args)
    {
        Log(cat, LogPriority.Info, format, args);
    }

    public static void Warn(LogCategory cat, string msg)
    {
        Log(cat, LogPriority.Warn, msg);
    }
    public static void Warn(LogCategory cat, string format, object? arg0)
    {
        Log(cat, LogPriority.Warn, format, arg0);
    }
    public static void Warn(LogCategory cat, string format, object? arg0, object? arg1)
    {
        Log(cat, LogPriority.Warn, format, arg0, arg1);
    }
    public static void Warn(LogCategory cat, string format, object? arg0, object? arg1, object? arg2)
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
    public static void Error(LogCategory cat, string format, object? arg0)
    {
        Log(cat, LogPriority.Error, format, arg0);
    }
    public static void Error(LogCategory cat, string format, object? arg0, object? arg1)
    {
        Log(cat, LogPriority.Error, format, arg0, arg1);
    }
    public static void Error(LogCategory cat, string format, object? arg0, object? arg1, object? arg2)
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
            Console.Write(DateTime.Now);
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
