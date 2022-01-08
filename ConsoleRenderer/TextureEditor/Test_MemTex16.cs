using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleRenderer.Core;
using ConsoleRenderer;

namespace ConsoleRenderer.TextureEditor
{
    class Test_MemTex16: CGScene
    {
        MemTex16 m_MemTex16;
        public override void OnInitialize()
        {
            ScreenWidth = 100;
            ScreenHeight = 100;
            PixelWidth = 8;
            PixelHeight = 8;
            m_MemTex16 = new MemTex16(80, 80);
            m_Current = m_MemTex16.GetPixel(1, 1);
        }
        public MemTex16.MT16Pix m_Current = null;
        public override void OnStart() { }
        public override void OnUpdate(float deltaTime)
        {
            if(CGInput.CheckKeyPress(ConsoleKey.DownArrow))
            {
                if(m_Current.DOWN != null)
                {
                    m_Current = m_Current.DOWN;
                }
            }
            if (CGInput.CheckKeyPress(ConsoleKey.UpArrow))
            {
                if (m_Current.UP!= null)
                {
                    m_Current = m_Current.UP;
                }
            }
            if (CGInput.CheckKeyPress(ConsoleKey.LeftArrow))
            {
                if (m_Current.LEFT != null)
                {
                    m_Current = m_Current.LEFT;
                }
            }
            if (CGInput.CheckKeyPress(ConsoleKey.RightArrow))
            {
                if (m_Current.RIGHT != null)
                {
                    m_Current = m_Current.RIGHT;
                }
            }

            if(CGInput.CheckKeyPress(ConsoleKey.Spacebar))
            {
                m_MemTex16.FloodFill(m_Current.X, m_Current.Y, col);
                col++;
            }


            if (CGInput.CheckKeyPress(ConsoleKey.Escape))
            {
                Exit();
            }
        }
        int col = 10;
        public override void OnDrawPerColumn(int x) { }
        public override void OnDraw()
        {
            for(int x = 0; x < m_MemTex16.Width;++x)
            {
                for(int y = 0; y< m_MemTex16.Height;++y)
                {
                    MemTex16.MT16Pix pix = m_MemTex16.GetPixel(x, y);
                    bool d = pix == m_Current;
                    CGBuffer.PutChar( d?'X':'0', (short)m_MemTex16.GetColor(x,y), x, y);
                }
            }
        }
        public override void OnExit() { }
    }
}
