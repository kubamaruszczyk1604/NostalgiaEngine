using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.RasterizerPipeline
{
    
    public class VertexPool
    {

        List<Vertex> m_Vertices;
        private int m_StackPtr;

        public VertexPool()
        {
            m_Vertices = new List<Vertex>();
            m_StackPtr = 0;
        }

        public void Allocate(int count)
        {

            for (int i = 0; i < count; ++i)
            {
                m_Vertices.Add(new Vertex(0.0f,0.0f,0.0f));
            }
        }

        public Vertex RequestAndSet(Vertex data)
        {
            if (m_StackPtr > m_Vertices.Count - 20)
            {
                Allocate(m_Vertices.Count);
            }
            Vertex ret = m_Vertices[m_StackPtr];
            ret.Set(data);
            m_StackPtr++;
            return ret;
        }

        public Vertex Get(ref NEVector4 pos, ref NEVector2 uv)
        {
            if (m_StackPtr > m_Vertices.Count - 20)
            {
                Allocate(m_Vertices.Count);
            }
            Vertex ret = m_Vertices[m_StackPtr];
            ret.Set(pos, uv);
            m_StackPtr++;
            return ret;
        }


        public void ReturnAllToPool()
        {
            m_StackPtr = 0;
        }


    }
}
