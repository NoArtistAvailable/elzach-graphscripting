using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace elZach.SimpleAI.BehaviourTree
{
    public abstract class Composite : Node
    {
        public List<Node> Children = new List<Node>();

        public override Color GetColor() => new Color(0f, 0.25f, 0.4f); 
        // public Composite(TreeDirector director, List<Node> children) : base(director)
        // {
        //     
        // }
        public override void Init(TreeDirector director)
        {
            base.Init(director);
            foreach(var child in Children) child.Init(director);
        }

        public override Node Clone()
        {
            Composite node = base.Clone() as Composite;
            node.Children = Children.ConvertAll(c => c.Clone());
            return node;
        }
    }
}