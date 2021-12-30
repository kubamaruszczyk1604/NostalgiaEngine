using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using OpenTK;
using System.Diagnostics;
using ConsoleRenderer.Core;

namespace ConsoleRenderer
{
    public class CGEngine
    {
        private readonly int DEFAULT_SCR_W = 120;
        private readonly int DEFAULT_SCR_H = 80;
        private readonly int DEFAULT_PIXEL_W = 6;
        private readonly int DEFAULT_PIXEL_H = 6;


        private CGApp m_App;
        private bool m_Running;
        private float m_Delta;

        public string Title { get; set; }
        static public int ScreenWidth { get; private set; }
        static public int ScreenHeight { get; private set; }
        static public int PixelWidth { get; private set; }
        static public int PixelHeight { get; private set; }
        public float RunningTime { get; private set; }
        object locker = new object();

        public CGEngine()
        {
            WindowControl.DisableConsoleWindowButtons();
            m_Running = false;
            m_Delta = 0.0f;
            //m_buffThread = new Thread(new ThreadStart(BufferSwapWorker));

        }

        private void BufferSwapWorker()
        {
            while(true)
            {
                CGBuffer.Swap();

            }
            
        }

        public void Start(CGApp app)
        {
            m_App = app;
            m_App.OnInitialize();
            ScreenWidth = m_App.ScreenWidth > 10 ? m_App.ScreenWidth : DEFAULT_SCR_W;
            ScreenHeight = m_App.ScreenHeight > 10 ? m_App.ScreenHeight : DEFAULT_SCR_H;
            PixelWidth = m_App.PixelWidth > 0 ? m_App.PixelWidth : DEFAULT_PIXEL_W;
            PixelHeight = m_App.PixelHeight > 0 ? m_App.PixelHeight : DEFAULT_PIXEL_H;
            Title = "D";

            CGBuffer.Initialize((short)ScreenWidth, (short)ScreenHeight, (short)PixelWidth, (short)PixelHeight);
            WindowControl.QuickEditMode(false);
            m_Running = true;
            m_App.OnStart();
           // m_buffThread.Start();
            //m_buffThread.Suspend();
            while (m_Running)
            {
                
                CGFrameTimer.Update();
                m_Delta = CGFrameTimer.GetDeltaTime();
                Console.SetCursorPosition(5, 1);
               Console.Title = Title + " @"+ ScreenWidth.ToString() + "x" +  ScreenHeight.ToString() + 
                    " FPS: " + CGFrameTimer.GetFPS() + "   FRAME TIME: " + m_Delta + "s ";


                m_App.OnUpdate(CGFrameTimer.GetDeltaTime());
                var resetEvent = new ManualResetEvent(false); // Will be reset when buffer is ready to be swaped
               
                //m_buffThread.Suspend();
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

                //m_buffThread.Resume();
                CGBuffer.Swap();

                RunningTime += CGFrameTimer.GetDeltaTime();
               
            }
            m_App.OnExit();

        }

    }
}
