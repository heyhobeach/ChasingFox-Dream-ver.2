using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniEventTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void EndShootAni()
    {
        Debug.Log("슛팅 끝");
        gameObject.SetActive(false);
    }
}
