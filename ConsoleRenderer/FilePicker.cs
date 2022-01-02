using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleRenderer.Core;
using System.IO;

namespace ConsoleRenderer
{
    class FilePicker:CGScene
    {

        private readonly int c_ColLength = 20;
        private readonly int c_DistanceBetweenColumns = 40;

        private Stack<string> m_DirStack;
        string[] m_DirectoryList;
        private string m_CurrentPath = "c:/";
        private int m_CurrentPosIndex = 0;

        public override void OnInitialize()
        {

            ScreenWidth = 120;
            ScreenHeight = 30;
            PixelWidth = 8;
            PixelHeight = 16;
            m_DirStack = new Stack<string>();
            if(TryObtainDirList(m_CurrentPath,out string[] dirList))
            {
                m_DirectoryList = dirList;
            }
            
        }


        public override void OnUpdate(float deltaTime)
        {
            if(CGInput.CheckKeyPress(ConsoleKey.DownArrow))
            {
                m_CurrentPosIndex++;
                if(m_CurrentPosIndex-m_ViewStartIndex > c_ColLength*3-2)
                {
                    m_ViewStartIndex++;
                }
            }
            if (CGInput.CheckKeyPress(ConsoleKey.UpArrow))
            {
                
                if ( m_CurrentPosIndex - m_ViewStartIndex <= 0 && m_CurrentPosIndex>0)
                {
                    m_ViewStartIndex--;
                }
                m_CurrentPosIndex--;

            }

            if (m_CurrentPosIndex < 0) m_CurrentPosIndex = 0;
            if (m_CurrentPosIndex >= m_DirectoryList.Length) m_CurrentPosIndex = m_DirectoryList.Length - 1;

            if (CGInput.CheckKeyPress(ConsoleKey.Enter))
            {
                string bk = m_DirectoryList[m_CurrentPosIndex].Substring(m_CurrentPath.Length);
                if (bk == "...")
                {
                    if (m_CurrentPath.Length > 3)
                    {
                        string del = m_DirStack.Pop();
                        m_CurrentPath = m_CurrentPath.Substring(0, m_CurrentPath.Length - del.Length - 1);
                        TryObtainDirList(m_CurrentPath, out string[] dirList);
                        m_DirectoryList = dirList;
                        m_CurrentPosIndex = 0;
                        m_ViewStartIndex = 0;
                    }
                }
                else
                {

                    string newPath = m_DirectoryList[m_CurrentPosIndex] + "/";
                    if (TryObtainDirList(newPath, out string[] dirList))
                    {
                        m_DirStack.Push(bk);
                        m_CurrentPath = newPath;
                        m_DirectoryList = dirList;
                        m_CurrentPosIndex = 0;
                        m_ViewStartIndex = 0;
                    }

                }

            }



        }





        int m_ViewStartIndex = 0;
        public override void OnDraw()
        {
            CGBuffer.Clear();
            CGBuffer.WriteXY(0, 0, 12, "FILE OPEN DIALOG");
            CGBuffer.WriteXY(0, 1, 10, m_CurrentPath);

            //int cnt = 0;

           
            //if (m_CurrentPosIndex >= 3*c_ColLength) start = c_ColLength;
            for (int i = m_ViewStartIndex; i < m_DirectoryList.Length; ++i)
            {
                int x = ((i-m_ViewStartIndex) / c_ColLength) * c_DistanceBetweenColumns;
                
                if(x < ScreenWidth)
                CGBuffer.WriteXY(x, 3 + ((i-m_ViewStartIndex)%c_ColLength), (short)(m_CurrentPosIndex==i?(11|1<<4):11), m_DirectoryList[i].Substring(m_CurrentPath.Length));
               // cnt++;
            }



 
            //foreach (string file in files)
            //{
            ////    CGBuffer.WriteXY(0,2+cnt, 12, file);
            //    cnt++;
            //}


        }
        public override void OnExit()
        {

        }



        private bool TryObtainDirList(string path, out string[] directoryList)
        {
            try
            {

                directoryList = Directory.GetDirectories(path);
                for (int i = 0; i < directoryList.Length; ++i)
                {
                    directoryList[i] = directoryList[i].ToUpper();
                }

                var temp = directoryList.ToList();
                string[] files = Directory.GetFiles(path);
                temp.AddRange(files);
                temp.Insert(0, path + "...");
                directoryList = temp.ToArray();
            }
            catch
            {
                directoryList = null;
                return false;
            }
            return true;


        }
    }
}
