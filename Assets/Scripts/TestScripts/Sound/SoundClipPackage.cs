using UnityEngine;

[CreateAssetMenu(fileName = "SoundClipPackage", menuName = "Scriptable Objects/SoundClipPackage")]
public class SoundClipPackage : ScriptableObject
{
    public SoundClip[] soundClips;
}
