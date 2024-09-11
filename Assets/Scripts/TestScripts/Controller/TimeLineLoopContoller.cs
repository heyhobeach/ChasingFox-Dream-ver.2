using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineLoopContoller : MonoBehaviour
{
    // Start is called before the first frame update
    PlayableDirector playableDirector;
    PlayableDirector idleDirector;
    private void Awake()
    {
        playableDirector = GetComponent<PlayableDirector>();
        idleDirector= GetComponent<PlayableDirector>();
    }

    public void IdleTimeLine(PlayableDirector dir)//인자로 받은 idle 타임라인
    {
        Debug.Log("IdleTimeLine");
        playableDirector.Pause();
        playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
        idleDirector = dir;
        idleDirector.Play();
    }

    public void ContinueTimeLine()//재생
    {
        idleDirector.Stop();
        playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
        playableDirector.Play();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)){
            ContinueTimeLine();
        }
    }

}
