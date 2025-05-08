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
        var canHit = blackboard.thisUnit.AttackCheck(blackboard.target.transform.position);
        switch(distanceType)
        {
            case DistanceType.Less:
            if(distanceA > targetDistance && canHit) return child.Update();
            break;
            case DistanceType.Greater:
            if(distanceA < targetDistance || !canHit) return child.Update();
            break;
            case DistanceType.Between:
            if(distanceA < targetDistance && distanceB > targetDistance && canHit) return child.Update();
            break;
        }
        return NodeState.Failure;
    }
}