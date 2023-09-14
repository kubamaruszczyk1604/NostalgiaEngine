using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

using System.Collections.Concurrent;

namespace NostalgiaEngine.Demos
{
    class Oscilloscope : NEScene
    {
        private static readonly Object obj = new Object();

        struct SignalData
        {
            public float X;
            public float Y;

            public void Set(float x, float y)
            {
                X = x;
                Y = y;
            }
        }

        ConcurrentQueue<SignalData> m_Data;

        public float SignalX { get; set; }
        public float SignalY { get; set; }

        public float wInv;
        public float hInv;
        public override bool OnLoad()
        {
            ScreenWidth = 100;
            ScreenHeight = 100;
            PixelWidth = 8;
            PixelHeight = 8;

            wInv = 1.0f / ScreenWidth;
            hInv = 1.0f / ScreenHeight;

            SignalX = 1.0f;
            SignalY = 1.0f;

            m_Data = new ConcurrentQueue<SignalData>();

            NEColorPalette pal = new NEColorPalette(NEColorPalette.NostalgiaPalette);
            pal.SetColor(10, new NEConsoleColorDef(64, 255, 168));
            pal.SetColor(0, new NEConsoleColorDef(0, 25, 22));
            pal.SetColor(2, new NEConsoleColorDef(0, 0, 0));
            NEColorManagement.SetPalette(pal);
            return base.OnLoad();
        }
   

        private float LetterOCap(float x, float y, float t)
        {
            for (int i = 0; i < 50; ++i)
            {
                t += 0.01f * i;
                SignalX = x + NEMath.Cos(t * 10.0f) * 0.1f;
                SignalY = y + NEMath.Sin(t * 10.0f) * 0.15f;
                SignalData d = new SignalData();
                d.Set(SignalX, SignalY);
                m_Data.Enqueue(d);
            }
            return t;
        }

        private float LetterS(float x, float y, float t)
        {

            for (int i = 0; i < 20; ++i)
            {
                t += 0.01f * i;
                SignalX = x - NEMath.Cos(t * 20.0f) * 0.05f;
                SignalY = 0.02f + y + NEMath.Sin(t * 10.0f) * 0.05f;
                SignalData d = new SignalData();
                d.Set(SignalX, SignalY);
                m_Data.Enqueue(d);
            }
            for (int i = 0; i < 20; ++i)
            {
                t += 0.01f * i;
                SignalX = x + NEMath.Cos(t * 20.0f) * 0.05f;
                SignalY = -0.04f + y + NEMath.Sin(t * 10.0f) * 0.05f;
                SignalData d = new SignalData();
                d.Set(SignalX, SignalY);
                m_Data.Enqueue(d);
            }
            return t;
        }

        private float LetterC(float x, float y, float t)
        {
            for (int i = 0; i < 20; ++i)
            {
                t += 0.01f * i;
                SignalX = x - NEMath.Cos(t * 20.0f) * 0.05f;
                SignalY = y + NEMath.Sin(t * 10.0f) * 0.1f;
                SignalData d = new SignalData();
                d.Set(SignalX, SignalY);
                m_Data.Enqueue(d);
            }
            return t;
        }

        private float LetterI(float x, float y, float t)
        {
            for (int i = 0; i < 10; ++i)
            {
                t += 0.01f * i;
                SignalX = x;
                SignalY = y + NEMath.Sin(t * 10.0f) * 0.1f;
                SignalData d = new SignalData();
                d.Set(SignalX, SignalY);
                m_Data.Enqueue(d);
            }
            return t;
        }

        private float LetterL(float x, float y, float t)
        {
            for (int i = 0; i < 10; ++i)
            {
                t += 0.01f * i;
                SignalX = x - NEMath.Cos(t * 20.0f) * 0.03f;
                SignalY = y - 0.1f;
                SignalData d = new SignalData();
                d.Set(SignalX, SignalY);
                m_Data.Enqueue(d);
            }
            for (int i = 0; i < 10; ++i)
            {
                t += 0.01f * i;
                SignalX = -0.04f + x;
                SignalY = y + NEMath.Sin(t * 10.0f) * 0.1f;
                SignalData d = new SignalData();
                d.Set(SignalX, SignalY);
                m_Data.Enqueue(d);
            }
            return t;
        }

        private float LetterO(float x, float y, float t)
        {
            for (int i = 0; i < 50; ++i)
            {
                t += 0.01f * i;
                SignalX = x + NEMath.Cos(t * 10.0f) * 0.05f;
                SignalY = y + NEMath.Sin(t * 10.0f) * 0.1f;
                SignalData d = new SignalData();
                d.Set(SignalX, SignalY);
                m_Data.Enqueue(d);
            }
            return t;
        }

        private float LetterP(float x, float y, float t)
        {
            for (int i = 0; i < 10; ++i)
            {
                t += 0.01f * i;
                SignalX = x;
                SignalY = -0.07f+ y + NEMath.Sin(t * 10.0f) * 0.15f;
                SignalData d = new SignalData();
                d.Set(SignalX, SignalY);
                m_Data.Enqueue(d);
            }
            for (int i = 0; i < 20; ++i)
            {
                t += 0.01f * i;
                SignalX = x + 0.05f + NEMath.Cos(t * 20.0f) * 0.05f;
                SignalY = y + NEMath.Sin(t * 10.0f) * 0.1f;
                SignalData d = new SignalData();
                d.Set(SignalX, SignalY);
                m_Data.Enqueue(d);
            }
            return t;
        }

        private float LetterE(float x, float y, float t)
        {
            for (int i = 0; i < 20; ++i)
            {
                t += 0.01f * i;
                SignalX = x + NEMath.Cos(t * 20.0f) * 0.05f;
                SignalY = y + NEMath.Sin(t * 5.0f) * 0.1f;
                SignalData d = new SignalData();
                d.Set(SignalX, SignalY);
                m_Data.Enqueue(d);
            }
            return t;
        }

        float t0 = 0.0f;
        public override void OnUpdate(float deltaTime)
        {
            float py = 0.18f;
            float t = 0.0f;
            if (!NEInput.CheckKeyDown(ConsoleKey.Spacebar))
            {
               
                if (Engine.Instance.TotalTime > 0.2f)
                {
                    t = LetterOCap(-0.8f, py, t0 + deltaTime);
                }
                if (Engine.Instance.TotalTime > 0.4f)
                {
                    t = LetterS(-0.6f, py, t);
                }
                if (Engine.Instance.TotalTime > 0.6f)
                {
                    t = LetterC(-0.45f, py, t);
                }
                if (Engine.Instance.TotalTime > 0.8f)
                {
                    t = LetterI(-0.32f, py, t);
                }
                if (Engine.Instance.TotalTime > 1.0f)
                {
                    t = LetterL(-0.2f, py, t);
                }
                if (Engine.Instance.TotalTime > 1.2f)
                {
                    t = LetterL(-0.08f, py, t);
                }
                if (Engine.Instance.TotalTime > 1.4f)
                {
                    t = LetterL(-0.08f, py, t);
                }
                if (Engine.Instance.TotalTime > 1.6f)
                {
                    t = LetterO(0.04f, py, t);
                }

                if (Engine.Instance.TotalTime > 1.7f)
                {
                    t = LetterS(0.2f, py, t);
                }

                if (Engine.Instance.TotalTime > 1.8f)
                {
                    t = LetterC(0.34f, py, t);
                }
                if (Engine.Instance.TotalTime > 1.9f)
                {
                    t = LetterO(0.48f, py, t);
                }
                if (Engine.Instance.TotalTime > 2.1f)
                {
         
                    t = LetterP(0.6f, py, t);
                    t = LetterE(0.78f, py, t);
                }
                if (Engine.Instance.TotalTime > 2.8f)
                {

                    for (int i = 0; i < 400; ++i)
                    {
                        float tr = Engine.Instance.TotalTime + 0.001f * i;
                        SignalX = NEMath.Cos(tr * 1.0f);
                        SignalY =-0.5f+ NEMath.Sin(tr * 10f)*0.3f;
                        SignalData d = new SignalData();
                        d.Set(SignalX, SignalY);
                        m_Data.Enqueue(d);
                    }
                }
              

            }
            else
            {
                for (int i = 0; i < 100; ++i)
                {
                    SignalData d = new SignalData();
                    d.Set(0.0f, 0.0f);
                    m_Data.Enqueue(d);
                }
            }
            t0 = t;
            lock (obj)
            {
                while (m_Data.Count > 4000)
                {
                    while(!m_Data.TryDequeue(out _))
                    {

                    }
                }
            }


            NEScreenBuffer.Clear();
            for (int x = 1; x < ScreenWidth; ++x)
            {

                for (int y = 1; y < ScreenHeight; ++y)
                {
                    if (x % 10 == 0 || y % 10 == 0)
                    {
                        
                        NEScreenBuffer.PutChar('#', 2, x-1, y-1);
                    }
                }
            }
        }

        public override void OnDrawPerColumn(int x)
        {
            base.OnDrawPerColumn(x);

            int cnt = m_Data.Count;
            int span = cnt / ScreenWidth;
            int start = x * span;
            int stop = start + span;
            for (int i = start; i < stop; ++i)
            {
                float dv = ((float)i) / cnt;
                //dv *= dv*dv;
                SignalData data = m_Data.ElementAt(i);
                int sigX = (int)(((data.X + 1.0f) * 0.5f) * (ScreenWidth - 2));
                int sigY = (int)(((-data.Y + 1.0f) * 0.5f) * (ScreenHeight - 2));
                //NEColorSample sample = NEColorSample.MakeColFromBlocks5(0, (ConsoleColor)10, dv);
                NEColorSample sample = NEColorSample.MakeCol(0, (ConsoleColor)10, dv, NECHAR_RAMPS.CHAR_RAMP_FULL_EXT);
                NEScreenBuffer.PutChar(sample.Character, sample.BitMask, sigX, sigY);
            }
        }

        public override bool OnDraw()
        {
            return true;
        }
    }
}
