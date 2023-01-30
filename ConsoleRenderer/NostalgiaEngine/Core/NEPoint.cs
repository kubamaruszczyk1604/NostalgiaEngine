using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace NostalgiaEngine.Core
{

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
}
