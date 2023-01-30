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

}
