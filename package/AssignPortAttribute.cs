using System;
using UnityEngine;

namespace elZach.GraphScripting
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AssignPortAttribute : PropertyAttribute
    {
        public bool isInput = true;
    }
}