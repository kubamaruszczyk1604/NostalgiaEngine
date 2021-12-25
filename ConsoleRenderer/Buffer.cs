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


        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int nStdHandle);


        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfoEx ConsoleCurrentFontEx);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool GetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfoEx ConsoleCurrentFontEx);

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


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct FontInfoEx
        {
            internal int cbSize;
            internal int FontIndex;
            public short FontWidth;
            public short FontSize;
            public int FontFamily;
            public int FontWeight;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.wc, SizeConst = 32)]
            public string FontName;
        }

        static public bool HalfTemporalResolution { get; set; }
        static SafeFileHandle m_ConsoleHandle;
        static int m_sWidth;
        static int m_sHeight;
        static CharInfo[] m_Bufer;
        static SmallRect rect;
        static Coord wh;
        static Coord orgin;
        static int m_sBuffPtr;

        static public bool Initialize(short width, short height, short pixelW, short pixelH)
        {
            HalfTemporalResolution = false;
            m_sWidth = width;
            m_sHeight = height;
            wh = new Coord() { X = (short)m_sWidth, Y = (short)m_sHeight };
            orgin = new Coord() { X = 0, Y = 0 };
            m_sBuffPtr = 0;
            m_ConsoleHandle = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            if (m_ConsoleHandle.IsInvalid) return false;
            try
            {

                // ConsoleHelper.SetCurrentFont("Consolas", pixelSize);
                FontInfoEx set = new FontInfoEx
                {
                    cbSize = Marshal.SizeOf<FontInfoEx>(),
                    FontIndex = 0,
                    FontFamily = 0x00,
                    FontName = "Consolas",
                    FontWeight = 400,
                    FontSize = pixelW,
                    FontWidth = pixelH
                };
                SetCurrentConsoleFontEx(m_ConsoleHandle.DangerousGetHandle(), false, ref set);
                Console.SetWindowSize(width + 10, height + 4);
            }
            catch
            {
                return false;
            }
           

            
            m_Bufer = new CharInfo[width * height];
            rect = new SmallRect() { Left = 5, Top = 2, Right = (short)(width + 5), Bottom = (short)(height + 2) };
            
            Console.CursorVisible = false;
            Console.Clear();
            return true;
        }

        static public void AddSequentialy(char c, short color)
        {

            m_Bufer[m_sBuffPtr].Attributes = color;
            m_Bufer[m_sBuffPtr].Char.AsciiChar = (byte)c;
            m_sBuffPtr++;
            if (m_sBuffPtr >= m_Bufer.Length) m_sBuffPtr = 0;

        }

        static public void AddAsync(char c, short color, int x, int y)
        {

            int index = m_sWidth * (y) + x;
            if (index >= m_Bufer.Length)
            {
                index = 0;
                //throw new Exception("DLUGOSC JEST: " + index.ToString());
            }
            m_Bufer[index].Attributes = color;
            m_Bufer[index].Char.AsciiChar = (byte)c;

        }
        static int i = 0;
        static public void Swap()
        {
            i = 1 -i;
            if (HalfTemporalResolution)
            {
                if (i == 1) WriteConsoleOutput(m_ConsoleHandle, m_Bufer, wh, orgin, ref rect);
            }
            else
            {
                WriteConsoleOutput(m_ConsoleHandle, m_Bufer, wh, orgin, ref rect);
            }

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
            public short FontWidth;
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
                    //FontFamily = FixedWidthTrueType,
                   // FontName = font,
                    FontWeight = 400,
                    FontSize = fontSize > 0 ? fontSize : before.FontSize,
                    FontWidth = fontSize
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



}
