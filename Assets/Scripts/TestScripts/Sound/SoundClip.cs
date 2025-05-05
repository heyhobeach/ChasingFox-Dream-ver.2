using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundClip", menuName = "Scriptable Objects/SoundClip")]
public class SoundClip : ScriptableObject
{
    public AudioClip clip;
    public Vector2 playRange = Vector2.zero;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (clip != null) playRange = new Vector2(0, clip.length);
        else playRange = Vector2.zero;
    }
#endif
}
