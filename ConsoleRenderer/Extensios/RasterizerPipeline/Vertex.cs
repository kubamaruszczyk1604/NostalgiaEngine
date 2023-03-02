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

        public float ZInViewSpace { get; private set; }
        public NEVector4 Vert2Camera { get; set; }

        public  bool m_WDividedFlag;

        public Vertex(float x, float y, float z)
        {
            m_Position = new NEVector4(x, y, z, 1.0f);
            m_UVs = new NEVector2(0.0f, 0.0f);
            m_WDividedFlag = false;
        }

        public Vertex(float x, float y, float z, float u, float v)
        {
            m_Position = new NEVector4(x, y, z, 1.0f);
            m_UVs = new NEVector2(u, v);
            m_WDividedFlag = false;
        }

        public static void Swap(ref Vertex v1, ref Vertex v2)
        {
            Vertex tmp = v1;
            v1 = v2;
            v2 = tmp;
            return;
        }

        public void AddZ(float v)
        {
            m_Position.Z += v;
        }
        public void SetValue(Vertex v)
        {
            m_Position = v.Position;
            m_UVs = v.UV;
        }
       // float oldX = 0.0f;
        public void WDivide()
        {
            if (m_WDividedFlag) return;
            //oldX= m_Position.X;
            m_WDividedFlag = true; 
            float posDiv = m_Position.W /*<= 0.0f ? 0.001f : m_Position.W*/;
            float signZ = Math.Sign(m_Position.W);
            m_Position.X /= posDiv;
            m_Position.Y /= posDiv;
            m_Position.Z /= posDiv;
            m_Position.Z *= signZ;
            m_UVs.X /= posDiv;
            m_UVs.Y /= posDiv;
            ZInViewSpace = m_Position.W;
            m_Position.W = 1.0f/posDiv;
        }

        static public Vertex Lerp(Vertex v0, Vertex v1, float t)
        {
            NEVector4 pos = NEVector4.Lerp(v0.Position, v1.Position, t);
            NEVector2 uvs = NEVector2.Lerp(v0.m_UVs, v1.m_UVs,t);
            float zViewSpace = v0.ZInViewSpace + (v1.ZInViewSpace - v0.ZInViewSpace) * t;
            NEVector4 vertToCam = NEVector4.Lerp(v0.Vert2Camera, v1.Vert2Camera, t);


            Vertex ret = new Vertex(0, 0, 0);
            ret.m_Position = pos;
            ret.m_UVs = uvs;
            ret.ZInViewSpace = zViewSpace;
            ret.Vert2Camera = vertToCam;
            return ret;
        }

        public Vertex Duplicate()
        {
            return new Vertex(X, Y, Z, U, V);
        }
        override public string ToString()
        {
            return m_Position.X.ToString() + ", " + m_Position.Y.ToString() + ", " + m_Position.Z.ToString()/* + ",   " + oldX.ToString()*/;

        }

        public string ToString2()
        {
            return m_Position.X.ToString() + ", " + m_Position.W.ToString() + ", " + ZInViewSpace.ToString();

        }

        static public void OrderByY(ref Vertex A, ref Vertex B)
        {
            if (A.Y > B.Y)
            {
                Vertex temp = A;
                A = B;
                B = temp;
            }
        }

        static public void OrderByX(ref Vertex A, ref Vertex B)
        {
            if (A.X > B.X)
            {
                Vertex temp = A;
                A = B;
                B = temp;
            }
        }

    }
}
