using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

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

        [Serializable]
        public class ExtraConnection
        {
            public Node origin;
            public int indexOfInputAction;
            public int indexOfOutputFunction;
        }

        public void ConnectAdditionalInput(Node origin, int indexOfInputAction, int indexOfOutputFunction)
        {
            var conn = new ExtraConnection()
            {
                origin = origin,
                indexOfInputAction = indexOfInputAction,
                indexOfOutputFunction = indexOfOutputFunction
            };
            var previousConn = extraConnections.Find(x => x.indexOfInputAction == indexOfInputAction);
            if (previousConn != null) extraConnections.Remove(previousConn);
            extraConnections.Add(conn);
        }
        [SerializeField, HideInInspector] public List<ExtraConnection> extraConnections = new List<ExtraConnection>();
        
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

        public List<object> GetAdditionalInputs()
        {
            var additionalInputs = new List<object>();
            var myType = GetType();
            var fields = myType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach(var field in fields)
            {
                var bindingAttribute = field.GetCustomAttribute<AssignPortAttribute>();
                // foreach(var att in bindingAttribute)
                //     if(att is BindingAttribute) Debug.Log(att.);
                if(bindingAttribute == null) continue;
                if (bindingAttribute.isInput)
                {
                    additionalInputs.Add(field.GetValue(this));
                    additionalInputActions.Add((val)=>field.SetValue(this,val));
                }
            }
            return additionalInputs;
        }

        private List<Action<object>> additionalInputActions = new List<Action<object>>();

        public List<Type> GetAdditionalOutputs()
        {
            var additionalOutputs = new List<Type>();
            var myType = GetType();
            var methods = myType.GetMethods( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var meth in methods)
            {
                var bindingAttribute = meth.GetCustomAttribute<AssignPortAttribute>();
                if (bindingAttribute == null) continue;
                if (!bindingAttribute.isInput)
                {
                    Debug.Log($"Found method {meth.Name}, has returnType {meth.ReturnType.Name}");
                    additionalOutputs.Add(meth.ReturnType);
                    //additionalOutputFunctions.Add( (Func<object>) meth.CreateDelegate(typeof(Func<object>),this));
                    additionalOutputFunctions.Add(meth);
                    //TODO: look if we can get this working: https://stackoverflow.com/questions/6430835/example-speeding-up-reflection-api-with-delegate-in-net-c
                    // https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/
                }
            }
            return additionalOutputs;
        }

        //private List<Func<object>> additionalOutputFunctions = new List<Func<object>>();
        private List<MethodInfo> additionalOutputFunctions = new List<MethodInfo>();

        public void GetOutput<T>(int index, out T output) //where T : object
        {
            var result = additionalOutputFunctions[index].Invoke(this, null); //additionalOutputFunctions[index](); //
            output = (T) result;
        }

        public object GetOutput(int index)
        {
            return additionalOutputFunctions[index].Invoke(this, null);
        }
        
        public virtual void Init()
        {
            if (!director) return;
            foreach (var serPar in parameters)
            {
                //Debug.Log("init " + serPar.path);
                var param = serPar.Parameter;
                foreach (var bind in director.GetBindingsForRealz())
                {
                    if (bind.Name == param.name && param.type.AssemblyQualifiedName == bind.Type)
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
                                    field.SetValue(this, bind.Value);
                                }
                            }
                            parentType = parentType.BaseType;
                        }
                    }
                }
            }
            GetAdditionalInputs();
            GetAdditionalOutputs();
            //this.director = director;
        }

        protected virtual void OnStart(){}
        protected virtual void OnStop(){}

        protected abstract State OnUpdate();
        
        public State Evaluate()
        {
            foreach (var connection in extraConnections)
            {
                Debug.Log($"Trying to get {connection.indexOfOutputFunction} [{connection.origin.additionalOutputFunctions.Count}] from {connection.origin.name} " +
                          $"to {connection.indexOfInputAction} [{additionalInputActions.Count}] ");
                if (connection.origin.additionalOutputFunctions.Count <= connection.indexOfOutputFunction)
                    connection.origin.GetAdditionalOutputs();
                additionalInputActions[connection.indexOfInputAction].Invoke(connection.origin.GetOutput(connection.indexOfOutputFunction));
            }
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
            
            //TODO: handle cloned data in extra connections
            
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