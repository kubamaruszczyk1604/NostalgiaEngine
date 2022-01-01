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
        public override void OnInitialize()
        {

            ScreenWidth = 120;
            ScreenHeight = 30;
            PixelWidth = 8;
            PixelHeight = 16;
        }
        public override void OnStart() { }
        public override void OnPause() { }
        public override void OnResume() { }
        public override void OnUpdate(float deltaTime)
        {
            if(CGInput.CheckKeyPress(ConsoleKey.DownArrow))
            {
                current++;
            }
            if (CGInput.CheckKeyPress(ConsoleKey.UpArrow))
            {
                current--;
            }

        }
        public override void OnDrawPerColumn(int x) { }
        string m_CurrentDir = @"c:\";
        int current = 0;
        int wrapH = 20;
        int colDist = 40;
        public override void OnDraw()
        {
            CGBuffer.Clear();
            CGBuffer.WriteXY(0, 0, 12, "FILE PICKER TEMPLATE");
            CGBuffer.WriteXY(0, 1, 10, m_CurrentDir);
            string[] files = Directory.GetFiles(m_CurrentDir);
            int cnt = 0;

            string[] dirs = Directory.GetDirectories(m_CurrentDir);

            var temp = dirs.ToList();
            temp.AddRange(files);
            temp.Insert(0, m_CurrentDir + "...");
            dirs = temp.ToArray();
            int start = 0;
            if (current >= 2*wrapH) start = wrapH;
            for (int i = start; i < dirs.Length; ++i)
            {
                int x = (cnt / wrapH) * colDist;
                
                if(x< ScreenWidth)
                CGBuffer.WriteXY(x, 3 + (cnt%wrapH), (short)(current-start==cnt?(11|1<<4):11), dirs[i].Substring(m_CurrentDir.Length));
                cnt++;
            }
            if(CGInput.CheckKeyPress(ConsoleKey.Enter))
            {
                    m_CurrentDir = dirs[current] + @"\";
                    current = 0;

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
