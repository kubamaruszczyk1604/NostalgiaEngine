using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer.Core
{
    public struct CGRectangle
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
    }

    public struct CGPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

    }


    public class CGPixel
    {
        public int X;
        public int Y;
        public int Col;

        public CGPixel(int x, int y, int col)
        {
            X = x;
            Y = y;
            Col = col;
        }
    }
}
