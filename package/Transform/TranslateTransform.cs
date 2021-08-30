using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace elZach.GraphScripting
{
    public class TranslateTransform : Node
    {
        // public string targetTransformBindingName;
        // public Transform Target => director.bindings.Find(x => x.name == targetTransformBindingName)?.data as Transform;

        [Binding]
        public Transform target;
        
        public Vector3 toMove;
        public AnimationCurve movementCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
        public float time = 0.5f;
        
        //private Coroutine movementRoutine;

        private float progress;
        private Vector3 startPosition;
        private Vector3 targetPosition;

        public override Color GetColor() => new Color(0.3f,0.5f,0.4f);
        protected override void OnStart()
        {
            if (!target) return;
            progress = 0f;
            startPosition = target.position;
            targetPosition = startPosition + toMove;

        }

        protected override State OnUpdate()
        {
            if (!target) return State.Failure;
            //if (movementRoutine != null) return State.Running;
            if (progress < 1f)
            {
                progress += Time.deltaTime / time; //<- should be evaluation delta Time
                target.position = Vector3.Lerp(startPosition, targetPosition, progress);
                return State.Running;
            }

            target.position = targetPosition;
            return State.Success;
        }

        // IEnumerator DoTheMove()
        // {
        //     float progress = 0f;
        //     Vector3 startPosition = target.position;
        //     Vector3 targetPosition = startPosition + toMove;
        //     while (progress < 1f)
        //     {
        //         progress += Time.deltaTime / time;
        //         target.position = Vector3.Lerp(startPosition, targetPosition, progress);
        //         yield return null;
        //     }
        //     target.position = targetPosition;
        //     movementRoutine = null;
        // }
    }
}
