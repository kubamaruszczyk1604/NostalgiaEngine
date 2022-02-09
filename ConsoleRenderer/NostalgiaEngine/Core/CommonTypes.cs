﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace NostalgiaEngine.Core
{

    [StructLayout(LayoutKind.Sequential)]
    public struct NERect
    {
        public short Left { get; set; }
        public short Top { get; set; }
        public short Right { get; set; }
        public short Bottom { get; set; }

        public NERect(short left, short top, short right, short bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public static NERect operator -(NERect lhs, NERect rhs)
        {
            return new NERect((short)(lhs.Left - rhs.Left), (short)(lhs.Top - rhs.Top), (short)(lhs.Right - rhs.Right), (short)(lhs.Bottom - rhs.Bottom));
        }
        public static NERect operator +(NERect lhs, NERect rhs)
        {
            return new NERect((short)(lhs.Left + rhs.Left), (short)(lhs.Top + rhs.Top), (short)(lhs.Right + rhs.Right), (short)(lhs.Bottom + rhs.Bottom));
        }
        public static NERect operator *(NERect lhs, int rhs)
        {
            return new NERect((short)(lhs.Left * rhs), (short)(lhs.Top * rhs), (short)(lhs.Right * rhs), (short)(lhs.Bottom * rhs));
        }

        public static NERect operator /(NERect lhs, int rhs)
        {
            return new NERect((short)(lhs.Left / rhs), (short)(lhs.Top / rhs), (short)(lhs.Right / rhs), (short)(lhs.Bottom / rhs));
        }

        public static NERect operator %(NERect lhs, int rhs)
        {
            return new NERect((short)(lhs.Left % rhs), (short)(lhs.Top % rhs), (short)(lhs.Right % rhs), (short)(lhs.Bottom % rhs));
        }

    }


    [StructLayout(LayoutKind.Sequential)]
    public struct NEPoint
    {
        public short X;
        public short Y;

        public NEPoint(short X, short Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public NEVector2 AsVector()
        {
            return new NEVector2(X, Y);
        }

        static public NEPoint operator +(NEPoint lhs, NEPoint rhs)
        {
            return new NEPoint((short)(lhs.X + rhs.X), (short)(lhs.Y + rhs.Y));
        }

        static public NEPoint operator -(NEPoint lhs, NEPoint rhs)
        {
            return new NEPoint((short)(lhs.X - rhs.X), (short)(lhs.Y - rhs.Y));
        }

        static public NEPoint operator *(NEPoint lhs, int rhs)
        {
            return new NEPoint((short)(lhs.X * rhs), (short)(lhs.Y * rhs));
        }

        static public NEPoint operator /(NEPoint lhs, int rhs)
        {
            return new NEPoint((short)(lhs.X / rhs), (short)(lhs.Y / rhs));
        }

        static public NEPoint operator %(NEPoint lhs, int rhs)
        {
            return new NEPoint((short)(lhs.X % rhs), (short)(lhs.Y % rhs));
        }

    };

    public struct NEVector2
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Length { get { return CalculateLength(this); } }
        public NEVector2 Normalized { get { return Normalize(this); } }

        public NEVector2(float x, float y)
        {
            X = x;
            Y = y;
        }


        public static float CalculateLength(NEVector2 v)
        {
            return (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
        }

        public static NEVector2 Normalize(NEVector2 v)
        {
            float l = CalculateLength(v);
            return new NEVector2(v.X / l, v.Y / l);
        }

        public static float Dot(NEVector2 v1, NEVector2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public static NEVector2 FindNormal(NEVector2 v)
        {
            float tx = v.X;
            v.X = -v.Y;
            v.Y = tx;
            return v;
        }

        static public void Rotate(ref NEVector2 v, float theta)
        {
            float s = (float)Math.Sin(theta);
            float c = (float)Math.Cos(theta);

            float tx = v.X * c - v.Y * s;
            float ty = v.X * s + v.Y * c;
            v.X = tx;
            v.Y = ty;
        }

        static public NEVector2 operator +(NEVector2 lhs, NEVector2 rhs)
        {
            return new NEVector2(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }

        static public NEVector2 operator +(NEVector2 lhs, NEPoint rhs)
        {
            return new NEVector2(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }

        static public NEVector2 operator -(NEVector2 lhs, NEVector2 rhs)
        {
            return new NEVector2(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }

        static public NEVector2 operator -(NEVector2 lhs, NEPoint rhs)
        {
            return new NEVector2(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }

        static public NEVector2 operator *(NEVector2 lhs, float rhs)
        {
            return new NEVector2(lhs.X * rhs, lhs.Y * rhs);
        }

        static public NEVector2 operator /(NEVector2 lhs, float rhs)
        {
            return new NEVector2(lhs.X / rhs, lhs.Y / rhs);
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NEConsoleColorDef
    {
        public uint ColMask;


        public NEConsoleColorDef(uint r, uint g, uint b)
        {
            r = NEMathHelper.Clamp(r, 0, 255);
            g = NEMathHelper.Clamp(g, 0, 255);
            b = NEMathHelper.Clamp(b, 0, 255);
            ColMask = r | (g << 8) | (b << 16);
        }

        public NEConsoleColorDef(float r, float g, float b)
        {
            uint ri = (uint)NEMathHelper.Clamp(r * 255.0f, 0, 255.0f);
            uint gi = (uint)NEMathHelper.Clamp(g * 255.0f, 0, 255.0f);
            uint bi = (uint)NEMathHelper.Clamp(b * 255.0f, 0, 255.0f);
             
            ColMask = ri | (gi << 8) | (bi << 16);
        }

        static public NEConsoleColorDef operator *(NEConsoleColorDef lhs, NEConsoleColorDef rhs)
        {
            float r = NEMathHelper.Clamp(lhs.RNormalized * rhs.RNormalized, 0.0f, 1.0f);
            float g = NEMathHelper.Clamp(lhs.GNormalized * rhs.GNormalized, 0.0f, 1.0f);
            float b = NEMathHelper.Clamp(lhs.BNormalized * rhs.BNormalized, 0.0f, 1.0f);
            return new NEConsoleColorDef(r, g, b);
        }

        static public NEConsoleColorDef operator *(NEConsoleColorDef lhs, float rhs)
        {
            float r = NEMathHelper.Clamp(lhs.RNormalized * rhs, 0.0f, 1.0f);
            float g = NEMathHelper.Clamp(lhs.GNormalized * rhs, 0.0f, 1.0f);
            float b = NEMathHelper.Clamp(lhs.BNormalized * rhs, 0.0f, 1.0f);
            return new NEConsoleColorDef(r, g, b);
        }

        static public NEConsoleColorDef operator +(NEConsoleColorDef lhs, NEConsoleColorDef rhs)
        {
            uint r = NEMathHelper.Clamp(lhs.R + rhs.R, 0, 255);
            uint g = NEMathHelper.Clamp(lhs.G + rhs.G, 0, 255);
            uint b = NEMathHelper.Clamp(lhs.B + rhs.B, 0, 255);
            return new NEConsoleColorDef(r, g, b);
        }

        static public NEConsoleColorDef operator -(NEConsoleColorDef lhs, NEConsoleColorDef rhs)
        {
            uint r = NEMathHelper.Clamp(lhs.R - rhs.R, 0, 255);
            uint g = NEMathHelper.Clamp(lhs.G - rhs.G, 0, 255);
            uint b = NEMathHelper.Clamp(lhs.B - rhs.B, 0, 255);
            return new NEConsoleColorDef(r, g, b);
        }

        static public NEConsoleColorDef operator -(NEConsoleColorDef val)
        {
            uint r = NEMathHelper.Clamp(255 - val.R, 0, 255);
            uint g = NEMathHelper.Clamp(255 - val.G, 0, 255);
            uint b = NEMathHelper.Clamp(255 - val.B, 0, 255);
            return new NEConsoleColorDef(r, g, b);
        }

        static public NEConsoleColorDef Inverse(NEConsoleColorDef val)
        {
            return -val;
        }

        public uint R { get { return ColMask & 0x0000ff;} }
        public uint G { get { return (ColMask & 0x00ff00) >> 8; } }
        public uint B { get { return (ColMask & 0xff0000) >> 16; } }

        public float RNormalized { get { return ((float)R) / 255.0f; } }
        public float GNormalized { get { return ((float)G) / 255.0f; } }
        public float BNormalized { get { return ((float)B) / 255.0f; } }

    }


    //public class NEPixel
    //{
    //    public int X;
    //    public int Y;
    //    public int Col;

    //    public NEPixel(int x, int y, int col)
    //    {
    //        X = x;
    //        Y = y;
    //        Col = col;
    //    }
    //}
}