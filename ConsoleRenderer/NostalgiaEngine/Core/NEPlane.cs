using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NostalgiaEngine.Core
{
    public class NEPlane
    {
        public NEVector4 P { get; set; }
        public NEVector4 N { get; set; }


        public NEPlane(NEVector4 point, NEVector4 normal)
        {
            P = point;
            N = normal;
        }

        static public PlaneIntersectionManifest IntersectionWithLineSegment(NEVector4 l0, NEVector4 l1, NEPlane plane)
        {
            NEVector4 p0 = plane.P;
            NEVector4 n = plane.N;
            l0.W = 0; l1.W = 0; p0.W = 0; n.W = 0;
            PlaneIntersectionManifest m = new PlaneIntersectionManifest();
            if (!NEMathHelper.FindRayEquation(l0, l1, out m.RayDirection, out m.RayLength))
            {
                m.Intersected = false;
                return m;
            }
            float numerator = NEVector4.Dot(p0 - l0, n);
            float denominator = NEVector4.Dot(m.RayDirection, n);

            if (denominator == 0)
            {
                m.Intersected = false;
                return m;
            }

            m.Magnitude = numerator / denominator;
            m.MagnitudeNormalized = m.Magnitude / m.RayLength;
            if (m.MagnitudeNormalized >= 1.0f || m.MagnitudeNormalized <= 0.0f)
            {
                m.Intersected = false;
                return m;
            }
            m.Intersected = true;
            return m;
        }


    }
}
