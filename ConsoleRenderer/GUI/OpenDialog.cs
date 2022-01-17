using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.GUI
{
    public class NEOpenDialog: NEScene
    {

        NEFileExplorer m_FileExplorer;
        string m_OpenPath;

        public NEOpenDialog(string initialPath = "C:/")
        {
            m_OpenPath = initialPath;
        }

        public override bool OnLoad()
        {
            base.OnLoad();
            ScreenWidth = 120;
            ScreenHeight = 30;
            PixelWidth = 8;
            PixelHeight = 16;
            m_FileExplorer = new NEFileExplorer(" SELECT FILE TO OPEN ", m_OpenPath);
            m_FileExplorer.onFileSelected = OnPathUpdate;
            m_FileExplorer.Focus();
            
            return true;
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            m_FileExplorer.Update();

            if(NEInput.CheckKeyPress(NEKey.Escape))
            {
                Exit(0);
            }
        }

        public override void OnDraw()
        {
            base.OnDraw();
            NEScreenBuffer.Clear();
            m_FileExplorer.Draw(ScreenWidth);
            NEScreenBuffer.WriteXY(54, 28, 15|2<<4, " ENTER - CONFIRM ");
            NEScreenBuffer.WriteXY(34, 28, 15 | 4 << 4, " ESC - CANCEL ");
        }

        private void OnPathUpdate(string path)
        {
            Exit(path);
        }
    }
}
