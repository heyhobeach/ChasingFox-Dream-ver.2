using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public const float lifeTime = 4;

    private int damage;
    private float speed;

    private Vector2 destination;

    private GameObject parentGo;
    private Rigidbody2D rg;
    private float startTime;

    /// <summary>
    /// 벽과 충돌시 총알이 남아있는 시간
    /// </summary>
    // 이거 이번 프레임에만 활성화 되어있게 변경함
    public float soundTime = 0.3f;

    public void Set(Vector3 shootPos, Vector3 targetPos, Vector3 rotation, int damage, float speed, GameObject gobj, Vector3 addPos = new Vector3())
    {
        // Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Bullet"), gobj.layer);
        parentGo = gobj;
        transform.position = (Vector2)shootPos + (Vector2)addPos;
        destination = ((Vector2)targetPos - (Vector2)shootPos).normalized;
        transform.GetChild(0).transform.localEulerAngles = rotation;
        this.damage = damage;
        this.speed = speed;
        gameObject.SetActive(true);
    }

    private void OnEnable() => startTime = 0;
    private void Awake()
    {
        rg = GetComponent<Rigidbody2D>();
        rg.excludeLayers = 1<<LayerMask.NameToLayer("Platform");
    }
    private void Update()
    {
        startTime += Time.unscaledDeltaTime;
        if(startTime >= lifeTime)
        {
            Destroy(gameObject);
            return;
        }
    } 
    private void FixedUpdate() => rg.velocity = destination * speed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Map")) BulletSound();
        if(collision.CompareTag("ground") || collision.CompareTag("Wall") || collision.CompareTag("Map")) Destroy(gameObject);
        if (collision.gameObject.tag == "guard")//필요없어보임
        {
            this.gameObject.GetComponent<Collider2D>().isTrigger = true;
        }
        if (collision.gameObject.tag == "Player" && !parentGo.CompareTag("Player"))//레이어 설정한 것 때문에 적군 총알만 플레이어 에게 충돌일어남
        {
            // Debug.Log("플레이어 충돌");
            var temp = collision.gameObject.GetComponent<IDamageable>();
            if(temp == null) temp = collision.gameObject.GetComponentInParent<IDamageable>();
            bool isDamaged = false;
            if(temp != null)
            {
                Debug.Log("Work");
                isDamaged = temp.GetDamage(damage);
            }
            if (isDamaged)
            {
                Destroy(this.gameObject);
                Destroy(gameObject);
            }
            else Debug.Log("Not Work");
        }
        if(collision.gameObject.tag == "Enemy" && !parentGo.CompareTag("Enemy"))//플레이어 총알이 적군에게 충돌시
        {
            Debug.Log("적 충돌");
            //Destroy(this.gameObject);
            var temp = collision.gameObject.GetComponent<IDamageable>();
            bool isDamaged = false;
            if(temp != null) isDamaged = temp.GetDamage(damage);//이거 작동안함
            if (isDamaged)
            {
                Debug.Log("데미지 받음");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("작동안함");
            }
        }
    }
    // private void OnTriggerStay2D(Collider2D collision)
    // {
    //     if(collision.CompareTag("Map")) BulletSound();
    //     if(collision.CompareTag("ground") || collision.CompareTag("Wall") || collision.CompareTag("Map")) Destroy(gameObject);
    //     if (collision.gameObject.tag == "guard")//필요없어보임
    //     {
    //         this.gameObject.GetComponent<Collider2D>().isTrigger = true;
    //     }
    //     if (collision.gameObject.tag == "Player")//레이어 설정한 것 때문에 적군 총알만 플레이어 에게 충돌일어남
    //     {

    //     }
    //     if(collision.gameObject.tag == "Enemy")//플레이어 총알이 적군에게 충돌시
    //     {
    //         Debug.Log("적 충돌");
    //         //Destroy(this.gameObject);
    //         var temp = collision.gameObject.GetComponent<IDamageable>();
    //         Debug.Log(temp.health);
    //         bool isDamaged = false;
    //         if(temp != null) isDamaged = temp.GetDamage(damage);//이거 작동안함
    //         if (isDamaged)
    //         {
    //             Debug.Log("데미지 받음");
    //             // Destroy(gameObject);
    //         }
    //         else
    //         {
    //             Debug.Log("작동안함");
    //         }
    //     }
    // }

    // private void OnTriggerExit2D(Collider2D collision)
    // {
    //     if (collision.gameObject.tag == "guard")
    //     {
            
    //     }
    // }
    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.tag == "Map")
    //     {
    //         BulletSound();
    //         Destroy(this.gameObject);
    //         //GameObject obj = SoundManager.Instance.bullet.standbyBullet.Dequeue();
    //         //obj.transform.position = this.transform.position;
    //         //Debug.Log(string.Format("queue name => " + obj));
    //         //SoundManager.Instance.CoStartBullet(obj);
    //         //StartCoroutine(SoundManager.Instance.CoBulletSound(obj));//해당 객체가 사라져서 그런듯 이 부분을 soundManager로 옮겨야함
    //         //SoundManager.Instance.bullet.standbyBullet.Enqueue(obj);
    //     }
    //     if (collision.gameObject.CompareTag("ground") || collision.gameObject.CompareTag("Wall")) Destroy(gameObject);

    //     if (collision.gameObject.tag == "guard")//가드 만날겨우 trigger를 켜서 collision이 안일어나도록함
    //     {
    //         this.gameObject.GetComponent<Collider2D>().isTrigger = true;
    //     }
    //     if (collision.gameObject.tag == "Player" && !parentGo.CompareTag("Player"))//레이어 설정한 것 때문에 적군 총알만 플레이어 에게 충돌일어남
    //     {
    //         // Debug.Log("플레이어 충돌");
    //         var temp = collision.gameObject.GetComponent<IDamageable>();
    //         bool isDamaged = false;
    //         Debug.Log(collision.gameObject.name);
    //         if(temp != null) isDamaged = temp.GetDamage(damage);
    //         if (isDamaged)
    //         {
    //             Destroy(this.gameObject);
    //             //Destroy(gameObject);
    //         }
    //         else Debug.Log("Not Work");
    //     }
    //     if (collision.gameObject.tag == "Enemy")//플레이어 총알이 적군에게 충돌시 적이 trigger로 설정되어있어서 안 쓸것 같음
    //     {
    //         //Debug.Log("적 충돌");
    //         //Destroy(this.gameObject);
    //     }


    // }

    // private void OnCollisionStay2D(Collision2D collision)
    // {
    //     Destroy(gameObject);
    //     if (collision.gameObject.tag == "Map")
    //     {
    //         BulletSound();
    //         Destroy(this.gameObject);
    //     }
    //     if (collision.gameObject.tag == "Player")
    //     {
    //         //Destroy(this.gameObject);
    //     }
    // }
    // private void OnCollisionExit2D(Collision2D collision)
    // {
    //     if (collision.gameObject.tag == "guard")//가드 벗어나면 tigger를 false해서 collision을 할 수 있도록 만든다
    //     {
    //         // Debug.Log("벗어남");
    //         this.gameObject.GetComponent<Collider2D>().isTrigger = false;
    //     }
    // }

    private void BulletSound()
    {
        Destroy(this.gameObject);
        GameObject obj = SoundManager.Instance.bullet.standbyBullet.Dequeue();
        obj.transform.position = this.transform.position;
        // Debug.Log(string.Format("queue name => " + obj));
        SoundManager.Instance.CoStartBullet(obj);
    }
}
