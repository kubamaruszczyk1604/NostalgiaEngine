using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.Demos
{
    public class AnalogClock : NEScene
    {

        Random m_Rng;
        float[] m_SignalBands;
        private NEFloatBuffer m_LumaBuffer;
        private float m_AspectRatio = 1.0f;
        public AnalogClock()
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
            ScreenWidth = 250;
            ScreenHeight = 200;
            PixelWidth = 4;
            PixelHeight = 4;
            m_AspectRatio = (float)ScreenWidth / (float)ScreenHeight;
            m_LumaBuffer = NEFloatBuffer.FromFile("BandLevelDemoResources/band/luma.buf");
            m_LumaBuffer.SampleMode = NESampleMode.Repeat;

            if (m_LumaBuffer == null) return false;
            return true;
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

        float tCount = 0;
        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (NEInput.CheckKeyPress(NEKey.Escape))
            {
                Exit();
            }
            tCount += deltaTime;
        }



        private bool DrawArm(NEVector2 uvs, float num, float armLen, float thickness)
        {
            uvs.Y *= -1;
            float rt = num * 3.141f;
            bool isOnLine = NEMathHelper.IsOnLine(uvs, new NEVector2(0, 0), new NEVector2(NEMathHelper.Sin(rt), NEMathHelper.Cos(-rt) + 0.02f),thickness);
            if (!isOnLine) return false;
            if (NEVector2.CalculateLength(uvs) > armLen) return false;

            return true;
        }

        public override bool OnDraw()
        {
            float seconds = (DateTime.UtcNow.Second / 60.0f) * 2.0f;
            float minutes = (DateTime.UtcNow.Minute / 60.0f) * 2.0f;
            float hours = ( (DateTime.UtcNow.Hour%12 + minutes*0.5f)/ 12.0f) * 2.0f;

            float time = Engine.Instance.TotalTime;
            
            if (tCount < 1.0f / 5.0f) return true; //this bit below is clocked at 5Hz for more consistent and smoother effect
            tCount = 0;

            NEScreenBuffer.Clear();
            for (int x = 0; x < ScreenWidth; ++x)
            {
                float u = (float)x / (float)ScreenWidth;

                for (int y = 0; y < ScreenHeight; ++y)
                {
                    float yNorm = (float)y / (float)ScreenHeight;
                    float yNormRev = 1.0f - yNorm;

                    //NEVector2 uvTex = new NEVector2(u+ time*0.05f, yNorm + NEMathHelper.Sin(u*15.0f)*0.02f);
                    NEVector2 uvTex = new NEVector2(u, yNorm);
                    //uvTex.X -= 0.5f;
                    //uvTex.Y -= 0.5f;
                    //NEMathHelper.Rotate(ref uvTex, Engine.Instance.TotalTime * 0.1f);

                    //uvTex.X += 0.5f;
                    //uvTex.Y += 0.5f;
                    float luma = m_LumaBuffer.Sample(uvTex.X, uvTex.Y);
                    NEColorSample cs = NEColorSample.MakeColFromBlocks5((ConsoleColor)0, (ConsoleColor)7, (luma)*yNormRev);

                    NEVector2 uvs = new NEVector2(u, yNorm) * 2.0f;
                    uvs.X -= 1;
                    uvs.Y -= 1;
                    uvs.X *= m_AspectRatio;
                    float circleL = NEVector2.CalculateLength(uvs);
                    float low = 0.76f;
                    float high = 0.8f;
                    if (circleL > low && circleL < high)
                    {
                        float d = (circleL - low) / (high - low);
                        float brightness = NEMathHelper.Sin(d * 3.14f);
                        brightness *= brightness * brightness;
                        cs = NEColorSample.MakeColFromBlocks5((ConsoleColor)0, (ConsoleColor)10,brightness + 0.25f);
                    }


                    if (DrawArm(uvs, seconds, 0.72f, 0.007f))
                    {
                        cs = NEColorSample.MakeColFromBlocks10((ConsoleColor)0, (ConsoleColor)10,1);
                    }
                    if (DrawArm(uvs, minutes, 0.72f, 0.02f))
                    {
                        cs = NEColorSample.MakeColFromBlocks10((ConsoleColor)0, (ConsoleColor)10,1);
                    }
                    if (DrawArm(uvs, hours, 0.46f, 0.02f))
                    {
                        cs = NEColorSample.MakeColFromBlocks5((ConsoleColor)0, (ConsoleColor)10, 1);
                    }

                    NEScreenBuffer.PutChar(cs.Character, cs.BitMask, x, y);
                }
            }
           return base.OnDraw();
        }
    }
}
