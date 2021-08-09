using System.Collections.Generic;

namespace elZach.SimpleAI.BehaviourTree
{
    public class Parallel : Composite
    {
        protected override State OnUpdate()
        {
            bool running = false;
            bool success = true;
            foreach (var child in Children)
            {
                State childState = child.Evaluate();
                switch (childState)
                {
                    case State.Running: running |= true;
                        break;
                    case State.Failure: success &= false;
                        break;
                }
            }

            if (running) return State.Running;
            if (success) return State.Success; //only returns if none are running and none are failures
            return State.Failure; //returns if none are running and any have failed
        }

        // public ParallelComposite(TreeDirector director, List<Node> children) : base(director, children)
        // {
        // }
    }
}