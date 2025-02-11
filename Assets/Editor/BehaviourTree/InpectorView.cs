using UnityEngine.UIElements;

namespace BehaviourTree.Editor
{
    
    [UxmlElement("InspectorView")]
    public partial class InspectorView : VisualElement
    {
        private UnityEditor.Editor editor;
        public InspectorView() {}

        internal void UpdateSelection(NodeView nodeView)
        {
            Clear();
            
            UnityEngine.Object.DestroyImmediate(editor);
            editor = UnityEditor.Editor.CreateEditor(nodeView.behaviourNode);
            IMGUIContainer container = new IMGUIContainer(() => { if(editor.target)editor.OnInspectorGUI(); });
            Add(container);
        }
    }
}