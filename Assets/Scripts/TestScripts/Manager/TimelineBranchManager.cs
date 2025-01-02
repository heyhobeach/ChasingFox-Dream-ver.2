using UnityEngine;

public class TimelineBranchManager : MonoBehaviour
{
    public TimeLineBranchContoller[] timeLineBranches;
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

    public void TimelineBranch(int branch_id)
    {
        timeLineBranches[branch_id-1].branchfunc();
    }
}
