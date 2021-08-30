using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace elZach.GraphScripting
{
    public abstract class Node : ScriptableObject, ISerializationCallbackReceiver
    {
        [Serializable]
        public class SerializableParameter
        {
            public TreeContainer.Parameter Parameter;
            public string path;

            public SerializableParameter(string name, Type type, string path)
            {
                Parameter = new TreeContainer.Parameter() { name = name, type = type };
                this.path = path;
            }
            
        }
        public enum State{Running, Failure, Success}

        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        [NonSerialized] public State state;
        [NonSerialized] public int lastEvaluation;
        [HideIfNotNull] public TreeContainer container;

        [HideInInspector] 
        public List<SerializableParameter> parameters = new List<SerializableParameter>();
        public TreeDirector director => container?.director;
        public virtual Color GetColor() => new Color(0.25f,0.25f,0.25f);
        private bool started = false;
        public bool Started => started;

        // private void OnValidate()
        // {
        //     Debug.Log($"Validating {name}.",this);
        // }

        internal virtual void GetParametersRecursive(ref List<TreeContainer.Parameter> parameters)
        {
            foreach (var parameter in GetPublicParameters())
            {
                if(!parameters.Exists(x=>x.name == parameter.name)) parameters.Add(parameter);
            }
        }
        
        public List<TreeContainer.Parameter> GetPublicParameters()
        {
            var list = new List<TreeContainer.Parameter>();
            foreach (var param in parameters)
            {
                list.Add(param.Parameter);
            }

            return list;
        }
        
        public virtual void Init()
        {
            if (!director) return;
            foreach (var serPar in parameters)
            {
                //Debug.Log("init " + serPar.path);
                var param = serPar.Parameter;
                foreach (var bind in director.bindings)
                {
                    if (bind.name == param.name && param.type.AssemblyQualifiedName == bind.type)
                    {
                        var myType = GetType();
                        var parentType = myType;
                        var currentPath = string.Empty;
                        var lastSeparatorIndex = serPar.path.LastIndexOf("/", StringComparison.Ordinal);
                        var searchPath =
                            serPar.path.Substring(0, lastSeparatorIndex);
                        var fieldName = serPar.path.Substring(lastSeparatorIndex + 1);
                        while (parentType != null && parentType != typeof(Node))
                        {
                            currentPath = "/" + parentType.Name + currentPath;
                            if (currentPath == searchPath)
                            {
                                var field = myType.GetField(fieldName,
                                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                                if (field != null)
                                {
                                    //Debug.Log("Set fields");
                                    field.SetValue(this, bind.data);
                                }
                            }
                            parentType = parentType.BaseType;
                        }
                    }
                }
            }
            //this.director = director;
        }

        protected virtual void OnStart(){}
        protected virtual void OnStop(){}

        protected abstract State OnUpdate();
        
        public State Evaluate()
        {
            lastEvaluation = container.currentEvaluation;
            if (!started)
            {
                started = true;
                OnStart();
            }
            state = OnUpdate();
            if (state == State.Failure || state == State.Success)
            {
                started = false;
                OnStop();
            }
            return state;
        }

        public virtual Node Clone()
        {
            Node clone = Instantiate(this);
            clone.name = this.name;
            return clone;
        }

        #if UNITY_EDITOR
        public virtual void OnDrawSelected()
        {
            //Debug.Log(name);
        }
        #endif

        public void OnBeforeSerialize()
        {
            //Debug.Log($"(Before)Serializing {name}",this);
            var shouldBeParameters = new List<SerializableParameter>();
            var myType = GetType();
            var currentPath = string.Empty;
            while (myType != null && myType != typeof(Node))
            {
                currentPath += "/" + myType.Name;
                foreach (var shouldBeParameter in shouldBeParameters)
                    shouldBeParameter.path = "/" + myType.Name + shouldBeParameter.path;
                var fields = myType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                foreach(var field in fields)
                {
                    var bindingAttribute = field.GetCustomAttribute<BindingAttribute>();
                    // foreach(var att in bindingAttribute)
                    //     if(att is BindingAttribute) Debug.Log(att.);
                    if(bindingAttribute == null) continue;
                    var fieldPath = currentPath + "/" + field.Name;
                    shouldBeParameters.Add(new SerializableParameter(bindingAttribute.bindingName, field.FieldType, fieldPath));
                }
                myType = myType.BaseType;
            }

            foreach (var parameter in shouldBeParameters)
            {
                var existing = parameters.Find(x => x.path == parameter.path);
                if (existing != null) parameter.Parameter = existing.Parameter;
            }

            parameters = shouldBeParameters;
        } 

        public void OnAfterDeserialize()
        {
        }

        public bool TryGetSerializedParameter(string memberName, out SerializableParameter parameter)
        {
            if (parameters == null)
            {
                parameter = null;
                return false;
            }
            var myType = GetType();
            var currentPath = string.Empty;
            while (myType != null && myType != typeof(Node))
            {
                currentPath = "/" + myType.Name + currentPath;
                myType = myType.BaseType;
            }
            currentPath += "/" + memberName;
            foreach (var param in parameters)
            {
                if (param.path == currentPath)
                {
                    parameter = param;
                    return true;
                }
            }
            parameter = null;
            return false;
        }
    }
}