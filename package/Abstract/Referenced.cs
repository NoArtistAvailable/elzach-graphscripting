using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace elZach.GraphScripting
{
    [Serializable]
    public class Referenced : ISerializationCallbackReceiver, IBinding
    {
        public string name;
        public string typeName;
        public string valueString;
        private object cached;

        public string Name => name;
        public string Type => typeName;

        public object Value
        {
            get
            {
                if (string.IsNullOrEmpty(typeName)) throw new Exception("typename is null");
                if (string.IsNullOrWhiteSpace(valueString)) return default;
                if (cached == null) cached = JsonUtility.FromJson(valueString, System.Type.GetType(typeName));// Convert.ChangeType(valueString, Type.GetType(typeName));
                return cached;
            }
            set
            {
                cached = value;
                if (value != null && string.IsNullOrEmpty(valueString))
                    valueString = JsonUtility.ToJson(value);
                else valueString = null;
                /*if (value != null)
                    valueString = value.ToString();
                else valueString = null;
                */
            }
        }

        public override string ToString()
        {
            return Name + " - " + Type + " - " + valueString;
        }

        T GetValueAs<T>() where T : struct
        {
            return (T) Convert.ChangeType(valueString, typeof(T));
        }

        public void OnBeforeSerialize()
        {
            if (cached != null || !string.IsNullOrEmpty(typeName))
            {
                var val = Value;
                if(val != null)
                    typeName = val.GetType().AssemblyQualifiedName;
            }

            if (!string.IsNullOrEmpty(typeName))
            {
                var val = Value ?? GetDefaultValue(System.Type.GetType(typeName));
                valueString = JsonUtility.ToJson(val) ?? string.Empty;
            }
        }
        
        public static object GetDefaultValue(Type type)
        {
            // Validate parameters.
            if (type == null) throw new ArgumentNullException("type");

            // We want an Func<object> which returns the default.
            // Create that expression here.
            var e = Expression.Lambda<Func<object>>(
                // Have to convert to object.
                Expression.Convert(
                    // The default value, always get what the *code* tells us.
                    Expression.Default(type), typeof(object)
                )
            );

            // Compile and return the value.
            return e.Compile()();
        }

        public void OnAfterDeserialize()
        {
        }
    }
    
    [Serializable]
    public class Referenced<T> : Referenced where T : struct
    {
        public new T Value
        {
            get
            {
                var val = base.Value;
                if (val == null) return default;
                return (T)val;
            }
            set => base.Value = value;
        }

        public Referenced()
        {
        }

        public Referenced(T value)
        {
            typeName = typeof(T).AssemblyQualifiedName;
            Value = value;
            //valueString = value.ToString();
        }
        
    }
}