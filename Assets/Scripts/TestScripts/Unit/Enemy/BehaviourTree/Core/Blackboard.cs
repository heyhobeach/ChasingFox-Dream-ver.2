using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace BehaviourTree
{
    [Serializable]
    public class Blackboard
    {
        public EnemyUnit thisUnit;
        public Transform target;
        public PlayableDirector playableDirector;
        [Serializable]
        public class Enemy_State//적군 상태값을 이너 클래스로 표현 중첩되는 표현 사용시 enum으로 표현 안 될것 같아 해당 방식 사용
        {
            // public bool Defalut=true;//생성시 기준 생성시 defalut는 true기 때문에
            public enum StateCase { Default, Alert, Chase }
            public StateCase stateCase;
            public bool recognition = true;
            public int Increase_Sight = 1;

            public void Reset_State()//모든 상태를 false로 전환
            {
                // this.Defalut = true;//생성시 기준 생성시 defalut는 true기 때문에
                this.stateCase = StateCase.Default;
                this.recognition = true;
                this.Increase_Sight = 1;
            }

            // public bool[] Get_State()
            // {
            //     bool[] states = new bool[3];
            //     // states[0] = Defalut;
            //     states[0] = Tracking;
            //     states[1] = Missing;
            //     states[2] = Increase_Sight;
            //     return states;
            // }
        }
        public Enemy_State enemy_state;
        public List<GameManager.Node> FinalNodeList = new();
    }
}
