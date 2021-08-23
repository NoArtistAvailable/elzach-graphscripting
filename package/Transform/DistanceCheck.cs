using UnityEngine;

namespace elZach.GraphScripting
{
    public class DistanceCheck : Node
    {
        [Binding] public Transform targetA, targetB;
        public float distance = 1f;
        
        protected override State OnUpdate()
        {
            if (!targetA || !targetB) return State.Failure;
            if (Vector3.Distance(targetA.position, targetB.position) > distance) return State.Running;
            return State.Success;
        }
    }
}