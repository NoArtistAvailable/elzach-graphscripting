﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

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
        
        // void Start()
        // {
        //     CreateRuntimeTree();
        //     (tree[2] as LogDecorator).message = "Hollo";
        //     StartCoroutine(RunTree());
        // }
        //
        // IEnumerator RunTree()
        // {
        //     while (tree[0].Evaluate() == Node.State.Running) 
        //         yield return null;
        // }
        //
        // public void CreateRuntimeTree()
        // {
        //     tree = new List<Node>();
        //     foreach (var serialized in data.nodes)
        //     {
        //         var type = serialized.NodeType;
        //         //Debug.Log(serialized.nodeType);// + ":" + type.FullName);
        //         var node = System.Activator.CreateInstance(type) as Node;
        //         
        //         tree.Add(node);
        //     }
        //
        //     for (int i = 0; i < tree.Count; i++)
        //     {
        //         var node = tree[i];
        //         if (node is Decorator asDecorator)
        //         {
        //             if (data.nodes[i].children.Count > 0)
        //                 asDecorator.Child = tree[data.nodes[i].children[0]];
        //         }
        //         else if (node is Composite asComposite)
        //         {
        //             var children = new List<Node>();
        //             foreach (var childIndex in data.nodes[i].children)
        //                 children.Add(tree[childIndex]);
        //             asComposite.Children = children;
        //         }
        //     }
        // }
    }
}