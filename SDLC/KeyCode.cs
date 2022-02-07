﻿namespace SDLC
{
    public enum KeyCode
    {
        SCANCODE_MASK = (1 << 30),
        UNKNOWN = 0,

        RETURN = '\r',
        ESCAPE = 27, 
        BACKSPACE = '\b',
        TAB = '\t',
        SPACE = ' ',
        EXCLAIM = '!',
        QUOTEDBL = '"',
        HASH = '#',
        PERCENT = '%',
        DOLLAR = '$',
        AMPERSAND = '&',
        QUOTE = '\'',
        LEFTPAREN = '(',
        RIGHTPAREN = ')',
        ASTERISK = '*',
        PLUS = '+',
        COMMA = ',',
        MINUS = '-',
        PERIOD = '.',
        SLASH = '/',
        NUM0 = '0',
        NUM1 = '1',
        NUM2 = '2',
        NUM3 = '3',
        NUM4 = '4',
        NUM5 = '5',
        NUM6 = '6',
        NUM7 = '7',
        NUM8 = '8',
        NUM9 = '9',
        COLON = ':',
        SEMICOLON = ';',
        LESS = '<',
        EQUALS = '=',
        GREATER = '>',
        QUESTION = '?',
        AT = '@',
        LEFTBRACKET = '[',
        BACKSLASH = '\\',
        RIGHTBRACKET = ']',
        CARET = '^',
        UNDERSCORE = '_',
        BACKQUOTE = '`',
        a = 'a',
        b = 'b',
        c = 'c',
        d = 'd',
        e = 'e',
        f = 'f',
        g = 'g',
        h = 'h',
        i = 'i',
        j = 'j',
        k = 'k',
        l = 'l',
        m = 'm',
        n = 'n',
        o = 'o',
        p = 'p',
        q = 'q',
        r = 'r',
        s = 's',
        t = 't',
        u = 'u',
        v = 'v',
        w = 'w',
        x = 'x',
        y = 'y',
        z = 'z',

        CAPSLOCK = (int)ScanCode.SCANCODE_CAPSLOCK | SCANCODE_MASK,

        F1 = (int)ScanCode.SCANCODE_F1 | SCANCODE_MASK,
        F2 = (int)ScanCode.SCANCODE_F2 | SCANCODE_MASK,
        F3 = (int)ScanCode.SCANCODE_F3 | SCANCODE_MASK,
        F4 = (int)ScanCode.SCANCODE_F4 | SCANCODE_MASK,
        F5 = (int)ScanCode.SCANCODE_F5 | SCANCODE_MASK,
        F6 = (int)ScanCode.SCANCODE_F6 | SCANCODE_MASK,
        F7 = (int)ScanCode.SCANCODE_F7 | SCANCODE_MASK,
        F8 = (int)ScanCode.SCANCODE_F8 | SCANCODE_MASK,
        F9 = (int)ScanCode.SCANCODE_F9 | SCANCODE_MASK,
        F10 = (int)ScanCode.SCANCODE_F10 | SCANCODE_MASK,
        F11 = (int)ScanCode.SCANCODE_F11 | SCANCODE_MASK,
        F12 = (int)ScanCode.SCANCODE_F12 | SCANCODE_MASK,

        PRINTSCREEN = (int)ScanCode.SCANCODE_PRINTSCREEN | SCANCODE_MASK,
        SCROLLLOCK = (int)ScanCode.SCANCODE_SCROLLLOCK | SCANCODE_MASK,
        PAUSE = (int)ScanCode.SCANCODE_PAUSE | SCANCODE_MASK,
        INSERT = (int)ScanCode.SCANCODE_INSERT | SCANCODE_MASK,
        HOME = (int)ScanCode.SCANCODE_HOME | SCANCODE_MASK,
        PAGEUP = (int)ScanCode.SCANCODE_PAGEUP | SCANCODE_MASK,
        DELETE = 127,
        END = (int)ScanCode.SCANCODE_END | SCANCODE_MASK,
        PAGEDOWN = (int)ScanCode.SCANCODE_PAGEDOWN | SCANCODE_MASK,
        RIGHT = (int)ScanCode.SCANCODE_RIGHT | SCANCODE_MASK,
        LEFT = (int)ScanCode.SCANCODE_LEFT | SCANCODE_MASK,
        DOWN = (int)ScanCode.SCANCODE_DOWN | SCANCODE_MASK,
        UP = (int)ScanCode.SCANCODE_UP | SCANCODE_MASK,

        NUMLOCKCLEAR = (int)ScanCode.SCANCODE_NUMLOCKCLEAR | SCANCODE_MASK,
        KP_DIVIDE = (int)ScanCode.SCANCODE_KP_DIVIDE | SCANCODE_MASK,
        KP_MULTIPLY = (int)ScanCode.SCANCODE_KP_MULTIPLY | SCANCODE_MASK,
        KP_MINUS = (int)ScanCode.SCANCODE_KP_MINUS | SCANCODE_MASK,
        KP_PLUS = (int)ScanCode.SCANCODE_KP_PLUS | SCANCODE_MASK,
        KP_ENTER = (int)ScanCode.SCANCODE_KP_ENTER | SCANCODE_MASK,
        KP_1 = (int)ScanCode.SCANCODE_KP_1 | SCANCODE_MASK,
        KP_2 = (int)ScanCode.SCANCODE_KP_2 | SCANCODE_MASK,
        KP_3 = (int)ScanCode.SCANCODE_KP_3 | SCANCODE_MASK,
        KP_4 = (int)ScanCode.SCANCODE_KP_4 | SCANCODE_MASK,
        KP_5 = (int)ScanCode.SCANCODE_KP_5 | SCANCODE_MASK,
        KP_6 = (int)ScanCode.SCANCODE_KP_6 | SCANCODE_MASK,
        KP_7 = (int)ScanCode.SCANCODE_KP_7 | SCANCODE_MASK,
        KP_8 = (int)ScanCode.SCANCODE_KP_8 | SCANCODE_MASK,
        KP_9 = (int)ScanCode.SCANCODE_KP_9 | SCANCODE_MASK,
        KP_0 = (int)ScanCode.SCANCODE_KP_0 | SCANCODE_MASK,
        KP_PERIOD = (int)ScanCode.SCANCODE_KP_PERIOD | SCANCODE_MASK,

        APPLICATION = (int)ScanCode.SCANCODE_APPLICATION | SCANCODE_MASK,
        POWER = (int)ScanCode.SCANCODE_POWER | SCANCODE_MASK,
        KP_EQUALS = (int)ScanCode.SCANCODE_KP_EQUALS | SCANCODE_MASK,
        F13 = (int)ScanCode.SCANCODE_F13 | SCANCODE_MASK,
        F14 = (int)ScanCode.SCANCODE_F14 | SCANCODE_MASK,
        F15 = (int)ScanCode.SCANCODE_F15 | SCANCODE_MASK,
        F16 = (int)ScanCode.SCANCODE_F16 | SCANCODE_MASK,
        F17 = (int)ScanCode.SCANCODE_F17 | SCANCODE_MASK,
        F18 = (int)ScanCode.SCANCODE_F18 | SCANCODE_MASK,
        F19 = (int)ScanCode.SCANCODE_F19 | SCANCODE_MASK,
        F20 = (int)ScanCode.SCANCODE_F20 | SCANCODE_MASK,
        F21 = (int)ScanCode.SCANCODE_F21 | SCANCODE_MASK,
        F22 = (int)ScanCode.SCANCODE_F22 | SCANCODE_MASK,
        F23 = (int)ScanCode.SCANCODE_F23 | SCANCODE_MASK,
        F24 = (int)ScanCode.SCANCODE_F24 | SCANCODE_MASK,
        EXECUTE = (int)ScanCode.SCANCODE_EXECUTE | SCANCODE_MASK,
        HELP = (int)ScanCode.SCANCODE_HELP | SCANCODE_MASK,
        MENU = (int)ScanCode.SCANCODE_MENU | SCANCODE_MASK,
        SELECT = (int)ScanCode.SCANCODE_SELECT | SCANCODE_MASK,
        STOP = (int)ScanCode.SCANCODE_STOP | SCANCODE_MASK,
        AGAIN = (int)ScanCode.SCANCODE_AGAIN | SCANCODE_MASK,
        UNDO = (int)ScanCode.SCANCODE_UNDO | SCANCODE_MASK,
        CUT = (int)ScanCode.SCANCODE_CUT | SCANCODE_MASK,
        COPY = (int)ScanCode.SCANCODE_COPY | SCANCODE_MASK,
        PASTE = (int)ScanCode.SCANCODE_PASTE | SCANCODE_MASK,
        FIND = (int)ScanCode.SCANCODE_FIND | SCANCODE_MASK,
        MUTE = (int)ScanCode.SCANCODE_MUTE | SCANCODE_MASK,
        VOLUMEUP = (int)ScanCode.SCANCODE_VOLUMEUP | SCANCODE_MASK,
        VOLUMEDOWN = (int)ScanCode.SCANCODE_VOLUMEDOWN | SCANCODE_MASK,
        KP_COMMA = (int)ScanCode.SCANCODE_KP_COMMA | SCANCODE_MASK,
        KP_EQUALSAS400 = (int)ScanCode.SCANCODE_KP_EQUALSAS400 | SCANCODE_MASK,
        ALTERASE = (int)ScanCode.SCANCODE_ALTERASE | SCANCODE_MASK,
        SYSREQ = (int)ScanCode.SCANCODE_SYSREQ | SCANCODE_MASK,
        CANCEL = (int)ScanCode.SCANCODE_CANCEL | SCANCODE_MASK,
        CLEAR = (int)ScanCode.SCANCODE_CLEAR | SCANCODE_MASK,
        PRIOR = (int)ScanCode.SCANCODE_PRIOR | SCANCODE_MASK,
        RETURN2 = (int)ScanCode.SCANCODE_RETURN2 | SCANCODE_MASK,
        SEPARATOR = (int)ScanCode.SCANCODE_SEPARATOR | SCANCODE_MASK,
        OUT = (int)ScanCode.SCANCODE_OUT | SCANCODE_MASK,
        OPER = (int)ScanCode.SCANCODE_OPER | SCANCODE_MASK,
        CLEARAGAIN = (int)ScanCode.SCANCODE_CLEARAGAIN | SCANCODE_MASK,
        CRSEL = (int)ScanCode.SCANCODE_CRSEL | SCANCODE_MASK,
        EXSEL = (int)ScanCode.SCANCODE_EXSEL | SCANCODE_MASK,
        KP_00 = (int)ScanCode.SCANCODE_KP_00 | SCANCODE_MASK,
        KP_000 = (int)ScanCode.SCANCODE_KP_000 | SCANCODE_MASK,
        THOUSANDSSEPARATOR = (int)ScanCode.SCANCODE_THOUSANDSSEPARATOR | SCANCODE_MASK,
        DECIMALSEPARATOR = (int)ScanCode.SCANCODE_DECIMALSEPARATOR | SCANCODE_MASK,
        CURRENCYUNIT = (int)ScanCode.SCANCODE_CURRENCYUNIT | SCANCODE_MASK,
        CURRENCYSUBUNIT = (int)ScanCode.SCANCODE_CURRENCYSUBUNIT | SCANCODE_MASK,
        KP_LEFTPAREN = (int)ScanCode.SCANCODE_KP_LEFTPAREN | SCANCODE_MASK,
        KP_RIGHTPAREN = (int)ScanCode.SCANCODE_KP_RIGHTPAREN | SCANCODE_MASK,
        KP_LEFTBRACE = (int)ScanCode.SCANCODE_KP_LEFTBRACE | SCANCODE_MASK,
        KP_RIGHTBRACE = (int)ScanCode.SCANCODE_KP_RIGHTBRACE | SCANCODE_MASK,
        KP_TAB = (int)ScanCode.SCANCODE_KP_TAB | SCANCODE_MASK,
        KP_BACKSPACE = (int)ScanCode.SCANCODE_KP_BACKSPACE | SCANCODE_MASK,
        KP_A = (int)ScanCode.SCANCODE_KP_A | SCANCODE_MASK,
        KP_B = (int)ScanCode.SCANCODE_KP_B | SCANCODE_MASK,
        KP_C = (int)ScanCode.SCANCODE_KP_C | SCANCODE_MASK,
        KP_D = (int)ScanCode.SCANCODE_KP_D | SCANCODE_MASK,
        KP_E = (int)ScanCode.SCANCODE_KP_E | SCANCODE_MASK,
        KP_F = (int)ScanCode.SCANCODE_KP_F | SCANCODE_MASK,
        KP_XOR = (int)ScanCode.SCANCODE_KP_XOR | SCANCODE_MASK,
        KP_POWER = (int)ScanCode.SCANCODE_KP_POWER | SCANCODE_MASK,
        KP_PERCENT = (int)ScanCode.SCANCODE_KP_PERCENT | SCANCODE_MASK,
        KP_LESS = (int)ScanCode.SCANCODE_KP_LESS | SCANCODE_MASK,
        KP_GREATER = (int)ScanCode.SCANCODE_KP_GREATER | SCANCODE_MASK,
        KP_AMPERSAND = (int)ScanCode.SCANCODE_KP_AMPERSAND | SCANCODE_MASK,
        KP_DBLAMPERSAND = (int)ScanCode.SCANCODE_KP_DBLAMPERSAND | SCANCODE_MASK,
        KP_VERTICALBAR = (int)ScanCode.SCANCODE_KP_VERTICALBAR | SCANCODE_MASK,
        KP_DBLVERTICALBAR = (int)ScanCode.SCANCODE_KP_DBLVERTICALBAR | SCANCODE_MASK,
        KP_COLON = (int)ScanCode.SCANCODE_KP_COLON | SCANCODE_MASK,
        KP_HASH = (int)ScanCode.SCANCODE_KP_HASH | SCANCODE_MASK,
        KP_SPACE = (int)ScanCode.SCANCODE_KP_SPACE | SCANCODE_MASK,
        KP_AT = (int)ScanCode.SCANCODE_KP_AT | SCANCODE_MASK,
        KP_EXCLAM = (int)ScanCode.SCANCODE_KP_EXCLAM | SCANCODE_MASK,
        KP_MEMSTORE = (int)ScanCode.SCANCODE_KP_MEMSTORE | SCANCODE_MASK,
        KP_MEMRECALL = (int)ScanCode.SCANCODE_KP_MEMRECALL | SCANCODE_MASK,
        KP_MEMCLEAR = (int)ScanCode.SCANCODE_KP_MEMCLEAR | SCANCODE_MASK,
        KP_MEMADD = (int)ScanCode.SCANCODE_KP_MEMADD | SCANCODE_MASK,
        KP_MEMSUBTRACT = (int)ScanCode.SCANCODE_KP_MEMSUBTRACT | SCANCODE_MASK,
        KP_MEMMULTIPLY = (int)ScanCode.SCANCODE_KP_MEMMULTIPLY | SCANCODE_MASK,
        KP_MEMDIVIDE = (int)ScanCode.SCANCODE_KP_MEMDIVIDE | SCANCODE_MASK,
        KP_PLUSMINUS = (int)ScanCode.SCANCODE_KP_PLUSMINUS | SCANCODE_MASK,
        KP_CLEAR = (int)ScanCode.SCANCODE_KP_CLEAR | SCANCODE_MASK,
        KP_CLEARENTRY = (int)ScanCode.SCANCODE_KP_CLEARENTRY | SCANCODE_MASK,
        KP_BINARY = (int)ScanCode.SCANCODE_KP_BINARY | SCANCODE_MASK,
        KP_OCTAL = (int)ScanCode.SCANCODE_KP_OCTAL | SCANCODE_MASK,
        KP_DECIMAL = (int)ScanCode.SCANCODE_KP_DECIMAL | SCANCODE_MASK,
        KP_HEXADECIMAL = (int)ScanCode.SCANCODE_KP_HEXADECIMAL | SCANCODE_MASK,
        LCTRL = (int)ScanCode.SCANCODE_LCTRL | SCANCODE_MASK,
        LSHIFT = (int)ScanCode.SCANCODE_LSHIFT | SCANCODE_MASK,
        LALT = (int)ScanCode.SCANCODE_LALT | SCANCODE_MASK,
        LGUI = (int)ScanCode.SCANCODE_LGUI | SCANCODE_MASK,
        RCTRL = (int)ScanCode.SCANCODE_RCTRL | SCANCODE_MASK,
        RSHIFT = (int)ScanCode.SCANCODE_RSHIFT | SCANCODE_MASK,
        RALT = (int)ScanCode.SCANCODE_RALT | SCANCODE_MASK,
        RGUI = (int)ScanCode.SCANCODE_RGUI | SCANCODE_MASK,
        MODE = (int)ScanCode.SCANCODE_MODE | SCANCODE_MASK,
        AUDIONEXT = (int)ScanCode.SCANCODE_AUDIONEXT | SCANCODE_MASK,
        AUDIOPREV = (int)ScanCode.SCANCODE_AUDIOPREV | SCANCODE_MASK,
        AUDIOSTOP = (int)ScanCode.SCANCODE_AUDIOSTOP | SCANCODE_MASK,
        AUDIOPLAY = (int)ScanCode.SCANCODE_AUDIOPLAY | SCANCODE_MASK,
        AUDIOMUTE = (int)ScanCode.SCANCODE_AUDIOMUTE | SCANCODE_MASK,
        MEDIASELECT = (int)ScanCode.SCANCODE_MEDIASELECT | SCANCODE_MASK,
        WWW = (int)ScanCode.SCANCODE_WWW | SCANCODE_MASK,
        MAIL = (int)ScanCode.SCANCODE_MAIL | SCANCODE_MASK,
        CALCULATOR = (int)ScanCode.SCANCODE_CALCULATOR | SCANCODE_MASK,
        COMPUTER = (int)ScanCode.SCANCODE_COMPUTER | SCANCODE_MASK,
        AC_SEARCH = (int)ScanCode.SCANCODE_AC_SEARCH | SCANCODE_MASK,
        AC_HOME = (int)ScanCode.SCANCODE_AC_HOME | SCANCODE_MASK,
        AC_BACK = (int)ScanCode.SCANCODE_AC_BACK | SCANCODE_MASK,
        AC_FORWARD = (int)ScanCode.SCANCODE_AC_FORWARD | SCANCODE_MASK,
        AC_STOP = (int)ScanCode.SCANCODE_AC_STOP | SCANCODE_MASK,
        AC_REFRESH = (int)ScanCode.SCANCODE_AC_REFRESH | SCANCODE_MASK,
        AC_BOOKMARKS = (int)ScanCode.SCANCODE_AC_BOOKMARKS | SCANCODE_MASK,
        BRIGHTNESSDOWN = (int)ScanCode.SCANCODE_BRIGHTNESSDOWN | SCANCODE_MASK,
        BRIGHTNESSUP = (int)ScanCode.SCANCODE_BRIGHTNESSUP | SCANCODE_MASK,
        DISPLAYSWITCH = (int)ScanCode.SCANCODE_DISPLAYSWITCH | SCANCODE_MASK,
        KBDILLUMTOGGLE = (int)ScanCode.SCANCODE_KBDILLUMTOGGLE | SCANCODE_MASK,
        KBDILLUMDOWN = (int)ScanCode.SCANCODE_KBDILLUMDOWN | SCANCODE_MASK,
        KBDILLUMUP = (int)ScanCode.SCANCODE_KBDILLUMUP | SCANCODE_MASK,
        EJECT = (int)ScanCode.SCANCODE_EJECT | SCANCODE_MASK,
        SLEEP = (int)ScanCode.SCANCODE_SLEEP | SCANCODE_MASK,
        APP1 = (int)ScanCode.SCANCODE_APP1 | SCANCODE_MASK,
        APP2 = (int)ScanCode.SCANCODE_APP2 | SCANCODE_MASK,
        AUDIOREWIND = (int)ScanCode.SCANCODE_AUDIOREWIND | SCANCODE_MASK,
        AUDIOFASTFORWARD = (int)ScanCode.SCANCODE_AUDIOFASTFORWARD | SCANCODE_MASK,
    }
}
