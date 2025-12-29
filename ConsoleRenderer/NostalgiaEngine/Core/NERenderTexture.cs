using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{
	public struct NEColorSample
	{
		public byte minCol;
		public byte maxCol;
		public float t;
	} 

	public class NERenderTexture: NETexture
	{
		private NEColorSample[] m_Data;
		public NERenderTexture(int width, int height)
		{
			Width = width;
			Height = height;
			m_Data = new NEColorSample[Width * Height];
			for(int i = 0; i < m_Data.Length; ++i)
			{
				ref NEColorSample cell = ref m_Data[i];
			}
		}

		public void Write(int x, int y, byte minCol, byte maxCol, float t)
		{
			int index = y * Width + x;
			ref NEColorSample cell = ref m_Data[index];
			cell.minCol = minCol;
			cell.maxCol = maxCol;
			cell.t = t;
		}

		public override NEColorSample GetPixel(int x, int y)
		{
			int index = y * Width + x;
			return m_Data[index];
		}
		public override NEColorSample Sample(float u, float v)
		{

			int index = ComputeDataIndex(u, v, m_Data.Length);
			if(index == -1)
			{
				return new NEColorSample();
			}

			return m_Data[index]; 
		}

		public override NECharacterCell SampleCell(float u, float v, float intensity = 1.0f)
		{
			NEColorSample cell = Sample(u, v);

			return NECharacterCell.Make(cell.minCol, cell.maxCol, cell.t * intensity, NECHAR_RAMPS.CHAR_RAMP_FULL);
		}

		public override NECharacterCell SampleCell(float u, float v, int[] charRamp, float intensity = 1.0f)
		{
			NEColorSample cell = Sample(u, v);

			return NECharacterCell.Make(cell.minCol, cell.maxCol, cell.t * intensity, charRamp);
		}
	}
}
