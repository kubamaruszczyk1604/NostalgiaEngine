using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.RasterizerPipeline
{
    class Skybox
    {
        private NEFloatBuffer[] m_Walls;

        public Skybox(string path)
        {
            m_Walls = new NEFloatBuffer[6];
            m_Walls[0] = ResourceManager.Instance.GetLumaTexture(path + "/right.buf");
            m_Walls[1] = ResourceManager.Instance.GetLumaTexture(path + "/left.buf");
            m_Walls[2] = ResourceManager.Instance.GetLumaTexture(path + "/top.buf");
            m_Walls[3] = ResourceManager.Instance.GetLumaTexture(path + "/bottom.buf");
            m_Walls[4] = ResourceManager.Instance.GetLumaTexture(path + "/front.buf");
            m_Walls[5] = ResourceManager.Instance.GetLumaTexture(path + "/back.buf");
        }


        public float Sample(NEVector4 direction)
        {
            float index = 0.0f;
            NEVector2 uv = SampleCube(direction, out index);

            int i = (int)index;

           return m_Walls[i].Sample(uv.X, uv.Y);
        }


        // The following methos is adapted from code published by L.Spiro on  gamedev.net.
        // source: https://www.gamedev.net/forums/topic/687535-implementing-a-cube-map-lookup-function/5337472/

        NEVector2 SampleCube(NEVector4 v, out float faceIndex)
        {

            NEVector4 vAbs = NEVector4.Abs(v);
            float ma;
            NEVector2 uv;
            if (vAbs.Z >= vAbs.X && vAbs.Z >= vAbs.Y)
            {
                faceIndex = v.Z < 0.0f ? 5.0f : 4.0f;
                ma = 0.5f / vAbs.Z;
                uv = new NEVector2(v.Z < 0.0 ? -v.X : v.X, -v.Y);
            }
            else if (vAbs.Y >= vAbs.X)
            {
                faceIndex = v.Y < 0.0f ? 3.0f : 2.0f;
                ma = 0.5f / vAbs.Y;
                uv = new NEVector2(v.X, v.Y < 0.0 ? -v.Z : v.Z);
            }
            else
            {
                faceIndex = v.X < 0.0f ? 1.0f : 0.0f;
                ma = 0.5f / vAbs.X;
                uv = new NEVector2(v.X < 0.0f ? v.Z : -v.Z, -v.Y);
            }
            return uv * ma + new NEVector2(0.5f, 0.5f);
        }

    }
}
