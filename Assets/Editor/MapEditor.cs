using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq; // Linq 사용 시 필요

[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    private SerializedProperty timedEventsProperty;
    private Map mapScript;

    private void OnEnable()
    {
        mapScript = (Map)target;
        timedEventsProperty = serializedObject.FindProperty("mapEvents");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, "m_Script", "mapEvents");

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(timedEventsProperty, true);

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            mapScript.SortTimedEvents();
            EditorUtility.SetDirty(mapScript);
            serializedObject.Update();
        }
    }
}