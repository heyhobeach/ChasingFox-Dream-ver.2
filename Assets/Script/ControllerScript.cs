using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static PlatformScript;




public class JumpState//���� Ŭ���������ұ�
{
    public float jumpPower = 0;//���� ��
    public bool isJump = false;//�Ʒ��� ������
    public float jumpStartTime = 0;
    public float jumpHight;
    public State jumptype;

    public enum State { IDLE, NORMAL_JUMP, LONG_JUMP };
}

public partial class ControllerScript : MonoBehaviour
{

    private static ControllerScript instance=null;
    public static ControllerScript Instance
    {
        get
        {
            if (instance == null) return null;
            return instance;
        }
    }

    private ControllerScript() { }


    public bool b_reload = false;
    public float currentTime = 0;//���������úκ�
    public float duration = 1f;

    Rigidbody2D rg2d;//�̹� ������Ʈ������ rigdbody�� �̿��ؼ� �����ϰ���

    JumpState jumpState = new JumpState();
    Charactor charactor = new Charactor();


    public float jumpForce = 5f;//���� ��
    public float jumpDuration = 0.5f;//�� ���� �� �� �ִ� �ð�
    [SerializeField]
    private bool isMoving = false;//update���� fixedUpdate�� ������ �����ֱ� ����
    [SerializeField]
    private bool isGround = true;//���� �� �����ѰͿ� ���� �׽�Ʈ�� ���Կ����̸鼭 ���� �� �ȴٸ� �ٽ� ���� �������� ������ �ߴ��� �Ǵ��ϱ� ����  
    public bool isHide = false;//ũ���ġ �� �� �ִ��� Ȯ�� �뺯��
    public bool isCrouching = false;//ũ���ġ ������ Ȯ�δ�

    [SerializeField] private GameObject currentOneWayPlatform;
    [SerializeField] private BoxCollider2D playerCollider;
    //[SerializeField] BoxCollider2D platformCollider;//���� �Ⱦ�

    public float downTime = 0.4f;//�ٿ� �����ϸ� ������Ʈ �浹 �����ϴ� �ð�
    public bool canDown = false;//�ٿ� ���� ���� ���� Ȯ��
    //private bool dJump = false;//�ٿ� ���� ���� �Ⱦ�



    public float moveSpeed = 5;
    private float InxPos;

    Vector2 vec;//���� ���� ��

    [SerializeField] private GameObject guard;//�������� ���´°�

    private void Awake()
    {
        vec = Vector2.left;//ĳ���� ����Ű �Է¿� ���� ���� ������ ����
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        StartCoroutine(DownJump());
    }

    void Start()
    {
        rg2d = GetComponent<Rigidbody2D>();
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground" || collision.gameObject.tag == "platform")//���� Ȯ�� ������
        {
            isGround = true;
            //WereWolf.Instance().isAttacking = false;// �� �κ��� ������ ������ ���� ���� ���� 
            if (collision.gameObject.tag == "platform")
            {
                currentOneWayPlatform = collision.gameObject;
                //platformCollider = currentOneWayPlatform.GetComponent<BoxCollider2D>();
                Debug.Log(collision.gameObject.GetComponent<PlatformScript>().dObject);//�ٿ� ������Ʈ Ÿ��Ȯ�ο� �α�
                switch (collision.gameObject.GetComponent<PlatformScript>().dObject)//�밢�� ���� ������Ʈ ���� �������� �ð��� �ٸ����� ������
                {
                    case downJumpObject.STRAIGHT://����
                        downTime = 0.4f;
                        break;
                    case downJumpObject.DIAGONAL://�밢��
                        downTime = 0.6f;
                        break;
                }
                //canDown = true;
            }
        }

        if (collision.gameObject.tag == "cover")//���买 Ȯ�ο� �����
        {
            Debug.Log("���买");
        }

        if (collision.gameObject.tag == "bullet")
        {
            Debug.Log("collision hit");
            Vector2 pos = collision.GetContact(0).point;




            float posCheck = Mathf.Sign(transform.position.x - pos.x);
            string leftright = "";
            leftright = (posCheck > 0) ? "��" : "��";
            Debug.Log($"�浹 ��ġ : {pos} ��ġ ���� {posCheck} {leftright} ���� �ǰ�");

        }



    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground" || collision.gameObject.tag == "platform")
        {
            isGround = false;
            if (collision.gameObject.tag == "platform")
            {
                currentOneWayPlatform = null;
                //canDown = false;


            }
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground" || collision.gameObject.tag == "platform")
        {
            isGround = true;
            if (collision.gameObject.tag == "platform")
            {
                currentOneWayPlatform = collision.gameObject;
            }
        }
        if (collision.gameObject.tag == "Wall")
        {

        }


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Ʈ����");

        if (collision.gameObject.tag == "cover")
        {
            Debug.Log($"��� {collision.bounds.min.x}");
            charactor.hidePos= HideDir(collision);
        }

        if(collision.gameObject.tag=="ammo")//�Ѿ��̶��
        {
            Debug.Log("�Ѿ� ȹ��");
            if (Human.Instance().ammo < 2)
            {
                Human.Instance().GetAmmo();
                Destroy(collision.gameObject);
                //Human.Instance().Reload();
            }

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "cover")
        {
            charactor.hidePos = HideDir(collision);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "cover")
        {
            Debug.Log("�����");
            isHide = false;
            guard.SetActive(false);
        }
    }

    private void FixedUpdate()
    {

        if (jumpState.isJump)
        {
            switch (jumpState.jumptype)
            {
                case JumpState.State.IDLE:
                    //Debug.Log("�Ϲ�");
                    break;
                case JumpState.State.NORMAL_JUMP:
                    //Debug.Log("����");
                    //Jump();
                    break;
                case JumpState.State.LONG_JUMP:
                    //Debug.Log("�� ����");
                    //Debug.Log("���� ����");
                    JumpHigher();
                    break;

            }

        }

        if (isMoving&&!isCrouching&&!WereWolf.Instance().isAttacking)
        {
            Move();
        }

    }

    private void Move()
    {
        Vector2 v2;
        //if (Mathf.Sign(InxPos) != Mathf.Sign(rg2d.velocity.x))
        //{
        //    v2 = new Vector2(0, rg2d.velocity.y);
        //}
        v2 = new Vector2(InxPos, rg2d.velocity.y);

        rg2d.velocity = v2;
    }

    private void _Attack()
    {
        Debug.Log("Attack");
        charactor.Attack();
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
            _Attack();


        }
        if (Input.GetMouseButtonDown(1)&&!isCrouching)//��Ŭ��, ũ���ġ �� �ϰ������� ��ü����
        {
            Formchange();
        }

        if (Input.GetKeyDown(KeyCode.W) && isGround&&!WereWolf.Instance().isAttacking)//����Ű ���� ���� Ű�� ���Ѵٸ� keycode�� �����ϸ� ��
        {//"W"�� ������� ���������� ��������
            Jump();
        }
        else if (Input.GetKey(KeyCode.W) && jumpState.isJump && Time.time - jumpState.jumpStartTime < jumpDuration)
        {
            //Debug.Log("HOLDDDDDDDDDDDDD");
            jumpState.jumptype = JumpState.State.LONG_JUMP;
        }
        else
        {
            //Debug.Log("else");
            jumpState.isJump = false;
            jumpState.jumptype = JumpState.State.IDLE;

        }

        //if (Input.GetKeyDown(KeyCode.S))
        if (Input.GetKey(KeyCode.S))
        {
            if (isHide)
            {
                guard.transform.position = this.gameObject.transform.GetChild(0).transform.position;//���� ��ġ�� ���� ����Ʈ ��ġ�� �ű�
                transform.position = charactor.hidePos;
                rg2d.velocity = Vector2.zero;//���� ������ �Ƿ����ϸ� x�� 0���� �ʱ�ȭ
                //rg2d.position = transform.position;
                charactor.Crouch(guard);
                isCrouching = true;
                isMoving = false;
            }
            if (currentOneWayPlatform != null)//�� ���� �κ�
            {
                Debug.Log("hello");
                canDown = true;
                //Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
                //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"),true);

                //StartCoroutine(DownJump());
            }

        }
        else
        {
            guard.SetActive(false);
            isCrouching = false;
        }
        //if (currentOneWayPlatform == null)
        //{
        //    BoxCollider2D platformCollider = currentOneWayPlatform.GetComponent<BoxCollider2D>();
        //    Physics2D.IgnoreCollision(playerCollider, platformCollider);
        //}
        //if (Input.GetKeyUp(KeyCode.W))//ū���� ���°� ���Ƽ� �ϴ� �ּ�ó���� ���� else�κа� ��ġ�� �κ� ���� �κп��� ���̰� ���ٰ� �Ѵٸ� ���� �ʿ� �Ƹ� ���� �ʿ������
        //{
        //    jumpState.isJump = false;
        //    jumpState.jumptype = JumpState.State.IDLE;
        //}
        //Debug.Log("InxPos value" + InxPos);

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            //���� Ŭ���� �ٶ󺸴� �������� �Ǿ� �ֱ� ���� ���� ĳ���� ������ �´ٸ� ad �Է½� filp.x��� ��� ���� 
            /*Vector3 tempvec = attackPoint.transform.localPosition;
            if (Input.GetKey(KeyCode.A))//�¿� �����϶� ��������Ʈ x�� �ٲ��ֱ�����
            {
                tempvec.x = -1.2f;
                attackPoint.transform.localPosition = tempvec;
            }
            else
            {
                tempvec.x = 1.2f;
                attackPoint.transform.localPosition = tempvec;   
            }*/
            InxPos = Input.GetAxis("Horizontal") * moveSpeed;
            isMoving = true;

            //Debug.Log("�¿�");
        }
        else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            //InxPos = 0;
            isMoving = false;

            //Debug.Log("�¿� �Է°� " + InxPos);
        }
    }

    private void Update()
    {
        InputManager();


        if (charactor.isHuman)//�ΰ������϶� ���������� �ؾ��ϴ� �Լ��� �ֱ�����
        {
            //StartCoroutine(UIController.Instance.DrawReload());
            charactor.Reload();
        }
        else
        {

        }
        //_Raycast();
    }

    void _Raycast()
    {

        if (Input.GetKey(KeyCode.A))
        {
            vec = Vector2.left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            vec = Vector2.right;
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, vec, 0.3f, LayerMask.GetMask("bullet"));
        Debug.DrawRay(transform.position, vec, Color.red);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.tag == "bullet")
            {
                CapsuleCollider2D cap2D = hit.collider.GetComponent<CapsuleCollider2D>();

                cap2D.isTrigger = true;
                Debug.Log("trigger on");


            }
            Debug.Log("hit");
            Debug.Log(hit.transform.name);
        }




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
        //Debug.Log("higer");
        rg2d.AddForce(Vector3.up * jumpForce * Time.deltaTime * 2, ForceMode2D.Impulse);//�߰����� ���� ��� �ִ°� time.deltatime�� �������� �Ҽ����� ���ð��̹Ƿ� ���ϰ� �Ǹ� ���� �����۾���
    }

    IEnumerator DownJump()
    {
        while (true)
        {
            if (canDown)
            {
                Debug.Log("hi");
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), true);
                yield return new WaitForSeconds(downTime);
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), false);
                canDown = false;
            }
            yield return null;


        }


    }

    Vector3 HideDir(Collider2D collision)
    {
        int check = CheckDir(collision.transform.position);
        Vector3 correct_pos = Vector3.zero;

        Debug.Log($"Ʈ���� üũ {Mathf.Sign(check)}");
        isHide = true;
        Debug.Log("Ʈ���ž��买");

        if ((int)Mathf.Sign(check) < 0)
        {
            this.gameObject.transform.GetChild(0).transform.localPosition = new Vector3(-0.78f, 0, 0);//0.78
            correct_pos = new Vector3(collision.bounds.max.x, transform.position.y, transform.position.z);
        }
        else
        {
            this.gameObject.transform.GetChild(0).transform.localPosition = new Vector3(0.78f, 0, 0);
            correct_pos = new Vector3(collision.bounds.min.x, transform.position.y, transform.position.z);
        }

        return correct_pos;
    }
    int CheckDir(Vector3 tr)
    {
        int check = (int)Mathf.Sign(tr.x - this.gameObject.transform.position.x);
        return check;
    }

    public bool _DrawReload(ref bool r_bool)
    {
        //������ �ִϸ��̼� �� �����
        if (currentTime <= duration)//1 = duration temp/duration 
        {
            currentTime += Time.deltaTime;
            Debug.Log("����"+currentTime);
            r_bool = true;
            //_DrawReload();
            return false;
        }
        else
        {

            Debug.Log("1�� ��");
            
            currentTime = 0;
            r_bool = false;
            return true;
        }
    }

    public Vector3 ClickPos()
    {
        var screenPoint = Input.mousePosition;//���콺 ��ġ ������
        screenPoint.z = Camera.main.transform.position.z;
        worldPosition=Camera.main.ScreenToWorldPoint(screenPoint);
        int check = CheckDir(worldPosition);
        if (check<0){
            this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else
        {
            this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }



        return worldPosition;
    }
}