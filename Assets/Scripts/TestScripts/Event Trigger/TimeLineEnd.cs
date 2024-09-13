using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineEnd : QTE_Prerequisites
{
    PlayableDirector playableDirector;
    bool isPlaying;
    public override bool isSatisfySatisfy { get => isPlaying; set => isPlaying = value; }

    void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
        playableDirector.stopped += (x) => isPlaying = true;
    }

    [ContextMenu("PDPlay")]
    public void PDPLay() => playableDirector.Play();
}
