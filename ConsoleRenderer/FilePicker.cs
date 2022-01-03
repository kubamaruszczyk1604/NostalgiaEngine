using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ConsoleRenderer.Core;
using System.IO;

namespace ConsoleRenderer
{
    public class FilePicker:CGScene
    {
        enum VisitState { NoAccess = -1, Directory = 0, File = 1}
        private readonly int c_ColLength = 20;
        private readonly int c_DistanceBetweenColumns = 40;

        private Stack<string> m_DirStack;
        string[] m_CurrentDirContent;
        private string m_CurrentPath = "c:/";
        private int m_CurrentPosIndex = 0;

        private string m_EditString = "select path";

        public override void OnInitialize()
        {

            ScreenWidth = 120;
            ScreenHeight = 30;
            PixelWidth = 8;
            PixelHeight = 16;
            m_DirStack = new Stack<string>();
            if(VisitDirectory(m_CurrentPath,out string[] dirList)==VisitState.Directory)
            {
                m_CurrentDirContent = dirList;
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
            if(CGInput.CheckKeyPress(ConsoleKey.RightArrow))
            {
                m_CurrentPosIndex += c_ColLength;
                if (m_CurrentPosIndex - m_ViewStartIndex > c_ColLength * 3 - 2)
                {
                    m_ViewStartIndex += c_ColLength;
                }
            }

            if (CGInput.CheckKeyPress(ConsoleKey.LeftArrow))
            {
               
                if (m_CurrentPosIndex - m_ViewStartIndex > 0 && m_CurrentPosIndex > 0)
                {
                    m_ViewStartIndex -= c_ColLength;
                    m_CurrentPosIndex -= c_ColLength;
                    if (m_ViewStartIndex < 0) m_ViewStartIndex = 0;
                }
                
            }

            if (m_CurrentPosIndex < 0) m_CurrentPosIndex = 0;
            if (m_CurrentPosIndex >= m_CurrentDirContent.Length) m_CurrentPosIndex = m_CurrentDirContent.Length - 1;

            if (CGInput.CheckKeyPress(ConsoleKey.Enter))
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
                    }
                    else if(state == VisitState.File)
                    {
                        m_EditString = newPath.Substring(0,newPath.Length-1);
                    }

                }

            }
            

              // ReadLine("test");


        }





        int m_ViewStartIndex = 0;
        public override void OnDraw()
        {
            
            CGBuffer.Clear();
            CGBuffer.WriteXY(0, 0, 12, "FILE OPEN DIALOG");
            CGBuffer.WriteXY(0, 1, 9, m_CurrentPath);

           
            //if (m_CurrentPosIndex >= 3*c_ColLength) start = c_ColLength;
            for (int i = m_ViewStartIndex; i < m_CurrentDirContent.Length; ++i)
            {
                int x = ((i-m_ViewStartIndex) / c_ColLength) * c_DistanceBetweenColumns;
                
                if(x < ScreenWidth)
                CGBuffer.WriteXY(x, 3 + ((i-m_ViewStartIndex)%c_ColLength), (short)(m_CurrentPosIndex==i?(15|1<<4):9), m_CurrentDirContent[i].Substring(m_CurrentPath.Length));
            }

            CGBuffer.WriteXY(0, 28, 15|(1<<4), m_EditString);


           

        }
        public override void OnExit()
        {

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

        string ReadLine(string defaultStr)
        {
            string displayString = defaultStr;
            Console.CursorVisible = true;
            int cursorPos = defaultStr.Length;
            ConsoleKeyInfo info;
            
            while (true)
            {
                info = Console.ReadKey(true);
                Console.SetCursorPosition(5, 29);
                Console.Write(new string(' ', displayString.Length+4));
                if (char.IsLetterOrDigit(info.KeyChar))
                {
                 //   Console.Write(info.KeyChar);
                    displayString = displayString.Insert(cursorPos, info.KeyChar.ToString());
                    cursorPos++;
                        //+= info.KeyChar;
                }
                if (info.Key == ConsoleKey.Backspace && cursorPos>0)
                {
                    displayString = displayString.Remove(cursorPos-1, 1);
                    cursorPos--;
                }
                else if (info.Key == ConsoleKey.LeftArrow)
                {
                    if (cursorPos > 0) cursorPos--;
                }
                else if (info.Key == ConsoleKey.RightArrow)
                {
                    if (cursorPos < displayString.Length) cursorPos++;
                }
                Console.SetCursorPosition(5, 29);
                Console.Write(displayString);
                Console.SetCursorPosition(5+cursorPos, 29);

            }

            
           
            // 
            return defaultStr;
        }

    }
}
