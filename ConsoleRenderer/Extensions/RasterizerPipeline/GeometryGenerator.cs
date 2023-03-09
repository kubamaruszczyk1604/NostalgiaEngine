using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.RasterizerPipeline
{
    public class GeometryGenerator
    {
        public static Mesh CreateQuad(float width, float height, NEVector4 orgin, int col = 15)
        {
            Mesh mesh = new Mesh();

            mesh.AddVertex(new Vertex(-width + orgin.X, -height + orgin.Y, orgin.Z, 0.0f, 0.0f));
            mesh.AddVertex(new Vertex(-width + orgin.X, height + orgin.Y, orgin.Z, 0.0f, 1.0f));
            mesh.AddVertex(new Vertex(width + orgin.X, height + orgin.Y, orgin.Z, 1.0f, 1.0f));
            mesh.AddVertex(new Vertex(width + orgin.X, -height  + orgin.Y, orgin.Z, 1.0f, 0.0f));

            mesh.AddTriangle(0, 1, 2);
            mesh.AddTriangle(0, 2, 3);

            mesh.Triangles[0].ColorAttrib = col;
            mesh.Triangles[1].ColorAttrib = col;
            return mesh;
        }

        public static Mesh CreateHorizontalQuad(float width, float length, NEVector4 orgin, int col = 15)
        {
            Mesh mesh = new Mesh();

            mesh.AddVertex(new Vertex(-width + orgin.X, orgin.Y, -length + orgin.Z,  0.0f, 0.0f));
            mesh.AddVertex(new Vertex(-width + orgin.X, orgin.Y, length + orgin.Z, 0.0f, 1.0f));
            mesh.AddVertex(new Vertex(width + orgin.X, orgin.Y, length + orgin.Z,  1.0f, 1.0f));
            mesh.AddVertex(new Vertex(width + orgin.X, orgin.Y, -length + orgin.Z, 1.0f, 0.0f));

            mesh.AddTriangle(0, 1, 2);
            mesh.AddTriangle(0, 2, 3);

            mesh.Triangles[0].ColorAttrib = col;
            mesh.Triangles[1].ColorAttrib = col;
            return mesh;
        }

        public static  Mesh GenerateCube(float width, float height, float length, NEVector4 orgin, int col)
        {
            Mesh mesh = new Mesh();
            float x = orgin.X;
            float y = orgin.Y;
            float z = orgin.Z;
            mesh.AddVertex(new Vertex(-width + x, -height + y, z - length, 0.0f, 0.0f));
            mesh.AddVertex(new Vertex(-width + x, height + y, z - length, 0.0f, 1.0f));
            mesh.AddVertex(new Vertex(width + x, height + y, z - length, 1.0f, 1.0f));
            mesh.AddVertex(new Vertex(width + x, -height + y, z - length, 1.0f, 0.0f));



            mesh.AddVertex(new Vertex(-width + x, -height + y, z + length, 1.0f, 0.0f));
            mesh.AddVertex(new Vertex(-width + x, height + y, z + length, 1.0f, 1.0f));
            mesh.AddVertex(new Vertex(width + x, height + y, z + length, 0.0f, 1.0f));
            mesh.AddVertex(new Vertex(width + x, -height + y, z + length, 0.0f, 0.0f));


            mesh.AddTriangle(0, 1, 2);
            mesh.AddTriangle(0, 2, 3);

            mesh.Triangles[0].ColorAttrib = col;
            mesh.Triangles[1].ColorAttrib = col;


            mesh.AddTriangle(4, 6, 5);
            mesh.AddTriangle(4, 7, 6);

            mesh.Triangles[2].ColorAttrib = col;
            mesh.Triangles[3].ColorAttrib = col;


            mesh.AddTriangle(4, 5, 1);
            mesh.AddTriangle(4, 1, 0);

            mesh.Triangles[4].ColorAttrib = col;
            mesh.Triangles[5].ColorAttrib = col;


            mesh.AddTriangle(3, 2, 6);
            mesh.AddTriangle(3, 6, 7);

            mesh.Triangles[6].ColorAttrib = col;
            mesh.Triangles[7].ColorAttrib = col;



            mesh.AddTriangle(1, 5, 6);
            mesh.AddTriangle(1, 6, 2);

            mesh.Triangles[8].ColorAttrib = col;
            mesh.Triangles[9].ColorAttrib = col;


            mesh.AddTriangle(0, 3, 7);
            mesh.AddTriangle(0, 7, 4);

            mesh.Triangles[10].ColorAttrib = col;
            mesh.Triangles[11].ColorAttrib = col;
            return mesh;
        }


        public static Mesh GenerateCube2(float width, float height, float length, NEVector4 orgin, int col)
        {
            Mesh mesh = new Mesh();
            float x = orgin.X;
            float y = orgin.Y;
            float z = orgin.Z;
            //front
            mesh.AddVertex(new Vertex(-width + x, -height + y, z - length, 0.0f, 0.0f));
            mesh.AddVertex(new Vertex(-width + x, height + y, z - length, 0.0f, 1.0f));
            mesh.AddVertex(new Vertex(width + x, height + y, z - length, 1.0f, 1.0f));
            mesh.AddVertex(new Vertex(width + x, -height + y, z - length, 1.0f, 0.0f));


            //back
            mesh.AddVertex(new Vertex(-width + x, -height + y, z + length, 1.0f, 0.0f));
            mesh.AddVertex(new Vertex(-width + x, height + y, z + length, 1.0f, 1.0f));
            mesh.AddVertex(new Vertex(width + x, height + y, z + length, 0.0f, 1.0f));
            mesh.AddVertex(new Vertex(width + x, -height + y, z + length, 0.0f, 0.0f));

            //left
            mesh.AddVertex(new Vertex(-width + x, -height + y, z + length, 0.0f, 0.0f));
            mesh.AddVertex(new Vertex(-width + x, height + y, z + length, 0.0f, 1.0f));
            mesh.AddVertex(new Vertex(-width + x, height + y, z - length, 1.0f, 1.0f));
            mesh.AddVertex(new Vertex(-width + x, -height + y, z - length, 1.0f, 0.0f));

            //right
            mesh.AddVertex(new Vertex(width + x, -height + y, z - length, 0.0f, 0.0f));
            mesh.AddVertex(new Vertex(width + x, height + y, z - length, 0.0f, 1.0f));
            mesh.AddVertex(new Vertex(width + x, height + y, z + length, 1.0f, 1.0f));
            mesh.AddVertex(new Vertex(width + x, -height + y, z + length, 1.0f, 0.0f));

            //top
            mesh.AddVertex(new Vertex(-width + x, height + y, z - length, 0.0f, 0.0f));
            mesh.AddVertex(new Vertex(-width + x, height + y, z + length, 0.0f, 1.0f));
            mesh.AddVertex(new Vertex(width + x, height + y, z + length, 1.0f, 1.0f));
            mesh.AddVertex(new Vertex(width + x, height + y, z - length, 1.0f, 0.0f));


            //bottom
            mesh.AddVertex(new Vertex(-width + x, -height + y, z + length, 0.0f, 0.0f));
            mesh.AddVertex(new Vertex(-width + x, -height + y, z - length, 0.0f, 1.0f));
            mesh.AddVertex(new Vertex(width + x, -height + y, z -length, 1.0f, 1.0f));
            mesh.AddVertex(new Vertex(width + x, -height + y, z + length, 1.0f, 0.0f));


            mesh.AddTriangle(0, 1, 2);
            mesh.AddTriangle(0, 2, 3);
            mesh.Triangles[0].ColorAttrib = col;
            mesh.Triangles[1].ColorAttrib = col;


            mesh.AddTriangle(4, 6, 5);
            mesh.AddTriangle(4, 7, 6);
            mesh.Triangles[2].ColorAttrib = col;
            mesh.Triangles[3].ColorAttrib = col;


            mesh.AddTriangle(8, 9, 10);
            mesh.AddTriangle(8, 10, 11);
            mesh.Triangles[4].ColorAttrib = col;
            mesh.Triangles[5].ColorAttrib = col;


            mesh.AddTriangle(12, 13, 14);
            mesh.AddTriangle(12, 14, 15);
            mesh.Triangles[6].ColorAttrib = col;
            mesh.Triangles[7].ColorAttrib = col;



            mesh.AddTriangle(16, 17, 18);
            mesh.AddTriangle(16, 18, 19);
            mesh.Triangles[8].ColorAttrib = col;
            mesh.Triangles[9].ColorAttrib = col;


            mesh.AddTriangle(20, 21, 22);
            mesh.AddTriangle(20, 22, 23);

            mesh.Triangles[10].ColorAttrib = col;
            mesh.Triangles[11].ColorAttrib = col;
            return mesh;
        }
    }
}
