using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;
namespace NostalgiaEngine.RasterizerPipeline
{
    public class Camera
    {
        public Transform Transform { get; private set; }
        public NEMatrix4x4 View { get; private set;}
        public NEMatrix4x4 RotationInv { get; private set; }

        public NEMatrix4x4 PointAt { get; private set;}


        public NEMatrix4x4 Projection { get; private set; }
        public float Near { get; private set; }
        public float Far { get; private set; }
        public float InverseFar { get; private set; }
        public float AspectRatio { get; private set; }
        public float InverseAspectRatio { get; private set; }
        public float FovRad { get; private set; }

        public Camera(int width, int height, float fovRad, float near, float far)
        {
            float dimLow = width;
            float dimHigh = height;
            if (dimLow > dimHigh) NEMathHelper.Swap(ref dimLow, ref dimHigh);
            float aspectRatio = dimLow / dimHigh;
            AspectRatio = aspectRatio;
            Transform = new Transform();
            Projection = NEMatrix4x4.CreatePerspectiveProjection(aspectRatio, fovRad, near, far);
            Near = near;
            Far = far;
            FovRad = fovRad;
            InverseFar = 1.0f / Far;
            InverseAspectRatio = 1.0f / AspectRatio;
        }

        public void UpdateTransform()
        {
            Transform.CalculateWorld();
            View = NEMatrix4x4.CreateView(Transform.LocalPosition, Transform.Forward, Transform.Up);
            PointAt = NEMatrix4x4.CreatePointAt(Transform.Forward, Transform.Up);
            RotationInv = NEMatrix4x4.RemoveTranslation(View);
        }
    }
}
