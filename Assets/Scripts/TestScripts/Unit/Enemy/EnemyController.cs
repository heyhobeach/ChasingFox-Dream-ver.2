using System;
using BehaviourTree;
using UnityEngine;
using UnityEngine.Playables;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyController : MonoBehaviour
{
    public BehaviourTree.BehaviourTree behaviorTree;
    public PlayableDirector playableDirector;
    public Blackboard blackboard;
    public SpriteRenderer spriteRenderer;

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

    public float soundRange;

    public bool isStop;

    void Start()
    {
        transform.position = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);
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

        ServiceLocator.Get<GameManager>().AddEnemyDeath(EnemyCheck);
        ServiceLocator.Get<GameManager>().AddGunsound(SoundCheck);

        blackboard.thisUnit.onDeath += () => {
            ServiceLocator.Get<GameManager>().DelEnemyDeath(EnemyCheck);
            ServiceLocator.Get<GameManager>().DelGunsound(SoundCheck);
            ServiceLocator.Get<GameManager>().OnEnemyDeath(blackboard.thisUnit);
        };
    }
    // void Update()
    // {
    // }

    void FixedUpdate()
    {
        if(!isStop || ServiceLocator.Get<GameManager>() != null) behaviorTree.Update();
        if(ServiceLocator.Get<GameManager>() != null) CircleRay();
    }

    private bool ViewCheck(Collider2D hit)
    {
        int layerMapMask = 1 << LayerMask.NameToLayer("Map") | 1 << LayerMask.NameToLayer("Wall");
        var subvec = (Vector2)hit.transform.position - (Vector2)(transform.position+Vector3.up);// ray2d=>tartget_ray2d[index_player]
        float deg = Mathf.Atan2(subvec.y, subvec.x);//mathf.de
        deg *= Mathf.Rad2Deg;
        var rayHits = Physics2D.RaycastAll(transform.position+Vector3.up, subvec.normalized, subvec.magnitude, layerMapMask).Where(x => !x.collider.isTrigger);
        bool inAngle = 180-viewAngle*0.5f < MathF.Abs(deg) || Mathf.Abs(deg) < viewAngle*0.5f;
        bool isForword = Mathf.Sign(subvec.normalized.x)>0&&!spriteRenderer.flipX ? true : Mathf.Sign(subvec.normalized.x)<0&&spriteRenderer.flipX ? true : false;
        if((subvec.magnitude <= viewInnerRange || (subvec.magnitude <= viewOuterRange && inAngle && isForword))
            && rayHits.Count() == 0)
        {
            return true;
        }
        else return false;
    }
    private bool ViewCheck(Vector2 pos)
    {
        int layerMapMask = 1 << LayerMask.NameToLayer("Map") | 1 << LayerMask.NameToLayer("Wall");
        var subvec = pos - (Vector2)(transform.position+Vector3.up);// ray2d=>tartget_ray2d[index_player]
        float deg = Mathf.Atan2(subvec.y, subvec.x);//mathf.de
        deg *= Mathf.Rad2Deg;
        var rayHits = Physics2D.RaycastAll(transform.position+Vector3.up, subvec.normalized, subvec.magnitude, layerMapMask).Where(x => !x.collider.isTrigger);
        bool inAngle = 180-viewAngle*0.5f < MathF.Abs(deg) || Mathf.Abs(deg) < viewAngle*0.5f;
        bool isForword = Mathf.Sign(subvec.normalized.x)>0&&!spriteRenderer.flipX ? true : Mathf.Sign(subvec.normalized.x)<0&&spriteRenderer.flipX ? true : false;
        if((subvec.magnitude <= viewInnerRange || (subvec.magnitude <= viewOuterRange && inAngle && isForword))
            && rayHits.Count() == 0)
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
            blackboard.target = null;
            isStop = true;
            return;
        }
        int layerMask = 1 << LayerMask.NameToLayer("Player");//enemy와 gunsound 객체 총알이 만약 바닥에 박히면 gunsound객체를 생성했다가 일정시간 이후 지우는식
        var hits = Physics2D.OverlapCircleAll(transform.position+Vector3.up, distance, layerMask);
        Collider2D hit = null;
        foreach(var h in hits)
        {
            if(h.CompareTag("Player")) 
            {
                hit = h;
                break;
            }
        }

        if(!hit) return;
        if(hit.transform.GetComponent<PlayerUnit>()?.UnitState == UnitState.Death)
        {
            blackboard.enemy_state.stateCase = Blackboard.Enemy_State.StateCase.Default;
            blackboard.target = null;
            isStop = true;
        }
        else if(blackboard.enemy_state.stateCase != Blackboard.Enemy_State.StateCase.Chase && blackboard.target != hit.transform && ViewCheck(hit))
        {
            if(blackboard.thisUnit.UnitState == UnitState.Pause) blackboard.thisUnit.UnitState = UnitState.Default;
            blackboard.enemy_state.stateCase = Blackboard.Enemy_State.StateCase.Chase;
            blackboard.target = hit.transform;
            blackboard.enemy_state.Increase_Sight++;
        }
    }

    private void EnemyCheck(EnemyUnit enemy)
    {
        if(blackboard.enemy_state.stateCase == Blackboard.Enemy_State.StateCase.Chase && enemy == blackboard.thisUnit) return;
        if(ViewCheck(enemy.transform.position+Vector3.up))
        {
            blackboard.enemy_state.stateCase = Blackboard.Enemy_State.StateCase.Chase;
            blackboard.target = ServiceLocator.Get<GameManager>().player.transform;
            blackboard.enemy_state.Increase_Sight++;
        }

    }

    private void SoundCheck(Transform tr, Vector2 pos, Vector2 size)
    {
        if(blackboard.enemy_state.stateCase == Blackboard.Enemy_State.StateCase.Chase) return;

        var subvec = pos - (Vector2)transform.position;
        if(subvec.magnitude <= soundRange + size.x)
        {
            blackboard.enemy_state.stateCase = Blackboard.Enemy_State.StateCase.Alert;
            blackboard.target = tr;
            blackboard.enemy_state.Increase_Sight++;
        }
    }

    [VisibleEnum(typeof(Blackboard.Enemy_State.StateCase))]
    public void SetState(int i)
    {
        blackboard.enemy_state.stateCase = (Blackboard.Enemy_State.StateCase)i;
        switch(blackboard.enemy_state.stateCase)
        {
            case Blackboard.Enemy_State.StateCase.Default:
                blackboard.enemy_state.stateCase = Blackboard.Enemy_State.StateCase.Default;
                blackboard.target = null;
                break;
            case Blackboard.Enemy_State.StateCase.Chase:
            case Blackboard.Enemy_State.StateCase.Alert:
                blackboard.target = ServiceLocator.Get<GameManager>().player.transform;
                blackboard.enemy_state.Increase_Sight++;
                break;
            default:
                break;
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

        Handles.DrawWireArc(transform.position+Vector3.up, Vector3.forward, startInner, 360-viewAngle, viewInnerRange);
        Handles.DrawWireArc(transform.position+Vector3.up, Vector3.forward, endInner, viewAngle, viewOuterRange);

        Handles.DrawLine(transform.position+Vector3.up + startInner, transform.position+Vector3.up + startOuter);
        Handles.DrawLine(transform.position+Vector3.up + endInner, transform.position+Vector3.up + endOuter);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position+Vector3.up, soundRange);
    }
#endif
}