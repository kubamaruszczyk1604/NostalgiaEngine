using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace NostalgiaEngine.Core
{
    public class Debug
    {

        static public void Print(string message)
        {
            System.Diagnostics.Debug.Print("NEDebug|  " + message);
        }
        static public void Print(int message)
        {
            System.Diagnostics.Debug.Print("NEDebug|  (int): " + message.ToString());
        }

        static public void Print(uint message)
        {
            System.Diagnostics.Debug.Print("NEDebug|  (uint): " + message.ToString());
        }

        static public void Print(short message)
        {
            System.Diagnostics.Debug.Print("NEDebug|  (short): " + message.ToString());
        }

        static public void Print(float message)
        {
            System.Diagnostics.Debug.Print("NEDebug|  (float): " + message.ToString());
        }

        static public void Print(double message)
        {
            System.Diagnostics.Debug.Print("NEDebug|  (double): " + message.ToString());
        }
    }
}
