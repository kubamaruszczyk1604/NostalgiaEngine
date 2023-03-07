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
        
        NEColorPalette m_Palette;


        protected Skybox SceneSkybox { get; set; }
        protected Camera MainCamera { get;  set; }
        protected List<Model> Models;



        int m_RenderedTriangleCount = 0;


        public Scene3D():base()
        {
            ScreenWidth = 320;
            ScreenHeight = 200;
            PixelWidth = 4;
            PixelHeight = 4;

            //ScreenWidth = 200;
            //ScreenHeight = 100;
            //PixelWidth = 6;
            //PixelHeight = 6;
            m_DepthBuffer = new NEDepthBuffer(ScreenWidth, ScreenHeight);
            Models = new List<Model>();
            SceneSkybox = new Skybox();
            MainCamera = new Camera(ScreenWidth, ScreenHeight, 1.05f, 0.1f, 100.0f);
            MainCamera.Transform.LocalPosition = new NEVector4(0.0f, 0.0f, -2.0f);
        }

        public override bool OnLoad()
        {


            m_Palette = NEColorPalette.FromFile("C:/test/skybox3/px/palette.txt");
            NEColorManagement.SetPalette(m_Palette);
            //Clipping.DebugMode = true;
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

            m_DepthBuffer.Clear();
            MainCamera.UpdateTransform();

            float yDisp = (float)Math.Sin(Engine.Instance.TotalTime);
            Engine.Instance.TitleBarAppend = "Rendered Triangles: " + m_RenderedTriangleCount.ToString();
            m_RenderedTriangleCount = 0;
            for (int i = 0; i < Models.Count; ++i)
            {
                ProcessModel(deltaTime, Models[i]);
            }

        }

        public override void OnDrawPerColumn(int x)
        {
            float xNorm = ((float)x) / ((float)ScreenWidth);
            float u = 2.0f * xNorm - 1.0f;


            for (int i = 0; i < Models.Count; ++i)
            {
                RenderModel(x, u, Models[i]);
            }

           if(SceneSkybox.Available) RenderSkybox(x, u);
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

       

        private void RenderSkybox(int x, float u)
        {
            float rayZ = 1.0f / (float)Math.Tan(MainCamera.FovRad * 0.5f);
            for (int y = 0; y < ScreenHeight; ++y)
            {
                if (m_DepthBuffer.TryUpdate(x, y, 1.0f))
                {
                    float v = (float)y / (float)ScreenHeight;
                    v = -((2.0f * v) - 1.0f);
                    NEVector4 sampleDir = (MainCamera.PointAt) * new NEVector4(u * MainCamera.InverseAspectRatio, v, rayZ, 0.0f).Normalized;
                    float luma = SceneSkybox.Sample(sampleDir);
                    int low = 0;
                    int high = 15;
                    if (luma > 0.1f) low = 6;
                    if (luma > 0.8f) high = 12;
                    var col = NEColorSample.MakeCol10((ConsoleColor)low, (ConsoleColor)high, luma);
                    NEScreenBuffer.PutChar(col.Character, col.BitMask, x, y);
                }

            }
        }

  

        void ProcessModel(float dt, Model model)
        {
            model.Transform.CalculateWorld();
            Mesh mesh = model.Mesh;
            mesh.ClearProcessedData();

            NEMatrix4x4 world = model.Transform.World;
            NEMatrix4x4 view = MainCamera.View;
            for (int i = 0; i < mesh.ModelVertices.Count; ++i)
            {
                mesh.ProcessedVertices.Add(mesh.ModelVertices[i].Duplicate());
                mesh.ProcessedVertices[i].Position = (view * world) * mesh.ProcessedVertices[i].Position;
                mesh.ProcessedVertices[i].Vert2Camera = -mesh.ProcessedVertices[i].Position.Normalized;
                mesh.ProcessedVertices[i].UV = mesh.ModelVertices[i].UV;
                mesh.ProcessedVertices[i].Position = (MainCamera.Projection) * mesh.ProcessedVertices[i].Position;
            }

            //Projection space
            for (int i = 0; i < mesh.ModelTriangles.Count; ++i)
            {
                Triangle tri = mesh.ModelTriangles[i];
                tri.TransformedNormal = NEMatrix4x4.RemoveTranslation(MainCamera.View) * model.Transform.RotationMat * mesh.ModelTriangles[i].ModelNormal;
                if (CullTest(tri, model.FaceCull)) continue;
                List<Triangle> nearClipped = Clipping.ClipTriangleAgainstPlane(tri, mesh, ClipPlane.Near);
                foreach (Triangle triangle in nearClipped)
                {    
                    mesh.TempTriangleContainer.Add(triangle);
                }

            }
            for (int i = 0; i < mesh.TempTriangleContainer.Count; ++i)
            {
                Triangle triangle = mesh.TempTriangleContainer[i];
                triangle.ZDivide();
                triangle.CalculateEdges();
                
            }

            for (int i = 0; i < mesh.TempTriangleContainer.Count; ++i)
            {
                Triangle triangle = mesh.TempTriangleContainer[i];
                if (IsOutsideFrustum(triangle)) continue;
                //if (FacingAway(triangle)) continue;
                List<Triangle> LeftClipped = Clipping.ClipTriangleAgainstPlane(triangle, mesh, ClipPlane.Left);
                List<Triangle> RightClipped = Clipping.ClipTrianglesAgainstPlane(LeftClipped, mesh, ClipPlane.Right);
                List<Triangle> BottomClipped = Clipping.ClipTrianglesAgainstPlane(RightClipped, mesh, ClipPlane.Bottom);
                List<Triangle> TopClipped = Clipping.ClipTrianglesAgainstPlane(BottomClipped, mesh, ClipPlane.Top);
                List<Triangle> FarClipped = Clipping.ClipTrianglesAgainstPlane(TopClipped, mesh, ClipPlane.Far);

                mesh.ProcessedTriangles.AddRange(FarClipped);
            }

            m_RenderedTriangleCount += mesh.ProcessedTriangles.Count;
            //NEScreenBuffer.ClearColor(0);
            //m_DepthBuffer.Clear();

        }

        bool IsOutsideFrustum(Triangle triangle)
        {
            float depthA = triangle.A.Z;
            float depthB = triangle.B.Z;
            float depthC = triangle.C.Z;

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

        bool CullTest(Triangle triangle, CullMode cullMode)
        {
            if (cullMode == CullMode.None) return false;
            //NEVector4 vA = -triangle.A.Position.Normalized;
            //NEVector4 vB = -triangle.B.Position.Normalized;
            //NEVector4 vC = -triangle.C.Position.Normalized;

            NEVector4 vA = -triangle.VBO.ProcessedVertices[triangle.Indices[0]].Position;
            NEVector4 vB = -triangle.VBO.ProcessedVertices[triangle.Indices[1]].Position;
            NEVector4 vC = -triangle.VBO.ProcessedVertices[triangle.Indices[2]].Position;

            float dotA = NEVector4.Dot3(vA, triangle.TransformedNormal);
            float dotB = NEVector4.Dot3(vB, triangle.TransformedNormal);
            float dotC = NEVector4.Dot3(vC, triangle.TransformedNormal);

            return (dotA < 0.0f && dotB < 0.0f && dotC < 0.0f) ^ (cullMode == CullMode.Front);
        }

        void RenderModel(int x, float u, Model model)
        {

            Mesh m_VBO = model.Mesh;
            for (int i = 0; i < m_VBO.ProcessedTriangles.Count; ++i)
            {
                Triangle tr = m_VBO.ProcessedTriangles[i];
                if (!tr.IsColScanlineInTriangle(u)) continue;

                ScanlineIntersectionManifest manifest;
                tr.ComputeScanlineIntersection(u, out manifest);

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

                    float t = ((float)y / span)* coeff + tOffset;

                    float depthBottom = (1.0f - manifest.bottom_t) * manifest.bottom_P0.Z+ manifest.bottom_t * manifest.bottom_P1.Z;
                    float depthTop = (1.0f - manifest.top_t) * manifest.top_P0.Z + manifest.top_t * manifest.top_P1.Z;
                    float fragmentDepth = (1.0f - t) * depthTop + t * depthBottom;

                    if (fragmentDepth > 1.0f || fragmentDepth <= 0)
                    {
                        continue;
                    }

                    if (m_DepthBuffer.TryUpdate(x, fillStart + y, fragmentDepth))
                    {
                        NEVector4 vDirBottom = manifest.bottom_P0.Vert2Camera * (1.0f - manifest.bottom_t) + manifest.bottom_P1.Vert2Camera * manifest.bottom_t;
                        NEVector4 vDirTop = manifest.top_P0.Vert2Camera * (1.0f - manifest.top_t) + manifest.top_P1.Vert2Camera * manifest.top_t;
                        NEVector4 vDir = vDirTop * (1.0f - t) + vDirBottom * t;

                        float dot = NEVector4.Dot3(tr.TransformedNormal, vDir);
                        dot = NEMathHelper.Abs(dot);
                        //dot = NEMathHelper.Clamp(dot, 0.0f, 1.0f);

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
                        float luma = 0.2f + dot * dot;
                        if (model.LumaTexture != null)
                        {
                            luma *= model.LumaTexture.FastSample(teX, 1.0f - teY);
                        }

                        var col = NEColorSample.MakeCol5(ConsoleColor.Black, (ConsoleColor)tr.ColorAttrib, 1.0f*luma);
                        //var col = m_Texture.Sample(teX, 1.0f - teY, dot);
                        NEScreenBuffer.PutChar(col.Character, col.BitMask, x, fillStart + y);
                    }

                }
            }

        }

        int c_ColorWindowWidth = 10;
        int c_ColorWindowHeight = 10;
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


    }
}
