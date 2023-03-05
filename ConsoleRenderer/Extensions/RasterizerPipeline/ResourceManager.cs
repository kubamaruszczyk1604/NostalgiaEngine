using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.RasterizerPipeline
{
    
    class ResourceManager
    {
        private static readonly ResourceManager s_Instance = new ResourceManager();
        public static ResourceManager Instance { get { return s_Instance; } }


        private Dictionary<string, NEColorTexture16> m_ColorTextures;
        private Dictionary<string, NEFloatBuffer> m_LumaTextures;
        private Dictionary<string, NEColorPalette> m_Palettes;
        private Dictionary<string, Mesh> m_Geometry;

        static ResourceManager()
        {
            
        }

        private ResourceManager()
        {
            m_Geometry = new Dictionary<string, Mesh>();
            m_Palettes = new Dictionary<string, NEColorPalette>();
            m_LumaTextures = new Dictionary<string, NEFloatBuffer>();
            m_ColorTextures = new Dictionary<string, NEColorTexture16>();
        }

        public NEColorTexture16 GetColorTexture(string path)
        {
            NEColorTexture16 tex = null;
            if (m_ColorTextures.TryGetValue(path.Trim(), out tex))
            {
                return tex;
            }
            tex = NEColorTexture16.LoadFromFile(path);
            if(tex != null)
            {
                m_ColorTextures.Add(path, tex);
            }

            return tex;
        }

        public NEFloatBuffer GetLumaTexture(string path)
        {
            NEFloatBuffer tex = null;
            if (m_LumaTextures.TryGetValue(path.Trim(), out tex))
            {
                return tex;
            }

            tex = NEFloatBuffer.FromFile(path);
            if(tex != null)
            {
                m_LumaTextures.Add(path, tex);
            }
            return tex;
        }

        public NEColorPalette GetPalette(string path)
        {
            NEColorPalette pal = null;
            if(m_Palettes.TryGetValue(path.Trim(), out pal))
            {
                return pal;
            }
            pal = NEColorPalette.FromFile(path);
            if(pal != null)
            {
                m_Palettes.Add(path, pal);
            }
            return pal;
        }

        public Mesh GetMesh(string path)
        {
            Mesh mesh = null;
            if (m_Geometry.TryGetValue(path.Trim(), out mesh))
            {
                return mesh;
            }
            mesh = NEObjLoader.LoadObj(path);
            if(mesh != null)
            {
                m_Geometry.Add(path, mesh);
            }
            return null;
        }

        public bool AddExplicitly(string id, Mesh mesh)
        {
            if(!m_Geometry.ContainsKey(id))
            {
                m_Geometry.Add(id, mesh);
                return true;
            }
            return false;
        }

        public bool AddExplicitly(string id, NEColorTexture16 tex)
        {
            if (!m_ColorTextures.ContainsKey(id))
            {
                m_ColorTextures.Add(id, tex);
                return true;
            }
            return false;
        }

        public bool AddExplicitly(string id, NEFloatBuffer tex)
        {
            if (!m_LumaTextures.ContainsKey(id))
            {
                m_LumaTextures.Add(id, tex);
                return true;
            }
            return false;
        }

        public bool AddExplicitly(string id, NEColorPalette pal)
        {
            if (!m_Palettes.ContainsKey(id))
            {
                m_Palettes.Add(id, pal);
                return true;
            }
            return false;
        }

    }
}
