using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LoopController : MonoBehaviour
{
    PlayableDirector playableDirector;
    PlayableDirector LoopDir;

    [SerializeField] private float targetFrameRate = 1 / 60f;
    [SerializeField] private PlayableDirector director;


    public void SetNone()
    {
        this.playableDirector.extrapolationMode = DirectorWrapMode.None;
    }
    public void SetLoop(PlayableDirector dir)
    {
        Debug.Log($"SetLoop - Current Frame: {playableDirector.time}");
        LoopDir = dir;

        //playableDirector.time=
        playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);

        playableDirector.Pause();
        LoopDir.Play();

    }

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
