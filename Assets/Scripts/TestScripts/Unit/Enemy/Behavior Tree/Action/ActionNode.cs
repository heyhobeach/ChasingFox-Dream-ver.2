using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviorTree
{
    [CreateAssetMenu(fileName = "ActionNode", menuName = "ScriptableObjects/BehaviorTree/ActionNode")]
    public abstract class ActionNode : BehaviorNode
    {
        public List<UnityAction> unityActions;
    }
}
