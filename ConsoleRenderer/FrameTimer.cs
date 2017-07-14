using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer
{
    class FrameTimer
    {
        private static long s_LastFpsCapture = System.Environment.TickCount;
        private static long s_LastCallTime = System.Environment.TickCount;
        private static int s_Fps = 1;
        private static int s_Frames;

        private static float deltaTime = 0.005f;

        public static void Update()
        {
            var currentTick = System.Environment.TickCount;
            if (currentTick - s_LastFpsCapture >= 1000)
            {
                s_Fps = s_Frames;
                s_Frames = 0;
                s_LastFpsCapture = currentTick;
            }
            s_Frames++;

            deltaTime = currentTick - s_LastCallTime;
            s_LastCallTime = currentTick;

        }

        public static int GetFPS()
        {
            return s_Fps;
        }

        public static float GetDeltaTime()
        {
            return (deltaTime / 1000.0f);
        }
    }
}
