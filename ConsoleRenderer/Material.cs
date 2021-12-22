using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer
{

    enum Block {Space= 32, Weak = 176, Middle = 177, Strong = 178, Solid = 219 } // mapped to ascii


    class BLOCKS
    {
        static public int[] BLOCK_ARR = new int[] { (int)Block.Space, (int)Block.Weak, (int)Block.Middle, (int)Block.Strong, (int)Block.Solid};
    }

    enum BlockMask
    {
        PIXEL_SOLID = 0x2588,
        PIXEL_THREEQUARTERS = 0x2593,
        PIXEL_HALF = 0x2592,
        PIXEL_QUARTER = 0x2591,
    };


    // Colour definitions lifted from One Lone Coder: https://github.com/OneLoneCoder/videos/blob/master/olcConsoleGameEngine.h
    enum ColorMask
    {
        FG_BLACK = 0x0000,
        FG_DARK_BLUE = 0x0001,
        FG_DARK_GREEN = 0x0002,
        FG_DARK_CYAN = 0x0003,
        FG_DARK_RED = 0x0004,
        FG_DARK_MAGENTA = 0x0005,
        FG_DARK_YELLOW = 0x0006,
        FG_GREY = 0x0007, 
        FG_DARK_GREY = 0x0008,
        FG_BLUE = 0x0009,
        FG_GREEN = 0x000A,
        FG_CYAN = 0x000B,
        FG_RED = 0x000C,
        FG_MAGENTA = 0x000D,
        FG_YELLOW = 0x000E,
        FG_WHITE = 0x000F,
        BG_BLACK = 0x0000,
        BG_DARK_BLUE = 0x0010,
        BG_DARK_GREEN = 0x0020,
        BG_DARK_CYAN = 0x0030,
        BG_DARK_RED = 0x0040,
        BG_DARK_MAGENTA = 0x0050,
        BG_DARK_YELLOW = 0x0060,
        BG_GREY = 0x0070,
        BG_DARK_GREY = 0x0080,
        BG_BLUE = 0x0090,
        BG_GREEN = 0x00A0,
        BG_CYAN = 0x00B0,
        BG_RED = 0x00C0,
        BG_MAGENTA = 0x00D0,
        BG_YELLOW = 0x00E0,
        BG_WHITE = 0x00F0,
    };

    class ColorSample
    {
        private static readonly int MAX_COL_COUNT = 10;
        public short BitMask { get; private set; }
        public char Character { get; private set; }


        static public short GetBGCol(ConsoleColor col)
        {
            return (short)col;
        }

        static public short GetFGCol(ConsoleColor col)
        {
            return (short)(((short)col) << 4);
        }

        static public ColorSample MakeCol(ConsoleColor col1, ConsoleColor col2, float t)
        {
            int BG1 = (int)col1;
            int FG1 = ((int)col1) << 4;
            int BG2 = (int)col2;
            int FG2 = ((int)col2) << 4;

            int[] pairs = new int[] { BG1 | FG2, BG2 | FG1 };
            float tFract = t - (float)Math.Floor(t);
            tFract = tFract >= 0 ? tFract : 1.0f - tFract; //mirror
            //tFract = Math.Abs(tFract); // repeat
            int index = (int)(tFract * (float)MAX_COL_COUNT);
            index = index >= (MAX_COL_COUNT - 1) ? (MAX_COL_COUNT - 1) : index;

            ColorSample sample = new ColorSample();
            if(index%2 == 0)
            {
                index /= 2;
                //add: col is first pair
                sample.BitMask = (short)pairs[1];
            }
            else
            {
                index = (MAX_COL_COUNT/2)-1 - (int)(index/2);
                sample.BitMask = (short)pairs[0];
            }

            sample.Character = (char)BLOCKS.BLOCK_ARR[index];

            return sample;
        }
        
    }

    partial class RayMarcher // ALL MATERIAL RELATED CONSTANTS
    {
        public const short BACKGROUND_INTENSITY = 0x0080;
        public const short FOREGROUND_INTENSITY = 0x0008;
        //Bitmasks for colors 

        //Green
        public const short BACKGROUND_GREEN = 0x0020;
        public const short FOREGROUND_GREEN = 0x0002;

        //Red
        public const short BACKGROUND_RED = 0x0040;
        public const short FOREGROUND_RED = 0x0004;

        //Blue
        public const short BACKGROUND_BLUE = 0x0010;
        public const short FOREGROUND_BLUE = 0x0001;

        //Red
        public const short BACKGROUND_YELLOW = 0x0040 | 0x0020;
        public const short FOREGROUND_YELLOW = 0x0004 | 0x0002;

        public const short BACKGROUND_CYAN = 0x0020 | 0x0010;
        public const short FOREGROUND_CYAN = 0x0002 | 0x0001;

        public const short BACKGROUND_MAGENTA = 0x0040 | 0x0010;
        public const short FOREGROUND_MAGENTA = 0x0004 | 0x0001;

        public const short BACKGROUND_WHITE = 0x0040 | 0x0010 | 0x0020;
        public const short FOREGROUND_WHITE = 0x0004 | 0x0001 | 0x0002;
    }

    interface Material
    {
        bool IsLit();
    }

    struct LitMaterial : Material
    {
        const bool m_LitFlag = true;
        public bool IsLit() { return m_LitFlag; }
        public short ColorA;
        public short ColorB;
    }

    struct UnlitMaterial : Material
    {
        const bool m_LitFlag = false;
        public bool IsLit() { return m_LitFlag; }
        public short ColorA;
        public short ColorB;
        public Block BlockType;
    }

}
