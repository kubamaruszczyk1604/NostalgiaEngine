using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NostalgiaEngine.Core;
using System.IO;

namespace NostalgiaEngine.GUI
{

    
    public class NEFileExplorer: INEGUIElement
    {
        public delegate void OnFileSelected(string path);
        //public delegate void OnFocusChanged(string path, bool focus);
        public delegate void OnPathUpdated(string path);

        private enum VisitState { NoAccess = -1, Directory = 0, File = 1 }
        private readonly int c_ColLength = 20;
        private readonly int c_DistanceBetweenColumns = 40;

        private Stack<string> m_DirStack;
        string[] m_CurrentDirContent;
        private string m_CurrentPath = "c:/";
        private int m_CurrentPosIndex = 0;
        private int m_ViewStartIndex = 0;


        private string m_EditString = "";
        private string m_Title;

        public OnFileSelected onFileSelected { get; set; }
        public OnPathUpdated onPathUpdated { get; set; }

        public int StartRow { get; set; }
        public bool DisplayPathString { get; set; }


        public NEFileExplorer(string title)
        {
            m_DirStack = new Stack<string>();

            if (VisitDirectory(m_CurrentPath, out string[] dirList) == VisitState.Directory)
            {
                m_CurrentDirContent = dirList;
            }
            m_Title = title;
            StartRow = 2;
            DisplayPathString = true;
        }
        public void Update()
        {
            if (!Focused) return;
            bool arrowPressed = false;
            if (NEInput.CheckKeyPress(ConsoleKey.DownArrow))
            {           
                 m_CurrentPosIndex++;
                
                if (m_CurrentPosIndex - m_ViewStartIndex > c_ColLength * 3 - 2)
                {
                    m_ViewStartIndex++;
                }
                arrowPressed = true;
            }
            if (NEInput.CheckKeyPress(ConsoleKey.UpArrow))
            {

                if (m_CurrentPosIndex - m_ViewStartIndex <= 0 && m_CurrentPosIndex > 0)
                {
                    m_ViewStartIndex--;
                }
                m_CurrentPosIndex--;
                arrowPressed = true;
            }
            if (NEInput.CheckKeyPress(ConsoleKey.RightArrow))
            {
                m_CurrentPosIndex += c_ColLength;
                if (m_CurrentPosIndex - m_ViewStartIndex > c_ColLength * 3 - 2)
                {
                    m_ViewStartIndex += c_ColLength;
                }
                arrowPressed = true;
            }

            if (NEInput.CheckKeyPress(ConsoleKey.LeftArrow))
            {

                if (m_CurrentPosIndex - m_ViewStartIndex > 0 && m_CurrentPosIndex > 0)
                {
                    m_ViewStartIndex -= c_ColLength;
                    m_CurrentPosIndex -= c_ColLength;
                    if (m_ViewStartIndex < 0) m_ViewStartIndex = 0;
                }
                arrowPressed = true;
                
            }

            if (m_CurrentPosIndex < 0) m_CurrentPosIndex = 0;
            if (m_CurrentPosIndex >= m_CurrentDirContent.Length) m_CurrentPosIndex = m_CurrentDirContent.Length - 1;

            if(arrowPressed) onPathUpdated?.Invoke(m_CurrentDirContent[m_CurrentPosIndex]);

            if (NEInput.CheckKeyPress(ConsoleKey.Enter))
            {
                string bk = m_CurrentDirContent[m_CurrentPosIndex].Substring(m_CurrentPath.Length);
                if (bk == "...")
                {
                    if (m_CurrentPath.Length > 3)
                    {
                        string del = m_DirStack.Pop();
                        m_CurrentPath = m_CurrentPath.Substring(0, m_CurrentPath.Length - del.Length - 1);
                        VisitDirectory(m_CurrentPath, out string[] dirList);
                        m_CurrentDirContent = dirList;
                        m_CurrentPosIndex = 0;
                        m_ViewStartIndex = 0;
                        m_EditString = m_CurrentPath + "untitled.tex";
                        onPathUpdated?.Invoke(m_EditString);
                    }
                }
                else
                {

                    string newPath = m_CurrentDirContent[m_CurrentPosIndex] + "/";
                    VisitState state = VisitDirectory(newPath, out string[] dirList);
                    if (state == VisitState.Directory)
                    {
                        m_DirStack.Push(bk);
                        m_CurrentPath = newPath;
                        m_CurrentDirContent = dirList;
                        m_CurrentPosIndex = 0;
                        m_ViewStartIndex = 0;
                        m_EditString = newPath + "untitled.tex";
                        onPathUpdated?.Invoke(m_EditString);
                    }
                    else if (state == VisitState.File)
                    {
                        m_EditString = newPath.Substring(0, newPath.Length - 1);
                        onPathUpdated?.Invoke(m_EditString);
                        onFileSelected?.Invoke(m_EditString);
                    }

                }

            }
        }

        public void Draw(int screenWidth)
        {
          
            NEConsoleScreen.WriteXY(0, 0, 12, m_Title);
            NEConsoleScreen.WriteXY(0, StartRow+2, 9, m_CurrentPath);
            if(Focused)
            {
                NEConsoleScreen.WriteXY(0, StartRow, 8 | (0 << 4), "Arrow keys: Navigate     Enter: Enter Subdirectory");
            }

            //if (m_CurrentPosIndex >= 3*c_ColLength) start = c_ColLength;
            for (int i = m_ViewStartIndex; i < m_CurrentDirContent.Length; ++i)
            {
                int x = ((i - m_ViewStartIndex) / c_ColLength) * c_DistanceBetweenColumns;
                int textCol = Focused ? 9 : 1;
                if (x < screenWidth)
                    NEConsoleScreen.WriteXY(2+x, StartRow + 4 + ((i - m_ViewStartIndex) % c_ColLength), (short)(m_CurrentPosIndex == i ? (15 | 1 << 4) : textCol), m_CurrentDirContent[i].Substring(m_CurrentPath.Length));
            }

           // CGBuffer.WriteXY(0, 28, 15 | (1 << 4), m_EditString);
        }



        public void TriggerOnPathUpdated()
        {

            onPathUpdated?.Invoke(m_CurrentPath);
           
        }

        public override void Dispose()
        {
            base.Dispose();
            if (onPathUpdated != null)
            foreach (var e in onPathUpdated.GetInvocationList())
            {
                onPathUpdated -= (OnPathUpdated)e;
            }
            if(onFileSelected != null)
            foreach (var e in onFileSelected.GetInvocationList())
            {
                onFileSelected -= (OnFileSelected)e;
            }
        }

        private VisitState VisitDirectory(string path, out string[] directoryList)
        {
            directoryList = null;
            string[] files = null;

            if (IsFile(path))
                return VisitState.File;

            try
            {

                directoryList = Directory.GetDirectories(path);
                files = Directory.GetFiles(path);


            }
            catch
            {
                directoryList = null;
                return VisitState.NoAccess;
            }


            for (int i = 0; i < directoryList.Length; ++i)
            {
                directoryList[i] = directoryList[i].ToUpper();
            }

            var temp = directoryList.ToList();

            temp.AddRange(files);
            temp.Insert(0, path + "...");
            directoryList = temp.ToArray();
            return VisitState.Directory;


        }

        bool IsFile(string path)
        {
            FileAttributes attr = File.GetAttributes(path);


            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                return false;
            else
                return true;
        }
    }
}
