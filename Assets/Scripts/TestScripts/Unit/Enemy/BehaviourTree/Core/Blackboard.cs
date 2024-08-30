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
        [DisableInspector] public EnemyUnit thisUnit;
        [DisableInspector] public Transform target;
        [HideInInspector] public PlayableDirector playableDirector;
        [Serializable]
        public class Enemy_State//적군 상태값을 이너 클래스로 표현 중첩되는 표현 사용시 enum으로 표현 안 될것 같아 해당 방식 사용
        {
            // public bool Defalut=true;//생성시 기준 생성시 defalut는 true기 때문에
            public enum StateCase { Default, Alert, Chase }
            [DisableInspector] public StateCase stateCase;
            [DisableInspector] public bool recognition = true;
            [DisableInspector] public int Increase_Sight = 0;

            public void Reset_State()//모든 상태를 false로 전환
            {
                // this.Defalut = true;//생성시 기준 생성시 defalut는 true기 때문에
                this.stateCase = StateCase.Default;
                this.recognition = true;
                this.Increase_Sight = 0;
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
        [HideInInspector] public List<GameManager.Node> FinalNodeList;
        private int _nodeIdx;
        [HideInInspector] public int nodeIdx 
        { 
            get
            {
                if(FinalNodeList != null) return Mathf.Clamp(_nodeIdx, 0, FinalNodeList.Count); 
                else return 0;
            }
            set 
            {
                _nodeIdx = value;
                if(FinalNodeList != null) _nodeIdx = Mathf.Clamp(_nodeIdx, 0, FinalNodeList.Count); 
            }
        }
    }
}
