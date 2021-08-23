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
        private Coroutine movementRoutine;

        public override Color GetColor() => new Color(0.3f,0.5f,0.4f);

        // public override List<TreeContainer.Parameter> GetPublicParameters()
        // {
        //     return new List<TreeContainer.Parameter>() { new TreeContainer.Parameter(){name = targetTransformBindingName, type = typeof(Transform)} };
        // }

        protected override void OnStart()
        {
            if(target)
                movementRoutine = director.StartCoroutine(DoTheMove());
        }

        protected override State OnUpdate()
        {
            if (!target) return State.Failure;
            if (movementRoutine != null) return State.Running;
            return State.Success;
        }

        IEnumerator DoTheMove()
        {
            float progress = 0f;
            Vector3 startPosition = target.position;
            Vector3 targetPosition = startPosition + toMove;
            while (progress < 1f)
            {
                progress += Time.deltaTime / time;
                target.position = Vector3.Lerp(startPosition, targetPosition, progress);
                yield return null;
            }
            target.position = targetPosition;
            movementRoutine = null;
        }
    }
}
