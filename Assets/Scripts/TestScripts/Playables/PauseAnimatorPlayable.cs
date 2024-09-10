using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PauseAnimatorPlayable : PlayableBehaviour
{
    public Animator animator;
    public AnimationTrack animationTrack;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (animator != null) animator.speed = 0f;
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (animator != null) animator.speed = 1f;
    }
}