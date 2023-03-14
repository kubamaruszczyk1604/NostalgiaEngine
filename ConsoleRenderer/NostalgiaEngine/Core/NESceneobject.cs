using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{
    abstract public class NESceneObject
    {
        public Transform Transform { get; private set; }

        private string m_Name;
        public string Name { get { return m_Name; } set { m_Name = (value == null) ? "null" : value; } }

        public bool Enabled { get; set; }

        private List<NESceneObject> m_Childreen;
        
        protected NESceneObject()
        {
            Transform = new Transform();
            Name = "NESceneObject";
            Enabled = true;
            m_Childreen = new List<NESceneObject>();
        }

        public override string ToString()
        {
            return Name;
        }

        public void AddChild(NESceneObject sceneObject)
        {
            m_Childreen.Add(sceneObject);
        }


    }
}
