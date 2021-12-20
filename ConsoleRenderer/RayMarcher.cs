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

    class RenderObject
    {
        public Material RenderMaterial;
        public float CalculatedDistance;



        public RenderObject(Material renderMaterial)
        {

            RenderMaterial = renderMaterial;
        }

        virtual public float CalculateDistance(Vector3 hitPoint, float deltaTime)
        {
            CalculatedDistance = RayMarcher.MAX_DIST;
            return CalculatedDistance;
        }
    }

    partial class RayMarcher
    {
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
        const float DEG_TO_RAD = 0.017453292519943295769236907684886f;
        const float M_PI = 3.14159265358979323846f;
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

        //ALL OBJECTS TO BE RENDERED
        public List<RenderObject> m_RenderableObjects;


        public RayMarcher(int width, int height)
        {
            m_ScrWidth = width;
            m_ScrHeight = height;

            Buffer.Initialize((short)m_ScrWidth, (short)m_ScrHeight,6);


            
            m_AspectRatio = (float)m_ScrWidth / (float)m_ScrHeight;
            m_Fov = 80.0f * DEG_TO_RAD;
            m_FovDist = (float)Math.Tan(m_Fov);
           // float test =  0.5f * M_PI / (180.0f * DEG_TO_RAD);

            m_RenderableObjects = new List<RenderObject>();

            Input.ev_KeyPressed += KeyPress;
        }
        Vector2 m_ViewerPos = new Vector2(3.0f, 1.0f);
        Vector2 m_ViewerDir = new Vector2(0.0f, 1.0f);
        void KeyPress(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.LeftArrow)
            {
                Vector3 tmp = new Vector3(m_ViewerDir.X, m_ViewerDir.Y, 0.0f) * Matrix3.CreateRotationZ(-0.1f);
                m_ViewerDir = Vector2.Normalize(tmp.Xy);
            }
            else if (keyInfo.Key == ConsoleKey.RightArrow)
            {
                Vector3 tmp = new Vector3(m_ViewerDir.X, m_ViewerDir.Y, 0.0f) * Matrix3.CreateRotationZ(0.1f);
                m_ViewerDir = Vector2.Normalize(tmp.Xy);
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

                         if (column == m_ScrWidth - 1) resetEvent.Set();
                     }), new object[] { x });
                }

                //Thread.Sleep(10);
                resetEvent.WaitOne();
                // Cycle counter hack :D
                m_TotalTime += FrameTimer.GetDeltaTime() * 1.2f;
                Buffer.Swap();
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
            dir *= Matrix3.CreateRotationZ(px);
            dir = Vector3.Normalize(dir);



            float t = 0.0f;
            const float stp = 0.05f;
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
            intensity *= intensity;

            for (int y = 0; y < m_ScrHeight; ++y)
            {

                float pixelY = -(y - m_ScrHeight / 2);
                float py = pixelY / ((float)m_ScrHeight) / 2.0f;
                py *= m_Fov*2.0f;
                ProduceShadedColor(out char ceilChar, out short ceilCol, Math.Abs(py), (short)COLOUR.FG_BLUE, (short)COLOUR.BG_BLUE);

                if (hit)
                {

                    ProduceShadedColor(out char wallChar, out short wallCol, intensity, FOREGROUND_INTENSITY, BACKGROUND_INTENSITY);
                    float rayFract = (ray.X - (float)Math.Floor(ray.X) - (ray.Y - (float)Math.Floor(ray.Y)));
                    if(Math.Abs(rayFract) < 0.05f || Math.Abs(rayFract) > 0.95f)
                    {
                        wallChar = (char)Block.Strong;
                        wallCol = 0;
                    }

                    if (py > ceiling) //draw ceiling
                    {
                        
                        Buffer.AddAsync(ceilChar, ceilCol, x, y);
                    }
                    else if (py < floorp)
                    {
                        Buffer.AddAsync((char)Block.Weak, 0x0000 | 0x0002, x, y);
                    }
                    else
                    {
                        Buffer.AddAsync(wallChar, wallCol, x, y);
                    }

                }
                else
                {

                    if (py > ceiling) Buffer.AddAsync(ceilChar, ceilCol, x, y);
                    else if (py < floorp) Buffer.AddAsync((char)Block.Weak, 0x0000 | 0x0002, x, y);
                    else Buffer.AddAsync((char)Block.Weak, 0x0000 | 0x0000, x, y);
                }


            }

        }

       







        private float CalculateShortestDistance(ref Ray ray, float start, float end, out int hitIndex)
        {
            float depth = start;
            hitIndex = 0;
            for (int i = 0; i < MAX_MARCHING_STEPS; ++i)
            {
                float dist = SceneSDF(ray.Orgin + depth * ray.Direction, false, out hitIndex);
                if (dist < EPSILON) // hit
                {
                    return depth;
                }
                depth += dist;
                if (depth >= end) // overrun
                {
                    return end;
                }
            }
            return end;
        }


        // Shortes distance for single hit in the scene - GIVES THE NEAREST HIT
        float SceneSDF(Vector3 rayHit, bool normalPass, out int hitIndex)
        {
            hitIndex = 0;
            //Shortest distance total - ALWAYS WILL BE THE NEAREST HIT!
            float nearestHit = MAX_DIST + 10; // 10000 so the next union will always override it
            if (m_RenderableObjects.Count == 0) return nearestHit; //way faaar :D
                                                                   // rayHit += new Vector3(0, 0, (float)Math.Sin(CycleCount)*10);
            rayHit *= Matrix3.CreateRotationY((float)Math.Sin(TotalTime * 0.8) * 0.06f);
            // rayHit *= Matrix3.CreateRotationX((float)Math.Cos(CycleCount*0.2) * 0.1f);
            for (int i = 0; i < m_RenderableObjects.Count; ++i)
            {
                float currentDist = m_RenderableObjects[i].CalculateDistance(rayHit, FrameTimer.GetDeltaTime());
                nearestHit = Op_Union(nearestHit, currentDist);

                if (!normalPass)
                {
                    if (nearestHit == currentDist)
                    {
                        // m_FisrtHitIndex = i;
                        hitIndex = i;
                    }
                }
            }

            return nearestHit;
        }



        Vector3 EstimateNormalRM(Vector3 p)
        {
            int dummy;
            return
               new Vector3(
                SceneSDF(new Vector3(p.X + EPSILON, p.Y, p.Z), true, out dummy) - SceneSDF(new Vector3(p.X - EPSILON, p.Y, p.Z), true, out dummy),
                SceneSDF(new Vector3(p.X, p.Y + EPSILON, p.Z), true, out dummy) - SceneSDF(new Vector3(p.X, p.Y - EPSILON, p.Z), true, out dummy),
                SceneSDF(new Vector3(p.X, p.Y, p.Z + EPSILON), true, out dummy) - SceneSDF(new Vector3(p.X, p.Y, p.Z - EPSILON), true, out dummy)
                ).Normalized();
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




        /* DISTANCE PRIMITIVES */

        static public float DistanceSphere(Vector3 p, float s)
        {

            float ret = p.Length - s;
            return ret;
        }

        static public float DistanceBox(Vector3 p, Vector3 b)
        {
            Vector3 d = Helper.Abs(p);
            d -= b;
            return Helper.Min(Helper.Max(d.X, Helper.Max(d.Y, d.Z)), 0.0f) + (Helper.Max(d, 0.0f)).Length;
        }

        static public float DistanceTorus(Vector3 p, Vector2 t)
        {
            Vector2 q = new Vector2(p.Xz.Length - t.X, p.Y);
            return q.Length - t.Y;
        }


        /*  OPERATIONS */
        static public float Op_Union(float d1, float d2)
        {
            return Helper.Min(d1, d2);
        }


        /* COLOR INTRNSITY CREATOR FOR LIGHT */
        //TO DO: NEEDS A BETTER WEIGHTING
        void ComputeColourChar(out char block, out short bitCol, float intensity, short bColor, short fColor)
        {
            block = (char)Block.Solid;
            bitCol = 0;

            block = (char)Block.Solid;
            bitCol = (short)(bColor|fColor);
            intensity = Helper.Min(intensity, 1.0f) - 0.01f;
            int index = (int)(intensity * 4.0f);


            block = (char)BLOCKS.BLOCK_ARR[index];
            bitCol = (short)(bColor | fColor);

        }


        /* COLOR INTRNSITY CREATOR FOR LIGHT */
        //TO DO: NEEDS A BETTER WEIGHTING
        void ProduceShadedColor(out char block, out short bitCol, float intensity, short bColor, short fColor)
        {
            block = (char)Block.Solid;
            bitCol = 0;
            if (intensity > 0 && intensity < 0.3)
            {
                block = (char)Block.Weak;
                bitCol = bColor;
            }
            else if (intensity > 0.3 && intensity < 0.5)
            {
                block = (char)Block.Middle;
                bitCol = bColor;
            }
            else if (intensity > 0.5 && intensity <= 0.7)
            {
                block = (char)Block.Strong;
                bitCol = bColor;
            }

            else if (intensity > 0.7 && intensity <= 0.9)
            {
                block = (char)Block.Weak;
                bitCol = (short)(fColor | bColor);
            }


            else if (intensity > 0.9 && intensity <= 0.98)
            {
                block = (char)Block.Middle;
                bitCol = (short)(fColor | bColor);
            }
            else if (intensity > 0.98)
            {
                block = (char)Block.Strong;
                bitCol = (short)(fColor | bColor);
            }


            //else if (intensity > 0.91)
            //{
            //    block = (char)Block.Solid;
            //    bitCol = (short)(FOREGROUND_INTENSITY);
            //}

        }

    }



}
