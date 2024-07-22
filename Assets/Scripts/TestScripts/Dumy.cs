using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public partial class Dumy : MonoBehaviour, IDamageable
{
    public int _maxHealth;
    public int maxHealth { get { return _maxHealth; } set { _maxHealth = value; } }

    private int _health;
    public int health { get { return _health; }set { _health = value; } }

    private bool _invalidation;
    public bool invalidation { get { return _invalidation; }set { _invalidation = value; } }


    public GameObject bullet;//�Ѿ� ����
    public GameObject[] bullets;//���� ����� �� �ϸ� Ȥ�ó� �ʿ��ұ� ����� �� �κ� ����� �� �ϴ��� 

    public GameObject player;//�÷��̾ Ÿ���� �ϱ����ؼ� �÷��̾ �����ϱ����� �κ�

    public Vector3 playerPos;//�÷��̾��� ��ġ�� ��� ����
    public Vector3 enemypos;//������ ��ġ�� ��� ����
    //public int a = 3;//�׽�Ʈ�� ���� ���������� �� ���µ� ������ �Ƹ��� ������
    public int index_player = 0;

    public float maxDistance = 1.06f;

    private bool follow = false;

    private Vector2 subvec;//ai거리관련
    /// <summary>
    /// 공격범위 관련
    /// </summary>
    [SerializeField]
    private float attackRange;

    private bool attacking = false;

    Coroutine Move_Cor;//움직이는 코루틴 함수 보관하기 위함

    private bool death = false;//적군 죽음

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;

        //Debug.Log(this.transform.parent);
        // player = GameObject.FindWithTag("Player");//�÷��̾ ã�Ƽ� ����, �̷��� �� ������ ó������ �� �����صΰ� ������ �����ϸ� ������ ���� �������� ������ �����ؾ��Ұ�츦 ���� ���� �κ�
        player = Player.pObject;

        // Debug.Log(player.transform);


    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(playerPos);
        if (!death)
        {
            enemypos = transform.position;//������ ��ġ�� ��� �ʱ�ȭ
                                          //targetPos = new Vector2Int((int)player.transform.position.x, (int)  player.transform.position.y);
            _targetPos = player.transform;
            if (Input.GetKeyDown(KeyCode.X))//�ش� �κ��� ���� �׽�Ʈ�� ���ظ��� �κ��̾��� �׳� ���� x�� ������ ������ �����ϴ°� �ϱ�����
            {
                Shoot();
            }
            // Debug.DrawRay
            CircleRay();

            //PathFinding();
            if (Input.GetKeyDown(KeyCode.F))
            {
                FinalNodeList.Clear();

                PathFinding();
                Debug.Log("hello");

                //Debug.Log(NodeArray[8 - bottomLeft.x, -2 - bottomLeft.y].ParentNode.x+"," +NodeArray[8 - bottomLeft.x, -2 - bottomLeft.y].ParentNode.y);    
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                StartCoroutine(cMove());
            }

            //GetComponent<pathfinding>().test();
        }


    }

    public void Shoot()//��� �ڵ� �ش� �ڵ�� ���߿� �����ؼ� ��� ����ص� �ɰ���
    {
        //Instantiate(bullet,new Vector3(0,0,0),Quaternion.identity);
        GameObject _bullet = Instantiate(bullet, enemypos, transform.rotation);

        //_bullet.transform.SetParent(this.transform);
        playerPos = player.transform.position;

        GameObject gObj = this.gameObject;
        _bullet.GetComponent<Bullet>().Set(transform.position, playerPos, Vector3.zero, 1, 1, gObj, (Vector2)(playerPos-transform.position).normalized);
         Debug.Log("shoot"+playerPos+"enemypos"+enemypos);    
        //_bullet.transform.position = Vector2.left;
    }


    private void OnTriggerEnter2D(Collider2D collision)//���� ������ ���������� ������� ����ǵ� �Ƹ� ���ݰ��� trigger���� ���߿� �÷��̾����� ������  ���� ���������� ������ �����̱��� �ٵ� �÷��̾��ʿ��� �ִ°� ���ƺ��̱���
    {
        Debug.Log(collision.gameObject.name);
        var temp = GetComponent<IDamageable>();
        if (collision.gameObject.name == "AttackBox")
        {
            temp.GetDamage(2);
            Debug.Log("근접공격");
        }
    }

    private void CircleRay()//유저 탐색할 레이 관련 함수
    {
         maxDistance = 5f;
        Vector3 myposition = transform.position;
        float mysize = 5f;//반지름
        //RaycastHit2D ray2d = Physics2D.Raycast(transform.position, transform.forward, 10f); 
        int layerMask = 1 << LayerMask.NameToLayer("DeadEnemy");//enemy와 gunsound 객체 총알이 만약 바닥에 박히면 gunsound객체를 생성했다가 일정시간 이후 지우는식
        int linelayerMask = 1 << LayerMask.NameToLayer("Player")|1<<LayerMask.NameToLayer("Ground");
        RaycastHit2D ray2d = Physics2D.CircleCast(myposition, mysize, Vector2.up, maxDistance,layerMask);

        //var index_player = Array.FindIndex(ray2d, x => x.transform.tag == "Player");
        Vector2 subpos = _targetPos.position - transform.position;
        //Debug.Log(subpos);
        //Debug.DrawRay(transform.position, subpos);
        RaycastHit2D[] target_ray2d = Physics2D.RaycastAll(myposition, subpos, attackRange, linelayerMask);
        index_player = Array.FindIndex(target_ray2d, x => x.transform.tag == "Player");

        if (ray2d&!follow)
        {
            Debug.Log("주변에 죽은 친구 있음");
            StartCoroutine(timer());
        }
        
        if (index_player>-1)//ray2d
        {
            Debug.Log(string.Format("타겟{0} index{1}", target_ray2d[index_player].transform.name,index_player));
            subvec = (Vector2)target_ray2d[index_player].transform.position - (Vector2)transform.position;// ray2d=>tartget_ray2d[index_player]
            float deg = Mathf.Atan2(subvec.y, subvec.x);//mathf.de
            deg *= Mathf.Rad2Deg;
            //a *= Mathf.Deg2Rad;
            //float dis = ray2d.distance;
            if (mysize <= subvec.magnitude)
            {
                // Debug.Log("범위 넘어감");
                //ray2d = null; 
            }
            else//범위 안에 들어왔을때
            {
                // Debug.Log(string.Format("{0}||{1}||{2}||", deg, mysize, subvec.magnitude));
                if (!follow)//이미 레이 = 시야에 감지 되었기에 계속 추격해야함
                {
                    //1초마다 갱신하게 코루틴 필요    
                    //Debug.Log(ray2d.collider.gameObject.name);
                    StartCoroutine(timer());
                }
                
            }
            
            
        }





        
        //float m
    }

    IEnumerator timer()
    {
        Debug.Log("코루틴 시작");    
        follow = true;
        while (true)
        {
            yield return new WaitForSeconds(1);
            Debug.Log(string.Format("index_player {0}", index_player));
            Debug.Log(string.Format("조건 거리{0} 인덱스{1}", subvec.magnitude <= attackRange, index_player == 0));
            if ((subvec.magnitude <= attackRange)&(index_player==0))//만약 벽이 2개 겹처있는 경우는 체크 안되어있음
            {
                //attack();
                //StopCoroutine(cMove());
             
                if (Move_Cor != null)
                {
                    StopCoroutine(Move_Cor);
                }
                
                attacking = true;
                Shoot();
                Debug.Log("공격");
            }
            else
            {
                startPos.x = (int)_startPos.position.x;
                startPos.y = (int)_startPos.position.y;
                targetPos.x = (int)_targetPos.position.x;
                targetPos.y = Mathf.FloorToInt(_targetPos.position.y);

                attacking = false;
                PathFinding();
                if (Move_Cor != null)
                {
                    StopCoroutine(Move_Cor);
                }
                Move_Cor = StartCoroutine(cMove());
                Debug.Log("경로 갱신");
            }

            
        }
        
    }
    public void Death()
    {
        //여기 사망 관련 처리
        //Destroy(this.gameObject);
        invalidation = true;
        death= true;
        this.gameObject.tag = "DeadEnemy";
        this.gameObject.layer = 14;
        StopAllCoroutines();
        Debug.Log("적군 사망");
    }
}
