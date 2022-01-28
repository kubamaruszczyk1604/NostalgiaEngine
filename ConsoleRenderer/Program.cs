using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using NostalgiaEngine.TextureEditor;
using NostalgiaEngine.Core;
using NostalgiaEngine.Raycaster;
using NostalgiaEngine.TextureDisplay;
using NostalgiaEngine.GUI;
using System.Diagnostics;
using ImageReader;
namespace NostalgiaEngineApplication
{

    class Program
    {


        static void Main(string[] args)
        {

            //BitmapRGB b = BitmapRGB.FromFile("C:/test/fd.bmp");

            //PixelRGB p = b.GetPixel(2, 2);

            Engine engine = new Engine();
            NETextureEditor ed = new NETextureEditor();
            NERaycaster2D raycaster = new NERaycaster2D();
            NETexturePreviewApp demo = new NETexturePreviewApp();
            engine.Start(demo);

        }
    }
}
