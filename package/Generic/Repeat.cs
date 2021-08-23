using System;

namespace elZach.GraphScripting
{
    public class Repeat : Decorator
    {
        // public RepeaterDecorator(TreeDirector director, Node child) : base(director, child)
        // {
        // }

        public int cycles = 0;
        [DisableInspector] public int cycle = 0;
        public bool exitOnFail = false;

        protected override void OnStart()
        {
            cycle = 0;
        }

        protected override State OnUpdate()
        {
            var childState = Child.Evaluate();
            if (childState == State.Failure && exitOnFail)
            {
                cycle = 0;
                return State.Failure;
            }

            if (cycles > 0 && childState != State.Running)
            {
                cycle++;
                if (cycle >= cycles)return State.Success;
            }
            return State.Running;
        }
    }
}