using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.GUI
{
    public class INEGUIElement: IDisposable
    {
        private static List<INEGUIElement> s_Elements = new List<INEGUIElement>();


        protected INEGUIElement()
        {
            s_Elements.Add(this);
        }

        public void Focus()
        {
            foreach (var el in s_Elements)
            {
                el.Focused = false;
            }
            Focused = true;
        }

        public bool Focused { get; private set; } 

        public virtual void Dispose()
        {
            s_Elements.Remove(this);
        }
    }
}
