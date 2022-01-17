using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace NostalgiaEngine.Core
{
    public class NEDebug
    {

        static public void Print(string message)
        {
            Debug.Print("NEDebug|  " + message);
        }
        static public void Print(int message)
        {
            Debug.Print("NEDebug|  (int): " + message.ToString());
        }

        static public void Print(uint message)
        {
            Debug.Print("NEDebug|  (uint): " + message.ToString());
        }

        static public void Print(short message)
        {
            Debug.Print("NEDebug|  (short): " + message.ToString());
        }

        static public void Print(float message)
        {
            Debug.Print("NEDebug|  (float): " + message.ToString());
        }

        static public void Print(double message)
        {
            Debug.Print("NEDebug|  (double): " + message.ToString());
        }
    }
}
