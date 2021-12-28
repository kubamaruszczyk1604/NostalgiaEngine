using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
namespace ConsoleRenderer.Core
{

    public class CGHelper
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

        static public float Sign(float a)
        {
            return (float)Math.Sign(a);
        }

        static public Vector2 FindNormal(Vector2 p1, Vector2 p2)
        {
            Vector2 dir = p2 - p1;
            float tx = dir.X;
            dir.X = -dir.Y;
            dir.Y = tx;
            return dir;
        }

        static public Vector2 FindNormal(Vector2 dir)
        {
            float tx = dir.X;
            dir.X = -dir.Y;
            dir.Y = tx;
            return dir;
        }

        static public float Dot(Vector2 a, Vector2 b)
        {
            return Vector2.Dot(a, b);
        }

        /// <summary>
        /// Finds terms "a" and "c" of the straight line (y = ax + c) passing through points p1 and p2
        /// </summary>
        /// <param name="p1">point 1</param>
        /// <param name="p2">point 2</param>
        /// <param name="a">gradient</param>
        /// <param name="c">intersection height with y axis </param>
        static public void FindLineEquation(Vector2 p1, Vector2 p2, out float a, out float c)
        {
            //gradient
            a = (p2.Y - p1.Y) / ((p2.X - p1.X)+0.01f);
            // y = ax + c, so c = y -ax
            c = p1.Y - p1.X * a; // p2.Y - p2.X*a would also be valid
           
        }

        public static bool InTriangle(Vector2 p, Vector2 A,Vector2 B, Vector2 C)
        {
            float a = Dot((p - A), FindNormal(A, B));
            float b = Dot((p - B), FindNormal(B, C));
            float c = Dot((p - C), FindNormal(C, A));

            if (Sign(a) + Sign(b) + Sign(c) <= -3.0)
                return true;
            return false;
        }

        public static bool InRectangle(Vector2 p, Vector2 orgin, float W, float H)
        {
            return ((p.X > orgin.X) && (p.X < (orgin.X + W)) &&
                (p.Y > orgin.Y) && (p.Y < (orgin.Y + H)));
        }

        public static float DistToLine(Vector2 p, float a, float c)
        {
            if (a == 0) a = 0.001f;
            //1. find perpendicular line that passes thourgh p
            float aPerpendicular = -1.0f / a;
            float cPerpendicular = p.Y - aPerpendicular * p.X;

            //2. Find intersection point
            float interX = (c - cPerpendicular) / (aPerpendicular - a);
            Vector2 intersection = new Vector2(interX, a * interX + c);

            //3. Distance between p and intersection is on the perp
            return (p - intersection).LengthFast;
        }

        public static float DistToLine(Vector2 p ,Vector2 A, Vector2 B)
        {
            FindLineEquation(A, B, out float a, out float c);
            return DistToLine(p, a, c);
        }

        public static bool IsOnLine(Vector2 p, Vector2 A, Vector2 B, float thickness = 1.0f)
        {
            float lowX = A.X < B.X ? A.X : B.X;
            float hiX = A.X > B.X ? A.X : B.X;
            float lowY = A.Y < B.Y ? A.Y : B.Y;
            float hiY = A.Y > B.Y ? A.Y : B.Y;

            if ((p.X >= lowX) && (p.X <= hiX) &&
                 (p.Y >= lowY) && (p.Y <= hiY))
            {
                return  DistToLine(p, A, B) < thickness ? true : false;
            }
            return false;
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



        static public void Rotate(ref Vector2 v, float theta)
        {
            float s = (float)Math.Sin(theta);
            float c = (float)Math.Cos(theta);
            
            float tx  = v.X * c - v.Y * s;
            float ty = v.X * s + v.Y * c;
            v.X = tx;
            v.Y = ty;
        }


    }
}
