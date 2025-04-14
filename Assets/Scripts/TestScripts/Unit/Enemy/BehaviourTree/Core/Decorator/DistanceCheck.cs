using BehaviourTree;
using UnityEngine;

public class DistanceCheck : DecoratorNode
{
    public enum DistanceType { Less, Greater, Between }
    public DistanceType distanceType;
    public bool snapshot;
    public float distanceA;
    public float distanceB;

    private float targetDistance;

    protected override void OnEnd() { }

    protected override void OnStart() { targetDistance = Vector2.Distance((Vector2)blackboard.target.position, (Vector2)blackboard.thisUnit.transform.position); }

    protected override NodeState OnUpdate()
    {
        if(!snapshot) targetDistance = Vector2.Distance((Vector2)blackboard.target.position, (Vector2)blackboard.thisUnit.transform.position);
        var pos = blackboard.target.CompareTag("Player") ? blackboard.target.transform.position + Vector3.up : blackboard.target.transform.position;
        var hit = blackboard.thisUnit.AttackCheck(pos);
        switch(distanceType)
        {
            case DistanceType.Less:
            if(distanceA > targetDistance && hit) return child.Update();
            break;
            case DistanceType.Greater:
            if(distanceA < targetDistance || !hit) return child.Update();
            break;
            case DistanceType.Between:
            if(distanceA < targetDistance && distanceB > targetDistance && hit) return child.Update();
            break;
        }
        return NodeState.Failure;
    }
}