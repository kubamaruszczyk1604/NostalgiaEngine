using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace NostalgiaEngine.Core
{
    public class NEColorMgr
    {

        [StructLayout(LayoutKind.Sequential)]
        internal struct NEColorDefinition
        {
            internal uint ColMask;


            internal NEColorDefinition(uint r, uint g, uint b)
            {
                ColMask = r | (g << 8) | (b << 16);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CONSOLE_SCREEN_BUFFER_INFO_EX
        {
            internal int Size;
            internal NEPoint WndSize;
            internal NEPoint CursorPosition;
            internal ushort Attributes;
            internal NERect Window;
            internal NEPoint MaxWndSize;
            internal ushort PopupAttributes;
            internal bool FullScreenSupportedFlag;

            internal NEColorDefinition Black;
            internal NEColorDefinition DarkBLue;
            internal NEColorDefinition DarkGreen;
            internal NEColorDefinition DarkCyan;
            internal NEColorDefinition DarkRed;
            internal NEColorDefinition DarkMagenta;
            internal NEColorDefinition DarkYellow;
            internal NEColorDefinition Gray;
            internal NEColorDefinition DarkGray;
            internal NEColorDefinition Blue;
            internal NEColorDefinition Green;
            internal NEColorDefinition Cyan;
            internal NEColorDefinition Red;
            internal NEColorDefinition Magenta;
            internal NEColorDefinition Yellow;
            internal NEColorDefinition White;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

        public static int SetColor(int consoleColor, uint r, uint g, uint b)
        {
            CONSOLE_SCREEN_BUFFER_INFO_EX screenBuffInfo = new CONSOLE_SCREEN_BUFFER_INFO_EX();
            screenBuffInfo.Size = Marshal.SizeOf(screenBuffInfo);                   
            IntPtr outputHandle = GetStdHandle((int)NEWindowControl.StdHandle.STD_OUTPUT_HANDLE);    

            if (outputHandle == new IntPtr(-1))
            {
                return Marshal.GetLastWin32Error();
            }

            if (!GetConsoleScreenBufferInfoEx(outputHandle, ref screenBuffInfo))
            {
                return Marshal.GetLastWin32Error();
            }

            switch (consoleColor)
            {
                case 0:
                    screenBuffInfo.Black = new NEColorDefinition(r, g, b);
                    break;
                case 1:
                    screenBuffInfo.DarkBLue = new NEColorDefinition(r, g, b);
                    break;
                case 2:
                    screenBuffInfo.DarkGreen = new NEColorDefinition(r, g, b);
                    break;
                case 3:
                    screenBuffInfo.DarkCyan = new NEColorDefinition(r, g, b);
                    break;
                case 4:
                    screenBuffInfo.DarkRed = new NEColorDefinition(r, g, b);
                    break;
                case 5:
                    screenBuffInfo.DarkMagenta = new NEColorDefinition(r, g, b);
                    break;
                case 6:
                    screenBuffInfo.DarkYellow = new NEColorDefinition(r, g, b);
                    break;
                case 7:
                    screenBuffInfo.Gray = new NEColorDefinition(r, g, b);
                    break;
                case 8:
                    screenBuffInfo.DarkGray = new NEColorDefinition(r, g, b);
                    break;
                case 9:
                    screenBuffInfo.Blue = new NEColorDefinition(r, g, b);
                    break;
                case 10:
                    screenBuffInfo.Green = new NEColorDefinition(r, g, b);
                    break;
                case 11:
                    screenBuffInfo.Cyan = new NEColorDefinition(r, g, b);
                    break;
                case 12:
                    screenBuffInfo.Red = new NEColorDefinition(r, g, b);
                    break;
                case 13:
                    screenBuffInfo.Magenta = new NEColorDefinition(r, g, b);
                    break;
                case 14:
                    screenBuffInfo.Yellow = new NEColorDefinition(r, g, b);
                    break;
                case 15:
                    screenBuffInfo.White = new NEColorDefinition(r, g, b);
                    break;
            }

            if (!SetConsoleScreenBufferInfoEx(outputHandle, ref screenBuffInfo))
            {
                return Marshal.GetLastWin32Error();
            }
            return 0;
        }
    }
}
