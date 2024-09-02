using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator animator;
    private Collider2D col;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if((other.collider.CompareTag("Player") && other.transform.GetComponent<Player>().ChagedForm.UnitState == UnitState.Default) || 
           (other.collider.CompareTag("Enemy") && other.transform.GetComponent<UnitBase>().UnitState == UnitState.Default))
           StartCoroutine(Open());
    }

    IEnumerator Open()
    {
        animator.SetTrigger("open");
        yield return null;
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        col.enabled = false;
    }
}
