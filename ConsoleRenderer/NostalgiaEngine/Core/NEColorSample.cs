using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{

    public enum NEBlock {Space= 32, Weak = 176, Middle = 177, Strong = 178, Solid = 219 } // mapped to ascii


    class NECHAR_RAMPS
    {
        static public int[] BLOCK_RAMP5 = new int[] { (int)NEBlock.Space, (int)NEBlock.Weak, (int)NEBlock.Middle, (int)NEBlock.Strong, (int)NEBlock.Solid};

        static public int[] CHAR_RAMP_10 = new int[] { (int)' ', (int)'.', (int)':', (int)'-', (int)'=', (int)'+', (int)'*', (int)'#', (int)'%', (int)'@'};

        static public int[] CHAR_RAMP_12 = new int[] { (int)' ', (int)'.', (int)':', (int)'-', (int)'=', (int)'+', (int)'*', (int)'#', (int)'%', (int)'@', (int)NEBlock.Strong, (int)NEBlock.Solid };

        static public int[] String2BlockArray(string str)
        {
            int[] ret = new int[str.Length+3];
            for(int i=0; i < str.Length; ++i)
            {
                ret[i] =  str[i];
            }
            ret[str.Length] = 177;
            ret[str.Length + 1] = 178;
            ret[str.Length + 2] = 219;
            return ret;
        }

        static public int[] CHAR_RAMP_FULL = String2BlockArray(@" `.-':_,^=;><+!rc*/z?sLTv)J7(|Fi{C}fI31tlu[neoZ5Yxjya]2ESwqkP6h9d4VpOGbUAKXHm8RD#$Bg0MNWQ%&@");

        //static public int[] BLOCK_ARR_FULL2 = new int[] { (int)NEBlock.Space, 221, 179,  (int)NEBlock.Weak, 181, 186, 185, (int)NEBlock.Middle, (int)NEBlock.Strong, (int)NEBlock.Solid };
    }


    public class NEColorSample
    {
        private static readonly int MAX_COL_COUNT = 10;
        public short BitMask { get; private set; }
        public char Character { get; private set; }

        public override string ToString()
        {
           
            return "Char = " + (short)Character + ", FG_Col = " + (BitMask& 0x000F).ToString() + ", BG_Col = " + ((BitMask&0x00F0)>>4).ToString();
        }

        static public short GetBGCol(ConsoleColor col)
        {
            return (short)col;
        }

        static public short GetFGCol(ConsoleColor col)
        {
            return (short)(((short)col) << 4);
        }

        static public NEColorSample MakeTransparent()
        {
            var s = new NEColorSample();
            s.BitMask = 16;
            s.Character = 't';
            return s;
        }

        static public NEColorSample MakeCol10(ConsoleColor col1, ConsoleColor col2, float t)
        {
            //if (t == float.NaN) t = 0.0f;
            int BG1 = (int)col1;
            int FG1 = ((int)col1) << 4;
            int BG2 = (int)col2;
            int FG2 = ((int)col2) << 4;

            int[] pairs = new int[] { BG1 | FG2, BG2 | FG1 };
            float tFract = t >= 1.0f? 1.0f: t - (float)Math.Floor(t);
            tFract = tFract <= 0 ? 0.01f:tFract; //clamp
            //tFract = Math.Abs(tFract); // repeat
            int index = (int)(tFract * (float)MAX_COL_COUNT);
            index = index >= (MAX_COL_COUNT - 1) ? (MAX_COL_COUNT - 1) : index;

            NEColorSample sample = new NEColorSample();
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

            sample.Character = (char)NECHAR_RAMPS.BLOCK_RAMP5[index];

            return sample;
        }

        static public NEColorSample MakeCol10F(ConsoleColor col1, ConsoleColor col2, float t)
        {
            float tFract = t >= 1.0f ? 1.0f : t - (float)Math.Floor(t);
            tFract = NEMathHelper.Clamp(tFract, 0.0f, 1.0f);


            int index = (int)(tFract * 10.0f);
            if (index > 9) index = 9;

            NEColorSample sample = new NEColorSample();
            sample.BitMask = (short)((int)col1 << 4 | ((int)col2));



            sample.Character = (char)NECHAR_RAMPS.CHAR_RAMP_10[index];

            return sample;
        }

        static public NEColorSample MakeCol(ConsoleColor col1, ConsoleColor col2, float t, int[] charRamp)
        {
            float tFract = t >= 1.0f ? 1.0f : t - (float)Math.Floor(t);
            tFract = NEMathHelper.Clamp(tFract, 0.0f, 1.0f);

            int rampLastIndex = charRamp.Length - 1; 
            int index = (int)(tFract * charRamp.Length);
            if (index > rampLastIndex) index = rampLastIndex;

            NEColorSample sample = new NEColorSample();
            sample.BitMask = (short)((int)col1 << 4 | ((int)col2));



            sample.Character = (char)charRamp[index];

            return sample;
        }


        static public NEColorSample MakeCol5(ConsoleColor col1, ConsoleColor col2, float t)
        {


            float tFract = t >= 1.0f ? 1.0f : t - (float)Math.Floor(t);
            tFract = NEMathHelper.Clamp(tFract, 0.0f, 1.0f);


            int index = (int)(tFract * 5.0f);
            if (index > 4) index = 4;

            NEColorSample sample = new NEColorSample();
            sample.BitMask = (short)((int)col1 << 4 | ((int)col2));



            sample.Character = (char)NECHAR_RAMPS.BLOCK_RAMP5[index];

            return sample;
        }
    }




}
