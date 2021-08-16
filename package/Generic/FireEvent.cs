using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace elZach.GraphScripting
{
    public class FireEvent : Node
    {
        public override Color GetColor() => new Color(0.6f, 0.6f, 0.05f);

        public UnityEvent OnTriggered;
        protected override State OnUpdate()
        {
            OnTriggered.Invoke();
            return State.Success;
        }
    }
}