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
            GenerateTestTriangles();
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
            NEScreenBuffer.ClearColor(1);
            m_VBO.CalculateTriangleEdges();
        }

        public override void OnDrawPerColumn(int x)
        {

            float u = ((float)x) / ((float)ScreenWidth);

            u = 2.0f * u - 1.0f;



            for (short i = 0; i < m_VBO.Triangles.Count; ++i)
            {
                Triangle tr = m_VBO.Triangles[i];
                if (!tr.IsXInTriangle(u)) continue;
                float y0;
                float y1;
                tr.FindYs(u, out y0, out y1);

                y0 = (-y0 + 1.0f) * 0.5f;
                y1 = (-y1 + 1.0f) * 0.5f;
                if (y0 > y1) NEMathHelper.Swap(ref y0, ref y1);
                for (int y = (int)(y0 * ScreenHeight); y < (int)(y1 * ScreenHeight); ++y)
                {
                    float v = ((float)y) / ((float)ScreenHeight);
                    if (m_DepthBuffer.TryUpdate(x,y,1.0f))
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
            for (int i = 0; i < 2000; ++i)
            {
                float normI = (float)i / (float)2000;
                float xDisp = deltaX * (float)Math.Cos(normI * 6.28f);
                float yDisp = deltaY * (float)Math.Sin(normI * 6.28f);
                float zDisp = deltaZ * i;

                m_VBO.AddVertex(new Vertex(orginX + 0.5f + xDisp, orginY + yDisp, orginZ + zDisp));
                m_VBO.AddVertex(new Vertex(orginX + 0.0f + xDisp, orginY + 0.5f + yDisp, orginZ + zDisp));
                m_VBO.AddVertex(new Vertex(orginX - 0.5f + xDisp, orginY + yDisp, orginZ + zDisp));
                m_VBO.AddTriangle(i * 3, i * 3 + 1, i * 3 + 2);

            }
        }

    }
}
