using UnityEngine;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjectDatas/PlayerData")]
[Serializable]
public class PlayerData : ScriptableObject
{
    [Serializable]
    public class JsonData
    {
        public int karma;
        public int formIdx;
        public int maxHealth;
        public int health;
        public int brutalGaugeRemaining;
        public PlayerController.PlayerControllerMask pcm;

        public static implicit operator JsonData(PlayerData data)
        {
            return new JsonData {
                karma = data.karma,
                formIdx = data.formIdx,
                maxHealth = data.maxHealth,
                health = data.health,
                brutalGaugeRemaining = data.brutalGaugeRemaining,
                pcm = data.pcm
            };
        }
    }

    public int karma;

    public int formIdx;
    public int maxHealth;
    public int health;
    public int brutalGaugeRemaining;

    public PlayerController.PlayerControllerMask pcm;

    public static int lastRoomIdx = 0;

    public void Init()
    {
        karma = 65;
        formIdx = 0;
        maxHealth = 1;
        health = maxHealth;
        brutalGaugeRemaining = 0;
        pcm = (PlayerController.PlayerControllerMask)~0;
    }
    public void Init(JsonData playerData)
    {
        karma = playerData.karma;
        formIdx = playerData.formIdx;
        maxHealth = playerData.maxHealth;
        health = playerData.health;
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
            karma = 65;
            formIdx = 0;
            maxHealth = 1;
            health = maxHealth;
            brutalGaugeRemaining = 0;
            pcm = (PlayerController.PlayerControllerMask)~0;
        }
    }
#endif
}
