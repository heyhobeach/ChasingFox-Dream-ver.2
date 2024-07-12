using System;
using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get => instance; }
    public BrutalDatas _brutalDatas;
    public static BrutalDatas brutalDatas;
    public HumanDatas _humanDatas;
    public static HumanDatas humanDatas;

    [CreateAssetMenu(fileName = "HumanData", menuName = "ScriptableObjects/HumanData", order = 1)]
    public class HumanDatas : ScriptableObject
    {
        public int[] counts;
    }

    [CreateAssetMenu(fileName = "BrutalData", menuName = "ScriptableObjects/BrutalData", order = 1)]
    public class BrutalDatas : ScriptableObject
    {
        public BrutalData[] brutalDatas;
    }
    [Serializable]
    public class BrutalData
    {
        public int maxGage;
        public int sec;
        public int atk;
        public int frm;

        public BrutalData(int maxGage, int sec, int atk, int frm)
        {
            this.maxGage = maxGage;
            this.sec = sec;
            this.atk = atk;
            this.frm = frm;
        }
    }

    private int karma = 100;
    [SerializeField] private int karmaRatio = 65;

    public int Humanity { get => karmaRatio; set { karmaRatio = value; ClampRatio(); } }
    public int Brutality { get => karma-karmaRatio; set { karmaRatio = -value; ClampRatio(); } }
    private int ClampRatio() => Mathf.Clamp(karmaRatio, 0, 100);

    public int GetHumanData() => humanDatas.counts[Humanity/10];
    public BrutalData GetBrutalData() => brutalDatas.brutalDatas[Brutality/10];

    private void Awake()
    {
        if (instance != null) return;
        instance = this;
        DontDestroyOnLoad(gameObject);
        humanDatas = _humanDatas;
        brutalDatas = _brutalDatas;
    }
}
