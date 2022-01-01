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

        public override void OnInitialize()
        {

            ScreenWidth = 120;
            ScreenHeight = 30;
            PixelWidth = 8;
            PixelHeight = 16;
            m_DirStack = new Stack<string>();
        }
        public override void OnStart() { }
        public override void OnPause() { }
        public override void OnResume() { }
        public override void OnUpdate(float deltaTime)
        {
            if(CGInput.CheckKeyPress(ConsoleKey.DownArrow))
            {
                m_CurrentPosIndex++;
            }
            if (CGInput.CheckKeyPress(ConsoleKey.UpArrow))
            {
                m_CurrentPosIndex--;
            }

        }
        public override void OnDrawPerColumn(int x) { }
        string m_CurrentDirStr = "c:/";
        int m_CurrentPosIndex = 0;

        public override void OnDraw()
        {
            CGBuffer.Clear();
            CGBuffer.WriteXY(0, 0, 12, "FILE PICKER TEMPLATE");
            CGBuffer.WriteXY(0, 1, 10, m_CurrentDirStr);
            string[] files = Directory.GetFiles(m_CurrentDirStr);
            int cnt = 0;

            string[] dirs = Directory.GetDirectories(m_CurrentDirStr);

            var temp = dirs.ToList();
            //temp.AddRange(files);
            temp.Insert(0, m_CurrentDirStr + "...");
            dirs = temp.ToArray();

            int start = 0;
            if (m_CurrentPosIndex >= 2*c_ColLength) start = c_ColLength;
            for (int i = start; i < dirs.Length; ++i)
            {
                int x = (cnt / c_ColLength) * c_DistanceBetweenColumns;
                
                if(x< ScreenWidth)
                CGBuffer.WriteXY(x, 3 + (cnt%c_ColLength), (short)(m_CurrentPosIndex-start==cnt?(11|1<<4):11), dirs[i].Substring(m_CurrentDirStr.Length));
                cnt++;
            }
            if (m_CurrentPosIndex < 0) m_CurrentPosIndex = 0;
            if (m_CurrentPosIndex >=  dirs.Length) m_CurrentPosIndex = dirs.Length-1;
            if (CGInput.CheckKeyPress(ConsoleKey.Enter))
            {
                string bk = dirs[m_CurrentPosIndex].Substring(m_CurrentDirStr.Length);
                if (bk == "...")
                {
                    if (m_CurrentDirStr.Length > 3)
                    {
                        string del = m_DirStack.Pop();
                        m_CurrentDirStr = m_CurrentDirStr.Substring(0, m_CurrentDirStr.Length - del.Length - 1);
                    }
                }
                else
                {
                    m_CurrentDirStr = dirs[m_CurrentPosIndex] + "/";
                    m_DirStack.Push(bk);
                }
                    m_CurrentPosIndex = 0;

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
    }
}
