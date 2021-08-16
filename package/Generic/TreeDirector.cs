using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
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
        public Dictionary<string, ISerializable> Bindings = new Dictionary<string, ISerializable>();

        public List<Binding> test = new List<Binding>();

        [ContextMenu("Set Test")]
        void SetTest()
        {
            test = new List<Binding>()
            {
                new Binding("SomeName", typeof(TreeDirector)),
                new Binding("AnotherName", typeof(Transform)),
                new Binding("AnEvent?", typeof(UnityEngine.Events.UnityEvent))
            };
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