using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{
    public class NEDebug
    {
        static private int paletteCellWidth = 10;
        static private int paletteCellHeight = 10;

        static private NEVector2 paletteStripPos = new NEVector2(10, 10);
        public static void DrawPalette(int screenWidth, int screenHeight)
        {
            paletteCellWidth = (int)(screenWidth * 0.039f);
            paletteCellHeight = paletteCellWidth;
            paletteStripPos = new NEVector2(paletteCellWidth *0.5f, paletteCellWidth *0.5f);
            for (int y = (int)paletteStripPos.Y; y < ((int)paletteStripPos.Y + paletteCellHeight); ++y)
            {
                for (int x = 0; x < screenWidth; ++x)
                {
                     DrawPalettePix(x, y);
                }
            }
        }

        private static void DrawPalettePix(int x, int y)
        {


            NEVector2 pixelPos = new NEVector2(x, y);
            for (int i = 0; i < 16; ++i)
            {
                if (NEMath.InRectangle(pixelPos, new NEVector2(paletteCellWidth * (i), 0) + paletteStripPos, paletteCellWidth-1, paletteCellHeight))
                {
                    NEScreenBuffer.PutChar(' ', (short)((i) << 4), x, y);
                    if (i == 16) NEScreenBuffer.PutChar((char)NEBlock.Solid, (short)(8 << 4), x, y);
                }
            }
        }

        static public void Print(string message)
        {
            System.Diagnostics.Debug.Print("NEDebug|  " + message);
        }
        static public void Print(int message)
        {
            System.Diagnostics.Debug.Print("NEDebug|  (int): " + message.ToString());
        }

        static public void Print(uint message)
        {
            System.Diagnostics.Debug.Print("NEDebug|  (uint): " + message.ToString());
        }

        static public void Print(short message)
        {
            System.Diagnostics.Debug.Print("NEDebug|  (short): " + message.ToString());
        }

        static public void Print(float message)
        {
            System.Diagnostics.Debug.Print("NEDebug|  (float): " + message.ToString());
        }

        static public void Print(double message)
        {
            System.Diagnostics.Debug.Print("NEDebug|  (double): " + message.ToString());
        }
    }
}
