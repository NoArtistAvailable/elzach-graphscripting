using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace elZach.GraphScripting
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Action<NodeView> onNodeSelected;
        public Node node;
        public UnityEditor.Experimental.GraphView.Port input, output;

        public NodeView(Node node)
        {
            this.node = node;
            this.title = node.name;
            this.viewDataKey = node.guid;

            this.titleContainer.style.backgroundColor = node.GetColor();
            style.left = node.position.x;
            style.top = node.position.y;

            CreateInputPorts();
            CreateOutputPorts();

        }

        private void CreateInputPorts()
        {
            if (node is Root) return;
            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Node));
            if (input != null)
            {
                input.portName = "";
                inputContainer.Add(input);
            }
        }
        private void CreateOutputPorts()
        {
            if (node is Decorator)
            {
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Node));
            }else if (node is Composite)
            {
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(Node));
            }else if (node is Root)
            {
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Node));
            }
            
            if (output != null)
            {
                output.portName = "";
                outputContainer.Add(output);
            }
        }

        public override void SetPosition(Rect newPos)
        {
            Undo.RecordObject(node, "Behaviourtree SetPosition");
            base.SetPosition(newPos);
            
            node.position.x = newPos.x;
            node.position.y = newPos.y;
            EditorUtility.SetDirty(node);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            if(onNodeSelected!=null) onNodeSelected.Invoke(this);
        }

        public void UpdateState()
        {
            // RemoveFromClassList("running");
            // RemoveFromClassList("succeeded");
            // RemoveFromClassList("failed");
            // if(Application.isPlaying && node.tr)
            //     switch (node.state)
            //     {
            //         case Node.State.Running:
            //             if(node.Started)
            //                 AddToClassList("running");
            //             break;
            //         case Node.State.Success:
            //             AddToClassList("succeeded");
            //             break;
            //         case Node.State.Failure:
            //             AddToClassList("failed");
            //             break;
            //     }
            // Debug.Log("!");
            //
            // var parentEdge = input.connections?.First();
            // if (parentEdge != null)
            // {
            //     Debug.Log(parentEdge.defaultColor +" -> " + Color.green);
            //     parentEdge.style.color = Color.green;
            //     
            //     parentEdge.MarkDirtyRepaint();
            // }
            //
            // var childEdge = output.connections?.First();
            // if (childEdge != null)
            // {
            //     childEdge.style.color = Color.red;
            //     Debug.Log(Color.red);
            //     childEdge.MarkDirtyRepaint();
            // }
            if (node.Started && node.state == Node.State.Running)
            {
                this.titleContainer.style.backgroundColor = new Color(0.05f,0.6f,0.35f);
                if (this.input != null)
                {
                    this.input.portColor = new Color(0.05f,0.6f,0.35f);
                    foreach (var connection in this.input.connections)
                    {
                        connection.edgeControl.MarkDirtyRepaint();
                    }
                }

                if (this.output != null)
                {
                    this.output.portColor = new Color(0.6f,0.05f,0.35f);
                    foreach (var connection in this.output.connections)
                    {
                        connection.edgeControl.MarkDirtyRepaint();
                    }
                }
                
            }
            else
            {
                this.titleContainer.style.backgroundColor = node.GetColor();
                if (this.input != null)
                {
                    this.input.portColor = Color.gray;
                    foreach (var connection in this.input.connections)
                    {
                        connection.edgeControl.MarkDirtyRepaint();
                    }
                    //this.input.connections?.MarkDirtyRepaint();
                }

                if (this.output != null)
                {
                    this.output.portColor = Color.gray;
                    foreach (var connection in this.output.connections)
                    {
                        connection.edgeControl.MarkDirtyRepaint();
                    }
                }
                this.MarkDirtyRepaint();
            }
        }
    }
}
