using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using OpenTK;
using System.Diagnostics;
using ConsoleRenderer.Core;




namespace ConsoleRenderer
{
    
    struct Ray
    {
        public Vector3 Direction;
        public Vector3 Orgin;
    }



    partial class RayMarcher
    {
        private int m_MapWidth = 20;
        private int m_MapHeight =20;
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

        static private float m_TotalTime = 0;

        //Used for animations - needs to be replaced with global time
        static public float TotalTime
        {
            get
            { return m_TotalTime; }
        }



        public RayMarcher(int width, int height,int pixelW, int pixelH)
        {

            m_ScrWidth = width;
            m_ScrHeight = height;

            CGBuffer.Initialize((short)m_ScrWidth, (short)m_ScrHeight, (short)pixelW,(short) pixelH);


            
            m_AspectRatio = (float)m_ScrWidth / (float)m_ScrHeight;
            m_Fov = 80.0f * DEG_TO_RAD;
            //m_FovDist = (float)Math.Tan(m_Fov);
            //Input_old.ev_KeyPressed += KeyPress;
        }



        Vector2 m_ViewerPos = new Vector2(3.0f, 1.0f);
        Vector2 m_ViewerDir = new Vector2(0.0f, 1.0f);
        float m_PlayerRotation = 0.0f;


        void KeyPress()
        {

            float deltaT = CGFrameTimer.GetDeltaTime();
            if (CGInput.CheckKeyDown(ConsoleKey.LeftArrow))
            {
                Vector2 direction = m_ViewerDir;
                if (CGInput.CheckKeyDown(0xA2))
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
                    Rotate(ref m_ViewerDir, deltaR);
                    Vector2.NormalizeFast(m_ViewerDir);
                }


            }
             if (CGInput.CheckKeyDown(ConsoleKey.RightArrow))
            {

                Vector2 direction = m_ViewerDir;
                if (CGInput.CheckKeyDown(0xA2))
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
                    Rotate(ref m_ViewerDir, deltaR);
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
           
        }





        public void Play()
        {

            CGFrameTimer.Update();
            //Initial setup



           // Console.ForegroundColor = ConsoleColor.DarkGray;
            // Console.BackgroundColor = ConsoleColor.White;
            float lastDelta = 0.0f;
            while (true)//Render Loop
            {

                KeyPress();
                Console.SetCursorPosition(5, 1);
                Console.Title = " FPS: " + CGFrameTimer.GetFPS() + "   FRAME TIME: " + lastDelta + "s ";

                var resetEvent = new ManualResetEvent(false); // Will be reset when buffer is ready to be swaped

                //For each column..
                for (int x = 0; x < m_ScrWidth; ++x)
                {
                   // Draw(x);
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
                CGBuffer.Swap();

                m_TotalTime += CGFrameTimer.GetDeltaTime();
                lastDelta = CGFrameTimer.GetDeltaTime();
                CGFrameTimer.Update();

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

        void Rotate(ref Vector2 v, float theta)
        {
            float s = (float)Math.Sin(theta);
            float c = (float)Math.Cos(theta);
            v.X = v.X * c - v.Y * s;
            v.Y = v.X * s + v.Y * c;
        }

        private void Draw4(int x)
        {
            // float pixelX = x - m_ScrWidth / 2;
            //float px = pixelX / ((float)m_ScrWidth) / 2.0f;

            float px = (((float)x / (float)m_ScrWidth) - 0.5f) * 2.0f;
            Console.WriteLine(px);

            //px *= m_Fov; // TO DO: consider aspect ratio 



        }

            private void Draw(int x)
        {
            // float pixelX = x - m_ScrWidth / 2;
            //float px = pixelX / ((float)m_ScrWidth) / 2.0f;

            float px = (((float)x / (float)m_ScrWidth) -0.5f)*0.5f;

            px *= m_Fov; // TO DO: consider aspect ratio 


            //Vector2 pos = m_ViewerPos;
            Vector2 dir = new Vector2(m_ViewerDir.X, m_ViewerDir.Y);
            Rotate(ref dir, px);
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
                py *= m_Fov*2.0f;

                //ColorSample floorSample = ColorSample.MakeCol(ConsoleColor.Black, ConsoleColor.DarkGray, fd);
                CGColorSample floorSample = CGColorSample.MakeCol(ConsoleColor.Black, ConsoleColor.DarkGreen, Math.Abs(py*1.2f) - Math.Abs(px*0.1f));


                CGColorSample ceilSample = CGColorSample.MakeCol(ConsoleColor.DarkRed, ConsoleColor.DarkMagenta, Math.Abs(py)-0.1f);
                if (py < 0.3f + (float)Math.Sin(px * 10 + m_PlayerRotation*4) * 0.1f)
                {
                    ceilSample = CGColorSample.MakeCol(ConsoleColor.Black, ConsoleColor.DarkGray, Math.Abs(py*0.75f) - Math.Abs(px * 0.1f));
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
                        float rayFract = (ray.X - (float)Math.Floor(ray.X) -(ray.Y - (float)Math.Floor(ray.Y)));
                        rayFract = Math.Abs(rayFract);

                        //ColorSample csample = ColorSample.MakeCol(ConsoleColor.Black, ConsoleColor.DarkBlue, intensity);
                        CGColorSample csample = tex.Sample(rayFract, py / (floorp - ceiling) + 0.5f, intensity);
                        char wallChar = csample.Character;
                        short wallCol = csample.BitMask;
                        
                        //if (Math.Abs(rayFract) < 0.05f || Math.Abs(rayFract) > 0.95f)
                        //{
                        //    wallChar = (char)Block.Strong;
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

        

        private void Draw2(int x)
        {

            float u = (float)x / 250.0f;
            CGColorSample csample = CGColorSample.MakeCol(ConsoleColor.White, ConsoleColor.Black, u);

            for (int y = 0; y < m_ScrHeight; ++y)
            {
                CGBuffer.AddAsync(csample.Character, csample.BitMask, x, y);
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

        CGTexture16 tex = CGTexture16.LoadFromFile($"C:/Users/Kuba/Desktop/eng/test.txt");
        private void Draw3(int x)
        {

            float u = (float)x / 250.0f;
            

            for (int y = 0; y < m_ScrHeight; ++y)
            {
                float v = (float)y / 150.0f;
                CGColorSample csample = tex.Sample(u, v,0.1f+u);
                CGBuffer.AddAsync(csample.Character, csample.BitMask, x, y);



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
