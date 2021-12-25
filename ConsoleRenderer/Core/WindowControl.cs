using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ConsoleRenderer.Core
{
    public class WindowControl
    {
        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        static public readonly int MF_BYCOMMAND = 0x00000000;
        static public readonly int SC_CLOSE = 0xF060;
        static public readonly int SC_MINIMIZE = 0xF020;
        static public readonly int SC_MAXIMIZE = 0xF030;
        static public readonly int SC_VSCROLL = 0xF070;
        static public readonly int SC_HSCROLL = 0xF080;
        static public readonly int SC_SIZE = 0xF000;


        public static void DisableConsoleWindowButtons()
        {


            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);

        }
    }
}
