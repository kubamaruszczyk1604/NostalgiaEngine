using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using OpenTK;
using System.Threading;
namespace ConsoleRenderer.Core
{
    public class NEScreen
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
          NEPoint dwBufferSize,
          NEPoint dwBufferCoord,
          ref NERect lpWriteRegion);


        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int nStdHandle);


        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfoEx ConsoleCurrentFontEx);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool GetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfoEx ConsoleCurrentFontEx);


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
            public string FontName;
        }

        static public bool HalfTemporalResolution { get; set; }
        static SafeFileHandle m_ConsoleHandle;
        static int m_sWidth;
        static int m_sHeight;
        static CharInfo[] m_Bufer;
        static NERect m_ConsoleRect;
        static NEPoint m_ScrTopLeft;
        static NEPoint m_ScrBottomRight;
        static int m_sBuffPtr;

        static public bool Initialize(short width, short height, short pixelW, short pixelH)
        {
            HalfTemporalResolution = false;
            m_sWidth = width;
            m_sHeight = height;
            m_ScrBottomRight = new NEPoint() { X = (short)m_sWidth, Y = (short)m_sHeight };
            m_ScrTopLeft = new NEPoint() { X = 0, Y = 0 };
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
                    FontSize = pixelH,
                    FontWidth = pixelW
                };
                SetCurrentConsoleFontEx(m_ConsoleHandle.DangerousGetHandle(), false, ref set);
                Console.SetWindowSize(width + 10, height + 4);
            }
            catch
            {
                return false;
            }
           

            
            m_Bufer = new CharInfo[width * height];
            m_ConsoleRect = new NERect() { Left = 5, Top = 2, Right = (short)(width + 5), Bottom = (short)(height + 2) };
            
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

        static public void PutChar(char c, short color, int x, int y)
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

        static public void WriteXY(int x, int y, short col, string line)
        {
            for (int i  = 0; i < line.Length;++i)
            {
                PutChar(line[i], col, x + i, y);
            }
        }

        static public void Clear()
        {
            Array.Clear(m_Bufer, 0, m_Bufer.Length);
        }

  
        static public void Swap()
        {


             WriteConsoleOutput(m_ConsoleHandle, m_Bufer, m_ScrBottomRight, m_ScrTopLeft, ref m_ConsoleRect);
             //WriteCon(0, 0, 320, 1);

            m_sBuffPtr = 0;
        }

        static private void WriteCon(short startX, short startY, short w, short h)
        {
            NERect rect = new NERect((short)(startX+5), (short)(startY+2), (short)(w+5), (short)(h+2)) ;
            WriteConsoleOutput(m_ConsoleHandle, m_Bufer, new NEPoint(w,h), new NEPoint(startX, startY), ref rect);
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
