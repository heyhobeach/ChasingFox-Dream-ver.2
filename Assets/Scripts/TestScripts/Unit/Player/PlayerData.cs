using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjectDatas/PlayerData")]
public class PlayerData : ScriptableObject
{
    public int formIdx;
    public int health;
    public BrutalData brutalData;
    public float brutalGaugeRemaining;

    public PlayerController.PlayerControllerMask pcm;

    public static int lastRoomIdx = 0;

    public void Init()
    {
        formIdx = 0;
        health = 3;
        brutalData = new();
        brutalGaugeRemaining = 0;
        pcm = PlayerController.PlayerControllerMask.None;
    }
    public void Init(PlayerData playerData)
    {
        formIdx = playerData.formIdx;
        health = playerData.health;
        brutalData = playerData.brutalData;
        brutalGaugeRemaining = playerData.brutalGaugeRemaining;
        pcm = playerData.pcm;
    }

#if UNITY_EDITOR
    void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
    void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
    {
        if(playModeStateChange == PlayModeStateChange.ExitingPlayMode)
        {
            lastRoomIdx = 0;
        }
    }
#endif
}
