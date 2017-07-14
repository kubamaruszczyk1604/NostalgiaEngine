using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;


namespace ConsoleRenderer
{
    class Buffer
    {

        /* TEXT FRAME BUFFER FOR FAST RENDERING */
        /* by Kuba Maruszczyk - based on stack overflow sample regarding interop services */
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

        static SafeFileHandle h = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
        static int m_sWidth;
        static int m_sHeight;
        static CharInfo[] buf;
        static SmallRect rect;

        static int m_sBuffPtr;

        static public void Initialize(short width, short height)
        {
            m_sWidth = width;
            m_sHeight = height;
            m_sBuffPtr = 0;
            if (!h.IsInvalid)
            {
                buf = new CharInfo[width * height];
                rect = new SmallRect() { Left = 5, Top = 2, Right = (short)(width + 5), Bottom = (short)(height + 2) };


            }
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
            WriteConsoleOutput(h, buf,
                         new Coord() { X = (short)m_sWidth, Y = (short)m_sHeight },
                         new Coord() { X = 0, Y = 0 },
                         ref rect);

            m_sBuffPtr = 0;
        }

    }
}
