using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{
    public delegate void OnSceneExit(NEScene scene);
    public class NEScene
    {
        public string Title { get; protected set; }
        public int ScreenWidth { get; protected set; }
        public int ScreenHeight { get; protected set; }
        public int PixelWidth { get; protected set; }
        public int PixelHeight { get; protected set; }
        public bool ParallelScreenDraw{ get; protected set; }
        public object ReturnData { get; private set; }
        public Type ReturnDataType { get; private set; }
        public OnSceneExit onSceneExit { get; set; }
        

        public void Exit(object returnData = null)
        {
            NEInput.FlushKeyboard();
            ReturnData = returnData;
            if (ReturnData != null)
            {
                ReturnDataType = ReturnData.GetType();
            }
            Engine.Instance.PopScene();
            onSceneExit?.Invoke(this);
            onSceneExit = null;
        }
        public virtual bool OnLoad() { NEInput.FlushKeyboard(); return true; }
        public virtual void OnStart() { NEInput.FlushKeyboard(); }
        public virtual void OnPause() { NEInput.FlushKeyboard(); }
        public virtual void OnResume() { NEInput.FlushKeyboard(); }
        public virtual void OnUpdate(float deltaTime) { }
        public virtual void OnDrawPerColumn(int x) { }
        public virtual void OnDraw() { }
        public virtual void OnExit()
        {
            NEInput.FlushKeyboard();
        }
    }
}
 