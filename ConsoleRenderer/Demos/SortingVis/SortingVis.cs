using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.Demos
{
    class SortingVis: NEScene
    {

        BubleSortVis m_Algorithm;
        readonly float c_TargetFrameDuration = 1.0f/90.0f;
        float m_FrameTimeAcumulator;
        public override bool OnLoad()
        {
            ScreenWidth = 50;
            ScreenHeight = 60;
            PixelWidth = 20;
            PixelHeight = 8;
            m_Algorithm = new BubleSortVis(50, 50);
            m_FrameTimeAcumulator = 0.0f;
            return base.OnLoad();
        }

        
        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            m_FrameTimeAcumulator += deltaTime;
            if (m_FrameTimeAcumulator < c_TargetFrameDuration) return;

            //m_Algorithm.DoPass();
            m_Algorithm.DoStep();
            if (NEInput.CheckKeyPress(ConsoleKey.Spacebar))
            {
                m_Algorithm = new BubleSortVis(50, 50);
            }
            m_FrameTimeAcumulator = 0.0f;
            //NESoundSynth.PlayBeep((ushort)(200+m_Algorithm.SwappedIndex0 * 10), 100);
            Engine.Instance.TitleBarAppend = " |  BUBBLE SORT:   Steps = " + m_Algorithm.StepCount.ToString() + "  |";
            NEScreenBuffer.Clear();
        }

        public override bool OnDraw()
        {
            int barCount = m_Algorithm.Data.Length;
            int barWidth = ScreenWidth / m_Algorithm.Data.Length;

            for (int iBar = 0; iBar < barCount; ++iBar)
            {
                int barLen = m_Algorithm.Data[iBar];

                for (int y = 0; y < barLen; ++y)
                {
                    float intensity = ((float)barLen) / ScreenHeight;
                    int col = ((iBar == m_Algorithm.SwappedIndex0)|| (iBar == m_Algorithm.SwappedIndex0+1))&&!m_Algorithm.Done ? 2 : 9;
                    if (col == 2 && m_Algorithm.Swapped) col = 13;
                    NEColorSample sample = NEColorSample.MakeColFromBlocks5(0, (ConsoleColor)col, 1.0f);
                    for (int i = 0; i < barWidth; ++i)
                    {
                        NEScreenBuffer.PutChar(sample.Character, sample.BitMask, iBar * barWidth + i, ScreenHeight-1 - y);
                    }
                }
            }

            NEDebug.DrawPalette(ScreenWidth, ScreenHeight);
            return base.OnDraw();
        }

       
    }
}
