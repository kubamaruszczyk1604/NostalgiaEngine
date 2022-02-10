using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.Demos
{
    class BandLevelDemo : NEScene
    {
        Random m_Rng;
        float[] m_SignalBands;
        private NEFloatBuffer m_LumaBuffer;
        public BandLevelDemo()
        {
            m_SignalBands = new float[20];
            for (int i = 0; i < 20; ++i)
            {
                m_SignalBands[i] = i / 19.0f;
            }
            m_Rng = new Random();
        }

        public override bool OnLoad()
        {
            base.OnLoad();
            ScreenWidth = 161;
            ScreenHeight = 100;
            PixelWidth = 6;
            PixelHeight = 7;

            m_LumaBuffer = NEFloatBuffer.FromFile(@"textures\band\luma.buf");
            //m_LumaBuffer = NEFloatBuffer.FromFile(@"c:\test\text\luma.buf");
            if (m_LumaBuffer == null) return false;
            return true;
        }
        public override void OnStart()
        {
            //NEColorManagement.SetSpectralPalette1();
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

        float m_RandomNoise = 0;

        float tCount = 0;
        public override void OnUpdate(float deltaTime)
        {
            
            base.OnUpdate(deltaTime);
            if (NEInput.CheckKeyPress(NEKey.Escape))
            {
                Exit();
            }
            tCount += deltaTime;
            if (tCount < 1.0f / 30.0f) return; //this bit below is clocked at 30Hz for more consistent and smoother effect
            tCount = 0;

            RandomNoiseDemo(deltaTime);

        }

        private void RandomNoiseDemo(float  dt)
        {
            float time = Engine.Instance.TotalTime;

            m_RandomNoise += 0.93f * (((float)m_Rng.NextDouble() - 0.5f) - m_RandomNoise);
            for (int i = 0; i < 20; ++i)
            {
                int ri = m_Rng.Next(0, 19);
                m_SignalBands[i] +=
                    (1.0f - (((i + 1) / 19.0f)) * 0.7f + 0.09f * (NEMathHelper.Sin(m_RandomNoise * i * dt * 40.0f + i + time * 0.08f) + 1.0f) - m_SignalBands[i]) * 0.2f;


                m_SignalBands[ri] += 0.1f * (m_RandomNoise - m_SignalBands[i]);

                //m_SignalBands[i] *= (NEMathHelper.Sin(time+i)+1.0f)*0.3f + 0.3f;
            }
        }

        public override void OnDraw()
        {
            NEScreenBuffer.Clear();
            for (int x = 0; x < ScreenWidth; ++x)
            {
                float u = (float)x / (float)ScreenWidth;
                //
                int currentBand = x / 8; // from 0 - 19
                for (int y = 0; y < ScreenHeight; ++y)
                {
                    float yNorm = (float)y / (float)ScreenHeight;
                    float yNormRev = 1.0f - yNorm;

                    //if (yNormRev > 0.5f & currentBand == 1) continue;
                    float luma = m_LumaBuffer.Sample(u, yNorm);
                    if (currentBand > 19) currentBand = 19;
                    NEColorSample sa = NEColorSample.MakeCol5((ConsoleColor)0, (ConsoleColor)8, luma * 0.5f);
                    NEScreenBuffer.PutChar(sa.Character, sa.BitMask, x, y);
                    if (yNormRev > m_SignalBands[currentBand]) continue;
                    if (x % 8 == 0) continue;
                    NEColorSample cs = NEColorSample.MakeCol5((ConsoleColor)4, (ConsoleColor)12, (1.0f - NEMathHelper.Pow(yNorm, 1.987f))*(luma));
                    NEScreenBuffer.PutChar(cs.Character, cs.BitMask, x, y);
                }
            }
            base.OnDraw();
        }
    }
}
