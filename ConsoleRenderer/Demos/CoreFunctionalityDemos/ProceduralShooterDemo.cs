using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;
namespace NostalgiaEngine.Demos
{
    class ProceduralShooterDemo: NEScene
    {

        NEFloatBuffer m_ShipTexture;
        NEFloatBuffer m_ProjectileTexture;

        NEStaticSpriteLuma m_ShipSprite;
        List<NEStaticSpriteLuma> m_ProjectileSprites = new List<NEStaticSpriteLuma>();
        public override bool OnLoad()
        {
            ScreenWidth = 320;
            ScreenHeight = 200;
            PixelWidth = 4;
            PixelHeight = 4;
            m_ShipTexture = NEFloatBuffer.FromFile("CoreDemosResources/shipTexture.buf");
            m_ProjectileTexture = NEFloatBuffer.FromFile("CoreDemosResources/projectileTexture.buf");
            NEColorManagement.SetDefaultPalette();
            return (base.OnLoad() && m_ShipTexture!=null && m_ProjectileTexture != null);
        }

        public override void OnInitializeSuccess()
        {
            m_ProjectileSprites = new List<NEStaticSpriteLuma>();
            m_ShipSprite = new NEStaticSpriteLuma(m_ShipTexture);
            base.OnInitializeSuccess();
        }


        public override void OnUpdate(float deltaTime)
        {
            List<NEStaticSpriteLuma> toRemove = new List<NEStaticSpriteLuma>();
            foreach (NEStaticSpriteLuma p in m_ProjectileSprites)
            {
                p.X += 0.05f;
                if (p.X > 2.0f)
                {
                    toRemove.Add(p);
                }
            }
            //Yes, there are more efficient ways to deal with inactive projectiles, like object pooling, etc
            //but this is sufficient for this demo
            foreach (NEStaticSpriteLuma p in toRemove)
            {
                m_ProjectileSprites.Remove(p);
            }

            m_ShipSprite.X = 0.3f + NEMathHelper.Sin(Engine.Instance.TotalTime) * 0.1f;
            m_ShipSprite.Y = 0.4f + NEMathHelper.Sin(Engine.Instance.TotalTime * 2.0f) * 0.3f;

            if (NEInput.CheckKeyPress(ConsoleKey.Spacebar))
            {
                NEStaticSpriteLuma proj = new NEStaticSpriteLuma(m_ProjectileTexture);
                proj.Position = new NEVector2(m_ShipSprite.X + 0.2f, m_ShipSprite.Y + 0.09f);
                m_ProjectileSprites.Add(proj);
            }
            base.OnUpdate(deltaTime);
        }

        // parallel draw 
        public override void OnDrawPerColumn(int x)
        {
            float t = Engine.Instance.TotalTime;
            float verticalMovement = NEMathHelper.Sin(t * 0.4f) * 0.1f;
            float xNorm = (x / ((float)ScreenWidth));
            float terrain1H = Terrain(xNorm * 0.65f + t * 0.2f, 0.8f + verticalMovement);
            float terrain2H = Terrain(xNorm + t * 0.05f, 0.5f + verticalMovement);

            for (int y = 0; y < ScreenHeight; ++y)
            {
                float yNorm = y / ((float)ScreenHeight);
                float interp = 0.0f;
                ConsoleColor c1 = ConsoleColor.DarkBlue;
                ConsoleColor c2 = ConsoleColor.Red;
                if (yNorm <= terrain2H)
                {
                    float skyWave = (NEMathHelper.Sin(xNorm * 10.0f) + 1.0f) * 0.01f;
                    interp = yNorm * yNorm + skyWave;
                }
                else if (yNorm > terrain2H && yNorm <= terrain1H)
                {
                    interp = 0.75f;
                    c2 = ConsoleColor.Black;
                }
                if (yNorm > terrain1H)
                {
                    interp = 0.8f;
                    c2 = ConsoleColor.Black;
                }
                NEColorSample col = NEColorSample.MakeColFromBlocks10(c1, c2, interp);
                NEScreenBuffer.PutChar(col.Character, col.BitMask, x, y);

                float shipSize = 0.2f;

                if (xNorm > m_ShipSprite.X && xNorm < (m_ShipSprite.X + shipSize) && yNorm > m_ShipSprite.Y && yNorm < (m_ShipSprite.Y + shipSize))
                {
                    float u = (xNorm - m_ShipSprite.X) / shipSize;
                    float v = (yNorm - m_ShipSprite.Y) / shipSize;
                    interp = m_ShipSprite.Texture.Sample(u, v);
                    if (interp < 0.99f)//white is transparent
                    {
                        col = NEColorSample.MakeCol(0, (ConsoleColor)15, interp, NECHAR_RAMPS.CHAR_RAMP_FULL);
                        NEScreenBuffer.PutChar(col.Character, col.BitMask, x, y);
                    }
                }


                foreach (NEStaticSpriteLuma p in m_ProjectileSprites)
                {
                    float projectileSize = 0.03f;

                    if (xNorm > p.X && xNorm < (p.X + projectileSize) && yNorm > p.Y && yNorm < (p.Y + projectileSize))
                    {
                        float u = (xNorm - p.X) / projectileSize;
                        float v = (yNorm - p.Y) / projectileSize;
                        interp = p.Texture.Sample(u, v);
                        if (interp > 0.001f)//black is transparent
                        {
                            col = NEColorSample.MakeCol((ConsoleColor)12, (ConsoleColor)14, interp, NECHAR_RAMPS.CHAR_RAMP_FULL);
                            NEScreenBuffer.PutChar(col.Character, col.BitMask, x, y);
                        }
                    }
                }
            }
                base.OnDrawPerColumn(x);
        }

        public override bool OnDraw()
        {
            return base.OnDraw();
        }


        public float Terrain(float x, float h)
        {
            float fx = (NEMathHelper.Sin(x * 16.0f) + 1.0f) * 0.3f + h;
            fx += NEMathHelper.Sin(x * 3.0f) * 0.2f;
            fx += NEMathHelper.Sin(x * 25.0f) * 0.1f;
            fx += NEMathHelper.Sin(x * 52.0f) * 0.1f;
            return fx * 0.6f;
        }
    }
}
