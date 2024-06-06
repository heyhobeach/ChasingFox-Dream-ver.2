using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float bullet_speed = 1f;

    public GameObject player;

    public GameObject temp;

    public Vector3 targetPos;
    public Vector3 shootPos;
    public Vector2 nowpos;
    public Vector2 destination;

    public float startTime;

    public Rigidbody2D bullet_rg;

    public bool block;
    // Start is called before the first frame update
    
    private void Awake()
    {
        //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("bullet"), LayerMask.NameToLayer("OneWayPlatform"), true);
        //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("bullet"), LayerMask.NameToLayer("Ground"), true);
    }
    void Start()
    {
        Debug.Log("총알 생성");
        startTime = 0;
        if (this.gameObject.tag != "bullet")//적군총알이라면
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyBullet"), LayerMask.NameToLayer("OneWayPlatform"), true);//총알 레이어 설정
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyBullet"), LayerMask.NameToLayer("Ground"), true);
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyBullet"), LayerMask.NameToLayer("Enemy"), true);
            destination = targetPos - transform.position;//타겟 목적지 설정
        }
        else//플레이어 총알이라면
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Bullet"), LayerMask.NameToLayer("OneWayPlatform"), true);//총알 레이어 설정
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Bullet"), LayerMask.NameToLayer("Ground"), true);
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Bullet"), LayerMask.NameToLayer("Player"), true);
            //this.gameObject.GetComponent<CapsuleCollider2D>().isTrigger = true;
            destination = targetPos - transform.position;//타겟 목적지 설정
        }


        bullet_rg = GetComponent<Rigidbody2D>();    
        //transform.position = Vector3.MoveTowards(transform.position, temp.GetComponent<EnemyScript>().playerPos, Time.deltaTime * 10);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log("player pos"+ targetPos + "enemy pos" + shootPos);
        nowpos = transform.position;//목적지 도착시 오브젝트 파괴를 위함
        bullet_rg.velocity = new Vector2(destination.x, destination.y).normalized*bullet_speed;
        //nowpos = Vector2.MoveTowards(transform.position, destination, Time.deltaTime * 10);
        
        if (nowpos == destination)
        {
            Debug.Log("도착");
            //Destroy(this.gameObject) ;
        }



    }


    private void Update()
    {
        startTime+= Time.deltaTime;
        if (startTime > 4)//그냥 총알이 4초 넘어가면 파괴하기 위함
        {
            Destroy(this.gameObject);
            
        }

        if (!ControllerScript.Instance.isCrouching)//엄폐중이라면 해당 오브젝트를 trigger를 꺼서 충돌 가능하도록 만듦. (이 부분은 적군총알과 아군총알 같이 쓰기때문에 오류 일으킬 가능성이 보임 따라서 this.gameObject로 접근하는게 아닌 적 총알 프리펩을 받아서 그 오브젝트를 수정해야할것으로 보임
        {//대충 테스트 했을때는 문제가 없었음
            this.gameObject.GetComponent<CapsuleCollider2D>().isTrigger = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "guard")//필요없어보임
        {
            this.gameObject.GetComponent<CapsuleCollider2D>().isTrigger = true;
        }
        if (collision.gameObject.tag == "Player")//레이어 설정한 것 때문에 적군 총알만 플레이어 에게 충돌일어남
        {

        }
        if(collision.gameObject.tag == "Enemy")//플레이어 총알이 적군에게 충돌시
        {
            Debug.Log("적 충돌");
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "guard")
        {
            
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "guard")//가드 만날겨우 trigger를 켜서 collision이 안일어나도록함
        {
            this.gameObject.GetComponent<CapsuleCollider2D>().isTrigger = true;
        }
        if (collision.gameObject.tag == "Player")//레이어 설정한 것 때문에 적군 총알만 플레이어 에게 충돌일어남
        {
            Debug.Log("플레이어 충돌");
            Destroy(this.gameObject);
        }
        if (collision.gameObject.tag == "Enemy")//플레이어 총알이 적군에게 충돌시 적이 trigger로 설정되어있어서 안 쓸것 같음
        {
            //Debug.Log("적 충돌");
            //Destroy(this.gameObject);
        }


    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            Destroy(this.gameObject);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "guard")//가드 벗어나면 tigger를 false해서 collision을 할 수 있도록 만든다
        {
            Debug.Log("벗어남");
            this.gameObject.GetComponent<CapsuleCollider2D>().isTrigger = false;
        }
    }
}
