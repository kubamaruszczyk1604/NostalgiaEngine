using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.GUI
{
    public class NEWindowRect
    {

        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }
        public string Title { get; set; }

        public NEWindowRect(int x, int y, int w, int h, string title = "")
        {
            X = x;
            Y = y;
            W = w;
            H = h;
            Title = title;
        }

        public void Draw()
        {
            for(int x = X; x < (X+W); ++x)
            {
                for (int y = Y; y < (Y+H); ++y)
                {
                    int col = (y == Y) ? (9 << 4)|15 : (8 << 4);
                    char c = (y == Y)&& (x-X<Title.Length) ? Title[x-X] : ' ';
                    NEConsoleScreen.PutChar(c,(short)col,  x, y);
                }
            }
        }
    }
}
