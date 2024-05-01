using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideBox : MonoBehaviour
{
    private float distance = 0;
    private void Start() => distance = GetComponent<Collider2D>().bounds.extents.x;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("AAA");
        if(collision.gameObject.CompareTag("Player"))
        {
            var pu = collision.GetComponent<PlayerUnit>();
            // pu.isHide = true;
            var dir = Mathf.Sign(transform.position.x - collision.transform.position.x);
            collision.transform.position = transform.position + Vector3.right * dir * distance;
            pu.coverBox.transform.position = transform.position;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        // if(collision.gameObject.CompareTag("Player")) collision.GetComponent<PlayerUnit>().isHide = false;
    }
}
