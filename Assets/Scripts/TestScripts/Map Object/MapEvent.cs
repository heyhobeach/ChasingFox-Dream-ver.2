using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class MapEvent : IComparable<MapEvent>
{
    public float time;
    public UnityEvent action;

    public int CompareTo(MapEvent other)
    {
        if (other == null) return 1;
        return this.time.CompareTo(other.time);
    }
}