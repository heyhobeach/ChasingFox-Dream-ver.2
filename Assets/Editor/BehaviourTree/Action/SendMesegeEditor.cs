using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Linq;
using UnityEngine.UIElements;

namespace BehaviourTree.Editor
{
    [CustomEditor(typeof(SendMesage))]
    public class SendMesageEditor : UnityEditor.Editor
    {
        private SerializedProperty script;
        private SerializedProperty guid;
        private SerializedProperty description;
        private SerializedProperty child;
        private SerializedProperty mesages;
        private ReorderableList list;

        private void OnEnable()
        {
            // script = serializedObject.FindProperty("m_script");
            guid = serializedObject.FindProperty("guid");
            description = serializedObject.FindProperty("description");
            mesages = serializedObject.FindProperty("mesages");
            // child = serializedObject.FindProperty("child");
            list = new ReorderableList(serializedObject, mesages, true, true, true, true);
            list.drawElementCallback = DrawListItem;
            list.drawHeaderCallback = DrawHeader;
        }

        public override VisualElement CreateInspectorGUI()
        {
            // VisualElement root = new();
            // serializedObject.Update();
            // EditorGUILayout.PropertyField(script, true);
            // EditorGUILayout.PropertyField(guid, true);
            // EditorGUILayout.PropertyField(description, true);
            // EditorGUILayout.PropertyField(child, true);
            // list.DoLayoutList();
            // serializedObject.ApplyModifiedProperties();
            // return root;
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            // EditorGUILayout.PropertyField(script, true);
            EditorGUILayout.PropertyField(guid, true);
            EditorGUILayout.PropertyField(description, true);
            // EditorGUILayout.PropertyField(child, true);
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader(Rect rect) => EditorGUI.LabelField(rect, "Mesages");
        private void DrawListItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, GUIContent.none);
        }
    }
}