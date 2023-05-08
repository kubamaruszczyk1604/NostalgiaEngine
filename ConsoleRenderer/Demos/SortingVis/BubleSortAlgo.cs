using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;
namespace NostalgiaEngine.Demos
{
    public class BubleSortAlgo
    {
        public int[] Data { get; private set; }
        public bool Done { get; private set; }
        public int SwappedIndex0 { get; private set; }
        public bool Swapped { get; private set; }
        public int StepCount { get; private set; }
        private int m_Index;
        private int m_SwapCount;

        public BubleSortAlgo(int dataLen, int maxVal = 100)
        {
            Data = new int[dataLen];
            for(int i = 0; i < Data.Length;++i)
            {
                Data[i] = i%maxVal + 1;
            }
            NETools.SuffleArray(Data);
            Done = false;
            m_Index = 0;
            m_SwapCount = 0;
            StepCount = 0;
        }


        public void DoStep()
        {
            Swapped = false;
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
            SwappedIndex0 = m_Index;
            if(a > b)
            {
                Data[m_Index] = b;
                Data[m_Index + 1] = a;
                m_SwapCount++;
                Swapped = true;
            }
            m_Index++;
            StepCount++;
        }

        //public void DoPass()
        //{
            
        //    if (Done) return;
        //    while (true)
        //    {
        //        if (m_Index > (Data.Length - 2))
        //        {
        //            m_Index = 0;
        //            if (m_SwapCount == 0)
        //            {
        //                Done = true;
        //            }
        //            m_SwapCount = 0;
        //            return;
        //        }
        //        int a = Data[m_Index];
        //        int b = Data[m_Index + 1];
        //        if (a > b)
        //        {
        //            Data[m_Index] = b;
        //            Data[m_Index + 1] = a;
        //            m_SwapCount++;
        //        }
        //        m_Index++;
        //    }
        //}

    }
}
