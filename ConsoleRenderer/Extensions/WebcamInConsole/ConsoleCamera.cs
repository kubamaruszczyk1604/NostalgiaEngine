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
        float normalizeConst = 1.0f / 255.0f;

        ColorPair[] m_ColorPairs;
        public override bool OnLoad()
        {
            ScreenWidth = 250;
            ScreenHeight = 160;

            PixelWidth = 5;
            PixelHeight = 5;
            NEColorPalette palette = NEColorPalette.FromFile("C:/Users/Kuba/Desktop/palettes/hsv_pal.txt");

            m_ColorPairs = ColorPair.GenerateColorPairs(palette);
            m_CamCapture = new CameraCapture();

            m_CamCapture.InitCamera();

            m_CamCapture.Start();
            NEColorManagement.SetPalette(palette);
            return base.OnLoad();
        }

        public override void OnUpdate(float deltaTime)
        {
            if(NEInput.CheckKeyPress(ConsoleKey.Escape))
            {
                m_CamCapture.Stop();
                Exit();
            }

            base.OnUpdate(deltaTime);
        }

        public override bool OnDraw()
        {
            byte[] frame = m_CamCapture.GetNextFrame();
            for(int x = 0; x < ScreenWidth; ++x)
            {
                float u = ((float)x) / ScreenWidth;
                int camX = (int)(u * m_CamCapture.FrameWidth);
                for (int y = 0; y< ScreenHeight; ++y)
                {
                    float v = ((float)y) / ScreenHeight;

                    int camY = (int)(v * m_CamCapture.FrameHeight);

                    float r = ((float)frame[m_CamCapture.Coords2Index(camX, camY) + 2]) * normalizeConst;
                    float g = ((float)frame[m_CamCapture.Coords2Index(camX, camY) + 1]) * normalizeConst;
                    float b = ((float)frame[m_CamCapture.Coords2Index(camX, camY)]) * normalizeConst;

                    //r *= r*r*r;
                    //g *= g*g*g;
                    //b *= b*b*b;
                    ColorPair bestPair = null;
                    float bestDist = 0.0f;
                    for (int c = 0; c < m_ColorPairs.Length; ++c)
                    {
                        float dist = m_ColorPairs[c].GetDistanceToLine(r, g, b);
                        if (dist > bestDist)
                        {
                            bestDist = dist;
                            bestPair = m_ColorPairs[c];
                        }
                    }


                    float val = (r + g + b) * 0.33f;
                    val *= val;

                     val *= bestPair.CalculateLerpCoeff();
                    //NEColorSample sample = NEColorSample.MakeCol((ConsoleColor)0, (ConsoleColor)15, val, NECHAR_RAMPS.BLOCK_RAMP5);
                    NEColorSample sample = NEColorSample.MakeCol((ConsoleColor)bestPair.CI0, (ConsoleColor)bestPair.CI1, val, NECHAR_RAMPS.CHAR_RAMP_FULL_EXT);
                    NEScreenBuffer.PutChar(sample.Character, sample.BitMask, x, y);

                }
            }
            NEDebug.DrawPalette(ScreenWidth, ScreenHeight);
            return base.OnDraw();
        }
    }
}
