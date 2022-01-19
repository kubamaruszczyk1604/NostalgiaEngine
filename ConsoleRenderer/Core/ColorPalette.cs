using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{
    public class NEColorPalette
    {
        static public readonly NEConsoleColorDef[] DefaultPalette =
        {
            new NEConsoleColorDef(0, 0, 0), new NEConsoleColorDef(0, 55, 218), new NEConsoleColorDef(19, 161, 14),
            new NEConsoleColorDef(58, 150, 221), new NEConsoleColorDef(195,17, 31), new NEConsoleColorDef(136, 23, 152),
            new NEConsoleColorDef(193, 156, 0), new NEConsoleColorDef(204, 204, 204), new NEConsoleColorDef(118, 118,118),
            new NEConsoleColorDef(59, 120, 255), new NEConsoleColorDef(22, 198, 12), new NEConsoleColorDef(97, 214, 214),
            new NEConsoleColorDef(231, 72, 86), new NEConsoleColorDef(180, 0, 158), new NEConsoleColorDef(249, 213, 150),
            new NEConsoleColorDef(242, 242, 242)
        };

        static public readonly NEConsoleColorDef[] ColorSpectrumPalette_1 = {
            new NEConsoleColorDef(0, 0, 0), new NEConsoleColorDef(15, 0, 229), new NEConsoleColorDef(175, 0, 229),
            new NEConsoleColorDef(229, 0, 114), new NEConsoleColorDef(229, 0, 15), new NEConsoleColorDef(229, 80, 0),
            new NEConsoleColorDef(229, 172, 0), new NEConsoleColorDef(229, 218, 0), new NEConsoleColorDef(95, 229, 0),
            new NEConsoleColorDef(0, 229, 3), new NEConsoleColorDef(0, 229, 130), new NEConsoleColorDef(0, 221, 229),
            new NEConsoleColorDef(0, 156, 229), new NEConsoleColorDef(0, 91, 229), new NEConsoleColorDef(179, 184, 229),
            new NEConsoleColorDef(242, 242, 242)};

        static public readonly NEConsoleColorDef[] NostalgiaPalette =
{
            new NEConsoleColorDef(0, 0, 0), new NEConsoleColorDef(0, 55, 218), new NEConsoleColorDef(82, 151, 255),
            new NEConsoleColorDef(255, 0, 110), new NEConsoleColorDef(127, 0, 0), new NEConsoleColorDef(136, 23, 152),
            new NEConsoleColorDef(94, 43, 43), new NEConsoleColorDef(80, 90, 90), new NEConsoleColorDef(118, 118,118),
            new NEConsoleColorDef(0, 160, 0), new NEConsoleColorDef(0, 225, 33), new NEConsoleColorDef(255, 255, 0),
            new NEConsoleColorDef(255, 95, 0), new NEConsoleColorDef(255, 12, 0), new NEConsoleColorDef(208, 165, 142),
            new NEConsoleColorDef(242, 242, 242)
        };


        public NEConsoleColorDef[] Colors { get; private set; }


        public NEColorPalette()
        {
            Colors = new NEConsoleColorDef[16];
            Array.Copy(DefaultPalette, Colors, 16);
        }

        public NEColorPalette(NEConsoleColorDef[] src)
        {
            Colors = new NEConsoleColorDef[16];
            Array.Copy(src, Colors, 16);          
        }

        public void MultiplyBy(float val)
        {
            for (int i = 0; i < Colors.Length; ++i)
            {
                Colors[i] *= val;
            }
        }

        public void MultiplyBy(NEConsoleColorDef col)
        {
            for(int i =0; i < Colors.Length; ++i)
            {
                Colors[i] *= col;
            }
        }

        public void MultiplyBy(NEColorPalette pal)
        {
            for (int i = 0; i < Colors.Length; ++i)
            {
                Colors[i] *= pal.Colors[i];
            }
        }

        public void Subtract(NEConsoleColorDef col)
        {
            for (int i = 0; i < Colors.Length; ++i)
            {
                Colors[i] -= col;
            }
        }

        public void Subtract(NEColorPalette pal)
        {
            for (int i = 0; i < Colors.Length; ++i)
            {
                Colors[i] -= pal.Colors[i];
            }
        }

        public void InvertColors()
        { 
            for (int i = 0; i < Colors.Length; ++i)
            {
                Colors[i] = -Colors[i];
            }
        }

        public NEConsoleColorDef GetColor(int colIndex)
        {
            colIndex = NEMathHelper.Clamp(colIndex, 0, 15);
            return Colors[colIndex];
        }

        public bool SetColor(int colIndex, NEConsoleColorDef col)
        {
            if (colIndex > 15) return false;
            if (colIndex < 0) return false;
            Colors[colIndex] = col;
            return true;
        }

        public bool SetColor(int colIndex, uint r, uint g, uint b)
        {
            return SetColor(colIndex, new NEConsoleColorDef(r, g, b));
        }

        public bool SetColor(int colIndex, float r, float g, float b)
        {
            return SetColor(colIndex, new NEConsoleColorDef(r, g, b));
        }

    }
}
