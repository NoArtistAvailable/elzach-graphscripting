using UnityEditor;
using UnityEngine;

namespace elZach.GraphScripting
{
    [CustomPropertyDrawer(typeof(Referenced<>))]
    public class ReferencedDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // var val = property.FindPropertyRelative("Value");
            // if (val == null)
            // {
            //     EditorGUI.LabelField(position, new GUIContent($"{property.propertyType}"));
            //     return;
            // }
            // var typeName = val.type;
            EditorGUI.LabelField(position, label);
        }
    }
}