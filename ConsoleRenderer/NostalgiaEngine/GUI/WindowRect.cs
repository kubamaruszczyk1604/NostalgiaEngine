using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.ConsoleGUI
{
    public class NEWindowRect
    {

        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }
        public string Title { get; set; }
        public int BarColor { get; set; }
        public int TextBarColor { get; set; }
        public int BodyColor { get; set; }

        public NEWindowRect(int x, int y, int w, int h, string title = "")
        {
            X = x;
            Y = y;
            W = w;
            H = h;
            Title = title;
            BarColor = 9;
            TextBarColor = 15;
            BodyColor = 8;
        }

        public void Draw()
        {
            for(int x = X; x < (X+W); ++x)
            {
                for (int y = Y; y < (Y+H); ++y)
                {
                    int col = (y == Y) ? (BarColor << 4)|TextBarColor : (BodyColor << 4);
                    char c = (y == Y)&& (x-X<Title.Length) ? Title[x-X] : ' ';
                    NEScreenBuffer.PutChar(c,(short)col,  x, y);
                }
            }
        }
    }
}
