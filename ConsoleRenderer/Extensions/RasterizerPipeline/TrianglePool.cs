using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.RasterizerPipeline
{
    public class TrianglePool
    {
        List<Triangle> m_Triangles;
        private int m_StackPtr;

        public TrianglePool()
        {
            m_Triangles = new List<Triangle>();
            m_StackPtr = 0;
        }

        public void Allocate(int count)
        {

            for (int i = 0; i < count; ++i)
            {
                m_Triangles.Add(new Triangle());
            }
        }

        public Triangle Get(Triangle triangle, VertexBuffer vbo)
        {
            if (m_StackPtr > m_Triangles.Count - 20)
            {
                Allocate(m_Triangles.Count);
            }
            Triangle ret = m_Triangles[m_StackPtr];
            ret.Set(triangle, vbo);
            m_StackPtr++;
            return ret;
        }

        public Triangle Get(int i0, int i1, int i2, VertexBuffer vbo, ref NEVector4 normal, ref NEVector4 normalView, ref NEVector4 normalWorld)
        {
            if (m_StackPtr > m_Triangles.Count - 20)
            {
                Allocate(m_Triangles.Count);
            }
            Triangle ret = m_Triangles[m_StackPtr];
            ret.Set(i0, i1, i2, vbo, normal, normalView, normalWorld);
            m_StackPtr++;
            return ret;
        }


        public void ReturnAllToPool()
        {
            m_StackPtr = 0;
        }

    }
}
