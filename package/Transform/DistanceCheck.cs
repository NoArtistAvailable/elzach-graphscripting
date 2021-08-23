using UnityEditor;
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

        public override void OnDrawSelected(SceneView sceneView)
        {
            if (!targetA || !targetB) return;
            //Gizmos.DrawLine(targetA.position, targetB.position);
            Handles.DrawLine(targetA.position,targetB.position);
            var dir = targetA.position - targetB.position;
            // Debug.DrawRay(targetA.position,-dir.normalized*distance*0.5f);
            // Debug.DrawRay(targetB.position,dir.normalized*distance*0.5f);
        }
    }
}