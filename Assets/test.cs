using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(testcor());
    }

    // Update is called once per frame
    IEnumerator testcor()
    {

        int count = 0;

        while (true)
        {
            Debug.Log(count);
            yield return new WaitForSeconds(1.0f);
            count++;
        }
    }
}
