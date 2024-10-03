using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class GaugeBar<T> : MonoBehaviour
{
    public delegate void GaugeUpdateDel(float currentNum, float totalNum);

    public T target;

    public Image baseImg;
    public Image gaugeImg;

    protected virtual void GaugeUpdate(float currentNum, float totalNum) => gaugeImg.fillAmount = currentNum / totalNum;
}
