using System.Collections.Generic;
using UnityEngine;

namespace elZach.GraphScripting
{
    public abstract class Composite : Node
    {
        public List<Node> Children = new List<Node>();

        public override Color GetColor() => new Color(0f, 0.25f, 0.4f); 
        
        internal override void GetParametersRecursive(ref List<TreeContainer.Parameter> parameters)
        {
            base.GetParametersRecursive(ref parameters);
            foreach(var child in Children)
                if(child) child.GetParametersRecursive(ref parameters);
        }
        
        public override void Init()
        {
            base.Init();
            foreach(var child in Children) child.Init();
        }

        public override Node Clone()
        {
            Composite node = base.Clone() as Composite;
            node.Children = Children.ConvertAll(c => c.Clone());
            return node;
        }
    }
}