using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NostalgiaEngine.Core;
using System.IO;

namespace NostalgiaEngine.GUI
{
    public class NESaveDialog:NEScene
    {

        NEFileExplorer m_FileExplorer;
        NETextInput m_TextInput;

        NEYesNoWindow m_YesNoWindow;

        string m_SavePath;
        public override bool OnLoad()
        {
            NEInput.FlushKeyboard();

            ScreenWidth = 120;
            ScreenHeight = 30;
            PixelWidth = 8;
            PixelHeight = 16;
            m_FileExplorer = new NEFileExplorer("SAVE FILE AS..");
            m_TextInput = new NETextInput("---", 15, 25);
            m_FileExplorer.onPathUpdated += OnPathUpdated;
            m_FileExplorer.TriggerOnPathUpdated();
            m_TextInput.onLineCommit += OnPathReady;
            m_YesNoWindow = new NEYesNoWindow(30, 5, 50, 8, "Confirm Save");
            m_YesNoWindow.onUserSelection += OnSaveDecision;
            m_FileExplorer.Focus();
            m_SavePath = "";
            return true;
        }


        public override void OnUpdate(float deltaTime)
        {
            m_FileExplorer.Update();
            m_TextInput.InputUpdate();
            m_YesNoWindow.Update();
            if (NEInput.CheckKeyPress(ConsoleKey.F2))
            {

                if (m_FileExplorer.Focused)
                {
                    m_TextInput.Focus();
                }
            }

            if(NEInput.CheckKeyPress(ConsoleKey.Escape))
            {
                if (m_TextInput.Focused)
                {
                    m_FileExplorer.Focus();
                }
                else if(m_FileExplorer.Focused)
                {
                    Exit(0);
                }

            }

            NEInput.FlushKeyboard();
        }


        public override void OnDraw()
        {
            NEConsoleScreen.Clear();
            m_FileExplorer.Draw(ScreenWidth);
            
           // CGBuffer.WriteXY(3, 27, 15 | (1 << 4), "SAVE PATH: ");
            if (m_FileExplorer.Focused)
            {
                m_TextInput.Draw(8);
                NEConsoleScreen.WriteXY(34, 27, 15 | (2 << 4)," F2 - SELECT ");
                NEConsoleScreen.WriteXY(64, 27, 15 | (2 << 4), " ESC - CANCEL ");
                NEConsoleScreen.WriteXY(4, 25, 15 | (1 << 4), "SAVE AS:");
            }
            else if (m_YesNoWindow.Focused)
            {
                m_YesNoWindow.Draw();
            }
            else
            {
                m_TextInput.Draw(15|(1<<4));
                NEConsoleScreen.WriteXY(34, 27, 15 | (4 << 4), " ENTER - SAVE ");
                NEConsoleScreen.WriteXY(64, 27, 15 | (4 << 4), " ESC - BACK ");
                NEConsoleScreen.WriteXY(4, 25, 15 | (4 << 4), "SAVE AS:");
            }

        }

        void OnPathReady(string path)
        {
            
            NEInput.FlushKeyboard();
            m_YesNoWindow.Focus();
            m_SavePath = path;
        }

        void OnSaveDecision(bool save)
        {
            if(save)
            {

                this.Exit(m_SavePath);
            }
            else
            {
               m_FileExplorer.Focus();

            }
        }

        public override void OnExit()
        {
            m_FileExplorer.Dispose();
            m_TextInput.Dispose();
            m_YesNoWindow.Dispose();
            NEInput.FlushKeyboard();
        }

        private void OnPathUpdated(string path)
        {
            m_TextInput.Reset(path);
        }

    }
}
