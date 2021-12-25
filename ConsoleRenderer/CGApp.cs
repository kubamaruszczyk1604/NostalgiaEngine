using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer
{
    public class CGApp
    {

        public virtual void OnStart() { }
        public virtual void OnUpdate(float deltaTime) { }
        public virtual void OnDrawPerColumn(int x) { }
        public virtual void OnDrawPerPixel(int x, int y) { }
        public virtual void OnPostDraw() { }
        public virtual void OnExit() { }
    }
}
