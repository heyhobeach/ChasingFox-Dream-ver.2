using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DisableInInspector), true)]
public class DisableInInspectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUI.PropertyField(position, property, label);
        EditorGUI.EndDisabledGroup();
    }
}
