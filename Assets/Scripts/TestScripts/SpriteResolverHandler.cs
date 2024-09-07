using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class SpriteResolverHandler : StateMachineBehaviour
{
    private SpriteResolver spriteResolver;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        if(spriteResolver) spriteResolver = animator.GetComponent<SpriteResolver>();
        if(spriteResolver && spriteResolver.enabled) spriteResolver.enabled = false;
    }
}
