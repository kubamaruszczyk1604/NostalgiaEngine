using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using NostalgiaEngineExtensions.TextureEditor;
using NostalgiaEngine.Core;
using NostalgiaEngine.Raycaster;
using NostalgiaEngine.RasterizerPipeline;
using TextureDisplay;
using NostalgiaEngine.ConsoleGUI;
using System.Diagnostics;
using NostalgiaEngine.Demos;
namespace NostalgiaEngineApplication
{

    class Program
    {

        static void TextureEditorDemo(Engine engine)
        {
            NETextureEditor ed = new NETextureEditor();
            engine.Start(ed);
        }

        static void RaycasterDemo(Engine engine)
        {
            NERaycaster2D raycaster = new NERaycaster2D();
            engine.Start(raycaster);
        }

        static void ImageViewerDemo(Engine engine)
        {
            string[] paths = new string[] {
                "ImageViewerDemoImages/nostalgia", "ImageViewerDemoImages/mario","ImageViewerDemoImages/dyna_intro",
                "ImageViewerDemoImages/dyna_gameplay","ImageViewerDemoImages/balcony","ImageViewerDemoImages/water",
                "ImageViewerDemoImages/playground", "ImageViewerDemoImages/mallorca", "ImageViewerDemoImages/kitchen",
                "ImageViewerDemoImages/food", "ImageViewerDemoImages/eastbourne"};
            AsciiImageViewer imageViewer = new AsciiImageViewer(paths);

            engine.Start(imageViewer);
        }

        static void AnalogClockDemo(Engine engine)
        {
            AnalogClock analogClock = new AnalogClock();
            engine.Start(analogClock);
        }

        static void BandLevelDemo(Engine engine)
        {
            BandLevelDemo bandDemo = new BandLevelDemo();
            engine.Start(bandDemo);
        }

        static void NightGardenDemo(Engine engine)
        {
            NightGardenScene3D scene3D = new NightGardenScene3D();
            engine.Start(scene3D);
        }

        static void TeapotDemo(Engine engine)
        {
            TeapotDemo teapotDemo = new TeapotDemo();
            engine.Start(teapotDemo);
        }

        static void CubeDemo(Engine engine)
        {
            RotatingCubeDemo cubeDemo = new RotatingCubeDemo();
            engine.Start(cubeDemo);
        }

        static void NoiseDemo(Engine engine)
        {
            ScreenNoiseDemo screenNoiseDemo = new ScreenNoiseDemo();
            engine.Start(screenNoiseDemo);
        }

        static void ShooterDemo(Engine engine)
        {
            ProceduralShooterDemo pcs = new ProceduralShooterDemo();
            engine.Start(pcs);
        }



        static void Main(string[] args)
        {

            Engine engine = new Engine();
            //TextureEditorDemo(engine);
            //RaycasterDemo(engine);

            //ImageViewerDemo(engine);
            //AnalogClockDemo(engine);
             BandLevelDemo(engine);
             //NightGardenDemo(engine);
             //TeapotDemo(engine);
             // CubeDemo(engine);
            // NoiseDemo(engine);

            //ShooterDemo(engine);

        }

    }
}
