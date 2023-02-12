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
        public Mesh Geometry { get; set; }
        public NETexture ColorTexture { get; set; }
        public NEFloatBuffer LumaTexture { get; set; }
    }
}
