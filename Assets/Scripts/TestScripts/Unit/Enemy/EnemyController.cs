using System;
using BehaviourTree;
using UnityEngine;
using UnityEngine.Playables;

public class EnemyController : MonoBehaviour
{
    public BehaviourTree.BehaviourTree behaviorTree;
    public PlayableDirector playableDirector;
    private Blackboard blackboard;

    public float layDistance;
    public float appendDistance;
    private float distance { get => layDistance*(blackboard.enemy_state.Increase_Sight*appendDistance); }
    private Vector2 subvec;//ai거리관련

    void Start()
    {
        behaviorTree.blackboard.thisUnit = GetComponent<EnemyUnit>();
        behaviorTree.blackboard.playableDirector = playableDirector;
        // behaviorTree.blackboard.playableDirector = GetComponent<PlayableDirector>();
        behaviorTree = behaviorTree.Clone();
        blackboard = behaviorTree.blackboard;
    }
    void Update()
    {
        CircleRay();
        behaviorTree.Update();
    }

    private void CircleRay()//유저 탐색할 레이 관련 함수
    {
        // float maxDistance = 5f;
        // float mysize = 5f;//반지름
        int layerMask = 1 << LayerMask.NameToLayer("DeadEnemy") | 1 << LayerMask.NameToLayer("GunSound");//enemy와 gunsound 객체 총알이 만약 바닥에 박히면 gunsound객체를 생성했다가 일정시간 이후 지우는식
        RaycastHit2D ray2d = Physics2D.CircleCast(transform.position, distance, Vector2.up, 0,layerMask);//죽은 적군 찾는 변수

        Vector2 subpos = GameManager.Instance.player.transform.position - transform.position;
        int linelayerMask = 1 << LayerMask.NameToLayer("Player");
        RaycastHit2D[] target_ray2d = Physics2D.RaycastAll(transform.position, subpos, distance, linelayerMask);
        int index_player = Array.FindIndex(target_ray2d, x => x.transform.tag == "Player");

        if (ray2d && blackboard.enemy_state.stateCase == Blackboard.Enemy_State.StateCase.Default)//추격상태가 아니고 죽은 
        {
            blackboard.enemy_state.stateCase = Blackboard.Enemy_State.StateCase.Alert;
            blackboard.target = ray2d.transform;
            // if (ray2d.transform.gameObject.layer == 15) // 이거 뭐임
            // {
            //     SetFollow();
            //     return;
            // }
            // StartCoroutine(timer());
        }
        
        if (index_player>-1)//ray2d,여기 !follow해도 될것 같앗음
        {
            blackboard.enemy_state.stateCase = Blackboard.Enemy_State.StateCase.Chase;
            blackboard.target = GameManager.Instance.player.transform;
            subvec = (Vector2)target_ray2d[index_player].transform.position - (Vector2)transform.position;// ray2d=>tartget_ray2d[index_player]
            float deg = Mathf.Atan2(subvec.y, subvec.x);//mathf.de
            deg *= Mathf.Rad2Deg;
            // if (attackRange <= subvec.magnitude) {}
            // else//범위 안에 들어왔을때
            // {
            //     // if (!behaviorTree.blackboard.enemy_state.Tracking)//이미 레이 = 시야에 감지 되었기에 계속 추격해야함
            //     // {
            //     //     StartCoroutine(timer());
            //     // }
            // }
        }
    }

    // public int index_player = 0;
    // bool attacking;
    // public float attackRange;
    // IEnumerator timer()
    // {
    //     // behaviorTree.blackboard.enemy_state.Tracking = true;
    //     // behaviorTree.blackboard.enemy_state.Defalut = false;
    //     while (behaviorTree.blackboard.thisUnit.UnitState != UnitState.Death)
    //     {
    //         yield return new WaitForSeconds(1);
    //         if ((subvec.magnitude <= attackRange)&(index_player==0))//만약 벽이 2개 겹처있는 경우는 체크 안되어있음, 공격부분
    //         {
    //             if (Move_Cor != null)
    //             {
    //                 StopCoroutine(Move_Cor);
    //             }
    //             attacking = true;
    //             // Attack((player.transform.position + transform.position).normalized);
    //         }
    //         else//추격부분
    //         {
    //             // SetFollow();
    //         }
    //         // if (!behaviorTree.blackboard.enemy_state.Missing)//못찾는 상태일때 트래킹을 끄기위해
    //         // {
    //         //     behaviorTree.blackboard.enemy_state.Tracking = true;
    //         // }
    //         // else
    //         // {
    //         //     behaviorTree.blackboard.enemy_state.Tracking = false;
    //         // }

            
    //     }
        
    // }
}
