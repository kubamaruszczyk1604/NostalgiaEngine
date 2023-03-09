using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.RasterizerPipeline
{
    public class Mesh
    {

        public List<Vertex> Vertices { get; private set; }
        public List<Triangle> Triangles { get; private set; }


        public Mesh()
        {
            Vertices = new List<Vertex>(100);
            Triangles = new List<Triangle>(100);
        }



        public void AddVertex(Vertex v)
        {
            Vertices.Add(v);
        }

        public void AddVertex(float x, float y, float z)
        {
            Vertices.Add(new Vertex(x, y, z));
        }

        public void AddVertex(float x, float y, float z, float u, float v)
        {
            Vertices.Add(new Vertex(x, y, z, u, v));
        }

        public void AddTriangle(int i0, int i1, int i2)
        {
            Triangles.Add(new Triangle(i0, i1, i2, this));
        }



        static public int GetLeftmost(List<Vertex> vertices, NEVector4 n, int iPt, int iA, int iB, int iC)
        {
            NEVector4 vA = vertices[iA].Position - vertices[iPt].Position;
            NEVector4 vB = vertices[iB].Position - vertices[iPt].Position;
            NEVector4 vC = vertices[iC].Position - vertices[iPt].Position;

            int winnerIndex = -1;
            NEVector4 winnerVec = vA;
            //first round
            if(NEVector4.CompareLeft(vA,vB,n))
            {
                winnerIndex = iA;
                winnerVec = vA;
            }
            else
            {
                winnerIndex = iB;
                winnerVec = vB;
            }

            //second round
            if (NEVector4.CompareLeft(winnerVec, vC, n))
            {
                //winnerIndex = iA;
                //winnerVec = vA;
            }
            else
            {
                winnerIndex = iC;
                winnerVec = vC;
            }

            return winnerIndex;
        }

        static public int GetLeftmost(List<Vertex> vertices, NEVector4 n, int iPt, int iA, int iB)
        {
            NEVector4 vA = vertices[iA].Position - vertices[iPt].Position;
            NEVector4 vB = vertices[iB].Position - vertices[iPt].Position;
            if (NEVector4.CompareLeft(vA, vB, n))
            {
                return iA;
            }

            return iB;
        }
     }
}
