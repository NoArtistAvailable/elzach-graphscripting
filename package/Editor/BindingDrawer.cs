using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace elZach.GraphScripting
{
    [CustomPropertyDrawer(typeof(TreeDirector.Binding))]
    public class BindingDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var typeName = property.FindPropertyRelative("type").stringValue;
            var dataProperty = property.FindPropertyRelative("data");
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            DrawAsObjectField(position, dataProperty, Type.GetType(typeName));//fieldInfo.FieldType);
            EditorGUI.EndProperty();
        }

        public static void DrawAsObjectField(Rect position, SerializedProperty property, Type type)
        {
            if (type != null && type.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                property.objectReferenceValue =
                    EditorGUI.ObjectField(position, property.objectReferenceValue, type, true);
            }
            else EditorGUI.LabelField(position, $"invalid bindingtype: {type}!");
        }


    }
}