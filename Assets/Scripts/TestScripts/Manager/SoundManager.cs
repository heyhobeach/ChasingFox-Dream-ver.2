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

    private Queue<(GameObject, bool)> objQueue = new ();

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

    private void Update()
    {
        if(objQueue.Count > 0)
        {
            while(objQueue.Count > 0) 
            {
                var temp = objQueue.Dequeue();
                StartCoroutine(CoBulletSound(temp.Item1, temp.Item2));
            }
        }
    }

    public void SFXSound(AudioClip Sound)
    {
        AudioSource SFXsound =GetComponent<AudioSource>();
    }

    public void CoStartBullet(GameObject obj, bool isQueue = true)
    {
        objQueue.Enqueue((obj, isQueue));
    }

    public IEnumerator CoBulletSound(GameObject obj, bool isQueue = true)
    {
        //GameObject _obj=bullet.standbyBullet.Dequeue();
        //Debug.Log("코루틴 호출");
#if UNITY_EDITOR
        drawobj.Add(obj);
#endif
        
        obj.SetActive(true);
        GameManager.Instance.OnGunsound(obj.transform, obj.transform.position, obj.GetComponent<CircleCollider2D>().bounds.extents);
        if (isQueue && obj.gameObject.layer == LayerMask.NameToLayer("GunSound"))
        {
            Debug.Log("총 소리 레이어");
            bullet.standbyBullet.Enqueue(obj);
        }

        // yield return new WaitForSeconds(bulletTime);
        // yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        yield return new WaitForEndOfFrame();

        obj.SetActive(false);

#if UNITY_EDITOR
        drawobj.Remove(obj);
#endif
        // Debug.Log(string.Format("태그{0} 오브젝트 이름{1}", obj.transform.parent.tag, obj.name));
       
        // if (obj.transform.parent != null && obj.transform.parent.tag == "Player")
        // {
        //     Debug.Log("유저 총 소리");
        //     yield return null;
        // }
        //else
        //{
        //    Debug.Log("성공적 삽입"+obj.name);
        //    bullet.standbyBullet.Enqueue(obj);
        //}
        
    }
#if UNITY_EDITOR
    List<GameObject> drawobj = new();
    void OnDrawGizmos()
    {
        if(drawobj.Count <= 0) return;
        Gizmos.color = Color.yellow;
        foreach(var obj in drawobj) Gizmos.DrawWireSphere(obj.transform.position, obj.GetComponent<CircleCollider2D>().bounds.extents.x * 0.5f);
    }
#endif
}
