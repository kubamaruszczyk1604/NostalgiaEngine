using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleRenderer.Core;
using ConsoleRenderer;
using System.Runtime.InteropServices;
using OpenTK;
using System.IO;

namespace ConsoleRenderer.TextureEditor
{
    class CGTextureEditor : CGScene
    {




        private readonly Vector2 c_ColorPanelPos = new Vector2(2, 0);
        private readonly Vector2 c_DrawingCanvasPos = new Vector2(3, 15);
        private readonly int c_ColorWindowWidth = 6;
        private readonly int c_ColorWindowHeight = 8;
        private readonly int c_MinImageH = 5;
        private readonly int c_MinImageW = 5;
        private readonly int c_MaxImageW = 100;
        private readonly int c_MaxImageH = 70;
        private readonly Vector2[] c_Directions = new Vector2[] { new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, -1), new Vector2(0, 1) };


        private readonly string fl = "MODE(Shift)";
        private readonly string c_Fill = "FILL";
        private readonly string c_Brush = "BRUSH";

        private bool m_BrushFlag = true;
        private int m_cursorX;
        private int m_cursorY;
        private int m_BrushW;
        private int m_BrushH;

        private int m_ImageW;
        private int m_ImageH;

        private bool m_ActionStarted;




        private int SelectedColor;
        private MemTex16 m_ImageData;

        public override void OnInitialize()
        {
            ScreenWidth = 110;
            ScreenHeight = 87;
            PixelWidth = 8;
            PixelHeight = 8;
            m_ImageW = 100;
            m_ImageH = 70;
            m_BrushW = 1;
            m_BrushH = 1;
            SelectedColor = 10;
            m_ImageData = new MemTex16(m_ImageW, m_ImageH);
            m_cursorX = 10;
            m_cursorY = 10;
            FlushKeys();
            m_ActionStarted = false;
        }

        private void FlushKeys()
        {
            CGInput.CheckKeyPress(ConsoleKey.Tab);
            CGInput.CheckKeyPress(ConsoleKey.UpArrow);
            CGInput.CheckKeyPress(ConsoleKey.DownArrow);
            CGInput.CheckKeyPress(ConsoleKey.LeftArrow);
            CGInput.CheckKeyPress(ConsoleKey.RightArrow);
            CGInput.CheckKeyDown(ConsoleKey.Spacebar);
            CGInput.CheckKeyPress(ConsoleKey.Enter);
            CGInput.CheckKeyPress(ConsoleKey.N);
            CGInput.CheckKeyPress(ConsoleKey.Escape);
        }
        public override void OnStart()
        {
            WindowControl.DisableConsoleWindowButtons();

        }
        CGPoint m_LastMouse = new CGPoint();
        public override void OnUpdate(float deltaTime)
        {
            //CGPoint mp = CGInput.GetMousePostion();
            //// Console.Title = (mp.X / PixelWidth).ToString() + ", " + mp.Y.ToString();
            //// if ((Math.Abs(mp.X - m_LastMouse.X) > 0) && (Math.Abs(mp.Y - m_LastMouse.Y) > 0))
            //{
            //    m_cursorX = (mp.X / PixelWidth) - 8;
            //    m_cursorY = (mp.Y) / PixelHeight - 15;
            //}
           // m_LastMouse = mp;
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
                else if (CGInput.CheckKeyDown(ConsoleKey.C))
                {
                    SelectedColor--;

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
                else if (CGInput.CheckKeyDown(ConsoleKey.C))
                {
                    SelectedColor++;

                }
                else
                {
                    m_cursorX += 1;
                }
            }
            //m_cursorY = m_cursorY % (m_ImageH-m_BrushH);
            if (m_cursorY >= (m_ImageH - m_BrushH)) m_cursorY = (m_ImageH - m_BrushH);
            if (m_cursorY < 0) m_cursorY = 0;

            // m_cursorX = m_cursorX % m_ImageW;
            if (m_cursorX < 0) m_cursorX = 0;
            if (m_cursorX >= (m_ImageW - m_BrushW)) m_cursorX = (m_ImageW - m_BrushW);

            SelectedColor %= 17;
            if (SelectedColor < 0) SelectedColor = 16;


            if (CGInput.CheckKeyDown(ConsoleKey.Spacebar))
            {
                if (m_BrushFlag)
                {
                    for (int w = 0; w < m_BrushW; ++w)
                    {
                        for (int h = 0; h < m_BrushH; ++h)
                        {
                            m_ImageData.SetPixel(m_cursorX + w, m_cursorY + h, SelectedColor);
                        }
                    }
                    
                }
                else
                {
                    m_ImageData.FloodFill(m_cursorX, m_cursorY, SelectedColor);
                    
                }
                m_ActionStarted = true;
            }
            else
            {
                if(m_ActionStarted)
                {
                    OnActionFinished();
                    m_ActionStarted = false;
                }
            }

            if(CGInput.CheckKeyPress(ConsoleKey.Enter))
            {
                m_ImageData.FloodFill(m_cursorX, m_cursorY, SelectedColor);
                OnActionFinished();
            }
            if (CGInput.CheckKeyPress((ConsoleKey)0x10))
            {
                if (m_BrushFlag == false) m_BrushFlag = true;
                else m_BrushFlag = false;

            }

            if (CGInput.CheckKeyPress(ConsoleKey.N))
            {
                // CGEngine.Instance.PushScene(new CGRaytracer2D());
                var sd = new CGSaveDialog();
                sd.onSceneExit += OnSave;
                CGEngine.Instance.PushScene(sd);
            }
            if (CGInput.CheckKeyPress(ConsoleKey.Escape))
            {
                Exit();
            }
        }
        private void OnSave(CGScene scene)
        {
           
            string texStr = m_ImageData.AsString(m_ImageW,m_ImageH);
            string path = scene.ReturnData.ToString();
            if(path != "0")
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.Write(texStr);
                    writer.Close();
                }
            }
        }

        private void OnActionFinished()
        {
           
            Console.Beep();
        }
        private void DrawPalette(int x, int y)
        {
            Vector2 pixelPos = new Vector2(x, y);
            for (int i = 0; i < 17; ++i)
            {
                if (CGHelper.InRectangle(pixelPos, new Vector2(c_ColorWindowWidth * (i), 0) + c_ColorPanelPos, c_ColorWindowWidth, c_ColorWindowHeight))
                {
                    CGBuffer.PutChar(' ', (short)((i) << 4), x, y);
                    if (i == 16) CGBuffer.PutChar((char)CGBlock.Solid, (short)(8 << 4), x, y);
                }

            }

            if (CGHelper.InRectangle(pixelPos, new Vector2(c_ColorWindowWidth * SelectedColor, 8) + c_ColorPanelPos, c_ColorWindowWidth, 2))
            {
                CGBuffer.PutChar((char)CGBlock.Middle, (short)((15)), x, y);
            }
        }

        public override void OnResume()
        {
            base.OnResume();
            FlushKeys();
        }

        public override void OnPause()
        {
            base.OnPause();
        }
        public override void OnDrawPerColumn(int x)
        {

            for (int y = 0; y < ScreenHeight; ++y)
            {

                CGBuffer.PutChar((char)CGBlock.Weak, (short)ConsoleColor.DarkBlue, x, y);


                DrawPalette(x, y);

 
                if (x >= (int)c_DrawingCanvasPos.X && x < m_ImageW + (int)c_DrawingCanvasPos.X && 
                    y >= (int)c_DrawingCanvasPos.Y && y< m_ImageH + (int)c_DrawingCanvasPos.Y )
                {
                   int col = m_ImageData.GetColor(x- (int)c_DrawingCanvasPos.X, y- (int)c_DrawingCanvasPos.Y);
                    if (col == 16)
                    {
                        CGBuffer.PutChar((char)CGBlock.Solid, (short)(8<<4), x, y);
                    }
                    else
                    {
                        CGBuffer.PutChar(' ', (short)(col << 4), x, y);
                    }
                }
            }
        }




        public override void OnDraw()
        {
            for(int w = 0; w < m_BrushW;++w)
            {
                for (int h =0; h<m_BrushH;++h)
                {
                    CGBuffer.PutChar('&', (short)(((int)SelectedColor << 4) | ((SelectedColor == 0) ? 15 : 0)),
                 (int)c_DrawingCanvasPos.X + m_cursorX+w, (int)c_DrawingCanvasPos.Y + m_cursorY+h);
                }
            }

            int offset = (int)c_DrawingCanvasPos.X;
            for (int i = 0; i < fl.Length;++i)
            {
                CGBuffer.PutChar(fl[i], 8<<4 , offset + i, (int)c_DrawingCanvasPos.Y-2);
            }
            offset += fl.Length + 2;

            for (int i = 0; i < c_Fill.Length; ++i)
            {
                CGBuffer.PutChar(c_Fill[i], (short)(m_BrushFlag?8:10), offset + i, (int)c_DrawingCanvasPos.Y - 2);
            }
            offset += c_Fill.Length + 2;
            for (int i = 0; i < c_Brush.Length; ++i)
            {
                CGBuffer.PutChar(c_Brush[i], (short)(m_BrushFlag ? 10 : 8), offset + i, (int)c_DrawingCanvasPos.Y - 2);
            }
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
