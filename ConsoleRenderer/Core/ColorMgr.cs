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
        struct CONSOLE_SCREEN_BUFFER_INFO_EX
        {
            //static internal void CopyColors(CONSOLE_SCREEN_BUFFER_INFO_EX src, ref CONSOLE_SCREEN_BUFFER_INFO_EX dest)
            //{

            //    dest.Black = src.Black;
            //    dest.DarkBlue = src.DarkBlue;
            //    dest.DarkGreen = src.DarkGreen;
            //    dest.DarkCyan = src.DarkCyan;
            //    dest.DarkRed = src.DarkRed;
            //    dest.DarkMagenta = src.DarkMagenta;
            //    dest.DarkYellow = src.DarkYellow;
            //    dest.Gray = src.Gray;
            //    dest.DarkGray = src.DarkGray;
            //    dest.Blue = src.Blue;
            //    dest.Green = src.Green;
            //    dest.Cyan = src.Cyan;
            //    dest.Red = src.Red;
            //    dest.Magenta = src.Magenta;
            //    dest.Yellow = src.Yellow;
            //    dest.White = src.White;

            //}

            internal int Size;
            internal NEPoint WndSize;
            internal NEPoint CursorPosition;
            internal ushort Attributes;
            internal NERect Window;
            internal NEPoint MaxWndSize;
            internal ushort PopupAttributes;
            internal bool FullScreenSupportedFlag;

            internal NEConsoleColorDefinition Black;
            internal NEConsoleColorDefinition DarkBlue;
            internal NEConsoleColorDefinition DarkGreen;
            internal NEConsoleColorDefinition DarkCyan;
            internal NEConsoleColorDefinition DarkRed;
            internal NEConsoleColorDefinition DarkMagenta;
            internal NEConsoleColorDefinition DarkYellow;
            internal NEConsoleColorDefinition Gray;
            internal NEConsoleColorDefinition DarkGray;
            internal NEConsoleColorDefinition Blue;
            internal NEConsoleColorDefinition Green;
            internal NEConsoleColorDefinition Cyan;
            internal NEConsoleColorDefinition Red;
            internal NEConsoleColorDefinition Magenta;
            internal NEConsoleColorDefinition Yellow;
            internal NEConsoleColorDefinition White;


        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);





        public static int SetColor(int consoleColor, uint r, uint g, uint b)
        {
            return SetColor(consoleColor, new NEConsoleColorDefinition(r, g, b));
        }

        public static int SetColor(int consoleColor, NEConsoleColorDefinition colDef)
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
            screenBuffInfo.Window.Bottom++;
            screenBuffInfo.Window.Right++;
            switch (consoleColor)
            {
                case 0:
                    screenBuffInfo.Black = colDef;
                    break;
                case 1:
                    screenBuffInfo.DarkBlue = colDef;
                    break;
                case 2:
                    screenBuffInfo.DarkGreen = colDef;
                    break;
                case 3:
                    screenBuffInfo.DarkCyan = colDef;
                    break;
                case 4:
                    screenBuffInfo.DarkRed = colDef;
                    break;
                case 5:
                    screenBuffInfo.DarkMagenta = colDef;
                    break;
                case 6:
                    screenBuffInfo.DarkYellow = colDef;
                    break;
                case 7:
                    screenBuffInfo.Gray = colDef;
                    break;
                case 8:
                    screenBuffInfo.DarkGray = colDef;
                    break;
                case 9:
                    screenBuffInfo.Blue = colDef;
                    break;
                case 10:
                    screenBuffInfo.Green = colDef;
                    break;
                case 11:
                    screenBuffInfo.Cyan = colDef;
                    break;
                case 12:
                    screenBuffInfo.Red = colDef;
                    break;
                case 13:
                    screenBuffInfo.Magenta = colDef;
                    break;
                case 14:
                    screenBuffInfo.Yellow = colDef;
                    break;
                case 15:
                    screenBuffInfo.White = colDef;
                    break;
            }


            if (!SetConsoleScreenBufferInfoEx(outputHandle, ref screenBuffInfo))
            {
                return Marshal.GetLastWin32Error();
            }
            return 0;
        }


        static public void SetNostalgiaPalette()
        {
            NEColorMgr.SetColor(12, 255, 95, 0);
            NEColorMgr.SetColor(14, 208, 156, 142);
            NEColorMgr.SetColor(10, 0, 225, 33);
            NEColorMgr.SetColor(13, 255, 12, 0);
            NEColorMgr.SetColor(6, 94, 43, 43);
            NEColorMgr.SetColor(3, 255, 0, 110);
            NEColorMgr.SetColor(11, 255, 255, 0);
            NEColorMgr.SetColor(4, 127, 0, 0);
            NEColorMgr.SetColor(2, 82, 151, 255);
            NEColorMgr.SetColor(9, 0, 160, 0);
            NEColorMgr.SetColor(7, 80, 90, 90);
        }



        static public void SetSpectralPalette1()
        {
            NEConsoleColorDefinition[] pal = {
            new NEConsoleColorDefinition(0, 0, 0), new NEConsoleColorDefinition(15, 0, 229), new NEConsoleColorDefinition(175, 0, 229),
            new NEConsoleColorDefinition(229, 0, 114), new NEConsoleColorDefinition(229, 0, 15), new NEConsoleColorDefinition(229, 80, 0),
            new NEConsoleColorDefinition(229, 172, 0), new NEConsoleColorDefinition(229, 218, 0), new NEConsoleColorDefinition(95, 229, 0),
            new NEConsoleColorDefinition(0, 229, 3), new NEConsoleColorDefinition(0, 229, 130), new NEConsoleColorDefinition(0, 221, 229),
            new NEConsoleColorDefinition(0, 156, 229), new NEConsoleColorDefinition(0, 91, 229), new NEConsoleColorDefinition(179, 184, 229),
            new NEConsoleColorDefinition(242, 242, 242)};

            for(int i = 0; i < 16; ++i)
            {
                SetColor(i, pal[i]);
            }

        }

        static public void SetDefaultPalette()
        {
            NEConsoleColorDefinition[] pal = {
            new NEConsoleColorDefinition(0, 0, 0), new NEConsoleColorDefinition(0, 55, 218), new NEConsoleColorDefinition(19, 161, 14),
            new NEConsoleColorDefinition(58, 150, 221), new NEConsoleColorDefinition(195,17, 31), new NEConsoleColorDefinition(136, 23, 152),
            new NEConsoleColorDefinition(193, 156, 0), new NEConsoleColorDefinition(204, 204, 204), new NEConsoleColorDefinition(118, 118,118),
            new NEConsoleColorDefinition(59, 120, 255), new NEConsoleColorDefinition(22, 198, 12), new NEConsoleColorDefinition(97, 214, 214),
            new NEConsoleColorDefinition(231, 72, 86), new NEConsoleColorDefinition(180, 0, 158), new NEConsoleColorDefinition(249, 213, 150),
            new NEConsoleColorDefinition(242, 242, 242)};

            for (int i = 0; i < 16; ++i)
            {
                SetColor(i, pal[i]);
            }

        }

    }
}
