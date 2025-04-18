using System.Collections;
using System.Collections.Generic;
using Damageables;
using UnityEngine;

public class MaleeAttack : MonoBehaviour
{
    private int damage;
    private GameObject parentGo;
    public GameObject effectObj;

    public void Set(int damage, GameObject go)
    {
        this.damage = damage;
        this.parentGo = go;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isDamaged = false;
        if (collision.gameObject.tag == "Hatch")
        {
            // collision.gameObject.SetActive(false);
            var temp = collision.gameObject.GetComponent<IDamageable>();
            if(temp == null) temp = collision.gameObject.GetComponentInParent<IDamageable>();
            if(temp != null) isDamaged = temp.GetDamage(damage, parentGo.transform);
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
            if(temp != null) isDamaged = temp.GetDamage(damage, parentGo.transform);
        }
        if(collision.gameObject.tag == "Enemy" && !parentGo.CompareTag("Enemy"))//플레이어 총알이 적군에게 충돌시
        {
            var temp = collision.gameObject.GetComponent<IDamageable>();
            if(temp != null) isDamaged = temp.GetDamage(damage, parentGo.transform);
        }

        if(isDamaged)
        {
            var dir = transform.position - collision.transform.position;
            var effect = Instantiate(effectObj, collision.transform.position + (Vector3.left * Mathf.Sign(dir.x) * collision.bounds.extents.x * 0.5f) + Vector3.up, Quaternion.identity);
            var sprite = effect.GetComponent<SpriteRenderer>();
            if(dir.x < 0) sprite.flipX = true;
            else sprite.flipX = false;
        }
    }
}
