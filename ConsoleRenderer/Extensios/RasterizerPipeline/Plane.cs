using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.RasterizerPipeline
{
    public class Plane
    {
        NEVector4 Point { get; set; }
        NEVector4 Normal { get; set; }


        public Plane(NEVector4 point, NEVector4 normal)
        {
            Point = point;
            Normal = normal;
        }

        static public Plane Left { get { return new Plane(NEVector4.Left, NEVector4.Right); } }
        static public Plane Right { get { return new Plane(NEVector4.Right, NEVector4.Left); } }
        static public Plane Top { get { return new Plane(NEVector4.Up, NEVector4.Down); } }
        static public Plane Bottom { get { return new Plane(NEVector4.Down, NEVector4.Up); } }
        static public Plane Near { get { return new Plane(new NEVector4(0.0f, 0.0f,0.01f), NEVector4.Forward); } }
        static public Plane Far { get { return new Plane(new NEVector4(0.0f, 0.0f, 1.0f), NEVector4.Back); } }
    }
}
