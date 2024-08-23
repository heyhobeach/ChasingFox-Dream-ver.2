using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Linq;

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

    [CustomPropertyDrawer(typeof(SendMesage.Mesage))]
    public class MesageDrawer : PropertyDrawer
    {
        private string[] methodsNames;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                position.height = EditorGUIUtility.singleLineHeight;
                var sizeX = position.size.x;

                EditorGUI.ObjectField(
                    new Rect(position.x-sizeX*0.4f, position.y, sizeX*0.8f, EditorGUIUtility.singleLineHeight),
                    property.FindPropertyRelative("targetNode")
                );
                if(property.FindPropertyRelative("targetNode").objectReferenceValue != null)
                {
                    List<string> sl = new();
                    var methods = TypeCache.GetMethodsWithAttribute<MesageTarget>().ToList();
                    foreach(var method in methods) sl.Add(method.Name);
                    sl.Insert(0, "None");
                    methodsNames = sl.ToArray();
                    var idx = property.FindPropertyRelative("methodIdx").intValue;
                    property.FindPropertyRelative("methodIdx").intValue = EditorGUI.Popup(
                        new Rect(position.x-sizeX*0.4f+sizeX*0.9f, position.y, sizeX*0.5f, EditorGUIUtility.singleLineHeight),
                        idx,
                        methodsNames
                    );
                    property.FindPropertyRelative("methodName").stringValue = methodsNames[idx];
                }
            }
        }
    }
}