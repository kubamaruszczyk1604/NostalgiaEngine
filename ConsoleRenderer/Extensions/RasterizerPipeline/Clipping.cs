using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.RasterizerPipeline
{
    public class Clipping
    {
        static public bool DebugMode { get; set; }
        static public void VertsToTris(Mesh mesh, Triangle triangle, int va, int vb, int vc, int vd, List<Triangle> triangleStream)
        {
            List<Vertex> vertices = mesh.ProcessedVertices;
            List<int> vrt = new List<int>(3);
            vrt.AddRange(new int[] { vb, vc, vd });

            int v1 = Mesh.GetLeftmost(vertices, triangle.TransformedNormal, va, vb, vc, vd);
            vrt.Remove(v1);
            int v2 = Mesh.GetLeftmost(vertices, triangle.TransformedNormal, v1, vrt[0], vrt[1]);
            vrt.Remove(v2);


            Triangle tr1 = new Triangle(va, v1, v2, mesh, triangle.ModelNormal, triangle.TransformedNormal);
            tr1.ColorAttrib =  DebugMode ? 2 : triangle.ColorAttrib;
            tr1.CalculateEdges();
            triangleStream.Add(tr1);

            Triangle tr2 = new Triangle(va, v2, vrt[0], mesh, triangle.ModelNormal, triangle.TransformedNormal);
            tr2.ColorAttrib = DebugMode ? 1 : triangle.ColorAttrib;
            tr2.CalculateEdges();
            triangleStream.Add(tr2);
        }

        static public void VertsToTris(Mesh mesh, Triangle triangle, int va, int vb, int vc, List<Triangle> triangleStream)
        {
            int v1 = Mesh.GetLeftmost(mesh.ProcessedVertices, triangle.TransformedNormal, va, vb, vc);
            int v2 = 0;
            if (vb == v1) v2 = vc;
            else v2 = vb;

            Triangle tr = new Triangle(va, v1, v2, mesh, triangle.ModelNormal, triangle.TransformedNormal);
            tr.ColorAttrib = DebugMode ? 9 : triangle.ColorAttrib;
            tr.CalculateEdges();
            triangleStream.Add(tr);
        }


        static public List<Triangle> ClipTriangleAgainstPlane(Triangle inTriangle, Mesh mesh, ClipPlane clPlane)
        {
            NEPlane plane = clPlane.Plane;
            List<Vertex> vertices = mesh.ProcessedVertices;
            List<Triangle> newTriangles = new List<Triangle>(4);
            int outCount = CheckBoundry(inTriangle, mesh, clPlane);
            if (outCount == 0)
            {
                newTriangles.Add(inTriangle);
                return newTriangles;
            }
            if (outCount == 3)
            {
                return newTriangles;
            }

            if (outCount == 1)
            {
                int vOutI = OUTS[0]; int vIn0I = INS[0]; int vIn1I = INS[1];

                PlaneIntersectionManifest m1 = NEPlane.IntersectionWithLineSegment(vertices[vOutI].Position, vertices[vIn0I].Position, plane);
                Vertex new0 = Vertex.Lerp(vertices[vOutI], vertices[vIn0I], m1.MagnitudeNormalized);
                m1 = NEPlane.IntersectionWithLineSegment(vertices[vOutI].Position, vertices[vIn1I].Position, plane);
                Vertex new1 = Vertex.Lerp(vertices[vOutI], vertices[vIn1I], m1.MagnitudeNormalized);

                vertices.Add(new0);
                vertices.Add(new1);
                int v0 = vertices.Count - 2;
                int v1 = vertices.Count - 1;
                VertsToTris(mesh, inTriangle, v0, v1, vIn0I, vIn1I, newTriangles);

                return newTriangles;

            }

            if (outCount == 2)
            {
                int vOut0I = OUTS[0]; int vOut1I = OUTS[1]; int vInI = INS[0];

                PlaneIntersectionManifest m1 = NEPlane.IntersectionWithLineSegment(vertices[vOut0I].Position, vertices[vInI].Position, plane);
                Vertex new0 = Vertex.Lerp(vertices[vOut0I], vertices[vInI], m1.MagnitudeNormalized);
                m1 = NEPlane.IntersectionWithLineSegment(vertices[vOut1I].Position, vertices[vInI].Position, plane);
                Vertex new1 = Vertex.Lerp(vertices[vOut1I], vertices[vInI], m1.MagnitudeNormalized);

                vertices.Add(new0);
                vertices.Add(new1);
                int v0 = vertices.Count - 2;
                int v1 = vertices.Count - 1;
                VertsToTris(mesh, inTriangle, v0, v1, vInI, newTriangles);

                return newTriangles;
            }

            return newTriangles;
        }

        static public List<Triangle> ClipTrianglesAgainstPlane(List<Triangle> inTriangles, Mesh mesh, ClipPlane clPlane)
        {
            List<Triangle> output = new List<Triangle>(inTriangles.Count * 2); // in worst case scenario, each input triangle 
                                                                               // will produce two new triangles, hence count*2

            for(int i =0; i < inTriangles.Count;++i)
            {
                output.AddRange(ClipTriangleAgainstPlane(inTriangles[i], mesh, clPlane));
            }

            return output;
        }



        static int[] INS = new int[3];
        static int[] OUTS = new int[3];
        static private int CheckBoundry(Triangle triangle, Mesh mesh, ClipPlane plane)
        {
            bool checkGreater = plane.RejectCriteria == RejectCriteria.GreaterThan;
            int inI = 0; int outI = 0;
            Vertex A = mesh.ProcessedVertices[triangle.Indices[0]];
            Vertex B = mesh.ProcessedVertices[triangle.Indices[1]];
            Vertex C = mesh.ProcessedVertices[triangle.Indices[2]];

            if ((A.Position.Data[(int)plane.Axis] < plane.Treshold) ^ checkGreater)
            {
                OUTS[outI] = triangle.Indices[0];
                outI++;
            }
            else
            {
                INS[inI] = triangle.Indices[0];
                inI++;
            }

            if ((B.Position.Data[(int)plane.Axis] < plane.Treshold) ^ checkGreater)
            {
                OUTS[outI] = triangle.Indices[1];
                outI++;
            }
            else
            {
                INS[inI] = triangle.Indices[1];
                inI++;
            }

            if ((C.Position.Data[(int)plane.Axis] < plane.Treshold) ^ checkGreater)
            {
                OUTS[outI] = triangle.Indices[2];
                outI++;
            }
            else
            {
                INS[inI] = triangle.Indices[2];
                inI++;
            }
            return outI;
        }
    }
}
