using UnityEngine;

namespace elZach.GraphScripting
{
    public class ValueProvider : Node
    {
        protected override State OnUpdate()
        {
            //Debug.LogWarning("This Node shouldn't Update");
            //return State.Failure;
            return State.Running;
        }
    }
}