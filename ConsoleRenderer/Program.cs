using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using NostalgiaEngineExtensions.TextureEditor;
using NostalgiaEngine.Core;
using NostalgiaEngine.Raycaster;
using TextureDisplay;
using NostalgiaEngine.ConsoleGUI;
using System.Diagnostics;
using ImageReader;
namespace NostalgiaEngineApplication
{

    class Program
    {


        static void Main(string[] args)
        {

            Engine engine = new Engine();
            NETextureEditor ed = new NETextureEditor();
            NERaycaster2D raycaster = new NERaycaster2D();
            PhotoViewer demo = new PhotoViewer();
            engine.Start(raycaster);


        }
    }
}
