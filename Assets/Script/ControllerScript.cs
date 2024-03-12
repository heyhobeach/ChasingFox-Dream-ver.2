using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;



public class Charactor
{
    public float speed;

    public virtual void Die(Charactor charactor)
    {

    }
}
public class WereWolf:Charactor
{

    void Setspeed()
    {
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
    void Setspeed()
    {
        this.speed = 3.0f;
    }
}

public class JumpState//���� Ŭ���������ұ�
{
    public float jumpPower = 0;//���� ��
    public bool isJump = false;//�Ʒ��� ����
    public float jumpStartTime = 0;
    public float jumpHight;
    public State jumptype;

    public enum State { IDLE, NORMAL_JUMP, LONG_JUMP };
}

public class ControllerScript : MonoBehaviour
{
    Rigidbody2D rg2d;//�̹� ������Ʈ������ rigdbody�� �̿��ؼ� �����ϰ���

    JumpState jumpState = new JumpState();



    public float jumpForce = 5f;//���� ��
    public float jumpDuration = 3f;//�� ���� �� �� �ִ� �ð�
    public bool isJumping = false;//�ٸ������� ��� �� ���� ������ �ٽ��� ���������� �� ���������� ���� Ŭ������ �־ ���� �ͱ⵵ ��

    private bool isMoving = false;//update���� fixedUpdate�� ������ �����ֱ� ����

    private float InxPos,InyPos;

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
    }

    private void Move()
    {
        Vector3 v3 = new Vector3(InxPos, rg2d.velocity.y, 0);
        
        rg2d.velocity = v3;
        Debug.Log(v3+"���� ������~~");
    }

    private void Update()
    {

        Debug.Log(jumpState.jumptype);

        if (Input.GetKeyDown(KeyCode.W)){//"W"�� ������� ���������� ��������
            Debug.Log("W");
            jumpState.jumpHight= jumpForce;
            jumpState.jumpStartTime=Time.time;
            Jump();
            jumpState.isJump = true;
            jumpState.jumptype= JumpState.State.NORMAL_JUMP;
            
        }
        else if (Input.GetKey(KeyCode.W)&&jumpState.isJump&&Time.time- jumpState.jumpStartTime < jumpDuration) {
            Debug.Log("HOLDDDDDDDDDDDDD");
            jumpState.jumptype = JumpState.State.LONG_JUMP;
        }
        else
        {
            Debug.Log("else");
            jumpState.jumptype = JumpState.State.IDLE;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            Debug.Log("UPPPPP");
            jumpState.isJump = false;
            jumpState.jumptype = JumpState.State.IDLE;
        }
        InxPos = Input.GetAxis("Horizontal") * 10;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            isMoving = true;
            
            Debug.Log("�¿�");
        }
        else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        { 
            isMoving= false;
            Debug.Log("�¿� �Է°� " + InxPos);
        }
    }

    private void Jump()
    {
        Debug.Log("nomal");
        Debug.Log("jump Hight "+jumpState.jumpHight);
        rg2d.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        //isJumping = true;
        //jumpStartTime = Time.time;
    }

    private void JumpHigher()
    {
        Debug.Log("higer");
        rg2d.AddForce(Vector3.up * jumpForce * Time.deltaTime*2, ForceMode2D.Impulse);//�߰����� ���� ��� �ִ°� time.deltatime�� �������� �Ҽ����� ���ð��̹Ƿ� ���ϰ� �Ǹ� ���� �����۾���
    }
}