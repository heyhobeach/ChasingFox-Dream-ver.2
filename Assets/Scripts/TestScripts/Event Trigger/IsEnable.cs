using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsEnable : QTE_Prerequisites
{
    public override bool isSatisfied { get => isEnable; set => isEnable = value; }

    private bool isEnable;

    void OnEnable() => isEnable = true;
    void OnDisable() => isEnable = false;
}
