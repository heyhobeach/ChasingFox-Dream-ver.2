using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrutalGauge : GaugeBar<Werewolf>
{
    void Awake() => target.brutalGauge += GaugeUpdate;
}
