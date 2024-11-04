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


    public int fixed_timeline_frame = 30;
    [SerializeField] private PlayableDirector director;
    public TimelineAsset timeline;

    /// <summary>
    /// 각각 리스트별로 인덱스
    /// </summary>
    private int stopListNum = 0;
    private int loopListNum = 0;
    private int holdListNum = 0;

    /// <summary>
    /// 현재 상태가 홀드인지 루프인지 판단위한 변수
    /// </summary>
    [SerializeField] private int isHold = 0;

    /// <summary>
    /// 정지할 시간
    /// </summary>
    [SerializeField] private List<double> stop_time = new List<double>();
    /// <summary>
    /// 루프 시작 시간
    /// </summary>
    [SerializeField] private List<double> loop_time = new List<double>();

    /// <summary>
    /// 루프 정지아닌 hold 포인트 해당 포인트는 다른 타임라인으로 이동시 메인 타임라인의 진행상황을 잡고 있기위함
    /// </summary>
    [SerializeField] private List<double> hold_time = new List<double>();

    public GameObject backgroundImage;

    public void setTime(float t)
    {
        time = (double)t;
    }

    public void SetNone()
    {
        this.playableDirector.extrapolationMode = DirectorWrapMode.None;
        ResetValue();
        //Application.targetFrameRate = 60;
        //QualitySettings.vSyncCount = 1;
    }

    public void ResetValue()//끝날때 다시 원래대로초기화 하는 함수
    {
        stopListNum = 0;
        loopListNum = 0;
        holdListNum = 0;
        stop_time.Clear();
        loop_time.Clear();
        hold_time.Clear();  
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
    }

    public void SetSkip()
    {
        InteractionEvent.Instance.SetSkip(true);
    }

    public void EndLoop()
    {
        backgroundImage.SetActive(false);
        if (isHold == 2)
        {
            Debug.Log("isHold");
            isHold = 0;
            holdListNum++;
            playableDirector.playableGraph.GetRootPlayable(0).SetDuration(playableDirector.duration);
            return;
        }
        if (isHold == 1)
        {
            Debug.Log("end loop");
            //LoopDir.Stop();
            isHold = 0;
            stopListNum++;
            loopListNum++;
            //playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
            playableDirector.playableGraph.GetRootPlayable(0).SetDuration(playableDirector.duration);
            //playableDirector.Resume();
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = fixed_timeline_frame;
        playableDirector = GetComponent<PlayableDirector>();
        //playableDirector.Evaluate();
        //playableDirector.RebuildGraph();
        //playableDirector.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
        foreach (var track in timeline.GetOutputTracks())
        {
            // 트랙에서 마커를 찾습니다.
            foreach (var marker in track.GetMarkers())
            {

                if (marker is SignalEmitter pointSignal)
                {
                    if (pointSignal.name == "StartPoint")
                    {
                        loop_time.Add(pointSignal.time);
                    }
                }

                if (marker is SignalEmitter stopSignal)
                {
                    if (stopSignal.name == "Stop")
                    {
                        stop_time.Add(stopSignal.time);
                    }
                }
                if (marker is SignalEmitter holdSignal)
                {
                    if (holdSignal.name == "Hold")
                    {
                        hold_time.Add(holdSignal.time);
                    }
                }
            }
        }
        stop_time.Sort();
        loop_time.Sort();
        hold_time.Sort();
    }
    private void Awake()
    {

    }
    private void FixedUpdate()
    {
        //if (playableDirector.time >= stop_time[stopListNum])

        //Debug.Log("deltaTime" + Time.deltaTime);
        //playableDirector.time += Time.deltaTime;
        //playableDirector.Evaluate();

        if(stop_time.Count > 1) //추가한부분
        {
            if (playableDirector.time >= stop_time[stopListNum] - (1 / fixed_timeline_frame / 2))//갑자기 루프타임라인 부분 다 지우니까 현재 부분 문제 발생, 확인해본바 stop_time이 0인곳이 생겨서 생김 따라서 위에 if를 추가했음
            {
                isHold = 1;
                double loopLineT = loop_time[loopListNum];//루프 시작 시간
                var a = playableDirector.duration;
                playableDirector.time = loopLineT;
                backgroundImage.SetActive(true);
            }
        }

   

        if (playableDirector.time >= hold_time[holdListNum])//현재 이 부분에서 자꾸 빨간 에러 뜨는데 컴파일러 상에서는 문제없어보임 나중에 빌드로 확인 필요 2024.10.23->loop부분을 다 지우니 에러 없어짐 2024.11.04
        {
            isHold = 2;
            playableDirector.playableGraph.GetRootPlayable(0).SetDuration(hold_time[holdListNum]);
            backgroundImage.SetActive(true);
            //playableDirector.time = hold_time[holdListNum];
        }
    }
    private void Update()
    {
        if (Input.anyKeyDown && !(Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.LeftArrow) ||Input.GetKeyDown(KeyCode.RightArrow)
                                                                  ||Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.D)))
        {
            Debug.Log("anyKeyDonw");
            if (isHold > 2 || isHold < 1)
            {//해당부분은 int로 수정해서 int로 진행할까함 0 = null, 1 = loop, 2 = hold, else error
                return;
            }
            EndLoop();
        }

    }
}
