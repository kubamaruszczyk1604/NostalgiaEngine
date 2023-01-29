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



        public VertexBuffer VBO { get; private set; }
        public int[] Indices { get; private set; }
        public int[] LeftSortedIndices { get; private set; }
 
        public Triangle(int i0, int i1, int i2, VertexBuffer vbo)
        {
            VBO = vbo;
            Indices = new int[] { i0, i1, i2 };
            LeftSortedIndices = new int[3];
            LeftSort();
          
        }




        public void LeftSort()
        {
            GetXSortedVertices(out LeftSortedIndices[0], out LeftSortedIndices[1], out LeftSortedIndices[2]);
        }

        private void GetXSortedVertices(out int left, out int middle, out int right)
        {
            left = Indices[0];
            middle = Indices[1];
            right = Indices[2];



            //if (left.X > middle.X) Vertex.Swap(ref left, ref middle);

            if(VBO.Vertices[left].X > VBO.Vertices[middle].X)
            {
                SwapInt(ref left, ref middle);
            }

            //if (middle.X > right.X) Vertex.Swap(ref middle, ref right);
            if (VBO.Vertices[middle].X > VBO.Vertices[right].X)
            {
                SwapInt(ref middle, ref right);
            }


            //if (left.X > middle.X) Vertex.Swap(ref left, ref middle);
            if (VBO.Vertices[left].X > VBO.Vertices[middle].X)
            {
                SwapInt(ref left, ref middle);
            }

        }

       
        private void SwapInt(ref int a, ref int b)
        {
           int tmp = a;
            a = b;
            b = tmp;
        }

        //private void CalculateNormal()
        //{
        //    NEVector4 a = (Vertices[1].Position - Vertices[0].Position).Normalized;
        //    NEVector4 b = (Vertices[2].Position - Vertices[0].Position).Normalized;

        //    float x = a.Y * b.Z - a.Z * b.Y;
        //    float y = a.Z * b.X - a.X * b.Z;
        //    float z = a.X * b.Y - a.Y * b.X;
        //    Normal = new NEVector4(x, y,z);
        //}
    }
}
