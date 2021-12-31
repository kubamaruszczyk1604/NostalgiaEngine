using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using ConsoleRenderer.Core;

namespace ConsoleRenderer
{
    class CGRaytracer2D : CGScene
    {

        private int m_MapWidth = 20;
        private int m_MapHeight = 20;
        private int[] m_Map = {
        1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,1,0,0,1,
        1,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1
        };



        const float EPSILON = 0.01f;
        const float DEG_TO_RAD = 0.0174532f;
        const float M_PI = 3.1415926535f;
        const float DEPTH = 24.0f;
        const float ROTATION_SPEED = 1.5f;
        const float MOVEMENT_SPEED = 4.0f;


        private float m_AspectRatio;
        private float m_Fov;

        Vector2 m_ViewerPos = new Vector2(3.0f, 1.0f);
        Vector2 m_ViewerDir = new Vector2(0.0f, 1.0f);
        float m_PlayerRotation = 0.0f;
        CGTexture16 m_WallTex;

        public override void OnInitialize()
        {
            ScreenWidth = 320;
            ScreenHeight = 200;
            PixelWidth = 4;
            PixelHeight = 4;

        }

        override public void OnStart()
        {
            m_AspectRatio = (float)ScreenWidth / (float)ScreenHeight;
            m_Fov = 80.0f * DEG_TO_RAD;
            m_WallTex = CGTexture16.LoadFromFile($"C:/Users/Kuba/Desktop/eng/test.txt");
           // CGBuffer.HalfTemporalResolution = true;
        }

        override public void OnUpdate(float dt)
        {

            float deltaT = dt;
            if (CGInput.CheckKeyDown(ConsoleKey.LeftArrow))
            {

                if (CGInput.CheckKeyDown(0xA2))
                {
                    m_ViewerPos -= CGHelper.FindNormal(m_ViewerDir) * MOVEMENT_SPEED * deltaT;

                }
                else
                {
                    float deltaR = -ROTATION_SPEED * deltaT;
                    m_PlayerRotation += deltaR;
                    CGHelper.Rotate(ref m_ViewerDir, deltaR);
                    Vector2.NormalizeFast(m_ViewerDir);
                }


            }
            if (CGInput.CheckKeyDown(ConsoleKey.RightArrow))
            {

                if (CGInput.CheckKeyDown(0xA2))
                {

                    m_ViewerPos += CGHelper.FindNormal(m_ViewerDir) * MOVEMENT_SPEED * deltaT;

                }
                else
                {
                    float deltaR = ROTATION_SPEED * deltaT;
                    m_PlayerRotation += deltaR;
                    CGHelper.Rotate(ref m_ViewerDir, deltaR);
                    Vector2.NormalizeFast(m_ViewerDir);
                }
            }



            if (CGInput.CheckKeyDown(ConsoleKey.UpArrow))
            {
                m_ViewerPos += m_ViewerDir * MOVEMENT_SPEED * deltaT;
            }
            if (CGInput.CheckKeyDown(ConsoleKey.DownArrow))
            {
                m_ViewerPos -= m_ViewerDir * MOVEMENT_SPEED * deltaT;
            }

            if (CGInput.CheckKeyPress(ConsoleKey.Escape))
            {
                Exit("some data");
            }

            if (CGInput.CheckKeyPress(ConsoleKey.N))
            {
                CGEngine.Instance.PushScene(new ConsoleRenderer.TextureEditor.Test_MemTex16());
            }

        }

        override public void OnDrawPerColumn(int x)
        {
            float px = (((float)x / (float)ScreenWidth) - 0.5f) * 0.5f;

            px *= m_Fov; // TO DO: consider aspect ratio 

            Vector2 dir = new Vector2(m_ViewerDir.X, m_ViewerDir.Y);
            CGHelper.Rotate(ref dir, px);
            dir = Vector2.NormalizeFast(dir);

            float t = 0.0f;
            const float stp = 0.01f;
            bool hit = false;
            Vector2 ray = Vector2.Zero;
            while ((t < DEPTH) && !hit)
            {
                ray = m_ViewerPos + dir * t;
                int cell = GetCell(ray);
                t += stp;
                if (cell != 0)
                    hit = true;
            }
            float ceiling = (1.0f / t);
            float floorp = (-1.0f / t);
            float intensity = 1.0f - (t / DEPTH);
            //intensity *= intensity;

            for (int y = 0; y < ScreenHeight; ++y)
            {

                float pixelY = -(y - ScreenHeight / 2);
                float py = pixelY / ((float)ScreenHeight) / 2.0f;
                py *= m_Fov * 2.0f;

                //ColorSample floorSample = ColorSample.MakeCol(ConsoleColor.Black, ConsoleColor.DarkGray, fd);
                CGColorSample floorSample = CGColorSample.MakeCol(ConsoleColor.Black, ConsoleColor.DarkGreen, Math.Abs(py * 1.4f) -Math.Abs(px * 0.1f));


                CGColorSample ceilSample = CGColorSample.MakeCol(ConsoleColor.DarkRed, ConsoleColor.DarkMagenta, Math.Abs(py) - 0.1f);
                if (py < 0.3f + (float)Math.Sin(px * 10 + m_PlayerRotation * 4) * 0.1f)
                {
                    ceilSample = CGColorSample.MakeCol(ConsoleColor.Black, ConsoleColor.DarkGray, Math.Abs(py * 0.75f) - Math.Abs(px * 0.1f));
                }

                if (hit)
                {

                    if (py > ceiling) //draw ceiling
                    {

                        CGBuffer.AddAsync(ceilSample.Character, ceilSample.BitMask, x, y);
                    }
                    else if (py < floorp)
                    {
                        CGBuffer.AddAsync(floorSample.Character, floorSample.BitMask, x, y);
                    }
                    else
                    {
                        float rayFract = (ray.X - (float)Math.Floor(ray.X) - (ray.Y - (float)Math.Floor(ray.Y)));
                        rayFract = Math.Abs(rayFract);

                        //ColorSample csample = ColorSample.MakeCol(ConsoleColor.Black, ConsoleColor.DarkBlue, intensity);
                        CGColorSample csample = m_WallTex.Sample(rayFract, py / (floorp - ceiling) + 0.5f, intensity);
                        char wallChar = csample.Character;
                        short wallCol = csample.BitMask;

                        //if (Math.Abs(rayFract) < 0.05f || Math.Abs(rayFract) > 0.95f)
                        //{
                        //    wallChar = (char)CGBlock.Strong;
                        //    wallCol = 0;
                        //}
                        CGBuffer.AddAsync(wallChar, wallCol, x, y);
                    }

                }
                else
                {

                    if (py > ceiling) CGBuffer.AddAsync(ceilSample.Character, ceilSample.BitMask, x, y);
                    else if (py < floorp) CGBuffer.AddAsync(floorSample.Character, floorSample.BitMask, x, y);
                    else CGBuffer.AddAsync((char)CGBlock.Weak, 0x0000 | 0x0000, x, y);
                }


            }

        }

        public override void OnDraw()
        {
            float imgW = (0.25f * ScreenWidth);
            float imgH = (0.25f * ScreenWidth);

            Vector2 mapXY = Vector2.Zero;
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
                        CGBuffer.AddAsync('#', 15, (int)imgW-x, (int)imgH-y);
                    }
                    else
                    {
                        CGBuffer.AddAsync('#', 8, (int)imgW - x, (int)imgH - y);
                    }

                    Vector2 A = new Vector2(-0.65f, -0.65f);
                    Vector2 B = new Vector2(0.0f, 1.28f);
                    Vector2 C = new Vector2(0.65f, -0.65f);

                    CGHelper.Rotate(ref A, m_PlayerRotation);
                    CGHelper.Rotate(ref B, m_PlayerRotation);
                    CGHelper.Rotate(ref C, m_PlayerRotation);
                    A +=m_ViewerPos;
                    B += m_ViewerPos;
                    C += m_ViewerPos;
                    if (CGHelper.InTriangle(mapXY,A,B,C))
                    {
                        CGBuffer.AddAsync('@', 13 << 4, (int)imgW - x, (int)imgH - y);
                    }

                }
            }

        }

        int GetCoord(int x, int y)
        {
            return (m_MapWidth * y) + x;
        }

        int GetCell(Vector2 p)
        {
            int x = (int)p.X;
            int y = (int)p.Y;
            if ((x > m_MapWidth - 1.0) || (x < 0)) return 0;
            if ((y > m_MapHeight - 1.0) || (y < 0)) return 0;
            return m_Map[GetCoord(x, y)];
        }
    }
}
