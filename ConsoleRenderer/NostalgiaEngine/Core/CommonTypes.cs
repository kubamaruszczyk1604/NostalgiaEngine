using System;
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

        public override string ToString()
        {
            return X.ToString() + "\n" + Y.ToString();
        }

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

        public static float Dot(float x1, float y1, float x2, float y2)
        {
            return x1 * x2 + y1 * y2;
        }

        public static NEVector2 FindNormal(NEVector2 v)
        {
            float tx = v.X;
            v.X = -v.Y;
            v.Y = tx;
            return v;
        }

        static public void RotateClockWise(ref NEVector2 v, float theta)
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


    public class NEMatrix2x2
    {
        public float [] Data { get; private set; }

        public override string ToString()
        {
            return Data[0].ToString() + " " + Data[1].ToString() + "\n" + Data[2].ToString() + " " + Data[3].ToString();
        }

        public NEMatrix2x2(float col1_1, float col1_2, float col2_1, float col2_2)
        {
            Data = new float[] { col1_1, col2_1,
                                 col1_2, col2_2 };
        }

        public NEMatrix2x2(float [] data)
        {
            Data = new float[4];
            int ceil = data.Length < 4 ? data.Length : 4;
            for(int i =0; i < ceil; ++i)
            {
                Data[i] = data[i];
            }
        }

        public NEMatrix2x2()
        {
            Data = new float[] { 1.0f, 0.0f, 0.0f, 1.0f };
        }

        public NEMatrix2x2(NEVector2 col1, NEVector2 col2): this(col1.X, col1.Y, col2.X, col2.Y)
        {

        }

        public float FindDeterminant()
        {
            return Data[0] * Data[3] - Data[1] * Data[2];
        }

        public bool TryCreateInverse(out NEMatrix2x2 inverse)
        {
            float det = FindDeterminant();
            if (det == 0.0f)
            {
                inverse = null;
                return false;
            }

            inverse = new NEMatrix2x2(Data[3] / det, -Data[2] / det, -Data[1] / det, Data[0] / det);
            return true;
        }

        public NEMatrix2x2 CreateTranspose()
        {
            return new NEMatrix2x2(Data[0], Data[1], Data[2], Data[3]);
        }

        public NEMatrix2x2 CreateElementWiseProduct(NEMatrix2x2 m)
        {
            return new NEMatrix2x2(Data[0] * m.Data[0], Data[2] * m.Data[2], Data[1] * m.Data[1], Data[3] * m.Data[3]);
        }

        public static NEMatrix2x2 CreateRotation(float theta)
        {
            return new NEMatrix2x2((float)Math.Cos(theta), -(float)Math.Sin(theta),
                                   (float)Math.Sin(theta), (float)Math.Cos(theta));
        }

        public static NEMatrix2x2 CreateScale(float scale)
        {
            return new NEMatrix2x2(scale, 0, 0, scale);
        }

        public static NEMatrix2x2 CreateZeroMatrix()
        {
            return new NEMatrix2x2(0, 0, 0, 0);
        }

        public static NEMatrix2x2 CreateIdentity()
        {
            return new NEMatrix2x2();
        }

        public bool Invert()
        {
            float det = FindDeterminant();
            if (det == 0.0f)
            {
                return false;
            }

            float r0 =  Data[3] / det;
            float r2 = -Data[2] / det;
            float r1 = -Data[1] / det;
            float r3 =  Data[0] / det;

            Data[0] = r0;
            Data[2] = r2;
            Data[1] = r1;
            Data[3] = r3;
            return true;
        }

        public void Transpose()
        {
            float tmp = Data[2];
            Data[2] = Data[1];
            Data[1] = tmp;
        }

        public void Multiply(float n)
        {
            Data[0] *= n;
            Data[1] *= n;
            Data[2] *= n;
            Data[3] *= n;
        }

        public void Multiply(NEMatrix2x2 m)
        {
            float r0 = Data[0] * m.Data[0] + Data[1] * m.Data[2];  // col1 x1
            float r1 = Data[0] * m.Data[1] + Data[1] * m.Data[3];  // col2 x1
            float r2 = Data[2] * m.Data[0] + Data[3] * m.Data[2];  // col1 x2
            float r3 = Data[2] * m.Data[1] + Data[3] * m.Data[3];  // col2 x2
            Data[0] = r0;
            Data[1] = r1;
            Data[2] = r2;
            Data[3] = r3;
        }

        public void Add(float n)
        {
            Data[0] += n;
            Data[1] += n;
            Data[2] += n;
            Data[3] += n;
        }

        public void Add(NEMatrix2x2 m)
        {
            Data[0] += m.Data[0];
            Data[1] += m.Data[1];
            Data[2] += m.Data[2];
            Data[3] += m.Data[3];
        }

        public void Subtract(float n)
        {
            Data[0] -= n;
            Data[1] -= n;
            Data[2] -= n;
            Data[3] -= n;
        }

        public void Subtract(NEMatrix2x2 m)
        {
            Data[0] -= m.Data[0];
            Data[1] -= m.Data[1];
            Data[2] -= m.Data[2];
            Data[3] -= m.Data[3];
        }

        public void Divide(float n)
        {
            Data[0] /= n;
            Data[1] /= n;
            Data[2] /= n;
            Data[3] /= n;
        }

        static public NEMatrix2x2 operator* (NEMatrix2x2 lhs, float rhs)
        {
            return new NEMatrix2x2( lhs.Data[0] * rhs, lhs.Data[1] * rhs, lhs.Data[2] * rhs, lhs.Data[3] * rhs);
        }

        static public NEVector2 operator *(NEMatrix2x2 lhs, NEVector2 rhs)
        {
            return new NEVector2(lhs.Data[0] * rhs.X + lhs.Data[1] * rhs.Y,  
                                 lhs.Data[2] * rhs.X + lhs.Data[3] * rhs.Y); 

        }

        static public NEMatrix2x2 operator *(NEMatrix2x2 lhs, NEMatrix2x2 rhs)
        {
            return new NEMatrix2x2(lhs.Data[0] * rhs.Data[0] + lhs.Data[1] * rhs.Data[2],  // col1
                                   lhs.Data[2] * rhs.Data[0] + lhs.Data[3] * rhs.Data[2],  // col1
                                   lhs.Data[0] * rhs.Data[1] + lhs.Data[1] * rhs.Data[3],  // col2
                                   lhs.Data[2] * rhs.Data[1] + lhs.Data[3] * rhs.Data[3]); // col2

        }

        static public NEMatrix2x2 operator /(NEMatrix2x2 lhs, float rhs)
        {
            return new NEMatrix2x2(lhs.Data[0] / rhs, lhs.Data[1] / rhs, lhs.Data[2] / rhs, lhs.Data[3] / rhs);
        }

        static public NEMatrix2x2 operator +(NEMatrix2x2 lhs, float rhs)
        {
            return new NEMatrix2x2(lhs.Data[0] + rhs, lhs.Data[1] + rhs, lhs.Data[2] + rhs, lhs.Data[3] + rhs);
        }

        static public NEMatrix2x2 operator +(NEMatrix2x2 lhs, NEMatrix2x2 rhs)
        {
            return new NEMatrix2x2(lhs.Data[0] + rhs.Data[0], 
                                   lhs.Data[1] + rhs.Data[1], 
                                   lhs.Data[2] + rhs.Data[2], 
                                   lhs.Data[3] + rhs.Data[3]);
        }

        static public NEMatrix2x2 operator -(NEMatrix2x2 lhs, float rhs)
        {
            return new NEMatrix2x2(lhs.Data[0] - rhs, lhs.Data[1] - rhs, lhs.Data[2] - rhs, lhs.Data[3] - rhs);
        }

        static public NEMatrix2x2 operator -(NEMatrix2x2 lhs, NEMatrix2x2 rhs)
        {
            return new NEMatrix2x2(lhs.Data[0] - rhs.Data[0],
                                   lhs.Data[1] - rhs.Data[1],
                                   lhs.Data[2] - rhs.Data[2],
                                   lhs.Data[3] - rhs.Data[3]);
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

        static public NEConsoleColorDef Black { get { return new NEConsoleColorDef(0.0f, 0.0f, 0.0f); } }
        static public NEConsoleColorDef White { get { return new NEConsoleColorDef(1.0f, 1.0f, 1.0f); } }
        static public NEConsoleColorDef Gray { get { return new NEConsoleColorDef(0.5f, 0.5f, 0.5f); } }
        static public NEConsoleColorDef Red { get { return new NEConsoleColorDef(1.0f, 0.0f, 0.0f); } }
        static public NEConsoleColorDef Green { get { return new NEConsoleColorDef(0.0f, 1.0f, 0.0f); } }
        static public NEConsoleColorDef Blue { get { return new NEConsoleColorDef(0.0f, 0.0f, 1.0f); } }
        static public NEConsoleColorDef Yellow { get { return new NEConsoleColorDef(1.0f, 1.0f, 0.0f); } }
        static public NEConsoleColorDef Magenta { get { return new NEConsoleColorDef(1.0f, 0.0f, 1.0f); } }
        static public NEConsoleColorDef Cyan { get { return new NEConsoleColorDef(0.0f, 1.0f, 1.0f); } }
        static public NEConsoleColorDef Orange { get { return new NEConsoleColorDef(0.9f, 0.5f, 0.0f); } }

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
            int r =  NEMathHelper.Clamp((int)lhs.R - (int)rhs.R, 0, 255);
            int g =  NEMathHelper.Clamp((int)lhs.G - (int)rhs.G, 0, 255);
            int b =  NEMathHelper.Clamp((int)lhs.B - (int)rhs.B, 0, 255);
            return new NEConsoleColorDef((uint)r, (uint)g, (uint)b);
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

    public struct NEVector4
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
        public NEVector2 XY { get { return new NEVector2(X, Y); } }
        public NEVector2 XZ { get { return new NEVector2(X, Z); } }
        public NEVector2 YZ { get { return new NEVector2(Y, Z); } }
        public float Length { get { return CalculateLength(this); } }
        public NEVector4 Normalized { get { return Normalize(this); } }

        public override string ToString()
        {
            return X.ToString() + "\n" + Y.ToString();
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


        public static float CalculateLength(NEVector4 v)
        {
            return (float)Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z + v.W * v.W);
        }

        public static NEVector4 Normalize(NEVector4 v)
        {
            float l = CalculateLength(v);
            return new NEVector4(v.X / l, v.Y / l, v.Z / l, v.W / l);
        }

        public static float Dot(NEVector4 v1, NEVector4 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z + v1.W * v2.W;
        }



        static public NEVector4 operator +(NEVector4 lhs, NEVector4 rhs)
        {
            return new NEVector4(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z, lhs.W + rhs.W);
        }


        static public NEVector4 operator -(NEVector4 lhs, NEVector4 rhs)
        {
            return new NEVector4(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z, lhs.W - rhs.W);
        }



        static public NEVector4 operator *(NEVector4 lhs, float rhs)
        {
            return new NEVector4(lhs.X * rhs, lhs.Y * rhs, lhs.Z * rhs, lhs.W * rhs);
        }

        static public NEVector4 operator /(NEVector4 lhs, float rhs)
        {
            return new NEVector4(lhs.X / rhs, lhs.Y / rhs, lhs.Z / rhs, lhs.W / rhs);
        }

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
