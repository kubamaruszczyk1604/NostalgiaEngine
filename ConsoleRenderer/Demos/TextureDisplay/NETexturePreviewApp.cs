﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace TextureDisplay
{
    public class NETexturePreviewApp: NEScene
    {
        NEColorTexture16 m_MainTex;
        NEColorPalette m_MainTexPal;
        NEFBuffer m_LumaBuffer;
        float m_Col;
        public override bool OnLoad()
        {
            ScreenWidth = 220;
            ScreenHeight = 180;
            PixelWidth = 5;
            PixelHeight = 5;

            //ScreenWidth = 120;
            //ScreenHeight = 50;
            //PixelWidth = 8;
            //PixelHeight = 12;
            m_Col = 0;
           // ParallelScreenDraw = true;
            m_MainTex = NEColorTexture16.LoadFromFile(@"C:\test\nowa_textura7\color.tex");
            if (m_MainTex == null) return false;

            m_MainTexPal = NEColorPalette.FromFile(@"C:\test\nowa_textura7\palette.txt");
            if (m_MainTexPal == null) return false;

           //m_LumaBuffer = NEFBuffer.FromFile(@"C:\test\NE_Texture4\luma.buf");

            return true;
        }

        public override void OnStart()
        {
            base.OnStart();
            NEColorManagement.SetPalette(m_MainTexPal);

        }
        override public void OnUpdate(float dt)
        {
            if (NEInput.CheckKeyPress(ConsoleKey.Escape))
            {
                Exit();
            }
            if(NEInput.CheckKeyPress(NEKey.RightArrow))
            {
                m_Col = 0;
            }
            // m_Col += dt;
            m_Col = m_Col >= 1.0f ? 1.0f : m_Col + dt;
            
        }


        public override void OnDraw()
        {
            //if (sampled) return;
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
                    if(m_LumaBuffer != null)
                    {
                        luma = m_LumaBuffer.Sample(du, v);
                    }
                    NEColorSample sample = m_MainTex.Sample(du, v, luma);
                   // NEColorSample sample = NEColorSample.MakeCol5(ConsoleColor.Black, ConsoleColor.Gray, luma);
                    NEScreenBuffer.PutChar(sample.Character, sample.BitMask, x, y);
                }
            }
           // sampled = true;
        }
    }
}
