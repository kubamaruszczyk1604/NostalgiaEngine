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

        public Mesh VBO { get; private set; }
        public int[] Indices { get; private set; }
        public int[] LeftSortedIndices { get; private set; }

        public NEEdge AB { get; private set; }
        public NEEdge AC { get; private set; }
        public NEEdge BC { get; private set; }

        public Vertex A { get; private set; }
        public Vertex B { get; private set; }
        public Vertex C { get; private set; }

        public int ColorAttrib = 1;
        public NEVector4 ModelNormal { get; private set; }
        public NEVector4 TransformedNormal { get; set; }

        public bool Discard;

        public Triangle(int i0, int i1, int i2, Mesh vbo)
        {
            VBO = vbo;
            Indices = new int[] { i0, i1, i2 };
            LeftSortedIndices = new int[3];
            CalculateNormal();
           
        }


        public void CalculateEdges()
        {
            GetXSortedVertices(out LeftSortedIndices[0], out LeftSortedIndices[1], out LeftSortedIndices[2]);
            A = VBO.Vertices[LeftSortedIndices[0]];
            B = VBO.Vertices[LeftSortedIndices[1]];
            C = VBO.Vertices[LeftSortedIndices[2]];


            AB = new NEEdge();
            NEMathHelper.FindLineEquation(A.Position.XY, B.Position.XY, out AB.a, out AB.c);


            AC = new NEEdge();
            NEMathHelper.FindLineEquation(A.Position.XY, C.Position.XY, out AC.a, out AC.c);


            BC = new NEEdge();
            NEMathHelper.FindLineEquation(B.Position.XY, C.Position.XY, out BC.a, out BC.c);

        }


        public bool IsColScanlineInTriangle(float x)
        {
            return ((x >= A.X) && (x <= C.X));
        }

        public void FindIntersectionHeights(float x, out float y0, out float y1)
        {
            y0 = 0;
            y1 = 0;

            if(x<=B.X)
            {
                y0 = AB.a * x + AB.c;
                y1 = AC.a * x + AC.c;
            }
            else
            {

                y0 = BC.a * x + BC.c;
                y1 = AC.a * x + AC.c;
            }

        }

        public void CreateIntersectionManifest(float x, out ScanlineIntersectionManifest manifest)
        {
            manifest = new ScanlineIntersectionManifest();
            float yAC = AC.a * x + AC.c;

            manifest.Y1 = yAC;

         
            float t_AC = (x - A.X) / (C.X - A.X);

            float t_Other = 0.0f;

            Vertex otherP0 = A;
            Vertex otherP1 = B;

            if (x <= B.X)
            {
                //AB is other 
                manifest.Y0 = AB.a * x + AB.c;
                t_Other = (x - A.X) / (B.X - A.X); 


            }
            else 
            {
                //BC is other
                manifest.Y0 = BC.a * x + BC.c;
                t_Other = (x - B.X) / (C.X - B.X);

                otherP0 = B;
                otherP1 = C;
            }

            if (yAC > manifest.Y0) //ac is upper
            {

                manifest.top_t = t_AC;
                manifest.bottom_t = t_Other;

                manifest.top_P0 = A;
                manifest.top_P1 = C;

                manifest.bottom_P0 = otherP0;
                manifest.bottom_P1 = otherP1;


            }
            else
            {
                manifest.top_t = t_Other;
                manifest.bottom_t = t_AC;

                manifest.top_P0 = otherP0;
                manifest.top_P1 = otherP1;

                manifest.bottom_P0 = A;
                manifest.bottom_P1 = C;
            }

        }

        private void GetXSortedVertices(out int left, out int middle, out int right)
        {
            left = Indices[0];
            middle = Indices[1];
            right = Indices[2];


            if(VBO.Vertices[left].X > VBO.Vertices[middle].X)
            {
                SwapInt(ref left, ref middle);
            }

            if (VBO.Vertices[middle].X > VBO.Vertices[right].X)
            {
                SwapInt(ref middle, ref right);
            }

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

        private void CalculateNormal()
        {
            NEVector4 a = (VBO.ModelVertices[Indices[1]].Position - VBO.ModelVertices[Indices[0]].Position).Normalized;
            NEVector4 b = (VBO.ModelVertices[Indices[2]].Position - VBO.ModelVertices[Indices[0]].Position).Normalized;

            float x = a.Y * b.Z - a.Z * b.Y;
            float y = a.Z * b.X - a.X * b.Z;
            float z = a.X * b.Y - a.Y * b.X;
            ModelNormal = new NEVector4(x, y, z,0.0f).Normalized;
        }



    }

    public class NEEdge
    {
        public float a; //gradient
        public float c; //intercept


    }

    public struct ScanlineIntersectionManifest
    {
        public float Y0;
        public float Y1;

        public float bottom_t;
        public float top_t;

        public Vertex bottom_P0;
        public Vertex bottom_P1;

        public Vertex top_P0;
        public Vertex top_P1;
    }
}
