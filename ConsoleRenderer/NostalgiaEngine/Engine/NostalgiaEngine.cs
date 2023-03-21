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
        private Thread m_TaskbarUpdateWorker;
        private bool m_SuspendTaskbarFlag;
        
        public static Engine Instance { get; private set; }
        public string Title { get; set; }
        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        public int PixelWidth { get; private set; }
        public int PixelHeight { get; private set; }
        public float TotalTime { get; private set; }
        public string PostMessage { get; set; }
        public string TitleBarAppend { get; set; }

        public Engine()
        {
            //NEColorMgr.SetNostalgiaPalette();
           // NEColorMgr.SetDefaultPalette();
            NEInput.FlushKeyboard();
            NEWindowControl.DisableConsoleWindowButtons();
            m_Running = false;
            m_Delta = 0.0f;
            m_SceneStack = new Stack<NEScene>();
            Instance = this;
            PostMessage = "Thank you for using Nostalgia Engine!";
            Title = "NOSTALGIA ENGINE";
            TitleBarAppend = "";
            //m_TaskbarUpdateWorker = new Thread(new ThreadStart(UpdateTaskbar));
            m_SuspendTaskbarFlag = false;
            
        }

        private void UpdateTaskbar()
        {
            while (m_Running)
            {
                if (m_SuspendTaskbarFlag)
                {
                    Thread.Sleep(1000);
                    continue;
                }
                Console.Title = Title +
                    "             Resolution: " + ScreenWidth.ToString() + "x" + ScreenHeight.ToString() +
                    "             FPS: " + NEFrameTimer.GetFPS() + "  " + TitleBarAppend;
                Thread.Sleep(500);

            }

        }
        private bool InitializeScreen(NEScene scene)
        {
            scene.ScreenWidth = scene.ScreenWidth > 10 ? scene.ScreenWidth : DEFAULT_SCR_W;
            scene.ScreenHeight = scene.ScreenHeight > 10 ? scene.ScreenHeight : DEFAULT_SCR_H;
            scene.PixelWidth = scene.PixelWidth > 0 ? scene.PixelWidth : DEFAULT_PIXEL_W;
            scene.PixelHeight = scene.PixelHeight > 0 ? scene.PixelHeight : DEFAULT_PIXEL_H;
            ScreenWidth = scene.ScreenWidth;
            ScreenHeight = scene.ScreenHeight;
            PixelWidth = scene.PixelWidth;
            PixelHeight = scene.PixelHeight;
            return NEScreenBuffer.Initialize((short)ScreenWidth, (short)ScreenHeight, (short)PixelWidth, (short)PixelHeight,scene.ParallelScreenDraw);
        }

        public bool PushScene(NEScene scene)
        {
            NEInput.FlushKeyboard();
            if(m_CurrentScene != null) m_CurrentScene.OnPause();
            bool loadOK = scene.OnLoad();
            bool screenOK = InitializeScreen(scene);
            if (loadOK && screenOK )
            {
                m_CurrentScene = scene;
                m_SceneStack.Push(scene);
                scene.OnInitializeSuccess();
                return true;
            }
            else
            {
                NEColorManagement.SetDefaultPalette();
                NEScreenBuffer.SetDefaultConsole();
                NEInput.FlushKeyboard();
                Console.Clear();
                Console.Title = Title;
                m_SuspendTaskbarFlag = true;
                Console.SetCursorPosition(0, 0);
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("   Failed to load scene!   \n");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("OnLoad() status:                " + (loadOK ? "OK" : "Failed"));
                Console.WriteLine("Window and Screen status:       " + (screenOK ? "OK" : "Failed"));
                Console.WriteLine("\n\n\nPress ENTER to continue...");

                NEInput.BlockUntilKeyPress(NEKey.Enter);
                m_SuspendTaskbarFlag = false;
                if (m_CurrentScene != null)
                {
                    InitializeScreen(m_CurrentScene);
                    m_CurrentScene.OnResume();
                }
                return false;
            }

        }

        public void PopScene()
        {
            NEInput.FlushKeyboard();
            m_CurrentScene.OnExit();
            m_SceneStack.Pop();
            if(m_SceneStack.Count == 0)
            {
                m_Running = false;
                return;
            }
            m_CurrentScene = m_SceneStack.Peek();
            m_CurrentScene.OnResume();
            InitializeScreen(m_CurrentScene);
        }

        public void Start(NEScene scene)
        {
            Console.Clear();
            NEInput.FlushKeyboard();
            NEColorManagement.SetNostalgiaPalette();
            if (!PushScene(scene))
            {
                return;
            }
            NEWindowControl.QuickEditMode(false);
            NEWindowControl.SetWindowPosition(90,20);
            m_Running = true;
            m_TaskbarUpdateWorker = new Thread(new ThreadStart(UpdateTaskbar));
            m_TaskbarUpdateWorker.Start();
            while (m_Running)
            {
                
                NEFrameTimer.Update();
                m_Delta = NEFrameTimer.GetDeltaTime();
                m_CurrentScene.OnUpdate(NEFrameTimer.GetDeltaTime());

                var sceneType = m_CurrentScene.GetType();
                //Execute OnDrawPerColumn() only if scene child class implements it.
                if (sceneType.GetMethod("OnDrawPerColumn").DeclaringType == sceneType)
                {
                    var resetEvent = new ManualResetEvent(false); // Will be reset when buffer is ready to be swaped

                    //For each column..
                    for (int x = 0; x < ScreenWidth; ++x)
                    {
                        //m_CurrentScene.OnDrawPerColumn(x);
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

                if (m_CurrentScene.OnDraw())
                {
                    NEScreenBuffer.SwapBuffers();
                }
                TotalTime += NEFrameTimer.GetDeltaTime();
               
            }
            m_TaskbarUpdateWorker.Join();
            NEColorManagement.SetDefaultPalette();
            Console.Title = Title;
            Console.Clear();
            NEScreenBuffer.SetDefaultConsole();
            Console.WriteLine(PostMessage);
            NEInput.BlockUntilKeyPress(NEKey.Enter);
        }

    }
}
