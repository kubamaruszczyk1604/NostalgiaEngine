using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.Extensions
{
    class ColorPair
    {
        public int CI0;
        public int CI1;
        public NEColorPalette Palette;
        NEVector4 col0;
        NEVector4 col1;

        public float DistCached;
        public float ColDistance;

        public float ColDistanceInv;
        public ColorPair(int ci0, int ci1, NEColorPalette palette)
        {
            CI0 = ci0;
            CI1 = ci1;
            Palette = palette;

            NEConsoleColorDef c0 = Palette.GetColor(CI0);
            NEConsoleColorDef c1 = Palette.GetColor(CI1);

            col0 = new NEVector4(c0.RNormalized, c0.GNormalized, c0.BNormalized, 0.0f);
            col1 = new NEVector4(c1.RNormalized, c1.GNormalized, c1.BNormalized, 0.0f);
            ColDistance = (col1 - col0).Length;
            ColDistanceInv = 1.0f/ColDistance;
        }

        public float GetDistanceToLine(float r, float g, float b)
        {

            NEVector4 c = new NEVector4(r, g, b, 0.0f);
            NEVector4 cProj = FindProjectedPoint(ref c, ref col0, ref col1);
            //float dist = (cProj - col0).Length;
            float dist = (c - cProj).Length;
            DistCached = (cProj - col0).Length;
            return dist;
        }

        public float CalculateLerpCoeff()
        {
            return DistCached * ColDistanceInv;
        }

        public static ColorPair[] GenerateColorPairs(NEColorPalette palette)
        {
            List<ColorPair> pairs = new List<ColorPair>();
            for (int i = 1; i < 16; ++i)
            {
                pairs.Add(new ColorPair(0, i, palette));
            }

            return pairs.ToArray();
        }

        private NEVector4 FindProjectedPoint(ref NEVector4 P, ref NEVector4 A, ref NEVector4 B)
        {
            NEVector4 d = (B - A) * ColDistanceInv;//(B - A).Length;
            NEVector4 v = P - A;
            float t = NEVector4.Dot(v, d);
            return A + d * t;
        }

    }
}
