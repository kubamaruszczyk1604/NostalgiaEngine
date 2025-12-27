using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{
	class NERenderTexture: NETexture
	{
		struct DATA_CELL
		{
			public int minCol;
			public int maxCol;
			public float t;
		}

		private DATA_CELL[] m_Data;
		public NERenderTexture(int width, int height)
		{
			Width = width;
			Height = height;
			m_Data = new DATA_CELL[Width * Height];
			for(int i = 0; i < m_Data.Length; ++i)
			{
				ref DATA_CELL cell = ref m_Data[i];
			}
		}


		public override NEColorSample Sample(float u, float v, float intensity)
		{

			if (SampleMode == NESampleMode.Clamp)
			{
				if (u < 0.0f || u > 1.0f)
				{
					return NEColorSample.MakeCol(ConsoleColor.Black, 0, intensity, NECHAR_RAMPS.CHAR_RAMP_FULL_EXT);
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
			ref DATA_CELL cell = ref m_Data[index];

			return NEColorSample.MakeCol((ConsoleColor)cell.minCol, (ConsoleColor)cell.minCol, cell.t, NECHAR_RAMPS.CHAR_RAMP_FULL_EXT);
		}
	}
}
