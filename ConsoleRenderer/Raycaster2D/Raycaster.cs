using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.Raycaster
{
    class NERaycaster2D : NEScene
    {


        private readonly float EPSILON = 0.01f;
        private readonly float DEG_TO_RAD = 0.0174532f;
        private readonly float M_PI = 3.1415926535f;
        private readonly float DEPTH = 24.0f;
        private readonly float ROTATION_SPEED = 1.5f;
        private readonly float MOVEMENT_SPEED = 4.0f;


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
        private NETexture16 m_WallTex;

        public override bool OnLoad()
        {
            ScreenWidth = 280;
            ScreenHeight = 180;
            PixelWidth = 4;
            PixelHeight = 4;
           // ParallelScreenDraw = true;
            m_WallTex = NETexture16.LoadFromFile($"C:/Users/Kuba/Desktop/untitled2.tex");
            if (m_WallTex == null) return false;
            return true;
        }

        override public void OnStart()
        {
            m_AspectRatio = (float)ScreenWidth / (float)ScreenHeight;
            m_Fov = 80.0f * DEG_TO_RAD;
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
                    NEVector2.Rotate(ref m_ViewerDir, deltaR);
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
                    NEVector2.Rotate(ref m_ViewerDir, deltaR);
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
                Engine.Instance.PushScene(new TextureEditor.Test_MemTex16());
            }

        }

        override public void OnDrawPerColumn(int x)
        {
            float px = (((float)x / (float)ScreenWidth) - 0.5f) * 0.5f;

            px *= m_Fov; // TO DO: consider aspect ratio 

            NEVector2 dir = new NEVector2(m_ViewerDir.X, m_ViewerDir.Y);
            NEVector2.Rotate(ref dir, px);
            dir = NEVector2.Normalize(dir);

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
            float ceiling = (1.0f / t);
            float floorp = (-1.0f / t);
            float intensity = 1.0f - (t / DEPTH);
            //intensity *= intensity*1.5f;

            for (int y = 0; y < ScreenHeight; ++y)
            {

                float pixelY = -(y - ScreenHeight / 2);
                float py = pixelY / ((float)ScreenHeight) / 2.0f;
                py *= m_Fov * 2.0f;

                //ColorSample floorSample = ColorSample.MakeCol(ConsoleColor.Black, ConsoleColor.DarkGray, fd);
                NEColorSample floorSample = NEColorSample.MakeCol5(ConsoleColor.Black, ConsoleColor.DarkGreen, 0.2f);// Math.Abs(py) -Math.Abs(px * 0.1f));


                NEColorSample ceilSample = NEColorSample.MakeCol10(ConsoleColor.DarkRed, ConsoleColor.DarkMagenta, Math.Abs(py) - 0.1f);
                if (py < 0.3f + (float)Math.Sin(px * 10 + m_PlayerRotation * 4) * 0.1f)
                {
                    ceilSample = NEColorSample.MakeCol5(ConsoleColor.DarkGreen, ConsoleColor.Green, Math.Abs(py * 0.75f) + NEMathHelper.Sin(px * 10 + m_PlayerRotation * 4) * 0.05f);
                }

                if (hit)
                {

                    if (py > ceiling) //draw ceiling
                    {

                        NEConsoleScreen.PutChar(ceilSample.Character, ceilSample.BitMask, x, y);
                    }
                    else if (py < floorp)
                    {
                        NEConsoleScreen.PutChar(floorSample.Character, floorSample.BitMask, x, y);
                    }
                    else
                    {
                        float rayFract = (ray.X - (float)Math.Floor(ray.X) - (ray.Y - (float)Math.Floor(ray.Y)));
                        rayFract = Math.Abs(rayFract);

                        //ColorSample csample = ColorSample.MakeCol(ConsoleColor.Black, ConsoleColor.DarkBlue, intensity);
                        NEColorSample csample = m_WallTex.Sample(rayFract, py / (floorp - ceiling) + 0.5f, intensity);
                        char wallChar = csample.Character;
                        short wallCol = csample.BitMask;

                        //if (Math.Abs(rayFract) < 0.05f || Math.Abs(rayFract) > 0.95f)
                        //{
                        //    wallChar = (char)CGBlock.Strong;
                        //    wallCol = 0;
                        //}
                        NEConsoleScreen.PutChar(wallChar, wallCol, x, y);
                    }

                }
                else
                {

                    if (py > ceiling) NEConsoleScreen.PutChar(ceilSample.Character, ceilSample.BitMask, x, y);
                    else if (py < floorp) NEConsoleScreen.PutChar(floorSample.Character, floorSample.BitMask, x, y);
                    else NEConsoleScreen.PutChar((char)NEBlock.Weak, 0x0000 | 0x0000, x, y);
                }


            }

        }

        public override void OnDraw()
        {
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
                        NEConsoleScreen.PutChar('#', 15, (int)imgW-x, (int)imgH-y);
                    }
                    else
                    {
                        NEConsoleScreen.PutChar('#', 8, (int)imgW - x, (int)imgH - y);
                    }

                    NEVector2 A = new NEVector2(-0.65f, -0.65f);
                    NEVector2 B = new NEVector2(0.0f, 1.28f);
                    NEVector2 C = new NEVector2(0.65f, -0.65f);

                    NEVector2.Rotate(ref A, m_PlayerRotation);
                    NEVector2.Rotate(ref B, m_PlayerRotation);
                    NEVector2.Rotate(ref C, m_PlayerRotation);
                    A +=m_ViewerPos;
                    B += m_ViewerPos;
                    C += m_ViewerPos;
                    if (NEMathHelper.InTriangle(mapXY,A,B,C))
                    {
                        NEConsoleScreen.PutChar('@', 13 << 4, (int)imgW - x, (int)imgH - y);
                    }

                }
            }

        }

        int GetCoord(int x, int y)
        {
            return (m_MapWidth * y) + x;
        }

        int GetCell(NEVector2 p)
        {
            int x = (int)p.X;
            int y = (int)p.Y;
            if ((x > m_MapWidth - 1.0) || (x < 0)) return 0;
            if ((y > m_MapHeight - 1.0) || (y < 0)) return 0;
            return m_Map[GetCoord(x, y)];
        }
    }
}
