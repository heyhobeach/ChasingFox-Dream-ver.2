using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.U2D.Animation;

public class PauseAnimatorPlayable : PlayableBehaviour
{
    public Animator animator;
    public AnimationTrack animationTrack;

    private RuntimeAnimatorController animatorController;
    private SpriteResolver spriteResolver;
    private SpriteRenderer spriteRenderer;
    private Sprite previousSprite;
    private string previousCategory;
    private string previousLabel;
    private bool sprResolvercontrol;

    public override void OnGraphStart(Playable playable)
    {
        base.OnGraphStart(playable);
        if (animator != null)
        {
            animator.speed = 0f;
            animatorController = animator.runtimeAnimatorController;
            animator.runtimeAnimatorController = null;
            ShootingAnimationController sac = null;
            if(animator.TryGetComponent<ShootingAnimationController>(out sac)) sac.NomalAni();
        //     spriteRenderer = animator.GetComponent<SpriteRenderer>();
        //     spriteResolver = animator.GetComponent<SpriteResolver>();
        //     previousCategory =  spriteResolver.GetCategory();
        //     previousLabel = spriteResolver.GetLabel();
        //     previousSprite = spriteRenderer.sprite;
        }
    }

    public override void OnGraphStop(Playable playable)
    {
        base.OnGraphStop(playable);
        if (animator != null)
        {
            animator.runtimeAnimatorController = animatorController;
            animator.speed = 1f;
        }
    }

    // public override void PrepareFrame(Playable playable, FrameData info)
    // {
    //     base.PrepareFrame(playable, info);

    //     if(previousSprite != spriteRenderer.sprite)
    //     {
    //         previousSprite = spriteRenderer.sprite;
    //         if(HasSpriteResolverChanged()) sprResolvercontrol = true;
    //         else sprResolvercontrol = false;
    //     }
    //     spriteResolver.enabled = sprResolvercontrol;
    //     if(sprResolvercontrol) spriteResolver.ResolveSpriteToSpriteRenderer();
    // }

    // bool HasSpriteResolverChanged()
    // {
    //     string currentCategory = spriteResolver.GetCategory();
    //     string currentLabel = spriteResolver.GetLabel();

    //     // 카테고리 또는 라벨이 변경되었으면 true 반환
    //     if (currentCategory != previousCategory || currentLabel != previousLabel)
    //     {
    //         // 이전 상태 업데이트
    //         previousCategory = currentCategory;
    //         previousLabel = currentLabel;
    //         return true;
    //     }

    //     return false;
    // }
}