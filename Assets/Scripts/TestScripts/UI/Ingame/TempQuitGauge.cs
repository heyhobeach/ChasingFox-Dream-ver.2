using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempQuitGauge : MonoBehaviour
{
    [SerializeField] private Image gauge;

    [SerializeField] private float holdTime = 1.5f;
    private float holdingTime;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Escape)) holdingTime += Time.deltaTime;
        else holdingTime -= Time.deltaTime;
        if(holdingTime < 0) holdingTime = 0;
        gauge.fillAmount = holdingTime;
        if(holdingTime >= holdTime) Application.Quit();
    }
}
