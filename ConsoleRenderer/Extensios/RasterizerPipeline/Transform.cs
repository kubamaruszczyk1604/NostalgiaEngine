using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.RasterizerPipeline
{
    public class Transform
    {
        private NEVector4 m_Position;
        private NEVector4 m_Rotation;
        private NEVector4 m_Scale;

        public NEVector4 Position { get { return m_Position; } set { m_Position = value; m_Position.W = 1.0f; } }
        public NEVector4 Rotation { get { return m_Rotation; } set { m_Rotation = value; m_Rotation.W = 0.0f; } }

        public float PositionX { get { return m_Position.X; } set { m_Position.X = value; } }
        public float PositionY { get { return m_Position.Y; } set { m_Position.Y = value; } }
        public float PositionZ { get { return m_Position.Z; } set { m_Position.Z = value; } }

        public float RotationX { get { return m_Rotation.X; } set {m_Rotation.X = value; }  }
        public float RotationY { get { return m_Rotation.Y; } set { m_Rotation.Y = value; } }
        public float RotationZ { get { return m_Rotation.Z; } set { m_Rotation.Z = value; } }

        public float Pitch { get { return m_Rotation.X; } set { m_Rotation.X = value; } }
        public float Yaw { get { return m_Rotation.Y; } set { m_Rotation.Y = value; } }
        public float Roll { get { return m_Rotation.Z; } set { m_Rotation.Z = value; } }

        public float ScaleX { get { return m_Scale.X; } set { m_Scale.X = value; } }
        public float ScaleY { get { return m_Scale.Y; } set { m_Scale.Y = value; } }
        public float ScaleZ { get { return m_Scale.Z; } set { m_Scale.Z = value; } }

        public Transform(float posX, float posY, float posZ, 
            float rotationX, float rotationY, float rotationZ,
            float scaleX, float scaleY, float scaleZ)
        {
            m_Position = new NEVector4(posX, posY, posZ, 1.0f);
            m_Rotation = new NEVector4(rotationX, rotationY, rotationZ, 0.0f);
            m_Scale = new NEVector4(m_Scale.X, m_Scale.Y, m_Scale.Z, 0.0f);
        }

        public Transform(float posX, float posY, float posZ,
                            float rotationX, float rotationY, float rotationZ):
            this(posX, posY, posZ, rotationX, rotationY, rotationZ, 1.0f, 1.0f, 1.0f)
        { }

        public Transform(float posX, float posY, float posZ) :
            this(posX, posY, posZ, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f)
        { }

        public Transform() : this(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f)
        { }


        public void PointAt(NEVector4 target)
        {
            NEVector4 forward = target - m_Position;
            forward.W = 0;
        }
    }
}
