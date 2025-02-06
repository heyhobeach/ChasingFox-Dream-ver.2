using Cinemachine;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class NotesMarker : SignalEmitter
{
    public Color color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    public bool showLineOverlay = false;
}