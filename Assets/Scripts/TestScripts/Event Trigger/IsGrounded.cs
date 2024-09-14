using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsGrounded : QTE_Prerequisites
{
    public override bool isSatisfySatisfy { get => isGrounded; set => isGrounded = value; }

    private bool isGrounded;
    private PlayerUnit playerUnit;

    void Start() => playerUnit = GetComponent<PlayerUnit>();

    void Update() => isGrounded = playerUnit.IsGrounded;
}
