using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace elZach.GraphScripting
{
    //[Serializable]
    [AttributeUsage(AttributeTargets.Field)]
    public class BindingAttribute : PropertyAttribute
    {
        public string bindingName;
    }
}
