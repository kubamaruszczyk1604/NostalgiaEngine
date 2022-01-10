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

            //CGRaytracer2D tracer = new CGRaytracer2D();
            CGEngine engine = new CGEngine();
            CGTextureEditor ed = new CGTextureEditor();
            engine.Start(ed);


            //StepSeries<int> sr = new StepSeries<int>(10, 1);
            //sr.AddStep(2);
            //sr.UndoStep();
            //sr.AddStep(3);
            //sr.AddStep(4);
            //sr.AddStep(5);
            //sr.AddStep(6);
            //Console.WriteLine(sr.Data);
            //for (int i = 0; i < 16; ++i)
            //{
            //    sr.UndoStep();
            //    Console.WriteLine(sr.Data);
            //}
            ////Console.WriteLine(sr.Data);
            //Console.ReadLine();
        }
    }
}
