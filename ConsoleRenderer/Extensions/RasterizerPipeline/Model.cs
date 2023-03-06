﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.RasterizerPipeline
{
    public enum CullMode { None = 0, Front = 1, Back = 2 }
    public class Model
    {
        public Mesh Mesh { get; set; }
        public NEColorTexture16 ColorTexture { get; set; }
        public NEFloatBuffer LumaTexture { get; set; }
        public Transform Transform { get; private set; }
        public CullMode FaceCull { get; set; }
        public Model(Mesh mesh, CullMode faceCull = CullMode.Back, NEColorTexture16 colorTexture = null, NEFloatBuffer lumaTexture = null)
        {
            Mesh = mesh;
            ColorTexture = colorTexture;
            LumaTexture = lumaTexture;
            Transform = new Transform();
            FaceCull = faceCull;
        }

        public Model(Mesh mesh, NEFloatBuffer lumaTexture): this(mesh, CullMode.Back, null, lumaTexture)
        {

        }

        public Model(Mesh mesh, CullMode faceCull, NEFloatBuffer lumaTexture) : this(mesh,faceCull, null, lumaTexture)
        {

        }

        public Model(Mesh mesh, NEColorTexture16 colorTexture) : this(mesh, CullMode.Back, colorTexture, null)
        {

        }

        public Model(Mesh mesh, NEColorTexture16 colorTexture, NEFloatBuffer lumaTexture) : this(mesh, CullMode.Back, colorTexture, lumaTexture)
        {

        }


    }
}
