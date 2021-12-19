using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleRenderer
{
    class Input
    {

        public delegate void OnKeyPressed(ConsoleKeyInfo keyInfo);

        public static OnKeyPressed ev_KeyPressed;

        private static void FireOnKeyPressed(ConsoleKeyInfo keyInfo)
        {
            if (ev_KeyPressed != null) ev_KeyPressed(keyInfo);
        }



        private static bool s_Detecting = false;
        public static void Start()
        {
            if (s_Detecting) return;
            Thread t = new Thread(Worker);
            s_Detecting = true;
            t.Start();
        }

        public static void Stop()
        {
            s_Detecting = false;
        }

        private static void Worker()
        {
            while (s_Detecting)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();

                    FlushKeyboard();
                    FireOnKeyPressed(key);
                }
            }
        }

        private static void FlushKeyboard()
        {
            while (Console.KeyAvailable)
                Console.ReadKey();
        }

    }
}
