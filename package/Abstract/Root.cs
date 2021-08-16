using UnityEngine;

namespace elZach.GraphScripting
{
    public class Root : Node
    {
        public Node Child;
        public override Color GetColor() => new Color(0.4f, 0.2f, 0.2f);

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
            Root node = base.Clone() as Root;
            node.Child = Child?.Clone();
            return node;
        }
    }
}