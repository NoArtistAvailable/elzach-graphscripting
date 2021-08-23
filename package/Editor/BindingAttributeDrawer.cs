using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace elZach.GraphScripting
{
    [CustomPropertyDrawer(typeof(BindingAttribute))]
    public class BindingAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) - EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var me = property.serializedObject.targetObject as Node;
            if (!me) return;

            if (me.TryGetSerializedParameter(property.name, out var param))
            {
                using (var changed = new EditorGUI.ChangeCheckScope())
                {
                    //Todo: this works in custom inspector but breaks the regular one
                    EditorGUILayout.BeginHorizontal();
                    position = EditorGUI.PrefixLabel(position, label);
                    EditorGUILayout.PrefixLabel(label);
                    if (EditorGUILayout.DropdownButton(GUIContent.none, FocusType.Passive))
                    {
                        //Debug.Log($"Looking for params of type {fieldInfo.FieldType}");
                        
                        void handleItemClicked(object parameter)
                        {
                            param.Parameter.name = parameter.ToString();
                            EditorUtility.SetDirty(me);
                            me.Init();
                        }
                        var exisiting = me.container.Parameters.FindAll(x => x.type == fieldInfo.FieldType);
                        GenericMenu menu = new GenericMenu();
                        foreach(var ex in exisiting)
                            menu.AddItem(new GUIContent(ex.name), false, handleItemClicked, ex.name);
                        menu.DropDown(position);
                    }
                    var newName = EditorGUILayout.DelayedTextField(param.Parameter.name);
                    EditorGUILayout.EndHorizontal();
                    param.Parameter.name = newName;
                    // var p = param.Parameter;
                    // p.name = newName;
                    // param.Parameter = p;
                    if (changed.changed)
                    {
                        EditorUtility.SetDirty(me);
                        me.Init();
                    }
                }
            }
        }
    }
}