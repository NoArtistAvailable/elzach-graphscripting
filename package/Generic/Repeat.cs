namespace elZach.SimpleAI.BehaviourTree
{
    public class Repeat : Decorator
    {
        // public RepeaterDecorator(TreeDirector director, Node child) : base(director, child)
        // {
        // }

        public bool exitOnFail = false;

        protected override State OnUpdate()
        {
            if (Child.Evaluate() == State.Failure && exitOnFail) 
                return State.Failure;
            return State.Running;
        }
    }
}