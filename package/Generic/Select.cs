using System.Collections.Generic;

namespace elZach.GraphScripting
{
    public class Select : Composite
    {
        protected override State OnUpdate()
        {
            State returnState = State.Failure;
            foreach (var child in Children)
            {
                State childState = child.Evaluate();
                switch (childState)
                {
                    case State.Running: return State.Running;
                    case State.Success:
                    {
                        returnState = State.Success;
                        break;
                    }
                }
            }
            return returnState;
        }

        // public SelectorComposite(TreeDirector director, List<Node> children) : base(director, children)
        // {
        // }
    }
}