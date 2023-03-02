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
        struct Intersection
        {
            public bool AB;
            public bool AC;
            public bool BC;
            
            public bool None { get { return !AB && !AC && !BC; } }

           
        }
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
            Mesh floorMesh = GenerateSquareFloor(0.0f, -0.3f,1.0f);
            Mesh teapotMesh = NEObjLoader.LoadObj("C:/Users/Kuba/Desktop/tst/teapot.obj");
            var luma = ResourceManager.Instance.GetLumaTexture("C:/test/ruler/luma.buf");
            //Model cubeModel = new Model(cubeMesh, luma);
            Model cubeModel = new Model(GenerateSquare(0.0f, 0.0f, 0.0f), luma);
            cubeModel.Transform.LocalPosition = new NEVector4(0.9f, 0.0f, 4.1f);


            Model floorModel = new Model(floorMesh, luma);

            Model teapotModel = new Model(teapotMesh);
            teapotModel.Transform.ScaleX = 0.4f;
            teapotModel.Transform.ScaleY = 0.4f;
            teapotModel.Transform.ScaleZ = 0.4f;
            teapotModel.Transform.LocalPosition = new NEVector4(-1.0f, -1.05f, 1.0f, 1.0f);

           m_Models.Add(cubeModel);
            //m_Models.Add(teapotModel);
            m_Models.Add(floorModel);

            m_Camera = new Camera(ScreenWidth, ScreenHeight, 1.05f, 0.8f, 100.0f);
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
            mesh.TempTriangles.Clear();

            NEMatrix4x4 world = model.Transform.World;
            NEMatrix4x4 view = m_Camera.View;
            mesh.FrameProcessedVertices.Clear();
            for (int i = 0; i < mesh.ModelVertices.Count; ++i)
            {
                mesh.FrameProcessedVertices.Add(mesh.ModelVertices[i].Duplicate());
                mesh.FrameProcessedVertices[i].Position = (view * world) * mesh.FrameProcessedVertices[i].Position;
                mesh.FrameProcessedVertices[i].Vert2Camera = -mesh.FrameProcessedVertices[i].Position.Normalized;
                mesh.FrameProcessedVertices[i].UV = mesh.ModelVertices[i].UV;
                mesh.FrameProcessedVertices[i].Position = (m_Camera.Projection) * mesh.FrameProcessedVertices[i].Position;

                //mesh.FrameProcessedVertices[i].WDivide();
            }

            //Projection space
            for (int i = 0; i < mesh.ModelTriangles.Count; ++i)
            {
                Triangle tri = mesh.ModelTriangles[i];
                tri.TransformedNormal = NEMatrix4x4.RemoveTranslation(m_Camera.View) * model.Transform.RotationMat * mesh.ModelTriangles[i].ModelNormal;
                List<Triangle> nearClipped = ClipNear(tri, mesh);
                foreach (Triangle triangle in nearClipped)
                {
                    //triangle.WDivide();


                    //triangle.CalculateEdges();
                    //if (IsOutsideFrustum(triangle)) continue;
                   // triangle.TransformedNormal = NEMatrix4x4.RemoveTranslation(m_Camera.View) * model.Transform.RotationMat * mesh.ModelTriangles[i].ModelNormal;
                    // if (FacingAway(triangle)) continue;
                    mesh.TempTriangles.Add(triangle);


                   // DoClipping(triangle, mesh);
                }
                //m_RenderedTriangleCount++;

                //mesh.TempTriangles.Add(triangle);
            }
            for (int i = 0; i < mesh.TempTriangles.Count; ++i)
            {
                Triangle triangle = mesh.TempTriangles[i];
                triangle.WDivide();


                triangle.CalculateEdges();
            }

                m_RenderedTriangleCount += mesh.TempTriangles.Count;
            NEScreenBuffer.ClearColor(0);
            m_DepthBuffer.Clear();

        }




        void DoClipping(Triangle inTriangle,  Mesh mesh)
        {
            List<Triangle> trianglesLeft = ClipLeft(inTriangle, mesh);
            List<Triangle> trianglesRight = new List<Triangle>(3);
            for (int i = 0; i < trianglesLeft.Count; ++i)
            {
               trianglesRight.AddRange(ClipRight(trianglesLeft[i], mesh));
            }

            //List<Triangle> trianglesNear = new List<Triangle>(9);
            //for (int i = 0; i < trianglesRight.Count; ++i)
            //{
            //    trianglesNear.AddRange(ClipNear(trianglesRight[i], mesh));
            //}

            mesh.TempTriangles.AddRange(trianglesRight);


            // outTriangles.AddRange(trl);

        }
       


        List<Triangle> ClipLeft(Triangle inTriangle,  Mesh mesh)
        {
            List<Vertex> vertices = mesh.FrameProcessedVertices;
            List<Triangle> newTriangles = new List<Triangle>();
            Vertex AC; Vertex BC; Vertex AB;
            Intersection lp = FindIntersections(inTriangle, out AB, out AC, out BC, NEVector4.Left, NEVector4.Right);

            if (!lp.AC)
            {
                newTriangles.Add(inTriangle);
            }
            else if (lp.AC && lp.BC)
            {
                vertices.Add(AC); vertices.Add(BC);
                int new1 = vertices.Count - 2;
                int new2 = vertices.Count - 1;
                int old = inTriangle.LeftSortedIndices[2];
                VertsToTris(mesh, inTriangle, new1, new2, old,  newTriangles);
            }
            else if (lp.AC && lp.AB)
            {

                vertices.Add(AC);  vertices.Add(AB);

                int new1 = vertices.Count - 2;
                int new2 = vertices.Count - 1;
                int old2 = inTriangle.LeftSortedIndices[1];
                int old3 = inTriangle.LeftSortedIndices[2];

                VertsToTris(mesh, inTriangle, new1, new2, old2, old3, newTriangles);

            }
            return newTriangles;
            
        }

        void VertsToTris(Mesh mesh,Triangle triangle, int va, int vb, int vc, int vd, List<Triangle> triangleStream)
        {
            List<Vertex> vertices = mesh.FrameProcessedVertices;
            List<int> vrt = new List<int>(3);
            vrt.AddRange(new int[] { vb, vc, vd});

            int v1 = Mesh.GetLeftmost(vertices, triangle.TransformedNormal, va, vb, vc, vd);
            vrt.Remove(v1);
            int v2 = Mesh.GetLeftmost(vertices, triangle.TransformedNormal, v1, vrt[0], vrt[1]);
            vrt.Remove(v2);


            Triangle tr1 = new Triangle(va, v1, v2, mesh, triangle.ModelNormal, triangle.TransformedNormal);
            tr1.ColorAttrib = 2;
            tr1.CalculateEdges();
            triangleStream.Add(tr1);

            Triangle tr2 = new Triangle(va, v2, vrt[0], mesh, triangle.ModelNormal, triangle.TransformedNormal);
            tr2.ColorAttrib = 1;
            tr2.CalculateEdges();
            triangleStream.Add(tr2);
        }

        void VertsToTris(Mesh mesh, Triangle triangle, int va, int vb, int vc,  List<Triangle> triangleStream)
        {
            int v1 = Mesh.GetLeftmost(mesh.FrameProcessedVertices, triangle.TransformedNormal, va, vb, vc);
            int v2 = 0;
            if(vb == v1) v2 = vc;
            else v2 = vb;

            Triangle tr = new Triangle(va, v1, v2, mesh, triangle.ModelNormal, triangle.TransformedNormal);
            tr.ColorAttrib = 9;
            tr.CalculateEdges();
            triangleStream.Add(tr);
        }

        List<Triangle> ClipRight(Triangle inTriangle,  Mesh mesh)
        {
            List<Vertex> vertices = mesh.FrameProcessedVertices;
            List<Triangle> newTriangles = new List<Triangle>();
            Vertex AC; Vertex BC; Vertex AB;
            Intersection lp = FindIntersections(inTriangle, out AB, out AC, out BC, NEVector4.Right, NEVector4.Left);

            if (!lp.AC)
            {
                newTriangles.Add(inTriangle);
            }
            else if (lp.AC && lp.AB)
            {
                vertices.Add(AC); vertices.Add(AB);
                int new1 = vertices.Count - 2;
                int new2 = vertices.Count - 1;
                int old = inTriangle.LeftSortedIndices[0];
                VertsToTris(mesh, inTriangle, new1, new2, old, newTriangles);
            }
            else if (lp.AC && lp.BC)
            {
                Vertex.OrderByY(ref AC, ref BC);

                vertices.Add(AC);
                vertices.Add(BC);
                int new1 = vertices.Count - 2;
                int new2 = vertices.Count - 1;
                int old2 = inTriangle.LeftSortedIndices[0];
                int old3 = inTriangle.LeftSortedIndices[1];

                VertsToTris(mesh, inTriangle, new1, new2, old2, old3, newTriangles);
            }
            return newTriangles;
        }


        List<Triangle> ClipNear(Triangle inTriangle,  Mesh mesh)
        {
            List<Vertex> vertices = mesh.FrameProcessedVertices;
            List<Triangle> newTriangles = new List<Triangle>(4);



            int outCount = CheckNear(inTriangle, mesh);
            if(outCount == 0)
            {
                newTriangles.Add(inTriangle);
                return newTriangles;
            }
            if (outCount == 3)
            {
                return newTriangles;
            }

            if (outCount == 1)
            {
                int vOutI = OUTS[0];
                int vIn0I = INS[0];
                int vIn1I = INS[1];


                PlaneLineIntersectionManifest m1;
                bool p1 = NEMathHelper.FindPlaneLineIntersection(vertices[vOutI].Position, vertices[vIn0I].Position,
                    new NEVector4(0.0f, 0.0f, 0.1f), NEVector4.Forward, out m1);

                Vertex new0 = Vertex.Lerp(vertices[vOutI], vertices[vIn0I], m1.t);


                bool p2 = NEMathHelper.FindPlaneLineIntersection(vertices[vOutI].Position, vertices[vIn1I].Position,
                      new NEVector4(0.0f, 0.0f, 0.1f), NEVector4.Forward, out m1);

                Vertex new1 = Vertex.Lerp(vertices[vOutI], vertices[vIn1I], m1.t);

                vertices.Add(new0);
                vertices.Add(new1);
                int v0 = vertices.Count - 2;
                int v1 = vertices.Count - 1;



                VertsToTris(mesh, inTriangle, v0, v1, vIn0I, vIn1I, newTriangles);

                return newTriangles;

            }

            if(outCount == 2)
            {
                int vOut0I = OUTS[0];
                int vOut1I = OUTS[1];
                int vInI = INS[0];

                PlaneLineIntersectionManifest m1;
                NEMathHelper.FindPlaneLineIntersection(vertices[vOut0I].Position, vertices[vInI].Position,
                    new NEVector4(0.0f, 0.0f, 0.1f), NEVector4.Forward, out m1);

                Vertex new0 = Vertex.Lerp(vertices[vOut0I], vertices[vInI], m1.t);


                NEMathHelper.FindPlaneLineIntersection(vertices[vOut1I].Position, vertices[vInI].Position,
                    new NEVector4(0.0f, 0.0f, 0.1f), NEVector4.Forward, out m1);

                Vertex new1 = Vertex.Lerp(vertices[vOut1I], vertices[vInI], m1.t);
                vertices.Add(new0);
                vertices.Add(new1);
                int v0 = vertices.Count - 2;
                int v1 = vertices.Count - 1;
                VertsToTris(mesh, inTriangle, v0, v1, vInI, newTriangles);

                return newTriangles;
            }

            return newTriangles;
        }


        int[] INS = new int[3];
        int[] OUTS = new int[3];
        int CheckNear(Triangle triangle, Mesh mesh)
        {
            int inI = 0;
            int outI = 0;
            int iA = triangle.Indices[0];
            int iB = triangle.Indices[1];
            int iC = triangle.Indices[2];
            Vertex A = mesh.FrameProcessedVertices[iA];
            Vertex B = mesh.FrameProcessedVertices[iB];
            Vertex C = mesh.FrameProcessedVertices[iC];

            if (A.Position.Z < 0.1f)
            {
                OUTS[outI] = triangle.Indices[0];
                outI++;

            }
            else
            {
               
                INS[inI] = triangle.Indices[0];
                inI++;
            }

            if (B.Position.Z < 0.1f)
            {
                OUTS[outI] = triangle.Indices[1];
                outI++;
                
            }
            else
            {
                INS[inI] = triangle.Indices[1];
                inI++;
                
            }

            if (C.Position.Z < 0.1f)
            {
                OUTS[outI] = triangle.Indices[2];
                outI++;
                
            }
            else
            {
                INS[inI] = triangle.Indices[2];
                inI++;
                
            }
            return outI;
        }

        Intersection FindIntersections(Triangle inTriangle, out Vertex AB, out Vertex AC, out Vertex BC,  NEVector4 p, NEVector4 d)
        {
            Intersection ret = new Intersection();
            AB = null; AC = null; BC = null;
            PlaneLineIntersectionManifest mAC;
            if (NEMathHelper.FindPlaneLineIntersection(inTriangle.A.Position, inTriangle.C.Position, p, d, out mAC))
            {
                ret.AC = true;
                AC = Vertex.Lerp(inTriangle.A, inTriangle.C, mAC.t);
            }

            PlaneLineIntersectionManifest mBC;
            if (NEMathHelper.FindPlaneLineIntersection(inTriangle.B.Position, inTriangle.C.Position, p, d, out mBC))
            {
                BC = Vertex.Lerp(inTriangle.B, inTriangle.C, mBC.t);
                ret.BC = true;
            }

            PlaneLineIntersectionManifest mAB;
            if (NEMathHelper.FindPlaneLineIntersection(inTriangle.A.Position, inTriangle.B.Position, p, d, out mAB))
            {
                AB = Vertex.Lerp(inTriangle.A, inTriangle.B, mAB.t);
                ret.AB = true;
            }
            return ret;
            
        }

       

        bool IsOutsideFrustum(Triangle triangle)
        {
            float depthA = triangle.A.ZInViewSpace * m_Camera.InverseFar;
            float depthB = triangle.B.ZInViewSpace * m_Camera.InverseFar;
            float depthC = triangle.C.ZInViewSpace * m_Camera.InverseFar;

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
           // m_Models[0].Transform.RotateY(deltaTime*0.5f);
            //m_Models[1].Transform.PositionY = -0.7f + (float)(Math.Sin(Engine.Instance.TotalTime) * 0.3);
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
            for (int i = 0; i < m_VBO.TempTriangles.Count; ++i)
            {
                Triangle tr = m_VBO.TempTriangles[i];
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

                    float depthBottom = (1.0f - manifest.bottom_t) * manifest.bottom_P0.ZInViewSpace + manifest.bottom_t * manifest.bottom_P1.ZInViewSpace;
                    float depthTop = (1.0f - manifest.top_t) * manifest.top_P0.ZInViewSpace + manifest.top_t * manifest.top_P1.ZInViewSpace;
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

                        var col = NEColorSample.MakeCol5(ConsoleColor.Black, (ConsoleColor)tr.ColorAttrib, 1.0f*luma);
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
            float size = 0.55f;
            //m_VBO.AddVertex(new Vertex(-size+x,-size+y, depth,0.0f,1.0f));
            //m_VBO.AddVertex(new Vertex(-size+x, size+ y, depth, 0.0f, 0.0f));
            //m_VBO.AddVertex(new Vertex(size + x, size + y, depth,1.0f,0.0f));
            //m_VBO.AddVertex(new Vertex(size + x, -size +y, depth,1.0f,1.0f));
            mesh.AddVertex(new Vertex(-size * 1 + x, -size * 1+ y, z, 0.0f, 0.0f));
            mesh.AddVertex(new Vertex(-size * 1 + x, size * 1 + y, z, 0.0f, 1.0f));
            mesh.AddVertex(new Vertex(size * 1 + x, size * 1 + y, z, 1.0f, 1.0f));
            mesh.AddVertex(new Vertex(size * 1+ x, -size * 1 + y, z, 1.0f, 0.0f));

            mesh.AddTriangle(0, 1, 2);
            mesh.AddTriangle(0, 2, 3);

            mesh.ModelTriangles[0].ColorAttrib = 10;
            mesh.ModelTriangles[1].ColorAttrib = 10;
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

            mesh.ModelTriangles[0].ColorAttrib = 12;
            mesh.ModelTriangles[1].ColorAttrib = 12;
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

            mesh.ModelTriangles[0].ColorAttrib = col;
            mesh.ModelTriangles[1].ColorAttrib = col;


            mesh.AddTriangle(4, 6, 5);
            mesh.AddTriangle(4, 7, 6);

            mesh.ModelTriangles[2].ColorAttrib = col;
            mesh.ModelTriangles[3].ColorAttrib = col;


            mesh.AddTriangle(4, 5, 1);
            mesh.AddTriangle(4, 1, 0);

            mesh.ModelTriangles[4].ColorAttrib = col;
            mesh.ModelTriangles[5].ColorAttrib = col;


            mesh.AddTriangle(3, 2, 6);
            mesh.AddTriangle(3, 6, 7);

            mesh.ModelTriangles[6].ColorAttrib = col;
            mesh.ModelTriangles[7].ColorAttrib = col;



            mesh.AddTriangle(1, 5, 6);
            mesh.AddTriangle(1, 6, 2);

            mesh.ModelTriangles[8].ColorAttrib = col;
            mesh.ModelTriangles[9].ColorAttrib = col;


            mesh.AddTriangle(0, 3, 7);
            mesh.AddTriangle(0, 7, 4);

            mesh.ModelTriangles[10].ColorAttrib = col;
            mesh.ModelTriangles[11].ColorAttrib = col;
            return mesh;
        }

    }
}
