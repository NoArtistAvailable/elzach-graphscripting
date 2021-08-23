using UnityEditor;
using UnityEngine;

namespace elZach.GraphScripting
{
    [CustomPropertyDrawer(typeof(HideIfNotNullAttribute))]
    public class HideIfNotNullAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue != null) return 0;
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null)
            {
                var col = GUI.contentColor;
                GUI.contentColor = Color.red;
                EditorGUI.PropertyField(position, property, label);
                GUI.contentColor = col;
            }
        }
    }
}