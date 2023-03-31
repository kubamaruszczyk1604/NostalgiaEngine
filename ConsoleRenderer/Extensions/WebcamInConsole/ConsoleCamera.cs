using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

using System.Drawing;
using KLMVideoCameraCapture;
namespace NostalgiaEngine.Extensions
{
    public class ConsoleCamera: NEScene
    {
        CameraCapture m_CamCapture;
        byte[] m_CurrentCameraFrame;

        float normalizeConst = 1.0f / 255.0f;
        ColorPair[] m_ColorPairs;

        int m_ScrW;
        int m_ScrH;
        int m_PixW;
        int m_PixH;

        float m_InputGamma;
        float m_OutputGamma;
        float m_Gain;

        float m_StrUpdate = 0;
        bool m_Color;

        public ConsoleCamera(int w, int h, int pixW = 4, int pixH = 4)
        {
            m_ScrW = w;
            m_ScrH = h;
            m_PixW = pixW;
            m_PixH = pixH;
            m_InputGamma = 2.0f;
            m_OutputGamma = 1.0f;
            m_Gain = 1.5f;
            m_Color = false;
        }

        public override bool OnLoad()
        {
            //ScreenWidth = 150;
            //ScreenHeight = 100;
            //PixelWidth = 8;
            //PixelHeight = 8;

            //ScreenWidth = 220;
            //ScreenHeight = 150;
            //PixelWidth = 5;
            //PixelHeight = 5;

            ScreenWidth = m_ScrW;
            ScreenHeight = m_ScrH;
            PixelWidth = m_PixW;
            PixelHeight = m_PixH;

            NEColorPalette palette = NEColorPalette.FromFile("C:/Users/Kuba/Desktop/palettes/hsvmod_pal.txt");

            m_ColorPairs = ColorPair.GenerateColorPairs(palette);
            m_CamCapture = new CameraCapture();

            m_CamCapture.InitCamera();

            m_CamCapture.Start();
            NEColorManagement.SetPalette(palette);
            return base.OnLoad();
        }

        public override void OnUpdate(float deltaTime)
        {
            m_CurrentCameraFrame = m_CamCapture.GetNextFrame();
            if (NEInput.CheckKeyPress(ConsoleKey.Escape))
            {
                Exit();
            }

            if (NEInput.CheckKeyDown(ConsoleKey.G))
            {
                if (NEInput.CheckKeyPress(ConsoleKey.UpArrow))
                {
                    m_Gain += 0.1f;
                }
                if (NEInput.CheckKeyPress(ConsoleKey.DownArrow))
                {
                    m_Gain -= 0.1f;
                    if(m_Gain < 0.1f)
                    {
                        m_Gain = 0.1f;
                    }
                    
                }
            }

            if (NEInput.CheckKeyDown(ConsoleKey.I))
            {
                if (NEInput.CheckKeyPress(ConsoleKey.UpArrow))
                {
                    m_InputGamma += 0.1f;
                }
                if (NEInput.CheckKeyPress(ConsoleKey.DownArrow))
                {
                    m_InputGamma -= 0.1f;
                    if (m_InputGamma < 0.1f)
                    {
                        m_InputGamma = 0.1f;
                    }
                }
            }

            if (NEInput.CheckKeyDown(ConsoleKey.O))
            {
                if (NEInput.CheckKeyPress(ConsoleKey.UpArrow))
                {
                    m_OutputGamma += 0.1f;
                }
                if (NEInput.CheckKeyPress(ConsoleKey.DownArrow))
                {
                    m_OutputGamma -= 0.1f;
                    if (m_OutputGamma < 0.1f)
                    {
                        m_OutputGamma = 0.1f;
                    }
                }
            }

            if (NEInput.CheckKeyPress(ConsoleKey.C))
            {
                m_Color = !m_Color;
            }

            m_StrUpdate += deltaTime;
            if(m_StrUpdate > 0.05f)
            {
                m_StrUpdate = 0;
                Engine.Instance.TitleBarAppend = "    Gain = " + m_Gain.ToString() + ", InputGamma = " + m_InputGamma.ToString()
                     + ", OutputGamma = " + m_OutputGamma.ToString();

            }
            base.OnUpdate(deltaTime);
        }



        public override bool OnDraw()
        {

            for (int x = 0; x < ScreenWidth; ++x)
            {
                float u = ((float)x) / ScreenWidth;
                int camX = (int)(u * m_CamCapture.FrameWidth);
                for (int y = 0; y < ScreenHeight; ++y)
                {
                    float v = ((float)y) / ScreenHeight;

                    int camY = (int)(v * m_CamCapture.FrameHeight);

                    float r = ((float)m_CurrentCameraFrame[m_CamCapture.Coords2Index(camX, camY) + 2]) * normalizeConst;
                    float g = ((float)m_CurrentCameraFrame[m_CamCapture.Coords2Index(camX, camY) + 1]) * normalizeConst;
                    float b = ((float)m_CurrentCameraFrame[m_CamCapture.Coords2Index(camX, camY)]) * normalizeConst;

                     r = NEMathHelper.Pow(r, m_InputGamma)*m_Gain;
                     g = NEMathHelper.Pow(g, m_InputGamma)*m_Gain;
                     b = NEMathHelper.Pow(b, m_InputGamma)*m_Gain;
                    ColorPair bestPair = null;
                    float bestDist = 10.0f;
                    for (int c = 0; c < m_ColorPairs.Length; ++c)
                    {
                        float dist = m_ColorPairs[c].GetDistanceToLine(r, g, b);
                        if (dist < bestDist)
                        {
                            bestDist = dist;
                            bestPair = m_ColorPairs[c];
                        }
                        if (dist < 0.1f) break;
                    }


                    float val = (r + g + b) * 0.33f;
                    //val *= val;

                    val *= bestPair.CalculateLerpCoeff();
                    val = NEMathHelper.Pow(val, m_OutputGamma);
                    //val *= val;
                    NEColorSample sample;
                    if (m_Color)
                    {
                        sample = NEColorSample.MakeCol((ConsoleColor)bestPair.CI0, (ConsoleColor)bestPair.CI1, val, NECHAR_RAMPS.CHAR_RAMP_FULL);
                    }
                    else
                    {
                        sample = NEColorSample.MakeCol((ConsoleColor)0, (ConsoleColor)1, val, NECHAR_RAMPS.CHAR_RAMP_FULL);
                    }
                    NEScreenBuffer.PutChar(sample.Character, sample.BitMask, x, y);

                }
            }

            //NEDebug.DrawPalette(ScreenWidth, ScreenHeight);
            return base.OnDraw();
        }

        public override void OnExit()
        {
            base.OnExit();
            m_CamCapture.Stop();
        }
    }
}
