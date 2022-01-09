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
        private readonly int VISITED = 10;
        private  MT16Pix[] m_Pixels;
        private int m_OrigColFlood;
        private int m_NewColFlood;

        public int Width { get; private set; }
        public int Height { get; private set; }

        static public MemTex16 Copy(MemTex16 source)
        {
            return new MemTex16(source);
        }

        private int XY2I(int x, int y)
        {
            return (Width * y + x);
        }

        public MemTex16(MemTex16 source)
        {
            Width = source.Width;
            Height = source.Height;
            CreateTexture(source.Width, source.Height, source.m_Pixels);
        }

        void CreateTexture(int w, int h, MT16Pix[] source = null)
        {
            m_Pixels = new MT16Pix[w * h];

            for (int y = 0; y < h; ++y)
            {
                for (int x = 0; x < w; ++x)
                {
                    MT16Pix pixel = new MT16Pix(x, y, (source==null)?16:source[XY2I(x,y)].Col);
                    pixel.Metadata = 0;
                    if ((y != 0))
                    {
                        pixel.UP = m_Pixels[XY2I(x, y - 1)];
                        pixel.UP.DOWN = pixel;
                    }
                    if (x != 0)
                    {
                        pixel.LEFT = m_Pixels[XY2I(x - 1, y)];
                        pixel.LEFT.RIGHT = pixel;
                    }

                    m_Pixels[XY2I(x, y)] = pixel;
                }
            }
        }
        public MemTex16(int w, int h)
        {
            Width = w;
            Height = h;
            CreateTexture(w, h);
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
                   if (pixel.Metadata == VISITED) continue;
                    pixel.Metadata = VISITED;
                    callCnt++;
                    if (pixel.LEFT != null)
                    {
                        if(pixel.LEFT.Col == m_OrigColFlood  && pixel.LEFT.Metadata == 0)
                        {
                            temp.Add(pixel.LEFT);
                            keepGoing = true;
                        }
                    }
                    if (pixel.RIGHT != null)
                    {
                        if (pixel.RIGHT.Col == m_OrigColFlood && pixel.RIGHT.Metadata == 0)
                        {
                           temp.Add(pixel.RIGHT);
                            keepGoing = true;
                        }
                    }
                    if (pixel.DOWN != null)
                    {
                        if (pixel.DOWN.Col == m_OrigColFlood && pixel.DOWN.Metadata == 0)
                        {
                            temp.Add(pixel.DOWN);
                            keepGoing = true;
                        }
                    }
                    if (pixel.UP != null)
                    {
                        if (pixel.UP.Col == m_OrigColFlood && pixel.UP.Metadata == 0)
                        {
                            temp.Add(pixel.UP);
                            keepGoing = true;
                        }
                    }
                   

                }
                l.AddRange(temp);
            }


            foreach (var pixel in l)
            {
                pixel.Col = m_NewColFlood;
                pixel.Metadata = 0;
            }
        }

        public void SetPixel(int x, int y, int color)
        {
            m_Pixels[XY2I(x, y)].Col = color;
        }
        

        static int callCnt = 0;


        public int GetColor(int x, int y)
        {
            return m_Pixels[XY2I(x, y)].Col;
        }
        public MT16Pix GetPixel(int x, int y)
        {
            return m_Pixels[XY2I(x, y)];
        }


        public string AsString(int w, int h)
        {
            string ret = "";
            for(int y =0; y< h; y++)
            {
                for (int x = 0; x < w; ++x)
                {
                    ret += m_Pixels[XY2I(x, y)].Col.ToString() + ((x==w-1)?"\n":",") ;
                }
            }
            //for (int i = 0; i < m_Pixels.Length;++i)
            //{
            //    ret += m_Pixels[i].Col + ((i % Width == 0) ? "\n" : ",");
            //}
            return ret;
        }
    }
}
