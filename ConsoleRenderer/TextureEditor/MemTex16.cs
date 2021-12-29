using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer.TextureEditor
{
    class MemTex16
    {
        public class MT16Pix
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Col { get; set; }
            public int Metadata { get; set; }

            public MT16Pix LEFT { get; set; }
            public MT16Pix RIGHT { get; set; }
            public MT16Pix DOWN { get; set; }
            public MT16Pix UP { get; set; }

            public MT16Pix(int x, int y, int col)
            {
                X = x;
                Y = y;
                Col = col;
            }
        }

        MT16Pix[] m_Pixels;
        public int Width { get; private set; }
        public int Height { get; private set; }

        int XY2I(int x, int y)
        {
            return (Width * y + x);
        }

        public MemTex16(int w, int h)
        {
            Width = w;
            Height = h;
            m_Pixels = new MT16Pix[w * h];
            for(int y = 0; y < h;++y)
            {
                for(int x= 0; x<w;++x)
                {
                    MT16Pix pixel = new MT16Pix(x, y, 15);
                    pixel.Metadata = XY2I(x, y);
                    if ((y!=0))
                    {
                        pixel.UP = m_Pixels[XY2I(x, y - 1)];
                        pixel.UP.DOWN = pixel;
                    }
                    if(x!=0)
                    {
                        pixel.LEFT = m_Pixels[XY2I(x - 1, y)];
                        pixel.LEFT.RIGHT = pixel;
                    }

                    m_Pixels[XY2I(x,y)] = pixel;
                }
            }
        }

        public int GetColor(int x, int y)
        {
            return m_Pixels[XY2I(x, y)].Col;
        }
        public MT16Pix GetPixel(int x, int y)
        {
            return m_Pixels[XY2I(x, y)];
        }

    }
}
