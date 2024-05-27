using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public partial class Dumy : MonoBehaviour
{
    public GameObject bullet;//�Ѿ� ����
    public GameObject[] bullets;//���� ����� �� �ϸ� Ȥ�ó� �ʿ��ұ� ����� �� �κ� ����� �� �ϴ��� 

    public GameObject player;//�÷��̾ Ÿ���� �ϱ����ؼ� �÷��̾ �����ϱ����� �κ�

    public Vector3 playerPos;//�÷��̾��� ��ġ�� ��� ����
    public Vector3 enemypos;//������ ��ġ�� ��� ����
    public int a = 3;//�׽�Ʈ�� ���� ���������� �� ���µ� ������ �Ƹ��� ������

    public float maxDistance = 1.06f;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(this.transform.parent);
        // player = GameObject.FindWithTag("Player");//�÷��̾ ã�Ƽ� ����, �̷��� �� ������ ó������ �� �����صΰ� ������ �����ϸ� ������ ���� �������� ������ �����ؾ��Ұ�츦 ���� ���� �κ�
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(playerPos);
        enemypos = transform.position;//������ ��ġ�� ��� �ʱ�ȭ
        if (Input.GetKeyDown(KeyCode.X))//�ش� �κ��� ���� �׽�Ʈ�� ���ظ��� �κ��̾��� �׳� ���� x�� ������ ������ �����ϴ°� �ϱ�����
        {
            Shoot();
        }
        // Debug.DrawRay
        CircleRay();

        //GetComponent<pathfinding>().test();

    }

    public void Shoot()//��� �ڵ� �ش� �ڵ�� ���߿� �����ؼ� ��� ����ص� �ɰ���
    {
        //Instantiate(bullet,new Vector3(0,0,0),Quaternion.identity);
        GameObject _bullet = Instantiate(bullet, enemypos, transform.rotation);

        //_bullet.transform.SetParent(this.transform);
        playerPos = player.gameObject.transform.position;
        // _bullet.GetComponent<BulletScript>().targetPos = playerPos;
        // _bullet.GetComponent<BulletScript>().shootPos = enemypos;
        //enemypos = transform.position;
        GameObject gObj = this.gameObject;
        _bullet.GetComponent<Bullet>().Set(transform.position, playerPos, 1, 1, gObj, (Vector2)(playerPos-transform.position).normalized);
        // Debug.Log("shoot"+playerPos+"enemypos"+enemypos);
        //_bullet.transform.position = Vector2.left;
    }


    private void OnTriggerEnter2D(Collider2D collision)//���� ������ ���������� ������� ����ǵ� �Ƹ� ���ݰ��� trigger���� ���߿� �÷��̾����� ������  ���� ���������� ������ �����̱��� �ٵ� �÷��̾��ʿ��� �ִ°� ���ƺ��̱���
    {
        if (collision.gameObject.name == "MeleeAttack")
        {
            Debug.Log("��@���� ���� ���� ������?");
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
            Debug.Log(ray2d.collider.gameObject.name);
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(1,1), Time.deltaTime);
            test();
        }

        
        //float m
    }
}
