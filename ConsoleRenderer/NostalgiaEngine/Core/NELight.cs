using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{
    public enum NELightType { Directional = 0, Point = 1 }; 
    abstract public class NELight: NESceneObject
    {
        public NELightType LightType { get; private set; }
        protected NELight(NELightType type, NEVector4 position): base()
        {
            LightType = type;
            Transform.LocalPosition = position;
        }
    }

    public class DirectionalLight: NELight
    {
        public NEVector4 Direction { get; set; }
        public float Intensity { get; set; }
        
        public DirectionalLight(NEVector4 direction, float intenisty = 1.0f): base(NELightType.Directional, NEVector4.Zero)
        {
            Direction = direction.Normalized;
            Intensity = Intensity;
        }
    }


}
