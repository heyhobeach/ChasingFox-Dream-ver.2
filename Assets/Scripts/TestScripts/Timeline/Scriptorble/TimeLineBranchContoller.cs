using UnityEngine;
using UnityEngine.Playables;

public class TimeLineBranchContoller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void branchfunc(TimeLineBranchScriptorble branchscriptorble)
    {
        this.gameObject.GetComponent<PlayableDirector>().Stop();    
        //
    }
}
