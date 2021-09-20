using System;
using UnityEngine;

namespace elZach.GraphScripting
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class AssignPortAttribute : PropertyAttribute
    {
        public bool isInput = true;
    }
}