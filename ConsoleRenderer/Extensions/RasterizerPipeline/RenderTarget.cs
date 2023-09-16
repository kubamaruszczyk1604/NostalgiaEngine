using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.RasterizerPipeline
{
    public struct FragmentData
    {
        public byte BKG;
        public byte FG;
        public float T;
        public float Depth;
        public float CamDistance;

        public void Set(byte bkg, byte fg, float t, float depth, float camDist)
        {
            BKG = bkg;
            FG = fg;
            T = t;
            Depth = depth;
            CamDistance = camDist;
        }
    }

    public class RenderTarget
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public FragmentData[] DATA;
        public int Size { get { return DATA.Length; } }

        public RenderTarget(int w, int h)
        {
            Width = w;
            Height = h;
            DATA = new FragmentData[w * h];
        }

        public void Write(int x, int y, byte bkg, byte fg, float t, float depth, float camDist)
        {
            int i = Width * y + x;
            DATA[i].Set(bkg, fg, t, depth, camDist);
        }

        public void Read(int x, int y, out FragmentData renderData)
        {
            int i = Width * y + x;
            renderData = DATA[i];
        }

        public float GetDepth(int x, int y)
        {
            int i = Width * y + x;
            return DATA[i].Depth;
        }

        public float GetCamDistance(int x, int y)
        {
            int i = Width * y + x;
            return DATA[i].CamDistance;
        }

        public NEColorSample GetColorSample(int x, int y, int[] charRamp, float intensityModifier = 1.0f)
        {
            int i = Width * y + x;
            return NEColorSample.MakeCol((ConsoleColor)DATA[i].BKG, (ConsoleColor)DATA[i].FG, DATA[i].T * intensityModifier, charRamp);
        }

    }
}
