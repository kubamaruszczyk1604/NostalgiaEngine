using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;





namespace ConsoleRenderer
{
    
    struct Ray
    {
        public Vector3 Direction;
        public Vector3 Orgin;
    }



    partial class RayMarcher
    {
        private int m_MapWidth = 10;
        private int m_MapHeight = 10;
        private int[] m_Map = {
        1,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,1,0,0,0,1,
        1,0,0,0,0,0,1,0,0,1,
        1,0,1,0,0,0,0,1,0,1,
        1,0,0,0,0,2,0,0,0,1,
        1,0,1,0,0,0,0,1,0,1,
        1,0,0,0,0,0,0,1,0,1,
        1,0,1,0,0,0,1,0,0,1,
        1,0,0,0,1,0,0,0,0,1,
        1,1,1,1,1,1,1,1,1,1
        };

        const int MAX_MARCHING_STEPS = 255;
        public const float MIN_DIST = 1.0f;
        public const float MAX_DIST = 1000.0f;
        const float EPSILON = 0.01f;
        const float DEG_TO_RAD = 0.0174532f;
        const float M_PI = 3.1415926535f;
        const float DEPTH = 10.0f;



        private int m_ScrWidth;
        private int m_ScrHeight;

        private float m_AspectRatio;
        private float m_Fov;
        private float m_FovDist;

        static private float m_TotalTime = 0;

        //Used for animations - needs to be replaced with global time
        static public float TotalTime
        {
            get
            { return m_TotalTime; }
        }



        public RayMarcher(int width, int height)
        {
            m_ScrWidth = width;
            m_ScrHeight = height;

            Buffer.Initialize((short)m_ScrWidth, (short)m_ScrHeight,4,4);


            
            m_AspectRatio = (float)m_ScrWidth / (float)m_ScrHeight;
            m_Fov = 80.0f * DEG_TO_RAD;
            m_FovDist = (float)Math.Tan(m_Fov);
            Input.ev_KeyPressed += KeyPress;
        }



        Vector2 m_ViewerPos = new Vector2(3.0f, 1.0f);
        Vector2 m_ViewerDir = new Vector2(0.0f, 1.0f);
        float m_PlayerRotation = 0.0f;
        void KeyPress(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.LeftArrow)
            {
                Vector3 tmp = new Vector3(m_ViewerDir.X, m_ViewerDir.Y, 0.0f) * Matrix3.CreateRotationZ(-0.05f);
                m_ViewerDir = Vector2.Normalize(tmp.Xy);
                m_PlayerRotation -= 0.05f;
            }
            else if (keyInfo.Key == ConsoleKey.RightArrow)
            {
                Vector3 tmp = new Vector3(m_ViewerDir.X, m_ViewerDir.Y, 0.0f) * Matrix3.CreateRotationZ(0.05f);
                m_ViewerDir = Vector2.Normalize(tmp.Xy);
                m_PlayerRotation += 0.05f;
            }
            else if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                //m_ViewerPos.Y += 0.1f;
                m_ViewerPos += m_ViewerDir * 0.1f;
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                m_ViewerPos -= m_ViewerDir * 0.1f;
            }
        }





        public void RenderLoop2()
        {

            FrameTimer.Update();
            //Initial setup



            Console.ForegroundColor = ConsoleColor.DarkGray;
            // Console.BackgroundColor = ConsoleColor.White;

            while (true)//Render Loop
            {


                Console.SetCursorPosition(5, 1);
                FrameTimer.Update();
                Console.Title = " FPS: " + FrameTimer.GetFPS() + "   FRAME TIME: " + FrameTimer.GetDeltaTime() + "s ";

                var resetEvent = new ManualResetEvent(false); // Will be reset when buffer is ready to be swaped

                //For each column..
                for (int x = 0; x < m_ScrWidth; ++x)
                {
                    // Queue new task
                    ThreadPool.QueueUserWorkItem(
                       new WaitCallback(
                     delegate (object state)
                     {
                         object[] array = state as object[];
                         int column = Convert.ToInt32(array[0]);

                         Draw(column);

                         if (column >= m_ScrWidth - 1) resetEvent.Set();
                     }), new object[] { x });
                }

                //Thread.Sleep(10);
                resetEvent.WaitOne();
                
                Buffer.Swap();
                m_TotalTime += FrameTimer.GetDeltaTime();
            }
        }


        int GetCoord(int x, int y)
        {
            return (10 * y) + x;
        }

        int GetCell(Vector2 p)
        {
            int x = (int)p.X;
            int y = (int)p.Y;
            if (x > (int)DEPTH - 1.0) return 0;
            if (y > (int)DEPTH - 1.0) return 0;

            if (x < 0) return 0;
            if (y < 0) return 0;

            return m_Map[GetCoord(x, y)];
        }
        


        private void Draw(int x)
        {
            float pixelX = x - m_ScrWidth / 2;
            float px = pixelX / ((float)m_ScrWidth) / 2.0f;
            px *= m_Fov; // TO DO: consider aspect ratio 


            Vector2 pos = m_ViewerPos;
            Vector3 dir = new Vector3(m_ViewerDir.X, m_ViewerDir.Y, 0.0f);
           // Vector3 dir = new Vector3((float)Math.Sin(m_PlayerRotation), (float)Math.Cos(m_PlayerRotation), 0.0f);
            dir *= Matrix3.CreateRotationZ(px);
            dir = Vector3.Normalize(dir);



            float t = 0.0f;
            const float stp = 0.01f;
            bool hit = false;
            Vector2 ray = Vector2.Zero;
            while ((t < DEPTH) && !hit)
            {
                ray = pos + dir.Xy * t;
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
                py *= m_Fov*2.0f;
                //Vector3 d = new Vector3(m_ScrWidth / 2, m_ScrHeight + 65, 0) - new Vector3(x,y,0);
                //float fd = (1.0f-d.LengthFast/m_ScrHeight);

                //ColorSample floorSample = ColorSample.MakeCol(ConsoleColor.Black, ConsoleColor.DarkGray, fd);
                ColorSample floorSample = ColorSample.MakeCol(ConsoleColor.Black, ConsoleColor.DarkGreen, Math.Abs(py*0.75f) - Math.Abs(px*0.1f));


                ColorSample ceilSample = ColorSample.MakeCol(ConsoleColor.DarkRed, ConsoleColor.DarkMagenta, Math.Abs(py)-0.1f);
                if (py < 0.3f + (float)Math.Sin(px * 30 + m_PlayerRotation*20) * 0.1f)
                {
                    ceilSample = ColorSample.MakeCol(ConsoleColor.Black, ConsoleColor.DarkGray, Math.Abs(py));
                }
                //(float)Math.Sin(px * 40) * 0.1f

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
                        ColorSample csample = ColorSample.MakeCol(ConsoleColor.Black, ConsoleColor.DarkBlue, intensity);
                        char wallChar = csample.Character;
                        short wallCol = csample.BitMask;
                        float rayFract = (ray.X - (float)Math.Floor(ray.X) - (ray.Y - (float)Math.Floor(ray.Y)));
                        if (Math.Abs(rayFract) < 0.05f || Math.Abs(rayFract) > 0.95f)
                        {
                            wallChar = (char)Block.Strong;
                            wallCol = 0;
                        }
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

        

        private void Draw2(int x)
        {

            float u = (float)x / 250.0f;
            ColorSample csample = ColorSample.MakeCol(ConsoleColor.White, ConsoleColor.Black, u);

            for (int y = 0; y < m_ScrHeight; ++y)
            {
                Buffer.AddAsync(csample.Character, csample.BitMask, x, y);
                //int index = (int)((((float)x) / 150.0f) * 5.0f);
                ////ProduceShadedColor(out char ceilChar, out short ceilCol, Math.Abs(py), (short)COLOUR.FG_BLUE, (short)COLOUR.BG_BLUE);
                //if (y < 70)
                //    Buffer.AddAsync((char)BLOCKS.BLOCK_ARR[index], (short)(ColorMask.BG_WHITE | ColorMask.FG_BLUE), x, y);
                //else
                //{
                //    int ind = (5 - index);
                //    Buffer.AddAsync((char)BLOCKS.BLOCK_ARR[ind > 4 ? 4 : ind], (short)(ColorMask.BG_BLUE | ColorMask.FG_WHITE), x, y);

                //}


            }

        }

        

        float CalculateLight(Vector3 lightDir, Vector3 normal)
        {
            //lightDir *= Matrix3.CreateRotationY((float)Math.Sin(CycleCount * 0.8) * 0.06f);
            Vector3 nLightDirection = lightDir.Normalized();
            //angle between direction vector and surface normal
            float theta = Vector3.Dot(nLightDirection, normal);
            float totalDiffuse = theta;

            return totalDiffuse;
        }







        


  

    }



}
