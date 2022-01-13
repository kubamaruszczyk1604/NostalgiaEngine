using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{

    public enum NEBlock {Space= 32, Weak = 176, Middle = 177, Strong = 178, Solid = 219 } // mapped to ascii


    class NEBLOCKS
    {
        static public int[] BLOCK_ARR = new int[] { (int)NEBlock.Space, (int)NEBlock.Weak, (int)NEBlock.Middle, (int)NEBlock.Strong, (int)NEBlock.Solid};
    }


    public class NEColorSample
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

            sample.Character = (char)NEBLOCKS.BLOCK_ARR[index];

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



            sample.Character = (char)NEBLOCKS.BLOCK_ARR[index];

            return sample;
        }
    }




}
