using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[CreateAssetMenu(fileName = "TimeLineBranchScriptorble", menuName = "Scriptable Objects/TimeLineBranchScriptorble")]
public class TimeLineBranchScriptorble : ScriptableObject
{
    public PlayableAsset branch1;
    public PlayableAsset branch2;
}
