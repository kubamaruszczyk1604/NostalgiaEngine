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

        
        private NEDepthBuffer m_DepthBuffer;
        private List<NELight> m_Lights;

        private int m_RenderedTriangleCount = 0;

        private bool m_DrawPaletteFlag;

        private float m_HeadlampIntensity;

        private float m_ScrHeightReciprocal;
        private float m_ScrWidthReciprocal;


        protected Skybox SceneSkybox { get; set; }
        protected Camera MainCamera { get;  set; }
        protected List<Model> Models;

        protected bool HeadlampOn { get; set; }
        protected bool TexturingOn { get; set; }
        protected float HeadlampIntensity
        {
            get { return m_HeadlampIntensity; }
            set { m_HeadlampIntensity = NEMathHelper.Clamp(value, 0.0f, 1.0f); }
        }


        protected NEVector4 GlobalLightDirection { get; set; }

        
        public Scene3D():base()
        {
            ScreenWidth = 320;
            ScreenHeight = 200;
            PixelWidth = 4;
            PixelHeight = 4;

            m_ScrHeightReciprocal = 1.0f / ScreenHeight;
            m_ScrWidthReciprocal = 1.0f / ScreenWidth;

            Models = new List<Model>();
            SceneSkybox = new Skybox();
            MainCamera = new Camera(ScreenWidth, ScreenHeight, 1.05f, 0.1f, 100.0f);
            MainCamera.Transform.LocalPosition = new NEVector4(0.0f, 0.0f, -2.0f);
            m_DrawPaletteFlag = false;
            m_HeadlampIntensity = 1.0f;
            TexturingOn = true;
            m_Lights = new List<NELight>();
        }

        public override bool OnLoad()
        {
            m_DepthBuffer = new NEDepthBuffer(ScreenWidth, ScreenHeight);
            m_ScrHeightReciprocal = 1.0f / ScreenHeight;
            m_ScrWidthReciprocal = 1.0f / ScreenWidth;
            return base.OnLoad();
        }



        public override void OnInitializeSuccess()
        {
            base.OnInitializeSuccess();
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
            if (SceneSkybox == null) SceneSkybox = new Skybox();
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
            if (SceneSkybox.Available) RenderSkybox(x, u);
            base.OnDrawPerColumn(x);
        }


        bool test = true;
        public override bool OnDraw()
        {

            if (m_DrawPaletteFlag)
            {

                NEDebug.DrawPalette(ScreenWidth, ScreenHeight);
            }
            test = !test;
            return base.OnDraw();
        }

        public override void OnExit()
        {
            base.OnExit();
        }


        protected virtual NEColorSample OnSkyboxSample(NEVector4 direction, float sampledValue)
        {

            return  NEColorSample.MakeCol((ConsoleColor)0, (ConsoleColor)6, sampledValue, NECHAR_RAMPS.CHAR_RAMP_FULL);
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

        protected void ToggleHeadlamp()
        {
            HeadlampOn = !HeadlampOn;
        }

        protected void ToggleTexturing()
        {
            TexturingOn = !TexturingOn;
        }

        protected void AddLight(NELight light)
        {
            m_Lights.Add(light);
        }

        protected void RemoveLigtht(NELight light)
        {
            if(m_Lights.Contains(light))
            {
                m_Lights.Remove(light);
            }
        }

        protected void RemoveLight(string name)
        {
            List<NELight> toRem = m_Lights.FindAll(x => x.Name == name);
            if (toRem == null) return;
            for(int i = 0; i < toRem.Count; ++i)
            {
                m_Lights.Remove(toRem[i]);
            }
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
                  
                    var col = OnSkyboxSample(sampleDir, luma); 
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
            
            VertexBuffer m_VBO = model.VBO;
            for (int i = 0; i < m_VBO.TrianglesReadyToRender.Count; ++i)
            {
                Triangle tr = m_VBO.TrianglesReadyToRender[i];
                if (!tr.IsColScanlineInTriangle(u)) continue;

                float directionalLightsSum = 0;// 
                for (int l = 0; l< m_Lights.Count; ++l)
                {
                    if (m_Lights[l].LightType != NELightType.Directional) continue;
                    float intensity = NEVector4.Dot3(tr.NormalWorld, ((DirectionalLight)m_Lights[l]).Direction);
                    directionalLightsSum += NEMathHelper.Clamp(intensity, 0.0f, 1.0f);
                }

                directionalLightsSum = NEMathHelper.Clamp(directionalLightsSum, 0.0f, 1.0f);
               

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

                //float coeff = distanceClamped / distance;

                //float tOffset = 0.0f;
                //if (y0 < 0.0f)
                //{
                //    tOffset = -y0 / distance;
                //}
                int fillStart = (int)(y0clamped * ScreenHeight);
                int fillEnd = (int)(y1clamped * ScreenHeight);

                float span = fillEnd - fillStart;
                float spanReciprocal = 1.0f / span;

                for (int y = 0; y < span; ++y)
                {

                    float t = ((float)y * spanReciprocal );// * coeff + tOffset;

                    float depthBottom = (1.0f - manifest.bottom_t) * manifest.bottom_P0.Z + manifest.bottom_t * manifest.bottom_P1.Z;
                    float depthTop = (1.0f - manifest.top_t) * manifest.top_P0.Z + manifest.top_t * manifest.top_P1.Z;
                    float fragmentDepth = (1.0f - t) * depthTop + t * depthBottom;

                    //if (fragmentDepth > 1.0f || fragmentDepth <= 0)
                    //{
                    //    continue;
                    //}

                    if (m_DepthBuffer.TryUpdate(x, fillStart + y, fragmentDepth))
                    {


                        // float dot = NEVector4.Dot3(tr.TransformedNormal, vDir);
                        float v = (float)(fillStart + y) *m_ScrHeightReciprocal;
                        v = -((2.0f * v) - 1.0f);


                        //float dotHeadlamp = NEVector4.Dot3(tr.NormalView, new NEVector4(u, v, -1.0f).Normalized);
                        float dotHeadlamp = 0;
                        if (HeadlampOn)
                        {
                            NEVector4 vDirBottom = manifest.bottom_P0.Vert2Camera * (1.0f - manifest.bottom_t) + manifest.bottom_P1.Vert2Camera * manifest.bottom_t;
                            NEVector4 vDirTop = manifest.top_P0.Vert2Camera * (1.0f - manifest.top_t) + manifest.top_P1.Vert2Camera * manifest.top_t;
                            NEVector4 vDir = vDirTop * (1.0f - t) + vDirBottom * t;
                            dotHeadlamp = NEMathHelper.Clamp(NEVector4.Dot3(tr.NormalView, vDir),0,1)*m_HeadlampIntensity;
                            float coneMask = NEMathHelper.Clamp(NEVector4.Dot3(vDir, new NEVector4(u*MainCamera.InverseAspectRatio, v, -1.0f).Normalized), 0.0f, 1.0f);
                            dotHeadlamp *= coneMask;
                        }

                        float fragWBottom = (1.0f - manifest.bottom_t) * manifest.bottom_P0.W + manifest.bottom_t * manifest.bottom_P1.W;
                        float fragWTop = (1.0f - manifest.top_t) * manifest.top_P0.W + manifest.top_t * manifest.top_P1.W;
                        float fragW = (1.0f - t) * fragWTop + t * fragWBottom;





                        float global =  directionalLightsSum;
                        float headlamp = NEMathHelper.Clamp(49.0f * fragW * fragW, 0.0f, 1.0f) * dotHeadlamp;
                        float diffuse = NEMathHelper.Clamp(global + headlamp, 0.0f, 1.0f);
                        if (model.LumaTexture != null && TexturingOn)
                        {
                            NEVector2 textCoordBottom = manifest.bottom_P0.UV * (1.0f - manifest.bottom_t)
                            + manifest.bottom_P1.UV * manifest.bottom_t;

                            NEVector2 textCoordTop = manifest.top_P0.UV * (1.0f - manifest.top_t)
                                                        + manifest.top_P1.UV * manifest.top_t;

                            NEVector2 texCoord = textCoordTop * (1.0f - t) + textCoordBottom * t;
                            float teX = texCoord.X / fragW;
                            float teY = texCoord.Y / fragW;
                            float sampleCol = model.LumaTexture.FastSample(teX, 1.0f - teY);
                            diffuse *= sampleCol;

                        }

                        int fullCol = (model.Color == -1) ? tr.ColorAttrib : model.Color;
                        int lowCol =  model.UnlitColor;
                         var col = NEColorSample.MakeCol((ConsoleColor)lowCol, (ConsoleColor)fullCol, diffuse, NECHAR_RAMPS.CHAR_RAMP_FULL);
                        //var col = NEColorSample.MakeCol10((ConsoleColor)0, (ConsoleColor)14, 1.0f-NEMathHelper.Pow(fragmentDepth,20));
                        //var col = m_Texture.Sample(teX, 1.0f - teY, dot);
                        NEScreenBuffer.PutChar(col.Character, col.BitMask, x, fillStart + y);
                    }

                }
            }

        }



    }
}
