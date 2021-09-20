using System;
using System.Collections.Generic;
using System.Linq;
using elZach.GraphScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Node = elZach.GraphScripting.Node;

// namespace elZach.SimpleAI.BehaviourTree
// {
    public class BehaviourTreeView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits>
        {
        }

        private TreeContainer tree;
        public Action<NodeView> onNodeSelected;

        public BehaviourTreeView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    "Packages/com.elzach.graphscripting/Editor/BehaviourTreeEditorWindow.uss");
            styleSheets.Add(styleSheet);

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnUndoRedo()
        {
            PopulateView(tree);
            AssetDatabase.Refresh();
        }

        public void PopulateView(TreeContainer treeContainer)
        {
            this.tree = treeContainer;
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;

            if (tree.rootNode == null)
            {
                tree.rootNode = tree.CreateNode(typeof(Root)) as Root;
                EditorUtility.SetDirty(tree);
                AssetDatabase.SaveAssets();
            }
            
            //nodeviews
            tree.nodes.ForEach(CreateNodeView);
            
            //edges
            tree.nodes.ForEach(n =>
            {
                var parentView = GetNodeView(n);
                foreach (var child in tree.GetChildren(n))
                {
                    var childView = GetNodeView(child);
                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                    if (child.extraConnections.Count > 0)
                    {
                        foreach (var extraConnection in child.extraConnections)
                        {
                            Edge extraEdge = GetNodeView(extraConnection.origin)
                                .additionalOutputPorts[extraConnection.indexOfOutputFunction]
                                .ConnectTo(childView.additionalInputPorts[extraConnection.indexOfInputAction]);
                            AddElement(extraEdge);
                        }
                    }
                }
            });
        }

        NodeView GetNodeView(Node node)
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }
        
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphviewchange)
        {
            if (graphviewchange.elementsToRemove != null)
            {
                graphviewchange.elementsToRemove.ForEach(elem =>
                {
                    if (elem is NodeView isNodeView)
                    {
                        tree.DeleteNode(isNodeView.node);
                    }

                    if (elem is Edge isEdge)
                    {
                        NodeView parentView = isEdge.output.node as NodeView;
                        NodeView childView = isEdge.input.node as NodeView;
                        tree.RemoveChild(parentView.node, childView.node);
                    }

                });
            }
            
            if (graphviewchange.edgesToCreate != null)
            {
                graphviewchange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    if (edge.output.portType == typeof(Node.State))
                    {
                        tree.AddChild(parentView.node, childView.node);
                    }
                    else
                    {
                        childView.node.ConnectAdditionalInput(parentView.node, 
                            (int)edge.input.userData,
                            (int)edge.output.userData);
                    }
                });
            }
            
            return graphviewchange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            var types = TypeCache.GetTypesDerivedFrom<Node>();
            foreach (var type in types)
            {
                if(!type.IsAbstract)
                    evt.menu.AppendAction($"Create/[{type.BaseType.Name}]/{type.Name}", (menu) => CreateNodeFromType(type));
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            //TODO: types of inputs and outputs have to align, because we now can't expect everything to be of the same type
            return ports.ToList().Where(x => x.direction != startPort.direction
            && x.node != startPort.node && x.portType == startPort.portType).ToList();
        }

        private void CreateNodeFromType(System.Type type)
        {
            Node node = tree.CreateNode(type);
            CreateNodeView(node);
        }

        void CreateNodeView(Node node)
        {
            NodeView nodeView = new NodeView(node);
            nodeView.onNodeSelected = onNodeSelected;
            AddElement(nodeView);
        }

        public void UpdateNodeViewStates()
        {
            nodes.ForEach((node) =>
            {
                (node as NodeView)?.UpdateState();
                //UpdateNodeViewEdges(node as NodeView);
            });
        }

        void UpdateNodeViewEdges(NodeView node)
        {
            
        }
    }
// }