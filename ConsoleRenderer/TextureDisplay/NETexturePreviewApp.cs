using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.TextureDisplay
{
    public class NETexturePreviewApp: NEScene
    {
        NEColorTexture16 m_WallTex;
        NEColorPalette pal;
       bool sampled;
        public override bool OnLoad()
        {
            ScreenWidth = 320;
            ScreenHeight = 200;
            PixelWidth = 4;
            PixelHeight = 4;
            //ParallelScreenDraw = true;
            m_WallTex = NEColorTexture16.LoadFromFile(@"C:\test\nowa_textura\color.tex");
            sampled = false;
            if (m_WallTex == null) return false;

            pal = NEColorPalette.FromFile(@"C:\test\nowa_textura\palette.txt");
           
            return true;
        }

        public override void OnStart()
        {
            base.OnStart();
            NEColorManagement.SetPalette(pal);

        }
        override public void OnUpdate(float dt)
        {

        }

        public override void OnDraw()
        {
            if (sampled) return;
            for (int x = 0; x < ScreenWidth; ++x)
            {
                float u = ((float)x) / ((float)ScreenWidth);
                for (int y = 0; y < ScreenHeight; ++y)
                {
                    float v = ((float)y) / ((float)ScreenHeight);
                    NEColorSample sample = m_WallTex.Sample(u, v, 0.99f);
                    NEScreenBuffer.PutChar(sample.Character, sample.BitMask, x, y);
                }
            }
            sampled = true;
        }
    }
}
