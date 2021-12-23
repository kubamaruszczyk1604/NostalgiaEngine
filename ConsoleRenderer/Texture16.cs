using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer
{
    class Texture16
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public string m_ErrorMsg { get; private set; }
     

        private int[] m_Data;



        private Texture16(int w, int h)
        {
            m_Data = new int[w * h];
        }

        private bool ReadFromFile(string path)
        {
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(path))
                {

                }
            }
            catch(Exception e)
            {

            }
                return false;
        }

        public static bool LoadFromFile(string file, out Texture16 texture)
        {
            texture = null;
            return false;
        }

        public ColorSample Sample(float u, float v, float intensity)
        {
            return null;
        }

    }
}
