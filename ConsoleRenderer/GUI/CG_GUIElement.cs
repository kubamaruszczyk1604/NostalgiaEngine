using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer.Tools
{
    public interface CG_GUIElement
    {
        void Focus();
        void UnFocus();
        bool InFocus { get; }
    }

}
