using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleRenderer.Core;
using System.Runtime.InteropServices;
using OpenTK;

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

        private int m_cursorX;
        private int m_cursorY;

        private int m_ImageW;
        private int m_ImageH;

        public override void OnInitialize()
        {
            ScreenWidth = 110;
            ScreenHeight = 80;
            PixelWidth = 8;
            PixelHeight = 8;
            m_ImageW = 100;
            m_ImageH = 68;
        }
        public override void OnStart()
        {
            WindowControl.DisableConsoleWindowButtons();

        }
        public override void OnUpdate(float deltaTime)
        {
            //Point p = new Point();
            //GetCursorPos(ref p);
            //Console.Title = p.X.ToString();
            if(CGInput.CheckKeyPress(ConsoleKey.UpArrow))
            {
                m_cursorY -= 1;
            }
            if (CGInput.CheckKeyPress(ConsoleKey.DownArrow))
            {
                m_cursorY += 1;
            }
            if (CGInput.CheckKeyPress(ConsoleKey.LeftArrow))
            {
                m_cursorX -= 1;
            }
            if (CGInput.CheckKeyPress(ConsoleKey.RightArrow))
            {
                m_cursorX += 1;
            }
            m_cursorY = m_cursorY % ScreenHeight;
            if (m_cursorY < 0) m_cursorY = 0;

            m_cursorX = m_cursorX % ScreenWidth;
            if (m_cursorX < 0) m_cursorX = 0;

        }

        private void DrawPalette(int x, int y)
        {

            for (int i = 0; i < 16; ++i)
            {
                if (CGHelper.InRectangle(new Vector2(x, y), new Vector2(6 * (i), 0), 6, 8))
                {
                    CGBuffer.AddAsync(' ', (short)((i) << 4), x, y);
                }

            }
        }
        public override void OnDrawPerColumn(int x)
        {

            for (int y = 0; y < CGEngine.ScreenHeight; ++y)
            {

                CGBuffer.AddAsync((char)CGBlock.Weak, (short)ConsoleColor.DarkBlue, x, y);


                DrawPalette(x, y);


                if (CGHelper.InRectangle(new Vector2(x,y),new Vector2(2,10), m_ImageW,m_ImageH))
                {
                    CGColorSample csample = CGColorSample.MakeCol(ConsoleColor.Black, ConsoleColor.DarkGray, 0.1f);

                    CGBuffer.AddAsync(csample.Character, csample.BitMask, x, y);
                }
            }
        }
        public override void OnPostDraw()
        {
            CGBuffer.AddAsync('&', (short)((int)ConsoleColor.DarkMagenta << 4), m_cursorX, m_cursorY);
        }
        public override void OnExit() { }
    }
}
