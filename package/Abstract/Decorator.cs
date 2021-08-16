using UnityEngine;

namespace elZach.GraphScripting
{
    public abstract class Decorator : Node
    {
        public Node Child;
        // override void Init(TreeDirector director, Node child)
        // {
        //     Child = child;
        // }
        public override void Init(TreeDirector director)
        {
            base.Init(director);
            if(Child) Child.Init(director);
        }

        protected override State OnUpdate()
        {
            return Child.Evaluate();
        }
        
        public override Node Clone()
        {
            Decorator node = base.Clone() as Decorator;
            node.Child = Child?.Clone();
            return node;
        }
    }
}