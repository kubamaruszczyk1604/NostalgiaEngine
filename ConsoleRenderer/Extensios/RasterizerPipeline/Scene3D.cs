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
        Triangle[] m_Triangles = new Triangle[16];

        public override bool OnLoad()
        {
            ScreenWidth = 320;
            ScreenHeight = 200;
            PixelWidth = 4;
            PixelHeight = 4;
            return base.OnLoad();
        }



        public override void OnStart()
        {
            //m_Triangles[0] = new Triangle(
            //    new Vertex(0.5f, 0.0f, 1.0f),
            //    new Vertex(0.0f, 0.5f, 1.0f),
            //    new Vertex(-0.5f, 0.0f, 1.0f));

            //m_Triangles[1] = new Triangle(
            //   new Vertex(-0.5f, 0.0f, 1.0f),
            //   new Vertex(0.0f, 0.5f, 1.0f),
            //   new Vertex(0.5f, 0.0f, 1.0f));

            //m_Triangles[2] = new Triangle(
            //   new Vertex(-0.5f, 0.0f, 1.0f),
            //   new Vertex(0.0f, 0.5f, 1.0f),
            //   new Vertex(0.5f, 0.0f, 1.0f));

            //m_Triangles[3] = new Triangle(
            //   new Vertex(-0.5f, 0.0f, 1.0f),
            //   new Vertex(0.0f, 0.5f, 1.0f),
            //   new Vertex(0.5f, 0.0f, 1.0f));

            //m_Triangles[4] = new Triangle(
            //   new Vertex(-0.5f, 0.0f, 1.0f),
            //   new Vertex(0.0f, 0.5f, 1.0f),
            //   new Vertex(0.5f, 0.0f, 1.0f));

            GenerateTestTriangles();
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
        }

        public override void OnDrawPerColumn(int x)
        {

            float u = ((float)x) / ((float)ScreenWidth);

            u = 2.0f * u - 1.0f;

            for (int y = 0; y < ScreenHeight; ++y)
            {

                float v = ((float)y) / ((float)ScreenHeight);
                v = -((2.0f * v) - 1.0f);
                for (short i = 0; i < m_Triangles.Length; ++i)
                {
                    if (CheckTriangle(m_Triangles[i], new NEVector2(u, v)))
                    {
                        NEScreenBuffer.PutChar((char)NEBlock.Solid, (Int16)(1 + i), x, y);
                    }
                }
            }

            base.OnDrawPerColumn(x);
        }

        public override bool OnDraw()
        {


            //for (int x = 0; x < ScreenWidth; ++x)
            //{
            //    float u = ((float)x) / ((float)ScreenWidth);

            //    u = 2.0f * u - 1.0f;

            //    for (int y = 0; y < ScreenHeight; ++y)
            //    {

            //        float v = ((float)y) / ((float)ScreenHeight);
            //        v = -((2.0f * v) - 1.0f);
            //        if (CheckTriangle(m_Triangles[0], new NEVector2(u, v)))
            //        {
            //            NEScreenBuffer.PutChar((char)NEBlock.Solid, 4, x, y);
            //        }

            //    }
            //}

            return base.OnDraw();
        }

        public override void OnExit()
        {
            base.OnExit();
        }


        private bool CheckTriangle(Triangle triangle, NEVector2 p)
        {

            Vertex l = triangle.LeftSortedVertices[0];
            Vertex m = triangle.LeftSortedVertices[1];
            Vertex r = triangle.LeftSortedVertices[2];
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
            for (int i = 0; i < m_Triangles.Length; ++i)
            {
                float normI = (float)i / (float)m_Triangles.Length;
                float xDisp = deltaX * (float)Math.Cos(normI * 6.28f);
                float yDisp = deltaY * (float)Math.Sin(normI * 6.28f);
                float zDisp = deltaZ * i;
                m_Triangles[i] = new Triangle(new Vertex(orginX + 0.5f + xDisp, orginY + yDisp, orginZ + zDisp),
                             new Vertex(orginX + 0.0f + xDisp, orginY + 0.5f + yDisp, orginZ + zDisp),
                             new Vertex(orginX - 0.5f + xDisp, orginY + yDisp, orginZ + zDisp));


            }
        }

    }
}
