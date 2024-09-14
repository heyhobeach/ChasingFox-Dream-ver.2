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

    public GameObject[] queueArr=new GameObject[4];
    public float bulletTime = 0.3f;

    [System.Serializable]
    public class BulletWrapper
    {
        public Queue<GameObject> standbyBullet;

        //public Queue<GameObject> useBullet = new Queue<GameObject>();
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
            DontDestroyOnLoad(this);
        }
        Instance = instance;
        foreach(var obj in queueArr)
        {
            //Instantiate(obj);
        }
        bullet.standbyBullet = new Queue<GameObject>(queueArr);
    }


    public void SFXSound(AudioClip Sound)
    {
        AudioSource SFXsound =GetComponent<AudioSource>();
    }

    public void CoStartBullet(GameObject obj)
    {
        StartCoroutine(CoBulletSound(obj));
    }

    public IEnumerator CoBulletSound(GameObject obj)
    {
        //GameObject _obj=bullet.standbyBullet.Dequeue();
        //Debug.Log("코루틴 호출");
        
        obj.SetActive(true);
        if (obj.gameObject.layer == LayerMask.NameToLayer("GunSound"))
        {
            // Debug.Log("총 소리 레이어");
            bullet.standbyBullet.Enqueue(obj);
        }

        yield return null;
        obj.SetActive(false);
        // Debug.Log(string.Format("태그{0} 오브젝트 이름{1}", obj.transform.parent.tag, obj.name));
       
        if (obj.transform.parent.tag == "Player")
        {
            // Debug.Log("유저 총 소리");
            yield return null;
        }
        //else
        //{
        //    Debug.Log("성공적 삽입"+obj.name);
        //    bullet.standbyBullet.Enqueue(obj);
        //}
        
    }

}
