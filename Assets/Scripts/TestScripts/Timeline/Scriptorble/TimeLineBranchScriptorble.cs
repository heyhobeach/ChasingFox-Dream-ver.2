using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[CreateAssetMenu(fileName = "TimeLineBranchScriptorble", menuName = "Scriptable Objects/TimeLineBranchScriptorble")]
public class TimeLineBranchScriptorble : ScriptableObject
{
    public TimelineAsset branch1;
    public TimelineAsset branch2;
}
