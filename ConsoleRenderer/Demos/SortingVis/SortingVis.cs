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
        public override bool OnLoad()
        {
            ScreenWidth = 300;
            ScreenHeight = 150;
            PixelWidth = 4;
            PixelHeight = 4;
            m_Algorithm = new BubleSortVis(100, 150);
            return base.OnLoad();
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            for (int i = 0; i < 50; ++i)
            {
                m_Algorithm.DoStep();
            }

            if(NEInput.CheckKeyPress(ConsoleKey.Spacebar))
            {
                m_Algorithm = new BubleSortVis(100, 140);
            }
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
                    //float intensity = ((float)y) / barLen;
                    NEColorSample sample = NEColorSample.MakeColFromBlocks5(0, (ConsoleColor)9, 1.0f);
                    for (int i = 0; i < barWidth; ++i)
                    {
                        NEScreenBuffer.PutChar(sample.Character, sample.BitMask, iBar * barWidth + i, ScreenHeight - y);
                    }
                }
            }
            return base.OnDraw();
        }

       
    }
}
