using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;
namespace NostalgiaEngine.RasterizerPipeline
{
    public class Triangle
    {
        public Vertex[] Vertices { get; private set; }
        public Vertex[] LeftSortedVertices { get; private set; }
        
        public NEVector4 Normal { get; private set; }
        
        public Triangle(Vertex v1, Vertex v2, Vertex v3, NEVector4 normal)
        {
            Vertices = new Vertex[] { v1, v2, v3 };
            LeftSortedVertices = new Vertex[3];
            Normal = normal;
            GetXSortedVertices(out LeftSortedVertices[0], out LeftSortedVertices[1], out LeftSortedVertices[2]);
        }

        public Triangle(Vertex v1, Vertex v2, Vertex v3): this(v1,v2,v3, new NEVector4())
        {
            CalculateNormal();
        }


        public void SetVertexPos(uint index)
        {
            if (index > 2) return;
        }


        public void LeftSortVertices()
        {
            GetXSortedVertices(out LeftSortedVertices[0], out LeftSortedVertices[1], out LeftSortedVertices[2]);
        }

        private void GetXSortedVertices(out Vertex left, out Vertex middle, out Vertex right)
        {
            left = Vertices[0];
            middle = Vertices[1];
            right = Vertices[2];

            if (left.X > middle.X) Vertex.Swap(ref left, ref middle);
            if (middle.X > right.X) Vertex.Swap(ref middle, ref right);
            if (left.X > middle.X) Vertex.Swap(ref left, ref middle);
        }

       


        private void CalculateNormal()
        {
            NEVector4 a = (Vertices[1].Position - Vertices[0].Position).Normalized;
            NEVector4 b = (Vertices[2].Position - Vertices[0].Position).Normalized;

            float x = a.Y * b.Z - a.Z * b.Y;
            float y = a.Z * b.X - a.X * b.Z;
            float z = a.X * b.Y - a.Y * b.X;
            Normal = new NEVector4(x, y,z);
        }
    }
}
