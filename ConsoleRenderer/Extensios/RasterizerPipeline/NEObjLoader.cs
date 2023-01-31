using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace NostalgiaEngine.RasterizerPipeline
{
    public class NEObjLoader
    {

        static public VertexBuffer LoadObj(string path)
        {
            VertexBuffer vbo = new VertexBuffer();
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    while(!reader.EndOfStream)
                    {
                        string line = reader.ReadLine().Trim();
                        if (line.Length == 0) continue;
                        if(line[0] == 'v')
                        {
                            string[] pieces = line.Split(' ');
                            vbo.AddVertex(new Vertex(float.Parse(pieces[1]), float.Parse(pieces[2]), float.Parse(pieces[3])));
                            
                        }
                        else if(line[0] == 'f')
                        {
                            string[] pieces = line.Split(' ');
                            vbo.AddTriangle(int.Parse(pieces[1])-1, int.Parse(pieces[2])-1, int.Parse(pieces[3])-1);
                        }
                    }
                }
            }
            catch
            {
                return null;
            }

            return vbo;
        }
    }
}
