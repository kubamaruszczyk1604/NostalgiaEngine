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

            public MT16Pix [] Neighbours { get; private set; }
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
                    MT16Pix pixel = new MT16Pix(x, y, (x==5&&y!=5)?1:15);
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
        public void FloodFill(int x, int y, int color)
        {
            m_OrigColFlood = m_Pixels[XY2I(x, y)].Col;
            m_NewColFlood = color;
            List<MT16Pix> l = new List<MT16Pix>();
            MT16Pix curr = m_Pixels[XY2I(x, y)];
            l.Add(curr);
            bool keepGoing = true;
            while (keepGoing)
            {
                List<MT16Pix> temp = new List<MT16Pix>();
                keepGoing = false;
                foreach (var pixel in l)
                {
                    if(pixel.LEFT != null)
                    {
                        if(pixel.LEFT.Col == m_OrigColFlood  && pixel.LEFT.Metadata != 10)
                        {
                            pixel.LEFT.Metadata = 10;
                            temp.Add(pixel.LEFT);
                            keepGoing = true;
                        }
                    }
                    if (pixel.RIGHT != null)
                    {
                        if (pixel.RIGHT.Col == m_OrigColFlood && pixel.RIGHT.Metadata != 10)
                        {
                            pixel.RIGHT.Metadata = 10;
                           temp.Add(pixel.RIGHT);
                            keepGoing = true;
                        }
                    }
                    if (pixel.DOWN != null)
                    {
                        if (pixel.DOWN.Col == m_OrigColFlood && pixel.DOWN.Metadata != 10)
                        {
                            pixel.DOWN.Metadata = 10;
                            temp.Add(pixel.DOWN);
                            keepGoing = true;
                        }
                    }
                    if (pixel.UP != null)
                    {
                        if (pixel.UP.Col == m_OrigColFlood && pixel.UP.Metadata != 10)
                        {
                            pixel.UP.Metadata = 10;
                            temp.Add(pixel.UP);
                            keepGoing = true;
                        }
                    }
                   
                }
                l.AddRange(temp);
            }
           // Flood(m_Pixels[XY2I(x, y)]);

            foreach(var pixel in l)
            {
                pixel.Col = m_NewColFlood;
                pixel.Metadata = 0;
            }
        }
        int m_OrigColFlood;
        int m_NewColFlood;
        static int callCnt = 0;
        private void Flood(MT16Pix pixel)
        {
            callCnt++;
            if (callCnt > 5000) return;
            if (pixel.Col != m_OrigColFlood) return;
            pixel.Col = m_NewColFlood;
            if (pixel.LEFT != null)
            {
                Flood(pixel.LEFT);

            }
            if (pixel.RIGHT != null)
            {
                Flood(pixel.RIGHT);

            }
            if (pixel.DOWN != null)
            {
                Flood(pixel.DOWN);

            }
            if (pixel.UP != null)
            {
                Flood(pixel.UP);

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
