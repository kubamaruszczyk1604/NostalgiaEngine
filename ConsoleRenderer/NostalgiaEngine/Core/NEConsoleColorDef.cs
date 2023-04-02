using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace NostalgiaEngine.Core
{

    [StructLayout(LayoutKind.Sequential)]
    public struct NEConsoleColorDef
    {
        public uint ColMask;
        private static readonly float s_NormConstant = 1.0f / 255.0f;

        public NEConsoleColorDef(uint r, uint g, uint b)
        {
            r = NEMath.Clamp(r, 0, 255);
            g = NEMath.Clamp(g, 0, 255);
            b = NEMath.Clamp(b, 0, 255);
            ColMask = r | (g << 8) | (b << 16);
        }

        public NEConsoleColorDef(float r, float g, float b)
        {
            uint ri = (uint)NEMath.Clamp(r * 255.0f, 0, 255.0f);
            uint gi = (uint)NEMath.Clamp(g * 255.0f, 0, 255.0f);
            uint bi = (uint)NEMath.Clamp(b * 255.0f, 0, 255.0f);
             
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
            float r = NEMath.Clamp(lhs.RNormalized * rhs.RNormalized, 0.0f, 1.0f);
            float g = NEMath.Clamp(lhs.GNormalized * rhs.GNormalized, 0.0f, 1.0f);
            float b = NEMath.Clamp(lhs.BNormalized * rhs.BNormalized, 0.0f, 1.0f);
            return new NEConsoleColorDef(r, g, b);
        }

        static public NEConsoleColorDef operator *(NEConsoleColorDef lhs, float rhs)
        {
            float r = NEMath.Clamp(lhs.RNormalized * rhs, 0.0f, 1.0f);
            float g = NEMath.Clamp(lhs.GNormalized * rhs, 0.0f, 1.0f);
            float b = NEMath.Clamp(lhs.BNormalized * rhs, 0.0f, 1.0f);
            return new NEConsoleColorDef(r, g, b);
        }

        static public NEConsoleColorDef operator +(NEConsoleColorDef lhs, NEConsoleColorDef rhs)
        {
            uint r = NEMath.Clamp(lhs.R + rhs.R, 0, 255);
            uint g = NEMath.Clamp(lhs.G + rhs.G, 0, 255);
            uint b = NEMath.Clamp(lhs.B + rhs.B, 0, 255);
            return new NEConsoleColorDef(r, g, b);
        }

        static public NEConsoleColorDef operator -(NEConsoleColorDef lhs, NEConsoleColorDef rhs)
        {
            int r =  NEMath.Clamp((int)lhs.R - (int)rhs.R, 0, 255);
            int g =  NEMath.Clamp((int)lhs.G - (int)rhs.G, 0, 255);
            int b =  NEMath.Clamp((int)lhs.B - (int)rhs.B, 0, 255);
            return new NEConsoleColorDef((uint)r, (uint)g, (uint)b);
        }

        static public NEConsoleColorDef operator -(NEConsoleColorDef val)
        {
            uint r = NEMath.Clamp(255 - val.R, 0, 255);
            uint g = NEMath.Clamp(255 - val.G, 0, 255);
            uint b = NEMath.Clamp(255 - val.B, 0, 255);
            return new NEConsoleColorDef(r, g, b);
        }

        static public NEConsoleColorDef Inverse(NEConsoleColorDef val)
        {
            return -val;
        }

        public uint R { get { return ColMask & 0x0000ff;} }
        public uint G { get { return (ColMask & 0x00ff00) >> 8; } }
        public uint B { get { return (ColMask & 0xff0000) >> 16; } }

        public float RNormalized { get { return ((float)R) * s_NormConstant; } }
        public float GNormalized { get { return ((float)G) * s_NormConstant; } }
        public float BNormalized { get { return ((float)B) * s_NormConstant; } }

    }

}
