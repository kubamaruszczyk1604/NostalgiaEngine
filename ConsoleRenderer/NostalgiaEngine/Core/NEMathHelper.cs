using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace NostalgiaEngine.Core
{

    public class NEMath
    {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public float Min(float a, float b)
        {
            return (a < b) ? a : b;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public float Max(float a, float b)
        {
            return (a > b) ? a : b;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public float Clamp(float val, float low, float high)
        {
            val = (val < low) ? low : val;
            val = (val > high) ? high : val;
            return val;

            //if (val < low) return low;
            //if (val > high) return high;
            //return val;

        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public float Abs(float a)
        {
            return Math.Abs(a);
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public int Abs(int a)
        {
            return Math.Abs(a);
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public int Min(int a, int b)
        {
            return (a < b) ? a : b;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public int Max(int a, int b)
        {
            return (a > b) ? a : b;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public int Clamp(int val, int low, int high)
        {
            val = (val < low) ? low : val;
            val = (val >= high) ? high : val;
            return val;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public uint Clamp(uint val, uint low, uint high)
        {
            val = (val < low) ? low : val;
            val = (val > high) ? high : val;

            return val;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public float Pow(float number, float power)
        { 
            return (float)Math.Pow(number, power);
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public float Sin(float a)
        {
            return  (float)Math.Sin(a);
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public float Cos(float a)
        {
            return (float)Math.Cos(a);
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public float Tan(float a)
        {
            return (float)Math.Tan(a);
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public float Sign(float a)
        {
            return (float)Math.Sign(a);
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public float Frac(float t)
		{
			int i = (int)t;
			if (t < i) i--;
			return t - i;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public NEVector2 FindNormal(NEVector2 p1, NEVector2 p2)
        {
            NEVector2 dir = p2 - p1;
            float tx = dir.X;
            dir.X = -dir.Y;
            dir.Y = tx;
            return dir;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public NEVector2 FindNormal(NEVector2 dir)
        {
            float tx = dir.X;
            dir.X = -dir.Y;
            dir.Y = tx;
            return dir;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public bool FindRayEquation(NEVector4 p0, NEVector4 p1, out NEVector4 dir, out float length)
        {
            length = 0.0f;
            dir = NEVector4.Zero;
            if(NEVector4.Compare(p0,p1))
            {
                return false;
            }
            NEVector4 diff = p1 - p0;
            length = diff.LengthFast;
			float recipLen = 1.0f / length;
			dir = diff * recipLen;// diff.Normalized;
            
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

        public static float DistToLine(NEVector4 p, NEVector4 l0, NEVector4 l1)
        {
           return NEVector4.Cross3(p - l0, l1 - l0).Length / (l1 - l0).Length;
        }

		public static bool AABBInsidePlane(NEAABB aabb, NEPlane plane)
		{
			NEVector4 v = new NEVector4();

			// Pick most positive AABB vertex relative to normal
			v.X = (plane.N.X >= 0) ? aabb.Max.X : aabb.Min.X;
			v.Y = (plane.N.Y >= 0) ? aabb.Max.Y : aabb.Min.Y;
			v.Z = (plane.N.Z >= 0) ? aabb.Max.Z : aabb.Min.Z;

			NEVector4 diff = v  - plane.P;

			float dot = NEVector4.Dot3(diff, plane.N);
			return dot >= 0.0f;
		}

		public static void BuildViewFrustumPlanes(float fovRad, float aspect, float near, float far, ref NEPlane[] planes)
		{
			float t = Tan(fovRad * 0.5f);
			float sx = t * aspect;
			float sy = t;

			planes[0].P = new NEVector4(0.0f, 0.0f, 0.0f, 1.0f);
			planes[0].N = new NEVector4(1.0f, 0.0f, sx, 0.0f);

			planes[1].P = new NEVector4(0.0f, 0.0f, 0.0f, 1.0f);
			planes[1].N = new NEVector4(-1.0f, 0.0f, sx, 0.0f);

			planes[2].P = new NEVector4(0.0f, 0.0f, 0.0f, 1.0f);
			planes[2].N = new NEVector4(0.0f, 1.0f, sy ,0.0f);

			planes[3].P = new NEVector4(0.0f, 0.0f, 0.0f, 1.0f);
			planes[3].N = new NEVector4(0.0f, -1.0f, sy, 0.0f);

			planes[4].P = new NEVector4(0.0f, 0.0f, near, 1.0f);
			planes[4].N = new NEVector4(0.0f, 0.0f, 1.0f, 0.0f);

			planes[5].P = new NEVector4(0.0f, 0.0f, far, 1.0f);
			planes[5].N = new NEVector4(0.0f, 0.0f, -1.0f, 0.0f);

		}

		public static void BuildViewFrustumPlanes(float fovRad, float aspect, ref NEPlane[] planes)
		{

			float t = Tan(fovRad * 0.5f);
			float sx= t * aspect;
			float sy = t;

			planes[0].P = new NEVector4(0.0f, 0.0f, 0.0f, 1.0f);
			planes[0].N = new NEVector4(1.0f, 0.0f, sx, 0.0f);

			planes[1].P = new NEVector4(0.0f, 0.0f, 0.0f, 1.0f);
			planes[1].N = new NEVector4(-1.0f, 0.0f, sx, 0.0f);

			planes[2].P = new NEVector4(0.0f, 0.0f, 0.0f, 1.0f);
			planes[2].N = new NEVector4(0.0f, 1.0f, sy, 0.0f);

			planes[3].P = new NEVector4(0.0f, 0.0f, 0.0f, 1.0f);
			planes[3].N = new NEVector4(0.0f, -1.0f, sy, 0.0f);

		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public NEVector2 Abs(NEVector2 a)
        {
            a.X = Math.Abs(a.X);
            a.Y = Math.Abs(a.Y);

            return a;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public void Rotate(ref NEVector2 v, float theta)
        {
            float s = (float)Math.Sin(theta);
            float c = (float)Math.Cos(theta);
            
            float tx  = v.X * c - v.Y * s;
            float ty = v.X * s + v.Y * c;
            v.X = tx;
            v.Y = ty;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public void Swap(ref float a, ref float b)
        {
            float temp = a;
            a = b;
            b = temp;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Swap(ref int a, ref int b)
        {
            int tmp = a;
            a = b;
            b = tmp;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Swap(ref NEVector2 a, ref NEVector2 b)
        {
            NEVector2 temp = a;
            a = b;
            b = a;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe float InvSqrtFast(float number)
		{
			const float threehalfs = 1.5f;

			float x2 = number * 0.5f;
			float y = number;

			int i = *(int*)&y;
			i = 0x5f3759df - (i >> 1); 
			y = *(float*)&i;

			y = y * (threehalfs - (x2 * y * y));

			return y;
		}

		public static bool CheckRayTriangleHit(NEVector4 O, NEVector4 D,
			NEVector4 V0, NEVector4 V1, NEVector4 V2,
			float tMin = 1e-4f,
			float tMax = float.PositiveInfinity,
			bool cullBackfaces = false)
		{
			const float EPS = 1e-8f;

			NEVector4 e1 = V1 - V0;
			NEVector4 e2 = V2 - V0;

			NEVector4 p = NEVector4.Cross3(D, e2);
			//p.W = 1;
			float det = NEVector4.Dot3(e1, p);

			if (cullBackfaces)
			{
				if (det < EPS) return false;
			}
			else
			{
				if (Math.Abs(det) < EPS) return false;
			}

			float invDet = 1.0f / det;

			NEVector4 tvec = O - V0;
			float u = NEVector4.Dot3(tvec, p) * invDet;
			if (u < 0.0f || u > 1.0f) return false;

			NEVector4 q = NEVector4.Cross3(tvec, e1);
			//q.W = 1.0f;
			float v = NEVector4.Dot3(D, q) * invDet;
			if (v < 0.0f || (u + v) > 1.0f) return false;

			float t = NEVector4.Dot3(e2, q) * invDet;

			return (t >= tMin && t <= tMax);
		
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
