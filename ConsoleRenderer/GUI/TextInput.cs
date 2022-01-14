using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NostalgiaEngine.Core;
using System.IO;
namespace NostalgiaEngine.GUI
{
    public class NETextInput : INEGUIElement
    {

        public delegate void OnLineCommit(string line);

        private string m_DataString;
        private int m_CursorPos;
        private ConsoleKeyInfo m_KeyInfo;
        private NEPoint m_Position;
        private readonly int c_HorizontalOffset = 5;
        private readonly int c_VerticalOffset = 2;

        public OnLineCommit onLineCommit { get; set; }


        public NETextInput(string defaultStr, int x, int y, bool inFocus = false)
        {
            m_DataString = defaultStr;
            m_CursorPos = defaultStr.Length;
            m_Position = new NEPoint();
            m_Position.X = (short)x;
            m_Position.Y = (short)y;
        }

        public void Reset(string defaultStr = "")
        {
            m_DataString = defaultStr;
            m_CursorPos = defaultStr.Length;
        }

        public void InputUpdate()
        {
            if (!Focused) return;
            
            Console.CursorVisible = true;
            Console.SetCursorPosition(m_Position.X + c_HorizontalOffset + m_CursorPos, m_Position.Y + c_VerticalOffset);

            m_KeyInfo = Console.ReadKey(true);
            if (char.IsLetterOrDigit(m_KeyInfo.KeyChar) || char.IsPunctuation(m_KeyInfo.KeyChar))
            {
                m_DataString = m_DataString.Insert(m_CursorPos, m_KeyInfo.KeyChar.ToString());
                m_CursorPos++;
            }
            if (m_KeyInfo.Key == ConsoleKey.Backspace && m_CursorPos > 0)
            {
                m_DataString = m_DataString.Remove(m_CursorPos - 1, 1);
                m_CursorPos--;
            }
            else if (m_KeyInfo.Key == ConsoleKey.LeftArrow)
            {
                if (m_CursorPos > 0) m_CursorPos--;
            }
            else if (m_KeyInfo.Key == ConsoleKey.RightArrow)
            {
                if (m_CursorPos < m_DataString.Length) m_CursorPos++;
            }
            else if (m_KeyInfo.Key == ConsoleKey.Enter)
            {
                onLineCommit?.Invoke(m_DataString);
                //m_FocusedFlag = false;
            }
        }

        public void Draw(short col = 15)
        {
            NEConsoleScreen.WriteXY(m_Position.X, m_Position.Y, col, m_DataString);

            Console.SetCursorPosition(m_Position.X + c_HorizontalOffset + m_CursorPos, m_Position.Y + c_VerticalOffset);
        }


        public override void Dispose()
        {
            base.Dispose();
            if(onLineCommit != null)
            foreach(var e in onLineCommit.GetInvocationList())
            {
                onLineCommit += (OnLineCommit)e;
            }
        }
    }
}
