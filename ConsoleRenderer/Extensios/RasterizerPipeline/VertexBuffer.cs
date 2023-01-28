using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.RasterizerPipeline
{
    public class VertexBuffer
    {

        public List<Vertex> Vertices { get; private set; }
        public List<Triangle> Triangles { get; private set; }

        public VertexBuffer()
        {
            Vertices = new List<Vertex>(100);
        }

        public bool Create(Triangle[] triangles)
        {
            if (triangles.Length == 0) return false;
            Vertices.Clear();
            Triangles = new List<Triangle>(triangles.Length);
            for (int i = 0; i < triangles.Length; ++i)
            {
                Vertices.AddRange(triangles[i].Vertices);
                Triangles.Add(triangles[i]);
            }
            return true;
        }

        //public bool Create(int[] indexBuffer)
        //{
        //    if(indexBuffer == null) return false;
        //    if (indexBuffer.Length < 3) return false;
        //    if (Vertices.Count < 3) return false;



        //    return true;
        //}


        public void AddNewVertex(Vertex v)
        {
            Vertices.Add(v);
        }

        public void AddNewVertex(float x, float y, float z)
        {
            Vertices.Add(new Vertex(x, y, z));

        }

        public void AddNewVertex(float x, float y, float z, float u, float v)
        {
            Vertices.Add(new Vertex(x, y, z, u, v));
        }

        public void Translate()
        {

        }
    }
}
