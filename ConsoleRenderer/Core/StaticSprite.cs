using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{
    public class NEStaticSprite
    {
        public int X { get; set; }
        public int Y { get; set; }

        public NETexture Texture { get; private set; }

        public NEStaticSprite(NETexture texture)
        {
            Texture = texture;
        }

    }
}
