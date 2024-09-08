using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanCrossHair : GaugeBar<Human>
{
    void Awake()
    {
        target.onEnable += () => baseImg.gameObject.SetActive(true);
        target.onDisable += () => baseImg.gameObject.SetActive(false);
        target.reloadGauge += GaugeUpdate;
    }

    protected override void GaugeUpdate(float currentNum, float totalNum)
    {
        base.GaugeUpdate(currentNum, totalNum);
        if(gaugeImg.fillAmount <= 0) baseImg.color = Color.red;
        else baseImg.color = Color.white;
    }
}
