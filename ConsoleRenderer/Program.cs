using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Runtime.InteropServices;
namespace ConsoleRenderer
{
    class Sphere:RenderObject
    {
      

        public Sphere(Material material):base(material)
        {
        }
        override public float CalculateDistance(Vector3 hitPoint, float deltaTime)
        {
            base.CalculateDistance(hitPoint,deltaTime);
           return  RayMarcher.Op_Union(CalculatedDistance,
                RayMarcher.DistanceSphere(hitPoint -
                new Vector3((float)Math.Sin(RayMarcher.TotalTime * 0.4) * 3 + 2,
                (float)Math.Sin(RayMarcher.TotalTime * 6) * 1,
                7 + (float)Math.Cos(RayMarcher.TotalTime * 0.4) * 6),
                1.0f));

        }
    }

    class CeilingFloor : RenderObject
    {


        public CeilingFloor(Material material) : base(material)
        {
        }
        override public float CalculateDistance( Vector3 hitPoint, float deltaTime)
        {
           
            base.CalculateDistance(hitPoint,deltaTime);

            float ret = CalculatedDistance;
            ret = RayMarcher.Op_Union(ret, hitPoint.Y + 10.0f);
            ret = RayMarcher.Op_Union(ret, -hitPoint.Y + 10.0f);
            return ret;

        }
    }

    class Box : RenderObject
    {


        public Box(Material material) : base(material)
        {
        }
        override public float CalculateDistance(Vector3 hitPoint, float deltaTime)
        {
           
            base.CalculateDistance(hitPoint,deltaTime);
            return
                RayMarcher.Op_Union(
                    CalculatedDistance, 
                    RayMarcher.DistanceBox((hitPoint - new Vector3(2.5f, 0, 10)) 
                    * Matrix3.CreateRotationY(RayMarcher.TotalTime*1f)
                    * Matrix3.CreateRotationX(RayMarcher.TotalTime*1f)
                    ,
                    new Vector3(1,1,1)));



        }
    }

    class SideWalls : RenderObject
    {


        public SideWalls(Material material) : base(material)
        {
        }
        override public float CalculateDistance(Vector3 hitPoint, float deltaTime)
        {
            
            base.CalculateDistance(hitPoint, deltaTime);
            float ret = CalculatedDistance;
            ret = RayMarcher.Op_Union(ret, hitPoint.X + 15.0f);
            ret= RayMarcher.Op_Union(ret, -hitPoint.X + 15.0f);
            return ret;

        }
    }


    class Torus : RenderObject
    {


        public Torus(Material material) : base(material)
        {
        }
        override public float CalculateDistance(Vector3 hitPoint, float deltaTime)
        {

            hitPoint -= new Vector3(-3.5f, 2, 30+ 20.0f*(float)Math.Sin(RayMarcher.TotalTime));

           hitPoint *= Matrix3.CreateRotationY(RayMarcher.TotalTime * 2.2f);
            hitPoint *= Matrix3.CreateRotationZ(RayMarcher.TotalTime * 2.4f);
            base.CalculateDistance(hitPoint, deltaTime);
            float ret = CalculatedDistance;
            ret = RayMarcher.Op_Union(ret, RayMarcher.DistanceTorus(hitPoint, new Vector2(2,1)));
           // ret = RayMarcher.Op_Union(ret, -hitPoint.X + 15.0f);
            return ret;

        }
    }

    class Program
    {
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_VSCROLL = 0xF070;
        public const int SC_HSCROLL = 0xF080;
        public const int SC_SIZE = 0xF000;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        static void Main(string[] args)
        {
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);

            LitMaterial greenLitMat;
            greenLitMat.ColorA = RayMarcher.BACKGROUND_GREEN;
            greenLitMat.ColorB = RayMarcher.FOREGROUND_GREEN;

            LitMaterial cyanLitMat;
            cyanLitMat.ColorA = RayMarcher.BACKGROUND_CYAN;
            cyanLitMat.ColorB = RayMarcher.FOREGROUND_CYAN;

            LitMaterial redLitMat;
            redLitMat.ColorA = RayMarcher.BACKGROUND_RED;
            redLitMat.ColorB = RayMarcher.FOREGROUND_RED;

            LitMaterial yellowLitMat;
            yellowLitMat.ColorA = RayMarcher.BACKGROUND_YELLOW;
            yellowLitMat.ColorB = RayMarcher.FOREGROUND_YELLOW;

            LitMaterial grayLitMat;
            grayLitMat.ColorA = RayMarcher.BACKGROUND_WHITE;
            grayLitMat.ColorB = RayMarcher.FOREGROUND_WHITE;

            UnlitMaterial unlitMat;
            unlitMat.ColorA = 0 ;
            unlitMat.ColorB = 0;
            unlitMat.BlockType = Block.Weak;

            Input.Start();
           
            RayMarcher rm = new RayMarcher(300, 120);
        

            //Sphere sphere = new Sphere(greenLitMat);
            //rm.m_RenderableObjects.Add(sphere);

            //CeilingFloor plane = new CeilingFloor(redLitMat);
            //rm.m_RenderableObjects.Add(plane);

            //Box cube = new Box(cyanLitMat);
            //rm.m_RenderableObjects.Add(cube);

            //SideWalls walls = new SideWalls(grayLitMat);
            //rm.m_RenderableObjects.Add(walls);

            //Torus torus = new Torus(redLitMat);
            //rm.m_RenderableObjects.Add(torus);


            rm.RenderLoop();
            Input.Stop();

        }
    }
}
