using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsGrounded : QTE_Prerequisites
{
    public override bool isSatisfied { get => playerUnit.IsGrounded; set {} }

    private PlayerUnit playerUnit;

    void Start() => playerUnit = GetComponent<PlayerUnit>();
}
