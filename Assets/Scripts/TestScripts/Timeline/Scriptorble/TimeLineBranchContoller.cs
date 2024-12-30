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
    public void branchfunc(TimeLineBranchScriptorble branchscriptorble)
    {
        PlayableDirector currentDirector = this.gameObject.GetComponent<PlayableDirector>();
        currentDirector.Stop();
        currentObj.SetActive(false);
        branch1Obj.SetActive(true);
        //branchscriptorble.branch1.
        //currentDirector.playableAsset = branchscriptorble.branch1;
        //currentDirector.Play();

        branch1Timeline.GetComponent<PlayableDirector>().Play();
        //
    }
}
