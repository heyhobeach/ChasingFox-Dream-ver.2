using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PauseAnimatorPlayable : PlayableBehaviour
{
    public Animator animator;
    public AnimationTrack animationTrack;

    private RuntimeAnimatorController animatorController;

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
}