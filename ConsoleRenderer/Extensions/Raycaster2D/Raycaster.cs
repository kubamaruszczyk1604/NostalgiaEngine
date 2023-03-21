using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;


/// <summary>
/// This is very much work in progress!
/// So far it is a naive implementation of 2D raycasting. There is plenty of ways to optimize this it (e.g. cell boundry - ray intersection).
/// Also lots of stuff is hardcoded atm. Eventually I will clean this up.
/// </summary>
namespace NostalgiaEngine.Raycaster
{
    class NERaycaster2D : NEScene
    {
        private readonly float EPSILON = 0.01f;
        private readonly float DEG_TO_RAD = 0.0174532f;
        private readonly float M_PI = 3.1415926535f;
        private readonly float DEPTH = 24.0f;
        private readonly float ROTATION_SPEED = 1.0f;
        private readonly float MOVEMENT_SPEED = 1.6f;


        private int m_MapWidth = 20;
        private int m_MapHeight = 20;
        private int[] m_Map = {
        1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,1,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,1,
        1,0,0,1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,1,
        1,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,1,
        1,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,1,0,0,0,0,1,0,0,0,0,1,0,0,0,1,0,1,
        1,0,0,1,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,1,
        1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,
        1,1,1,1,1,1,1,1,0,0,1,0,0,0,0,1,1,0,0,1,
        1,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,1,
        1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1
        };


        private float m_AspectRatio;
        private float m_Fov;
        private NEVector2 m_ViewerPos = new NEVector2(3.0f, 1.0f);
        private NEVector2 m_ViewerDir = new NEVector2(0.0f, 1.0f);
        private float m_PlayerRotation = 0.0f;
        private NEColorTexture16 m_WallTex;
        private NEDepthBuffer m_DepthBuffer;
        private NEStaticSprite m_Lamp1Sprite;
        private NEStaticSprite m_Lamp2Sprite;
        private NEFloatBuffer m_Wall;
        private NEFloatBuffer m_Sky;


        private bool m_ShowPalette;
        public override bool OnLoad()
        {
            ScreenWidth = 320;
            ScreenHeight = 200;
            PixelWidth = 4;
            PixelHeight = 4;

            m_Wall = NEFloatBuffer.FromFile("RaycasterDemoResources/nt1/luma.buf");
            if (m_Wall == null) return false;
            m_Wall.SampleMode = NESampleMode.Repeat;

            m_Sky = NEFloatBuffer.FromFile("RaycasterDemoResources/sky/luma.buf");
            if (m_Sky == null) return false;
            m_Sky.SampleMode = NESampleMode.Repeat;

            NEColorTexture16 lampTex = NEColorTexture16.LoadFromFile("RaycasterDemoResources/lantern1.tex");
            if (lampTex == null) return false;
            m_Lamp1Sprite = new NEStaticSprite(lampTex);
            m_Lamp1Sprite.X = 7.8f;
            m_Lamp1Sprite.Y = 4.0f;

            m_Lamp2Sprite = new NEStaticSprite(lampTex);
            m_Lamp2Sprite.X = 5.2f;
            m_Lamp2Sprite.Y = 4.0f;
            return true;
        }

        override public void OnInitializeSuccess()
        {
            m_AspectRatio = (float)ScreenWidth / (float)ScreenHeight;
            m_Fov = 80.0f * DEG_TO_RAD;
            m_DepthBuffer = new NEDepthBuffer(ScreenWidth, ScreenHeight);
            m_ShowPalette = false;
        }

        override public void OnUpdate(float dt)
        {

            float deltaT = dt;
            if (NEInput.CheckKeyDown(ConsoleKey.LeftArrow))
            {

                if (NEInput.CheckKeyDown(NEKey.Alt))
                {
                    m_ViewerPos -= NEVector2.FindNormal(m_ViewerDir) * MOVEMENT_SPEED * deltaT;

                }
                else
                {
                    float deltaR = -ROTATION_SPEED * deltaT;
                    m_PlayerRotation += deltaR;
                    NEVector2.RotateClockWise(ref m_ViewerDir, deltaR);
                    NEVector2.Normalize(m_ViewerDir);
                }


            }
            if (NEInput.CheckKeyDown(ConsoleKey.RightArrow))
            {

                if (NEInput.CheckKeyDown(NEKey.Alt))
                {

                    m_ViewerPos += NEVector2.FindNormal(m_ViewerDir) * MOVEMENT_SPEED * deltaT;

                }
                else
                {
                    float deltaR = ROTATION_SPEED * deltaT;
                    m_PlayerRotation += deltaR;
                    NEVector2.RotateClockWise(ref m_ViewerDir, deltaR);
                    NEVector2.Normalize(m_ViewerDir);
                }
            }



            if (NEInput.CheckKeyDown(ConsoleKey.UpArrow))
            {
                m_ViewerPos += m_ViewerDir * MOVEMENT_SPEED * deltaT;
            }
            if (NEInput.CheckKeyDown(ConsoleKey.DownArrow))
            {
                m_ViewerPos -= m_ViewerDir * MOVEMENT_SPEED * deltaT;
            }

            if (NEInput.CheckKeyPress(ConsoleKey.Escape))
            {
                Exit("some data");
            }

            if (NEInput.CheckKeyPress(ConsoleKey.N))
            {
                //Engine.Instance.PushScene(new TextureEditor.Test_MemTex16());
            }
            if(NEInput.CheckKeyPress(ConsoleKey.P))
            {
                m_ShowPalette = !m_ShowPalette;
            }
            m_DepthBuffer.Clear();

        }

        override public void OnDrawPerColumn(int x)
        {
            float rayAngle = (((float)x / (float)ScreenWidth) - 0.5f) * 0.5f;
            rayAngle *= m_Fov; // TO DO: consider aspect ratio 

            NEVector2 dir = new NEVector2(m_ViewerDir.X, m_ViewerDir.Y);
            NEVector2.RotateClockWise(ref dir,  rayAngle);



            dir =   NEVector2.Normalize(dir);

            float t = 0.0f;
            const float stp = 0.01f;
            bool hit = false;
            NEVector2 ray = new NEVector2(0, 0);
            while ((t < DEPTH) && !hit)
            {
                ray = m_ViewerPos + dir * t;
                int cell = GetCell(ray);
                t += stp;
                if (cell != 0)
                    hit = true;
            }
            t = t * NEMathHelper.Cos(rayAngle);
            float depth = (t / DEPTH);
            float ceilingStartY = (1.0f / t);
            float floorStartY = (-1.0f / t);
            float intensity = 1.0f - depth;
            //intensity *= intensity*1.5f;

            for (int y = 0; y < ScreenHeight; ++y)
            {

                float pixelY = -(y - ScreenHeight / 2);
                float py = pixelY / ((float)ScreenHeight);
                py *= m_Fov;

                NEColorSample floorSample = NEColorSample.MakeColFromBlocks5(ConsoleColor.Black, (ConsoleColor)7, 0.2f);// Math.Abs(py) -Math.Abs(px * 0.1f));

               float dd = m_Sky.Sample((((float)x)/((float)ScreenWidth) ) +m_PlayerRotation*0.4f, py);
                NEColorSample ceilSample = NEColorSample.MakeColFromBlocks5((ConsoleColor)12, (ConsoleColor)4, dd* (Math.Abs(py) - 0.71f));
                if (py < 0.3f + (float)Math.Sin(rayAngle * 10 + m_PlayerRotation * 4) * 0.1f)
                {
                    ceilSample = NEColorSample.MakeColFromBlocks5((ConsoleColor)0, (ConsoleColor)0, 0.0f);
                }

                if (hit)
                {

                    if (py > ceilingStartY) //draw ceiling
                    {

                        NEScreenBuffer.PutChar(ceilSample.Character, ceilSample.BitMask, x, y);
                    }
                    else if (py < floorStartY)
                    {
                        NEScreenBuffer.PutChar(floorSample.Character, floorSample.BitMask, x, y);
                    }
                    else
                    {
                        float fractX = ray.X - (float)Math.Floor(ray.X);
                        float fractY = ray.Y - (float)Math.Floor(ray.Y);

                        //if viewer is left of the wall 
                        if(m_ViewerPos.X < ray.X)
                        {
                            // swap forward mapping contribution 
                            fractY = -fractY;
                        }
                        //if viewer is in front ofthe wall
                        if(m_ViewerPos.Y < ray.Y)
                        {
                            //swap sidways mapping contribution
                            fractX = -fractX;
                        }

                        float u = fractX - fractY;
                        
                        float v = py / (floorStartY - ceilingStartY) + 0.5f;
                       // m_WallTex.SampleMode = NESampleMode.Repeat;
                        float luma = m_Wall.Sample(u, v);
                       // NEColorSample csample = m_WallTex.Sample(u, v, intensity*luma);



                        NEColorSample csample = NEColorSample.MakeCol(ConsoleColor.Black, ConsoleColor.White, luma*luma*luma * intensity, NECHAR_RAMPS.CHAR_RAMP_FULL);

                        char wallChar = csample.Character;
                        short wallCol = csample.BitMask;
                        m_DepthBuffer.TryUpdate(x, y, depth);
                        NEScreenBuffer.PutChar(wallChar, wallCol, x, y);
                    }

                }
                else
                {

                    if (py > ceilingStartY) NEScreenBuffer.PutChar(ceilSample.Character, ceilSample.BitMask, x, y);
                    else if (py < floorStartY) NEScreenBuffer.PutChar(floorSample.Character, floorSample.BitMask, x, y);
                    else NEScreenBuffer.PutChar((char)NEBlock.Weak, 0x0000 | 0x0000, x, y);
                }

                // Sprites
                //RenderSpriteCol(m_Lamp1Sprite, dir, py, x, y);
                //RenderSpriteCol(m_Lamp2Sprite, dir, py, x, y);
                //NEScreenBuffer.PutChar((char)NEBlock.Weak, 0x0000 | 0x0000, x, 10);
                //NEColorSample s = NEColorSample.MakeCol10(ConsoleColor.Black, ConsoleColor.Red, m_DepthBuffer.Sample(x, y));
                //NEScreenBuffer.PutChar(s.Character, s.BitMask, x, y);
            }

        }

        //private void RenderSpriteCol(NEStaticSprite sprite, NEVector2 dir, float py, int x, int y)
        //{
        //    // Sprites
        //    NEVector2 rayToSprite = sprite.Position - m_ViewerPos;

        //    float angleFromRay = (float)Math.Acos(NEVector2.Dot(dir, rayToSprite.Normalized));
        //    NEVector2 rayPerp = NEVector2.FindNormal(rayToSprite.Normalized);


        //    float distanceToSprite = rayToSprite.Length;
        //    float scailingFactor = (1.0f / distanceToSprite)*m_Fov ;// scale here

        //    float aspectRatio = sprite.AstpectRatio / m_Fov;

        //    //render only if deviation from sprite ray is within selected bounds
        //    bool shouldRender = (angleFromRay) <= (aspectRatio * scailingFactor) * 0.5f;
        //    if (shouldRender)
        //    {
        //        float spriteTop = scailingFactor;
        //        float spriteFloor = -scailingFactor;
        //        if (py < spriteTop && py >= spriteFloor)
        //        {
        //            // figure out whether current pos is left or right of the sprite ray
        //            float sign = NEMathHelper.Sign(NEVector2.Dot(dir, rayPerp));
        //            float u = (angleFromRay / aspectRatio / scailingFactor) * sign + 0.5f;
        //            float v = py / (spriteFloor - spriteTop) + 0.5f;
        //            NEColorSample csample = sprite.Texture.Sample(u, v, 1.0f);
        //            if (csample.Character != 't')
        //            {
        //                if (m_DepthBuffer.TryUpdate(x, y, distanceToSprite / DEPTH))
        //                    NEScreenBuffer.PutChar(csample.Character, csample.BitMask, x, y);
        //            }
        //        }

        //    }
        //}

        void RenderSpriteFull(NEStaticSprite sprite)
        {
            NEVector2 rayToSprite = sprite.Position - m_ViewerPos;
            float distanceToSprite = rayToSprite.Length;
            if (distanceToSprite > 15) return;
            float angleFromRay = (float)Math.Acos(NEVector2.Dot(m_ViewerDir, rayToSprite.Normalized));
            // 0.3 because:
            //   2* angleFromRay < m_Fov*0.5
            //   angleFromRay< m_Fov*0.5/2.0
            //   angleFromRay< m_Fov*0.25 
            //   rounded up to 0.3 so objects dont pop up suddenly from sides
            if (angleFromRay < (m_Fov*0.3f))
            {
                NEVector2 rayPerp = NEVector2.FindNormal(rayToSprite.Normalized);
                float sign = -NEMathHelper.Sign(NEVector2.Dot(m_ViewerDir, rayPerp));

                //float distanceToSprite = rayToSprite.Length;
                float scalingFactor = (1.0f / distanceToSprite);// scale here

                float aspectRatio = sprite.AstpectRatio / m_Fov;

                int middleSpriteOnScreen = (int)((0.5f + ((angleFromRay * sign) / (m_Fov * 0.5f)))  * ScreenWidth);

                float ceiling = ((float)ScreenHeight * 0.5f) - ((float)ScreenHeight) * scalingFactor;
                float floor = ScreenHeight - ceiling;
                float height = floor - ceiling;
                float width = height * aspectRatio*m_AspectRatio;
                for (int x = 0; x < width; ++x)
                {
                    float u = ((float)x) / width;
                    for (int y = 0; y < height; ++y)
                    {
                       
                        float v = (float)y / height;
                        NEColorSample s = sprite.Texture.Sample(u, v, 1.0f);

                        if (s.Character != 't')
                        {

                            int drawX = middleSpriteOnScreen + x - (int)(width*0.5f);
                            int drawY = (int)ceiling + y;
                            if(drawX>0 && drawX <ScreenWidth && drawY>0 && drawY<ScreenHeight)
                            { 

                                if (m_DepthBuffer.TryUpdate(drawX,drawY , distanceToSprite / DEPTH))
                                {
                                    NEScreenBuffer.PutChar(s.Character, s.BitMask, drawX, drawY);
                                }
                            }
                        }
                    }
                }
            }
        }

        public override bool OnDraw()
        {
            
            RenderSpriteFull(m_Lamp1Sprite);
            RenderSpriteFull(m_Lamp2Sprite);

            //for (int x = 0; x < m_Lamp1Sprite.Texture.Width; ++x)
            //{
            //    for (int y = 0; y < m_Lamp1Sprite.Texture.Height; ++y)
            //    {
            //        float u = (float)x / (float)m_Lamp1Sprite.Texture.Width;
            //        float v = (float)y / (float)m_Lamp1Sprite.Texture.Height;
            //        NEColorSample s = m_Lamp1Sprite.Texture.Sample(u, v, 0.9f);

            //        if (s.Character != 't')
            //        {
            //            NEScreenBuffer.PutChar(s.Character, s.BitMask, 90 + x, 50 + y);
            //        }
            //    }
            //}



            float imgW = (0.25f * ScreenWidth);
            float imgH = (0.25f * ScreenWidth);
            NEVector2 mapXY = new NEVector2(0, 0);
            for (int x = 0; x < imgW; ++x)
            {
                float nx = ((float)x) / imgW;
                mapXY.X = nx * (float)m_MapWidth;
                for (int y = 0; y < imgH; ++y)
                {
                    float ny = ((float)y) / imgH;
                    mapXY.Y = ny * (float)m_MapHeight;
                    int cell = GetCell(mapXY);
                    if(cell != 0)
                    {
                        NEScreenBuffer.PutChar('#', 15, (int)imgW-x, (int)imgH-y);
                    }
                    else
                    {
                        NEScreenBuffer.PutChar('#', 8, (int)imgW - x, (int)imgH - y);
                    }

                    NEVector2 A = new NEVector2(-0.65f, -0.65f);
                    NEVector2 B = new NEVector2(0.0f, 1.28f);
                    NEVector2 C = new NEVector2(0.65f, -0.65f);

                    NEVector2.RotateClockWise(ref A, m_PlayerRotation);
                    NEVector2.RotateClockWise(ref B, m_PlayerRotation);
                    NEVector2.RotateClockWise(ref C, m_PlayerRotation);
                    A += m_ViewerPos;
                    B += m_ViewerPos;
                    C += m_ViewerPos;
                    if (NEMathHelper.InTriangle(mapXY,A,B,C))
                    {
                        NEScreenBuffer.PutChar('@', 13 << 4, (int)imgW - x, (int)imgH - y);
                    }

                    if(NEVector2.CalculateLength(mapXY - m_Lamp1Sprite.Position) < 0.4f)
                    {
                        NEScreenBuffer.PutChar('@', 13 << 4, (int)imgW - x, (int)imgH - y);
                    }
                }
            }
            if(m_ShowPalette)NEDebug.DrawPalette(ScreenWidth, ScreenHeight);
            return base.OnDraw();
        }

        int GetCell(NEVector2 p)
        {
            int x = (int)p.X;
            int y = (int)p.Y;
            if ((x > m_MapWidth - 1.0) || (x < 0)) return 0;
            if ((y > m_MapHeight - 1.0) || (y < 0)) return 0;
            return m_Map[(m_MapWidth * y) + x];
        }
    }
}
