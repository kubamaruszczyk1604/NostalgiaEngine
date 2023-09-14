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
        public List<Triangle> TrianglesReadyToRender;

        public List<Triangle> TempTriangleList;

        private VertexPool m_VertexPool;
        private TrianglePool m_TrianglePool;
       
        public VertexBuffer(Model model)
        {
            AssociatedModel = model;
            ProcessedVertices = new List<Vertex>(AssociatedModel.Mesh.Vertices.Count * 2);

            TrianglesReadyToRender = new List<Triangle>(AssociatedModel.Mesh.Triangles.Count * 2);
            TempTriangleList = new List<Triangle>(AssociatedModel.Mesh.Triangles.Count * 2);

            m_VertexPool = new VertexPool();
            m_VertexPool.Allocate(AssociatedModel.Mesh.Vertices.Count * 2);

            m_TrianglePool = new TrianglePool();
            m_TrianglePool.Allocate(AssociatedModel.Mesh.Triangles.Count * 2);

        }

        public void PrepareForRender(Camera camera)
        {
            Mesh mesh = AssociatedModel.Mesh;
            ClearProcessedData();
            Model model = AssociatedModel;

            NEMatrix4x4 MVP = camera.Projection * camera.View * model.Transform.World;
            for (int i = 0; i < mesh.Vertices.Count; ++i)
            {
               // ProcessedVertices.Add(mesh.Vertices[i].Duplicate());
                ProcessedVertices.Add(m_VertexPool.RequestAndSet(mesh.Vertices[i]));
                ProcessedVertices[i].Position = MVP * ProcessedVertices[i].Position;
               // ProcessedVertices[i].Vert2Camera = -ProcessedVertices[i].Position.Normalized;

            }
            int currentTriangle = 0;
            NEMatrix4x4 normalTransformMat = camera.RotationInv * model.Transform.RotationMat;
            //Projection space
            for (int i = 0; i < mesh.Triangles.Count; ++i)
            {
                Triangle tri = mesh.Triangles[i];
                tri.NormalView = normalTransformMat * tri.NormalModel;
                tri.NormalWorld = model.Transform.RotationMat * tri.NormalModel;
               // tri = new Triangle(tri, model.VBO);
                tri = RequestFromPool(tri);
                if (CullTest(tri, model.FaceCull)) continue;
                List<Triangle> nearClipped = Clipping.ClipTriangleAgainstPlane(tri, this, ClipPlane.Near);
                foreach (Triangle triangle in nearClipped)
                {
                   
                   TempTriangleList.Add(triangle);
                    currentTriangle++;
                }

            }
            for (int i = 0; i < TempTriangleList.Count; ++i)
            {
                Triangle triangle = TempTriangleList[i];
                triangle.ZDivide();
                triangle.CalculateEdges();

            }

            for (int i = 0; i < TempTriangleList.Count; ++i)
            {
                Triangle triangle = TempTriangleList[i];
                if (IsOutsideFrustum(triangle)) continue;
                List<Triangle> LeftClipped = Clipping.ClipTriangleAgainstPlane(triangle, this, ClipPlane.Left);
                List<Triangle> RightClipped = Clipping.ClipTrianglesAgainstPlane(LeftClipped, this, ClipPlane.Right);
                List<Triangle> BottomClipped = Clipping.ClipTrianglesAgainstPlane(RightClipped, this, ClipPlane.Bottom);
                List<Triangle> TopClipped = Clipping.ClipTrianglesAgainstPlane(BottomClipped, this, ClipPlane.Top);
                List<Triangle> FarClipped = Clipping.ClipTrianglesAgainstPlane(TopClipped, this, ClipPlane.Far);


                TrianglesReadyToRender.AddRange(FarClipped);
            }

            for (int i = 0; i < ProcessedVertices.Count; ++i)
            {
                ProcessedVertices[i].Vert2Camera = -ProcessedVertices[i].Position.Normalized;
            }

        }

        public Vertex RequestFromPool(Vertex setValue)
        {
            return m_VertexPool.RequestAndSet(setValue);
        }

        public Vertex RequestFromPool(ref NEVector4 setPosition, ref NEVector2 setUVs)
        {
            return m_VertexPool.Get(ref setPosition, ref setUVs);
        }

        public Triangle RequestFromPool(Triangle setValue)
        {
            return m_TrianglePool.Get(setValue, this);
        }

        public Triangle RequestFromPool(int i0, int i1, int i2,  NEVector4 normal,  NEVector4 normalView, NEVector4 normalWorld)
        {
            return m_TrianglePool.Get(i0, i1, i2, this, ref normal, ref normalView, ref normalWorld);
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

            NEVector4 vA = -triangle.VBO.ProcessedVertices[triangle.Indices[0]].Position;
            NEVector4 vB = -triangle.VBO.ProcessedVertices[triangle.Indices[1]].Position;
            NEVector4 vC = -triangle.VBO.ProcessedVertices[triangle.Indices[2]].Position;

            float dotA = NEVector4.Dot3(vA, triangle.NormalView);
            float dotB = NEVector4.Dot3(vB, triangle.NormalView);
            float dotC = NEVector4.Dot3(vC, triangle.NormalView);

            return (dotA < 0.0f && dotB < 0.0f && dotC < 0.0f) ^ (cullMode == CullMode.Front);
        }


        public void ClearProcessedData()
        {
            TrianglesReadyToRender.Clear();
            ProcessedVertices.Clear();
            TempTriangleList.Clear();
            m_VertexPool.ReturnAllToPool();
            m_TrianglePool.ReturnAllToPool();
        }
    }
}
