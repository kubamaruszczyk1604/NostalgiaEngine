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

        public NEVector4 Position { get { return m_Position; } set { m_Position = value; } }
        public NEVector2 UV { get { return m_UVs; }  set { m_UVs = value; } }


        public float X { get { return m_Position.X; } }
        public float Y { get { return m_Position.Y; } }
        public float Z { get { return m_Position.Z; } }
        public float W { get { return m_Position.W; } }

        public float U { get { return m_UVs.X; } }
        public float V { get { return m_UVs.Y; } }

        public float UnidividedW { get; private set; }
        public NEVector4 Vert2Camera { get; set; }


        public Vertex(float x, float y, float z)
        {
            m_Position = new NEVector4(x, y, z, 1.0f);
            m_UVs = new NEVector2(0.0f, 0.0f);
        }

        public Vertex(float x, float y, float z, float u, float v)
        {
            m_Position = new NEVector4(x, y, z, 1.0f);
            m_UVs = new NEVector2(u, v);
        }

        public static void Swap(ref Vertex v1, ref Vertex v2)
        {
            Vertex tmp = v1;
            v1 = v2;
            v2 = tmp;
            return;
        }


        public void SetValue(Vertex v)
        {
            m_Position = v.Position;
            m_UVs = v.UV;
        }
        float oldX = 0.0f;
        public void WDivide()
        {
            oldX= m_Position.X;
            float posDiv = m_Position.W <= 0.0f ? 0.001f : m_Position.W;
            m_Position.X /= posDiv;
            m_Position.Y /= posDiv;
            m_Position.Z /= posDiv;
            m_UVs.X /= posDiv;
            m_UVs.Y /= posDiv;
            UnidividedW = m_Position.W;
            m_Position.W = 1.0f/posDiv;
        }

        public Vertex Duplicate()
        {
            return new Vertex(X, Y, Z, U, V);
        }
        override public string ToString()
        {
            return m_Position.X.ToString() + ", " + m_Position.Y.ToString() + ", " + UnidividedW.ToString() + ",   " + oldX.ToString();

        }

        public string ToString2()
        {
            return m_Position.X.ToString() + ", " + m_Position.W.ToString() + ", " + UnidividedW.ToString();

        }

    }
}
