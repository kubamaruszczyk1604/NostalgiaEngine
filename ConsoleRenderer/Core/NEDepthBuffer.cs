using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{
    public class NEDepthBuffer
    {
        public float[] DATA { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }


        public NEDepthBuffer(int width, int height, float initalDepthVal = float.PositiveInfinity)
        {
            Width = width;
            Height = height;
            DATA = new float[width * height];
            for(int i = 0; i < DATA.Length; ++i)
            {
                DATA[i] = initalDepthVal;
            }
        }

        private int XY2I(int x, int y)
        {
            return (Width * y + x);
        }
    }
}
