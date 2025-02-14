using UnityEngine;
using UnityEngine.Playables;

public class TimeLineBranchContoller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject upBranchobj;//up
    public GameObject downBranchobj;//donw
    public GameObject currentObj;


    /// <summary>
    /// 여기 밑에 얘네 확인은 아직 안 했는데 시그널 까지 존재 할지는 모르겠음
    /// </summary>
    public PlayableDirector upBranchTimeline;
    public PlayableDirector downBranchTimeline;

    public PlayableDirector targetBranch;

    [SerializeField]
    private int branch_brutality = 50;

    [ContextMenu("Branch")]
    public void branchfunc()
    {

        PlayableDirector currentDirector = this.gameObject.GetComponent<PlayableDirector>();

        //currentDirector.Stop();//여기 부분 none으로 해야하는가?
        currentDirector.time = currentDirector.duration;
        currentDirector.Stop();
        currentObj.SetActive(false);

        //branchscriptorble.branch1.
        //currentDirector.playableAsset = branchscriptorble.branch1;
        //currentDirector.Play();
        //InteractionEvent.Instance.contentNum
        GameObject targetBranchObj;
        PlayableDirector targetBranch;
        Debug.Log("branch 선택지 번호" + InteractionEvent.Instance.user_select);
        Debug.Log("기준 브루탈 수치 확인 " + branch_brutality + "유저 브루탈" + GameManager.Brutality);
        if (InteractionEvent.Instance.user_select==0)
        {
            targetBranchObj = downBranchobj;
            targetBranch = downBranchTimeline;

        }
        else
        {
            targetBranchObj = upBranchobj;
            targetBranch = upBranchTimeline;
        }
        if(branch_brutality< GameManager.Brutality)
        {
            targetBranchObj = upBranchobj;
            targetBranch = upBranchTimeline;
        }

        targetBranchObj.SetActive(true);
        targetBranch.gameObject.SetActive(true);

        if (!targetBranch.transform.parent.gameObject.activeSelf)//혹시 타겟 브런치의 부모가 false일경우 켜기 위함
        {
            targetBranch.transform.parent.gameObject.SetActive(true);
        }

        //currentDirector.gameObject.SetActive(false);
        targetBranch.Play();

        Debug.Log("branch func");

        //
    }
}
