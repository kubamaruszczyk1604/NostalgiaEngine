using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace NostalgiaEngine.Core
{
    public class NEColorManagement
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

            internal NEConsoleColorDef Black;
            internal NEConsoleColorDef DarkBlue;
            internal NEConsoleColorDef DarkGreen;
            internal NEConsoleColorDef DarkCyan;
            internal NEConsoleColorDef DarkRed;
            internal NEConsoleColorDef DarkMagenta;
            internal NEConsoleColorDef DarkYellow;
            internal NEConsoleColorDef Gray;
            internal NEConsoleColorDef DarkGray;
            internal NEConsoleColorDef Blue;
            internal NEConsoleColorDef Green;
            internal NEConsoleColorDef Cyan;
            internal NEConsoleColorDef Red;
            internal NEConsoleColorDef Magenta;
            internal NEConsoleColorDef Yellow;
            internal NEConsoleColorDef White;


        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);





        public static int RedefineColor(int consoleColor, uint r, uint g, uint b)
        {
            return RedefineColor(consoleColor, new NEConsoleColorDef(r, g, b));
        }

        public static int RedefineColor(int consoleColor, NEConsoleColorDef colDef)
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



        static public void SetPalette(NEColorPalette pal)
        {
            for (int i = 0; i < 16; ++i)
            {
                RedefineColor(i, pal.GetColor(i));
            }
        }

        static public void SetPalette(NEConsoleColorDef [] pal)
        {
            if(pal.Length != 16)
            {
                for (int i = 0; i < 16; ++i)
                {
                    RedefineColor(i, new NEConsoleColorDef(250,i,250));
                }
            }
            else
            {
                for (int i = 0; i < 16; ++i)
                {
                    RedefineColor(i, pal[i]);
                }
            }
        }



        static public void SetNostalgiaPalette()
        {
            SetPalette(NEColorPalette.NostalgiaPalette);
        }

        static public void SetSpectralPalette1()
        {
            SetPalette(NEColorPalette.ColorSpectrumPalette_1);
        }

        static public void SetDefaultPalette()
        {
            SetPalette(NEColorPalette.DefaultPalette);
        }

    }
}
