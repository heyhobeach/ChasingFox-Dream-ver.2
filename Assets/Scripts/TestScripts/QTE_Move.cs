using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTE_Move : QTE_Prerequisites
{
    public override bool isSatisfySatisfy { get => isArrival; set => isArrival = value; }

    private bool isArrival;
    public Vector3 targetPos;

    // Update is called once per frame
    void Update()
    {
        if((transform.position - targetPos).magnitude < 0.3f) isArrival = true;
        else isArrival = false;
    }
}
