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
            int[] ret = new int[str.Length];
            for(int i=0; i < str.Length; ++i)
            {
                ret[i] =  str[i];
            }
            return ret;
        }

        static public int[] CoppyWithAppendBlocks(int[] sourceCharRamp, int[] setToAppend)
        {
            int[] ret = new int[sourceCharRamp.Length + setToAppend.Length];
            Array.Copy(sourceCharRamp, ret, sourceCharRamp.Length);
            Array.Copy(setToAppend, 0, ret, sourceCharRamp.Length, setToAppend.Length);
            return ret;

        }

        static public readonly int[] CHAR_RAMP_FULL = String2BlockArray(@" `.-':_,^=;><+!rc*/z?sLTv)J7(|Fi{C}fI31tlu[neoZ5Yxjya]2ESwqkP6h9d4VpOGbUAKXHm8RD#$Bg0MNWQ%&@");

        static public readonly int[] CHAR_RAMP_FULL_EXT = CoppyWithAppendBlocks(CHAR_RAMP_FULL, new int[] { (int)NEBlock.Middle, (int)NEBlock.Strong, (int)NEBlock.Solid });
    }


    public struct NECharacterCell
    {
        private static readonly int MAX_COL_COUNT = 10;
        public short BitMask { get; set; }
        public char Character { get; set; }

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

        static public NECharacterCell MakeTransparent()
        {
            var s = new NECharacterCell();
            s.BitMask = 16;
            s.Character = 't';
            return s;
        }

        static public NECharacterCell MakeFromBlocks10(ConsoleColor col1, ConsoleColor col2, float t)
        {
			//if (t == float.NaN) t = 0.0f;
			int c1 = (int)col1;
			int c2 = (int)col2;

			int pair1 = c1 | (c2 << 4);
			int pair2 = c2 | (c1 << 4);

			float tFract = t >= 1.0f ? 1.0f : NEMath.Frac(t);
			tFract = tFract <= 0 ? 0.01f : tFract; //clamp
            //tFract = Math.Abs(tFract); // repeat
            int index = (int)(tFract * (float)MAX_COL_COUNT);
			int maxIndex = MAX_COL_COUNT - 1;
			if(index > maxIndex)
			{
				index = maxIndex;
			}

            NECharacterCell sample = new NECharacterCell();
            if((index & 1) == 0)
            {
                index >>= 1;
                //add: col is first pair
                sample.BitMask = (short)pair2;
            }
            else
            {
                index = (MAX_COL_COUNT >> 1) - 1 - (index >> 1);
                sample.BitMask = (short)pair1;
            }

            sample.Character = (char)NECHAR_RAMPS.BLOCK_RAMP5[index];

            return sample;
        }

		//static public NEColorSample MakeCol10F(ConsoleColor col1, ConsoleColor col2, float t)
		//{
		//    float tFract = t >= 1.0f ? 1.0f : t - (float)Math.Floor(t);
		//    tFract = NEMathHelper.Clamp(tFract, 0.0f, 1.0f);


		//    int index = (int)(tFract * 10.0f);
		//    if (index > 9) index = 9;

		//    NEColorSample sample = new NEColorSample();
		//    sample.BitMask = (short)((int)col1 << 4 | ((int)col2));



		//    sample.Character = (char)NECHAR_RAMPS.CHAR_RAMP_10[index];

		//    return sample;
		//}


		static public NECharacterCell Make(byte col1, byte col2, float t, int[] charRamp)
		{
			//float tFract = t >= 1.0f ? 1.0f : t - (float)Math.Floor(t);
			float tFract = t >= 1.0f ? 1.0f : NEMath.Frac(t);
			tFract = NEMath.Clamp(tFract, 0.0f, 1.0f);

			int rampLastIndex = charRamp.Length - 1;
			int index = (int)(tFract * charRamp.Length);
			if (index > rampLastIndex) index = rampLastIndex;

			NECharacterCell sample = new NECharacterCell();
			sample.BitMask = (short)((int)col1 << 4 | (int)col2);
			sample.Character = (char)charRamp[index];

			return sample;
		}


		static public NECharacterCell MakeColFromBlocks5(byte col1, byte col2, float t)
        {
			float tFract = t >= 1.0f ? 1.0f : NEMath.Frac(t);
            tFract = NEMath.Clamp(tFract, 0.0f, 1.0f);

            int index = (int)(tFract * 5.0f);
            if (index > 4) index = 4;

            NECharacterCell sample = new NECharacterCell();
            sample.BitMask = (short)((int)col1 << 4 | ((int)col2));

            sample.Character = (char)NECHAR_RAMPS.BLOCK_RAMP5[index];

            return sample;
        }
    }

}
