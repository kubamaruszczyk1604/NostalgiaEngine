﻿using System;
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
        public NEMatrix4x4 View
        {
            get
            {
                return NEMatrix4x4.CreateView(Transform.Position, Transform.Forward, Transform.Up);
            }
        }


        public NEMatrix4x4 Projection { get; private set; }

        public Camera(int width, int height, float fovRad, float near, float far)
        {
            float dimLow = width;
            float dimHigh = height;
            if (dimLow > dimHigh) NEMathHelper.Swap(ref dimLow, ref dimHigh);
            float aspectRatio = dimLow / dimHigh;
            Transform = new Transform();
            Projection = NEMatrix4x4.CreatePerspectiveProjection(aspectRatio, fovRad, near, far);
        }
    }
}
