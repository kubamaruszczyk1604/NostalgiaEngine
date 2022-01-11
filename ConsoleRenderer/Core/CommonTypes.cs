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

        static public NEPoint operator+(NEPoint lhs, NEPoint rhs)
        {
            return new NEPoint((short)(lhs.X + rhs.X), (short)(lhs.Y + rhs.Y));
        }

        static public NEPoint operator-(NEPoint lhs, NEPoint rhs)
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


    public class NEPixel
    {
        public int X;
        public int Y;
        public int Col;

        public NEPixel(int x, int y, int col)
        {
            X = x;
            Y = y;
            Col = col;
        }
    }
}
