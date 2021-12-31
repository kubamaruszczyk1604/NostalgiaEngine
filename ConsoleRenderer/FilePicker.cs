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
        int current = 1;
        public override void OnDraw()
        {

            CGBuffer.WriteXY(0, 1, 12, "FILE PICKER TEMPLATE");
            string[] files = Directory.GetFiles(m_CurrentDir);
            int cnt = 1;

            string[] dirs = Directory.GetDirectories(m_CurrentDir);

            foreach (string dir in dirs)
            {
                CGBuffer.WriteXY(0, 2 + cnt, (short)(current==cnt?(11|1<<4):11), dir);
                cnt++;
            }

            foreach (string file in files)
            {
                CGBuffer.WriteXY(0,2+cnt, 12, file);
                cnt++;
            }


        }
        public override void OnExit()
        {

        }
    }
}
