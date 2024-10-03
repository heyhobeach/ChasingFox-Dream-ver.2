using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace BehaviourTree.Editor
{
    public class InspectorView : VisualElement
    {
        private UnityEditor.Editor editor;

        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> {}
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