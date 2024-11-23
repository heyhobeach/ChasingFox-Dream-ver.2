using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ButtonSelector))]
public class ButtonSelectorEditor : Editor
{
    private SerializedProperty arraySize;
    private SerializedProperty currentIdx;
    private SerializedProperty buttons;

    // 배열 크기 변경을 위한 임시 배열
    private ButtonWrapper[] tempArray;
    private Vector2Int tempSize;

    private void OnEnable()
    {
        arraySize = serializedObject.FindProperty("arraySize");
        currentIdx = serializedObject.FindProperty("buttonIdx");
        buttons = serializedObject.FindProperty("buttons");

        // 초기화
        ButtonSelector targetScript = (ButtonSelector)target;
        tempArray = targetScript.buttons;
        tempSize = targetScript.arraySize;
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 행과 열 표시
        EditorGUILayout.PropertyField(arraySize, new GUIContent("Array Size"));
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(currentIdx, new GUIContent("Current Idx"));
        EditorGUI.EndDisabledGroup();

        ButtonSelector targetScript = (ButtonSelector)target;

        // 배열 크기 변경 처리
        if (tempArray.Length != GetIndex(targetScript.arraySize.x, targetScript.arraySize.y))
        {
            ResizeArray(ref tempArray, targetScript.arraySize.x , targetScript.arraySize.y);
        }

        // 2차원 배열 표시
        EditorGUILayout.LabelField("Buttons:");
        for (int i = 0; i < targetScript.arraySize.x; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < targetScript.arraySize.y; j++)
            {
                tempArray[GetIndex(i, j)] = EditorGUILayout.ObjectField(tempArray[GetIndex(i, j)], typeof(ButtonWrapper), true, GUILayout.Width(100)) as ButtonWrapper;
            }
            EditorGUILayout.EndHorizontal();
        }

        // 배열을 스크립트에 다시 저장
        targetScript.buttons = tempArray;

        serializedObject.ApplyModifiedProperties();
    }

    private void ResizeArray(ref ButtonWrapper[] array, int newRows, int newColumns)
    {
        ButtonWrapper[] newArray = new ButtonWrapper[newRows * newColumns];
        for (int i = 0; i < Mathf.Min(tempSize.x, newRows); i++)
        {
            for (int j = 0; j < Mathf.Min(tempSize.y, newColumns); j++)
            {
                newArray[GetIndex(i, j)] = array[GetIndex(i, j, tempSize)];
            }
        }
        array = newArray;
        tempSize = new Vector2Int(newRows, newColumns);
    }
    private int GetIndex(int i, int j) => i * ((ButtonSelector)target).arraySize.y + j;
    private int GetIndex(int i, int j, Vector2Int size) => i * size.y + j;
}

