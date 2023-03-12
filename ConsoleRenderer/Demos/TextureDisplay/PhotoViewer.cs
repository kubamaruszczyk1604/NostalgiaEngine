using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;
using NostalgiaEngine.ConsoleGUI;
namespace TextureDisplay
{
    public class PhotoViewer : NEScene
    {

        class TexImage
        {
            public bool OK { get { return Color != null; } }

            public int Width { get { return Color != null ? Color.Width : 50; } }
            public int Height { get { return Color != null ? Color.Height : 50; } }

            public NEColorTexture16 Color { get; private set; }
            public NEColorPalette Palette { get; private set; }
            public NEFloatBuffer Luma { get; private set; }

            public TexImage(string path)
            {
                Color = NEColorTexture16.LoadFromFile(path + "/color.tex");
                Palette = NEColorPalette.FromFile(path + "/palette.txt");
                Luma = NEFloatBuffer.FromFile(path + "/luma.buf");
            }

            public void SetPalette()
            {
                if (Palette != null)
                {
                    NEColorManagement.SetPalette(Palette);
                }
                else
                {
                    NEColorManagement.SetDefaultPalette();
                }
            }
        }

        NEColorTexture16 m_MainTex;
        NEColorPalette m_MainTexPal;
        NEFloatBuffer m_LumaBuffer;
        private float m_RefreshIntervalCounter;
        private float m_Col;
        private bool m_IntroPhase;
        private int m_Index = 0;
        private bool m_ShowPalette;

        List<TexImage> m_Images;

        public PhotoViewer(string[] paths)
        {
            m_IntroPhase = true;

            m_Images = new List<TexImage>(paths.Length);
            for (int i = 0; i < paths.Length; ++i)
            {
                TexImage img = new TexImage(paths[i]);
                if (img.OK)
                {
                    m_Images.Add(img);
                }
            }
        }

        public override bool OnLoad()
        {
            ScreenWidth = 220;
            ScreenHeight = 180;
            PixelWidth = 5;
            PixelHeight = 5;
            m_Col = 0;
           // ParallelScreenDraw = true;

            m_MainTex = NEColorTexture16.LoadFromFile(@"ImageViewerResources\intro.dat");
            m_RefreshIntervalCounter = 0.0f;
            if (m_MainTex == null) return false;

            m_MainTexPal = NEColorPalette.FromFile(@"ImageViewerResources\intro.res");
            m_Col = 1.0f;
            if (m_MainTexPal == null) return false;

            return true;
        }

        public override void OnStart()
        {
            base.OnStart();
            NEColorManagement.SetPalette(m_MainTexPal);

        }

        override public void OnUpdate(float dt)
        {

            if (NEInput.CheckKeyPress(ConsoleKey.P))
            {
                m_ShowPalette = !m_ShowPalette;
            }
            if (m_RefreshIntervalCounter <= 2.0f)
            {
                m_RefreshIntervalCounter += dt;

            }

            if (NEInput.CheckKeyPress(ConsoleKey.Escape))
            {
                Exit();
            }
            if (NEInput.CheckKeyPress(NEKey.RightArrow))
            {
                m_Col = 0;
                SetImage(1);
            }
            if (NEInput.CheckKeyPress(NEKey.LeftArrow))
            {
                m_Col = 0;
                SetImage(-1);
            }
            //m_Col -= dt;
            m_Col = m_Col >= 1.0f ? 1.0f : m_Col + dt * 1.5f;


            if (!m_IntroPhase) return;
            m_MainTexPal.FadeIn(dt * 0.65f);
            NEColorManagement.SetPalette(m_MainTexPal);
        }


        public override bool OnDraw()
        {
           
            if (m_RefreshIntervalCounter <= 2.0f)
            {
                NEScreenBuffer.Clear();
                for (int x = 0; x < ScreenWidth; ++x)
                {
                    float u = ((float)x) / ((float)ScreenWidth);

                    for (int y = 0; y < ScreenHeight; ++y)
                    {
                        float du = u;
                        if (y % 2 == 0)
                        {
                            du = u - 1 + m_Col;
                        }
                        else
                        {
                            du = u - m_Col + 1.0f;
                        }
                        float v = ((float)y) / ((float)ScreenHeight);

                        float luma = 1.0f;
                        if (m_LumaBuffer != null)
                        {
                            luma = m_LumaBuffer.Sample(du, v);
                        }
                        NEColorSample sample = m_MainTex.Sample(du, v, luma);
                        //NEColorSample sample = NEColorSample.MakeCol5(ConsoleColor.Black, ConsoleColor.Gray, luma);
                        NEScreenBuffer.PutChar(sample.Character, sample.BitMask, x, y);
                    }
                }
            }
            if (m_ShowPalette) NEDebug.DrawPalette(ScreenWidth, ScreenHeight);
            return true;
        }


        private void SetImage(int step)
        {
            m_RefreshIntervalCounter = 0.0f;
            if (m_IntroPhase)
            {
                m_IntroPhase = false;
                step = 0;
            }
            if (m_Images.Count == 0)
            {
                NEConsoleSounds.ErrorBeep();
                return;
            }
            m_Index += step;
            m_Index %= m_Images.Count;

            if (m_Index < 0)
            {
                m_Index += m_Images.Count;
            }

            m_MainTex = m_Images[m_Index].Color;
            m_Images[m_Index].SetPalette();
            m_LumaBuffer = null;
            if (m_Images[m_Index].Luma != null)
            {
                m_LumaBuffer = m_Images[m_Index].Luma;
            }



        }

    }
}
