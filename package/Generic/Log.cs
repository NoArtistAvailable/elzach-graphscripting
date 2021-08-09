using UnityEngine;

namespace elZach.SimpleAI.BehaviourTree
{
    public class Log : Decorator
    {
        public enum Condition{Everytime, Never, OnExit, OnSuccess, OnFailure}
        public string message;
        public Condition condition;
        public override Color GetColor() => Color.HSVToRGB(0.7f,0.3f,0.65f);
        
        // public LogDecorator(TreeDirector director, Node child) : base(director, child)
        // {
        // }

        protected override State OnUpdate()
        {
            if (Child == null)
            {
                Debug.Log(message);
                return State.Success;
            }
            
            
            State childState = base.OnUpdate();
            switch (condition)
            {
                case Condition.Everytime: ShowLog(childState);
                    break;
                case Condition.OnExit: if(childState == State.Failure || childState == State.Success) ShowLog(childState);
                    break;
                case Condition.OnSuccess: if(childState == State.Success) ShowLog(childState);
                    break;
                case Condition.OnFailure: if(childState == State.Failure) ShowLog(childState);
                    break;
            }
            
            return childState;
        }

        void ShowLog(State childState)
        {
            Debug.Log(message + " : " + childState);
        }
    }
}