using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ConsoleRenderer.Core
{
    //public struct CGRectangle
    //{
    //    public int Left { get; set; }
    //    public int Top { get; set; }
    //    public int Right { get; set; }
    //    public int Bottom { get; set; }
    //}

    [StructLayout(LayoutKind.Sequential)]
    public struct CGRectangle
    {
        public short Left { get; set; }
        public short Top { get; set; }
        public short Right { get; set; }
        public short Bottom { get; set; }

        public CGRectangle(short left, short top, short right, short bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public static CGRectangle operator -(CGRectangle lhs, CGRectangle rhs)
        {
            return new CGRectangle((short)(lhs.Left - rhs.Left), (short)(lhs.Top - rhs.Top), (short)(lhs.Right - rhs.Right), (short)(lhs.Bottom - rhs.Bottom));
        }
        public static CGRectangle operator +(CGRectangle lhs, CGRectangle rhs)
        {
            return new CGRectangle((short)(lhs.Left + rhs.Left), (short)(lhs.Top + rhs.Top), (short)(lhs.Right + rhs.Right), (short)(lhs.Bottom + rhs.Bottom));
        }
        public static CGRectangle operator *(CGRectangle lhs, int rhs)
        {
            return new CGRectangle((short)(lhs.Left * rhs), (short)(lhs.Top * rhs), (short)(lhs.Right * rhs), (short)(lhs.Bottom * rhs));
        }

        public static CGRectangle operator /(CGRectangle lhs, int rhs)
        {
            return new CGRectangle((short)(lhs.Left / rhs), (short)(lhs.Top / rhs), (short)(lhs.Right / rhs), (short)(lhs.Bottom / rhs));
        }

        public static CGRectangle operator %(CGRectangle lhs, int rhs)
        {
            return new CGRectangle((short)(lhs.Left % rhs), (short)(lhs.Top % rhs), (short)(lhs.Right % rhs), (short)(lhs.Bottom % rhs));
        }

    }
    //public struct CGPoint
    //{
    //    public int X { get; set; }
    //    public int Y { get; set; }

    //}


    [StructLayout(LayoutKind.Sequential)]
    public struct CGPoint
    {
        public short X;
        public short Y;

        public CGPoint(short X, short Y)
        {
            this.X = X;
            this.Y = Y;
        }

        static public CGPoint operator+(CGPoint lhs, CGPoint rhs)
        {
            return new CGPoint((short)(lhs.X + rhs.X), (short)(lhs.Y + rhs.Y));
        }

        static public CGPoint operator-(CGPoint lhs, CGPoint rhs)
        {
            return new CGPoint((short)(lhs.X - rhs.X), (short)(lhs.Y - rhs.Y));
        }

        static public CGPoint operator *(CGPoint lhs, int rhs)
        {
            return new CGPoint((short)(lhs.X * rhs), (short)(lhs.Y * rhs));
        }

        static public CGPoint operator /(CGPoint lhs, int rhs)
        {
            return new CGPoint((short)(lhs.X / rhs), (short)(lhs.Y / rhs));
        }

        static public CGPoint operator %(CGPoint lhs, int rhs)
        {
            return new CGPoint((short)(lhs.X % rhs), (short)(lhs.Y % rhs));
        }

    };


    public class CGPixel
    {
        public int X;
        public int Y;
        public int Col;

        public CGPixel(int x, int y, int col)
        {
            X = x;
            Y = y;
            Col = col;
        }
    }
}
