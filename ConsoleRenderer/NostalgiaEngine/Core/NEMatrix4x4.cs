using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{
    public class NEMatrix4x4
    {
        private float[,] m_Data;

        public NEMatrix4x4()
        {
            m_Data = new float[,] { { 1.0f, 0.0f, 0.0f, 0.0f },
                                    { 0.0f, 1.0f, 0.0f, 0.0f },
                                    { 0.0f, 0.0f, 1.0f, 0.0f },
                                    { 0.0f, 0.0f, 0.0f, 1.0f }};
        }

        public NEMatrix4x4(NEMatrix4x4 mat)
        {
            m_Data = new float[,] { { mat.m_Data[0,0],  mat.m_Data[0,1], mat.m_Data[0,2], mat.m_Data[0,3]},
                                    { mat.m_Data[1,0],  mat.m_Data[1,1], mat.m_Data[1,2], mat.m_Data[1,3]},
                                    { mat.m_Data[2,0],  mat.m_Data[2,1], mat.m_Data[2,2], mat.m_Data[2,3]},
                                    { mat.m_Data[3,0],  mat.m_Data[3,1], mat.m_Data[3,2], mat.m_Data[3,3]}};
        }




        public override string ToString()
        {
            string ret = "";

            for (int i = 0; i < 4; ++i)
            {
                ret += m_Data[i, 0].ToString() + " " + m_Data[i, 1].ToString()
                    + " " + m_Data[i, 2].ToString() + " " + m_Data[i, 3].ToString() + "\n";
            }

            return ret;
        }



        static public bool Compare(NEMatrix4x4 lhs, NEMatrix4x4 rhs)
        {
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    if (lhs.m_Data[i, j] != rhs.m_Data[i, j]) return false;
                }
            }
            return true;
        }


        static public NEMatrix4x4 operator *(NEMatrix4x4 lhs, NEMatrix4x4 rhs)
        {
            NEMatrix4x4 result = new NEMatrix4x4();
            //col0
            result.m_Data[0, 0] = lhs.m_Data[0, 0] * rhs.m_Data[0, 0] + lhs.m_Data[0, 1] * rhs.m_Data[1, 0]
                                + lhs.m_Data[0, 2] * rhs.m_Data[2, 0] + lhs.m_Data[0, 3] * rhs.m_Data[3, 0];

            result.m_Data[1, 0] = lhs.m_Data[1, 0] * rhs.m_Data[0, 0] + lhs.m_Data[1, 1] * rhs.m_Data[1, 0]
                                + lhs.m_Data[1, 2] * rhs.m_Data[2, 0] + lhs.m_Data[1, 3] * rhs.m_Data[3, 0];

            result.m_Data[2, 0] = lhs.m_Data[2, 0] * rhs.m_Data[0, 0] + lhs.m_Data[2, 1] * rhs.m_Data[1, 0]
                                + lhs.m_Data[2, 2] * rhs.m_Data[2, 0] + lhs.m_Data[2, 3] * rhs.m_Data[3, 0];

            result.m_Data[3, 0] = lhs.m_Data[3, 0] * rhs.m_Data[0, 0] + lhs.m_Data[3, 1] * rhs.m_Data[1, 0]
                                + lhs.m_Data[3, 2] * rhs.m_Data[2, 0] + lhs.m_Data[3, 3] * rhs.m_Data[3, 0];

            //col1
            result.m_Data[0, 1] = lhs.m_Data[0, 0] * rhs.m_Data[0, 1] + lhs.m_Data[0, 1] * rhs.m_Data[1, 1]
                                + lhs.m_Data[0, 2] * rhs.m_Data[2, 1] + lhs.m_Data[0, 3] * rhs.m_Data[3, 1];

            result.m_Data[1, 1] = lhs.m_Data[1, 0] * rhs.m_Data[0, 1] + lhs.m_Data[1, 1] * rhs.m_Data[1, 1]
                                + lhs.m_Data[1, 2] * rhs.m_Data[2, 1] + lhs.m_Data[1, 3] * rhs.m_Data[3, 1];

            result.m_Data[2, 1] = lhs.m_Data[2, 0] * rhs.m_Data[0, 1] + lhs.m_Data[2, 1] * rhs.m_Data[1, 1]
                                + lhs.m_Data[2, 2] * rhs.m_Data[2, 1] + lhs.m_Data[2, 3] * rhs.m_Data[3, 1];

            result.m_Data[3, 1] = lhs.m_Data[3, 0] * rhs.m_Data[0, 1] + lhs.m_Data[3, 1] * rhs.m_Data[1, 1]
                                + lhs.m_Data[3, 2] * rhs.m_Data[2, 1] + lhs.m_Data[3, 3] * rhs.m_Data[3, 1];

            //col2
            result.m_Data[0, 2] = lhs.m_Data[0, 0] * rhs.m_Data[0, 2] + lhs.m_Data[0, 1] * rhs.m_Data[1, 2]
                                + lhs.m_Data[0, 2] * rhs.m_Data[2, 2] + lhs.m_Data[0, 3] * rhs.m_Data[3, 2];

            result.m_Data[1, 2] = lhs.m_Data[1, 0] * rhs.m_Data[0, 2] + lhs.m_Data[1, 1] * rhs.m_Data[1, 2]
                                + lhs.m_Data[1, 2] * rhs.m_Data[2, 2] + lhs.m_Data[1, 3] * rhs.m_Data[3, 2];

            result.m_Data[2, 2] = lhs.m_Data[2, 0] * rhs.m_Data[0, 2] + lhs.m_Data[2, 1] * rhs.m_Data[1, 2]
                                + lhs.m_Data[2, 2] * rhs.m_Data[2, 2] + lhs.m_Data[2, 3] * rhs.m_Data[3, 2];

            result.m_Data[3, 2] = lhs.m_Data[3, 0] * rhs.m_Data[0, 2] + lhs.m_Data[3, 1] * rhs.m_Data[1, 2]
                                + lhs.m_Data[3, 2] * rhs.m_Data[2, 2] + lhs.m_Data[3, 3] * rhs.m_Data[3, 2];


            //col3
            result.m_Data[0, 3] = lhs.m_Data[0, 0] * rhs.m_Data[0, 3] + lhs.m_Data[0, 1] * rhs.m_Data[1, 3]
                                + lhs.m_Data[0, 2] * rhs.m_Data[2, 3] + lhs.m_Data[0, 3] * rhs.m_Data[3, 3];

            result.m_Data[1, 3] = lhs.m_Data[1, 0] * rhs.m_Data[0, 3] + lhs.m_Data[1, 1] * rhs.m_Data[1, 3]
                                + lhs.m_Data[1, 2] * rhs.m_Data[2, 3] + lhs.m_Data[1, 3] * rhs.m_Data[3, 3];

            result.m_Data[2, 3] = lhs.m_Data[2, 0] * rhs.m_Data[0, 3] + lhs.m_Data[2, 1] * rhs.m_Data[1, 3]
                                + lhs.m_Data[2, 2] * rhs.m_Data[2, 3] + lhs.m_Data[2, 3] * rhs.m_Data[3, 3];

            result.m_Data[3, 3] = lhs.m_Data[3, 0] * rhs.m_Data[0, 3] + lhs.m_Data[3, 1] * rhs.m_Data[1, 3]
                                + lhs.m_Data[3, 2] * rhs.m_Data[2, 3] + lhs.m_Data[3, 3] * rhs.m_Data[3, 3];


            return result;
        }


        static public NEVector4 operator *(NEMatrix4x4 lhs, NEVector4 rhs)
        {

            float x = lhs.m_Data[0, 0] * rhs.X + lhs.m_Data[0, 1] * rhs.Y + lhs.m_Data[0, 2] * rhs.Z + lhs.m_Data[0, 3] * rhs.W;
            float y = lhs.m_Data[1, 0] * rhs.X + lhs.m_Data[1, 1] * rhs.Y + lhs.m_Data[1, 2] * rhs.Z + lhs.m_Data[1, 3] * rhs.W;
            float z = lhs.m_Data[2, 0] * rhs.X + lhs.m_Data[2, 1] * rhs.Y + lhs.m_Data[2, 2] * rhs.Z + lhs.m_Data[2, 3] * rhs.W;
            float w = lhs.m_Data[3, 0] * rhs.X + lhs.m_Data[3, 1] * rhs.Y + lhs.m_Data[3, 2] * rhs.Z + lhs.m_Data[3, 3] * rhs.W;
            return new NEVector4(x, y, z, w);
        }


        static public NEMatrix4x4 CreatePerspectiveProjection(float aspectRatio, float fovRad, float near, float far)
        {
            float invTanFov = 1.0f / (float)Math.Tan(fovRad * 0.5f);
            float frustumZLength = far - near;
            float zScalingFactor = far / frustumZLength;
            float zCorrection = zScalingFactor * near;
            NEMatrix4x4 mat = new NEMatrix4x4();
            mat.m_Data = new float[,] { { aspectRatio*invTanFov, 0.0f, 0.0f, 0.0f },
                                        { 0.0f, invTanFov, 0.0f, 0.0f },
                                        { 0.0f, 0.0f, zScalingFactor, -zCorrection},
                                        { 0.0f, 0.0f, 1.0f, 0.0f }};
            return mat;

        }

        static public NEMatrix4x4 CreateRotationX(float thetaRad)
        {
            float sinTheta = (float)Math.Sin(thetaRad);
            float cosTheta = (float)Math.Cos(thetaRad);
            NEMatrix4x4 mat = new NEMatrix4x4();
            mat.m_Data = new float[,] { { 1.0f, 0.0f, 0.0f, 0.0f },
                                        { 0.0f, cosTheta, sinTheta, 0.0f },
                                        { 0.0f,-sinTheta, cosTheta, 0.0f},
                                        { 0.0f, 0.0f, 0.0f, 1.0f }};
            return mat;
        }


        static public NEMatrix4x4 CreateRotationY(float thetaRad)
        {
            float sinTheta = (float)Math.Sin(thetaRad);
            float cosTheta = (float)Math.Cos(thetaRad);
            NEMatrix4x4 mat = new NEMatrix4x4();
            mat.m_Data = new float[,] { { cosTheta, 0.0f,-sinTheta, 0.0f },
                                        { 0.0f, 1.0f, 0.0f, 0.0f },
                                        { sinTheta, 0.0f, cosTheta, 0.0f},
                                        { 0.0f, 0.0f, 0.0f, 1.0f }};


            return mat;
        }

        static public NEMatrix4x4 CreateRotationZ(float thetaRad)
        {
            float sinTheta = (float)Math.Sin(thetaRad);
            float cosTheta = (float)Math.Cos(thetaRad);
            NEMatrix4x4 mat = new NEMatrix4x4();
            mat.m_Data = new float[,] { { cosTheta,-sinTheta, 0.0f, 0.0f },
                                        { sinTheta, cosTheta, 0.0f, 0.0f },
                                        { 0.0f, 0.0f, 1.0f, 0.0f},
                                        { 0.0f, 0.0f, 0.0f, 1.0f }};
            return mat;
        }


        static public NEMatrix4x4 CreateTranslation(NEVector4 xyz)
        {
            NEMatrix4x4 mat = new NEMatrix4x4();
            mat.m_Data = new float[,] { { 1.0f, 0.0f, 0.0f, xyz.X },
                                        { 0.0f, 1.0f, 0.0f, xyz.Y },
                                        { 0.0f, 0.0f, 1.0f, xyz.Z },
                                        { 0.0f, 0.0f, 0.0f, 1.0f }};
            return mat;
        }

        static public NEMatrix4x4 CreateTranslation(float x, float y, float z)
        {
            NEMatrix4x4 mat = new NEMatrix4x4();
            mat.m_Data = new float[,] { { 1.0f, 0.0f, 0.0f, x },
                                        { 0.0f, 1.0f, 0.0f, y },
                                        { 0.0f, 0.0f, 1.0f, z },
                                        { 0.0f, 0.0f, 0.0f, 1.0f }};
            return mat;
        }

        public static NEMatrix4x4 CreateScale(NEVector4 xyz)
        {
            NEMatrix4x4 mat = new NEMatrix4x4();
            mat.m_Data = new float[,] { { xyz.X, 0.0f, 0.0f, 0 },
                                        { 0.0f, xyz.Y, 0.0f, 0},
                                        { 0.0f, 0.0f, xyz.Z, 0 },
                                        { 0.0f, 0.0f, 0.0f, 1.0f }};
            return mat;
        }

        public static NEMatrix4x4 CreateScale(float x, float y, float z)
        {
            NEMatrix4x4 mat = new NEMatrix4x4();
            mat.m_Data = new float[,] { { x, 0.0f, 0.0f, 0 },
                                        { 0.0f, y, 0.0f, 0},
                                        { 0.0f, 0.0f, z, 0 },
                                        { 0.0f, 0.0f, 0.0f, 1.0f }};
            return mat;
        }


        public static NEMatrix4x4 CreatePointAt(NEVector4 forward, NEVector4 up )
        {
            forward = forward.Normalized;
            up -=  (forward * NEVector4.Dot(up, forward));
            
            NEVector4 right = NEVector4.Cross3(up, forward).Normalized;

            NEMatrix4x4 mat = new NEMatrix4x4();
            mat.m_Data = new float[,] { { right.X, up.X, forward.X, 0},
                                        { right.Y, up.Y, forward.Y, 0},
                                        { right.Z, up.Z, forward.Z, 0},
                                        { 0.0f, 0.0f, 0.0f, 1.0f }};
            return mat;
        }

        public static NEMatrix4x4 RemoveTranslation(NEMatrix4x4 mat)
        {
            NEMatrix4x4 matRet = new NEMatrix4x4(mat);
            matRet.m_Data[0, 3] = 0;
            matRet.m_Data[1, 3] = 0;
            matRet.m_Data[2, 3] = 0;
            matRet.m_Data[3, 3] = 1;
            return matRet;
        }

        //public static NEMatrix4x4 CreateView(NEVector4 pos, NEVector4 forward, NEVector4 up)
        //{
        //    forward = forward.Normalized;
        //    up -= (forward * NEVector4.Dot(up, forward));

        //    NEVector4 right = NEVector4.Cross3(forward, up).Normalized;

        //    NEMatrix4x4 mat = new NEMatrix4x4();
        //    mat.m_Data = new float[,] { 
        //                                { right.X, right.Y, right.Z, NEVector4.Dot(pos,right)},
        //                                { up.X, up.Y, up.Z, -NEVector4.Dot(pos, up)},
        //                                { forward.X, forward.Y, forward.Z, -NEVector4.Dot(pos,forward) },
        //                                { 0.0f, 0.0f, 0.0f, 1.0f }};
        //    return mat;
        //}

        public static NEMatrix4x4 CreateView(NEVector4 pos, NEVector4 forward, NEVector4 up)
        {
            forward = forward.Normalized;
            up -= (forward * NEVector4.Dot(up, forward));
            up = up.Normalized;

            NEVector4 right = NEVector4.Cross3(up, forward).Normalized;
            NEMatrix4x4 mat = new NEMatrix4x4();
            mat.m_Data = new float[,] {
                                        { right.X, right.Y, right.Z, -NEVector4.Dot(pos,right)},
                                        { up.X, up.Y, up.Z, -NEVector4.Dot(pos, up)},
                                        { forward.X, forward.Y, forward.Z, -NEVector4.Dot(pos,forward) },
                                        { 0.0f, 0.0f, 0.0f, 1.0f }};

            return mat;
        }


        static public bool UnitTest_MatMatMultiply()
        {
            NEMatrix4x4 correctAnswer = new NEMatrix4x4();
            correctAnswer.m_Data = new float[,] { { 210.0f, 267.0f, 236.0f, 271.0f },
                                                { 93.0f, 149.0f, 104.0f, 149.0f },
                                                { 171.0f, 146.0f, 172.0f, 268.0f },
                                                { 105.0f, 169.0f, 128.0f, 169.0f }};

            bool pass = Compare(GenerateTestMatrixA() * GenerateTestMatrixB(), correctAnswer);
            return pass;

        }

        static public bool UnitTest_MatVecMultiply()
        {
            NEMatrix4x4 inMat = new NEMatrix4x4();
            inMat.m_Data = new float[,] { { 9.0f, 2.0f, 6.0f, 4.0f },
                                          { 6.0f, 2.0f, 5.0f, 5.0f },
                                          { 1.0f, 2.0f, 4.0f, 5.0f },
                                          { 5.0f, 9.0f, 2.0f, 4.0f }};

            NEVector4 inVec = new NEVector4(2.0f, 3.0f, 6.0f, 2.0f);
            NEVector4 correctAnswer = new NEVector4(68.0f, 58.0f, 42.0f, 57.0f);

            bool pass = NEVector4.Compare(inMat * inVec, correctAnswer);
            return pass;

        }

        static public NEMatrix4x4 GenerateTestMatrixA()
        {
            NEMatrix4x4 tm = new NEMatrix4x4();
            tm.m_Data = new float[,] { { 5.0f, 7.0f, 9.0f, 10.0f },
                                       { 2.0f, 3.0f, 3.0f, 8.0f },
                                       { 8.0f, 10.0f, 2.0f, 3.0f },
                                       { 3.0f, 3.0f, 4.0f, 8.0f }};
            return tm;
        }

        static public NEMatrix4x4 GenerateTestMatrixB()
        {
            NEMatrix4x4 tm = new NEMatrix4x4();
            tm.m_Data = new float[,] { { 3.0f, 10.0f, 12.0f, 18.0f },
                                       { 12.0f, 1.0f, 4.0f, 9.0f },
                                       { 9.0f, 10.0f, 12.0f, 2.0f },
                                       { 3.0f, 12.0f, 4.0f, 10.0f }};
            return tm;
        }
    }
}
