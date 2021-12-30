﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleRenderer.Core;
using System.Runtime.InteropServices;
using OpenTK;

namespace ConsoleRenderer.TextureEditor
{
    class CGTextureEditor : CGApp
    {


        [DllImport("user32.dll")]

        // GetCursorPos() makes everything possible
        static extern bool GetCursorPos(ref Point lpPoint);


        struct Point
        {
            public int X;
            public int Y;
        }

        class Pixel
        {
            public int X;
            public int Y;
            public int Col;

            public Pixel(int x, int y, int col)
            {
                X = x;
                Y = y;
                Col = col;
            }
        }

        private readonly Vector2 c_ColorPanelPos = new Vector2(2, 0);
        private readonly Vector2 c_DrawingCanvasPos = new Vector2(3, 12);
        private readonly int c_ColorWindowWidth = 6;
        private readonly int c_ColorWindowHeight = 8;
        private readonly int c_MinImageH = 5;
        private readonly int c_MinImageW = 5;
        private readonly int c_MaxImageW = 100;
        private readonly int c_MaxImageH = 70;
        private readonly Vector2[] c_Directions = new Vector2[] { new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, -1), new Vector2(0, 1) };





        private int m_cursorX;
        private int m_cursorY;

        private int m_ImageW;
        private int m_ImageH;




        private int SelectedColor;
        private MemTex16 m_ImageData;

        public override void OnInitialize()
        {
            ScreenWidth = 110;
            ScreenHeight = 83;
            PixelWidth = 8;
            PixelHeight = 8;
            m_ImageW = 100;
            m_ImageH = 68;
            SelectedColor = 10;
            m_ImageData = new MemTex16(m_ImageW, m_ImageH);
            m_cursorX = 10;
            m_cursorY = 10;
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
            if (CGInput.CheckKeyPress(ConsoleKey.UpArrow))
            {
                if (CGInput.CheckKeyDown((ConsoleKey)0xA2))
                {
                    m_ImageH--;
                    if (m_ImageH < c_MinImageH) m_ImageH = c_MinImageH;

                }
                else
                {
                    m_cursorY -= 1;
                }

            }
            if (CGInput.CheckKeyPress(ConsoleKey.DownArrow))
            {
                if (CGInput.CheckKeyDown((ConsoleKey)0xA2))
                {
                    m_ImageH++;
                    if (m_ImageH > c_MaxImageH) m_ImageH = c_MaxImageH;

                }
                else
                {
                    m_cursorY += 1;
                }
            }
            if (CGInput.CheckKeyPress(ConsoleKey.LeftArrow))
            {

                if (CGInput.CheckKeyDown((ConsoleKey)0xA2))
                {
                    m_ImageW--;
                    if (m_ImageW < c_MinImageW) m_ImageW = c_MinImageW;

                }
                else
                {
                    m_cursorX -= 1;
                }

            }
            if (CGInput.CheckKeyPress(ConsoleKey.RightArrow))
            {
                if (CGInput.CheckKeyDown((ConsoleKey)0xA2))
                {
                    m_ImageW++;
                    if (m_ImageW > c_MaxImageW) m_ImageW = c_MaxImageW;

                }
                else
                {
                    m_cursorX += 1;
                }
            }
            m_cursorY = m_cursorY % m_ImageH;
            if (m_cursorY < 0) m_cursorY = 0;

            m_cursorX = m_cursorX % m_ImageW;
            if (m_cursorX < 0) m_cursorX = 0;

            if (CGInput.CheckKeyPress(ConsoleKey.C))
            {
                SelectedColor++;
                SelectedColor %= 16;
            }

            if (CGInput.CheckKeyDown(ConsoleKey.Spacebar))
            {
                m_ImageData.SetPixel(m_cursorX, m_cursorY, SelectedColor);
            }
            if (CGInput.CheckKeyDown(ConsoleKey.F))
            {
                
                m_ImageData.FloodFill(m_cursorX, m_cursorY, SelectedColor);
            }

        }

        private void DrawPalette(int x, int y)
        {
            Vector2 pixelPos = new Vector2(x, y);
            for (int i = 0; i < 16; ++i)
            {
                if (CGHelper.InRectangle(pixelPos, new Vector2(c_ColorWindowWidth * (i), 0) + c_ColorPanelPos, c_ColorWindowWidth, c_ColorWindowHeight))
                {
                    CGBuffer.AddAsync(' ', (short)((i) << 4), x, y);
                }

            }

            if (CGHelper.InRectangle(pixelPos, new Vector2(c_ColorWindowWidth * SelectedColor, 8) + c_ColorPanelPos, c_ColorWindowWidth, 2))
            {
                CGBuffer.AddAsync((char)CGBlock.Middle, (short)((15)), x, y);
            }
        }





        public override void OnDrawPerColumn(int x)
        {

            for (int y = 0; y < CGEngine.ScreenHeight; ++y)
            {

                CGBuffer.AddAsync((char)CGBlock.Weak, (short)ConsoleColor.DarkBlue, x, y);


                DrawPalette(x, y);


                //if (CGHelper.InRectangle(new Vector2(x, y), c_DrawingCanvasPos, m_ImageW + 1, m_ImageH + 1))
                //{
                //    CGColorSample csample = CGColorSample.MakeCol(ConsoleColor.Black, ConsoleColor.DarkGray, 0.1f);

                //    CGBuffer.AddAsync(csample.Character, csample.BitMask, x, y);
                //}
 
                if (x >= (int)c_DrawingCanvasPos.X && x < m_ImageData.Width + (int)c_DrawingCanvasPos.X && 
                    y >= (int)c_DrawingCanvasPos.Y && y< m_ImageData.Height + (int)c_DrawingCanvasPos.Y )
                {
                   int col = m_ImageData.GetColor(x- (int)c_DrawingCanvasPos.X, y- (int)c_DrawingCanvasPos.Y);
                        CGBuffer.AddAsync(' ', (short)(col<<4), x, y);
                }
            }
        }




        public override void OnPostDraw()
        {

            CGBuffer.AddAsync('&', (short)(((int)SelectedColor << 4) | ((SelectedColor == 0) ? 15 : 0)),
                             (int)c_DrawingCanvasPos.X + m_cursorX , (int)c_DrawingCanvasPos.Y + m_cursorY );
        }
        public override void OnExit() { }
    }
}
