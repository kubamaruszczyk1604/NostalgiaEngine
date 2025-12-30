using NostalgiaEngine.Core;

namespace NostalgiaEngine.RasterizerPipeline
{
	public class Triangle
	{
		public Mesh ParentMesh { get; private set; }
		public VertexBuffer VBO { get; private set; }
		//public int[] Indices { get; private set; }
		//public int[] LeftSortedIndices { get; private set; }
		public int I0, I1, I2;
		public int LS0, LS1, LS2;

		public NEEdge AB;
		public NEEdge AC;
		public NEEdge BC;

		public Vertex A { get; private set; }
		public Vertex B { get; private set; }
		public Vertex C { get; private set; }

		public int ColorAttrib = 1;
		public NEVector4 NormalModel { get; private set; }
		public NEVector4 NormalView { get; set; }
		public NEVector4 NormalWorld { get; set; }

		public Triangle(int i0, int i1, int i2, Mesh mesh)
		{
			ParentMesh = mesh;
			//Indices = new int[] { i0, i1, i2 };
			//LeftSortedIndices = new int[3];
			I0 = i0;
			I1 = i1;
			I2 = i2;
			CalculateNormal();
		}

		public Triangle()
		{
			//ParentMesh = mesh;
		}

		public Triangle(int i0, int i1, int i2, VertexBuffer vbo, NEVector4 normal, NEVector4 transformedNormal, NEVector4 normalWorld)
		{
			VBO = vbo;
			I0 = i0;
			I1 = i1;
			I2 = i2;
			NormalModel = normal;
			NormalView = transformedNormal;
			NormalWorld = normalWorld;
		}

		public Triangle(Triangle triangle, VertexBuffer vbo)
		{
			VBO = vbo;
			I0 = triangle.I0;
			I1 = triangle.I1;
			I2 = triangle.I2;

			NormalModel = triangle.NormalModel;
			NormalView = triangle.NormalView;
			NormalWorld = triangle.NormalWorld;
			ColorAttrib = triangle.ColorAttrib;
		}

		public void Set(Triangle triangle, VertexBuffer vbo)
		{
			VBO = vbo;
			ParentMesh = triangle.ParentMesh;
			I0 = triangle.I0;
			I1 = triangle.I1;
			I2 = triangle.I2;
			LS0 = triangle.LS0;
			LS1 = triangle.LS1;
			LS2 = triangle.LS2;
			NormalModel = triangle.NormalModel;
			NormalView = triangle.NormalView;
			NormalWorld = triangle.NormalWorld;
			ColorAttrib = triangle.ColorAttrib;
		}

		public void Set(Triangle triangle)
		{
			VBO = triangle.VBO;
			ParentMesh = triangle.ParentMesh;
			I0 = triangle.I0;
			I1 = triangle.I1;
			I2 = triangle.I2;
			LS0 = triangle.LS0;
			LS1 = triangle.LS1;
			LS2 = triangle.LS2;
			NormalModel = triangle.NormalModel;
			NormalView = triangle.NormalView;
			NormalWorld = triangle.NormalWorld;
			ColorAttrib = triangle.ColorAttrib;
		}

		public void Set(int i0, int i1, int i2, VertexBuffer vbo, NEVector4 normal, NEVector4 normalView, NEVector4 normalWorld)
		{
			VBO = vbo;
			I0 = i0;
			I1 = i1;
			I2 = i2;
			LS0 = 0;
			LS1 = 0;
			LS2 = 0;
			NormalModel = normal;
			NormalView = normalView;
			NormalWorld = normalWorld;
		}

		public void ZDivide()
		{
			VBO.ProcessedVertices[I0].ZDivide();
			VBO.ProcessedVertices[I1].ZDivide();
			VBO.ProcessedVertices[I2].ZDivide();
		}

		//public void DoLeftSort()
		//{
		//    SortX(out LeftSortedIndices[0], out LeftSortedIndices[1], out LeftSortedIndices[2]);
		//    A = VBO.ProcessedVertices[LeftSortedIndices[0]];
		//    B = VBO.ProcessedVertices[LeftSortedIndices[1]];
		//    C = VBO.ProcessedVertices[LeftSortedIndices[2]];
		//}

		public void CalculateEdges()
		{
			SortX(out LS0, out LS1, out LS2);
			A = VBO.ProcessedVertices[LS0];
			B = VBO.ProcessedVertices[LS1];
			C = VBO.ProcessedVertices[LS2];

			//AB = new NEEdge();
			NEMath.Find2DLineEquation(A.Position.XY, B.Position.XY, out AB.a, out AB.c);

			//AC = new NEEdge();
			NEMath.Find2DLineEquation(A.Position.XY, C.Position.XY, out AC.a, out AC.c);

			//BC = new NEEdge();
			NEMath.Find2DLineEquation(B.Position.XY, C.Position.XY, out BC.a, out BC.c);
		}

		public bool IsColScanlineInTriangle(float x)
		{
			return ((x >= A.X) && (x <= C.X));
		}

		public void FindIntersectionHeights(float x, out float y0, out float y1)
		{
			y0 = 0;
			y1 = 0;

			if (x <= B.X)
			{
				y0 = AB.a * x + AB.c;
				y1 = AC.a * x + AC.c;
			}
			else
			{
				y0 = BC.a * x + BC.c;
				y1 = AC.a * x + AC.c;
			}
		}

		public void ComputeScanlineIntersection(float x, out ScanlineIntersectionManifest manifest)
		{
			//manifest = new ScanlineIntersectionManifest();
			float yAC = AC.a * x + AC.c;

			manifest.Y1 = yAC;

			float denCA = (C.X - A.X);
			denCA = NEMath.Abs(denCA) >= 0.01f ? denCA : 0.01f;
			float t_AC = (x - A.X) / denCA;

			float t_Other = 0.0f;

			Vertex otherP0 = A;
			Vertex otherP1 = B;

			if (x <= B.X)
			{
				//AB is other 
				manifest.Y0 = AB.a * x + AB.c;
				float denBA = (B.X - A.X);
				denBA = NEMath.Abs(denBA) >= 0.01f ? denBA : 0.01f;
				t_Other = (x - A.X) / denBA;
			}
			else
			{
				//BC is other
				manifest.Y0 = BC.a * x + BC.c;
				float denCB = (C.X - B.X);
				denCB = NEMath.Abs(denCB) >= 0.01f ? denCB : 0.01f;
				t_Other = (x - B.X) / denCB;

				otherP0 = B;
				otherP1 = C;
			}

			if (yAC > manifest.Y0) //ac is upper
			{

				manifest.top_t = t_AC;
				manifest.bottom_t = t_Other;

				manifest.top_P0 = A;
				manifest.top_P1 = C;

				manifest.bottom_P0 = otherP0;
				manifest.bottom_P1 = otherP1;

			}
			else
			{
				manifest.top_t = t_Other;
				manifest.bottom_t = t_AC;

				manifest.top_P0 = otherP0;
				manifest.top_P1 = otherP1;

				manifest.bottom_P0 = A;
				manifest.bottom_P1 = C;
			}
		}

		private void SortX(out int left, out int middle, out int right)
		{
			left = I0;
			middle = I1;
			right = I2;

			if (VBO.ProcessedVertices[left].X > VBO.ProcessedVertices[middle].X)
			{
				SwapInt(ref left, ref middle);
			}

			if (VBO.ProcessedVertices[middle].X > VBO.ProcessedVertices[right].X)
			{
				SwapInt(ref middle, ref right);
			}

			if (VBO.ProcessedVertices[left].X > VBO.ProcessedVertices[middle].X)
			{
				SwapInt(ref left, ref middle);
			}
		}

		private void SwapInt(ref int a, ref int b)
		{
			int tmp = a;
			a = b;
			b = tmp;
		}

		private void CalculateNormal()
		{
			NEVector4 a = (ParentMesh.Vertices[I1].Position - ParentMesh.Vertices[I0].Position).Normalized;
			NEVector4 b = (ParentMesh.Vertices[I2].Position - ParentMesh.Vertices[I0].Position).Normalized;

			float x = a.Y * b.Z - a.Z * b.Y;
			float y = a.Z * b.X - a.X * b.Z;
			float z = a.X * b.Y - a.Y * b.X;
			NormalModel = new NEVector4(x, y, z, 0.0f).Normalized;
		}
	}

	public struct NEEdge
	{
		public float a; //gradient
		public float c; //intercept
	}

	public struct ScanlineIntersectionManifest
	{
		public float Y0;
		public float Y1;

		public float bottom_t;
		public float top_t;

		public Vertex bottom_P0;
		public Vertex bottom_P1;

		public Vertex top_P0;
		public Vertex top_P1;
	}
}
