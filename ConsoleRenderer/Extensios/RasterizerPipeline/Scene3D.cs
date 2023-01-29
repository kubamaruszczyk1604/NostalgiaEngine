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

        public override bool OnLoad()
        {
            ScreenWidth = 320;
            ScreenHeight = 200;
            PixelWidth = 4;
            PixelHeight = 4;

            m_DepthBuffer = new NEDepthBuffer(ScreenWidth, ScreenHeight);
            m_VBO = new VertexBuffer();
            //GenerateTestTriangles();
            GenerateSquare(0.0f, 0.0f, 0.5f);
            GenerateSquare(0.2f, 0.1f, 0.45f);
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
            NEScreenBuffer.ClearColor(0);
            m_VBO.CalculateTriangleEdges();
        }

        public override void OnDrawPerColumn(int x)
        {

            float u = ((float)x) / ((float)ScreenWidth);

            u = 2.0f * u - 1.0f;

            float maxDist = m_VBO.Triangles.Count;
            float oneOverScr = 1.0f / ScreenHeight;
            float oneOverMaxDist = 1.0f / maxDist;

            for (short i = 0; i < m_VBO.Triangles.Count; ++i)
            {
                Triangle tr = m_VBO.Triangles[i];
                if (!tr.IsXInTriangle(u)) continue;
                float yA = (-tr.A.Y + 1.0f) * 0.5f;
                float yB = (-tr.B.Y + 1.0f) * 0.5f;
                float yC = (-tr.C.Y + 1.0f) * 0.5f;

                float y0;
                float y1 ;
                tr.FindYs(u, out y0, out y1);

                float y0ss = (-y0 + 1.0f) * 0.5f;
                float y1ss = (-y1 + 1.0f) * 0.5f;
                if (y0 > y1) NEMathHelper.Swap(ref y0, ref y1);
               
                
                for (int y = (int)(y0ss * ScreenHeight); y < (int)(y1ss * ScreenHeight); ++y)
                {
                    float v = ((float)y) * oneOverScr;
                    v = -((2.0f * v) - 1.0f);
                    NEVector2 frag = new NEVector2(u, v);
                    float dA = (tr.A.Position.XY - frag).LengthSqared;
                    float dB = (tr.B.Position.XY - frag).LengthSqared;
                    float dC = (tr.C.Position.XY - frag).LengthSqared;

                    float sm = 1.0f / (dA + dB + dC);
                    dA *= sm;
                    dB *= sm;
                    dC *= sm;
                    float fragmentDepth = (dA * tr.A.Z + dB * tr.B.Z + dC * tr.C.Z) * oneOverMaxDist;
                   // float fragmentDepth = tr.A.Z / maxDist;
                    if (m_DepthBuffer.TryUpdate(x, y, fragmentDepth))
                    {
                        NEScreenBuffer.PutChar((char)NEBlock.Solid, (Int16)(1 + i), x, y);


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


        private bool CheckTriangle(Triangle triangle, NEVector2 p)
        {

            Vertex l = m_VBO.Vertices[triangle.LeftSortedIndices[0]];
            Vertex m = m_VBO.Vertices[triangle.LeftSortedIndices[1]];
            Vertex r = m_VBO.Vertices[triangle.LeftSortedIndices[2]];
            return NEMathHelper.InTriangle(p, l.Position.XY,
                m.Position.XY, r.Position.XY);
        }

        private void GenerateTestTriangles()
        {
            float deltaX = 0.5f;
            float deltaY = -0.5f;
            float deltaZ = 0.1f;
            float orginX = 0.0f;
            float orginY = 0.0f;
            float orginZ = 1.0f;
            for (int i = 0; i < 1000; ++i)
            {
                float normI = (float)i / (float)1000;
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
        private void GenerateSquare(float x, float y, float depth)
        {
            float size = 0.25f;
            m_VBO.AddVertex(new Vertex(-size+x,-size+y,depth));
            m_VBO.AddVertex(new Vertex(-size+x, size+ y, depth));
            m_VBO.AddVertex(new Vertex(size + x, size + y, depth));
            m_VBO.AddVertex(new Vertex(size + x, -size +y, depth));

            int startAt = squareCount * 4;
            m_VBO.AddTriangle(0+ startAt, 1 + startAt, 2 + startAt);
            m_VBO.AddTriangle(0 + startAt, 2 + startAt, 3 + startAt);

            squareCount++;

        }

    }
}
