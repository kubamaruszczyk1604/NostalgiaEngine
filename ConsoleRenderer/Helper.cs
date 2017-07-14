using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
namespace ConsoleRenderer
{
    class Helper
    {

        static public float Min(float a, float b)
        {
            if (a < b) return a;
            else return b;
        }

        static public float Max(float a, float b)
        {
            if (a > b) return a;
            else return b;

        }


        static public float Abs(float a)
        {
            return Math.Abs(a);
        }

        static public float Pow(double number, double power)
        {
            return (float)Math.Pow(number, power);
        }

        static public float Sin(float a)
        {
            return  (float)Math.Sin(a);
        }

        static public float Cos(float a)
        {
            return (float)Math.Cos(a);
        }

        static public Vector4 Abs(Vector4 a)
        {
            a.X = Math.Abs(a.X);
            a.Y = Math.Abs(a.Y);
            a.Z = Math.Abs(a.Z);
            a.W = Math.Abs(a.W);
            return a;
        }

        static public Vector3 Abs(Vector3 a)
        {
            a.X = Math.Abs(a.X);
            a.Y = Math.Abs(a.Y);
            a.Z = Math.Abs(a.Z);
            return a;
        }

        static public Vector2 Abs(Vector2 a)
        {
            a.X = Math.Abs(a.X);
            a.Y = Math.Abs(a.Y);

            return a;
        }

        static public Vector3 Max(Vector3 a, float b)
        {
            if (a.X < b) a.X = b;
            if (a.Y < b) a.Y = b;
            if (a.Z < b) a.Z = b;
            return a;
        }


    }
}
