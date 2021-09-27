using System.Reflection;
using UnityEngine;

namespace elZach.GraphScripting
{
    public class LerpVector : ValueProvider
    {
        [AssignPort]
        public float time;

        public Vector3 startValue;
        public Vector3 targetValue;

        [AssignPort(isInput = false)]
        public Vector3 Execute()
        {
            Evaluate();
            return Vector3.Lerp(startValue, targetValue, time);
        }
    }
}