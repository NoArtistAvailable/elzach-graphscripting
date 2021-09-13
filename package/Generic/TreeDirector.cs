using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace elZach.GraphScripting
{
    public interface IBinding
    {
        string Name { get; }
        string  Type { get; }
        object Value { get; }
    }
    
    public class TreeDirector : MonoBehaviour
    {
        [Serializable]
        public class Binding : IBinding
        {
            public string name;
            public string type;
            [SerializeField]
            public UnityEngine.Object data;
            public Binding(string name, string type, UnityEngine.Object data = null)
            {
                this.name = name;
                this.type = type;
                this.data = data;
            }

            public Binding(string name, Type type, UnityEngine.Object data = null)
            {
                this.name = name;
                this.type = type.AssemblyQualifiedName;
                this.data = data;
            }

            public string Name => name;
            public string Type => type;
            public object Value => data;
        }
        
        public TreeContainer data;
        public List<Binding> bindings = new List<Binding>();
        public List<Referenced> values = new List<Referenced>();

        [ContextMenu("Set Test")]
        void SetTest()
        {
            bindings = new List<Binding>()
            {
                new Binding("SomeName", typeof(TreeDirector)),
                new Binding("AnotherName", typeof(Transform)),
                new Binding("AnEvent?", typeof(UnityEngine.Events.UnityEvent)),
                new Binding("SomeVector", typeof(Vector3))
            };

            values = new List<Referenced>()
            {
                new Referenced<Vector3>(Vector3.down)
            };
        }

        [ContextMenu("Get Test")]
        void GetTest()
        {
            Debug.Log(values[0].Value);
        }

        private void OnValidate()
        {
            GetBindings();
        }

        // public class MyType
        // {
        //     public float Hello;
        // }

        [ContextMenu("Get Bindings")]
        public void GetBindings()
        {
            // foreach (var val in values)
            // {
            //     val.Value = new MyType(); 
            // }
            
            var parameters = new List<TreeContainer.Parameter>();
            data.rootNode.GetParametersRecursive(ref parameters);
            List<Binding> newBindings = new List<Binding>();
            List<Referenced> newValues = new List<Referenced>();
            //Debug.Log($"Found parameters: {parameters.Count}");
            foreach (var parameter in parameters)
                newBindings.Add(new Binding(parameter.name, parameter.type));


            for (var index = newBindings.Count - 1; index >= 0; index--)
            {
                var binding = newBindings[index];
                var existing = bindings.Find(x => x.name == binding.name && x.type == binding.type);
                if (existing != null)
                {
                    binding.data = existing.data;
                    continue;
                }

                var existingValue = values.Find(x => x.name == binding.name);
                if (existingValue != null)
                {
                    //Debug.Log("FOUND REFERENCE " + existingValue);
                    newValues.Add(existingValue);
                    newBindings.RemoveAt(index);
                    continue;
                }

                var dataType = Type.GetType(binding.type);
                if (dataType != null && dataType.IsGenericType && dataType.BaseType == typeof(Referenced))
                {
                    // var genericBase = typeof(Referenced<>);
                    // var combinedType = genericBase.MakeGenericType(dataType);
                    var instance = (Referenced)Activator.CreateInstance(dataType);
                    instance.name = binding.name;
                    instance.typeName = dataType.GenericTypeArguments.FirstOrDefault()!.AssemblyQualifiedName;
                    instance.Value = binding.Value;
                    Debug.Log(instance.Value);
                    Debug.Log(instance);
                    newValues.Add(instance);
                    newBindings.RemoveAt(index);

                    Debug.Log(((Referenced<Vector3>)instance).Value);
                    instance.OnBeforeSerialize();

                    //values = instance;// Convert.ChangeType(instance, dataType.DeclaringType);
                    // binding.data = new Referenced()
                    //     { typeName = Type.GetType(binding.type).GetGenericTypeDefinition().AssemblyQualifiedName };
                    // var dataType = Type.GetType(binding.type);
                    // var instance = Activator.CreateInstance(dataType,BindingFlags.Public);
                    // binding.data = instance as Object;
                    // var refData = binding.data as Referenced;
                    // var genericType = binding.data.GetType().TypeInitializer;
                    // var actualData = genericType.GetGenericArguments();
                    // binding.data = new Referenced(){}
                }
            }

            bindings = newBindings;
            values = newValues;
        }

        public IEnumerable<IBinding> GetBindingsForRealz()
        {
            foreach(var bind in bindings) yield return bind;
            foreach (var val in values) yield return val;
        }

        private void Start()
        {
            if (!data) return;
            data = data.Clone();
            Play();
        }

        [ContextMenu("Play")]
        void Play()
        {
            data.Init(this);
            StartCoroutine(RunTree());
        }
        
        IEnumerator RunTree()
        {
            while (data.Evaluate() == Node.State.Running) 
                yield return null;
        }
    }
}