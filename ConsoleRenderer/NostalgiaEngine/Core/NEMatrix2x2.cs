using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{

    public class NEMatrix2x2
    {
        public float[] Data { get; private set; }

        public override string ToString()
        {
            return Data[0].ToString() + " " + Data[1].ToString() + "\n" + Data[2].ToString() + " " + Data[3].ToString();
        }

        public NEMatrix2x2(float col1_1, float col1_2, float col2_1, float col2_2)
        {
            Data = new float[] { col1_1, col2_1,
                                 col1_2, col2_2 };
        }

        public NEMatrix2x2(float[] data)
        {
            Data = new float[4];
            int ceil = data.Length < 4 ? data.Length : 4;
            for (int i = 0; i < ceil; ++i)
            {
                Data[i] = data[i];
            }
        }

        public NEMatrix2x2()
        {
            Data = new float[] { 1.0f, 0.0f, 0.0f, 1.0f };
        }

        public NEMatrix2x2(NEVector2 col1, NEVector2 col2) : this(col1.X, col1.Y, col2.X, col2.Y)
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

            float r0 = Data[3] / det;
            float r2 = -Data[2] / det;
            float r1 = -Data[1] / det;
            float r3 = Data[0] / det;

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

        static public NEMatrix2x2 operator *(NEMatrix2x2 lhs, float rhs)
        {
            return new NEMatrix2x2(lhs.Data[0] * rhs, lhs.Data[1] * rhs, lhs.Data[2] * rhs, lhs.Data[3] * rhs);
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
}
