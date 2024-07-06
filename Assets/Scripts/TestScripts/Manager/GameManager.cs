using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get => instance; }

    [System.Serializable]
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
    public List<BrutalData> brutalDatas = new();
    public List<int> humanDatas = new();

    private int karma = 100;
    [SerializeField] private int karmaRatio = 65;

    public int Humanity { get => karmaRatio; set { karmaRatio = value; ClampRatio(); } }
    public int Brutality { get => karma-karmaRatio; set { karmaRatio = -value; ClampRatio(); } }
    private int ClampRatio() => Mathf.Clamp(karmaRatio, 0, 100);

    public int GetHumanData()
    {
        if(Humanity >= 30) return humanDatas[1];
        else return humanDatas[0];
    }
    public BrutalData GetBrutalData() => brutalDatas[Brutality/10];

    private void Awake()
    {
        if (instance != null) return;
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
