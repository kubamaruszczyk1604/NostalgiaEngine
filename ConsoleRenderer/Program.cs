using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Runtime.InteropServices;
using ConsoleRenderer.TextureEditor;
namespace ConsoleRenderer
{

    class Program
    {


        static void Main(string[] args)
        {


           // RayMarcher rm = new RayMarcher(300, 200, 4, 4);
           // Buffer.HalfTemporalResolution = true;

           // rm.Play();
           // CGRaytracer2D tracer = new CGRaytracer2D();
            CGEngine engine = new CGEngine();
            CGTextureEditor ed = new CGTextureEditor();
            //Test_MemTex16 test = new Test_MemTex16();
            engine.Start(ed);


        }
    }
}
