using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NostalgiaEngine.Core
{
    public class NEFloatBuffer
    {
        public float[] Data { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public static string LastErrorMessage { get; private set; }
        public NESampleMode SampleMode { get; set; }

        public static NEFloatBuffer FromFile(string path)
        {
            int h = 0;
            List<float> data = new List<float>();
            NEFloatBuffer ret = null;
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(path))
                {
                    int lastW = -1;
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] vals = line.Split(',');
                        for (int i = 0; i < vals.Length; ++i)
                        {
                            data.Add(float.Parse(vals[i]));
                        }
                        if (lastW == -1)
                        {
                            lastW = vals.Length;
                        }
                        else
                        {
                            if (lastW != vals.Length)
                            {
                                LastErrorMessage = "Row size mismatch.";
                                return null;
                            }
                        }
                        h++;
                    }
                    if (lastW < 5)
                    {
                        LastErrorMessage = "Texture width cannot be lower than 5 pixels. It is: " + lastW.ToString() + " pixels wide.";
                        return null;
                    }
                    if (h < 5)
                    {
                        LastErrorMessage = "Texture height cannot be lower than 5 pixels. It is: " + h.ToString() + " pixels high.";
                        return null;
                    }
                    ret = new NEFloatBuffer(lastW, h, data.ToArray());
                }
            }
            catch (Exception e)
            {
                LastErrorMessage = e.Message;
                return null;
            }


            return ret;
        }

        public NEFloatBuffer(int w, int h)
        {
            Width = w;
            Height = h;
            Data = new float[Width * Height];
            SampleMode = NESampleMode.Clamp;
        }

        public NEFloatBuffer(int w, int h, float[] data)
        {
            if ((w * h) != data.Length)
            {
                throw new Exception("Data length and dimensions do not match");
            }
            Width = w;
            Height = h;
            Data = data;
            SampleMode = NESampleMode.Clamp;
        }

        public void SetField(int x, int y, float val)
        {
            Data[XY2I(x, y)] = val;
        }

        public void SetField(int i, float val)
        {
            Data[i] = val;
        }

        public float GetField(int x, int y)
        {
            return Data[XY2I(x, y)];
        }

        public float GetField(int i)
        {
            return Data[i];
        }

        public float Sample(float u, float v)
        {
            // u = NEMathHelper.Abs(u);
            //v = NEMathHelper.Abs(v);


            if (SampleMode == NESampleMode.Clamp)
            {
                if (u < 0.0f || u > 1.0f)
                {
                    return 0;
                }
                u -= (int)u;
            }
            else if (SampleMode == NESampleMode.Repeat)
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
            float col = Data != null ? Data[index] : 13;


            return col;
        }


        private int XY2I(int x, int y)
        {
            return y * Width + x;
        }
    }
}
