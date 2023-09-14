using System;
using System.IO;
using NostalgiaEngineExtensions.TextureEditor;
using NostalgiaEngine.Core;
using NostalgiaEngine.Raycaster;
using TextureDisplay;
using NostalgiaEngine.Demos;
using NostalgiaEngine.Extensions;

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
            //string[] paths = new string[] {
            //    "ImageViewerDemoImages/nostalgia", "ImageViewerDemoImages/mario","ImageViewerDemoImages/dyna_intro",
            //    "ImageViewerDemoImages/dyna_gameplay","ImageViewerDemoImages/balcony","ImageViewerDemoImages/water",
            //    "ImageViewerDemoImages/playground", "ImageViewerDemoImages/mallorca", "ImageViewerDemoImages/kitchen",
            //    "ImageViewerDemoImages/food", "ImageViewerDemoImages/eastbourne"};

            string[] paths = Directory.GetDirectories("ImageViewerDemoImages");
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
        
        static void ConsoleCameraDemo(Engine engine, int w, int h, int pixW, int pixH)
        {
            ConsoleCamera consoleCamera = new ConsoleCamera(w, h, pixW, pixH);
            engine.Start(consoleCamera);
        }

        static void SortingDemo(Engine engine)
        {
            SortingVis sortingVis = new SortingVis();
            engine.Start(sortingVis);
        }

        static void OscilloscopeDemo(Engine engine)
        {
            Oscilloscope oscilloscope = new Oscilloscope();
            engine.Start(oscilloscope);
        }

        static void Main(string[] args)
        {

            Engine engine = new Engine();
            //TextureEditorDemo(engine);
            //RaycasterDemo(engine);

            //ImageViewerDemo(engine);
            //AnalogClockDemo(engine);
            //BandLevelDemo(engine);

            //NoiseDemo(engine);
            //CubeDemo(engine);
            //TeapotDemo(engine);
             NightGardenDemo(engine);
            //SortingDemo(engine);
            //ShooterDemo(engine);

             //ConsoleCameraDemo(engine, 150, 50, 8, 16);
            //ConsoleCameraDemo(engine, 190, 50, 8, 16);
            //ConsoleCameraDemo(engine, 220, 140, 4, 4);


             //OscilloscopeDemo(engine);
           
        }

    }
}
