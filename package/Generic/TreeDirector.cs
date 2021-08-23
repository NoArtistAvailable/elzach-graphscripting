using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;

namespace elZach.GraphScripting
{
    public class TreeDirector : MonoBehaviour
    {
        [Serializable]
        public class Binding
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
        }
        
        public TreeContainer data;
        public List<Binding> bindings = new List<Binding>();

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
        }

        private void OnValidate()
        {
            GetBindings();
        }

        [ContextMenu("Get Bindings")]
        public void GetBindings()
        {
            var parameters = new List<TreeContainer.Parameter>();
            data.rootNode.GetParametersRecursive(ref parameters);
            List<Binding> newBindings = new List<Binding>();
            //Debug.Log($"Found parameters: {parameters.Count}");
            foreach (var parameter in parameters)
                newBindings.Add(new Binding(parameter.name, parameter.type));

            foreach (var binding in newBindings)
            {
                var existing = bindings.Find(x => x.name == binding.name && x.type == binding.type);
                if (existing != null) binding.data = existing.data;
            }

            bindings = newBindings;
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