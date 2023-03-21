using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{
    public class NEStaticSprite
    {
        private NEVector2 m_Position;

        public float X { get { return m_Position.X; } set { m_Position.X = value; } }
        public float Y { get { return m_Position.Y; } set { m_Position.Y = value; } }


        public NEVector2 Position { get { return m_Position; } set { m_Position = value; } }
        public float AstpectRatio { get { return ((float)Texture.Width) / ((float)Texture.Height); } }

        public NETexture Texture { get; private set; }

        public NEStaticSprite(NETexture texture)
        {
            Texture = texture;
            m_Position = new NEVector2();
        }

    }

    public class NEStaticSpriteLuma
    {
        private NEVector2 m_Position;

        public float X { get { return m_Position.X; } set { m_Position.X = value; } }
        public float Y { get { return m_Position.Y; } set { m_Position.Y = value; } }


        public NEVector2 Position { get { return m_Position; } set { m_Position = value; } }
        public float AstpectRatio { get { return ((float)Texture.Width) / ((float)Texture.Height); } }

        public NEFloatBuffer Texture { get; private set; }

        public NEStaticSpriteLuma(NEFloatBuffer texture)
        {
            Texture = texture;
            m_Position = new NEVector2();
        }

    }
}
