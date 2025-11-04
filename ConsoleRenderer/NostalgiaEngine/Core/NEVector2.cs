using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct NEVector2
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Length { get { return CalculateLength(this); } }
        public float LengthSqared { get { return X * X + Y * Y; } }
        public NEVector2 Normalized { get { return Normalize(this); } }

        public override string ToString()
        {
            return X.ToString() + "\n" + Y.ToString();
        }

        public NEVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float CalculateLength(NEVector2 v)
        {
            return (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static NEVector2 Normalize(NEVector2 v)
        {
            float l = CalculateLength(v);
            return new NEVector2(v.X / l, v.Y / l);
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(NEVector2 v1, NEVector2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(float x1, float y1, float x2, float y2)
        {
            return x1 * x2 + y1 * y2;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static NEVector2 FindNormal(NEVector2 v)
        {
            float tx = v.X;
            v.X = -v.Y;
            v.Y = tx;
            return v;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public void RotateClockWise(ref NEVector2 v, float theta)
        {
            float s = (float)Math.Sin(theta);
            float c = (float)Math.Cos(theta);

            float tx = v.X * c - v.Y * s;
            float ty = v.X * s + v.Y * c;
            v.X = tx;
            v.Y = ty;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static NEVector2 Lerp(NEVector2 a, NEVector2 b, float t)
        {
            return a + (b - a) * t;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public NEVector2 operator +(NEVector2 lhs, NEVector2 rhs)
        {
            return new NEVector2(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public NEVector2 operator +(NEVector2 lhs, NEPoint rhs)
        {
            return new NEVector2(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public NEVector2 operator -(NEVector2 lhs, NEVector2 rhs)
        {
            return new NEVector2(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public NEVector2 operator -(NEVector2 lhs, NEPoint rhs)
        {
            return new NEVector2(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public NEVector2 operator *(NEVector2 lhs, float rhs)
        {
            return new NEVector2(lhs.X * rhs, lhs.Y * rhs);
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public NEVector2 operator /(NEVector2 lhs, float rhs)
        {
            return new NEVector2(lhs.X / rhs, lhs.Y / rhs);
        }

    }
}
