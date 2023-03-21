using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.Collections.Generic;
namespace NostalgiaEngine.Core
{
    public class NEScreenBuffer
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


        static private readonly object LOCK = new object();
        static private SafeFileHandle m_ConsoleHandle;
        static private int m_sWidth;
        static private int m_sHeight;
        static private List<CharInfo[]> m_Bufer;
        static private NERect m_ConsoleRect;
        static private NEPoint m_ScrTopLeft;
        static private NEPoint m_ScrBottomRight;
        static private int m_WriteBufferPtr;
        static private int m_DrawBufferPtr;
        static private bool m_MultiThreadEnabled;

        static private int m_InitialW;
        static private int m_InitialH;
        static private bool m_FirstRun = true;
        static private bool m_SwapRequestedFlag;

        static private Thread m_ConsoleDrawWorker;

        static public bool Initialize(short width, short height, short pixelW, short pixelH, bool renderOnSeparateThread = true)
        {
            m_MultiThreadEnabled = renderOnSeparateThread;
            if (m_ConsoleDrawWorker != null) m_ConsoleDrawWorker.Abort();

            m_sWidth = width;
            m_sHeight = height;
            m_ScrBottomRight = new NEPoint() { X = (short)m_sWidth, Y = (short)m_sHeight };
            m_ScrTopLeft = new NEPoint() { X = 0, Y = 0 };
            m_ConsoleHandle = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            if (m_ConsoleHandle.IsInvalid) return false;


            if (m_FirstRun)
            {
                m_InitialW = Console.WindowWidth;
                m_InitialH = Console.WindowHeight;
            }
            m_FirstRun = false;
            try
            {
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
                Console.SetWindowSize(width + 2, height + 4);
                //Console.SetBufferSize(width + 2, height + 4);
            }
            catch
            {
                SetDefaultConsole();
                return false;
            }
            m_Bufer = new List<CharInfo[]>(2);
            m_Bufer.Add(new CharInfo[width * height]);
            m_Bufer.Add(new CharInfo[width * height]);
            m_ConsoleRect = new NERect() { Left = 1, Top = 2, Right = (short)(width + 1), Bottom = (short)(height + 2) };

            Console.CursorVisible = false;
            Console.Clear();
            m_WriteBufferPtr = 0;
            m_DrawBufferPtr = 0;
            if (m_MultiThreadEnabled)
            {
                m_ConsoleDrawWorker = new Thread(new ThreadStart(SwapWorker));
                m_SwapRequestedFlag = false;
                m_ConsoleDrawWorker.Start();
            }
           
            return true;
        }

        static public void PutChar(char c, short color, int x, int y)
        {

            int index = m_sWidth * (y) + x;
            if (index >= m_Bufer[m_WriteBufferPtr].Length)
            {
                index = 0;
                //throw new Exception("DLUGOSC JEST: " + index.ToString());
            }
            m_Bufer[m_WriteBufferPtr][index].Attributes = color;
            m_Bufer[m_WriteBufferPtr][index].Char.AsciiChar = (byte)c;

        }

        static public void WriteXY(int x, int y, short col, string line)
        {
            for (int i = 0; i < line.Length; ++i)
            {
                int putX = x + i;
                if (putX < m_sWidth)
                {
                    PutChar(line[i], col, putX, y);
                }
            }
        }

        static public void Clear()
        {
            Array.Clear(m_Bufer[m_WriteBufferPtr], 0, m_Bufer[0].Length);
        }

        static public void ClearColor(int col)
        {
            int len = m_sWidth * m_sHeight;
            for (int i = 0; i < len; ++i)
            {
                m_Bufer[m_WriteBufferPtr][i].Attributes = (short)(col << 4);
                m_Bufer[m_WriteBufferPtr][i].Char.AsciiChar = (byte)' ';
            }
        }

        static public void SwapBuffers()
        {
            if (!m_MultiThreadEnabled)
            {
                WriteConsoleOutput(m_ConsoleHandle, m_Bufer[m_DrawBufferPtr], m_ScrBottomRight, m_ScrTopLeft, ref m_ConsoleRect);
                return;
            }

            lock (LOCK)
            {
                if (m_SwapRequestedFlag == false)
                {
                    m_SwapRequestedFlag = true;
                    m_DrawBufferPtr = m_WriteBufferPtr;
                    m_WriteBufferPtr = 1 - m_WriteBufferPtr;
                }
            }
        }


        static public void SetDefaultConsole()
        {
            if (m_ConsoleDrawWorker != null) m_ConsoleDrawWorker.Abort();
            Console.Clear();
            Console.SetWindowSize(120, 30);
            FontInfoEx set = new FontInfoEx
            {
                cbSize = Marshal.SizeOf<FontInfoEx>(),
                FontIndex = 0,
                FontFamily = 0x00,
                FontName = "Consolas",
                FontWeight = 400,
                FontSize = 16,
                FontWidth = 8
            };
            SetCurrentConsoleFontEx(m_ConsoleHandle.DangerousGetHandle(), false, ref set);
            Console.CursorVisible = true;
            Console.SetCursorPosition(0, 0);
            Console.SetBufferSize(120, 30);
        }

        static public void Reallign()
        {
            Console.SetWindowSize(m_sWidth + 2, m_sHeight + 4);
        }

        static private void SwapWorker()
        {
            while (true)
            {
                while (!m_SwapRequestedFlag) { }
                WriteConsoleOutput(m_ConsoleHandle, m_Bufer[m_DrawBufferPtr], m_ScrBottomRight, m_ScrTopLeft, ref m_ConsoleRect);
                lock (LOCK)
                {
                    m_SwapRequestedFlag = false;
                }
            }
        }
    }



   



}
