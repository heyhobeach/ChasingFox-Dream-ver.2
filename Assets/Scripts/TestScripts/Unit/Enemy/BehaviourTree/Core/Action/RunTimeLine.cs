using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace BehaviourTree
{
    public class RunTimeLine : ActionNode
    {
        private bool isCan;

        private PlayableDirector playableDirector;

        private void OnEnable()
        {
            playableDirector = blackboard.playableDirector;
            isCan = true;
            playableDirector.stopped -= OnTimeLineStoped;
            playableDirector.stopped += OnTimeLineStoped;
        }
        protected override void OnEnd() {}

        protected override void OnStart()
        {
            if(playableDirector != null && isCan && playableDirector.state != PlayState.Playing)
            {
                playableDirector.Play();
            }
        }

        protected override NodeState OnUpdate()
        {
            if(playableDirector == null || !isCan || playableDirector.state != PlayState.Playing)
            {
                if(playableDirector != null && playableDirector.state == PlayState.Playing) TimeLineStop();
                return NodeState.Failure;
            }
            else return NodeState.Success;
        }

        private void OnTimeLineStoped(PlayableDirector pd)
        {
            isCan = false;
            playableDirector.enabled = false;
            GameManager.Instance.interactionEvent._DisableUI();
        }
        [MesageTarget] public void TimeLineStop()
        {
            if(playableDirector != null) playableDirector.Stop();
        }
    }
}