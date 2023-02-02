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

        NEFloatBuffer m_LumaBuffer;

        public override bool OnLoad()
        {
            ScreenWidth = 320;
            ScreenHeight = 200;
            PixelWidth = 4;
            PixelHeight = 4;

            //ScreenWidth = 120;
            //ScreenHeight = 110;
            //PixelWidth = 8;
            //PixelHeight = 6;

            //ScreenWidth = 220;
            //ScreenHeight = 180;
            //PixelWidth = 5;
            //PixelHeight = 5;

            m_LumaBuffer = NEFloatBuffer.FromFile("C:/test/nowa_textura12/luma.buf");

            m_DepthBuffer = new NEDepthBuffer(ScreenWidth, ScreenHeight);
            m_VBO = new VertexBuffer();
            m_ProjectionMat = NEMatrix4x4.CreatePerspectiveProjection((float)ScreenHeight / (float)ScreenWidth, 1.05f, 0.1f, 100.0f);

            GenerateSquare(0.0f, 0.0f, 0.0f, 1);


           // GenerateCube(0.0f, 0.0f, 0f,2);

           // m_VBO = NEObjLoader.LoadObj("C:/Users/Kuba/Desktop/tst/teapot.obj");

            //GenerateTestTriangles();
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
            //NEMatrix4x4 rotation = NEMatrix4x4.CreateRotationY(Engine.Instance.TotalTime)
            //    * NEMatrix4x4.CreateRotationZ(Engine.Instance.TotalTime);
            float yDisp = (float)Math.Sin(Engine.Instance.TotalTime);
            NEMatrix4x4 rotation = NEMatrix4x4.CreateRotationX(0.5f + yDisp) *
            NEMatrix4x4.CreateRotationY(Engine.Instance.TotalTime);


            // NEMatrix4x4 rotation = NEMatrix4x4.CreateRotationY(yDisp);


            for (int i = 0; i < m_VBO.Triangles.Count; ++i)
            {
                m_VBO.Triangles[i].TransformedNormal = /*rotation **/ m_VBO.Triangles[i].ModelNormal;
            }

            for (int i=0; i < m_VBO.Vertices.Count;++i)
            {
                m_VBO.Vertices[i].Position = /*(rotation) **/ m_VBO.ModelVertices[i].Position;
                m_VBO.Vertices[i].UV= m_VBO.ModelVertices[i].UV;
                //m_VBO.Vertices[i].Position += new NEVector4(0,0.0f,10.6f,0.0f);
                m_VBO.Vertices[i].Position += new NEVector4(0, 0.0f, 1.2f, 0.0f);
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


            for (int i = 0; i < m_VBO.Triangles.Count; ++i)
            {
                Triangle tr = m_VBO.Triangles[i];
                if (!tr.IsColScanlineInTriangle(u)) continue;
                //if (tr.TransformedNormal.Z > 0) continue;

                ScanlineIntersectionManifest manifest;
                tr.CreateIntersectionManifest(u, out manifest);
                float y0 = manifest.Y0;
                float y1 = manifest.Y1;


                float y0ss = (-y0 + 1.0f) * 0.5f;
                float y1ss = (-y1 + 1.0f) * 0.5f;
                if (y0ss > y1ss) NEMathHelper.Swap(ref y0ss, ref y1ss);
                y0ss = NEMathHelper.Clamp(y0ss, 0, 1.0f);
                y1ss = NEMathHelper.Clamp(y1ss, 0, 1.0f);
                

                int fillStart = (int)(y0ss * ScreenHeight);
                int fillEnd = (int)(y1ss * ScreenHeight);

                float span = fillEnd - fillStart;
                float itCnt = 0;
                for (int y = fillStart; y < fillEnd ; ++y)
                {
                    float v = ((float)y) * oneOverScr;
                    v = -((2.0f * v) - 1.0f);
                    NEVector2 frag = new NEVector2(u, v);

                    float t = itCnt / span;

                    float depthBottom = (1.0f - manifest.bottom_t) * manifest.bottom_P0.Z + manifest.bottom_t * manifest.bottom_P1.Z;
                    float depthTop= (1.0f - manifest.top_t) * manifest.top_P0.Z + manifest.top_t * manifest.top_P1.Z;
                    float fragmentDepth = (1.0f - t) * depthTop + t * depthBottom;




                    NEVector2 textCoordUpper = manifest.bottom_P0.UV * (1.0f - manifest.bottom_t)
                        + manifest.bottom_P1.UV * manifest.bottom_t;

                    NEVector2 textCoordLower = manifest.top_P0.UV * (1.0f - manifest.top_t)
                         + manifest.top_P1.UV * manifest.top_t;

                    NEVector2 texCoord = textCoordLower * (1.0f - t) + textCoordUpper * t;




                    itCnt++;
                    if (m_DepthBuffer.TryUpdate(x, y, fragmentDepth))
                    {
                        //float dot = NEVector4.Dot(tr.TransformedNormal, new NEVector4(0.0f, 0.0f, -1.0f, 0.0f));

                        //dot = NEMathHelper.Clamp(dot, 0.0f, 1.0f);
                        float luma = m_LumaBuffer.FastSample(texCoord.X, texCoord.Y);
                        var col = NEColorSample.MakeCol10(ConsoleColor.Black, (ConsoleColor)tr.ColorAttrib, luma);
                        NEScreenBuffer.PutChar(col.Character, col.BitMask, x, y);
                    }
                }


            }



            base.OnDrawPerColumn(x);
        }




        public override bool OnDraw()
        {


            return base.OnDraw();
        }

        public override void OnExit()
        {
            base.OnExit();
        }



        private void GenerateTestTriangles()
        {
            float deltaX = 0.5f;
            float deltaY = -0.5f;
            float deltaZ = 0.01f;
            float orginX = 0.0f;
            float orginY = 0.0f;
            float orginZ = 0.0f;
            for (int i = 0; i < 2000; ++i)
            {
                float normI = (float)i / (float)2000;
                float xDisp = deltaX * (float)Math.Cos(normI * 6.28f*3.0f);
                float yDisp = deltaY * (float)Math.Sin(normI * 6.28f * 3.0f);
                float zDisp = deltaZ * i * 0.6f;

                m_VBO.AddVertex(new Vertex(orginX - 0.2f + xDisp, orginY + yDisp, orginZ + zDisp));
                m_VBO.AddVertex(new Vertex(orginX + 0.0f + xDisp, orginY + 0.2f + yDisp, orginZ + zDisp));
                m_VBO.AddVertex(new Vertex(orginX + 0.2f + xDisp, orginY + yDisp, orginZ + zDisp));
                m_VBO.AddTriangle(i * 3, i * 3 + 1, i * 3 + 2);
                m_VBO.Triangles[i].ColorAttrib = 1 + i;

            }
        }

        int squareCount = 0;
        private void GenerateSquare(float x, float y, float depth, int col)
        {
            float size = 0.55f;
            m_VBO.AddVertex(new Vertex(-size+x,-size+y, depth,0.0f,1.0f));
            m_VBO.AddVertex(new Vertex(-size+x, size+ y, depth, 0.0f, 0.0f));
            m_VBO.AddVertex(new Vertex(size + x, size + y, depth,1.0f,0.0f));
            m_VBO.AddVertex(new Vertex(size + x, -size +y, depth,1.0f,1.0f));


            m_VBO.AddTriangle(0, 1, 2);
            m_VBO.AddTriangle(0, 2, 3);

            m_VBO.Triangles[0].ColorAttrib = 7;
            m_VBO.Triangles[1].ColorAttrib = 7;

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
