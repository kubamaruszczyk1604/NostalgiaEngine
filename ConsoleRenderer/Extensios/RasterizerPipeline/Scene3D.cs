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
        NETexture m_Texture;
        NEColorPalette m_Palette;

        public override bool OnLoad()
        {
            ScreenWidth = 320;
            ScreenHeight = 200;
            PixelWidth = 4;
            PixelHeight = 4;

            //ParallelScreenDraw = true;
            //ScreenWidth = 120;
            //ScreenHeight = 110;
            //PixelWidth = 8;
            //PixelHeight = 6;

            //ScreenWidth = 220;
            //ScreenHeight = 180;
            //PixelWidth = 5;
            //PixelHeight = 5;

            m_LumaBuffer = NEFloatBuffer.FromFile("C:/test/ntex/luma.buf");
            m_Texture = NEColorTexture16.LoadFromFile("C:/test/nowa_textura12/color.tex");
            m_Palette = NEColorPalette.FromFile("C:/test/nowa_textura12/palette.txt");
            m_LumaBuffer.SampleMode = NESampleMode.Repeat;
            m_DepthBuffer = new NEDepthBuffer(ScreenWidth, ScreenHeight);
            m_VBO = new VertexBuffer();
            m_ProjectionMat = NEMatrix4x4.CreatePerspectiveProjection((float)ScreenHeight / (float)ScreenWidth, 1.05f, 1.1f, 100.0f);

            //GenerateSquare(0.0f, 0.0f, 0.0f, 1);

            NEColorManagement.SetPalette(m_Palette);
            GenerateCube(0.0f, 0.0f, 0f,2);

            //m_VBO = NEObjLoader.LoadObj("C:/Users/Kuba/Desktop/tst/teapot.obj");

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
            NEMatrix4x4 rotation = NEMatrix4x4.CreateRotationX(yDisp * 0.3f) *
            NEMatrix4x4.CreateRotationY(Engine.Instance.TotalTime);


            //NEMatrix4x4 rotation = NEMatrix4x4.CreateRotationY(Engine.Instance.TotalTime);


            for (int i = 0; i < m_VBO.Triangles.Count; ++i)
            {
                m_VBO.Triangles[i].TransformedNormal = rotation * m_VBO.Triangles[i].ModelNormal;
            }

            NEMatrix4x4 translation = NEMatrix4x4.CreateTranslation(0.0f, 0.0f, 1.0f);
            for (int i=0; i < m_VBO.Vertices.Count;++i)
            {
                m_VBO.Vertices[i].Position =  (translation * rotation) * m_VBO.ModelVertices[i].Position;
                m_VBO.Vertices[i].UV = m_VBO.ModelVertices[i].UV;
                //m_VBO.Vertices[i].Position += new NEVector4(0,0.0f,10.6f,0.0f);
               // m_VBO.Vertices[i].Position += new NEVector4(0.0f, 0.0f,1.0f /*+ yDisp*/, 0.0f);
                m_VBO.Vertices[i].Position = (m_ProjectionMat) * m_VBO.Vertices[i].Position;
                m_VBO.Vertices[i].WDivide();
            }
            NEScreenBuffer.ClearColor(2);
            m_DepthBuffer.Clear();
            m_VBO.CalculateTriangleEdges();
        }

        public override void OnDrawPerColumn(int x)
        {
            float u = ((float)x) / ((float)ScreenWidth);
            u = 2.0f * u - 1.0f;

            for (int i = 0; i < m_VBO.Triangles.Count; ++i)
            {
                Triangle tr = m_VBO.Triangles[i];
                if (!tr.IsColScanlineInTriangle(u)) continue;
                float dot = NEVector4.Dot(tr.TransformedNormal, new NEVector4(0.0f, 0.0f, -1.0f, 0.0f));
                //if (tr.TransformedNormal.Z > 0) continue;
                 //if (tr.A.Position.W < 0.0f) continue;
                ScanlineIntersectionManifest manifest;
                tr.CreateIntersectionManifest(u, out manifest);

                float y0 = (-manifest.Y0 + 1.0f) * 0.5f;
                float y1 = (-manifest.Y1 + 1.0f) * 0.5f;
                if (y0 > y1) NEMathHelper.Swap(ref y0, ref y1);
                float distance = y1 - y0;

                float y0clamped = NEMathHelper.Clamp(y0, 0, 1.0f);
                float y1clamped = NEMathHelper.Clamp(y1, 0, 1.0f);
                float distanceClamped = y1clamped - y0clamped;

                float coeff = distanceClamped / distance;

                float tOffset = 0.0f;
                if (y0 < 0.0f)
                {
                   tOffset = -y0 / distance;
                }
                int fillStart = (int) (y0clamped * (float)ScreenHeight);
                int fillEnd = (int)(y1clamped * (float)ScreenHeight);

                float span = fillEnd - fillStart;
     
                for (int y = 0; y < span ; ++y)
                {

                    float t = ((float)y / span) * coeff + tOffset;
                    float depthBottom = (1.0f - manifest.bottom_t) * manifest.bottom_P0.Z + manifest.bottom_t * manifest.bottom_P1.Z;
                    float depthTop= (1.0f - manifest.top_t) * manifest.top_P0.Z + manifest.top_t * manifest.top_P1.Z;
                    float fragmentDepth = (1.0f - t) * depthTop + t * depthBottom;

                    float fragWBottom = (1.0f - manifest.bottom_t) * manifest.bottom_P0.W + manifest.bottom_t * manifest.bottom_P1.W;
                    float fragWTop = (1.0f - manifest.top_t) * manifest.top_P0.W + manifest.top_t * manifest.top_P1.W;
                    float fragW = (1.0f - t) * fragWTop + t * fragWBottom;


                    //if (fragmentDepth < 0.0f) continue;
                    if (m_DepthBuffer.TryUpdate(x, fillStart + y, fragmentDepth))
                    {
                      

                        dot = NEMathHelper.Clamp(dot, 0.0f, 1.0f);

                        NEVector2 textCoordBottom= manifest.bottom_P0.UV * (1.0f - manifest.bottom_t)
                                                    + manifest.bottom_P1.UV * manifest.bottom_t;

                        NEVector2 textCoordTop = manifest.top_P0.UV * (1.0f - manifest.top_t)
                                                    + manifest.top_P1.UV * manifest.top_t;

                        NEVector2 texCoord = textCoordTop* (1.0f - t) + textCoordBottom* t;


                        float teX =  texCoord.X / fragW;
                        float teY =  texCoord.Y / fragW;
                        dot = 1.0f;
                        //float luma = m_LumaBuffer.FastSample(teX, 1.0f - teY);
                        //var col = NEColorSample.MakeCol10(ConsoleColor.Black, (ConsoleColor)15/*tr.ColorAttrib*/,luma *dot);
                        var col = m_Texture.Sample(teX, 1.0f - teY, dot);
                        NEScreenBuffer.PutChar(col.Character, col.BitMask, x, fillStart+y);
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

 
        private void GenerateSquare(float x, float y, float depth, int col)
        {
            float size = 0.55f;
            //m_VBO.AddVertex(new Vertex(-size+x,-size+y, depth,0.0f,1.0f));
            //m_VBO.AddVertex(new Vertex(-size+x, size+ y, depth, 0.0f, 0.0f));
            //m_VBO.AddVertex(new Vertex(size + x, size + y, depth,1.0f,0.0f));
            //m_VBO.AddVertex(new Vertex(size + x, -size +y, depth,1.0f,1.0f));
            m_VBO.AddVertex(new Vertex(-size + x, -size + y, depth, 0.0f, 0.0f));
            m_VBO.AddVertex(new Vertex(-size + x, size + y, depth, 0.0f, 1.0f));
            m_VBO.AddVertex(new Vertex(size + x, size + y, depth, 1.0f, 1.0f));
            m_VBO.AddVertex(new Vertex(size + x, -size + y, depth, 1.0f, 0.0f));

            m_VBO.AddTriangle(0, 1, 2);
            m_VBO.AddTriangle(0, 2, 3);

            m_VBO.Triangles[0].ColorAttrib = 9;
            m_VBO.Triangles[1].ColorAttrib = 9;

        }




        private void GenerateCube(float x, float y, float z, int col)
        {
            float size = 0.25f;
            m_VBO.AddVertex(new Vertex(-size + x, -size + y, z - size, 0.0f, 0.0f));
            m_VBO.AddVertex(new Vertex(-size + x, size + y, z - size, 0.0f, 1.0f));
            m_VBO.AddVertex(new Vertex(size + x, size + y, z - size, 1.0f, 1.0f));
            m_VBO.AddVertex(new Vertex(size + x, -size + y, z - size, 1.0f, 0.0f));



            m_VBO.AddVertex(new Vertex(-size + x, -size + y, z + size, 1.0f, 0.0f));
            m_VBO.AddVertex(new Vertex(-size + x, size + y, z + size, 1.0f, 1.0f));
            m_VBO.AddVertex(new Vertex(size + x, size + y, z + size, 0.0f, 1.0f));
            m_VBO.AddVertex(new Vertex(size + x, -size + y, z + size, 0.0f, 0.0f));


            m_VBO.AddTriangle(0, 1 , 2 );
            m_VBO.AddTriangle(0 , 2 , 3 );

            m_VBO.Triangles[0].ColorAttrib = 9;
            m_VBO.Triangles[1].ColorAttrib = 9;


            m_VBO.AddTriangle(4, 6, 5);
            m_VBO.AddTriangle(4, 7, 6);

            m_VBO.Triangles[2].ColorAttrib = 9;
            m_VBO.Triangles[3].ColorAttrib = 9;


            m_VBO.AddTriangle(4, 5, 1);
            m_VBO.AddTriangle(4, 1, 0);

            m_VBO.Triangles[4].ColorAttrib = 9;
            m_VBO.Triangles[5].ColorAttrib = 9;


            m_VBO.AddTriangle(3, 2, 6);
            m_VBO.AddTriangle(3, 6, 7);

            m_VBO.Triangles[6].ColorAttrib = 9;
            m_VBO.Triangles[7].ColorAttrib = 9;



            m_VBO.AddTriangle(1, 5, 6);
            m_VBO.AddTriangle(1, 6, 2);

            m_VBO.Triangles[8].ColorAttrib = 9;
            m_VBO.Triangles[9].ColorAttrib = 9;


            m_VBO.AddTriangle(0, 3, 7);
            m_VBO.AddTriangle(0, 7, 4);

            m_VBO.Triangles[10].ColorAttrib = 9;
            m_VBO.Triangles[11].ColorAttrib = 9;

        }

    }
}
