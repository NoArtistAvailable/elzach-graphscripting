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

        public override void OnUnselected()
        {
            base.OnUnselected();
            onNodeSelected.Invoke(null);
        }

        public void UpdateState()
        {
            if (node.Started && node.state == Node.State.Running)
            {
                if (this.input?.contentContainer != null)
                {
                    this.input.contentContainer.style.backgroundColor = new Color(0.05f,0.6f,0.35f);
                }

                if (this.output?.contentContainer != null)
                {
                    this.output.contentContainer.style.backgroundColor = new Color(0.05f,0.6f,0.35f);
                }
                
            }
            else
            {
                if (this.input?.contentContainer != null)
                {
                    this.input.contentContainer.style.backgroundColor = Color.HSVToRGB(0f, 0f, 0.225f);
                }

                if (this.output != null)
                {
                    this.output.contentContainer.style.backgroundColor = Color.HSVToRGB(0f, 0f, 0.175f);
                }
                this.MarkDirtyRepaint();
            }
        }
    }
}
