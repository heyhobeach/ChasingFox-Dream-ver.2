using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTrigger : MonoBehaviour
{
    public GameObject UI;
    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if(collider2D.CompareTag("Player")) UI.SetActive(true);
    }
}
