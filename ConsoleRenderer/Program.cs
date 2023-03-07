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

        static void Main(string[] args)
        {
            //Note[] notes = new Note[] { new Note(Note.GetNoteFrequency(12), 510),
            //    new Note(Note.GetNoteFrequency(13), 110),
            //new Note(Note.GetNoteFrequency(14), 110),
            // new Note(Note.GetNoteFrequency(15), 110),
            //  new Note(Note.GetNoteFrequency(16), 110),
            //   new Note(Note.GetNoteFrequency(17), 110),
            //    new Note(Note.GetNoteFrequency(18), 110),
            //     new Note(Note.GetNoteFrequency(19), 110),
            //      new Note(Note.GetNoteFrequency(20), 110),
            //       new Note(Note.GetNoteFrequency(21), 110),
            //        new Note(Note.GetNoteFrequency(22), 110),
            //         new Note(Note.GetNoteFrequency(23), 110),
            //          new Note(Note.GetNoteFrequency(24), 110),
            //};
           // NESoundSynth.Play(notes);


            Engine engine = new Engine();
            NETextureEditor ed = new NETextureEditor();
            NERaycaster2D raycaster = new NERaycaster2D();
            string[] paths = new string[] {
                @"C:\test\nowa_textura10", @"C:\test\nowa_textura4",
                @"C:\test\nowa_textura6", @"C:\test\nowa_textura5",@"C:\test\balcony",@"C:\test\water",
                @"C:\test\playground", @"C:\test\nowa_textura", @"C:\test\nowa_textura2", @"C:\test\food",
                @"C:\test\example1"};
            PhotoViewer demo = new PhotoViewer(paths);
            BandLevelDemo bld = new BandLevelDemo();
            TextDemo ted = new TextDemo();
            //Scene3D sc = new Scene3D();

            ExampleRasterizerScene sc = new ExampleRasterizerScene();
            engine.Start(sc);


            //Envelope env = new Envelope(200, 300, 200);
            //env.SampleEnvelope(44000, 300);


            //NEVector4 left = new NEVector4(-1.0f, 1.0f, 1.0f);
            //NEVector4 right = new NEVector4(1.0f, 1.0f, 1.0f);

            //bool res = NEVector4.CompareLeft(left, right, NEVector4.Up);
            //Console.WriteLine(res);
            //Console.ReadLine();


            //Console.WriteLine("Matrix-Vector multiply test: " + (NEMatrix4x4.UnitTest_MatVecMultiply()?"pass":"fail"));
            //Console.WriteLine("Matrix-Matrix multiply test: " + (NEMatrix4x4.UnitTest_MatMatMultiply() ? "pass" : "fail"));
            //Console.ReadLine();

        }
    }
}
