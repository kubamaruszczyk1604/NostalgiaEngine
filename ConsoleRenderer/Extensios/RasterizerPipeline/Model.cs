using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.RasterizerPipeline
{
    public class Model
    {
        public Mesh Mesh { get; set; }
        public NEColorTexture16 ColorTexture { get; set; }
        public NEFloatBuffer LumaTexture { get; set; }
        public Transform Transform { get; private set; }

        public Model(Mesh mesh, NEColorTexture16 colorTexture = null, NEFloatBuffer lumaTexture = null)
        {
            Mesh = mesh;
            ColorTexture = colorTexture;
            LumaTexture = lumaTexture;
            Transform = new Transform();
        }

        public Model(Mesh mesh, NEFloatBuffer lumaTexture): this(mesh, null, lumaTexture)
        {

        }

        
    }
}
