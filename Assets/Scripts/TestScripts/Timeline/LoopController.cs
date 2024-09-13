using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LoopController : MonoBehaviour
{
    PlayableDirector playableDirector;
    PlayableDirector LoopDir;


    public void EndLoop()
    {
        LoopDir.Stop();
        playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
        playableDirector.Resume();
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
