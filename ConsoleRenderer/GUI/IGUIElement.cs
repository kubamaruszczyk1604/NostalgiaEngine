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


    public class NEGUIMgr
    {
        static List<INEGUIElement> s_Elements = new List<INEGUIElement>();

        public static void RegisterElement(INEGUIElement element)
        {
            if(!s_Elements.Contains(element))
            {
                s_Elements.Add(element);
            }
        }

        public static void Focus(INEGUIElement element)
        {
            RegisterElement(element);
            foreach(var el in s_Elements)
            {
                el.UnFocus();
            }
            element.Focus();
        }


    }

}
