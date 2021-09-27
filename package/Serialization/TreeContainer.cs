using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;

namespace elZach.GraphScripting
{
    [CreateAssetMenu(menuName = "Graph/TreeContainer")]
    public class TreeContainer : ScriptableObject
    {
        [Serializable]
        public class Parameter
        {
            public string name;
            private Type _type;
            public Type type
            {
                get
                {
                    if (_type == null) _type = Type.GetType(typeName);
                    return _type;
                }
                set => typeName = value.AssemblyQualifiedName;
            }

            [SerializeField, HideInInspector] private string typeName;
            
            //public object data;
        }
        
        public Node.State state = Node.State.Running;
        [HideInInspector] public TreeDirector director;
        public Node rootNode;
        public List<Node> nodes = new List<Node>();
        [NonSerialized] public int currentEvaluation;
        public List<Parameter> Parameters => GetExposedParameters();

        public T GetFromDirector<T>(string name) where T:UnityEngine.Object
        {
            var data = director.bindings.Find(x => x.name == name)?.data;
            if (data && data is T) return data as T;
            return null;
        }

        private List<Parameter> GetExposedParameters()
        {
            var parameters = new List<Parameter>();
            foreach (var node in nodes)
            {
                parameters.AddRange(node.GetPublicParameters());
            }
            return parameters;
        }

        public void Init(TreeDirector director)
        {
            this.director = director;
            currentEvaluation = -1;
            foreach (var node in nodes)
            {
                //Debug.Log($"Initiating {node.name}.");
                node.Init();
            }
            //ForeachExtraConnection();
        }
        
        public Node.State Evaluate()
        {
            currentEvaluation++;
            return rootNode.Evaluate();
        }

        public Node CreateNode(System.Type type)
        {
            Undo.RecordObject(this, nameof(TreeContainer) + " " + nameof(CreateNode));
            
            Node node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();
            node.container = this;
            nodes.Add(node);
            if(!Application.isPlaying)
                AssetDatabase.AddObjectToAsset(node, this);
            
            Undo.RegisterCreatedObjectUndo(node, nameof(TreeContainer) + " " + nameof(CreateNode));
            AssetDatabase.SaveAssets();
            return node;
        }

        public void DeleteNode(Node node)
        {
            Undo.RecordObject(this, nameof(TreeContainer) + " " + nameof(DeleteNode));
            nodes.Remove(node);
            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(Node parent, Node child)
        {
            Undo.RecordObject(parent, nameof(TreeContainer) + " " + nameof(AddChild));
            if (parent is Decorator deco) deco.Child = child;
            else if (parent is Composite compo) compo.Children.Add(child);
            else if (parent is Root root) root.Child = child;
            EditorUtility.SetDirty(parent);
        }

        public void RemoveChild(Node parent, Node child)
        {
            Undo.RecordObject(parent, nameof(TreeContainer) + " " + nameof(RemoveChild));
            if (parent is Decorator deco) deco.Child = null;
            else if (parent is Composite compo) compo.Children.Remove(child);
            else if (parent is Root root) root.Child = null;
            EditorUtility.SetDirty(parent);
        }

        public List<Node> GetChildren(Node parent)
        {
            if (parent is Decorator deco && deco.Child != null) return new List<Node>(){ deco.Child};
            if (parent is Root root && root.Child != null) return new List<Node>(){ root.Child};
            if (parent is Composite compo) return compo.Children;
            return new List<Node>();
        }

        public TreeContainer Clone()
        {
            var tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            tree.nodes = new List<Node>();
            ForeachNode(tree.rootNode, (node) =>
            {
                if (!tree.nodes.Contains(node))
                {
                    tree.nodes.Add(node);
                    node.container = tree;
                    ForeachExtraConnection(node, (connectionNode) =>
                    {
                        foreach (var conn in connectionNode.extraConnections)
                        {
                            if (!tree.nodes.Any(x => x.guid == conn.origin.guid))
                            {
                                var connectedNode = conn.origin.Clone();
                                conn.origin = connectedNode;
                                tree.nodes.Add(connectedNode);
                            }
                            else
                            {
                                conn.origin = tree.nodes.Find(x => x.guid == conn.origin.guid);
                            }
                        }
                    });
                }
            });
            
            return tree;
        }

        public void ForeachNode(Node node, Action<Node> action)
        {
            if (node) action.Invoke(node);
            foreach (var child in GetChildren(node))
                ForeachNode(child, action);
        }

        public void ForeachExtraConnection(Node node, Action<Node> action)
        {
            if (node) action.Invoke(node);
            foreach(var conn in node.extraConnections)
                ForeachExtraConnection(conn.origin, action);
        }
        
        // public List<SerializedNode> nodes = new List<SerializedNode>()
        // {
        //     new SerializedNode() {index = 0, nodeType = typeof(SelectorComposite).Namespace +"."+ nameof(SelectorComposite)} // rootnode
        // };
        //
        // public Dictionary<string, Type> propertyFields = new Dictionary<string, Type>();
    }

    // [System.Serializable]
    // public class SerializedNode
    // {
    //     public int index;
    //     public string nodeType;
    //     public Type NodeType => Type.GetType(nodeType);
    //     public List<int> children = new List<int>();
    //     public List<ISerializable> data = new List<ISerializable>(); // needs to be serializable
    //     //public List<byte[]> serializedData = new List<byte[]>();
    // }
}