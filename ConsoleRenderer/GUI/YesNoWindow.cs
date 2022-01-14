using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;

namespace NostalgiaEngine.GUI
{
    public enum NEWindowStyle { Normal = 0, Warning = 1, Error =2};
    public class NEYesNoWindow :INEGUIElement
    {
        public delegate void OnUserSelection(bool yn);

        public OnUserSelection onUserSelection { get; set; }
        private NEWindowRect m_Window;
        private NEWindowStyle m_Style;
        private string m_Question;

        public NEYesNoWindow(int x, int y,int w, int h, string question, NEWindowStyle style = NEWindowStyle.Normal)
        {
            m_Question = question;
            if (w < question.Length)
            {
                w = question.Length + 5;
            }
            if (h < 10) h = 10;
            m_Style = style;
            m_Window = new NEWindowRect(x, y, w, h, question);
            if(style == NEWindowStyle.Warning)
            {
                m_Window.BarColor = 14;
                m_Window.TextBarColor = 1;
            }

            if(style == NEWindowStyle.Error)
            {
                m_Window.BarColor = 12;
                m_Window.TextBarColor = 15;
                m_Window.BodyColor = 4;
            }
        }

        public void Update()
        {
            if (!Focused) return;

            if(NEInput.CheckKeyPress(NEKey.Enter))
            {
                onUserSelection?.Invoke(true);
            }
            if(NEInput.CheckKeyPress(NEKey.Escape))
            {
                onUserSelection?.Invoke(false);
            }
        }

        public void Draw()
        {
            m_Window.Draw();
            NEConsoleScreen.WriteXY(m_Window.X + m_Window.W / 8, m_Window.Y + m_Window.H / 2, (2 << 4) | 15, "  YES - ENTER  ");
            NEConsoleScreen.WriteXY(m_Window.X + m_Window.W - m_Window.W/8- 15, m_Window.Y + m_Window.H / 2, (12 << 4) | 15, "   NO - ESC    ");
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void Focus()
        {
            base.Focus();
            if(m_Style == NEWindowStyle.Normal)
            {
                NEConsoleSounds.AB_Beep();
            }
            else if(m_Style == NEWindowStyle.Warning)
            {
                NEConsoleSounds.WarningBeep();
            }
            else
            {
                NEConsoleSounds.FailBeep();
            }
        }
    }
}
