using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleRenderer.Core;
using System.Runtime.InteropServices;

namespace ConsoleRenderer.TextureEditor
{
    class CGTextureEditor:CGApp
    {


        [DllImport("user32.dll")]

        // GetCursorPos() makes everything possible
        static extern bool GetCursorPos(ref Point lpPoint);


        struct Point
        {
            public int X;
            public int Y;

        }


        public override void OnInitialize()
        {
            ScreenWidth = 140;
            ScreenHeight = 100;
            PixelWidth = 8;
            PixelHeight = 8;
        }
        public override void OnStart()
        {


        }
        public override void OnUpdate(float deltaTime)
        {
            Point p = new Point();
            GetCursorPos(ref p);
            Console.Title = p.X.ToString();

        }
        public override void OnDrawPerColumn(int x)
        {

            float u = (float)x / CGEngine.ScreenWidth;
            CGColorSample csample = CGColorSample.MakeCol(ConsoleColor.White, ConsoleColor.Black, u);

            for (int y = 0; y < CGEngine.ScreenHeight; ++y)
            {
                CGBuffer.AddAsync(csample.Character, csample.BitMask, x, y);
            }
        }
        public override void OnPostDraw() { }
        public override void OnExit() { }
    }
}
