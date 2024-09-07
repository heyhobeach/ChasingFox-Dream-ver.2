using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrutalCrossHair : GaugeBar<Werewolf>
{
    void Awake()
    {
        target.onEnable += () => baseImg.gameObject.SetActive(true);
        target.onDisable += () => baseImg.gameObject.SetActive(false);
        target.brutalGauge += GaugeUpdate;
    }
}
