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

        public Mesh ParentMesh { get; private set; }
        public VertexBuffer VBO { get; private set; }
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

       

        public Triangle(int i0, int i1, int i2, Mesh mesh)
        {
            ParentMesh = mesh;
            Indices = new int[] { i0, i1, i2 };
            LeftSortedIndices = new int[3];
            CalculateNormal();
           
        }

        public Triangle()
        {
            //ParentMesh = mesh;
            Indices = new int[3];
            LeftSortedIndices = new int[3];

        }

        public Triangle(int i0, int i1, int i2, VertexBuffer vbo, NEVector4 normal, NEVector4 transformedNormal)
        {
            VBO = vbo;
            Indices = new int[] { i0, i1, i2 };
            LeftSortedIndices = new int[3];
            ModelNormal = normal;
            TransformedNormal = transformedNormal;
        }

        public Triangle(Triangle triangle, VertexBuffer vbo)
        {
            VBO = vbo;
            Indices = new int[] { triangle.Indices[0], triangle.Indices[1], triangle.Indices[2]};
            LeftSortedIndices = new int[3];
            ModelNormal = triangle.ModelNormal;
            TransformedNormal = triangle.TransformedNormal;
            ColorAttrib = triangle.ColorAttrib;
        }

        public void Set(Triangle triangle, VertexBuffer vbo)
        {
            VBO = vbo;
            ParentMesh = triangle.ParentMesh;
            Indices = new int[] { triangle.Indices[0], triangle.Indices[1], triangle.Indices[2] };
            LeftSortedIndices = new int[3];
            ModelNormal = triangle.ModelNormal;
            TransformedNormal = triangle.TransformedNormal;
            ColorAttrib = triangle.ColorAttrib;

        }

        public void Set(Triangle triangle)
        {
            VBO = triangle.VBO;
            ParentMesh = triangle.ParentMesh;
            Indices = new int[] { triangle.Indices[0], triangle.Indices[1], triangle.Indices[2] };
            LeftSortedIndices = new int[3];
            ModelNormal = triangle.ModelNormal;
            TransformedNormal = triangle.TransformedNormal;
            ColorAttrib = triangle.ColorAttrib;

        }

        public void ZDivide()
        {
            VBO.ProcessedVertices[Indices[0]].ZDivide();
            VBO.ProcessedVertices[Indices[1]].ZDivide();
            VBO.ProcessedVertices[Indices[2]].ZDivide();
        }



        //public void DoLeftSort()
        //{
        //    SortX(out LeftSortedIndices[0], out LeftSortedIndices[1], out LeftSortedIndices[2]);
        //    A = VBO.ProcessedVertices[LeftSortedIndices[0]];
        //    B = VBO.ProcessedVertices[LeftSortedIndices[1]];
        //    C = VBO.ProcessedVertices[LeftSortedIndices[2]];
        //}

        public void CalculateEdges()
        {
            SortX(out LeftSortedIndices[0], out LeftSortedIndices[1], out LeftSortedIndices[2]);
            A = VBO.ProcessedVertices[LeftSortedIndices[0]];
            B = VBO.ProcessedVertices[LeftSortedIndices[1]];
            C = VBO.ProcessedVertices[LeftSortedIndices[2]];

            AB = new NEEdge();
            NEMathHelper.Find2DLineEquation(A.Position.XY, B.Position.XY, out AB.a, out AB.c);

            AC = new NEEdge();
            NEMathHelper.Find2DLineEquation(A.Position.XY, C.Position.XY, out AC.a, out AC.c);

            BC = new NEEdge();
            NEMathHelper.Find2DLineEquation(B.Position.XY, C.Position.XY, out BC.a, out BC.c);


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



        public void ComputeScanlineIntersection(float x, out ScanlineIntersectionManifest manifest)
        {
            manifest = new ScanlineIntersectionManifest();
            float yAC = AC.a * x + AC.c;

            manifest.Y1 = yAC;

            float denCA = (C.X - A.X);
            denCA = NEMathHelper.Abs(denCA) >= 0.01f ? denCA : 0.01f;
            float t_AC = (x - A.X) / denCA;


            float t_Other = 0.0f;

            Vertex otherP0 = A;
            Vertex otherP1 = B;

            if (x <= B.X)
            {
                //AB is other 
                manifest.Y0 = AB.a * x + AB.c;
                float denBA = (B.X - A.X);
                denBA = NEMathHelper.Abs(denBA) >= 0.01f ? denBA : 0.01f;
                t_Other = (x - A.X) / denBA;

            }
            else
            {
                //BC is other
                manifest.Y0 = BC.a * x + BC.c;
                float denCB = (C.X - B.X);
                denCB = NEMathHelper.Abs(denCB) >= 0.01f ? denCB : 0.01f;
                t_Other = (x - B.X) / denCB;

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

        private void SortX(out int left, out int middle, out int right)
        {
            left = Indices[0];
            middle = Indices[1];
            right = Indices[2];


            if(VBO.ProcessedVertices[left].X > VBO.ProcessedVertices[middle].X)
            {
                SwapInt(ref left, ref middle);
            }

            if (VBO.ProcessedVertices[middle].X > VBO.ProcessedVertices[right].X)
            {
                SwapInt(ref middle, ref right);
            }

            if (VBO.ProcessedVertices[left].X > VBO.ProcessedVertices[middle].X)
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
            NEVector4 a = (ParentMesh.Vertices[Indices[1]].Position - ParentMesh.Vertices[Indices[0]].Position).Normalized;
            NEVector4 b = (ParentMesh.Vertices[Indices[2]].Position - ParentMesh.Vertices[Indices[0]].Position).Normalized;

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
