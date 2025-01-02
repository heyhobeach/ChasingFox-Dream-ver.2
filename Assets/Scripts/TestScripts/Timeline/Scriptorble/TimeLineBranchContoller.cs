using UnityEngine;
using UnityEngine.Playables;

public class TimeLineBranchContoller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject upBranchobj;//up
    public GameObject downBranchobj;//donw
    public GameObject currentObj;

    public PlayableDirector upBranchTimeline;
    public PlayableDirector downBranchTimeline;

    [SerializeField]
    private int branch_brutality = 50;
    public void branchfunc(TimeLineBranchScriptorble branchscriptorble)
    {

        PlayableDirector currentDirector = this.gameObject.GetComponent<PlayableDirector>();

        currentDirector.Stop();//여기 부분 none으로 해야하는가?
        currentObj.SetActive(false);

        //branchscriptorble.branch1.
        //currentDirector.playableAsset = branchscriptorble.branch1;
        //currentDirector.Play();
        if(branch_brutality > GameManager.Brutality)
        {
            downBranchobj.SetActive(true);
            downBranchTimeline.gameObject.SetActive(true);
            downBranchTimeline.Play();
        }
        else
        {
            upBranchobj.SetActive(true);
            upBranchTimeline.gameObject.SetActive(true);
            upBranchTimeline.Play();
        }

        //
    }
}
