using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public const float lifeTime = 4;

    private int damage;
    private float speed;

    private Vector2 destination;

    private Rigidbody2D rg;
    private float startTime;
    private string ignoreTag;

    public void Set(Vector3 shootPos, Vector3 targetPos, int damage, float speed, GameObject gobj, Vector3 addPos = new Vector3())
    {
        // if (gobj.tag == "Enemy")
        // {
        //     ignoreTag = gobj.tag;
        //     // Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Bullet"), LayerMask.NameToLayer("Enemy"));
        // }
        // else if(gobj.tag == "Player")
        // {
        //     ignoreTag = gobj.tag;
        //     // Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Bullet"), LayerMask.NameToLayer("Player"));
        // }
        ignoreTag = gobj.tag;
        transform.position = (Vector2)shootPos + (Vector2)addPos;
        destination = ((Vector2)targetPos - (Vector2)shootPos).normalized;
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
        if(collision.CompareTag("ground") || collision.CompareTag("Wall")) Destroy(gameObject);
        if (collision.gameObject.tag == "guard")//필요없어보임
        {
            this.gameObject.GetComponent<Collider2D>().isTrigger = true;
        }
        if (collision.gameObject.tag == "Player" && !collision.gameObject.CompareTag(ignoreTag))//레이어 설정한 것 때문에 적군 총알만 플레이어 에게 충돌일어남
        {

        }
        if(collision.gameObject.tag == "Enemy" && !collision.gameObject.CompareTag(ignoreTag))//플레이어 총알이 적군에게 충돌시
        {
            Debug.Log("적 충돌");
            //Destroy(this.gameObject);
            var temp = collision.gameObject.GetComponent<IDamageable>();
            Debug.Log(temp.health);
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "guard")
        {
            
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("ground") || collision.gameObject.CompareTag("Wall")) Destroy(gameObject);

        if (collision.gameObject.tag == "guard")//가드 만날겨우 trigger를 켜서 collision이 안일어나도록함
        {
            this.gameObject.GetComponent<Collider2D>().isTrigger = true;
        }
        if (collision.gameObject.tag == "Player" && !collision.gameObject.CompareTag(ignoreTag))//레이어 설정한 것 때문에 적군 총알만 플레이어 에게 충돌일어남
        {
            // Debug.Log("플레이어 충돌");
            var temp = collision.gameObject.GetComponent<IDamageable>();
            bool isDamaged = false;
            if(temp != null) isDamaged = temp.GetDamage(damage);
            if (isDamaged)
            {
                Destroy(this.gameObject);
                //Destroy(gameObject);
            }

        }
        if (collision.gameObject.tag == "Enemy" && !collision.gameObject.CompareTag(ignoreTag))//플레이어 총알이 적군에게 충돌시 적이 trigger로 설정되어있어서 안 쓸것 같음
        {
            //Debug.Log("적 충돌");
            //Destroy(this.gameObject);
        }


    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player" && !collision.gameObject.CompareTag(ignoreTag))
        {
            //Destroy(this.gameObject);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "guard")//가드 벗어나면 tigger를 false해서 collision을 할 수 있도록 만든다
        {
            // Debug.Log("벗어남");
            this.gameObject.GetComponent<Collider2D>().isTrigger = false;
        }
    }
}
