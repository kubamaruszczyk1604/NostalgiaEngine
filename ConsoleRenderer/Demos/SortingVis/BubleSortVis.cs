using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;
namespace NostalgiaEngine.Demos
{
    public class BubleSortVis
    {
        public int[] Data { get; private set; }
        public bool Done { get; private set; }
        private int m_Index;
        private int m_SwapCount;

        public BubleSortVis(int dataLen, int maxVal = 100)
        {
            Data = new int[dataLen];
            for(int i = 0; i < Data.Length;++i)
            {
                Data[i] = i%maxVal;
            }
            NETools.SuffleArray(Data);
            Done = false;
            m_Index = 0;
            m_SwapCount = 0;
        }


        public void DoStep()
        {
            if (Done) return;
            if(m_Index > (Data.Length-2))
            {
                m_Index = 0;
                if(m_SwapCount == 0)
                {
                    Done = true;
                    return;
                }
                m_SwapCount = 0;
            }
            int a = Data[m_Index];
            int b = Data[m_Index + 1];
            if(a > b)
            {
                Data[m_Index] = b;
                Data[m_Index + 1] = a;
                m_SwapCount++;
            }
            m_Index++;
        }

    }
}
