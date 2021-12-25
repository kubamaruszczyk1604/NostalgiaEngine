using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace ConsoleRenderer
{
    class CGRaytracer : CGApp
    {

        private int m_MapWidth = 20;
        private int m_MapHeight = 20;
        private int[] m_Map = {
        1,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1
        };



        const float EPSILON = 0.01f;
        const float DEG_TO_RAD = 0.0174532f;
        const float M_PI = 3.1415926535f;
        const float DEPTH = 24.0f;
        const float ROTATION_SPEED = 1.5f;
        const float MOVEMENT_SPEED = 10.0f;



        private int m_ScrWidth;
        private int m_ScrHeight;

        private float m_AspectRatio;
        private float m_Fov;

        Vector2 m_ViewerPos = new Vector2(3.0f, 1.0f);
        Vector2 m_ViewerDir = new Vector2(0.0f, 1.0f);
        float m_PlayerRotation = 0.0f;
        Texture16 m_WallTex;


        override public void OnStart()
        {
            m_ScrWidth = CGEngine.ScreenWidth;
            m_ScrHeight = CGEngine.ScreenHeight;
            m_AspectRatio = (float)m_ScrWidth / (float)m_ScrHeight;
            m_Fov = 80.0f * DEG_TO_RAD;
            m_WallTex = Texture16.LoadFromFile($"C:/Users/Kuba/Desktop/eng/test.txt");
           // Buffer.HalfTemporalResolution = true;
        }

        override public void OnUpdate(float dt)
        {
            
            float deltaT = dt;
            if (Input.CheckKeyDown(ConsoleKey.LeftArrow))
            {
                Vector2 direction = m_ViewerDir;
                if (Input.CheckKeyDown(0xA2))
                {
                    float tx = direction.X;
                    direction.X = -direction.Y;
                    direction.Y = tx;
                    m_ViewerPos -= direction * MOVEMENT_SPEED * deltaT;

                }
                else
                {
                    float deltaR = -ROTATION_SPEED * deltaT;
                    m_PlayerRotation += deltaR;
                    Helper.Rotate(ref m_ViewerDir, deltaR);
                    Vector2.NormalizeFast(m_ViewerDir);
                }


            }
            if (Input.CheckKeyDown(ConsoleKey.RightArrow))
            {

                Vector2 direction = m_ViewerDir;
                if (Input.CheckKeyDown(0xA2))
                {
                    float tx = direction.X;
                    direction.X = -direction.Y;
                    direction.Y = tx;
                    m_ViewerPos += direction * MOVEMENT_SPEED * deltaT;

                }
                else
                {
                    float deltaR = ROTATION_SPEED * deltaT;
                    m_PlayerRotation += deltaR;
                    Helper.Rotate(ref m_ViewerDir, deltaR);
                    Vector2.NormalizeFast(m_ViewerDir);
                }
            }



            if (Input.CheckKeyDown(ConsoleKey.UpArrow))
            {
                m_ViewerPos += m_ViewerDir * MOVEMENT_SPEED * deltaT;
            }
            if (Input.CheckKeyDown(ConsoleKey.DownArrow))
            {
                m_ViewerPos -= m_ViewerDir * MOVEMENT_SPEED * deltaT;
            }

        }

        override public void OnDrawPerColumn(int x)
        {
            float px = (((float)x / (float)m_ScrWidth) - 0.5f) * 0.5f;

            px *= m_Fov; // TO DO: consider aspect ratio 


            //Vector2 pos = m_ViewerPos;
            Vector2 dir = new Vector2(m_ViewerDir.X, m_ViewerDir.Y);
            Helper.Rotate(ref dir, px);
            dir = Vector2.Normalize(dir);



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

            for (int y = 0; y < m_ScrHeight; ++y)
            {

                float pixelY = -(y - m_ScrHeight / 2);
                float py = pixelY / ((float)m_ScrHeight) / 2.0f;
                py *= m_Fov * 2.0f;

                //ColorSample floorSample = ColorSample.MakeCol(ConsoleColor.Black, ConsoleColor.DarkGray, fd);
                ColorSample floorSample = ColorSample.MakeCol(ConsoleColor.Black, ConsoleColor.DarkGreen, Math.Abs(py * 1.2f) - Math.Abs(px * 0.1f));


                ColorSample ceilSample = ColorSample.MakeCol(ConsoleColor.DarkRed, ConsoleColor.DarkMagenta, Math.Abs(py) - 0.1f);
                if (py < 0.3f + (float)Math.Sin(px * 10 + m_PlayerRotation * 4) * 0.1f)
                {
                    ceilSample = ColorSample.MakeCol(ConsoleColor.Black, ConsoleColor.DarkGray, Math.Abs(py * 0.75f) - Math.Abs(px * 0.1f));
                }


                if (hit)
                {

                    if (py > ceiling) //draw ceiling
                    {

                        Buffer.AddAsync(ceilSample.Character, ceilSample.BitMask, x, y);
                    }
                    else if (py < floorp)
                    {
                        Buffer.AddAsync(floorSample.Character, floorSample.BitMask, x, y);
                    }
                    else
                    {
                        float rayFract = (ray.X - (float)Math.Floor(ray.X) - (ray.Y - (float)Math.Floor(ray.Y)));
                        rayFract = Math.Abs(rayFract);

                        //ColorSample csample = ColorSample.MakeCol(ConsoleColor.Black, ConsoleColor.DarkBlue, intensity);
                        ColorSample csample = m_WallTex.Sample(rayFract, py / (floorp - ceiling) + 0.5f, intensity);
                        char wallChar = csample.Character;
                        short wallCol = csample.BitMask;

                        //if (Math.Abs(rayFract) < 0.05f || Math.Abs(rayFract) > 0.95f)
                        //{
                        //    wallChar = (char)Block.Strong;
                        //    wallCol = 0;
                        //}
                        Buffer.AddAsync(wallChar, wallCol, x, y);
                    }

                }
                else
                {

                    if (py > ceiling) Buffer.AddAsync(ceilSample.Character, ceilSample.BitMask, x, y);
                    else if (py < floorp) Buffer.AddAsync(floorSample.Character, floorSample.BitMask, x, y);
                    else Buffer.AddAsync((char)Block.Weak, 0x0000 | 0x0000, x, y);
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
