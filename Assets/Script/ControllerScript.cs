using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
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

public class ControllerScript : MonoBehaviour
{

    public class Charactor
    {
        public float speed;
        public bool isHuman = true;
        public int hidePos = 0;


        public virtual void Die(Charactor charactor)
        {

        }

        public virtual void Setspeed() { }

        public virtual void Attack() { }
        public void Crouch(GameObject guard)
        {
            Debug.Log("ũ���ġ");
            guard.SetActive(true);

            if (hidePos < 0)
            {

            }

        }//����� �ΰ� �Ѵ� ��ũ���� ���ٰ� �����ϱ⿡ �׳� �Ϲ��Լ��� ����
    }
    public class WereWolf : Charactor
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
        public override void Attack()
        {
            Debug.Log("���� ����");
        }

        float Getspeed()
        {
            return this.speed;
        }
    }

    public class Human : Charactor
    {
        int ammo = 0;
        private static Human instance;

        private Human() { }
        public static Human Instance()
        {
            if (instance == null)
            {
                return instance = new Human();
            }
            else
            {
                return instance;
            }
        }
        public override void Attack()
        {
            Debug.Log("��� ����");
        }
        public override void Setspeed()
        {
            Debug.Log("��� �ӵ�");
            this.speed = 3.0f;
        }
    }

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




    Rigidbody2D rg2d;//�̹� ������Ʈ������ rigdbody�� �̿��ؼ� �����ϰ���

    JumpState jumpState = new JumpState();
    Charactor charactor = new Charactor();


    public float jumpForce = 5f;//���� ��
    public float jumpDuration = 0.5f;//�� ���� �� �� �ִ� �ð�
    [SerializeField]
    private bool isMoving = false;//update���� fixedUpdate�� ������ �����ֱ� ����
    [SerializeField]
    private bool isGround = true;
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

    [SerializeField] private GameObject guard;

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


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Ʈ����");

        if (collision.gameObject.tag == "cover")
        {
            Debug.Log($"��� {collision.bounds.min.x}");
            charactor.hidePos= coverDir(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "cover")
        {
            charactor.hidePos = coverDir(collision);
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

        if (isMoving)
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
        if (Input.GetMouseButtonDown(1))//��Ŭ��
        {
            Formchange();
        }

        if (Input.GetKeyDown(KeyCode.W) && isGround)//����Ű ���� ���� Ű�� ���Ѵٸ� keycode�� �����ϸ� ��
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
                guard.transform.position = this.gameObject.transform.GetChild(0).transform.position;
                charactor.Crouch(guard);
                isCrouching = true;
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

    int coverDir(Collider2D collision)
    {
        float check = collision.gameObject.transform.position.x - this.gameObject.transform.position.x;

        Debug.Log($"Ʈ���� üũ {Mathf.Sign(check)}");
        isHide = true;
        Debug.Log("Ʈ���ž��买");
        if (Mathf.Sign(check) < 0)
        {
            this.gameObject.transform.GetChild(0).transform.localPosition = new Vector3(-0.76f, 0, 0);
        }
        else
        {
            this.gameObject.transform.GetChild(0).transform.localPosition = new Vector3(0.76f, 0, 0);
        }

        return (int)Mathf.Sign(check);
    }
}