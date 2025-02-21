using UnityEngine;

public class SpriteResolverHandler : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        animator.Play(stateInfo.fullPathHash, layerIndex);
    }
}
