using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using NostalgiaEngine.TextureEditor;
using NostalgiaEngine.Core;
using NostalgiaEngine.Raycaster;
using NostalgiaEngine.GUI;
using System.Diagnostics;
namespace NostalgiaEngineApplication
{

    class Program
    {


        static void Main(string[] args)
        {
            //NEConsoleColorDef d = new NEConsoleColorDef(144, 1255, 98);

            //uint r = d.R;
            //uint g = d.G;
            //uint b = d.B;
            //Console.ReadLine();
            Engine engine = new Engine();
            NETextureEditor ed = new NETextureEditor();
            NERaycaster2D raycaster = new NERaycaster2D();
            engine.Start(raycaster);

        }
    }
}
