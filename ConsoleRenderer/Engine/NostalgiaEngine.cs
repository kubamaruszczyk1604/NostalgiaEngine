using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.Core
{
    public class Engine
    {
        private readonly int DEFAULT_SCR_W = 120;
        private readonly int DEFAULT_SCR_H = 80;
        private readonly int DEFAULT_PIXEL_W = 6;
        private readonly int DEFAULT_PIXEL_H = 6;


        private NEScene m_CurrentScene;
        private Stack<NEScene> m_SceneStack;
        private bool m_Running;
        private float m_Delta;

        
        public static Engine Instance { get; private set; }
        public string Title { get; set; }
        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        public int PixelWidth { get; private set; }
        public int PixelHeight { get; private set; }
        public float RunningTime { get; private set; }
        public string PostMessage { get; set; }

        public Engine(string postMessage = "Thank you for using NostalgiaEngine!")
        {
            NEWindowControl.DisableConsoleWindowButtons();
            m_Running = false;
            m_Delta = 0.0f;
            m_SceneStack = new Stack<NEScene>();
            Instance = this;
            PostMessage = postMessage;
        }


        private bool InitializeScreen(NEScene scene)
        {
            ScreenWidth = scene.ScreenWidth > 10 ? scene.ScreenWidth : DEFAULT_SCR_W;
            ScreenHeight = scene.ScreenHeight > 10 ? scene.ScreenHeight : DEFAULT_SCR_H;
            PixelWidth = scene.PixelWidth > 0 ? scene.PixelWidth : DEFAULT_PIXEL_W;
            PixelHeight = scene.PixelHeight > 0 ? scene.PixelHeight : DEFAULT_PIXEL_H;
            Title = "NOSTALGIA ENGINE";

            return NEConsoleScreen.Initialize((short)ScreenWidth, (short)ScreenHeight, (short)PixelWidth, (short)PixelHeight);
        }

        public bool PushScene(NEScene scene)
        {
            NEInput.FlushKeyboard();
            if(m_CurrentScene != null) m_CurrentScene.OnPause();
            if (scene.OnLoad())
            {
                if(!InitializeScreen(scene))
                {
                    return false;
                }
                m_CurrentScene = scene;
                m_SceneStack.Push(scene);
                scene.OnStart();
                return true;
            }
            else
            {
                if (m_CurrentScene != null) m_CurrentScene.OnResume();
                return false;
            }

        }

        public void PopScene()
        {
            m_CurrentScene.OnExit();
            m_SceneStack.Pop();
            if(m_SceneStack.Count == 0)
            {
                m_Running = false;
                return;
            }
            m_CurrentScene = m_SceneStack.Peek();
            NEInput.FlushKeyboard();
            m_CurrentScene.OnResume();
            InitializeScreen(m_CurrentScene);
        }

        public void Start(NEScene scene)
        {
            if(!PushScene(scene))
            {
                return;
            }
            NEWindowControl.QuickEditMode(false);
            m_Running = true;
            
            while (m_Running)
            {
                
                NEFrameTimer.Update();
                m_Delta = NEFrameTimer.GetDeltaTime();
                Console.SetCursorPosition(5, 1);
               Console.Title = Title + " @"+ ScreenWidth.ToString() + "x" +  ScreenHeight.ToString() + 
                    " FPS: " + NEFrameTimer.GetFPS() + "   FRAME TIME: " + m_Delta + "s ";

                
                m_CurrentScene.OnUpdate(NEFrameTimer.GetDeltaTime());

                var sceneType = m_CurrentScene.GetType();
                //Execute OnDrawPerColumn() only if scene child class implements it.
                if (sceneType.GetMethod("OnDrawPerColumn").DeclaringType == sceneType)
                {
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

                             m_CurrentScene.OnDrawPerColumn(column);

                             if (column >= ScreenWidth - 1) resetEvent.Set();
                         }), new object[] { x });
                    }

                    resetEvent.WaitOne();
                    
                }
                m_CurrentScene.OnDraw();

                NEConsoleScreen.SwapBuffers();

                RunningTime += NEFrameTimer.GetDeltaTime();
               
            }
            Console.Clear();
            NEConsoleScreen.SetDefaultConsole();
            Console.WriteLine(PostMessage);
            Console.ReadLine();
        }

    }
}
