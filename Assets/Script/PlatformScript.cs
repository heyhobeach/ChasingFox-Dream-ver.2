using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CompositeCollider2D))]
[RequireComponent(typeof(PlatformEffector2D))]
public class PlatformScript : MonoBehaviour
{
    public enum downJumpObject { STRAIGHT, DIAGONAL };
    public downJumpObject dObject;
    private PlatformEffector2D platformScr;
    private WaitForSeconds waitForSeconds;

    private void Awake()
    {
        platformScr = GetComponent<PlatformEffector2D>();
        waitForSeconds = new WaitForSeconds(0.1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var distance = collision.GetContact(0).separation;
        var normal = collision.GetContact(0).normal;
        var position = collision.rigidbody.transform.position;
        collision.rigidbody.MovePosition((Vector2)position + (-normal * distance));
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("Player")) StartCoroutine(DelayOnTrigger(collider));
    }

    IEnumerator DelayOnTrigger(Collider2D collider)
    {
        yield return waitForSeconds;
        AddColliderMask(1<<collider.gameObject.layer);
    }

    public void RemoveColliderMask(int layer) => platformScr.colliderMask &= ~layer;
    public void AddColliderMask(int layer) => platformScr.colliderMask |= layer;
}
