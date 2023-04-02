using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{
    public abstract class NEMaterial
    {

        public NEFloatBuffer LumaTexture { get; set; }
        public int[] CharacterSpectrum = NECHAR_RAMPS.CHAR_RAMP_10;
        public float LitCoefficient
        {
            get { return m_LitCoefficient; }
            set { m_LitCoefficient = NEMath.Clamp(value, 0.0f, 1.0f); }
        }

        private float m_LitCoefficient;

        protected NEMaterial(NEFloatBuffer lumaTex, float litCoeff = 1.0f)
        {
            LumaTexture = lumaTex;
            LitCoefficient = litCoeff;
        }


        //public abstract float SampleLuma(float u, float v);
    }


    public class NEMaterialTex: NEMaterial
    {
        public NEColorTexture16 LitTexture { get; set; }

        public NEMaterialTex(NEColorTexture16 litTexture, NEFloatBuffer lumaTex = null): base(lumaTex)
        {
            LitTexture = litTexture;
        }


    }

    public class NEMaterialLuma: NEMaterial
    {
        public int LitColor { get; set; }
        public int UnlitColor { get; set; }

        public NEMaterialLuma(int litColor, int unlitColor, NEFloatBuffer lumaTexture) : base(lumaTexture)
        {
            LitColor = litColor;
            UnlitColor = unlitColor;
        }

    }
}
