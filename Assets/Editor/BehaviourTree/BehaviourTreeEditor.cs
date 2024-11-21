using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using System;
using Unity.VisualScripting;

namespace BehaviourTree.Editor
{    
    /// <summary>
    /// 출처 : https://www.youtube.com/@TheKiwiCoder
    /// </summary>
    public class BehaviourTreeEditor : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        private BehaviourTreeView treeView;
        private InspectorView inspectorView;
        private IMGUIContainer blackboardView;
        SerializedObject treeObject;
        SerializedProperty blackboardProperty;

        [MenuItem("BehaviourTreeEditor/Editor")]
        public static void OpenWindow()
        {
            BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
            wnd.titleContent = new GUIContent("Behaviour Tree Editor");
        }

        [OnOpenAsset] // 에셋 더블 클릭 시 실행
        public static bool OnOpenAsset(int instanceID, int line) // 클릭 시 창 오픈
        {
            if(Selection.activeObject is BehaviourTree)
            {
                OpenWindow();
                return true;
            }
            return false;
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            switch(change)
            {
                case PlayModeStateChange.EnteredPlayMode:
                case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
                case PlayModeStateChange.ExitingPlayMode:
                case PlayModeStateChange.ExitingEditMode:
                break;
            }
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            m_VisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/BehaviourTree/BehaviourTreeEditor.uxml");
            m_VisualTreeAsset.CloneTree(root);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTree/BehaviourTreeEditor.uss");
            root.styleSheets.Add(styleSheet);

            treeView = root.Q<BehaviourTreeView>();
            inspectorView = root.Q<InspectorView>();
            blackboardView = root.Q<IMGUIContainer>();
            blackboardView.onGUIHandler = () => {
                if(treeObject.IsUnityNull()) return;
                treeObject.Update();
                EditorGUILayout.PropertyField(blackboardProperty);
                treeObject.ApplyModifiedProperties();
            };

            treeView.OnNodeSelected = OnNodeSelectionChanged;

            OnSelectionChange();
        }

        private void OnSelectionChange() // 에셋 선택이 바꼈을 때 바뀐 요소에 대응
        {
            var tree = Selection.activeObject as BehaviourTree;
            if(!tree && Selection.activeGameObject)
            {
                var runner = Selection.activeGameObject.GetComponent<EnemyController>();
                if(runner) tree = runner.behaviorTree;
            }
            if(Application.isPlaying) { if(tree && treeView != null) treeView.PopulateView(tree); } 
            else { if(tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID())) treeView.PopulateView(tree); }
            if(tree) 
            {
                treeObject = new(tree);
                blackboardProperty = treeObject.FindProperty("blackboard");
            }
        }

        private void OnNodeSelectionChanged(NodeView nodeView) => inspectorView.UpdateSelection(nodeView); // 노드 선택이 바꼈을 때

        private void OnInspectorUpdate() => treeView?.NodeStateUpdate();
    }
}
