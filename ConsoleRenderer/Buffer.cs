using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using OpenTK;

namespace ConsoleRenderer
{
    class Buffer
    {



        /* TEXT FRAME BUFFER FOR FAST RENDERING */
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteConsoleOutput(
          SafeFileHandle hConsoleOutput,
          CharInfo[] lpBuffer,
          Coord dwBufferSize,
          Coord dwBufferCoord,
          ref SmallRect lpWriteRegion);

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;

            public Coord(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct CharUnion
        {
            [FieldOffset(0)]
            public char UnicodeChar;
            [FieldOffset(0)]
            public byte AsciiChar;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct CharInfo
        {
            [FieldOffset(0)]
            public CharUnion Char;
            [FieldOffset(2)]
            public short Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        static SafeFileHandle m_ConsoleHandle;
        static int m_sWidth;
        static int m_sHeight;
        static CharInfo[] buf;
        static SmallRect rect;

        static int m_sBuffPtr;

        static public bool Initialize(short width, short height, short pixelSize)
        {
            m_sWidth = width;
            m_sHeight = height;
            m_sBuffPtr = 0;
            try
            {

                ConsoleHelper.SetCurrentFont("Consolas", pixelSize);
                Console.SetWindowSize(width + 10, height + 4);
            }
            catch
            {
                return false;
            }
           
            m_ConsoleHandle = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            if (m_ConsoleHandle.IsInvalid) return false;
            
            buf = new CharInfo[width * height];
            rect = new SmallRect() { Left = 5, Top = 2, Right = (short)(width + 5), Bottom = (short)(height + 2) };
            
            Console.CursorVisible = false;
            Console.Clear();
            return true;
        }

        static public void AddSequentialy(char c, short color)
        {

            buf[m_sBuffPtr].Attributes = color;
            buf[m_sBuffPtr].Char.AsciiChar = (byte)c;
            m_sBuffPtr++;
            if (m_sBuffPtr >= buf.Length) m_sBuffPtr = 0;

        }

        static public void AddAsync(char c, short color, int x, int y)
        {


            int index = m_sWidth * (y) + x;
            if (index >= buf.Length)
            {
                index = 0;
                //throw new Exception("DLUGOSC JEST: " + index.ToString());
            }
            buf[index].Attributes = color;
            buf[index].Char.AsciiChar = (byte)c;

        }

        static public void Swap()
        {
            WriteConsoleOutput(m_ConsoleHandle, buf,
                         new Coord() { X = (short)m_sWidth, Y = (short)m_sHeight },
                         new Coord() { X = 0, Y = 0 },
                         ref rect);

            m_sBuffPtr = 0;
        }

    }


    static class ConsoleHelper
    {
        private const int FixedWidthTrueType = 54;
        private const int StandardOutputHandle = -11;

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int nStdHandle);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfo ConsoleCurrentFontEx);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool GetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfo ConsoleCurrentFontEx);


        private static readonly IntPtr ConsoleOutputHandle = GetStdHandle(StandardOutputHandle);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct FontInfo
        {
            internal int cbSize;
            internal int FontIndex;
            internal short FontWidth;
            public short FontSize;
            public int FontFamily;
            public int FontWeight;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.wc, SizeConst = 32)]
            public string FontName;
        }

        public static FontInfo[] SetCurrentFont(string font, short fontSize = 0)
        {
            Console.WriteLine("Set Current Font: " + font);

            FontInfo before = new FontInfo
            {
                cbSize = Marshal.SizeOf<FontInfo>()
            };

            if (GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref before))
            {

                FontInfo set = new FontInfo
                {
                    cbSize = Marshal.SizeOf<FontInfo>(),
                    FontIndex = 0,
                    FontFamily = FixedWidthTrueType,
                    FontName = font,
                    FontWeight = 400,
                    FontSize = fontSize > 0 ? fontSize : before.FontSize
                };

                // Get some settings from current font.
                if (!SetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref set))
                {
                    var ex = Marshal.GetLastWin32Error();
                    Console.WriteLine("Set error " + ex);
                    throw new System.ComponentModel.Win32Exception(ex);
                }

                FontInfo after = new FontInfo
                {
                    cbSize = Marshal.SizeOf<FontInfo>()
                };
                GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref after);

                return new[] { before, set, after };
            }
            else
            {
                var er = Marshal.GetLastWin32Error();
                Console.WriteLine("Get error " + er);
                throw new System.ComponentModel.Win32Exception(er);
            }
        }
    }

    //class SetScreenColors
    //{
    //    [StructLayout(LayoutKind.Sequential)]
    //    internal struct COORD
    //    {
    //        internal short X;
    //        internal short Y;
    //    }

    //    [StructLayout(LayoutKind.Sequential)]
    //    internal struct SMALL_RECT
    //    {
    //        internal short Left;
    //        internal short Top;
    //        internal short Right;
    //        internal short Bottom;
    //    }

    //    [StructLayout(LayoutKind.Sequential)]
    //    internal struct COLORREF
    //    {
    //        internal uint ColorDWORD;

    //        internal COLORREF(Vector3 color)
    //        {
    //            ColorDWORD = (uint)color.X + (((uint)color.Y) << 8) + (((uint)color.Z) << 16);
    //        }

    //        internal COLORREF(uint r, uint g, uint b)
    //        {
    //            ColorDWORD = r + (g << 8) + (b << 16);
    //        }

    //        internal Vector3 GetColor()
    //        {
    //            return new Vector3( (int)(0x000000FFU & ColorDWORD),
    //                                  (int)(0x0000FF00U & ColorDWORD) >> 8, (int)(0x00FF0000U & ColorDWORD) >> 16);
    //        }

    //        internal void SetColor(Vector3 color)
    //        {
    //            ColorDWORD = (uint)color.X + (((uint)color.Y) << 8) + (((uint)color.Z) << 16);
    //        }
    //    }

    //    [StructLayout(LayoutKind.Sequential)]
    //    internal struct CONSOLE_SCREEN_BUFFER_INFO_EX
    //    {
    //        internal int cbSize;
    //        internal COORD dwSize;
    //        internal COORD dwCursorPosition;
    //        internal ushort wAttributes;
    //        internal SMALL_RECT srWindow;
    //        internal COORD dwMaximumWindowSize;
    //        internal ushort wPopupAttributes;
    //        internal bool bFullscreenSupported;
    //        internal COLORREF black;
    //        internal COLORREF darkBlue;
    //        internal COLORREF darkGreen;
    //        internal COLORREF darkCyan;
    //        internal COLORREF darkRed;
    //        internal COLORREF darkMagenta;
    //        internal COLORREF darkYellow;
    //        internal COLORREF gray;
    //        internal COLORREF darkGray;
    //        internal COLORREF blue;
    //        internal COLORREF green;
    //        internal COLORREF cyan;
    //        internal COLORREF red;
    //        internal COLORREF magenta;
    //        internal COLORREF yellow;
    //        internal COLORREF white;
    //    }

    //    const int STD_OUTPUT_HANDLE = -11;                                        // per WinBase.h
    //    internal static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);    // per WinBase.h

    //    [DllImport("kernel32.dll", SetLastError = true)]
    //    private static extern IntPtr GetStdHandle(int nStdHandle);

    //    [DllImport("kernel32.dll", SetLastError = true)]
    //    private static extern bool GetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

    //    [DllImport("kernel32.dll", SetLastError = true)]
    //    private static extern bool SetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

    //    // Set a specific console color to an RGB color
    //    // The default console colors used are gray (foreground) and black (background)
    //    public static int SetColor(ConsoleColor consoleColor, Vector3 targetColor)
    //    {
    //        return SetColor(consoleColor, (uint)targetColor.X, (uint)targetColor.Y, (uint)targetColor.Z);
    //    }

    //    public static int SetColor(ConsoleColor color, uint r, uint g, uint b)
    //    {
    //        CONSOLE_SCREEN_BUFFER_INFO_EX csbe = new CONSOLE_SCREEN_BUFFER_INFO_EX();
    //        csbe.cbSize = (int)Marshal.SizeOf(csbe);                    // 96 = 0x60
    //        IntPtr hConsoleOutput = GetStdHandle(STD_OUTPUT_HANDLE);    // 7
    //        if (hConsoleOutput == INVALID_HANDLE_VALUE)
    //        {
    //            return Marshal.GetLastWin32Error();
    //        }
    //        bool brc = GetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
    //        if (!brc)
    //        {
    //            return Marshal.GetLastWin32Error();
    //        }

    //        switch (color)
    //        {
    //            case ConsoleColor.Black:
    //                csbe.black = new COLORREF(r, g, b);
    //                break;
    //            case ConsoleColor.DarkBlue:
    //                csbe.darkBlue = new COLORREF(r, g, b);
    //                break;
    //            case ConsoleColor.DarkGreen:
    //                csbe.darkGreen = new COLORREF(r, g, b);
    //                break;
    //            case ConsoleColor.DarkCyan:
    //                csbe.darkCyan = new COLORREF(r, g, b);
    //                break;
    //            case ConsoleColor.DarkRed:
    //                csbe.darkRed = new COLORREF(r, g, b);
    //                break;
    //            case ConsoleColor.DarkMagenta:
    //                csbe.darkMagenta = new COLORREF(r, g, b);
    //                break;
    //            case ConsoleColor.DarkYellow:
    //                csbe.darkYellow = new COLORREF(r, g, b);
    //                break;
    //            case ConsoleColor.Gray:
    //                csbe.gray = new COLORREF(r, g, b);
    //                break;
    //            case ConsoleColor.DarkGray:
    //                csbe.darkGray = new COLORREF(r, g, b);
    //                break;
    //            case ConsoleColor.Blue:
    //                csbe.blue = new COLORREF(r, g, b);
    //                break;
    //            case ConsoleColor.Green:
    //                csbe.green = new COLORREF(r, g, b);
    //                break;
    //            case ConsoleColor.Cyan:
    //                csbe.cyan = new COLORREF(r, g, b);
    //                break;
    //            case ConsoleColor.Red:
    //                csbe.red = new COLORREF(r, g, b);
    //                break;
    //            case ConsoleColor.Magenta:
    //                csbe.magenta = new COLORREF(r, g, b);
    //                break;
    //            case ConsoleColor.Yellow:
    //                csbe.yellow = new COLORREF(r, g, b);
    //                break;
    //            case ConsoleColor.White:
    //                csbe.white = new COLORREF(r, g, b);
    //                break;
    //        }
    //        ++csbe.srWindow.Bottom;
    //        ++csbe.srWindow.Right;
    //        brc = SetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
    //        if (!brc)
    //        {
    //            return Marshal.GetLastWin32Error();
    //        }
    //        return 0;
    //    }

    //    public static int SetScreenColor(Vector3 foregroundColor, Vector3 backgroundColor)
    //    {
    //        int irc;
    //        irc = SetColor(ConsoleColor.Gray, foregroundColor);
    //        if (irc != 0) return irc;
    //        irc = SetColor(ConsoleColor.Black, backgroundColor);
    //        if (irc != 0) return irc;

    //        return 0;
    //    }
    //}

}
