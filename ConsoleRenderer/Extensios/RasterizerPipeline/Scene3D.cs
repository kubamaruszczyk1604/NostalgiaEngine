using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;
namespace NostalgiaEngine.RasterizerPipeline
{
    public class Scene3D : NEScene
    {
        VertexBuffer m_VBO;
        NEDepthBuffer m_DepthBuffer;

        NEMatrix4x4 m_ProjectionMat;

        public override bool OnLoad()
        {
            ScreenWidth = 320;
            ScreenHeight = 200;
            PixelWidth = 4;
            PixelHeight = 4;

            m_DepthBuffer = new NEDepthBuffer(ScreenWidth, ScreenHeight);
            m_VBO = new VertexBuffer();
            m_ProjectionMat = NEMatrix4x4.CreatePerspectiveProjection((float)ScreenHeight / (float)ScreenWidth, 1.05f, 0.1f, 100.0f);
            //GenerateTestTriangles();
            //GenerateSquare(0.0f, 0.0f, 0.5f, 1);
            //GenerateSquare(0.2f, 0.1f, 0.45f, 3);
            //GenerateSquare2(0.3f, 0.3f, 0.04f, 9);

            GenerateCube(0.0f, 0.0f, 0f,2);
            return base.OnLoad();
        }



        public override void OnStart()
        {
            base.OnStart();
        }

        public override void OnPause()
        {
            base.OnPause();
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            NEMatrix4x4 rotation = NEMatrix4x4.CreateRotationY(Engine.Instance.TotalTime)
                * NEMatrix4x4.CreateRotationZ(Engine.Instance.TotalTime);

            //NEMatrix4x4 rotation = NEMatrix4x4.CreateRotationX(Engine.Instance.TotalTime);
            for (int i=0; i < m_VBO.Vertices.Count;++i)
            {
                m_VBO.Vertices[i].Position = ( rotation)  * m_VBO.ModelVertices[i].Position;
                m_VBO.Vertices[i].Position += new NEVector4(0,0,2.0f,0.0f);
                m_VBO.Vertices[i].Position = (m_ProjectionMat) * m_VBO.Vertices[i].Position;
                m_VBO.Vertices[i].WDivide();
            }
            NEScreenBuffer.ClearColor(0);
            m_DepthBuffer.Clear();
            m_VBO.CalculateTriangleEdges();
        }

        public override void OnDrawPerColumn(int x)
        {

            float u = ((float)x) / ((float)ScreenWidth);

            u = 2.0f * u - 1.0f;
            float oneOverScr = 1.0f / ScreenHeight;


            for (short i = 0; i < m_VBO.Triangles.Count; ++i)
            {
                Triangle tr = m_VBO.Triangles[i];
                if (!tr.IsColScanlineInTriangle(u)) continue;
                if (tr.CalculateNormal().Z > 0) continue;
                //float yA = (-tr.A.Y + 1.0f) * 0.5f;
                //float yB = (-tr.B.Y + 1.0f) * 0.5f;
                //float yC = (-tr.C.Y + 1.0f) * 0.5f;

                float y0;
                float y1 ;
                tr.FindIntersectionHeights(u, out y0, out y1);

                float y0ss = (-y0 + 1.0f) * 0.5f;
                float y1ss = (-y1 + 1.0f) * 0.5f;
                if (y0ss > y1ss) NEMathHelper.Swap(ref y0ss, ref y1ss);
                y0ss = NEMathHelper.Clamp(y0ss, 0, 1.0f);
                y1ss = NEMathHelper.Clamp(y1ss, 0, 1.0f);
                for (int y = (int)(y0ss * ScreenHeight); y < (int)(y1ss * ScreenHeight); ++y)
                {
                    //if ((y<0)||(y >= ScreenHeight)) continue;
                    float v = ((float)y) * oneOverScr;
                    v = -((2.0f * v) - 1.0f);
                    NEVector2 frag = new NEVector2(u, v);
                    float dA = (tr.A.Position.XY - frag).Length;
                    float dB = (tr.B.Position.XY - frag).Length;
                    float dC = (tr.C.Position.XY - frag).Length;

                    float sm = 1.0f / (dA + dB + dC);
                    dA = 1.0f - dA * sm;
                    dB = 1.0f - dB * sm;
                    dC = 1.0f - dC * sm;

                    float fragmentDepth = (dA * tr.A.Z + dB * tr.B.Z + dC * tr.C.Z);
                    if (m_DepthBuffer.TryUpdate(x, y, fragmentDepth))
                    {
                        NEScreenBuffer.PutChar((char)NEBlock.Solid, (Int16)(tr.ColorAttrib), x, y);
                    }
                }


            }



            base.OnDrawPerColumn(x);
        }



        //public override void OnDrawPerColumn(int x)
        //{

        //    float u = ((float)x) / ((float)ScreenWidth);

        //    u = 2.0f * u - 1.0f;

        //    for (int y = 0; y < ScreenHeight; ++y)
        //    {

        //        float v = ((float)y) / ((float)ScreenHeight);
        //        v = -((2.0f * v) - 1.0f);

        //        for (short i = 0; i < m_VBO.Triangles.Count; ++i)
        //        {
        //            if (CheckTriangle(m_VBO.Triangles[i], new NEVector2(u, v)))
        //            {
        //                NEScreenBuffer.PutChar((char)NEBlock.Solid, (Int16)(1 + i), x, y);
        //            }
        //        }
        //    }

        //    base.OnDrawPerColumn(x);
        //}
        public override bool OnDraw()
        {


            return base.OnDraw();
        }

        public override void OnExit()
        {
            base.OnExit();
        }


        //private bool CheckTriangle(Triangle triangle, NEVector2 p)
        //{

        //    Vertex l = m_VBO.Vertices[triangle.LeftSortedIndices[0]];
        //    Vertex m = m_VBO.Vertices[triangle.LeftSortedIndices[1]];
        //    Vertex r = m_VBO.Vertices[triangle.LeftSortedIndices[2]];
        //    return NEMathHelper.InTriangle(p, l.Position.XY,
        //        m.Position.XY, r.Position.XY);
        //}

        private void GenerateTestTriangles()
        {
            float deltaX = 0.5f;
            float deltaY = -0.5f;
            float deltaZ = 0.1f;
            float orginX = 0.0f;
            float orginY = 0.0f;
            float orginZ = 1.0f;
            for (int i = 0; i < 2; ++i)
            {
                float normI = (float)i / (float)2;
                float xDisp = deltaX * (float)Math.Cos(normI * 6.28f);
                float yDisp = deltaY * (float)Math.Sin(normI * 6.28f);
                float zDisp = deltaZ*i*0.3f ;

                m_VBO.AddVertex(new Vertex(orginX + 0.5f + xDisp, orginY + yDisp, orginZ + zDisp));
                m_VBO.AddVertex(new Vertex(orginX + 0.0f + xDisp, orginY + 0.5f + yDisp, orginZ + zDisp));
                m_VBO.AddVertex(new Vertex(orginX - 0.5f + xDisp, orginY + yDisp, orginZ + zDisp));
                m_VBO.AddTriangle(i * 3, i * 3 + 1, i * 3 + 2);

            }
        }

        int squareCount = 0;
        private void GenerateSquare(float x, float y, float depth, int col)
        {
            float size = 0.25f;
            m_VBO.AddVertex(new Vertex(-size+x,-size+y,depth));
            m_VBO.AddVertex(new Vertex(-size+x, size+ y, depth));
            m_VBO.AddVertex(new Vertex(size + x, size + y, depth));
            m_VBO.AddVertex(new Vertex(size + x, -size +y, depth));

            int startAt = squareCount * 4;
            m_VBO.AddTriangle(0+ startAt, 1 + startAt, 2 + startAt);
            m_VBO.AddTriangle(0 + startAt, 2 + startAt, 3 + startAt);
            int colStart = squareCount * 2;
            m_VBO.Triangles[colStart].ColorAttrib = col;
            m_VBO.Triangles[colStart + 1].ColorAttrib = col;
            squareCount++;

        }

        private void GenerateSquare2(float x, float y, float depth, int col)
        {
            float size = 0.25f;
            m_VBO.AddVertex(new Vertex(-size + x, -size + y, depth));
            m_VBO.AddVertex(new Vertex(-size + x, size + y, depth));
            m_VBO.AddVertex(new Vertex(size + x, size + y, depth));
            m_VBO.AddVertex(new Vertex(size + x, -size + y, depth));

            int startAt = squareCount * 4;
            m_VBO.AddTriangle(0 + startAt, 1 + startAt, 2 + startAt);
            m_VBO.AddTriangle(0 + startAt, 2 + startAt, 3 + startAt);
            int colStart = squareCount * 2;
            m_VBO.Triangles[colStart].ColorAttrib = col;
            m_VBO.Triangles[colStart + 1].ColorAttrib = col;
            squareCount++;

        }


        private void GenerateCube(float x, float y, float z, int col)
        {
            float size = 0.25f;
            m_VBO.AddVertex(new Vertex(-size + x, -size + y, z - size));
            m_VBO.AddVertex(new Vertex(-size + x, size + y, z - size));
            m_VBO.AddVertex(new Vertex(size + x, size + y, z - size));
            m_VBO.AddVertex(new Vertex(size + x, -size + y, z - size));

            m_VBO.AddVertex(new Vertex(-size + x, -size + y, z + size));
            m_VBO.AddVertex(new Vertex(-size + x, size + y, z + size));
            m_VBO.AddVertex(new Vertex(size + x, size + y, z + size));
            m_VBO.AddVertex(new Vertex(size + x, -size + y, z + size));


            m_VBO.AddTriangle(0, 1 , 2 );
            m_VBO.AddTriangle(0 , 2 , 3 );

            m_VBO.Triangles[0].ColorAttrib = 1;
            m_VBO.Triangles[1].ColorAttrib = 1;


            m_VBO.AddTriangle(4, 6, 5);
            m_VBO.AddTriangle(4, 7, 6);

            m_VBO.Triangles[2].ColorAttrib = 2;
            m_VBO.Triangles[3].ColorAttrib = 2;


            m_VBO.AddTriangle(4, 5, 1);
            m_VBO.AddTriangle(4, 1, 0);

            m_VBO.Triangles[4].ColorAttrib = 3;
            m_VBO.Triangles[5].ColorAttrib = 3;


            m_VBO.AddTriangle(3, 2, 6);
            m_VBO.AddTriangle(3, 6, 7);

            m_VBO.Triangles[6].ColorAttrib = 5;
            m_VBO.Triangles[7].ColorAttrib = 5;



            m_VBO.AddTriangle(1, 5, 6);
            m_VBO.AddTriangle(1, 6, 2);

            m_VBO.Triangles[8].ColorAttrib = 6;
            m_VBO.Triangles[9].ColorAttrib = 6;


            m_VBO.AddTriangle(0, 3, 7);
            m_VBO.AddTriangle(0, 7, 4);

            m_VBO.Triangles[10].ColorAttrib = 7;
            m_VBO.Triangles[11].ColorAttrib = 7;

        }

    }
}
