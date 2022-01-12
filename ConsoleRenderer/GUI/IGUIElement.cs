using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Tools
{
    public interface INEGUIElement
    {
        void Focus();
        void UnFocus();
        bool InFocus { get; }
    }

}
