using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleRenderer.Core;

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
        public override void OnUpdate(float deltaTime) { }
        public override void OnDrawPerColumn(int x) { }
        public override void OnDraw()
        {

            CGBuffer.WriteXY(0, 10, 12, "FILE PICKER TEMPLATE");
        }
        public override void OnExit()
        {

        }
    }
}
