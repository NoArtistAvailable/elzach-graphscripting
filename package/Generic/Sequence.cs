using System;
using System.Collections.Generic;

namespace elZach.SimpleAI.BehaviourTree
{
    public class Sequence : Composite
    {
        public bool repeat = false;
        private int current = 0;
        private State terminatedState;
        private bool terminated = false;

        public override void Init(TreeDirector director)
        {
            base.Init(director);
            current = 0;
            terminated = false;
        }
        
        protected override State OnUpdate()
        {
            if (terminated) return terminatedState; 
            State childState = Children[current].Evaluate();
            switch (childState)
            {
                case State.Running: return State.Running;
                case State.Success:
                    current++;
                    if (current < Children.Count) return State.Running;
                    if (!repeat) Terminate(State.Success);
                    current = 0;
                    return State.Success;
                case State.Failure:
                    if (!repeat) Terminate(State.Failure);
                    current = 0;
                    return State.Failure;
            }

            return base.Evaluate();
        }

        void Terminate(State state)
        {
            terminatedState = state;
            terminated = true;
        }

        // public SequencerComposite(TreeDirector director, List<Node> children) : base(director, children)
        // {
        // }
    }
}