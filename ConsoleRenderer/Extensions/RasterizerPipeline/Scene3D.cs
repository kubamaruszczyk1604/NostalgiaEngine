﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;
namespace NostalgiaEngine.RasterizerPipeline
{
    public class Scene3D : NEScene
    {

        
        NEDepthBuffer m_DepthBuffer;
        
        NEColorPalette m_Palette;


        protected Skybox SceneSkybox { get; set; }
        protected Camera MainCamera { get;  set; }
        protected List<Model> Models;



        int m_RenderedTriangleCount = 0;

        bool m_DrawPaletteFlag;
        bool m_ShowClippingFlag;

        float m_ScrHeightReciprocal;
        float m_ScrWidthReciprocal;

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
            m_DrawPaletteFlag = false;
            m_ShowClippingFlag = false;
            
        }

        public override bool OnLoad()
        {


            m_Palette = NEColorPalette.FromFile("C:/test/skybox3/px/palette.txt");
            NEColorManagement.SetPalette(m_Palette);
            m_PaletteCellWidth = (int)(ScreenWidth * 0.039f);
            m_PaletteCellHeight = m_PaletteCellWidth;
            m_PaletteStripPos = new NEVector2(m_PaletteCellWidth / 2, m_PaletteCellWidth / 2);
            //Clipping.DebugMode = true;
            m_ScrHeightReciprocal = 1.0f / ScreenHeight;
            m_ScrWidthReciprocal = 1.0f / ScreenWidth;
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
            float xNorm = ((float)x) *m_ScrWidthReciprocal;
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

            if (m_DrawPaletteFlag)
            {
                for (int y = (int)m_PaletteStripPos.Y; y < ((int)m_PaletteStripPos.Y + m_PaletteCellHeight); ++y)
                {
                    for (int x = 0; x < ScreenWidth; ++x)
                    {
                        DrawPalette(x, y);
                    }
                }
            }
            return base.OnDraw();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

       
        protected void TogglePalette()
        {
            m_DrawPaletteFlag = !m_DrawPaletteFlag;
        }

        protected void ShowPalette(bool show)
        {
            m_DrawPaletteFlag = show;
        }


        public void ToggleShowClipping()
        {
            Clipping.DebugMode = !Clipping.DebugMode;
        }

        private void RenderSkybox(int x, float u)
        {
            float rayZ = 1.0f / (float)Math.Tan(MainCamera.FovRad * 0.5f);
            for (int y = 0; y < ScreenHeight; ++y)
            {
                if (m_DepthBuffer.TryUpdate(x, y, 1.0f))
                {
                    float v = (float)y * m_ScrHeightReciprocal;
                    v = -((2.0f * v) - 1.0f);
                    NEVector4 sampleDir = (MainCamera.PointAt) * new NEVector4(u * MainCamera.InverseAspectRatio, v, rayZ, 0.0f).Normalized;
                    float luma = SceneSkybox.Sample(sampleDir);
                    int low = 0;
                    int high = 6;
                    if (luma > 0.7f) low = 6;
                    if (luma > 0.8f) high = 15;
                    var col = NEColorSample.MakeCol10((ConsoleColor)low, (ConsoleColor)high, luma);
                    NEScreenBuffer.PutChar(col.Character, col.BitMask, x, y);
                }

            }
        }

  

        private void ProcessModel(float dt, Model model)
        {

            model.Transform.CalculateWorld();
            

            NEMatrix4x4 MVP = MainCamera.Projection * MainCamera.View * model.Transform.World;


            model.VBO.PrepareForRender(MainCamera);
            m_RenderedTriangleCount += model.VBO.TrianglesReadyToRender.Count;


        }


        private void RenderModel(int x, float u, Model model)
        {
           // float len = 1.0f - NEVector4.CalculateLength(MainCamera.View * model.Transform.World * new NEVector4(0,0,0,1)) / 15.0f;
           // len = NEMathHelper.Clamp(len, 0.0f, 1.0f);

            
            VertexBuffer m_VBO = model.VBO;
            for (int i = 0; i < m_VBO.TrianglesReadyToRender.Count; ++i)
            {
                Triangle tr = m_VBO.TrianglesReadyToRender[i];
                if (!tr.IsColScanlineInTriangle(u)) continue;


                float dotGlobalLight = NEVector4.Dot3(tr.NormalWorld, new NEVector4(-1.0f, 1.0f, 1.0f).Normalized);
                dotGlobalLight = NEMathHelper.Clamp(dotGlobalLight, 0.0f, 1.0f);

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
                        //NEVector4 vDirBottom = manifest.bottom_P0.Vert2Camera * (1.0f - manifest.bottom_t) + manifest.bottom_P1.Vert2Camera * manifest.bottom_t;
                        //NEVector4 vDirTop = manifest.top_P0.Vert2Camera * (1.0f - manifest.top_t) + manifest.top_P1.Vert2Camera * manifest.top_t;
                        //NEVector4 vDir = vDirTop * (1.0f - t) + vDirBottom * t;

                        // float dot = NEVector4.Dot3(tr.TransformedNormal, vDir);
                        float v = (float)(fillStart + y) *m_ScrHeightReciprocal;
                        v = -((2.0f * v) - 1.0f);
                       

                        float dotHeadlamp = NEVector4.Dot3(tr.NormalView, new NEVector4(u, v, -1.0f).Normalized);
                        dotHeadlamp = NEMathHelper.Clamp(dotHeadlamp, 0.0f, 1.0f);



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
                        float global =  dotGlobalLight;
                        float headlamp = 3 * fragW * dotHeadlamp;
                        float diffuse = global + headlamp;
                        if (model.LumaTexture != null)
                        {
                            float sampleCol = model.LumaTexture.FastSample(teX, 1.0f - teY);
                            diffuse *= sampleCol;

                        }

                        int fullCol = model.Color == -1 ? tr.ColorAttrib : model.Color;
                        int lowCol =  model.UnlitColor;
                        //float luma = NEMathHelper.Clamp(model.GlowIntensity + ambient + diffuse, 0.0f, 1.0f);
                        var col = NEColorSample.MakeCol5((ConsoleColor)lowCol, (ConsoleColor)fullCol,diffuse);
                        //var col = m_Texture.Sample(teX, 1.0f - teY, dot);
                        NEScreenBuffer.PutChar(col.Character, col.BitMask, x, fillStart + y);
                    }

                }
            }

        }
        private int m_PaletteCellWidth = 10;
        private int m_PaletteCellHeight = 10;

        private NEVector2 m_PaletteStripPos = new NEVector2(10, 10);
        private void DrawPalette(int x, int y)
        {
        

            NEVector2 pixelPos = new NEVector2(x, y);
            for (int i = 0; i < 16; ++i)
            {
                if (NEMathHelper.InRectangle(pixelPos, new NEVector2(m_PaletteCellWidth * (i), 0) + m_PaletteStripPos, m_PaletteCellWidth, m_PaletteCellHeight))
                {
                    NEScreenBuffer.PutChar(' ', (short)((i) << 4), x, y);
                    if (i == 16) NEScreenBuffer.PutChar((char)NEBlock.Solid, (short)(8 << 4), x, y);
                }
            }
        }


    }
}
