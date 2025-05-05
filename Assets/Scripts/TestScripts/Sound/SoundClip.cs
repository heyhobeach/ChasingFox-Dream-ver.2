using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundClip", menuName = "Scriptable Objects/SoundClip")]
public class SoundClip : ScriptableObject
{
    public AudioClip clip;
    public Vector2 playRange = Vector2.zero;


#if UNITY_EDITOR
    private AudioClip previousClip;
    private Vector2 previousPlayRange;

    private void OnValidate()
    {
        if (clip != previousClip)
        {
            if (clip != null) playRange = new Vector2(0, clip.length);
            else playRange = Vector2.zero;
        }
        else if (playRange != previousPlayRange)
        {
             if (clip != null)
             {
                 playRange.x = Mathf.Clamp(playRange.x, 0, clip.length);
                 playRange.y = Mathf.Clamp(playRange.y, playRange.x, clip.length);
             }
             else playRange = Vector2.zero;
        }
        previousClip = clip;
        previousPlayRange = playRange;
    }
#endif
}
