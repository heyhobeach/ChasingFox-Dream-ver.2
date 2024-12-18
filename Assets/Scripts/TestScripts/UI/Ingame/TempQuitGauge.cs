using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempQuitGauge : MonoBehaviour
{
    [SerializeField] private Image gauge;

    [SerializeField] private float holdTime = 1.5f;
    private float holdingTime;

    private void Start() => StartCoroutine(MyUpdate());

    // Update is called once per frame
    private IEnumerator MyUpdate()
    {
        while(true)
        {
            if(Input.GetKey(KeyCode.Escape) && holdingTime < holdTime) holdingTime += Time.unscaledDeltaTime;
            else holdingTime -= Time.unscaledDeltaTime;
            if(holdingTime < 0) holdingTime = 0;
            gauge.fillAmount = holdingTime;
            if(holdingTime >= holdTime) Application.Quit();

            yield return new WaitForEndOfFrame();
        }
    }
}
