using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using OpenTK;
using System.Diagnostics;

namespace ConsoleRenderer
{
    public class CGEngine
    {
        private CGApp m_App;
        private bool m_Running;
        private float m_Delta;

        public string Title { get; set; }
        static public int ScreenWidth { get; private set; }
        static public int ScreenHeight { get; private set; }
        static public int PixelWidth { get; private set; }
        static public int PixelHeight { get; private set; }
        public float RunningTime { get; private set; }


        public CGEngine(string title, int scrW, int scrH, int pixelW, int pixelH)
        {
            
            m_Running = false;
            ScreenWidth = scrW;
            ScreenHeight = scrH;
            PixelWidth = pixelW;
            PixelHeight = pixelH;
            Title = title;
            m_Delta = 0.0f;
        }

        public void Start(CGApp app)
        {
            m_App = app;
            Buffer.Initialize((short)ScreenWidth, (short)ScreenHeight, (short)PixelWidth, (short)PixelHeight);
            m_Running = true;
            m_App.OnStart();

            while(m_Running)
            {

                //Console.SetCursorPosition(5, 1);
                Console.Title = Title + " FPS: " + FrameTimer.GetFPS() + "   FRAME TIME: " + m_Delta + "s ";

                m_App.OnUpdate(FrameTimer.GetDeltaTime());
                var resetEvent = new ManualResetEvent(false); // Will be reset when buffer is ready to be swaped

                //For each column..
                for (int x = 0; x < ScreenWidth; ++x)
                {
                    
                    // Queue new task
                    ThreadPool.QueueUserWorkItem(
                       new WaitCallback(
                     delegate (object state)
                     {
                         object[] array = state as object[];
                         int column = Convert.ToInt32(array[0]);

                         m_App.OnDrawPerColumn(column);

                         if (column >= ScreenWidth - 1) resetEvent.Set();
                     }), new object[] { x });
                }

                resetEvent.WaitOne();
                m_App.OnPostDraw();
                Buffer.Swap();

                RunningTime += FrameTimer.GetDeltaTime();
                m_Delta = FrameTimer.GetDeltaTime();
                FrameTimer.Update();

            }
            m_App.OnExit();

        }

    }
}
