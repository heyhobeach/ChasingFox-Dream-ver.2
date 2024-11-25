using Damageables;
using JetBrains.Annotations;
using System;
using UnityEngine;

public class FallingTrap : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int damage = 3;

    public HingeJoint2D joint2D;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (joint2D != null)
        {
            joint2D.useConnectedAnchor = false;
            joint2D.connectedBody = null;
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
        bool isDamaged = false;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))//플레이어 총알이 적군에게 충돌시
        {
            Debug.Log("적충돌");
            var temp = collision.gameObject.GetComponent<IDamageable>();
            Func<Collider2D, Vector2> func = null;
            //func += DamagedFeedBack;
            if (temp != null) 
            {
                isDamaged = temp.GetDamage(damage, collision, func);
            }

            if (isDamaged) { 
                Debug.Log("isDamaged is not null");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("isDamaged is null");
            }
        }
    }
}
