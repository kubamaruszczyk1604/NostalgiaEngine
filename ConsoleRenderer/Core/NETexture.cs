using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{
    public abstract class NETexture
    {
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public abstract NEColorSample Sample(float u, float v, float intensity);
    }
}
