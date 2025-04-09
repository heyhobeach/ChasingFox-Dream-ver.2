using BehaviourTree;
using UnityEngine;

public class DistanceCheck : DecoratorNode
{
    public enum DistanceType { Less, Greater }
    public DistanceType distanceType;
    public float distance;

    protected override void OnEnd() { }

    protected override void OnStart() { }

    protected override NodeState OnUpdate()
    {
        var targetDistance = ((Vector2)blackboard.target.position - (Vector2)blackboard.thisUnit.transform.position).magnitude;
        switch(distanceType)
        {
            case DistanceType.Less:
            if(distance > targetDistance) return child.Update();
            break;
            case DistanceType.Greater:
            if(distance < targetDistance) return child.Update();
            break;
        }
        return NodeState.Failure;
    }
}