using System;
using BehaviourTree;
using UnityEngine;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyController : MonoBehaviour
{
    public BehaviourTree.BehaviourTree behaviorTree;
    public PlayableDirector playableDirector;
    private Blackboard blackboard;
    public SpriteRenderer spriteRenderer;

    private Collider2D[] hits = new Collider2D[0];

    public float _viewOuterRange;
    public float viewOuterRange
    {
        get => _viewOuterRange+(blackboard.enemy_state.Increase_Sight*appendDistance);
    }
    public float _viewInnerRange;
    public float viewInnerRange
    {
        get => _viewInnerRange+(blackboard.enemy_state.Increase_Sight*appendDistance);
    }
    public float viewAngle;
    public float appendDistance;
    private float distance { get => viewOuterRange; }

    void Start()
    {
        if(spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        behaviorTree.blackboard.thisUnit = GetComponent<EnemyUnit>();
        var posObj = new GameObject(){
            name = transform.name+" Pos",
            layer = LayerMask.NameToLayer("Ignore Raycast")
        };
        behaviorTree.blackboard.originPos = posObj.transform;
        behaviorTree.blackboard.originPos.position = transform.position;
        behaviorTree.blackboard.playableDirector = playableDirector;
        behaviorTree = behaviorTree.Clone();
        blackboard = behaviorTree.blackboard;
        blackboard.FinalNodeList = null;
    }
    void Update()
    {
        CircleRay();
        behaviorTree.Update();
    }

    private bool ViewCheck(int idx)
    {
        int layerMapMask = 1 << LayerMask.NameToLayer("Map") | 1 << LayerMask.NameToLayer("Wall");
        var subvec = (Vector2)hits[idx].transform.position - (Vector2)transform.position;// ray2d=>tartget_ray2d[index_player]
        float deg = Mathf.Atan2(subvec.y, subvec.x);//mathf.de
        deg *= Mathf.Rad2Deg;
        bool inAngle = 180-viewAngle*0.5f < MathF.Abs(deg) || Mathf.Abs(deg) < viewAngle*0.5f;
        bool isForword = Mathf.Sign(subvec.normalized.x)>0&&!spriteRenderer.flipX ? true : Mathf.Sign(subvec.normalized.x)<0&&spriteRenderer.flipX ? true : false;
        if((subvec.magnitude <= viewInnerRange || (subvec.magnitude <= viewOuterRange && inAngle && isForword))
            && !Physics2D.Raycast(transform.position, subvec.normalized, subvec.magnitude, layerMapMask))
        {                
            return true;
        }
        else return false;
    }

    private void CircleRay()//유저 탐색할 레이 관련 함수
    {
        if(blackboard.thisUnit.UnitState == UnitState.Death)
        {
            blackboard.enemy_state.stateCase = Blackboard.Enemy_State.StateCase.Default;
            return;
        }
        int layerMask = 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("GunSound") | 1 << LayerMask.NameToLayer("Player");//enemy와 gunsound 객체 총알이 만약 바닥에 박히면 gunsound객체를 생성했다가 일정시간 이후 지우는식
        hits = Physics2D.OverlapCircleAll(transform.position, distance, layerMask);//죽은 적군 찾는 변수

        if(hits.Length<=0) return;

        int index_player = -1;
        int index_enemy = -1;
        int index_gunsound = -1;
        int idx = 0;

        foreach(Collider2D ray in hits)
        {
            switch(LayerMask.LayerToName(ray.transform.gameObject.layer))
            {
                case "Player": index_player = idx;
                break;
                case "Enemy": if(hits[idx].transform.GetInstanceID() != blackboard.thisUnit.GetInstanceID() && hits[idx].transform.GetComponent<UnitBase>().UnitState == UnitState.Death) index_enemy = idx;
                break;
                case "GunSound": index_gunsound = idx;
                break;
            }
            idx++;
        }

        if(index_player>-1)
        {
            if(hits[index_player].transform.GetComponent<UnitBase>()?.UnitState == UnitState.Death)
            {
                blackboard.enemy_state.stateCase = Blackboard.Enemy_State.StateCase.Default;
                blackboard.target = null;
            }
            else if(blackboard.enemy_state.stateCase != Blackboard.Enemy_State.StateCase.Chase && blackboard.target != hits[index_player].transform && ViewCheck(index_player))
            {
                blackboard.enemy_state.stateCase = Blackboard.Enemy_State.StateCase.Chase;
                blackboard.target = hits[index_player].transform;
                blackboard.enemy_state.Increase_Sight++;
            }
        }

        if(blackboard.enemy_state.stateCase == Blackboard.Enemy_State.StateCase.Chase) return;

        if(index_enemy>-1 && ViewCheck(index_enemy))
        {               
            blackboard.enemy_state.stateCase = Blackboard.Enemy_State.StateCase.Chase;
            blackboard.target = GameManager.Instance.player.transform;
            blackboard.enemy_state.Increase_Sight++;
        }

        if(blackboard.enemy_state.stateCase == Blackboard.Enemy_State.StateCase.Chase) return;

        if(index_gunsound>-1)
        {
            var subvec = (Vector2)hits[index_gunsound].transform.position - (Vector2)transform.position;
            if(subvec.magnitude <= viewInnerRange + (hits[index_gunsound].bounds.extents.x * 0.5f))
            {
                blackboard.enemy_state.stateCase = Blackboard.Enemy_State.StateCase.Alert;
                blackboard.target = hits[index_gunsound].transform;
                blackboard.enemy_state.Increase_Sight++;
            }
        }
    }

#if UNITY_EDITOR
    public bool showRange;
    void OnDrawGizmos()
    {
        if(!showRange || !EditorApplication.isPlaying) return;

        Handles.color = Color.red;
        Vector3 vec = spriteRenderer.flipX ? Vector3.left : Vector3.right;
        Vector3 startInner = Quaternion.Euler(0, 0, viewAngle*0.5f) * vec * viewInnerRange;
        Vector3 endInner = Quaternion.Euler(0, 0, -viewAngle*0.5f) * vec * viewInnerRange;
        Vector3 startOuter = Quaternion.Euler(0, 0, viewAngle*0.5f) * vec * viewOuterRange;
        Vector3 endOuter = Quaternion.Euler(0, 0, -viewAngle*0.5f) * vec * viewOuterRange;

        Handles.DrawWireArc(transform.position, Vector3.forward, startInner, 360-viewAngle, viewInnerRange);
        Handles.DrawWireArc(transform.position, Vector3.forward, endInner, viewAngle, viewOuterRange);

        Handles.DrawLine(transform.position + startInner, transform.position + startOuter);
        Handles.DrawLine(transform.position + endInner, transform.position + endOuter);
    }
#endif
}