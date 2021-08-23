using System.Collections.Generic;

namespace elZach.GraphScripting
{
    public abstract class Decorator : Node
    {
        public Node Child;
        
        internal override void GetParametersRecursive(ref List<TreeContainer.Parameter> parameters)
        {
            base.GetParametersRecursive(ref parameters);
            if(Child) Child.GetParametersRecursive(ref parameters);
        }
        
        public override void Init()
        {
            base.Init();
            if(Child) Child.Init();
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