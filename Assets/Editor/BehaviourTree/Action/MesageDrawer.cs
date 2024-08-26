using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree.Editor
{
    [CustomPropertyDrawer(typeof(SendMesage.Mesage))]
    public class MesageDrawer : PropertyDrawer
    {
        private string[] methodsNames;

        // private void OnEnable()
        // {

        // }

        // public override VisualElement CreatePropertyGUI(SerializedProperty property)
        // {
        //     VisualElement root = new();
        //     var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/BehaviourTree/Action/MesageDrawer.uxml");
        //     visualTreeAsset.CloneTree(root);
        //     var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTree/Action/MesageDrawer.uss");
        //     root.styleSheets.Add(styleSheet);

        //     return root;
        // }

        // public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        // {
        //     return base.GetPropertyHeight(property, label)*2;
        // }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                position.height = EditorGUIUtility.singleLineHeight*2;
                var sizeX = position.size.x;
                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(
                    new Rect(position.x-sizeX*0.4f, position.y, sizeX*0.8f, EditorGUIUtility.singleLineHeight),
                    property.FindPropertyRelative("targetNode")
                );
                if(EditorGUI.EndChangeCheck()) property.FindPropertyRelative("methodIdx").intValue = 0;
                if(property.FindPropertyRelative("targetNode").objectReferenceValue != null)
                {
                    EditorGUI.LabelField(
                        new Rect(position.x, position.y+EditorGUIUtility.singleLineHeight, sizeX, EditorGUIUtility.singleLineHeight),
                        (property.FindPropertyRelative("targetNode").objectReferenceValue as BehaviourNode).guid
                    );
                    List<string> sl = new();
                    var type = property.FindPropertyRelative("targetNode").objectReferenceValue.GetType();
                    sl = Array.ConvertAll<MethodInfo, string>(type.GetMethods(), (n) => {
                        if(n.GetCustomAttribute<MesageTarget>() != null) return n.Name;
                        else return "";
                    }).ToList();
                    sl.Insert(0, "None");
                    string.Join(", ", sl);
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