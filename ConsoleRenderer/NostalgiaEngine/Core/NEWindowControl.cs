﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace NostalgiaEngine.Core
{
    public class NEWindowControl
    {

        public enum StdHandle : int
        {
            STD_INPUT_HANDLE = -10,
            STD_OUTPUT_HANDLE = -11,
            STD_ERROR_HANDLE = -12,
        }

        public enum ConsoleMode : uint
        {
            ENABLE_ECHO_INPUT = 0x0004,
            ENABLE_EXTENDED_FLAGS = 0x0080,
            ENABLE_INSERT_MODE = 0x0020,
            ENABLE_LINE_INPUT = 0x0002,
            ENABLE_MOUSE_INPUT = 0x0010,
            ENABLE_PROCESSED_INPUT = 0x0001,
            ENABLE_QUICK_EDIT_MODE = 0x0040,
            ENABLE_WINDOW_INPUT = 0x0008,
            ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200,

            //screen buffer handle
            ENABLE_PROCESSED_OUTPUT = 0x0001,
            ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
            ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
            DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
            ENABLE_LVB_GRID_WORLDWIDE = 0x0010
        }

        [DllImport("user32.dll")]
         static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();


        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("user32.dll")]
         static extern bool GetWindowRect(IntPtr hwnd, ref NERect rectangle);
        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hwnd, ref NERect rectangle );

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, uint wFlags);

        static public readonly int MF_BYCOMMAND = 0x00000000;
        static public readonly int SC_CLOSE = 0xF060;
        static public readonly int SC_MINIMIZE = 0xF020;
        static public readonly int SC_MAXIMIZE = 0xF030;
        static public readonly int SC_VSCROLL = 0xF070;
        static public readonly int SC_HSCROLL = 0xF080;
        static public readonly int SC_SIZE = 0xF000;


        static uint SWP_NOSIZE = 1;
        static uint SWP_NOZORDER = 4;

        static private Process c_CurrentProcess = Process.GetCurrentProcess();
        static private NERect c_MainWindowRect = new NERect();
        static private NEPoint c_MainWindowPos = new NEPoint();

        public static void DisableConsoleWindowButtons()
        {

            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);
           
        }

        public static void QuickEditMode(bool Enable)
        {
            //QuickEdit lets the user select text in the console window with the mouse, to copy to the windows clipboard.
            //But selecting text stops the console process (e.g. unzipping). This may not be always wanted.
            IntPtr consoleHandle = GetStdHandle((int)StdHandle.STD_INPUT_HANDLE);
            UInt32 consoleMode;

            GetConsoleMode(consoleHandle, out consoleMode);
            if (Enable)
                consoleMode |= ((uint)ConsoleMode.ENABLE_QUICK_EDIT_MODE);
            else
                consoleMode &= ~((uint)ConsoleMode.ENABLE_QUICK_EDIT_MODE);

            consoleMode |= ((uint)ConsoleMode.ENABLE_EXTENDED_FLAGS);

            SetConsoleMode(consoleHandle, consoleMode);
        }
        
        static public NEPoint GetWindowPosition()
        {    
            GetWindowRect(c_CurrentProcess.MainWindowHandle, ref c_MainWindowRect);
            c_MainWindowPos.X = c_MainWindowRect.Left;
            c_MainWindowPos.Y = c_MainWindowRect.Top;
            return c_MainWindowPos;
        }

        public static void SetWindowPosition(int x, int y)
        {
            var consoleWnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            SetWindowPos(consoleWnd, 0, x, y, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
        }
    }
}
