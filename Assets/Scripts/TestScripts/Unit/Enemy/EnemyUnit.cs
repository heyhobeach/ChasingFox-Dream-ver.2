using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyUnit : UnitBase
{
    GameObject player;
    public float maxDistance = 1.06f;

    public GameObject bullet;//�Ѿ� ����
    // public GameObject[] bullets;//���� ����� �� �ϸ� Ȥ�ó� �ʿ��ұ� ����� �� �κ� ����� �� �ϴ��� 

    public class Enemy_State//적군 상태값을 이너 클래스로 표현 중첩되는 표현 사용시 enum으로 표현 안 될것 같아 해당 방식 사용
    {
        public bool Defalut=true;//생성시 기준 생성시 defalut는 true기 때문에
        public bool Tracking=false;
        public bool Missing = false;
        public bool Increase_Sight = false;

        public void Reset_State()//모든 상태를 false로 전환
        {
            this.Defalut = true;//생성시 기준 생성시 defalut는 true기 때문에
            this.Tracking = false;
            this.Missing= false;
            this.Increase_Sight = false;
        }

        public bool[] Get_State()
        {
            bool[] states = new bool[4];
            states[0] = Defalut;
            states[1] = Tracking;
            states[2] = Missing;
            states[3] = Increase_Sight;
            return states;
        }
    }

    Coroutine Move_Cor;//움직이는 코루틴 함수 보관하기 위함
    Enemy_State enemy_state;
    private Vector2 subvec;//ai거리관련
    public int index_player = 0;

    protected override void OnEnable() { GameObject.FindGameObjectWithTag("Player"); }

    public override bool Move(float dir)
    {
        return base.Move(dir);
    }

    public override bool Crouch(KeyState crouchKey) => false;

    public override bool Jump(KeyState jumpKey) => false;

    public override bool Attack(Vector3 attackPos)
    {
        if(ControllerChecker()) return false;
        GameObject _bullet = Instantiate(bullet, transform.position, transform.rotation);

        GameObject gObj = this.gameObject;
        _bullet.GetComponent<Bullet>().Set(transform.position, attackPos, Vector3.zero, 1, 1, gObj, (Vector2)(attackPos-transform.position).normalized);
        return true;
    }

    protected override void Update()
    {
        base.Update();
        CircleRay();
    }

    private RaycastHit2D ray2d;
    private void CircleRay()//유저 탐색할 레이 관련 함수
    {
        float maxDistance = 5f;
        float mysize = 5f;//반지름
        int layerMask = 1 << LayerMask.NameToLayer("DeadEnemy");//enemy와 gunsound 객체 총알이 만약 바닥에 박히면 gunsound객체를 생성했다가 일정시간 이후 지우는식
        ray2d = Physics2D.CircleCast(transform.position, mysize, Vector2.up, maxDistance,layerMask);//죽은 적군 찾는 변수


        Vector2 subpos = GameManager.Instance.player.transform.position - transform.position;
        int linelayerMask = 1 << LayerMask.NameToLayer("Player")|1<<LayerMask.NameToLayer("Ground");
        RaycastHit2D[] target_ray2d = Physics2D.RaycastAll(transform.position, subpos, attackRange, linelayerMask);
        int index_player = Array.FindIndex(target_ray2d, x => x.transform.tag == "Player");

        if (ray2d&!enemy_state.Tracking)//추격상태가 아니고 죽은 
        {
            if (ray2d.transform.gameObject.layer == 15)
            {
                SetFollow();
                return;
            }
            StartCoroutine(timer());
        }
        
        if (index_player>-1)//ray2d,여기 !follow해도 될것 같앗음
        {
            subvec = (Vector2)target_ray2d[index_player].transform.position - (Vector2)transform.position;// ray2d=>tartget_ray2d[index_player]
            float deg = Mathf.Atan2(subvec.y, subvec.x);//mathf.de
            deg *= Mathf.Rad2Deg;
            if (mysize <= subvec.magnitude) {}
            else//범위 안에 들어왔을때
            {
                if (!enemy_state.Tracking)//이미 레이 = 시야에 감지 되었기에 계속 추격해야함
                {
                    StartCoroutine(timer());
                }
            }
        }
    }

    bool attacking;
    public float attackRange;
    IEnumerator timer()
    {
        enemy_state.Tracking = true;
        enemy_state.Defalut = false;
        while (true)
        {
            yield return new WaitForSeconds(1);
            if ((subvec.magnitude <= attackRange)&(index_player==0))//만약 벽이 2개 겹처있는 경우는 체크 안되어있음, 공격부분
            {
                if (Move_Cor != null)
                {
                    StopCoroutine(Move_Cor);
                }
                attacking = true;
                Attack((player.transform.position + transform.position).normalized);
            }
            else//추격부분
            {
                SetFollow();
            }
            if (!enemy_state.Missing)//못찾는 상태일때 트래킹을 끄기위해
            {
                enemy_state.Tracking = true;
            }
            else
            {
                enemy_state.Tracking = false;
            }

            
        }
        
    }

    public Vector2Int bottomLeft, topRight, startPos, targetPos;
    public Transform _startPos, _targetPos;
    List<GameManager.Node> FinalNodeList;
    private void SetFollow()
    {
        attacking = false;
        FinalNodeList = GameManager.Instance.PathFinding(transform.position, player.transform.position);
        if (Move_Cor != null) StopCoroutine(Move_Cor);
        Move_Cor = StartCoroutine(cMove());
    }

    IEnumerator cMove()
    {
        for (int i = 1; i < FinalNodeList.Count; )
        {
            if (attacking) break;
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(FinalNodeList[i].x, FinalNodeList[i].y + 1), Time.deltaTime);
            if(transform.position== new Vector3(FinalNodeList[i].x, FinalNodeList[i].y + 1, transform.position.z)) i++;
            yield return null;
        }
    }
}
