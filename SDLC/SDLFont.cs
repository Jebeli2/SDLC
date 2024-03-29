﻿// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

public class SDLFont : SDLObject
{
    private static readonly StringBuilder stringBuffer = new(512);
    private static int nextFontId;
    private readonly int fontId;
    private readonly IntPtr mem;
    private readonly int ySize;
    private FontStyle fontStyle;
    private int fontOutline;
    private FontHinting fontHinting;
    private int fontHeight;
    private int fontAscent;
    private int fontDescent;
    private int fontLineSkip;
    private int fontKerning;
    private string familyName;
    private string styleName;
    private bool disposedValue;

    internal SDLFont(IntPtr handle, int ySize)
        : this(handle, ySize, IntPtr.Zero)
    {

    }
    internal SDLFont(IntPtr handle, int ySize, IntPtr mem)
        : base(handle, BuildFontName(handle, ySize))
    {
        fontId = ++nextFontId;
        this.mem = mem;
        this.ySize = ySize;
        fontStyle = (FontStyle)TTF_GetFontStyle(this.handle);
        fontOutline = TTF_GetFontOutline(this.handle);
        fontHinting = (FontHinting)TTF_GetFontHinting(this.handle);
        fontHeight = TTF_FontHeight(this.handle);
        fontAscent = TTF_FontAscent(this.handle);
        fontDescent = TTF_FontDescent(this.handle);
        fontLineSkip = TTF_FontLineSkip(this.handle);
        fontKerning = TTF_GetFontKerning(this.handle);
        familyName = SDLApplication.IntPtr2String(TTF_FontFaceFamilyName(this.handle)) ?? "unknown";
        styleName = SDLApplication.IntPtr2String(TTF_FontFaceStyleName(this.handle)) ?? "regular";
        SDLApplication.Track(this);
    }

    public static string BuildFontName(IntPtr fontHandle, int ySize)
    {
        string familyName = SDLApplication.IntPtr2String(TTF_FontFaceFamilyName(fontHandle)) ?? "unknown";
        string styleName = SDLApplication.IntPtr2String(TTF_FontFaceStyleName(fontHandle)) ?? "regular";
        return familyName + "-" + styleName + "-" + ySize;
    }
    public int FontId => fontId;
    public int YSize => ySize;
    public string FamilyName => familyName;
    public string StyleName => styleName;
    public FontStyle FontStyle => fontStyle;
    public int Outline => fontOutline;
    public FontHinting Hinting => fontHinting;
    public int Height => fontHeight;
    public int Ascent => fontAscent;
    public int Descent => fontDescent;
    public int LineSkip => fontLineSkip;
    public int Kerning => fontKerning;

    public Size MeasureText(ReadOnlySpan<char> text)
    {
        int w = 0;
        int h = 0;
        if (text != null && text.Length > 0)
        {
            stringBuffer.Clear();
            stringBuffer.Append(text);
            _ = TTF_SizeUTF8(handle, stringBuffer, out w, out h);
        }
        return new Size(w, h);
    }

    public void GetGlyphMetrics(char c, out int minx, out int maxx, out int miny, out int maxy, out int advance)
    {
        _ = TTF_GlyphMetrics32(handle, (uint)c, out minx, out maxx, out miny, out maxy, out advance);
    }
    public static SDLFont? LoadFont(string fileName, int ptSize)
    {
        SDLFont? font = null;
        if (!string.IsNullOrEmpty(fileName))
        {
            IntPtr fnt = TTF_OpenFont(fileName, ptSize);
            if (fnt != IntPtr.Zero)
            {
                font = new SDLFont(fnt, ptSize);
                SDLLog.Verbose(LogCategory.FONT, "Font {0} loaded from file '{1}'", font.Name, fileName);
            }
        }
        return font;
    }

    public static SDLFont? LoadFont(byte[] data, string name, int ptSize)
    {
        SDLFont? font = null;
        if (data != null)
        {
            int size = data.Length;
            IntPtr mem = Marshal.AllocHGlobal(size);
            Marshal.Copy(data, 0, mem, size);
            IntPtr rw = SDLApplication.SDL_RWFromMem(mem, size);
            IntPtr handle = TTF_OpenFontRW(rw, 1, ptSize);
            if (handle != IntPtr.Zero)
            {
                font = new SDLFont(handle, ptSize, mem);
                SDLLog.Verbose(LogCategory.FONT, "Font {0} loaded from resource '{1}'", font.Name, name);
            }
        }
        return font;
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                SDLApplication.Untrack(this);
            }
            if (handle != IntPtr.Zero)
            {
                TTF_CloseFont(handle);
            }
            if (mem != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(mem);
            }
            disposedValue = true;
        }
        base.Dispose(disposing);
    }
    private const string LibName = "SDL2_ttf";


    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int TTF_Init();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void TTF_Quit();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int TTF_WasInit();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    internal static extern IntPtr TTF_OpenFont([In()][MarshalAs(UnmanagedType.LPUTF8Str)] string file, int ptsize);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr TTF_OpenFontRW(IntPtr src, int freesrc, int ptsize);


    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void TTF_CloseFont(IntPtr font);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr TTF_FontFaceFamilyName(IntPtr font);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr TTF_FontFaceStyleName(IntPtr font);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int TTF_FontFaceIsFixedWidth(IntPtr font);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void TTF_SetFontKerning(IntPtr font, int allowed);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int TTF_GetFontKerning(IntPtr font);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int TTF_FontLineSkip(IntPtr font);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int TTF_FontDescent(IntPtr font);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int TTF_FontAscent(IntPtr font);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int TTF_FontHeight(IntPtr font);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int TTF_GetFontHinting(IntPtr font);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void TTF_SetFontHinting(IntPtr font, int hinting);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int TTF_GetFontOutline(IntPtr font);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void TTF_SetFontOutline(IntPtr font, int outline);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int TTF_GetFontStyle(IntPtr font);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void TTF_SetFontStyle(IntPtr font, int style);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int TTF_SetFontSize(IntPtr font, int ptsize);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    internal static extern IntPtr TTF_RenderUTF8_Solid(IntPtr font, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string text, int fg);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    internal static extern IntPtr TTF_RenderUTF8_Shaded(IntPtr font, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string text, int fg, int bg);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    internal static extern IntPtr TTF_RenderUTF8_Blended(IntPtr font, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string text, int fg);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    internal static extern IntPtr TTF_RenderUTF8_Blended(IntPtr font, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] StringBuilder text, int fg);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    internal static extern int TTF_MeasureUTF8(IntPtr font, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string text, int measure_width, out int extent, out int count);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    internal static extern int TTF_MeasureUTF8(IntPtr font, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] StringBuilder text, int measure_width, out int extent, out int count);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    internal static extern int TTF_SizeUTF8(IntPtr font, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string text, out int w, out int h);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    internal static extern int TTF_SizeUTF8(IntPtr font, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] StringBuilder text, out int w, out int h);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr TTF_RenderGlyph_Blended(IntPtr font, ushort ch, int fg);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int TTF_GlyphMetrics(IntPtr font, ushort ch, out int minx, out int maxx, out int miny, out int maxy, out int advance);
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int TTF_GlyphMetrics32(IntPtr font, uint ch, out int minx, out int maxx, out int miny, out int maxy, out int advance);


}
