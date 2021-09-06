using System;
using System.Reflection;
using UnityEngine;

namespace elZach.GraphScripting
{
    [Serializable]
    public class Referenced : UnityEngine.Object
    {
        public string typeName;
        public string valueString;
        
        T GetValueAs<T>() where T : struct
        {
            return (T) Convert.ChangeType(valueString, typeof(T));
        }
        
    }
    
    [Serializable]
    public class Referenced<T> : Referenced where T : struct
    {
        private T? cached;

        public T Value
        {
            get
            {
                if (cached == null) cached = GetValueFromString();
                return cached.Value;
            }
            set
            {
                cached = value;
                valueString = value.ToString();
            }
        }
        public Referenced(T value)
        {
            typeName = typeof(T).AssemblyQualifiedName;
            Value = value;
            //valueString = value.ToString();
        }

        T GetValueFromString()
        {
            return (T) Convert.ChangeType(valueString, typeof(T));
        }
        
    }
}