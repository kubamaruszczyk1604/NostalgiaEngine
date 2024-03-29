﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace NostalgiaEngine.RasterizerPipeline
{
    public class NEObjLoader
    {
        static int index = 1;
        static public Mesh LoadObj(string path, int colAttrib = 1)
        {
            Mesh vbo = new Mesh();
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
                            vbo.Triangles[vbo.Triangles.Count - 1].ColorAttrib = colAttrib;
                            //index++;
                            //index %= 15;
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
