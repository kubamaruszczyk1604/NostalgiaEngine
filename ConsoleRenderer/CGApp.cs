using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer
{
    public class CGApp
    {
        public string Title { get; protected set; }
        public int ScreenWidth { get; protected set; }
        public int ScreenHeight { get; protected set; }
        public int PixelWidth { get; protected set; }
        public int PixelHeight { get; protected set; }

        public virtual void OnInitialize() { }
        public virtual void OnStart() { }
        public virtual void OnUpdate(float deltaTime) { }
        public virtual void OnDrawPerColumn(int x) { }
        public virtual void OnPostDraw() { }
        public virtual void OnExit() { }
    }
}
