using NostalgiaEngineExtensions.TextureEditor;
using NostalgiaEngine.Core;
using NostalgiaEngine.Raycaster;
using TextureDisplay;
using NostalgiaEngine.Demos;
using NostalgiaEngine.Extensions;
using System;
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
            string[] paths = new string[] {"C:/Users/Kuba/Documents/NE_Texture",
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
        
        static void ConsoleCameraDemo(Engine engine)
        {
            ConsoleCamera consoleCamera = new ConsoleCamera();
            engine.Start(consoleCamera);
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
            //NightGardenDemo(engine);
            //ShooterDemo(engine);
             ConsoleCameraDemo(engine);


        }

    }
}
