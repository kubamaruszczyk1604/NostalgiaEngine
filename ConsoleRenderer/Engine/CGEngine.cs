﻿using System;
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


        private CGScene m_CurrentScene;
        private Stack<CGScene> m_SceneStack;
        private bool m_Running;
        private float m_Delta;

        
        public static CGEngine Instance { get; private set; }
        public string Title { get; set; }
        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        public int PixelWidth { get; private set; }
        public int PixelHeight { get; private set; }
        public float RunningTime { get; private set; }
        object locker = new object();

        public CGEngine()
        {
            WindowControl.DisableConsoleWindowButtons();
            m_Running = false;
            m_Delta = 0.0f;
            m_SceneStack = new Stack<CGScene>();
            Instance = this;
        }


        private void SetSceneAsCurrent(CGScene scene)
        {
            ScreenWidth = scene.ScreenWidth > 10 ? scene.ScreenWidth : DEFAULT_SCR_W;
            ScreenHeight = scene.ScreenHeight > 10 ? scene.ScreenHeight : DEFAULT_SCR_H;
            PixelWidth = scene.PixelWidth > 0 ? scene.PixelWidth : DEFAULT_PIXEL_W;
            PixelHeight = scene.PixelHeight > 0 ? scene.PixelHeight : DEFAULT_PIXEL_H;
            Title = "CGENGINE";

            CGBuffer.Initialize((short)ScreenWidth, (short)ScreenHeight, (short)PixelWidth, (short)PixelHeight);
        }

        public void PushScene(CGScene scene)
        {

            if(m_CurrentScene != null) m_CurrentScene.OnPause();
 
            scene.OnInitialize();
            SetSceneAsCurrent(scene);
            scene.OnStart();
            m_SceneStack.Push(scene);
            m_CurrentScene = scene;
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
            m_CurrentScene.OnResume();
            SetSceneAsCurrent(m_CurrentScene);
        }

        public void Start(CGScene scene)
        {
            PushScene(scene);
            WindowControl.QuickEditMode(false);
            m_Running = true;
            
            while (m_Running)
            {
                
                CGFrameTimer.Update();
                m_Delta = CGFrameTimer.GetDeltaTime();
                Console.SetCursorPosition(5, 1);
               Console.Title = Title + " @"+ ScreenWidth.ToString() + "x" +  ScreenHeight.ToString() + 
                    " FPS: " + CGFrameTimer.GetFPS() + "   FRAME TIME: " + m_Delta + "s ";

                var sceneType = m_CurrentScene.GetType();
                m_CurrentScene.OnUpdate(CGFrameTimer.GetDeltaTime());
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

                CGBuffer.Swap();

                RunningTime += CGFrameTimer.GetDeltaTime();
               
            }
            m_CurrentScene.OnExit();

        }

    }
}