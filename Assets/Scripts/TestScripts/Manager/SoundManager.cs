using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager:MonoBehaviour
{
    // Start is called before the first frame update
    /// <summary>
    /// 배경음악을 담당할 오디오 소스 변수
    /// </summary>
    AudioSource BGsound;

    public int publicTestInt = 0;
    [SerializeField]
    private int privateTestInt = 1;

    [System.Serializable]
    public class BulletWrapper
    {
        public Queue<GameObject> standbyBullet = new Queue<GameObject>();

        public Queue<GameObject> useBullet = new Queue<GameObject>();
    }
    

    [Serializable]
    public class TestWrapper
    {
        public int wrapperInt = 3;
        public int[] wrapperArr = new int[30];
    }

    [SerializeField]
    public BulletWrapper bullet = new BulletWrapper();

    public TestWrapper tw=new TestWrapper();
    public static SoundManager Instance;
    private static SoundManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        Instance = instance;


    }


    public void SFXSound(AudioClip Sound)
    {
        AudioSource SFXsound =GetComponent<AudioSource>();
    }

}
