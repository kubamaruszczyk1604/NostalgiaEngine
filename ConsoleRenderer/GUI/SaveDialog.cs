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

        NEYesNoWindow m_ConfirmSaveWindow;
        NEYesNoWindow m_ConfirmOverwriteWindow;

        string m_SavePath;
        public override bool OnLoad()
        {
            NEInput.FlushKeyboard();
            NEColorManagement.SetDefaultPalette();
            ScreenWidth = 120;
            ScreenHeight = 30;
            PixelWidth = 8;
            PixelHeight = 16;
            m_FileExplorer = new NEFileExplorer(" SELECT WHERE TO SAVE YOUR TEXTURE ");
            m_TextInput = new NETextInput("---", 25, 27);
            m_FileExplorer.onPathUpdated += OnPathUpdated;
            m_FileExplorer.TriggerOnPathUpdated();
            m_TextInput.onLineCommit += OnPathReady;
            m_ConfirmSaveWindow = new NEYesNoWindow(30, 5, 50, 8, "Confirm Save?");
            m_ConfirmOverwriteWindow = new NEYesNoWindow(30, 7, 50, 8, "File already exists. Overwite?", NEWindowStyle.Warning);
            m_ConfirmSaveWindow.onUserSelection += OnSaveDecision;
            m_ConfirmOverwriteWindow.onUserSelection += OnOverwriteDecision;
            m_FileExplorer.Focus();
            m_SavePath = "";
            return true;
        }


        public override void OnUpdate(float deltaTime)
        {
            m_FileExplorer.Update();
            m_TextInput.InputUpdate();
            m_ConfirmSaveWindow.Update();
            m_ConfirmOverwriteWindow.Update();
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
            NEScreenBuffer.Clear();
            m_FileExplorer.Draw(ScreenWidth);
            

            if (m_FileExplorer.Focused)
            {
                m_TextInput.Draw(8);

                NEScreenBuffer.WriteXY(64, 29, 15 | (2 << 4)," F2 - NEXT ");
                NEScreenBuffer.WriteXY(34, 29, 15 | (4 << 4), " ESC - BACK ");

                NEScreenBuffer.WriteXY(10, 27, 7 | (0 << 4), "CURRENT PATH:");
            }
            else if (m_ConfirmSaveWindow.Focused)
            {
                m_ConfirmSaveWindow.Draw();
            }
            else if (m_ConfirmOverwriteWindow.Focused)
            {
                m_ConfirmOverwriteWindow.Draw();
            }
            else
            {
                m_TextInput.Draw(15|(1<<4));
                NEScreenBuffer.WriteXY(54, 29, 15 | (2 << 4), " ENTER - CONFIRM ");
                NEScreenBuffer.WriteXY(34, 29, 15 | (4 << 4), " ESC - BACK ");
                NEScreenBuffer.WriteXY(12, 27, 15 | (0 << 4), "ENTER NAME: ");
            }

        }

        void OnPathReady(string path)
        {
            
            NEInput.FlushKeyboard();
            m_ConfirmSaveWindow.Focus();
            m_SavePath = path;
        }

        void OnSaveDecision(bool save)
        {
            if(save)
            {
                if(File.Exists(m_SavePath))
                {
                    NEInput.FlushKeyboard();
                    m_ConfirmOverwriteWindow.Focus();
                }
                else
                {
                    this.Exit(m_SavePath);
                }
                
            }
            else
            {
               m_FileExplorer.Focus();
            }
        }

        void OnOverwriteDecision(bool overwrite)
        {
            if(overwrite)
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
            m_ConfirmSaveWindow.Dispose();
            m_ConfirmOverwriteWindow.Dispose();
            NEInput.FlushKeyboard();
        }

        private void OnPathUpdated(string path)
        {
            m_TextInput.Reset(path);
        }

    }
}
