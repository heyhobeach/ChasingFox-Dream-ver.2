using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineEnd : QTE_Prerequisites
{
    FixedEventTrigger fixedEventTrigger;
    PlayableDirector playableDirector;
    bool isPlaying;
    public override bool isSatisfied 
    { 
        get
        {
            if(fixedEventTrigger == null) Init();
            Debug.Log(playableDirector.state);
            return fixedEventTrigger.used ? true : isPlaying; 
        }
        set => isPlaying = value; 
    }

    private void Init()
    {
        playableDirector = GetComponent<PlayableDirector>();
        fixedEventTrigger = GetComponent<FixedEventTrigger>();
        playableDirector.stopped += (x) => isPlaying = true;
    }

    void Start()
    {
        Init();
    }

    [ContextMenu("PDPlay")]
    public void PDPLay() => playableDirector.Play();
}
