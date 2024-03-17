using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;



public class Charactor
{
    public float speed;
    public bool isHuman = true;

    public virtual void Die(Charactor charactor)
    {

    }

    public virtual void Setspeed() { }
}
public class WereWolf:Charactor
{
    public int life = 2;
    private static WereWolf instance;

    public static WereWolf Instance()
    {
            if (instance == null)
            {
                //Debug.Log("����� ����");
                instance = new WereWolf();
            }
            else
            {
                //Debug.Log("�� ����⶧���� ������ ����");
            }
            return instance;
    }

    private WereWolf()
    {
        //if (instance == null) Debug.Log("����");
    }

    public override void Setspeed()
    {
        Debug.Log("���� �ӵ�");
        this.speed = 5.0f;
    }
    float Getspeed()
    {
        return this.speed;
    }
}

public class Human: Charactor
{
    int ammo = 0;
    private static Human instance;

    private Human() { }
    public static Human Instance()
    {
        if(instance == null)
        {
            return instance = new Human();
        }
        else
        {
            return instance;
        }
    }
    public override void Setspeed()
    {
        Debug.Log("��� �ӵ�");
        this.speed = 3.0f;
    }
}


public class JumpState//���� Ŭ���������ұ�
{
    public float jumpPower = 0;//���� ��
    public bool isJump = false;//�Ʒ��� ������
    public float jumpStartTime = 0;
    public float jumpHight;
    public State jumptype;

    public enum State { IDLE, NORMAL_JUMP, LONG_JUMP };
}

public class ControllerScript : MonoBehaviour
{
    Rigidbody2D rg2d;//�̹� ������Ʈ������ rigdbody�� �̿��ؼ� �����ϰ���

    JumpState jumpState = new JumpState();
    Charactor charactor = new Charactor();


    



    public float jumpForce = 5f;//���� ��
    public float jumpDuration = 0.5f;//�� ���� �� �� �ִ� �ð�
    public bool isJumping = false;//�ٸ������� ��� �� ���� ������ �ٽ��� ���������� �� ���������� ���� Ŭ������ �־ ���� �ͱ⵵ ��
    [SerializeField]
    private bool isMoving = false;//update���� fixedUpdate�� ������ �����ֱ� ����

    public float moveSpeed = 1;
    private float InxPos;

    private void Awake()
    {

    }

    void Start()
    {
        rg2d = GetComponent<Rigidbody2D>();
    }


    private void FixedUpdate()
    {
        //Debug.Log("fixedUpdate");


        if (jumpState.isJump)
        {
            //Jump();
            switch (jumpState.jumptype)
            {
                case JumpState.State.IDLE:
                    Debug.Log("�Ϲ�");
                    break;
                case JumpState.State.NORMAL_JUMP:
                    Debug.Log("����");
                    //Jump();
                    break;
                case JumpState.State.LONG_JUMP:
                    Debug.Log("�� ����");
                    Debug.Log("���� ����");
                    JumpHigher();
                    break;
        
            }
         
        }

        if (isMoving)
        {
            Move();
        }
        //else
        //{
        //    rg2d.velocity = new Vector2(0, rg2d.velocity.y);//Ű �Է� Ǯ����� 0���� ����� ����
        //}


    }

    private void Move()
    {
        Vector2 v2;
        //   Vector3 v3 = new Vector3(InxPos, rg2d.velocity.y, 0);
        if (Mathf.Sign(InxPos) != Mathf.Sign(rg2d.velocity.x))
        {
            v2 = new Vector2(0, rg2d.velocity.y);
        }
         v2 = new Vector2(InxPos, rg2d.velocity.y);
        
        rg2d.velocity = v2;
        
        Debug.Log(v2+"���� ������~~");
    }

    private void Attack()
    {
        Debug.Log("Attack");
    }

    private void Formchange()
    {
        //if(charactor is WereWolf)//�Ʒ��� ����
        //{
        //
        //}
        //else
        //{
        //
        //}
        if (charactor.isHuman)//����϶� ����
        {
            charactor = WereWolf.Instance();
            charactor.isHuman = false;
            Debug.Log("����� ����");
        }
        else//�����϶� ����
        {
            charactor = Human.Instance();
            charactor.isHuman = true;
            Debug.Log("������� ����");
        }
        charactor.Setspeed();
        moveSpeed = charactor.speed;

    }

    private void InputManager()
    {
        if (Input.GetMouseButtonDown(0))//��Ŭ��
        {
            Attack();
        }
        if(Input.GetMouseButtonDown(1))//��Ŭ��
        {
            Formchange();
        }
        if (Input.GetKeyDown(KeyCode.W))//����Ű ���� ���� Ű�� ���Ѵٸ� keycode�� �����ϸ� ��
        {//"W"�� ������� ���������� ��������
            Jump();

        }
        else if (Input.GetKey(KeyCode.W) && jumpState.isJump && Time.time - jumpState.jumpStartTime < jumpDuration)
        {
            Debug.Log("HOLDDDDDDDDDDDDD");
            jumpState.jumptype = JumpState.State.LONG_JUMP;
        }
        else
        {
            //Debug.Log("else");
            jumpState.isJump = false;
            jumpState.jumptype = JumpState.State.IDLE;

        }
        //if (Input.GetKeyUp(KeyCode.W))//ū���� ���°� ���Ƽ� �ϴ� �ּ�ó���� ���� else�κа� ��ġ�� �κ� ���� �κп��� ���̰� ���ٰ� �Ѵٸ� ���� �ʿ� �Ƹ� ���� �ʿ������
        //{
        //    jumpState.isJump = false;
        //    jumpState.jumptype = JumpState.State.IDLE;
        //}
        //Debug.Log("InxPos value" + InxPos);

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            InxPos = Input.GetAxis("Horizontal") * moveSpeed;
            isMoving = true;

            Debug.Log("�¿�");
        }
        else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            InxPos = 0;
            isMoving = false;

            Debug.Log("�¿� �Է°� " + InxPos);
        }
    }

    private void Update()
    {
        InputManager();
    }

    private void Jump()
    {
        //Debug.Log("nomal");
        //Debug.Log("jump Hight "+jumpState.jumpHight);
        jumpState.jumpHight = jumpForce;
        jumpState.jumpStartTime = Time.time;
        rg2d.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        jumpState.isJump = true;
        jumpState.jumptype = JumpState.State.NORMAL_JUMP;
    }

    private void JumpHigher()
    {
        Debug.Log("higer");
        rg2d.AddForce(Vector3.up * jumpForce * Time.deltaTime*2, ForceMode2D.Impulse);//�߰����� ���� ��� �ִ°� time.deltatime�� �������� �Ҽ����� ���ð��̹Ƿ� ���ϰ� �Ǹ� ���� �����۾���
    }
}