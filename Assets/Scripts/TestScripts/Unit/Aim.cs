using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    void OnEnable()
    {
        // Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void Update()
    {
        var pos = Input.mousePosition;
        // var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        gameObject.transform.position = pos;
    }
}
