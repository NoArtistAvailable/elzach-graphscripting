using System.Collections;
using System.Collections.Generic;
using elZach.GraphScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

    private Editor editor;
    
    
    public InspectorView()
    {
        
    }

    public void UpdateSelection(NodeView nodeView)
    {
        Clear();
        if (nodeView == null) return;
        if(editor != null) UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(nodeView.node);
        IMGUIContainer container = new IMGUIContainer(()=>
        {
            if (editor.target != null)
            {
                editor.OnInspectorGUI();
            }
        });
        Add(container);
    }
}
