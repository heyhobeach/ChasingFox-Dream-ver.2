using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LoopController : MonoBehaviour
{
    PlayableDirector playableDirector;
    PlayableDirector LoopDir;
    public void SetLoop(PlayableDirector dir)
    {
        Debug.Log("TEstLoop");
        LoopDir = dir;
        playableDirector.Pause();
        playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
        LoopDir.Play();
    }

    public void EndLoop()
    {
        LoopDir.Stop();
        playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
        playableDirector.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        playableDirector=GetComponent<PlayableDirector>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            EndLoop();
        }
    }
}
