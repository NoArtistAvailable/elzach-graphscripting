using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace elZach.GraphScripting
{
    public class TransformNode : Node
    {
        public string targetTransformBindingName;
        public Transform Target => director.test.Find(x => x.name == targetTransformBindingName)?.data as Transform;
        public Vector3 toMove;
        public AnimationCurve movementCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
        public float time = 0.5f;
        private Coroutine movementRoutine;

        protected override void OnStart()
        {
            if(Target)
                movementRoutine = director.StartCoroutine(DoTheMove());
        }

        protected override State OnUpdate()
        {
            if (!Target) return State.Failure;
            if (movementRoutine != null) return State.Running;
            return State.Success;
        }

        IEnumerator DoTheMove()
        {
            float progress = 0f;
            Vector3 startPosition = Target.position;
            Vector3 targetPosition = startPosition + toMove;
            while (progress < 1f)
            {
                progress += Time.deltaTime / time;
                Target.position = Vector3.Lerp(startPosition, targetPosition, progress);
                yield return null;
            }
            Target.position = targetPosition;
            movementRoutine = null;
        }
    }
}
