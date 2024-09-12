using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LoopController : MonoBehaviour
{
    PlayableDirector playableDirector;
    void SetLoop()
    {
        playableDirector.Stop();
    }
    // Start is called before the first frame update
    void Start()
    {
        playableDirector=GetComponent<PlayableDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
