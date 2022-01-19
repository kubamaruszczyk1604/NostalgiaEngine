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


        private NEConsoleColorDef[] m_Colors;

        public NEColorPalette()
        {
            m_Colors = new NEConsoleColorDef[16];
            Array.Copy(DefaultPalette, m_Colors, 16);
        }

        public NEColorPalette(NEConsoleColorDef[] src)
        {
            m_Colors = new NEConsoleColorDef[16];
            Array.Copy(src, m_Colors, 16);
            
        }


    }
}
