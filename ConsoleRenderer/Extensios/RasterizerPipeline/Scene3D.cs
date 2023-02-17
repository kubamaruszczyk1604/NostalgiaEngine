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
        // Mesh m_VBO;
        NEDepthBuffer m_DepthBuffer;

        Skybox m_TestSkybox;
        //NEFloatBuffer m_Skybox;
        //NETexture m_Texture;
        NEColorPalette m_Palette;



        Camera m_Camera;
        List<Model> m_Models;

        int m_RenderedTriangleCount = 0;
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
            //PixelHeight = 8;

            //ScreenWidth = 220;
            //ScreenHeight = 180;
            //PixelWidth = 5;
            //PixelHeight = 5;
            // ResourceManager.Instance.Test();
            //m_LumaBuffer = NEFloatBuffer.FromFile("C:/test/ruler/luma.buf");
            //m_Texture = NEColorTexture16.LoadFromFile("C:/test/nowa_textura12/color.tex");
            // m_Palette = NEColorPalette.FromFile("C:/test/nowa_textura12/palette.txt");
            m_Palette = NEColorPalette.FromFile("C:/test/skybox3/px/palette.txt");
            //m_LumaBuffer.SampleMode = NESampleMode.Repeat;
            m_DepthBuffer = new NEDepthBuffer(ScreenWidth, ScreenHeight);
            m_Models = new List<Model>();

            //m_Skybox = ResourceManager.Instance.GetLumaTexture("C:/test/skybox/right.buf");
            //m_Skybox.SampleMode = NESampleMode.Repeat;

            m_TestSkybox = new Skybox("c:/test/skybox3");

            Mesh cubeMesh = GenerateCube(0.0f, 0.0f, 0f, 15);
            Mesh floorMesh = GenerateSquare(0.0f, -0.3f,1.0f);
            Mesh teapotMesh = NEObjLoader.LoadObj("C:/Users/Kuba/Desktop/tst/teapot.obj");
            var luma = ResourceManager.Instance.GetLumaTexture("C:/test/ntex/luma.buf");
            Model cubeModel = new Model(cubeMesh, luma);
            //Model cubeModel = new Model(GenerateSquare(0.0f, 0.0f, 0.0f), luma);
            cubeModel.Transform.LocalPosition = new NEVector4(0.9f, 0.0f, 4.1f);


            Model floorModel = new Model(floorMesh, luma);

            Model teapotModel = new Model(teapotMesh);
            teapotModel.Transform.ScaleX = 0.4f;
            teapotModel.Transform.ScaleY = 0.4f;
            teapotModel.Transform.ScaleZ = 0.4f;
            teapotModel.Transform.LocalPosition = new NEVector4(-1.0f, -1.05f, 1.0f, 1.0f);

            m_Models.Add(cubeModel);
            m_Models.Add(teapotModel);
            //m_Models.Add(floorModel);

            m_Camera = new Camera(ScreenWidth, ScreenHeight, 1.05f, 0.1f, 100.0f);
            m_Camera.Transform.LocalPosition = new NEVector4(0.0f, 0.0f, -2.0f);


            NEColorManagement.SetPalette(m_Palette);
            //NEColorManagement.SetSpectralPalette1();

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

        private void Movement(float dt)
        {
            if (NEInput.CheckKeyDown(ConsoleKey.LeftArrow))
            {
                if (NEInput.CheckKeyDown(NEKey.Alt))
                {
                    m_Camera.Transform.LocalPosition = m_Camera.Transform.LocalPosition - m_Camera.Transform.Right * dt;
                }
                else
                {
                    m_Camera.Transform.RotateY(dt);
                }
            }

            if (NEInput.CheckKeyDown(ConsoleKey.RightArrow))
            {

                if (NEInput.CheckKeyDown(NEKey.Alt))
                {
                    m_Camera.Transform.LocalPosition = m_Camera.Transform.LocalPosition + m_Camera.Transform.Right * dt;
                }
                else
                {
                    m_Camera.Transform.RotateY(-dt);
                }
            }

            if (NEInput.CheckKeyDown(ConsoleKey.UpArrow))
            {
                m_Camera.Transform.LocalPosition = m_Camera.Transform.LocalPosition + m_Camera.Transform.Forward * dt;
                // m_Camera.Transform.RotateX(dt);
            }
            if (NEInput.CheckKeyDown(ConsoleKey.DownArrow))
            {
                m_Camera.Transform.LocalPosition = m_Camera.Transform.LocalPosition - m_Camera.Transform.Forward * dt;
               // m_Camera.Transform.RotateX(-dt);
            }
        }

        public void ProcessModel(float dt, Model model)
        {
            model.Transform.CalculateWorld();
            Mesh mesh = model.Mesh;
            mesh.TempTriangleContainer = new List<Triangle>(mesh.Triangles.Count);

            NEMatrix4x4 world = model.Transform.World;
            NEMatrix4x4 view = m_Camera.View;

            for (int i = 0; i < mesh.Vertices.Count; ++i)
            {
                mesh.Vertices[i].Position = (view * world) * mesh.ModelVertices[i].Position;
                mesh.Vertices[i].Vert2Camera = -mesh.Vertices[i].Position.Normalized;
                mesh.Vertices[i].UV = mesh.ModelVertices[i].UV;
                mesh.Vertices[i].Position = (m_Camera.Projection) * mesh.Vertices[i].Position;

                mesh.Vertices[i].WDivide();
            }

            //Projection space
            //mesh.CalculateTriangleEdges();
            for (int i = 0; i < mesh.Triangles.Count; ++i)
            {
                Triangle triangle = mesh.Triangles[i];
                triangle.CalculateEdges();
                if (IsOutsideFrustum(triangle)) continue;
                triangle.TransformedNormal = NEMatrix4x4.RemoveTranslation(m_Camera.View) * model.Transform.RotationMat * mesh.Triangles[i].ModelNormal;
                if (FacingAway(triangle)) continue;
                m_RenderedTriangleCount++;

                mesh.TempTriangleContainer.Add(triangle);
            }



            NEScreenBuffer.ClearColor(0);
            m_DepthBuffer.Clear();

        }


        bool IsOutsideFrustum(Triangle triangle)
        {
            float depthA = triangle.A.UnidividedW * m_Camera.InverseFar;
            float depthB = triangle.B.UnidividedW * m_Camera.InverseFar;
            float depthC = triangle.C.UnidividedW * m_Camera.InverseFar;

            if (depthA < 0.0f && depthB < 0.0f && depthC < 0.0f) return true;
            if (depthA > 1.0f && depthB > 1.0f && depthC > 1.0f) return true;

            float xA = triangle.A.X;
            float xB = triangle.B.X;
            float xC = triangle.C.X;

            float yA = triangle.A.Y;
            float yB = triangle.B.Y;
            float yC = triangle.C.Y;

            //bool sameX = (Math.Abs(Math.Sign(xA) + Math.Sign(xB) + Math.Sign(xC)) == 3);
            //bool sameY = (Math.Abs(Math.Sign(yA) + Math.Sign(yB) + Math.Sign(yC)) == 3);

            if (xA < -1.0f && xB < -1.0f && xC < -1.0f) return true;
            if (xA > 1.0f && xB > 1.0f && xC > 1.0f) return true;

            if (yA < -1.0f && yB < -1.0f && yC < -1.0f) return true;
            if (yA > 1.0f && yB > 1.0f && yC > 1.0f) return true;


            return false;
        }

        bool FacingAway(Triangle triangle)
        {
            NEVector4 vA = -triangle.A.Position.Normalized;
            NEVector4 vB = -triangle.B.Position.Normalized;
            NEVector4 vC = -triangle.C.Position.Normalized;

            float dotA = NEVector4.Dot(vA, triangle.TransformedNormal);
            float dotB = NEVector4.Dot(vB, triangle.TransformedNormal);
            float dotC = NEVector4.Dot(vC, triangle.TransformedNormal);

            return (dotA < -0.4f && dotB < -0.4f && dotC < -0.4f);
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            Movement(deltaTime);
            m_Models[1].Transform.RotateY(deltaTime*0.5f);
            m_Models[1].Transform.PositionY = -0.7f + (float)(Math.Sin(Engine.Instance.TotalTime) * 0.3);
            m_Camera.UpdateTransform();

            float yDisp = (float)Math.Sin(Engine.Instance.TotalTime);
            Engine.Instance.TitleBarAppend = "Rendered Triangles: " + m_RenderedTriangleCount.ToString();
            m_RenderedTriangleCount = 0;
            for (int i = 0; i < m_Models.Count; ++i)
            {
                ProcessModel(deltaTime, m_Models[i]);
            }
        }

        void RenderModel(int x, float u, Model model)
        {

            Mesh m_VBO = model.Mesh;
            for (int i = 0; i < m_VBO.TempTriangleContainer.Count; ++i)
            {
                Triangle tr = m_VBO.TempTriangleContainer[i];
                if (!tr.IsColScanlineInTriangle(u)) continue;

                //float dot = NEVector4.Dot(tr.TransformedNormal, new NEVector4(0.0f, 0.0f, -1.0f, 0.0f));
                ScanlineIntersectionManifest manifest;
                tr.CreateIntersectionManifest(u, out manifest);

                //go from normailzed device coordinates to screen space
                float y0 = (-manifest.Y0 + 1.0f) * 0.5f;
                float y1 = (-manifest.Y1 + 1.0f) * 0.5f;

                //y0 is first from the top of the viewport 
                if (y0 > y1) NEMathHelper.Swap(ref y0, ref y1);

                //normalised span of rendered line segment
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
                int fillStart = (int)(y0clamped * ScreenHeight);
                int fillEnd = (int)(y1clamped * ScreenHeight);

                float span = fillEnd - fillStart;

                for (int y = 0; y < span; ++y)
                {

                    float t = ((float)y / span) * coeff + tOffset;

                    float depthBottom = (1.0f - manifest.bottom_t) * manifest.bottom_P0.UnidividedW + manifest.bottom_t * manifest.bottom_P1.UnidividedW;
                    float depthTop = (1.0f - manifest.top_t) * manifest.top_P0.UnidividedW + manifest.top_t * manifest.top_P1.UnidividedW;
                    float fragmentDepth = (1.0f - t) * depthTop + t * depthBottom;
                    fragmentDepth *= m_Camera.InverseFar;

                    if (fragmentDepth > 1.0f || fragmentDepth <= 0)
                    {
                        continue;
                    }

                    if (m_DepthBuffer.TryUpdate(x, fillStart + y, fragmentDepth))
                    {
                        NEVector4 vDirBottom = manifest.bottom_P0.Vert2Camera * (1.0f - manifest.bottom_t) + manifest.bottom_P1.Vert2Camera * manifest.bottom_t;
                        NEVector4 vDirTop = manifest.top_P0.Vert2Camera * (1.0f - manifest.top_t) + manifest.top_P1.Vert2Camera * manifest.top_t;
                        NEVector4 vDir = vDirTop * (1.0f - t) + vDirBottom * t;

                        float dot = NEVector4.Dot(tr.TransformedNormal, vDir);
                        dot = NEMathHelper.Clamp(dot, 0.0f, 1.0f);

                        float fragWBottom = (1.0f - manifest.bottom_t) * manifest.bottom_P0.W + manifest.bottom_t * manifest.bottom_P1.W;
                        float fragWTop = (1.0f - manifest.top_t) * manifest.top_P0.W + manifest.top_t * manifest.top_P1.W;
                        float fragW = (1.0f - t) * fragWTop + t * fragWBottom;

                        NEVector2 textCoordBottom = manifest.bottom_P0.UV * (1.0f - manifest.bottom_t)
                                                    + manifest.bottom_P1.UV * manifest.bottom_t;

                        NEVector2 textCoordTop = manifest.top_P0.UV * (1.0f - manifest.top_t)
                                                    + manifest.top_P1.UV * manifest.top_t;

                        NEVector2 texCoord = textCoordTop * (1.0f - t) + textCoordBottom * t;


                        float teX = texCoord.X / fragW;
                        float teY = texCoord.Y / fragW;
                        // dot = 1.0f;
                        float luma = 0.9f + dot;// * dot;
                        if (model.LumaTexture != null)
                        {
                            luma *= model.LumaTexture.FastSample(teX, 1.0f - teY);
                        }

                        var col = NEColorSample.MakeCol5(ConsoleColor.Black, (ConsoleColor)tr.ColorAttrib, luma);
                        //var col = m_Texture.Sample(teX, 1.0f - teY, dot);
                        NEScreenBuffer.PutChar(col.Character, col.BitMask, x, fillStart + y);
                    }

                }
            }

        }


        public override void OnDrawPerColumn(int x)
        {
            float xNorm = ((float)x) / ((float)ScreenWidth);
            float u = 2.0f * xNorm - 1.0f;


            for (int i = 0; i < m_Models.Count; ++i)
            {
                RenderModel(x, u, m_Models[i]);
            }
            float rayZ = 1.0f / (float)Math.Tan(m_Camera.FovRad * 0.5f);
            for (int y = 0; y < ScreenHeight; ++y)
            {
                if (m_DepthBuffer.TryUpdate(x, y, 1.0f))
                {
                    float v = (float)y / (float)ScreenHeight;
                    v = -((2.0f * v) - 1.0f);
                    NEVector4 sampleDir = (m_Camera.PointAt) * new NEVector4(u * m_Camera.InverseAspectRatio, v, rayZ, 0.0f).Normalized;
                    float luma = m_TestSkybox.Sample(sampleDir);
                    int low = 0;
                    int high = 15;
                    if (luma > 0.1f) low = 6;
                    if (luma > 0.8f) high = 12;
                    var col = NEColorSample.MakeCol10((ConsoleColor)low, (ConsoleColor)high, luma);
                    NEScreenBuffer.PutChar(col.Character, col.BitMask, x, y);
                }

            }
            base.OnDrawPerColumn(x);
        }




        public override bool OnDraw()
        {
            for (int y = (int)c_ColorPanelPos.Y; y < ((int)c_ColorPanelPos.Y + c_ColorWindowHeight); ++y)
            {
                for (int x = 0; x < ScreenWidth; ++x)
                {

                    DrawPalette(x, y);
                }
            }

            return base.OnDraw();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        readonly int c_ColorWindowWidth = 10;
        readonly int c_ColorWindowHeight = 10;
        NEVector2 c_ColorPanelPos = new NEVector2(10, 10);
        private void DrawPalette(int x, int y)
        {
            NEVector2 pixelPos = new NEVector2(x, y);
            for (int i = 0; i < 16; ++i)
            {
                if (NEMathHelper.InRectangle(pixelPos, new NEVector2(c_ColorWindowWidth * (i), 0) + c_ColorPanelPos, c_ColorWindowWidth, c_ColorWindowHeight))
                {
                    NEScreenBuffer.PutChar(' ', (short)((i) << 4), x, y);
                    if (i == 16) NEScreenBuffer.PutChar((char)NEBlock.Solid, (short)(8 << 4), x, y);
                }
            }
        }

        private Mesh GenerateTestTriangles()
        {
            Mesh mesh = new Mesh();
            return mesh;
        }


        private Mesh GenerateSquare(float x, float y, float z)
        {
            Mesh mesh = new Mesh();
            float size = 1.55f;
            //m_VBO.AddVertex(new Vertex(-size+x,-size+y, depth,0.0f,1.0f));
            //m_VBO.AddVertex(new Vertex(-size+x, size+ y, depth, 0.0f, 0.0f));
            //m_VBO.AddVertex(new Vertex(size + x, size + y, depth,1.0f,0.0f));
            //m_VBO.AddVertex(new Vertex(size + x, -size +y, depth,1.0f,1.0f));
            mesh.AddVertex(new Vertex(-size * 100 + x, -size * 10 + y, z, 0.0f, 0.0f));
            mesh.AddVertex(new Vertex(-size * 100 + x, size * 10 + y, z, 0.0f, 1.0f));
            mesh.AddVertex(new Vertex(size * 100 + x, size * 10 + y, z, 1.0f, 1.0f));
             mesh.AddVertex(new Vertex(size * 100 + x, -size * 10 + y, z, 1.0f, 0.0f));

            mesh.AddTriangle(0, 1, 2);
            mesh.AddTriangle(0, 2, 3);

            mesh.Triangles[0].ColorAttrib = 10;
            mesh.Triangles[1].ColorAttrib = 10;
            return mesh;

        }


        private Mesh GenerateSquareFloor(float x, float y, float z)
        {
            Mesh mesh = new Mesh();
            float size = 0.55f;
            mesh.AddVertex(new Vertex(-size + x, y, -size + z, 0.0f, 0.0f));
            mesh.AddVertex(new Vertex(-size + x, y, size + z, 0.0f, 1.0f));
            mesh.AddVertex(new Vertex(size + x, y, size  + z, 1.0f, 1.0f));
            mesh.AddVertex(new Vertex(size + x, y, -size  + z, 1.0f, 0.0f));

            //mesh.AddVertex(new Vertex(-size + x, -size + y, z, 0.0f, 0.0f));
            //mesh.AddVertex(new Vertex(-size + x, size + y, z, 0.0f, 1.0f));
            //mesh.AddVertex(new Vertex(size + x, size + y, z, 1.0f, 1.0f));
            //mesh.AddVertex(new Vertex(size + x, -size + y, z, 1.0f, 0.0f));

            mesh.AddTriangle(0, 1, 2);
             mesh.AddTriangle(0, 2, 3);

            mesh.Triangles[0].ColorAttrib = 12;
            mesh.Triangles[1].ColorAttrib = 12;
            return mesh;

        }




        private Mesh GenerateCube(float x, float y, float z, int col)
        {
            Mesh mesh = new Mesh();
            float size = 1.25f;
            mesh.AddVertex(new Vertex(-size + x, -size + y, z - size, 0.0f, 0.0f));
            mesh.AddVertex(new Vertex(-size + x, size + y, z - size, 0.0f, 1.0f));
            mesh.AddVertex(new Vertex(size + x, size + y, z - size, 1.0f, 1.0f));
            mesh.AddVertex(new Vertex(size + x, -size + y, z - size, 1.0f, 0.0f));



            mesh.AddVertex(new Vertex(-size + x, -size + y, z + size, 1.0f, 0.0f));
            mesh.AddVertex(new Vertex(-size + x, size + y, z + size, 1.0f, 1.0f));
            mesh.AddVertex(new Vertex(size + x, size + y, z + size, 0.0f, 1.0f));
            mesh.AddVertex(new Vertex(size + x, -size + y, z + size, 0.0f, 0.0f));


            mesh.AddTriangle(0, 1, 2);
            mesh.AddTriangle(0, 2, 3);

            mesh.Triangles[0].ColorAttrib = col;
            mesh.Triangles[1].ColorAttrib = col;


            mesh.AddTriangle(4, 6, 5);
            mesh.AddTriangle(4, 7, 6);

            mesh.Triangles[2].ColorAttrib = col;
            mesh.Triangles[3].ColorAttrib = col;


            mesh.AddTriangle(4, 5, 1);
            mesh.AddTriangle(4, 1, 0);

            mesh.Triangles[4].ColorAttrib = col;
            mesh.Triangles[5].ColorAttrib = col;


            mesh.AddTriangle(3, 2, 6);
            mesh.AddTriangle(3, 6, 7);

            mesh.Triangles[6].ColorAttrib = col;
            mesh.Triangles[7].ColorAttrib = col;



            mesh.AddTriangle(1, 5, 6);
            mesh.AddTriangle(1, 6, 2);

            mesh.Triangles[8].ColorAttrib = col;
            mesh.Triangles[9].ColorAttrib = col;


            mesh.AddTriangle(0, 3, 7);
            mesh.AddTriangle(0, 7, 4);

            mesh.Triangles[10].ColorAttrib = col;
            mesh.Triangles[11].ColorAttrib = col;
            return mesh;
        }

    }
}
