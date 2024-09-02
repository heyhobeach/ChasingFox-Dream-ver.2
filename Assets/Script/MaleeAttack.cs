using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaleeAttack : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Hatch")
        {
            collision.gameObject.SetActive(false);
        }
    }

}
