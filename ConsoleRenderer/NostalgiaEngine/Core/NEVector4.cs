using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{
    public struct NEVector4
    {
        public float[] Data { get { return new float[] { X, Y, Z, W }; } }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
        public NEVector2 XY { get { return new NEVector2(X, Y); } }
        public NEVector2 XZ { get { return new NEVector2(X, Z); } }
        public NEVector2 YZ { get { return new NEVector2(Y, Z); } }
        public float Length { get { return CalculateLength(this); } }
        public NEVector4 Normalized { get { return Normalize(this); } }


        static public readonly NEVector4 Up =  new NEVector4(0.0f, 1.0f, 0.0f);
        static public readonly NEVector4 Down = new NEVector4(0.0f, -1.0f, 0.0f);
        static public readonly NEVector4 Left = new NEVector4(-1.0f, 0.0f, 0.0f);
        static public readonly NEVector4 Right = new NEVector4(1.0f, 0.0f, 0.0f);
        static public readonly NEVector4 Forward = new NEVector4(0.0f, 0.0f, 1.0f);
        static public readonly NEVector4 Back = new NEVector4(0.0f, 0.0f, -1.0f);

        static public readonly NEVector4 Zero = new NEVector4(0.0f, 0.0f, 0.0f);
        static public readonly NEVector4 One = new NEVector4(1.0f, 1.0f, 1.0f, 1.0f);


        public override string ToString()
        {
            return X.ToString() + ", " + Y.ToString() + ", " + Z.ToString()  + ", " + W.ToString();
        }

        public NEVector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public NEVector4(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
            W = 0.0f;
        }

        public static bool Compare(NEVector4 lhs, NEVector4 rhs)
        {
            return ((lhs.X == rhs.X) && (lhs.Y == rhs.Y) && (lhs.Z == rhs.Z) && (lhs.W == rhs.W));
        }

        public static float CalculateLength(NEVector4 v)
        {
            return (float)Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z + v.W * v.W);
        }

        public static NEVector4 Normalize(NEVector4 v)
        {
            float l = 1.0f/CalculateLength(v);
            return new NEVector4(v.X * l, v.Y * l, v.Z * l, v.W * l);
        }

        /// <summary>
        /// Checks if both coplanar vectors left and right are pointing left and right on the plane with the normal "up" 
        /// </summary>
        /// <param name="left">V</param>
        /// <param name="right"></param>
        /// <param name="up"></param>
        /// <returns>True if left vector is pointing left and right vector is pointing right, otherwise false </returns>
        public static bool CompareLeft(NEVector4 left, NEVector4 right, NEVector4 up)
        {
            up.W = 0;
            NEVector4 dir = NEVector4.Cross3(left, right);
            float projected = NEVector4.Dot(dir, up);
            return (projected > 0.0f);

        }

        public static float Dot(NEVector4 v1, NEVector4 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z + v1.W * v2.W;
        }

        public static float Dot3(NEVector4 v1, NEVector4 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static NEVector4 Cross3(NEVector4 a, NEVector4 b)
        {
            float x = a.Y * b.Z - a.Z * b.Y;
            float y = a.Z * b.X - a.X * b.Z;
            float z = a.X * b.Y - a.Y * b.X;
            return new NEVector4(x, y, z, 0.0f);
        }

        public static NEVector4 Abs(NEVector4 a)
        {
            return new NEVector4(Math.Abs(a.X), Math.Abs(a.Y), Math.Abs(a.Z), Math.Abs(a.W));
        }

        public static NEVector4 Lerp(NEVector4 a, NEVector4 b, float t)
        {
            return a + (b - a) * t;
        }


        public static NEVector4 LerpInvW(NEVector4 a, NEVector4 b, float t)
        {
            a.W = 1.0f / a.W;
            b.W = 1.0f / b.W;
           NEVector4 output =  a + (b - a) * t;
            output.W = 1.0f / output.W;
            return output;
        }

        static public NEVector4 operator +(NEVector4 lhs, NEVector4 rhs)
        {
            return new NEVector4(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z, lhs.W + rhs.W);
        }


        static public NEVector4 operator -(NEVector4 lhs, NEVector4 rhs)
        {
            return new NEVector4(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z, lhs.W - rhs.W);
        }

        static public NEVector4 operator -(NEVector4 lhs)
        {
            return new NEVector4(-lhs.X , -lhs.Y , -lhs.Z , -lhs.W);
        }

        static public NEVector4 operator *(NEVector4 lhs, float rhs)
        {
            return new NEVector4(lhs.X * rhs, lhs.Y * rhs, lhs.Z * rhs, lhs.W * rhs);
        }

        static public NEVector4 operator *(NEVector4 lhs, NEVector4 rhs)
        {
            return new NEVector4(lhs.X * rhs.X, lhs.Y * rhs.Y, lhs.Z * rhs.Z, lhs.W * rhs.W);
        }

        static public NEVector4 operator /(NEVector4 lhs, float rhs)
        {
            return new NEVector4(lhs.X / rhs, lhs.Y / rhs, lhs.Z / rhs, lhs.W / rhs);
        }

        static public NEVector4[] Axes = new NEVector4[] { Left, Right, Up, Down, Forward, Back };
    }

     public enum Axis { X = 0, Y = 1, Z = 2 }
}
