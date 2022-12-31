using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using NostalgiaEngineExtensions.TextureEditor;
using NostalgiaEngine.Core;
using NostalgiaEngine.Raycaster;
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
            Note[] notes = new Note[] { new Note(Note.GetNoteFrequency(12), 510)
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
            };
            //NESoundSynth.Play(notes);


            //Engine engine = new Engine();
            //NETextureEditor ed = new NETextureEditor();
            //NERaycaster2D raycaster = new NERaycaster2D();
            //PhotoViewer demo = new PhotoViewer();
            //BandLevelDemo bld = new BandLevelDemo();
            //TextDemo ted = new TextDemo();
            //engine.Start(raycaster);


            //Envelope env = new Envelope(200, 300, 200);
            //env.SampleEnvelope(44000, 300);



            NEMatrix2x2 m1 = NEMatrix2x2.CreateRotation(0.5f);
            NEMatrix2x2 m2 = new NEMatrix2x2(3, 8, 8, 9);

            NEVector2 r = m1 * new NEVector2(3, 4);
            
      
            Console.Write(r.ToString());
            Console.ReadLine();

        }
    }
}
