using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
namespace NostalgiaEngine.ConsoleGUI
{
    public class NEConsoleSounds
    {
        //[DllImport("kernel32.dll", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //static extern bool Beep(uint dwFreq, uint dwDuration);

        public static void ConfirmBeep()
        {
            Console.Beep(659, 20);
            Console.Beep(831, 15);
            Console.Beep(987, 10);

        }

        public static void ConfirmBeep2()
        {
            Console.Beep(659, 15);
            Console.Beep(987, 20);
        }

        public static void WarningBeep()
        {
            Console.Beep(587, 20);
        }
        public static void WarningBeep2()
        {
            Console.Beep(784, 20);
            Console.Beep(587, 20);
        }

        public static void BA_Beep()
        {
            Console.Beep(739, 30);
            Console.Beep(493, 20);
            
        }

        public static void AB_Beep()
        {
            Console.Beep(493, 20);
            Console.Beep(739, 30);
        }

        public static void ForbidenBeep()
        {
            Console.Beep(300, 100);
            Console.Beep(250, 100);
        }

        public static void ErrorBeep()
        {

            Console.Beep(300, 200);
            Console.Beep(290, 200);
        }

        public static void ErrorBeep2()
        {
            Console.Beep(180, 300);
            Console.Beep(150, 10);
        }
    }
}
