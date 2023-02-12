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
        public NEMatrix4x4 RotationMat { get; private set; }

        public NEMatrix4x4 World { get; private set; }
       


        public NEVector4 LocalPosition { get { return m_Position; } set { m_Position = value; m_Position.W = 1.0f; } }
        public NEVector4 Rotation { get { return m_Rotation; } /*set { m_Rotation = value; m_Rotation.W = 0.0f; }*/ }
        public NEVector4 Forward { get { return RotationMat * NEVector4.Forward; } }
        public NEVector4 Up { get { return RotationMat * NEVector4.Up; } }
        public NEVector4 Right { get { return RotationMat * NEVector4.Right; } }

        public float PositionX { get { return m_Position.X; } set { m_Position.X = value; } }
        public float PositionY { get { return m_Position.Y; } set { m_Position.Y = value; } }
        public float PositionZ { get { return m_Position.Z; } set { m_Position.Z = value; } }

        public float RotationX { get { return m_Rotation.X; } /*set {m_Rotation.X = value; } */ }
        public float RotationY { get { return m_Rotation.Y; } /*set { m_Rotation.Y = value; }*/ }
        public float RotationZ { get { return m_Rotation.Z; } /*set { m_Rotation.Z = value; }*/ }

        public float Pitch { get { return m_Rotation.X; } /*set { m_Rotation.X = value; }*/ }
        public float Yaw { get { return m_Rotation.Y; } /*set { m_Rotation.Y = value; }*/ }
        public float Roll { get { return m_Rotation.Z; } /*set { m_Rotation.Z = value; }*/ }

        public float ScaleX { get { return m_Scale.X; } set { m_Scale.X = value; } }
        public float ScaleY { get { return m_Scale.Y; } set { m_Scale.Y = value; } }
        public float ScaleZ { get { return m_Scale.Z; } set { m_Scale.Z = value; } }

        public Transform(float posX, float posY, float posZ, 
            float rotationX, float rotationY, float rotationZ,
            float scaleX, float scaleY, float scaleZ)
        {
            m_Position = new NEVector4(posX, posY, posZ, 1.0f);
            m_Rotation = new NEVector4(rotationX, rotationY, rotationZ, 0.0f);
            m_Scale = new NEVector4(scaleX, scaleY, scaleZ, 0.0f);
            World = new NEMatrix4x4();
            RotationMat = new NEMatrix4x4(); 
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

        public void PointAt(NEVector4 direction, NEVector4 up)
        {
            RotationMat = NEMatrix4x4.CreatePointAt(direction, up);
        }

        public void PointAt(NEVector4 direction)
        {
            RotationMat = NEMatrix4x4.CreatePointAt(direction, NEVector4.Up);
        }

        public void RotateX(float thetaRad)
        {
            m_Rotation.X += thetaRad;
            RotationMat = NEMatrix4x4.CreateRotationX(m_Rotation.X) *
    NEMatrix4x4.CreateRotationY(m_Rotation.Y) * NEMatrix4x4.CreateRotationZ(m_Rotation.Z);
        }

        public void RotateY(float thetaRad)
        {
            m_Rotation.Y += thetaRad;
            RotationMat = NEMatrix4x4.CreateRotationX(m_Rotation.X) *
    NEMatrix4x4.CreateRotationY(m_Rotation.Y) * NEMatrix4x4.CreateRotationZ(m_Rotation.Z);
        }

        public void RotateZ(float thetaRad)
        {
            m_Rotation.Z += thetaRad;
            RotationMat = NEMatrix4x4.CreateRotationX(m_Rotation.X) *
    NEMatrix4x4.CreateRotationY(m_Rotation.Y) * NEMatrix4x4.CreateRotationZ(m_Rotation.Z);
        }

        public void Rotate(float x, float y, float z)
        {
            m_Rotation.X += x;
            m_Rotation.Y += y;
            m_Rotation.Z += z;
            RotationMat = NEMatrix4x4.CreateRotationX(m_Rotation.X) *
                NEMatrix4x4.CreateRotationY(m_Rotation.Y) * NEMatrix4x4.CreateRotationZ(m_Rotation.Z);
        }

        public void SetRotation(float x, float y, float z)
        {
            m_Rotation.X = x;
            m_Rotation.Y = y;
            m_Rotation.Z = z;
            RotationMat = NEMatrix4x4.CreateRotationX(m_Rotation.X) * 
                NEMatrix4x4.CreateRotationY(m_Rotation.Y) * NEMatrix4x4.CreateRotationZ(m_Rotation.Z);
        }

        public void CalculateWorld()
        {
            //RotationMat = NEMatrix4x4.CreateRotationX(m_Rotation.X) *
            //    NEMatrix4x4.CreateRotationY(m_Rotation.Y) * NEMatrix4x4.CreateRotationZ(m_Rotation.Z);


            World = NEMatrix4x4.CreateTranslation(m_Position)
            *RotationMat * NEMatrix4x4.CreateScale(m_Scale);
        }


    }
}
