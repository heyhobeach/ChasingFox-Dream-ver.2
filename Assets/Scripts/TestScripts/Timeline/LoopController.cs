using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class LoopController : MonoBehaviour
{
    PlayableDirector playableDirector;
    PlayableDirector LoopDir;
    public double time = 0;

    [SerializeField] private float targetFrameRate = 1 / 60f;
    [SerializeField] private PlayableDirector director;
    public TimelineAsset timeline;
    static int timeListNum = 0;

    /// <summary>
    /// 정지할 시간
    /// </summary>
    [SerializeField] private List<double> stop_time = new List<double>();

    public void setTime(float t)
    {
        time = (double)t;
    }

    public void SetNone()
    {
        this.playableDirector.extrapolationMode = DirectorWrapMode.None;
    }
    public void SetLoop(PlayableDirector dir)
    {
        Debug.Log($"SetLoop - Current Frame: {playableDirector.time}");
        double timeLineT = stop_time[timeListNum];
        Debug.Log("Time test" + timeLineT + "arg Time" + time);
        LoopDir = dir;

        //mark
        //SignalAsset.

        //playableDirector.time=

        var a =playableDirector.duration;
        Debug.Log("길이 " + a);
        //SetNone();
        playableDirector.time = timeLineT;
        playableDirector.playableGraph.GetRootPlayable(0).SetDuration(timeLineT);

        Debug.Log("테스트 후 길이 " + timeLineT);

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
        timeListNum++;
        //playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
        playableDirector.playableGraph.GetRootPlayable(0).SetDuration(23.116666666666);
        //playableDirector.Resume();
    }

    // Start is called before the first frame update
    void Start()
    {

        playableDirector =GetComponent<PlayableDirector>();
        foreach (var track in timeline.GetOutputTracks())
        {
            // 트랙에서 마커를 찾습니다.
            foreach (var marker in track.GetMarkers())
            {


                if (marker is SignalEmitter signalEmitter)
                {
                    // 마커의 시간을 가져옵니다.

                    double markerTime = signalEmitter.time;
                    //Debug.Log("Marker Time: " + markerTime);
                    if (signalEmitter.name == "Stop")
                    {
                        //Debug.Log("Retroactive Signal Found at Time: " + signalEmitter.time);//딱 정지해야하는 부분을 찾을수는 있음
                        stop_time.Add(signalEmitter.time);
                    }
                    if (signalEmitter.retroactive)
                    {
                        //Debug.Log("Retroactive Signal Found at Time: " + signalEmitter.time);//딱 정지해야하는 부분을 찾을수는 있음
                    }
                }
                else
                {
                    //Debug.Log("can't find marker");
                }
            }
        }
        stop_time.Sort();
        foreach(var time in stop_time)//테스트용
        {
            //Debug.Log("Retroactive Signal Found at Time: " + time);//딱 정지해야하는 부분을 찾을수는 있
        }
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
