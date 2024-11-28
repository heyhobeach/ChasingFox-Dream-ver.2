using UnityEngine;

[CreateAssetMenu(fileName = "DialogueIndex", menuName = "Scriptable Objects/DialogueIndex")]
public class DialogueIndex : ScriptableObject
{
    [Tooltip("문단 번호")]
    public int indexNum;
    [Tooltip("번호")]
    public int num;

    //public int x;
}
