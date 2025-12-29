using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{

    public class NEColorTexture16: NETexture
    {

        public byte[] DATA { get { return m_Data; } }
        public static string LastErrorMessage { get; private set; }

        private byte[] m_Data;

        private NEColorTexture16()
        {
            SampleMode = NESampleMode.Clamp;
        }

        private bool ReadFromFile(string path)
        {
            int h = 0;
            List<byte> data = new List<byte>();
           
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
                            data.Add(byte.Parse(vals[i]));
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

		public override NEColorSample Sample(float u, float v)
		{

			// u = NEMathHelper.Abs(u);
			//v = NEMathHelper.Abs(v);

			NEColorSample cs = new NEColorSample();
			int index = ComputeDataIndex(u, v, m_Data.Length);
			if(index == -1)
			{
				return cs;
			}
			
			cs.minCol = 0;
			cs.maxCol = (m_Data != null) ? m_Data[index] : (byte)13;
			cs.t = 1.0f;

			return cs;
		}

		public override NEColorSample GetPixel(int x, int y)
		{
			int index = y * Width + x;
			NEColorSample sample = new NEColorSample();
			sample.minCol = 0;
			sample.maxCol = m_Data[index];
			sample.t = 1.0f;

			return sample;
		}

		public override NECharacterCell SampleCell(float u, float v, float intensity)
        {
			// u = NEMathHelper.Abs(u);
			//v = NEMathHelper.Abs(v);
			NEColorSample cs = Sample(u, v);
			byte col = cs.maxCol;
            if (col == 16)
            {
                return NECharacterCell.MakeTransparent();
            }

            return NECharacterCell.Make(cs.minCol, col, cs.t * intensity, NECHAR_RAMPS.CHAR_RAMP_FULL_EXT);
        }

		public override NECharacterCell SampleCell(float u, float v, int[] charRamp, float intensity)
		{

			NEColorSample cs = Sample(u, v);
			byte col = cs.maxCol;
			if (col == 16)
			{
				return NECharacterCell.MakeTransparent();
			}

			return NECharacterCell.Make(cs.minCol, col, cs.t * intensity, charRamp);
		}

		public NECharacterCell SampleFromBlocks10(float u, float v, float intensity)
        {

            if (SampleMode == NESampleMode.Clamp)
            {
                if (u < 0.0f || u > 1.0f)
                {
                    return NECharacterCell.MakeFromBlocks10(ConsoleColor.Black, 0, intensity);
                }
                u -= (int)u;
            }
            else if (SampleMode == NESampleMode.Repeat)
            {
                u -= (int)u;
                u = u < 0 ? 1.0f - NEMath.Abs(u) : u;
            }

            v -= (int)v;
            v = v < 0 ? 1.0f - NEMath.Abs(v) : v;

            int x = (int)Math.Round(u * (float)Width);
            if (x >= (Width - 1)) x = Width - 1;

            int y = (int)Math.Round(v * (float)Height);
            if (y >= (Height - 1)) y = Height - 1;
            int index = y * Width + x;
            int col = m_Data != null ? m_Data[index] : 13;
            if (col == 16)
            {
                return NECharacterCell.MakeTransparent();
            }

            return NECharacterCell.MakeFromBlocks10(ConsoleColor.Black, (ConsoleColor)col, intensity);
        }

    }
}
