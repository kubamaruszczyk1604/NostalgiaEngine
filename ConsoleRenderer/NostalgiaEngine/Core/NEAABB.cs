using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{
	public struct NEAABB
	{
		public NEVector4 Min;
		public NEVector4 Max;

		public void CreateEmpty()
		{
			Min = new NEVector4(float.MaxValue, float.MaxValue, float.MaxValue, 1.0f);
			Max = new NEVector4(float.MinValue, float.MinValue, float.MinValue, 1.0f);
		}

		public void Update(NEVector4 pos)
		{
			Min.X = NEMath.Min(Min.X, pos.X);
			Min.Y = NEMath.Min(Min.Y, pos.Y);
			Min.Z = NEMath.Min(Min.Z, pos.Z);

			Max.X = NEMath.Max(Max.X, pos.X);
			Max.Y = NEMath.Max(Max.Y, pos.Y);
			Max.Z = NEMath.Max(Max.Z, pos.Z);
		}

		public void Update(List<NEVector4> positions)
		{
			for (int i = 0; i < positions.Count; ++i)
			{
				Update(positions[i]);
			}
		}

		public void Update(NEVector4[] positions)
		{
			for (int i = 0; i < positions.Length; ++i)
			{
				Update(positions[i]);
			}
		}

		public NEAABB GetTransformed(ref NEMatrix4x4 transform)
		{
			NEVector4 center = (Min + Max) * 0.5f;
			center.W = 1.0f;

			NEVector4 extents = (Max - Min) * 0.5f;
			extents.W = 0.0f;

			center = transform * center;

			NEMatrix4x4 rotationScale = new NEMatrix4x4();
			rotationScale = transform;

			rotationScale.M3 = 0;
			rotationScale.M7 = 0;
			rotationScale.M11 = 0;

			rotationScale.M0 = NEMath.Abs(rotationScale.M0);
			rotationScale.M4 = NEMath.Abs(rotationScale.M4);
			rotationScale.M8 = NEMath.Abs(rotationScale.M8);

			rotationScale.M1 = NEMath.Abs(rotationScale.M1);
			rotationScale.M5 = NEMath.Abs(rotationScale.M5);
			rotationScale.M9 = NEMath.Abs(rotationScale.M9);

			rotationScale.M2 = NEMath.Abs(rotationScale.M2);
			rotationScale.M6 = NEMath.Abs(rotationScale.M6);
			rotationScale.M10 = NEMath.Abs(rotationScale.M10);

			extents = rotationScale * extents;

			NEAABB newAABB = new NEAABB();
			newAABB.Min = center - extents;
			newAABB.Max = center + extents;

			return newAABB;

		}
	}
}
