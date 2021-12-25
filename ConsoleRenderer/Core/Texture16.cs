using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer.Core
{
    class CGTexture16
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public static string LastErrorMessage { get; private set; }
     

        private int[] m_Data;



        private CGTexture16()
        {
           
        }

        private bool ReadFromFile(string path)
        {
            int h = 0;
            List<int> data = new List<int>();
           
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(path))
                {
                    int lastW = -1;
                    while (!reader.EndOfStream)
                    {
                       string line = reader.ReadLine();
                       string[] vals = line.Split(',');
                       for(int i =0; i < vals.Length;++i)
                       {
                            data.Add(int.Parse(vals[i]));
                       }
                       if(lastW == -1)
                        {
                            lastW = vals.Length;
                        }
                       else
                        {
                           if(lastW != vals.Length)
                            {
                                LastErrorMessage = "Row size mismatch.";
                                return false;
                            }
                        }
                        h++;
                    }
                    if(lastW < 5)
                    {
                        LastErrorMessage = "Texture width cannot be lower than 5 pixels. It is: " + lastW.ToString() + " pixels wide.";
                        return false;
                    }
                    if(h < 5)
                    {
                        LastErrorMessage = "Texture height cannot be lower than 5 pixels. It is: " + h.ToString() + " pixels high.";
                        return false;
                    }

                    Width = lastW;
                    Height = h;
                    m_Data = data.ToArray();
                }
            }
            catch(Exception e)
            {
                LastErrorMessage = e.Message;
                return false;
            }

           
            return true;
        }

        public static CGTexture16 LoadFromFile(string file)
        {

            CGTexture16 texture = new CGTexture16();
            if(texture.ReadFromFile(file))
            {
                return texture;
            }
            return null;
        }

        public CGColorSample Sample(float u, float v, float intensity)
        {
            int x = (int)(u * (float)Width);
            if (x >= (Width - 1)) x = Width - 1; 

            int y = (int)(v * (float)Height);
            if (y >= (Height - 1)) y = Height - 1;
            int index = y * Width + x;
            int col = m_Data[index];

            return CGColorSample.MakeCol(ConsoleColor.Black, (ConsoleColor)col,intensity);
        }

    }
}
