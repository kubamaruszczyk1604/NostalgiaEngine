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
        internal struct NEColRGB
        {
            internal uint ColorDWORD;

            internal NEColRGB(uint r, uint g, uint b)
            {
                ColorDWORD = r + (g << 8) + (b << 16);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CONSOLE_SCREEN_BUFFER_INFO_EX
        {
            internal int cbSize;
            internal NEPoint dwSize;
            internal NEPoint dwCursorPosition;
            internal ushort wAttributes;
            internal NERect srWindow;
            internal NEPoint dwMaximumWindowSize;
            internal ushort wPopupAttributes;
            internal bool bFullscreenSupported;
            internal NEColRGB black;
            internal NEColRGB darkBlue;
            internal NEColRGB darkGreen;
            internal NEColRGB darkCyan;
            internal NEColRGB darkRed;
            internal NEColRGB darkMagenta;
            internal NEColRGB darkYellow;
            internal NEColRGB gray;
            internal NEColRGB darkGray;
            internal NEColRGB blue;
            internal NEColRGB green;
            internal NEColRGB cyan;
            internal NEColRGB red;
            internal NEColRGB magenta;
            internal NEColRGB yellow;
            internal NEColRGB white;
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
            screenBuffInfo.cbSize = Marshal.SizeOf(screenBuffInfo);                   
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
                    screenBuffInfo.black = new NEColRGB(r, g, b);
                    break;
                case 1:
                    screenBuffInfo.darkBlue = new NEColRGB(r, g, b);
                    break;
                case 2:
                    screenBuffInfo.darkGreen = new NEColRGB(r, g, b);
                    break;
                case 3:
                    screenBuffInfo.darkCyan = new NEColRGB(r, g, b);
                    break;
                case 4:
                    screenBuffInfo.darkRed = new NEColRGB(r, g, b);
                    break;
                case 5:
                    screenBuffInfo.darkMagenta = new NEColRGB(r, g, b);
                    break;
                case 6:
                    screenBuffInfo.darkYellow = new NEColRGB(r, g, b);
                    break;
                case 7:
                    screenBuffInfo.gray = new NEColRGB(r, g, b);
                    break;
                case 8:
                    screenBuffInfo.darkGray = new NEColRGB(r, g, b);
                    break;
                case 9:
                    screenBuffInfo.blue = new NEColRGB(r, g, b);
                    break;
                case 10:
                    screenBuffInfo.green = new NEColRGB(r, g, b);
                    break;
                case 11:
                    screenBuffInfo.cyan = new NEColRGB(r, g, b);
                    break;
                case 12:
                    screenBuffInfo.red = new NEColRGB(r, g, b);
                    break;
                case 13:
                    screenBuffInfo.magenta = new NEColRGB(r, g, b);
                    break;
                case 14:
                    screenBuffInfo.yellow = new NEColRGB(r, g, b);
                    break;
                case 15:
                    screenBuffInfo.white = new NEColRGB(r, g, b);
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
