using UnityEditor;
using UnityEngine;

namespace elZach.GraphScripting
{
    [CustomEditor(typeof(TreeDirector))]
    public class TreeDirectorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Tree"))
            {
                BehaviourTreeEditorWindow.Init();
            }
            DrawDefaultInspector();
        }
    }
}