using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.RasterizerPipeline
{
    public enum RejectCriteria { LessThan = 0, GreaterThan = 1 }
   public class ClipPlane
    {
        public NEPlane Plane { get; }
        public float Treshold { get; }
        public RejectCriteria RejectCriteria { get; }
        public Axis Axis { get; }

       private ClipPlane(Axis axis,  RejectCriteria comparison)
        {
            Axis = axis;
            RejectCriteria = comparison;
            NEVector4 p = NEVector4.Zero;
            NEVector4 n = NEVector4.Zero;
            if(Axis == Axis.X)
            {
               
                if (comparison == RejectCriteria.LessThan)  // left plane
                {
                    p = NEVector4.Left - new NEVector4(0.01f,0.0f,0.0f,0.0f);
                    n = NEVector4.Right;
                }
                else  // right plane                                
                {
                    p = NEVector4.Right; 
                    n = NEVector4.Left; 
                }
                Treshold = p.X;
            }
            else if(Axis == Axis.Y)
            {
                if(comparison == RejectCriteria.LessThan) // bottom plane
                {
                    p = NEVector4.Down - new NEVector4(0.0f, 0.01f, 0.0f, 0.0f); ;
                    n = NEVector4.Up;
                }
                else   // top plane
                {
                    p = NEVector4.Up;
                    n = NEVector4.Down;
                }
                Treshold = p.Y;
            }
            else if(Axis == Axis.Z)
            {
                if(comparison == RejectCriteria.GreaterThan)
                {
                    p = NEVector4.Forward;
                    n = NEVector4.Back;
                    Treshold = p.Z;
                }
                else
                {
                    Treshold = 0.01f;
                    p = new NEVector4(0.0f, 0.0f, Treshold);
                    n = NEVector4.Forward;
                }
            }

            Plane = new NEPlane(p, n);

        }

        static public ClipPlane Left { get { return new ClipPlane(Axis.X, RejectCriteria.LessThan); } }
        static public ClipPlane Right { get { return new ClipPlane(Axis.X, RejectCriteria.GreaterThan); } }
        static public ClipPlane Top { get { return new ClipPlane(Axis.Y, RejectCriteria.GreaterThan); } }
        static public ClipPlane Bottom { get { return new ClipPlane(Axis.Y, RejectCriteria.LessThan); } }
        static public ClipPlane Near { get { return new ClipPlane(Axis.Z,  RejectCriteria.LessThan); } }
        static public ClipPlane Far { get { return new ClipPlane(Axis.Z, RejectCriteria.GreaterThan); } }
    }
}
