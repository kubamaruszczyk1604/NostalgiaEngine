using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{
    public class NEStaticSprite
    {
        public float X { get; set; }
        public float Y { get; set; }

        public NEVector2 Position { get { return new NEVector2(X, Y); } }
        public float AstpectRatio { get { return ((float)Texture.Width) / ((float)Texture.Height); } }

        public NETexture Texture { get; private set; }

        public NEStaticSprite(NETexture texture)
        {
            Texture = texture;
        }

    }
}
