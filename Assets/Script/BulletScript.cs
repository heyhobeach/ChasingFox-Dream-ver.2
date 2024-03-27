using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float bullet_speed = 1f;

    public GameObject player;

    public GameObject temp;

    public Vector2 nowpos;

    public Vector2 destination;
    public float startTime;

    public Rigidbody2D bullet_rg;

    public bool block;
    // Start is called before the first frame update

    private void Awake()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("bullet"), LayerMask.NameToLayer("OneWayPlatform"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("bullet"), LayerMask.NameToLayer("Ground"), true);
    }
    void Start()
    {
        startTime = 0;
        temp = transform.parent.gameObject;
        Debug.Log("�θ�" + temp);
        Debug.Log("����� "+ transform.parent.transform.parent.gameObject);
        Debug.Log("_playerPos"+temp.GetComponent<EnemyScript>().playerPos);
        destination = temp.GetComponent<EnemyScript>().playerPos- transform.position ;

        bullet_rg = GetComponent<Rigidbody2D>();    
        //transform.position = Vector3.MoveTowards(transform.position, temp.GetComponent<EnemyScript>().playerPos, Time.deltaTime * 10);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log("player pos"+temp.GetComponent<EnemyScript>().playerPos+ "enemy pos" + temp.GetComponent<EnemyScript>().enemypos);
        nowpos = transform.position;
        bullet_rg.velocity = new Vector2(destination.x, destination.y).normalized*bullet_speed;
        //nowpos = Vector2.MoveTowards(transform.position, destination, Time.deltaTime * 10);

        if (nowpos == destination)
        {
            Debug.Log("����");
            //Destroy(this.gameObject) ;
        }



    }


    private void Update()
    {
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, 0.1f,LayerMask.GetMask("Player"));//����ĳ��Ʈ �κ�
        //Debug.DrawRay(transform.position, Vector2.left);
        //if (hit.collider!=null)
        //{
        //    Debug.Log("���̾� �浹 ��ġ" + hit.point);
        //    Destroy(this.gameObject);
        //    Debug.Log("��� " + (hit.point.x - hit.collider.transform.position.x));//��� �¿��� �浹 ���� �쿡�� �浹
        //}
        startTime+= Time.deltaTime;
        if (startTime > 4)
        {
            Destroy(this.gameObject);
            
        }

        if (!ControllerScript.Instance.isCrouching)
        {
            this.gameObject.GetComponent<CapsuleCollider2D>().isTrigger = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "guard")
        {
            this.gameObject.GetComponent<CapsuleCollider2D>().isTrigger = true;
        }
        if (collision.gameObject.tag == "Player")
        {
            //float result;
            //Debug.Log("���� ����");
            //collision.
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
        if (collision.gameObject.name == "guard")
        {
            this.gameObject.GetComponent<CapsuleCollider2D>().isTrigger = true;
        }
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("collison �浹");
            Destroy(this.gameObject);
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
        if (collision.gameObject.name == "guard")
        {
            Debug.Log("���");
            this.gameObject.GetComponent<CapsuleCollider2D>().isTrigger = false;
        }
    }
}
