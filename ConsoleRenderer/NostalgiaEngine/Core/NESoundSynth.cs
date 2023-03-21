using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

// Work in progress..
namespace NostalgiaEngine.Core
{
    public struct Note
    {
        public UInt16 Frequency;//Hz
        public int Duration; //ms
        public float Volume;

        public Note(UInt16 freq, int duration, float volume = 1.0f)
        {
            Frequency = freq;
            Duration = duration;
            Volume = volume;
        }

        public static UInt16 GetNoteFrequency(uint noteI)
        {
            return (UInt16)(Math.Pow(2, ((double)noteI) / 12.0) * 110.0);
        }
    }

    public class Envelope
    {
        public int AttackMilliseconds { get; set; }
        public int DecayMilliseconds { get; set; }
        public int SustainMilliseconds { get; private set; }
        public int ReleaseMilliseconds { get; set; }
        public int SampleRate { get; set; }


        public int GetTotalDuration(Note note)
        {
            return AttackMilliseconds + DecayMilliseconds + ReleaseMilliseconds + note.Duration;
        }

        public Envelope(int attack_ms, int decay_ms, int release_ms, int sampleRate = 44100)
        {
            AttackMilliseconds = attack_ms<0?0:attack_ms;
            DecayMilliseconds = decay_ms<0?0:decay_ms;
            SustainMilliseconds = 0;
            ReleaseMilliseconds = release_ms<0?0:release_ms;
            SampleRate = sampleRate > 0 ? sampleRate : 44100;
        }

        public double SampleEnvelope(int sampleIndex, int noteDuration)
        {
            int msecs = (int) Math.Round(((double)sampleIndex) / ((double)SampleRate) * 1000);
            if(msecs <= AttackMilliseconds) //Attack phase
            {
                Console.WriteLine("Attack");
               
            }
            else if(msecs <= (DecayMilliseconds + AttackMilliseconds)) //Decay phase
            {
                Console.WriteLine("Decay");
            }
            else if(msecs <= (noteDuration+DecayMilliseconds+AttackMilliseconds)) //Sustain phase
            {
                Console.WriteLine("Sustain");
            }
            else if(msecs <= (ReleaseMilliseconds + noteDuration + DecayMilliseconds + AttackMilliseconds)) //Release phase
            {
                Console.WriteLine("Release");
            }
            else
            {
                Console.WriteLine("Ended");
                return 0.0;
            }
            return 1.0;
        }

    }
    public class NESoundSynth
    {
        static readonly Int32 RIFF_TAG = 0x46464952;
        static readonly Int32 WAVE_TAG = 0x45564157;
        static readonly Int32 fmt_TAG = 0x20746D66;
        static readonly Int32 data_TAG = 0x61746164;
        static readonly double TAU = 2 * Math.PI;
        static int s_ChunkSize = 16;
        static int s_HeaderSize = 8;
        static short s_FormatType = 1;
        static short s_Tracks = 1;
        static int s_SamplesPerSecond = 44100;
        static short s_BitsPerSample = 16;
        static short s_frameSize = (short)(s_Tracks * ((s_BitsPerSample + 7) / 8));
        static int s_BytesPerSecond = s_SamplesPerSecond * s_frameSize;

        public static void PlayBeep(UInt16 frequency, int msDuration, UInt16 volume = 16383)
        {
            var mStrm = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(mStrm);



            int waveSize = 4;
            int samples = (int)((decimal)s_SamplesPerSecond * msDuration / 1000);
            int dataChunkSize = samples * s_frameSize;
            int fileSize = waveSize + s_HeaderSize + s_ChunkSize + s_HeaderSize + dataChunkSize;
            // var encoding = new System.Text.UTF8Encoding();
            writer.Write(RIFF_TAG); 
            writer.Write(fileSize);
            writer.Write(WAVE_TAG);
            writer.Write(fmt_TAG); 
            writer.Write(s_ChunkSize);
            writer.Write(s_FormatType);
            writer.Write(s_Tracks);
            writer.Write(s_SamplesPerSecond);
            writer.Write(s_BytesPerSecond);
            writer.Write(s_frameSize);
            writer.Write(s_BitsPerSample);
            writer.Write(data_TAG);
            writer.Write(dataChunkSize);
            {
                double theta = frequency * TAU / (double)s_SamplesPerSecond;
                // 'volume' is UInt16 with range 0 thru Uint16.MaxValue ( = 65 535)
                // we need 'amp' to have the range of 0 thru Int16.MaxValue ( = 32 767)
                double amp = volume/ 2; // so we simply set amp = volume / 2
                for (int step = 0; step < samples; step++)
                {
                    short s = (short)(amp * Math.Sin(theta * (double)step));
                    writer.Write(s);
                }
            }

            mStrm.Seek(0, SeekOrigin.Begin);
            var player = new System.Media.SoundPlayer(mStrm);
            player.PlaySync();
            writer.Close();
            mStrm.Close();
        }


        static short SineWave(double a, double w, int step)
        {
            return (short)(a * Math.Sin(w * step));
        }

        static short SineWaveInOctave(double a, double w, int step)
        {
            short s = (short)(a / 2 * Math.Sin(w * step));
            s += (short)(a / 2 * Math.Sin(w * 2.0 *step));
            return s;
        }
        static short SineWaveOverDriveOctave(double a, double w, int step)
        {
            short s = (short)(a * Math.Sin(w * (double)step));
            s += (short) (a * Math.Sin(w*2 * (double) step));
            s /= 2;
            return s;
        }

        static short SquareWave(double a, double w, int step)
        {
            return (short)(a * Math.Sign(Math.Sin(w * step)));
        }

        public static void Play(Note[] notes)
        {
            var mStrm = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(mStrm);

            int msDuration = 0;
            foreach(Note note in notes)
            {
                msDuration += note.Duration;
            }

            int waveSize = 4;
            int samples = (int)((decimal)s_SamplesPerSecond * msDuration / 1000);
            int dataChunkSize = samples * s_frameSize;
            int fileSize = waveSize + s_HeaderSize + s_ChunkSize + s_HeaderSize + dataChunkSize;
            // var encoding = new System.Text.UTF8Encoding();
            writer.Write(RIFF_TAG);
            writer.Write(fileSize);
            writer.Write(WAVE_TAG);
            writer.Write(fmt_TAG);
            writer.Write(s_ChunkSize);
            writer.Write(s_FormatType);
            writer.Write(s_Tracks);
            writer.Write(s_SamplesPerSecond);
            writer.Write(s_BytesPerSecond);
            writer.Write(s_frameSize);
            writer.Write(s_BitsPerSample);
            writer.Write(data_TAG);
            writer.Write(dataChunkSize);
            for(int i =0; i < notes.Length;++i)
            {
                double theta = notes[i].Frequency * TAU / (double)s_SamplesPerSecond;

                double amp = notes[i].Volume *Int16.MaxValue;
                int sampleCount = (int)((decimal)s_SamplesPerSecond * notes[i].Duration / 1000);
                for (int step = 0; step <sampleCount; step++)
                {
                    short s = SineWaveOverDriveOctave(amp, theta, step);

                    writer.Write(s);
                }
            }

            mStrm.Seek(0, SeekOrigin.Begin);
            var player = new System.Media.SoundPlayer();
          
            player.Stream = mStrm;
            player.Play();
            writer.Close();
            mStrm.Close();
        }

    }
}
