using UnityEngine;

public class MoveBlendHandler : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        SetAni(animator, layerIndex);
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        SetAni(animator, layerIndex);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        SetAni(animator, layerIndex);
    }

    private void SetAni(Animator animator, int layerIndex)
    {
        animator.Play(GetName(Calc(animator.GetFloat("hzForce"))), layerIndex);
    }

    private float Calc(float origin)
    {
        if(origin > 0.8f) return 0.8f;
        else if(origin > 0.6f) return 0.5f;
        else if(origin > 0.5f) return 0.2f;
        else if(origin > 0.2f) return 0.2f;
        else if(origin >= 0) return 0;
        else return -1;
    }

    private string GetName(float value)
    {
        if(value > 0.8f) return "Deceleration 4";
        else if(value > 0.6f) return "Deceleration 3";
        else if(value > 0.5f) return "Deceleration 2";
        else if(value > 0.2f) return "Deceleration 1";
        else return "Acceleration";
    }
}
