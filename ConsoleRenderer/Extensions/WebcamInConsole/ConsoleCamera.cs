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

        public override bool OnLoad()
        {
            ScreenWidth = 350;
            ScreenHeight = 230;
            //ScreenWidth = 150;
            //ScreenHeight = 80;
            PixelWidth = 4;
            PixelHeight = 4;
            m_CamCapture = new CameraCapture();

            m_CamCapture.InitCamera();

            m_CamCapture.Start();
           
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

                    float r = ((float)frame[m_CamCapture.Coords2Index(camX, camY) + 2]) / 255.0f;
                    float g = ((float)frame[m_CamCapture.Coords2Index(camX, camY) + 1]) / 255.0f;
                    float b = ((float)frame[m_CamCapture.Coords2Index(camX, camY)]) / 255.0f;


                    float val = (r + g + b) * 0.33f;
                    val *= val;
                    NEColorSample sample = NEColorSample.MakeCol((ConsoleColor)0, (ConsoleColor)15, val, NECHAR_RAMPS.CHAR_RAMP_FULL);
                    NEScreenBuffer.PutChar(sample.Character, sample.BitMask, x, y);

                }
            }
            NEDebug.DrawPalette(ScreenWidth, ScreenHeight);
            return base.OnDraw();
        }
    }
}
