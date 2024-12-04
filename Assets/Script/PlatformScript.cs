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
        waitForSeconds = new WaitForSeconds(0.2f);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("Player")) StartCoroutine(DelayOnTrigger(collider));
    }

    private IEnumerator DelayOnTrigger(Collider2D collider)
    {
        yield return waitForSeconds;
        AddColliderMask(1<<collider.gameObject.layer);
    }

    public void RemoveColliderMask(int layer) => platformScr.colliderMask &= ~layer;
    public void AddColliderMask(int layer) => platformScr.colliderMask |= layer;
    public int GetUpDown(Vector2 otherPos)
    {
        var temp = otherPos - (Vector2)transform.position;
        return (int) Mathf.Sign(temp.y);
    }
}
