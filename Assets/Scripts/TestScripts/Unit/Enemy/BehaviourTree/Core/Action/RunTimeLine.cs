using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace BehaviourTree
{
    public class RunTimeLine : ActionNode
    {
        public bool isCan;

        private void OnEnable()
        {
            blackboard.playableDirector.stopped -= OnTimeLineStoped;
            blackboard.playableDirector.stopped += OnTimeLineStoped;
            isCan = true;
        }
        protected override void OnEnd() {}

        protected override void OnStart()
        {
            Debug.Log(isCan + ", " + (blackboard.playableDirector.state != PlayState.Playing));
            if(blackboard.playableDirector != null && isCan && blackboard.playableDirector.state != PlayState.Playing)
            {
                blackboard.playableDirector.Play();
            }
        }

        protected override NodeState OnUpdate()
        {
            if(isCan) return NodeState.Success;
            else return NodeState.Failure;
        }

        private void OnTimeLineStoped(PlayableDirector playableDirector) => isCan = false;
        public void TimeLineStop() => blackboard.playableDirector.Stop();
    }
}