using UnityEngine;
using UnityEngine.Playables;

public class TimeLineBranchContoller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject branch1Obj;
    public GameObject branch2Obj;
    public GameObject currentObj;

    public GameObject branch1Timeline;
    public GameObject branch2Timeline;

    [SerializeField]
    private int branch_brutality = 50;
    public void branchfunc(TimeLineBranchScriptorble branchscriptorble)
    {

        PlayableDirector currentDirector = this.gameObject.GetComponent<PlayableDirector>();

        currentDirector.Stop();
        currentObj.SetActive(false);

        //branchscriptorble.branch1.
        //currentDirector.playableAsset = branchscriptorble.branch1;
        //currentDirector.Play();
        if(branch_brutality > GameManager.Brutality)
        {
            branch1Obj.SetActive(true);
            branch1Timeline.SetActive(true);
            branch1Timeline.GetComponent<PlayableDirector>().Play();
        }
        else
        {
            branch2Obj.SetActive(true);
            branch2Timeline.SetActive(true);
            branch2Timeline.GetComponent<PlayableDirector>().Play();
        }

        //
    }
}
