using UnityEngine;

public class IsMapClear : QTE_Prerequisites
{
    public override bool isSatisfied { get => map.enemyClear; set {} }

    private Map map;

    void Start() => map = GetComponent<Map>();
}
