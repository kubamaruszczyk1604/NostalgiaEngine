using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;
using NostalgiaEngine;
using System.Runtime.InteropServices;
using System.IO;
using NostalgiaEngine.GUI;

namespace NostalgiaEngine.TextureEditor
{
    class NETextureEditor : NEScene
    {

        private readonly NEVector2 c_ColorPanelPos = new NEVector2(2, 0);
        private readonly NEVector2 c_DrawingCanvasPos = new NEVector2(3, 15);
        private readonly int c_ColorWindowWidth = 6;
        private readonly int c_ColorWindowHeight = 8;
        private readonly int c_MinImageH = 5;
        private readonly int c_MinImageW = 5;
        private readonly int c_MaxImageW = 100;
        private readonly int c_MaxImageH = 70;
        private readonly NEVector2[] c_Directions = new NEVector2[] { new NEVector2(-1, 0), new NEVector2(1, 0), new NEVector2(0, -1), new NEVector2(0, 1) };


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
        //private MemTex16 m_ImageData;
        private StepHistorySeries<MemTex16> m_ImageData;

        public override bool OnLoad()
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
            m_ImageData = new StepHistorySeries<MemTex16>(20, new MemTex16(m_ImageW, m_ImageH));
            m_cursorX = 10;
            m_cursorY = 10;
            m_ActionStarted = false;

            return true;
        }

       // private void FlushKeys()
        //{
        //    NEInput.FlushKeyboard();
        //}
        public override void OnStart()
        {
            NEWindowControl.DisableConsoleWindowButtons();

        }
        NEPoint m_LastMouse = new NEPoint();
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
            if (NEInput.CheckKeyPress(ConsoleKey.UpArrow))
            {
                if (NEInput.CheckKeyDown(NEKey.Control))
                {
                    m_ImageH--;
                    if (m_ImageH < c_MinImageH) m_ImageH = c_MinImageH;
                }
                else
                {
                    m_cursorY -= 1;
                }
            }

            if (NEInput.CheckKeyPress(ConsoleKey.DownArrow))
            {
                if (NEInput.CheckKeyDown(NEKey.Control))
                {
                    m_ImageH++;
                    if (m_ImageH > c_MaxImageH) m_ImageH = c_MaxImageH;
                }
                else
                {
                    m_cursorY += 1;
                }
            }

            if (NEInput.CheckKeyPress(ConsoleKey.LeftArrow))
            {

                if (NEInput.CheckKeyDown(NEKey.Control))
                {
                    m_ImageW--;
                    if (m_ImageW < c_MinImageW) m_ImageW = c_MinImageW;
                }
                else if (NEInput.CheckKeyDown(ConsoleKey.C))
                {
                    SelectedColor--;
                }
                else
                {
                    m_cursorX -= 1;
                }
            }

            if (NEInput.CheckKeyPress(ConsoleKey.RightArrow))
            {
                if (NEInput.CheckKeyDown(NEKey.Control))
                {
                    m_ImageW++;
                    if (m_ImageW > c_MaxImageW) m_ImageW = c_MaxImageW;

                }
                else if (NEInput.CheckKeyDown(ConsoleKey.C))
                {
                    SelectedColor++;
                }
                else
                {
                    m_cursorX += 1;
                }
            }

            if (m_cursorY >= (m_ImageH - m_BrushH)) m_cursorY = (m_ImageH - m_BrushH);
            if (m_cursorY < 0) m_cursorY = 0;
            if (m_cursorX < 0) m_cursorX = 0;
            if (m_cursorX >= (m_ImageW - m_BrushW)) m_cursorX = (m_ImageW - m_BrushW);

            SelectedColor %= 17;
            if (SelectedColor < 0) SelectedColor = 16;


            if (NEInput.CheckKeyDown(ConsoleKey.Spacebar))
            {
                if(!m_ActionStarted)
                {
                    OnActionStarted();
                    m_ActionStarted = true;
                }
                if (m_BrushFlag)
                {
                    for (int w = 0; w < m_BrushW; ++w)
                    {
                        for (int h = 0; h < m_BrushH; ++h)
                        {
                            m_ImageData.Data.SetPixel(m_cursorX + w, m_cursorY + h, SelectedColor);
                        }
                    }
                }
                else
                {
                    m_ImageData.Data.FloodFill(m_cursorX, m_cursorY, SelectedColor);
                    
                }
            }
            else
            {
                m_ActionStarted = false;
            }

            if(NEInput.CheckKeyPress(ConsoleKey.Enter))
            {
                OnActionStarted();
                m_ImageData.Data.FloodFill(m_cursorX, m_cursorY, SelectedColor);
                
            }

            if (NEInput.CheckKeyPress(NEKey.Shift))
            {
                if (m_BrushFlag == false) m_BrushFlag = true;
                else m_BrushFlag = false;

            }

            if(NEInput.CheckKeyDown((ConsoleKey)NEKey.Control) && NEInput.CheckKeyPress(ConsoleKey.Z))
            {
                m_ImageData.UndoStep();
            }
            if (NEInput.CheckKeyDown((ConsoleKey)NEKey.Control) && NEInput.CheckKeyPress(ConsoleKey.O))
            {
                var od = new NEOpenDialog();
                od.onSceneExit += OnFileOpen;
                Engine.Instance.PushScene(od);
            }
            if (NEInput.CheckKeyDown((ConsoleKey)NEKey.Control) && NEInput.CheckKeyPress(ConsoleKey.S))
            {
                var sd = new NESaveDialog();
                sd.onSceneExit += OnSave;
                Engine.Instance.PushScene(sd);

            }

            if (NEInput.CheckKeyDown((ConsoleKey)NEKey.Control) && NEInput.CheckKeyPress(ConsoleKey.D1))
            {

                NEColorManagement.SetSpectralPalette1();
                NEScreenBuffer.Reallign();
            }

            if (NEInput.CheckKeyPress(ConsoleKey.Escape))
            {
                Exit();
            }


        }
        private void OnSave(NEScene scene)
        {
           
            string texStr = m_ImageData.Data.AsString(m_ImageW,m_ImageH);
            string path = scene.ReturnData.ToString();
            if(path != "0")
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(path))
                    {
                        writer.Write(texStr);
                        writer.Close();
                    }
                    NEConsoleSounds.ConfirmBeep2();
                }
                catch
                {
                    NEConsoleSounds.ForbidenBeep();
                }
            }
        }


        private void OnFileOpen(NEScene scene)
        {
            NEColorTexture16 nativeTex = NEColorTexture16.LoadFromFile(scene.ReturnData.ToString()); 
            if(nativeTex != null)
            {
                m_ImageData.AddStep(new MemTex16(nativeTex));
                m_ImageW = nativeTex.Width;
                m_ImageH = nativeTex.Height;
            }
            else
            {
                NEConsoleSounds.ErrorBeep();
            }

        }

        private void OnActionStarted()
        { 
            MemTex16 next = MemTex16.Copy(m_ImageData.Data);
            m_ImageData.AddStep(next);
        }
        private void DrawPalette(int x, int y)
        {
            NEVector2 pixelPos = new NEVector2(x, y);
            for (int i = 0; i < 17; ++i)
            {
                if (NEMathHelper.InRectangle(pixelPos, new NEVector2(c_ColorWindowWidth * (i), 0) + c_ColorPanelPos, c_ColorWindowWidth, c_ColorWindowHeight))
                {
                    NEScreenBuffer.PutChar(' ', (short)((i) << 4), x, y);
                    if (i == 16) NEScreenBuffer.PutChar((char)NEBlock.Solid, (short)(8 << 4), x, y);
                }
            }

            if (NEMathHelper.InRectangle(pixelPos, new NEVector2(c_ColorWindowWidth * SelectedColor, 8) + c_ColorPanelPos, c_ColorWindowWidth, 2))
            {
                NEScreenBuffer.PutChar((char)NEBlock.Middle, (short)((15)), x, y);
            }
        }

        public override void OnResume()
        {
            base.OnResume();
            NEColorManagement.SetNostalgiaPalette();
        }

        public override void OnPause()
        {
            base.OnPause();
        }

        public override void OnDrawPerColumn(int x)
        {

            for (int y = 0; y < ScreenHeight; ++y)
            {

                NEScreenBuffer.PutChar((char)NEBlock.Weak, (short)ConsoleColor.DarkBlue, x, y);
                DrawPalette(x, y);
 
                if (x >= (int)c_DrawingCanvasPos.X && x < m_ImageW + (int)c_DrawingCanvasPos.X && 
                    y >= (int)c_DrawingCanvasPos.Y && y< m_ImageH + (int)c_DrawingCanvasPos.Y )
                {
                   int col = m_ImageData.Data.GetColor(x- (int)c_DrawingCanvasPos.X, y- (int)c_DrawingCanvasPos.Y);
                    if (col == 16)
                    {
                        NEScreenBuffer.PutChar((char)NEBlock.Solid, (short)(8<<4), x, y);
                    }
                    else
                    {
                        NEScreenBuffer.PutChar(' ', (short)(col << 4), x, y);
                    }
                }
            }
        }


        public override void OnDraw()
        {
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;
            for (int w = 0; w < m_BrushW;++w)
            {
                for (int h =0; h<m_BrushH;++h)
                {
                    NEScreenBuffer.PutChar('&', (short)(((int)SelectedColor << 4) | ((SelectedColor == 0) ? 15 : 0)),
                 (int)c_DrawingCanvasPos.X + m_cursorX+w, (int)c_DrawingCanvasPos.Y + m_cursorY+h);
                }
            }

            int offset = (int)c_DrawingCanvasPos.X;
            for (int i = 0; i < fl.Length;++i)
            {
                NEScreenBuffer.PutChar(fl[i], 8<<4 , offset + i, (int)c_DrawingCanvasPos.Y-2);
            }
            offset += fl.Length + 2;

            for (int i = 0; i < c_Fill.Length; ++i)
            {
                NEScreenBuffer.PutChar(c_Fill[i], (short)(m_BrushFlag?8:10), offset + i, (int)c_DrawingCanvasPos.Y - 2);
            }
            offset += c_Fill.Length + 2;
            for (int i = 0; i < c_Brush.Length; ++i)
            {
                NEScreenBuffer.PutChar(c_Brush[i], (short)(m_BrushFlag ? 10 : 8), offset + i, (int)c_DrawingCanvasPos.Y - 2);
            }
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
