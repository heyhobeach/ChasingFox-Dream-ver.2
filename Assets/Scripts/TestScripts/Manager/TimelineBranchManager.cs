using UnityEngine;
using UnityEngine.Playables;

public class TimelineBranchManager : MonoBehaviour
{
    public GameObject[] timeLineBranches;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private static TimelineBranchManager instance = null;
    public static TimelineBranchManager Instance
    {  
        get 
        {
            if (instance == null) { return null; }
            return instance;
        }
    }
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    public  void TimelineBranch(int branch_id)
    {
        
        timeLineBranches[branch_id-1].GetComponent<TimeLineBranchContoller>().branchfunc();
        //await RestartTimeline(branch_id-1);
        //while(timeLineBranches[branch_id - 1].GetComponent<PlayableDirector>().time == 0)
        //{
        //    Debug.Log("time 0 restart");
        //    timeLineBranches[branch_id - 1].GetComponent<TimeLineBranchContoller>().branchfunc();
        //}
    }
    //while (timeLineBranches[id].GetComponent<PlayableDirector>().time == 0)


}
