using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

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
    public int a = 3;//�׽�Ʈ�� ���� ���������� �� ���µ� ������ �Ƹ��� ������

    public float maxDistance = 1.06f;

    private bool follow = false;

    private Vector2 subvec;//ai거리관련
    /// <summary>
    /// 공격범위 관련
    /// </summary>
    [SerializeField]
    private float attackRange;

    private bool attacking = false;
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
        if (collision.gameObject.name == "MeleeAttack")
        {

        }
    }

    private void CircleRay()//유저 탐색할 레이 관련 함수
    {
         maxDistance = 5f;
        Vector3 myposition = transform.position;
        float mysize = 5f;//반지름
        //RaycastHit2D ray2d = Physics2D.Raycast(transform.position, transform.forward, 10f); 
        int layerMask = 1 << LayerMask.NameToLayer("Player");
        RaycastHit2D ray2d = Physics2D.CircleCast(myposition, mysize, Vector2.up, maxDistance,layerMask);
        if (ray2d)
        {
            subvec = (Vector2)ray2d.transform.position - (Vector2)transform.position;
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
                    Debug.Log(ray2d.collider.gameObject.name);
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
            if (subvec.magnitude <= attackRange)
            {
                //attack();
                //StopCoroutine(cMove());
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


                PathFinding();
                Debug.Log("경로 갱신");
            }

            
        }
        
    }
    public void Death()
    {
        //여기 사망 관련 처리
        Destroy(this.gameObject);
        Debug.Log("적군 사망");
    }
}
