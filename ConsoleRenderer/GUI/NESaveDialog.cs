using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ConsoleRenderer.Core;
using System.IO;
using ConsoleRenderer.Tools;

namespace ConsoleRenderer.GUI
{
    public class NESaveDialog:NEScene
    {

        NEFileExplorer m_FileExplorer;
        NETextInput m_TextInput;

        NEYesNoWindow m_WindowRect;

        public override void OnInitialize()
        {
            while (Console.KeyAvailable) Console.ReadKey(); // flush key buffer

            ScreenWidth = 120;
            ScreenHeight = 30;
            PixelWidth = 8;
            PixelHeight = 16;
            m_FileExplorer = new NEFileExplorer("SAVE FILE AS..");
            m_TextInput = new NETextInput("---", 15, 25);
            m_FileExplorer.onFocusChanged += OnExplorerFocusChanged;
            m_FileExplorer.onPathUpdated += OnPathUpdated;
            m_FileExplorer.TriggerOnPathUpdated();
            m_TextInput.onLineCommit += OnPathReady;
            //CGInput.CheckKeyPress(ConsoleKey.F2); // flush key state
            //CGInput.CheckKeyPress(ConsoleKey.F2);
            CGInput.FlushKeyboard();
            m_WindowRect = new NEYesNoWindow(30, 5, 50, 8, "Confirm Save");
        }


        public override void OnUpdate(float deltaTime)
        {
            if (CGInput.CheckKeyPress(ConsoleKey.F2))
            {

                if (m_FileExplorer.InFocus)
                {
                    m_FileExplorer.UnFocus();
                    m_TextInput.Focus();
                }
            }

            if(CGInput.CheckKeyPress(ConsoleKey.Escape))
            {
                if (m_TextInput.InFocus)
                {
                    m_TextInput.UnFocus();
                    m_FileExplorer.Focus();
                }
                else if(m_FileExplorer.InFocus)
                {
                    m_FileExplorer.Dispose();
                    m_TextInput.Dispose();
                    Exit(0);
                }
            }

            
            m_FileExplorer.Update();
            m_TextInput.InputUpdate();
            while (Console.KeyAvailable) Console.ReadKey(); // flush key buffer
        }


        public override void OnDraw()
        {
            NEScreen.Clear();
            m_FileExplorer.Draw(ScreenWidth);
            
           // CGBuffer.WriteXY(3, 27, 15 | (1 << 4), "SAVE PATH: ");
            if (m_FileExplorer.InFocus)
            {
                m_TextInput.Draw(8);
                NEScreen.WriteXY(34, 27, 15 | (2 << 4)," F2 - SELECT ");
                NEScreen.WriteXY(64, 27, 15 | (2 << 4), " ESC - CANCEL ");
                NEScreen.WriteXY(4, 25, 15 | (1 << 4), "SAVE AS:");
            }
            else
            {
                m_TextInput.Draw(15|(1<<4));
                NEScreen.WriteXY(34, 27, 15 | (4 << 4), " ENTER - SAVE ");
                NEScreen.WriteXY(64, 27, 15 | (4 << 4), " ESC - BACK ");
                NEScreen.WriteXY(4, 25, 15 | (4 << 4), "SAVE AS:");
            }
            //CGBuffer.WriteXY(40, 29, 12, m_FileExplorer.InFocus.ToString());
            m_WindowRect.Draw();

        }

        void OnPathReady(string path)
        {
            m_FileExplorer.Dispose();
            m_TextInput.Dispose();
            this.Exit(path);
        }
        public override void OnExit()
        {

        }


        private void OnExplorerFocusChanged(string path, bool focus)
        {
            if (!focus)
            {
                m_TextInput.Reset(path);
                m_TextInput.Focus();
            }
        }

        private void OnPathUpdated(string path)
        {
            m_TextInput.Reset(path);
        }




    }
}
