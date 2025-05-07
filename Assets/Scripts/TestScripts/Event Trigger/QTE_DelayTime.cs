using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTE_DelayTime : QTE_Prerequisites
{
    public override bool isSatisfied 
    { 
        get
        {
            if(!isStarted) 
            {
                isStarted = true;
                time = 0;
            }
            return delayTime <= time;
        }
         set => throw new System.NotImplementedException(); 
    }

    public float delayTime = 0.5f;
    [SerializeField, DisableInInspector] private float time = 0;
    private bool isStarted = false;

    private void Update()
    {
        if(!isStarted || delayTime<=time) return;
        time += ServiceLocator.Get<GameManager>().ingameDeltaTime;
    }
}
