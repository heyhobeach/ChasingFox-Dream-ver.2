using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

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
        double timeLineT = playableDirector.time;
        LoopDir = dir;

        //playableDirector.time=

        var a =playableDirector.duration;
        Debug.Log("길이 " + a);
        //SetNone();
        playableDirector.playableGraph.GetRootPlayable(0).SetDuration(timeLineT);
        Debug.Log("테스트 후 길이 " + a);
        //playableDirector.Pause();
        //LoopDir.Play();

    }
    //public override double duration
    //{
    //    get
    //    {
    //        if (playableDirector == null)
    //            return base.duration;
    //
    //        // use this instead of length to avoid rounding precision errors,
    //        return (double)m_Clip.samples / (double)m_Clip.frequency;
    //    }
    //}

    public void EndLoop()
    {
        LoopDir.Stop();
        //playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
        playableDirector.playableGraph.GetRootPlayable(0).SetDuration(23.116666666666);
        //playableDirector.Resume();
    }

    // Start is called before the first frame update
    void Start()
    {
        playableDirector=GetComponent<PlayableDirector>();
    }
    private void Update()
    {
        if (Input.anyKeyDown&&!Input.GetKeyDown(KeyCode.Escape))
        {
            if(LoopDir==null){
                //Debug.Log("왤케 급하냐 게이야 으하하하");
                return;
            }
            EndLoop();
        }
    }
}
