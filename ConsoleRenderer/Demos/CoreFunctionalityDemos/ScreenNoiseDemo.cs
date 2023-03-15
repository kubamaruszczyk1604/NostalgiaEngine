using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.Demos
{
    class ScreenNoiseDemo: NEScene
    {
        Random m_Random;

        public override bool OnLoad()
        {
            m_Random = new Random();
            ScreenWidth = 140;
            ScreenHeight = 100;
            PixelWidth = 6;
            PixelHeight = 6;
            return base.OnLoad();
        }

        public override void OnUpdate(float deltaTime)
        {
            if(NEInput.CheckKeyPress(ConsoleKey.Escape))
            {
                Exit();
            }
            base.OnUpdate(deltaTime);
        }

        public override bool OnDraw()
        {
            for (int x = 0; x < ScreenWidth; ++x)
            {
                for (int y = 0; y < ScreenHeight; ++y)
                {
                    NEColorSample col = NEColorSample.MakeCol(0, (ConsoleColor)15, (float)m_Random.NextDouble(), NECHAR_RAMPS.CHAR_RAMP_10);
                    NEScreenBuffer.PutChar(col.Character, col.BitMask, x, y);
                }
            }
            return base.OnDraw();
        }
    }
}
