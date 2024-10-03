using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class PauseAnimatorPlayableAsset : PlayableAsset
{
    public ExposedReference<Animator> animator;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<PauseAnimatorPlayable>.Create(graph);

        PauseAnimatorPlayable behaviour = playable.GetBehaviour();
        behaviour.animator = animator.Resolve(graph.GetResolver());

        return playable;
    }
}