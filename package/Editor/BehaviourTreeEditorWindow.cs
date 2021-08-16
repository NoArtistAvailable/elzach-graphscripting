using elZach.GraphScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class BehaviourTreeEditorWindow : EditorWindow
{
    private BehaviourTreeView treeView;
    private InspectorView inspectorView;
    
    [MenuItem("Window/Graph/Scripting Graph")]
    public static void ShowExample()
    {
        BehaviourTreeEditorWindow wnd = GetWindow<BehaviourTreeEditorWindow>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditorWindow");
    }

    void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }
    
    void OnDisable() => EditorApplication.playModeStateChanged -= OnPlayModeChanged;

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
    }

    void OnInspectorUpdate()
    {
        treeView?.UpdateNodeViewStates();
    }
    
}