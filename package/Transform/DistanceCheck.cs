using UnityEngine;

namespace elZach.GraphScripting
{
    public class DistanceCheck : Decorator
    {
        [Binding] public Transform targetA, targetB;
        public float distance = 1f;
        
        protected override State OnUpdate()
        {
            if (!targetA || !targetB)
            {
                Debug.Log("NULLLLLLLLLL");
                return State.Failure;
            }
            if (Vector3.Distance(targetA.position, targetB.position) > distance)
                return State.Failure;
            return base.OnUpdate();
        }

#if UNITY_EDITOR
        public override void OnDrawSelected()
        {
            if (!targetA || !targetB) return;
            //Gizmos.DrawLine(targetA.position, targetB.position);
            //UnityEditor.Handles.DrawLine(targetA.position,targetB.position);
            var dir = targetA.position - targetB.position;
            Debug.DrawRay(targetA.position,-dir.normalized*distance*0.5f);
            Debug.DrawRay(targetB.position,dir.normalized*distance*0.5f);
        } 
#endif
    }
}