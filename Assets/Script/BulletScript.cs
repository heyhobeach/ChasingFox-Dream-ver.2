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
        startTime = 0;
        if (this.gameObject.tag != "bullet")//�����Ѿ��̶��
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyBullet"), LayerMask.NameToLayer("OneWayPlatform"), true);//�Ѿ� ���̾� ����
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyBullet"), LayerMask.NameToLayer("Ground"), true);
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyBullet"), LayerMask.NameToLayer("Enemy"), true);
            destination = targetPos - transform.position;//Ÿ�� ������ ����
        }
        else//�÷��̾� �Ѿ��̶��
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Bullet"), LayerMask.NameToLayer("OneWayPlatform"), true);//�Ѿ� ���̾� ����
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Bullet"), LayerMask.NameToLayer("Ground"), true);
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Bullet"), LayerMask.NameToLayer("Player"), true);
            //this.gameObject.GetComponent<CapsuleCollider2D>().isTrigger = true;
            destination = targetPos - transform.position;//Ÿ�� ������ ����
        }


        bullet_rg = GetComponent<Rigidbody2D>();    
        //transform.position = Vector3.MoveTowards(transform.position, temp.GetComponent<EnemyScript>().playerPos, Time.deltaTime * 10);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log("player pos"+ targetPos + "enemy pos" + shootPos);
        nowpos = transform.position;//������ ������ ������Ʈ �ı��� ����
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
        if (collision.gameObject.tag == "Player")//���̾� ������ �� ������ ���� �Ѿ˸� �÷��̾� ���� �浹�Ͼ
        {

        }
        if(collision.gameObject.tag == "Enemy")//�÷��̾� �Ѿ��� �������� �浹��
        {
            Debug.Log("�� �浹");
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
        if (collision.gameObject.name == "guard")
        {
            this.gameObject.GetComponent<CapsuleCollider2D>().isTrigger = true;
        }
        if (collision.gameObject.tag == "Player")//���̾� ������ �� ������ ���� �Ѿ˸� �÷��̾� ���� �浹�Ͼ
        {
            Debug.Log("�÷��̾� �浹");
            Destroy(this.gameObject);
        }
        if (collision.gameObject.tag == "Enemy")//�÷��̾� �Ѿ��� �������� �浹�� ���� trigger�� �����Ǿ��־ �� ���� ����
        {
            //Debug.Log("�� �浹");
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
        if (collision.gameObject.name == "guard")
        {
            Debug.Log("���");
            this.gameObject.GetComponent<CapsuleCollider2D>().isTrigger = false;
        }
    }
}
