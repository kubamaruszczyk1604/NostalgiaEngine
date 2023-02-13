using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.RasterizerPipeline
{
    public class Mesh
    {

        public List<Vertex> ModelVertices { get; private set; }
        public List<Vertex> Vertices { get; private set; }
        public List<Triangle> Triangles { get; private set; }

        public List<Triangle> TempTriangleContainer { get; set; }

        public Mesh()
        {
            ModelVertices = new List<Vertex>(100);
            Vertices = new List<Vertex>(100);
            Triangles = new List<Triangle>(100);
            TempTriangleContainer = new List<Triangle>(100);
        }


        public void AddVertex(Vertex v)
        {
            ModelVertices.Add(v);
            Vertices.Add(v.Duplicate());
        }

        public void AddVertex(float x, float y, float z)
        {
            ModelVertices.Add(new Vertex(x, y, z));
            Vertices.Add(new Vertex(x, y, z));
        }

        public void AddVertex(float x, float y, float z, float u, float v)
        {
            ModelVertices.Add(new Vertex(x, y, z, u, v));
            Vertices.Add(new Vertex(x, y, z, u, v));
        }

        public void AddTriangle(int i0, int i1, int i2)
        {
            Triangles.Add(new Triangle(i0, i1, i2, this));
        }


        public void CalculateTriangleEdges()
        {
            for (int i =0; i < Triangles.Count; ++i)
            {
                Triangles[i].CalculateEdges();
            }
        }

    }
}
