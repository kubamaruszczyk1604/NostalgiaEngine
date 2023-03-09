using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.RasterizerPipeline
{
    public class VertexBuffer
    {

        public Model AssociatedModel { get; private set; }

        public List<Vertex> ProcessedVertices;
        public List<Triangle> ProcessedTriangles;

        public List<Triangle> TempTriangleContainer;


        public VertexBuffer(Model model)
        {
            AssociatedModel = model;
            ProcessedVertices = new List<Vertex>(AssociatedModel.Mesh.ModelVertices.Count + 50);
            ProcessedTriangles = new List<Triangle>(AssociatedModel.Mesh.ModelVertices.Count + 50);
            TempTriangleContainer = new List<Triangle>(AssociatedModel.Mesh.ModelVertices.Count + 50);
        }


        public void PrepareForRender(Camera camera)
        {
            Mesh mesh = AssociatedModel.Mesh;
            ClearProcessedData();
            Model model = AssociatedModel;

            NEMatrix4x4 MVP = camera.Projection * camera.View * model.Transform.World;
            for (int i = 0; i < mesh.ModelVertices.Count; ++i)
            {
               ProcessedVertices.Add(mesh.ModelVertices[i].Duplicate());
               ProcessedVertices[i].Position = MVP * ProcessedVertices[i].Position;
               ProcessedVertices[i].Vert2Camera = -ProcessedVertices[i].Position.Normalized;

            }

            NEMatrix4x4 normalTransformMat = camera.RotationInv * model.Transform.RotationMat;
            //Projection space
            for (int i = 0; i < mesh.ModelTriangles.Count; ++i)
            {
                Triangle tri = mesh.ModelTriangles[i];
                tri.TransformedNormal = normalTransformMat * mesh.ModelTriangles[i].ModelNormal;
                tri = new Triangle(tri, model.VBO);
                if (CullTest(tri, model.FaceCull)) continue;
                List<Triangle> nearClipped = Clipping.ClipTriangleAgainstPlane(tri, this, ClipPlane.Near);
                foreach (Triangle triangle in nearClipped)
                {
                   TempTriangleContainer.Add(triangle);
                }

            }
            for (int i = 0; i < TempTriangleContainer.Count; ++i)
            {
                Triangle triangle = TempTriangleContainer[i];
                triangle.ZDivide();
                triangle.CalculateEdges();

            }

            for (int i = 0; i < TempTriangleContainer.Count; ++i)
            {
                Triangle triangle = TempTriangleContainer[i];
                if (IsOutsideFrustum(triangle)) continue;
                List<Triangle> LeftClipped = Clipping.ClipTriangleAgainstPlane(triangle, this, ClipPlane.Left);
                List<Triangle> RightClipped = Clipping.ClipTrianglesAgainstPlane(LeftClipped, this, ClipPlane.Right);
                List<Triangle> BottomClipped = Clipping.ClipTrianglesAgainstPlane(RightClipped, this, ClipPlane.Bottom);
                List<Triangle> TopClipped = Clipping.ClipTrianglesAgainstPlane(BottomClipped, this, ClipPlane.Top);
                List<Triangle> FarClipped = Clipping.ClipTrianglesAgainstPlane(TopClipped, this, ClipPlane.Far);

                ProcessedTriangles.AddRange(FarClipped);
            }

        }


        private bool IsOutsideFrustum(Triangle triangle)
        {
            float depthA = triangle.A.Z;
            float depthB = triangle.B.Z;
            float depthC = triangle.C.Z;

            if (depthA < 0.0f && depthB < 0.0f && depthC < 0.0f) return true;
            if (depthA > 1.0f && depthB > 1.0f && depthC > 1.0f) return true;

            float xA = triangle.A.X;
            float xB = triangle.B.X;
            float xC = triangle.C.X;

            float yA = triangle.A.Y;
            float yB = triangle.B.Y;
            float yC = triangle.C.Y;

            if (xA < -1.0f && xB < -1.0f && xC < -1.0f) return true;
            if (xA > 1.0f && xB > 1.0f && xC > 1.0f) return true;

            if (yA < -1.0f && yB < -1.0f && yC < -1.0f) return true;
            if (yA > 1.0f && yB > 1.0f && yC > 1.0f) return true;


            return false;
        }

        private bool CullTest(Triangle triangle, CullMode cullMode)
        {
            if (cullMode == CullMode.None) return false;
            //NEVector4 vA = -triangle.A.Position.Normalized;
            //NEVector4 vB = -triangle.B.Position.Normalized;
            //NEVector4 vC = -triangle.C.Position.Normalized;

            NEVector4 vA = -triangle.VBO.ProcessedVertices[triangle.Indices[0]].Position;
            NEVector4 vB = -triangle.VBO.ProcessedVertices[triangle.Indices[1]].Position;
            NEVector4 vC = -triangle.VBO.ProcessedVertices[triangle.Indices[2]].Position;

            float dotA = NEVector4.Dot3(vA, triangle.TransformedNormal);
            float dotB = NEVector4.Dot3(vB, triangle.TransformedNormal);
            float dotC = NEVector4.Dot3(vC, triangle.TransformedNormal);

            return (dotA < 0.0f && dotB < 0.0f && dotC < 0.0f) ^ (cullMode == CullMode.Front);
        }


        public void ClearProcessedData()
        {
            ProcessedTriangles.Clear();
            ProcessedVertices.Clear();
            TempTriangleContainer.Clear();
        }
    }
}
