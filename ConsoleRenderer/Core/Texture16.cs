using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{

    public class NEColorTexture16: NETexture
    {

        public int[] DATA { get { return m_Data; } }
        public static string LastErrorMessage { get; private set; }

        private int[] m_Data;

        private NEColorTexture16()
        {
            SampleMode = NESampleMode.Clamp;
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

        public static NEColorTexture16 LoadFromFile(string file)
        {

            NEColorTexture16 texture = new NEColorTexture16();
            if(texture.ReadFromFile(file))
            {
                return texture;
            }
            return null;
        }

        public override NEColorSample Sample(float u, float v, float intensity)
        {
            // u = NEMathHelper.Abs(u);
            //v = NEMathHelper.Abs(v);
            
            
            if (SampleMode == NESampleMode.Clamp)
            {
                if (u < 0.0f || u > 1.0f )
                {
                    return NEColorSample.MakeCol10(ConsoleColor.Black, 0, intensity);
                }
                u -= (int)u;
            }
            else if(SampleMode == NESampleMode.Repeat)
            {
                u -= (int)u;
                u = u < 0 ? 1.0f - NEMathHelper.Abs(u) : u;
            }

            v -= (int)v;
            v = v < 0 ? 1.0f - NEMathHelper.Abs(v) : v;

            int x = (int)Math.Round(u * (float)Width);
            if (x >= (Width - 1)) x = Width - 1; 

            int y = (int)Math.Round(v * (float)Height);
            if (y >= (Height - 1)) y = Height - 1;
            int index = y * Width + x;
            int col = m_Data != null?m_Data[index]:13;
            if (col == 16)
            {
                return NEColorSample.MakeTransparent();
            }

            return NEColorSample.MakeCol10(ConsoleColor.Black, (ConsoleColor)col, intensity);
        }

    }
}
