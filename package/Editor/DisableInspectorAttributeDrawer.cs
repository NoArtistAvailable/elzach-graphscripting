using UnityEditor;
using UnityEngine;

namespace elZach.GraphScripting
{
    [CustomPropertyDrawer(typeof(DisableInspectorAttribute))]
    public class DisableInspectorAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.EndDisabledGroup();
        }
    }
}