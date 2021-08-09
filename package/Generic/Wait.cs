using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace elZach.SimpleAI.BehaviourTree
{
    public class Wait : Node
    {
        public float waitTime;
        private float waitUntil = -1;

        public override Color GetColor() => new Color(0.15f, 0.15f, 0.25f);

        protected override State OnUpdate()
        {
            if (waitUntil < 0) waitUntil = Time.time + waitTime;
            if (waitUntil > Time.time) return State.Running;

            waitUntil = -1;
            return State.Success;
        }
    }
}