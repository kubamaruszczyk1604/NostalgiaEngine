using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{
    public struct NEMatrix4x4
    {
		public float M0, M1, M2, M3;
		public float M4, M5, M6, M7;
		public float M8, M9, M10, M11;
		public float M12, M13, M14, M15;

		public void SetToIdentiy()
		{
			M0 = 1.0f; M1 = 0.0f; M2 = 0.0f; M3 = 0.0f;
			M4 = 0.0f; M5 = 1.0f; M6 = 0.0f; M7 = 0.0f;
			M8 = 0.0f; M9 = 0.0f; M10 = 1.0f; M11 = 0.0f;
			M12 = 0.0f; M13 = 0.0f; M14 = 0.0f; M15 = 1.0f;
		}

		public void SetToeros()
		{
			M0 = 0.0f; M1 = 0.0f; M2 = 0.0f; M3 = 0.0f;
			M4 = 0.0f; M5 = 0.0f; M6 = 0.0f; M7 = 0.0f;
			M8 = 0.0f; M9 = 0.0f; M10 = 0.0f; M11 = 0.0f;
			M12 = 0.0f; M13 = 0.0f; M14 = 0.0f; M15 = 0.0f;
		}

		public void CopyFrom(NEMatrix4x4 mat)
        {
			M0 = mat.M0; M1 = mat.M1; M2 = mat.M2; M3 = mat.M3;
			M4 = mat.M4; M5 = mat.M5; M6 = mat.M6; M7 = mat.M7;
			M8 = mat.M8; M9 = mat.M9; M10 = mat.M10; M11 = mat.M11;
			M12 = mat.M12; M13 = mat.M13; M14 = mat.M14; M15 = mat.M15;
		}

        public override string ToString()
        {
			return
				M0.ToString() + " " + M1.ToString() + " " + M2.ToString() + " " + M3.ToString() + "\n" +
				M4.ToString() + " " + M5.ToString() + " " + M6.ToString() + " " + M7.ToString() + "\n" +
				M8.ToString() + " " + M9.ToString() + " " + M10.ToString() + " " + M11.ToString() + "\n" +
				M12.ToString() + " " + M13.ToString() + " " + M14.ToString() + " " + M15.ToString() + "\n";
		}

		static public bool Compare(NEMatrix4x4 lhs, NEMatrix4x4 rhs)
		{
			return
				lhs.M0 == rhs.M0 && lhs.M1 == rhs.M1 && lhs.M2 == rhs.M2 && lhs.M3 == rhs.M3 &&
				lhs.M4 == rhs.M4 && lhs.M5 == rhs.M5 && lhs.M6 == rhs.M6 && lhs.M7 == rhs.M7 &&
				lhs.M8 == rhs.M8 && lhs.M9 == rhs.M9 && lhs.M10 == rhs.M10 && lhs.M11 == rhs.M11 &&
				lhs.M12 == rhs.M12 && lhs.M13 == rhs.M13 && lhs.M14 == rhs.M14 && lhs.M15 == rhs.M15;
		}

		static public bool Compare(NEMatrix4x4 lhs, NEMatrix4x4 rhs, float eps)
		{
			return
				Math.Abs(lhs.M0 - rhs.M0) <= eps && Math.Abs(lhs.M1 - rhs.M1) <= eps &&
				Math.Abs(lhs.M2 - rhs.M2) <= eps && Math.Abs(lhs.M3 - rhs.M3) <= eps &&
				Math.Abs(lhs.M4 - rhs.M4) <= eps && Math.Abs(lhs.M5 - rhs.M5) <= eps &&
				Math.Abs(lhs.M6 - rhs.M6) <= eps && Math.Abs(lhs.M7 - rhs.M7) <= eps &&
				Math.Abs(lhs.M8 - rhs.M8) <= eps && Math.Abs(lhs.M9 - rhs.M9) <= eps &&
				Math.Abs(lhs.M10 - rhs.M10) <= eps && Math.Abs(lhs.M11 - rhs.M11) <= eps &&
				Math.Abs(lhs.M12 - rhs.M12) <= eps && Math.Abs(lhs.M13 - rhs.M13) <= eps &&
				Math.Abs(lhs.M14 - rhs.M14) <= eps && Math.Abs(lhs.M15 - rhs.M15) <= eps;
		}


		static public NEMatrix4x4 operator *(NEMatrix4x4 lhs, NEMatrix4x4 rhs)
        {
            NEMatrix4x4 result = new NEMatrix4x4();

			result.M0 = lhs.M0 * rhs.M0 + lhs.M1 * rhs.M4
				      + lhs.M2 * rhs.M8 + lhs.M3 * rhs.M12;

			result.M4 = lhs.M4 * rhs.M0 + lhs.M5 * rhs.M4
					  + lhs.M6 * rhs.M8 + lhs.M7 * rhs.M12;

			result.M8 = lhs.M8 * rhs.M0 + lhs.M9 * rhs.M4
					  + lhs.M10 * rhs.M8 + lhs.M11 * rhs.M12;

			result.M12 = lhs.M12 * rhs.M0 + lhs.M13 * rhs.M4
					   + lhs.M14 * rhs.M8 + lhs.M15 * rhs.M12;

			//col1
			result.M1 = lhs.M0 * rhs.M1 + lhs.M1 * rhs.M5
					  + lhs.M2 * rhs.M9 + lhs.M3 * rhs.M13;

			result.M5 = lhs.M4 * rhs.M1 + lhs.M5 * rhs.M5
					  + lhs.M6 * rhs.M9 + lhs.M7 * rhs.M13;

			result.M9 = lhs.M8 * rhs.M1 + lhs.M9 * rhs.M5
					  + lhs.M10 * rhs.M9 + lhs.M11 * rhs.M13;

			result.M13 = lhs.M12 * rhs.M1 + lhs.M13 * rhs.M5
					   + lhs.M14 * rhs.M9 + lhs.M15 * rhs.M13;

			//col2
			result.M2 = lhs.M0 * rhs.M2 + lhs.M1 * rhs.M6
					  + lhs.M2 * rhs.M10 + lhs.M3 * rhs.M14;

			result.M6 = lhs.M4 * rhs.M2 + lhs.M5 * rhs.M6
					  + lhs.M6 * rhs.M10 + lhs.M7 * rhs.M14;

			result.M10 = lhs.M8 * rhs.M2 + lhs.M9 * rhs.M6
					   + lhs.M10 * rhs.M10 + lhs.M11 * rhs.M14;

			result.M14 = lhs.M12 * rhs.M2 + lhs.M13 * rhs.M6
					   + lhs.M14 * rhs.M10 + lhs.M15 * rhs.M14;

			//col3
			result.M3 = lhs.M0 * rhs.M3 + lhs.M1 * rhs.M7
					  + lhs.M2 * rhs.M11 + lhs.M3 * rhs.M15;

			result.M7 = lhs.M4 * rhs.M3 + lhs.M5 * rhs.M7
					  + lhs.M6 * rhs.M11 + lhs.M7 * rhs.M15;

			result.M11 = lhs.M8 * rhs.M3 + lhs.M9 * rhs.M7
					   + lhs.M10 * rhs.M11 + lhs.M11 * rhs.M15;

			result.M15 = lhs.M12 * rhs.M3 + lhs.M13 * rhs.M7
					   + lhs.M14 * rhs.M11 + lhs.M15 * rhs.M15;

			return result;
		}


        static public NEVector4 operator *(NEMatrix4x4 lhs, NEVector4 rhs)
        {

			float x = lhs.M0 * rhs.X + lhs.M1 * rhs.Y + lhs.M2 * rhs.Z + lhs.M3 * rhs.W;
			float y = lhs.M4 * rhs.X + lhs.M5 * rhs.Y + lhs.M6 * rhs.Z + lhs.M7 * rhs.W;
			float z = lhs.M8 * rhs.X + lhs.M9 * rhs.Y + lhs.M10 * rhs.Z + lhs.M11 * rhs.W;
			float w = lhs.M12 * rhs.X + lhs.M13 * rhs.Y + lhs.M14 * rhs.Z + lhs.M15 * rhs.W;

			return new NEVector4(x, y, z, w);
        }


        static public NEMatrix4x4 CreatePerspectiveProjection(float aspectRatio, float fovRad, float near, float far)
        {
            float invTanFov = 1.0f / ((float)Math.Tan(fovRad * 0.5f));
            float frustumZLength = far - near;
            if (frustumZLength == 0.0f) frustumZLength = 0.01f;
            float zScalingFactor = far / (frustumZLength + 0.001f);
            float zCorrection = zScalingFactor * near;

            NEMatrix4x4 mat = new NEMatrix4x4();
			mat.M0 = aspectRatio * invTanFov;
			mat.M5 = invTanFov;
			mat.M10 = zScalingFactor;
			mat.M11 = -zCorrection;
			mat.M14 = 1.0f;

			return mat;

        }

        static public NEMatrix4x4 CreateRotationX(float thetaRad)
        {
            float sinTheta = (float)Math.Sin(thetaRad);
            float cosTheta = (float)Math.Cos(thetaRad);

            NEMatrix4x4 mat = new NEMatrix4x4();
			mat.M0 = 1.0f;
			mat.M5 = cosTheta;
			mat.M6 = sinTheta;
			mat.M9 = -sinTheta;
			mat.M10 = cosTheta;
			mat.M15 = 1.0f;

            return mat;
        }


        static public NEMatrix4x4 CreateRotationY(float thetaRad)
        {
            float sinTheta = (float)Math.Sin(thetaRad);
            float cosTheta = (float)Math.Cos(thetaRad);

			NEMatrix4x4 mat = new NEMatrix4x4();
			mat.M0 = cosTheta;
			mat.M2 = -sinTheta;
			mat.M5 = 1.0f;
			mat.M8 = sinTheta;
			mat.M10 = cosTheta;
			mat.M15 = 1.0f;

            return mat;
        }

        static public NEMatrix4x4 CreateRotationZ(float thetaRad)
        {
            float sinTheta = (float)Math.Sin(thetaRad);
            float cosTheta = (float)Math.Cos(thetaRad);

			NEMatrix4x4 mat = new NEMatrix4x4();
			mat.M0 = cosTheta;
			mat.M1 = -sinTheta;
			mat.M4 = sinTheta;
			mat.M5 = cosTheta;
			mat.M10 = 1.0f;
			mat.M15 = 1.0f;

            return mat;
        }


        static public NEMatrix4x4 CreateTranslation(NEVector4 xyz)
        {
            NEMatrix4x4 mat = new NEMatrix4x4();
			mat.M0 = 1.0f;
			mat.M3 = xyz.X;
			mat.M5 = 1.0f;
			mat.M7 = xyz.Y;
			mat.M10 = 1.0f;
			mat.M11 = xyz.Z;
			mat.M15 = 1.0f;

            return mat;
        }

        static public NEMatrix4x4 CreateTranslation(float x, float y, float z)
        {
            NEMatrix4x4 mat = new NEMatrix4x4();
			mat.M0 = 1.0f;
			mat.M3 = x;
			mat.M5 = 1.0f;
			mat.M7 = y;
			mat.M10 = 1.0f;
			mat.M11 = z;
			mat.M15 = 1.0f;

            return mat;
        }

        public static NEMatrix4x4 CreateScale(NEVector4 xyz)
        {
            NEMatrix4x4 mat = new NEMatrix4x4();
			mat.M0 = xyz.X;
			mat.M5 = xyz.Y;
			mat.M10 = xyz.Z;
			mat.M15 = 1.0f;

            return mat;


        }

        public static NEMatrix4x4 CreateScale(float x, float y, float z)
        {
            NEMatrix4x4 mat = new NEMatrix4x4();
			mat.M0 = x;
			mat.M5 = y;
			mat.M10 = z;
			mat.M15 = 1.0f;

            return mat;
        }


        public static NEMatrix4x4 CreatePointAt(NEVector4 forward, NEVector4 up )
        {
			forward = forward.Normalized;
            up -=  (forward * NEVector4.Dot(up, forward));

			NEVector4 right = NEVector4.Cross3(up, forward).Normalized;

            NEMatrix4x4 mat = new NEMatrix4x4();
			mat.M0 = right.X;
			mat.M1 = up.X;
			mat.M2 = forward.X;
			mat.M4 = right.Y;
			mat.M5 = up.Y;
			mat.M6 = forward.Y;
			mat.M8 = right.Z;
			mat.M9 = up.Z;
			mat.M10 = forward.Z;
			mat.M15 = 1.0f;

            return mat;
        }

        public static NEMatrix4x4 RemoveTranslation(NEMatrix4x4 mat)
        {
            NEMatrix4x4 matRet = new NEMatrix4x4();
			matRet.CopyFrom(mat);
            matRet.M3 = 0;
            matRet.M7 = 0;
            matRet.M11 = 0;
            matRet.M15 = 1;

            return matRet;
        }


        public static NEMatrix4x4 CreateView(NEVector4 pos, NEVector4 forward, NEVector4 up)
        {
            forward = forward.Normalized;
            up -= (forward * NEVector4.Dot(up, forward));
            up = up.Normalized;

            NEVector4 right = NEVector4.Cross3(up, forward).Normalized;
            NEMatrix4x4 mat = new NEMatrix4x4();
			mat.M0 = right.X;
			mat.M1 = right.Y;
			mat.M2 = right.Z;
			mat.M3 = -NEVector4.Dot(pos, right);

			mat.M4 = up.X;
			mat.M5 = up.Y;
			mat.M6 = up.Z;
			mat.M7 = -NEVector4.Dot(pos, up);

			mat.M8 = forward.X;
			mat.M9 = forward.Y;
			mat.M10 = forward.Z;
			mat.M11 = -NEVector4.Dot(pos, forward);

			mat.M15 = 1.0f;

			return mat;
        }


        static public bool UnitTest_MatMatMultiply()
        {
            NEMatrix4x4 correctAnswer = new NEMatrix4x4();
			correctAnswer.M0 = 210.0f; correctAnswer.M1 = 267.0f; correctAnswer.M2 = 236.0f; correctAnswer.M3 = 271.0f;
			correctAnswer.M4 = 93.0f; correctAnswer.M5 = 149.0f; correctAnswer.M6 = 104.0f; correctAnswer.M7 = 149.0f;
			correctAnswer.M8 = 171.0f; correctAnswer.M9 = 146.0f; correctAnswer.M10 = 172.0f; correctAnswer.M11 = 268.0f;
			correctAnswer.M12 = 105.0f; correctAnswer.M13 = 169.0f; correctAnswer.M14 = 128.0f; correctAnswer.M15 = 169.0f;

			bool pass = Compare(GenerateTestMatrixA() * GenerateTestMatrixB(), correctAnswer);
            return pass;

        }

        static public bool UnitTest_MatVecMultiply()
        {
            NEMatrix4x4 inMat = new NEMatrix4x4();
			inMat.M0 = 9.0f; inMat.M1 = 2.0f; inMat.M2 = 6.0f; inMat.M3 = 4.0f;
			inMat.M4 = 6.0f; inMat.M5 = 2.0f; inMat.M6 = 5.0f; inMat.M7 = 5.0f;
			inMat.M8 = 1.0f; inMat.M9 = 2.0f; inMat.M10 = 4.0f; inMat.M11 = 5.0f;
			inMat.M12 = 5.0f; inMat.M13 = 9.0f; inMat.M14 = 2.0f; inMat.M15 = 4.0f;

            NEVector4 inVec = new NEVector4(2.0f, 3.0f, 6.0f, 2.0f);
            NEVector4 correctAnswer = new NEVector4(68.0f, 58.0f, 42.0f, 57.0f);

            bool pass = NEVector4.Compare(inMat * inVec, correctAnswer);
            return pass;

        }

        static public NEMatrix4x4 GenerateTestMatrixA()
        {
            NEMatrix4x4 tm = new NEMatrix4x4();
			tm.M0 = 5.0f; tm.M1 = 7.0f; tm.M2 = 9.0f; tm.M3 = 10.0f;
			tm.M4 = 2.0f; tm.M5 = 3.0f; tm.M6 = 3.0f; tm.M7 = 8.0f;
			tm.M8 = 8.0f; tm.M9 = 10.0f; tm.M10 = 2.0f; tm.M11 = 3.0f;
			tm.M12 = 3.0f; tm.M13 = 3.0f; tm.M14 = 4.0f; tm.M15 = 8.0f;

			return tm;
        }

        static public NEMatrix4x4 GenerateTestMatrixB()
        {
            NEMatrix4x4 tm = new NEMatrix4x4();
			tm.M0 = 3.0f; tm.M1 = 10.0f; tm.M2 = 12.0f; tm.M3 = 18.0f;
			tm.M4 = 12.0f; tm.M5 = 1.0f; tm.M6 = 4.0f; tm.M7 = 9.0f;
			tm.M8 = 9.0f; tm.M9 = 10.0f; tm.M10 = 12.0f; tm.M11 = 2.0f;
			tm.M12 = 3.0f; tm.M13 = 12.0f; tm.M14 = 4.0f; tm.M15 = 10.0f;

            return tm;
        }
    }
}
