using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.RasterizerPipeline
{
    public class Vertex
    {
        NEVector4 m_Position;
        NEVector2 m_UVs;

        public NEVector4 Position { get { return m_Position; } }
        public NEVector2 UV { get { return m_UVs; } }


        public float X { get { return m_Position.X; } }
        public float Y { get { return m_Position.Y; } }
        public float Z { get { return m_Position.Z; } }

        public float U { get { return m_UVs.X; } }
        public float V { get { return m_UVs.Y; } }

        

        public Vertex(float x, float y, float z)
        {
            m_Position = new NEVector4(x, y, z, 0.0f);
            m_UVs = new NEVector2(0.0f, 0.0f);
        }

        public Vertex(float x, float y, float z, float u, float v)
        {
            m_Position = new NEVector4(x, y, z, 0.0f);
            m_UVs = new NEVector2(u, v);
        }

        public static void Swap(ref Vertex v1, ref Vertex v2)
        {
            Vertex tmp = v1;
            v1 = v2;
            v2 = tmp;
            return;
        }

        public void Translate(float x, float y, float z)
        {
            m_Position.X += x;
            m_Position.Y += y;
            m_Position.Z += z;
        }

        public void Translate(NEVector4 delta)
        {
            m_Position += delta;
        }

        public void SetValue(Vertex v)
        {
            m_Position = v.Position;
            m_UVs = v.UV;
        }

    }
}
