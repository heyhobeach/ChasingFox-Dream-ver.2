using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaleeAttack : MonoBehaviour
{
    private int damage;
    private GameObject parentGo;

    public void Set(int damage, GameObject go)
    {
        this.damage = damage;
        this.parentGo = go;
    }

    // Start is called before the first frame update
    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Hatch")
        {
            collision.gameObject.SetActive(false);
        }
        // if(collision.CompareTag("ground") || collision.CompareTag("Wall") || collision.CompareTag("Map")) return;
        // if (collision.gameObject.tag == "guard")//필요없어보임
        // {
        //     this.gameObject.GetComponent<Collider2D>().isTrigger = true;
        // }
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
            }
            else
            {
                Debug.Log("작동안함");
            }
        }
    }
}
