using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.GUI
{
    public class NEYesNoWindow
    {
        NEWindowRect m_Window;
        string m_Question;

        public NEYesNoWindow(int x, int y,int w, int h, string question)
        {
            m_Question = question;
            if (w < question.Length)
            {
                w = question.Length + 5;
            }
            if (h < 10) h = 10;
            m_Window = new NEWindowRect(x, y, w, h, question);
        }


        public void Draw()
        {
            m_Window.Draw();
            NEScreen.WriteXY(m_Window.X + m_Window.W / 8, m_Window.Y + m_Window.H / 2, (2 << 4) | 15, "  YES - ENTER  ");
            NEScreen.WriteXY(m_Window.X + m_Window.W - m_Window.W/8- 15, m_Window.Y + m_Window.H / 2, (12 << 4) | 15, "   NO - ESC    ");
        }
    }
}
