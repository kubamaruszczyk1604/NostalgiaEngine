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
        
        protected NESceneObject()
        {
            Transform = new Transform();
            Name = "NESceneObject";
            Enabled = true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
