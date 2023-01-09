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
        Triangle[] m_Triangles = new Triangle[10];

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
            m_Triangles[0] = new Triangle(
                new Vertex(0.5f, 0.0f, 1.0f),
                new Vertex(0.0f, 0.9f, 1.0f),
                new Vertex(-0.5f, 0.0f, 1.0f));

            //m_Triangles[1] = new Triangle(
            //   new Vertex(-0.5f, 0.0f, 1.0f),
            //   new Vertex(0.0f, 0.8f, 1.0f),
            //   new Vertex(0.5f, 0.0f, 1.0f));

            //m_Triangles[2] = new Triangle(
            //   new Vertex(-0.5f, 0.0f, 1.0f),
            //   new Vertex(0.0f, 0.7f, 1.0f),
            //   new Vertex(0.5f, 0.0f, 1.0f));

            //m_Triangles[3] = new Triangle(
            //   new Vertex(-0.5f, 0.0f, 1.0f),
            //   new Vertex(0.0f, 0.6f, 1.0f),
            //   new Vertex(0.5f, 0.0f, 1.0f));

            //m_Triangles[4] = new Triangle(
            //   new Vertex(-0.5f, 0.0f, 1.0f),
            //   new Vertex(0.0f, 0.5f, 1.0f),
            //   new Vertex(0.5f, 0.0f, 1.0f));
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
                for (short i = 0; i < 1; ++i)
                {
                    if (CheckTriangle(m_Triangles[i], new NEVector2(u, v)))
                    {
                        NEScreenBuffer.PutChar((char)NEBlock.Solid, i, x, y);
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

            triangle.GetXSortedVertices(out Vertex l, out Vertex m, out Vertex r);
            return NEMathHelper.InTriangle(p, l.Position.XY, 
                m.Position.XY, r.Position.XY);
        }

    }
}
