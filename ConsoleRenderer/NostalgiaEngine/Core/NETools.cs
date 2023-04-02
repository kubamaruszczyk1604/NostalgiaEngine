using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{
    public class NETools
    {

        static public void SuffleArray<T>(T[] arr)
        {
            Random random = new Random();
            int len = arr.Length;
            for (int i = 0; i < (len - 1); ++i)
            {
                int newInd = i + random.Next(len - i);
                T temp = arr[newInd];
                arr[newInd] = arr[i];
                arr[i] = temp;
            }
        }
    }
}
