using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace elZach.GraphScripting
{
    public class Wait : Node
    {
        public float waitTime;
        private float waitUntil = -1;
        public bool expose;

        public override List<TreeContainer.Parameter> GetPublicParameters()
        {
            var parameters = new List<TreeContainer.Parameter>();
            if (expose) parameters.Add(new TreeContainer.Parameter(){name="WaitTime",type = typeof(float)});
            return parameters;
        }

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