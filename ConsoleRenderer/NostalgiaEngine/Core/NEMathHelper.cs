﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NostalgiaEngine.Core
{

    public class NEMathHelper
    {

        static public float Min(float a, float b)
        {
            return (a < b) ? a : b;
        }

        static public float Max(float a, float b)
        {
            return (a > b) ? a : b;
        }

        static public float Clamp(float val, float low, float high)
        {
            val = (val < low) ? low : val;
            val = (val > high) ? high : val;
            return val;

            //if (val < low) return low;
            //if (val > high) return high;
            //return val;

        }

        static public float Abs(float a)
        {
            return Math.Abs(a);
        }

        static public int Abs(int a)
        {
            return Math.Abs(a);
        }

        static public int Min(int a, int b)
        {
            return (a < b) ? a : b;
        }

        static public int Max(int a, int b)
        {
            return (a > b) ? a : b;
        }

        static public int Clamp(int val, int low, int high)
        {
            val = (val < low) ? low : val;
            val = (val >= high) ? high : val;
            return val;
        }

        static public uint Clamp(uint val, uint low, uint high)
        {
            val = (val < low) ? low : val;
            val = (val > high) ? high : val;

            return val;
        }

        static public float Pow(float number, float power)
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

        static public float Tan(float a)
        {
            return (float)Math.Tan(a);
        }

        static public float Sign(float a)
        {
            return (float)Math.Sign(a);
        }

        static public NEVector2 FindNormal(NEVector2 p1, NEVector2 p2)
        {
            NEVector2 dir = p2 - p1;
            float tx = dir.X;
            dir.X = -dir.Y;
            dir.Y = tx;
            return dir;
        }

        static public NEVector2 FindNormal(NEVector2 dir)
        {
            float tx = dir.X;
            dir.X = -dir.Y;
            dir.Y = tx;
            return dir;
        }


        /// <summary>
        /// Finds terms "a" and "c" of the straight line (y = ax + c) passing through points p1 and p2
        /// </summary>
        /// <param name="p1">point 1</param>
        /// <param name="p2">point 2</param>
        /// <param name="a">gradient</param>
        /// <param name="c">intersection height with y axis </param>
        static public void Find2DLineEquation(NEVector2 p1, NEVector2 p2, out float a, out float c)
        {
            //gradient
            float den = p2.X - p1.X;
            //den = NEMathHelper.Abs(den) >= 0.001f ? den : 0.001f;
            a = (p2.Y - p1.Y) / ((den));
            // y = ax + c, so c = y -ax
            c = p1.Y - p1.X * a; // p2.Y - p2.X*a would also be valid
           
        }

        static public bool FindRayEquation(NEVector4 p0, NEVector4 p1, out NEVector4 dir, out float length)
        {
            length = 0.0f;
            dir = NEVector4.Zero;
            if(NEVector4.Compare(p0,p1))
            {
                return false;
            }
            NEVector4 diff = p1 - p0;
            length = diff.Length;
            dir = diff.Normalized;
            
            return true;

        }

        public static bool InTriangle(NEVector2 p, NEVector2 A, NEVector2 B, NEVector2 C)
        {
            float a = NEVector2.Dot((p - A), FindNormal(A, B));
            float b = NEVector2.Dot((p - B), FindNormal(B, C));
            float c = NEVector2.Dot((p - C), FindNormal(C, A));

            return (Sign(a) + Sign(b) + Sign(c) <= -3.0);
            //    return true;
            //return false;
        }

        public static bool InRectangle(NEVector2 p, NEVector2 orgin, float W, float H)
        {
            return ((p.X > orgin.X) && (p.X < (orgin.X + W)) &&
                (p.Y > orgin.Y) && (p.Y < (orgin.Y + H)));
        }

        public static bool InRectangle(NEPoint p, NEPoint orgin, int W, int H)
        {
            return ((p.X > orgin.X) && (p.X < (orgin.X + W)) &&
                (p.Y > orgin.Y) && (p.Y < (orgin.Y + H)));
        }

        public static float DistToLine(NEVector2 p, float a, float c)
        {
            if (a == 0) a = 0.001f;
            //1. find perpendicular line that passes thourgh p
            float aPerpendicular = -1.0f / a;
            float cPerpendicular = p.Y - aPerpendicular * p.X;

            //2. Find intersection point
            float interX = (c - cPerpendicular) / (aPerpendicular - a);
            NEVector2 intersection = new NEVector2(interX, a * interX + c);

            //3. Distance between p and intersection is on the perp
            return NEVector2.CalculateLength(p - intersection);
        }

        public static float DistToLine(NEVector2 p , NEVector2 A, NEVector2 B)
        {
            Find2DLineEquation(A, B, out float a, out float c);
            return DistToLine(p, a, c);
        }

        public static bool IsOnLine(NEVector2 p, NEVector2 A, NEVector2 B, float thickness = 1.0f)
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


        static public NEVector2 Abs(NEVector2 a)
        {
            a.X = Math.Abs(a.X);
            a.Y = Math.Abs(a.Y);

            return a;
        }

        static public void Rotate(ref NEVector2 v, float theta)
        {
            float s = (float)Math.Sin(theta);
            float c = (float)Math.Cos(theta);
            
            float tx  = v.X * c - v.Y * s;
            float ty = v.X * s + v.Y * c;
            v.X = tx;
            v.Y = ty;
        }


        static public void Swap(ref float a, ref float b)
        {
            float temp = a;
            a = b;
            b = temp;
        }

        private void Swap(ref int a, ref int b)
        {
            int tmp = a;
            a = b;
            b = tmp;
        }

        public void Swap(ref NEVector2 a, ref NEVector2 b)
        {
            NEVector2 temp = a;
            a = b;
            b = a;
        }

    }

    public struct PlaneIntersectionManifest
    {
       public NEVector4 RayDirection;
       public float RayLength;
       public float Magnitude;
       public float MagnitudeNormalized;
       public bool Intersected; 
    }
}
