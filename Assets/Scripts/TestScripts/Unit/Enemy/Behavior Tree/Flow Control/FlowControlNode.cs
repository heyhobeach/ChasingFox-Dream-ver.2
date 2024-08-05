using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public abstract class FlowControlNode : BehaviorNode
    {
        
        protected NodeState nodeState;
        public List<BehaviorNode> behaviorNodes = new();
        protected IEnumerator<BehaviorNode> runningNode;

        public virtual void Reset()
        {   
            runningNode = behaviorNodes.GetEnumerator();
            if(runningNode.Current != null && runningNode.Current is FlowControlNode) ((FlowControlNode)runningNode.Current).Reset();
        }

        private void Awake() => runningNode = behaviorNodes.GetEnumerator();

        protected static System.Random random = new System.Random();
    
        protected static void Shuffle(List<BehaviorNode> nodes)
        { 
            for (int i = nodes.Count - 1; i > 0; i--) 
            {
                int k = random.Next(i + 1);
                var value = nodes[k];
                nodes[k] = nodes[i];
                nodes[i] = value;
            }
        }
    }
}
