using UnityEngine;

namespace elZach.GraphScripting.Animation
{
    public class CurveNode : Node
    {
        public AnimationCurve curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
        public float time = 1f;
        private float progress = 0f;

        private float output;

        [AssignPort(isInput = false)]
        public float GetOutput() => output;
        
        protected override void OnStart()
        {
            progress = 0f;
        }

        protected override State OnUpdate()
        {
            progress += director.deltaTime / time;
            output = curve.Evaluate(progress);
            return progress < 1 ? State.Running : State.Success;
        }
    }
}