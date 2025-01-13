using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class EventList
{
    public QTE_Prerequisites enterPrerequisites;
    public KeyCode keyCode;
    public UnityEvent action;
    public QTE_Prerequisites exitPrerequisites;
}
