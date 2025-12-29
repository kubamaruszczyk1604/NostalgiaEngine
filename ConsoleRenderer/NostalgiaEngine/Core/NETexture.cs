using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{
	public enum NESampleMode { Clamp = 0, Repeat = 1 }

	public abstract class NETexture
    {

        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public NESampleMode SampleMode { get; set; }

        public abstract NECharacterCell SampleCell(float u, float v, float intensity);
		public abstract NECharacterCell SampleCell(float u, float v, int[] charRamp, float intensity);

		public abstract NEColorSample Sample(float u, float v);
		public abstract NEColorSample GetPixel(int x, int y);

		protected int ComputeDataIndex(float u, float v, int dataLen)
		{
			if (SampleMode == NESampleMode.Clamp)
			{
				if (u < 0.0f || u > 1.0f)
				{
					return -1;
				}

				//get fractional part of u
				u -= (int)u;
			}
			else if (SampleMode == NESampleMode.Repeat)
			{
				//get fractional part of u
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

			return index;
		}
	}

}
