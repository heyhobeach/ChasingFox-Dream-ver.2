using Damageables;
using JetBrains.Annotations;
using System;
using UnityEngine;

public class FallingTrap : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int damage = 3;

    public HingeJoint2D joint2D;
    public GameObject TrapWerck;
    void Start()
    {
        joint2D = this.gameObject.GetComponent<HingeJoint2D>();
        TrapWerck = this.gameObject.transform.GetComponent<TrapSet>().TrapWreck;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name + "이름");
        if (joint2D != null)
        {
            joint2D.useConnectedAnchor = false;
            joint2D.connectedBody = null;
        }
        if(joint2D!= null && collision.gameObject.layer == LayerMask.NameToLayer("Map"))//오브젝트 접점 위치 판단을 위해
        {
            Vector2 vec2 = this.transform.position;
            GameObject gobj = transform.parent.GetComponent<TrapSet>().TrapWreck;
            //GameManager.Instance.
            ContactPoint2D[] contacts = new ContactPoint2D[20];
            int points = collision.GetContacts(contacts);
            vec2 = contacts[0].point;//여기 지우면 아래쪽 오브젝트로
            Debug.Log("접점 개수" + points);//현재 접점개수가 0개로 나옴
            for (int i = 0; i < points; i++)
            {
                Debug.Log(contacts[i].point);
            }

            float ysize = gobj.gameObject.GetComponent<BoxCollider2D>().size.y / 2;
            vec2.y += ysize;
            Instantiate(gobj, vec2, Quaternion.identity);
            this.transform.parent.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (joint2D != null&&collision.gameObject.layer==LayerMask.NameToLayer("Bullet"))
        {
            Destroy(collision.gameObject);
            joint2D.useConnectedAnchor = false;
            joint2D.connectedBody = null;
        }
        if (joint2D != null && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Vector2 vec2 = this.transform.position;
            GameObject gobj = transform.parent.GetComponent<TrapSet>().TrapWreck;
            //GameManager.Instance.
            ContactPoint2D[] contacts = new ContactPoint2D[20];
            int points = collision.GetContacts(contacts);
            vec2 = contacts[0].point;//여기 지우면 아래쪽 오브젝트로
            Debug.Log("접점 개수" + points);//현재 접점개수가 0개로 나옴
            for (int i = 0; i < points; i++)
            {
                Debug.Log(contacts[i].point);
            }
            //this.transform.parent.gameObject.SetActive(false);
            Instantiate(gobj, vec2, Quaternion.identity);

        }

        //bool isDamaged = false;//적에게 바로 충돌하는 함수
        //if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))//
        //{
        //    Debug.Log("적충돌");
        //    var temp = collision.gameObject.GetComponent<IDamageable>();
        //    Func<Collider2D, Vector2> func = null;
        //    //func += DamagedFeedBack;
        //    if (temp != null) 
        //    {
        //        isDamaged = temp.GetDamage(damage, collision, func);
        //    }
        //
        //    if (isDamaged) { 
        //        Debug.Log("isDamaged is not null");
        //        Destroy(gameObject);
        //    }
        //    else
        //    {
        //        Debug.Log("isDamaged is null");
        //    }
        //}
    }
}
