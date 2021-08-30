using System;
using elZach.GraphScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class BehaviourTreeEditorWindow : EditorWindow
{
    private BehaviourTreeView treeView;
    private InspectorView inspectorView;
    private NodeView selectedNodeView;
    
    [MenuItem("Window/Graph/Scripting Graph")]
    public static void Init()
    {
        BehaviourTreeEditorWindow wnd = CreateWindow<BehaviourTreeEditorWindow>();  //GetWindow<BehaviourTreeEditorWindow>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditorWindow");
        wnd.minSize = new Vector2(250, 250);
    }

    private void OnFocus()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }
    
    void OnDisable()
    { 
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        SceneView.duringSceneGui -= OnSceneGUI;
    } 

    private void OnPlayModeChanged(PlayModeStateChange state)
    {
        Debug.Log(state);
        if(state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.EnteredEditMode)
            OnSelectionChange();
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.elzach.graphscripting/Editor/BehaviourTreeEditorWindow.uxml");
        visualTree.CloneTree(root);
        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.elzach.graphscripting/Editor/BehaviourTreeEditorWindow.uss");
        root.styleSheets.Add(styleSheet);

        treeView = root.Q<BehaviourTreeView>();
        inspectorView = root.Q<InspectorView>();

        treeView.onNodeSelected = OnNodeViewSelectionChanged;
        
        OnSelectionChange();
    }

    void OnSelectionChange()
    {
        //if (treeView == null) return;
        
        TreeContainer tree = Selection.activeObject as TreeContainer;
        if (!tree) tree = Selection.activeGameObject?.GetComponent<TreeDirector>()?.data;
        if (tree && (Application.isPlaying || AssetDatabase.CanOpenForEdit(tree)))  //AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
        {
            treeView.PopulateView(tree);
        }
    }

    void OnNodeViewSelectionChanged(NodeView nodeView)
    {
        inspectorView.UpdateSelection(nodeView);
        selectedNodeView = nodeView;
    }

    void OnInspectorUpdate()
    {
        treeView?.UpdateNodeViewStates();
    }

    void OnSceneGUI(SceneView sceneView)
    {
        //Handles.BeginGUI();
        if(selectedNodeView != null) selectedNodeView.node.OnDrawSelected();
        //Handles.EndGUI();
    }

    // [DrawGizmo()]
    // static void DrawGizmos(Component comp, GizmoType gizmoType)
    // {
    //     Debug.Log("huh");
    // }
    
}