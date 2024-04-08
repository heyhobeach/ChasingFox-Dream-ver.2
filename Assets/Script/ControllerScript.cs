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

    public enum State { IDLE, NORMAL_JUMP, LONG_JUMP };//���� ���¿����� �ٸ������� ����� ���� ���� enum�̾���
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


    public bool b_reload = false;//������ ��Ʈ�� bool��
    public float currentTime = 0;//���������úκ�
    public float duration = 1f;//������ �ð� ���߿� ���� �������� 2�ʷ� �����ϵ��� Ŭ���� �����ʿ� SetDuration()�����Լ��� ����� �ΰ��� 1�� ����� 2�ʷ� ����

    Rigidbody2D rg2d;//�̹� ������Ʈ������ rigdbody�� �̿��ؼ� �����ϰ���

    JumpState jumpState = new JumpState();//���� ���¸� ����
    Charactor charactor = new Charactor();


    public float jumpForce = 5f;//���� �� �ش� �κ��� ���߿� Ŭ������ �����ؼ� ���� ���ϰ� �����ؾ���
    public float jumpDuration = 0.5f;//�� ���� �� �� �ִ� �ð� �ش� �κ��� ���߿� Ŭ������ �����ؼ� ���� ���ϰ� �����ؾ���
    [SerializeField]
    private bool isMoving = false;//update���� fixedUpdate�� ������ �����ֱ� ���� 
    [SerializeField]
    private bool isGround = true;//���� �� �����ѰͿ� ���� �׽�Ʈ�� ���Կ����̸鼭 ���� �� �ȴٸ� �ٽ� ���� �������� ������ �ߴ��� �Ǵ��ϱ� ����  
    public bool isHide = false;//ũ���ġ �� �� �ִ��� Ȯ�� �뺯��
    public bool isCrouching = false;//ũ���ġ ������ Ȯ�δ�
    private bool isJumping;
    private bool isJumpReady;
    private const float gravity = -9.81f;

    private LayerMask lm;

    [SerializeField] private float speed;
    [SerializeField] private float accelerate;

    
    private float moveVec = 0;
    private float velocity = 0;

    private float distanceToCheck = 0.5f;
    // private bool isGrounded;
    private bool jumpKey;
    private bool jumpKeyUp;

    [SerializeField] private GameObject currentOneWayPlatform;//�������� �ִ� ���ٴڰ��� ������Ʈ ��� ����
    //[SerializeField] private BoxCollider2D playerCollider;//��� ����
    //[SerializeField] BoxCollider2D platformCollider;//���� �Ⱦ�

    public float downTime = 0.4f;//�ٿ� �����ϸ� ������Ʈ �浹 �����ϴ� �ð�
    public bool canDown = false;//�ٿ� ���� ���� ���� Ȯ��
    //private bool dJump = false;//�ٿ� ���� ���� �Ⱦ�


    public Vector3 worldPosition;//Ŭ���ϴ� ��ġ�� ������� ����
    public float moveSpeed = 5;//ĳ���� �̵��ӵ� ���� �κ� �ش� �κ��� ĳ���� �� ü������ speed�� ������ ���� �Ʒ��κ� ���� ��ü������ �پ�����
    private float InxPos;//�̵� ���� ���� �� �κ��� ���߿� �̵��� ���ս� ���� ����

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
        StartCoroutine(DownJump());//�Ʒ� ���� �ڷ�ƾ �ش� ��
    }

    void Start()
    {
        rg2d = GetComponent<Rigidbody2D>();
        lm = ~(1<<gameObject.layer);
        isJumpReady = true;
        distanceToCheck = gameObject.GetComponent<BoxCollider2D>().size.y * 0.7f;
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground" || collision.gameObject.tag == "platform")//���� Ȯ�� ������
        {
            // isGround = true;
            //WereWolf.Instance().isAttacking = false;// �� �κ��� ������ ������ ���� ���� ���� 
            if (collision.gameObject.tag == "platform")
            {
                currentOneWayPlatform = collision.gameObject;//�÷����̶�� ���� �÷����� ����
                //platformCollider = currentOneWayPlatform.GetComponent<BoxCollider2D>();
                Debug.Log(collision.gameObject.GetComponent<PlatformScript>().dObject);//�ٿ� ������Ʈ Ÿ��Ȯ�ο� �α�
                switch (collision.gameObject.GetComponent<PlatformScript>().dObject)//�밢�� ���� ������Ʈ ���� �������� �ð��� �ٸ����� ������
                {
                    case downJumpObject.STRAIGHT://����
                        downTime = 0.4f;//�������� �ð� �ٸ��� �ϱ� ����
                        break;
                    case downJumpObject.DIAGONAL://�밢��
                        downTime = 0.6f;//�������� �ð� �ٸ��� �ϱ� ����
                        break;
                }
                //canDown = true;
            }
        }

        if (collision.gameObject.tag == "cover")//���买 Ȯ�ο� ����� ���� ���ϱ� �ʿ���°Ű��� �ֳ��ϸ� ���买�� ���� trigger�� �ߵ���
        {
            Debug.Log("���买");
        }

        if (collision.gameObject.tag == "enemyBullet")//�÷��̾ �Ѿ��� ���� ��츦 ������ �¿��� ������ �쿡�� ������
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
        if (collision.gameObject.tag == "ground" || collision.gameObject.tag == "platform")//������ ������
        {
            // isGround = false;
            if (collision.gameObject.tag == "platform")
            {
                currentOneWayPlatform = null;//platform���� ����Ŷ�� �÷��� ������ ���
                //canDown = false;


            }
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground" || collision.gameObject.tag == "platform")
        {
            // isGround = true;
            if (collision.gameObject.tag == "platform")//�÷����̶�� ���� �÷����� ����
            {
                currentOneWayPlatform = collision.gameObject;
            }
        }
        if (collision.gameObject.tag == "Wall")//���� �� �پ��ִ� ���� ��� �����ϱ� ���� ������� ����
        {

        }


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Ʈ����");

        if (collision.gameObject.tag == "cover")//�����϶�
        {
            Debug.Log($"��� {collision.bounds.min.x}");
            charactor.hidePos= HideDir(collision);//HideDir �Լ� ���� �ش� �Լ��� ���� + ���� ����
        }

        if(collision.gameObject.tag=="ammo")//�Ѿ��̶��
        {
            Debug.Log("�Ѿ� ȹ��");
            if (Human.Instance().ammo < 2)
            {
                Human.Instance().GetAmmo();//GetAmmo�� ������ ���ֵ� �ɵ� �ߺ��� ������
                Destroy(collision.gameObject);//���������Ƿ� źâ������Ʈ����
                //Human.Instance().Reload();
            }

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "cover")//���� ���� �̺κ��� �߰��� �صа��� ���� ��ġ ������Ʈ �ӿ� ���»��¿��� ���������� ����
        {
            charactor.hidePos = HideDir(collision);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "cover")//���� ������ �����츦 ���� ����� ������ ���� �����߿� �̵� �Ұ����ϱ⿡ ���� �ʿ�� ������ ���� �̵������ϴٸ� �ʿ��Ѻκ�
        {
            Debug.Log("�����");
            isHide = false;
            guard.SetActive(false);
        }
    }

    private void FixedUpdate()
    {

        // if (jumpState.isJump)//�������̶��
        // {
        //     switch (jumpState.jumptype)//LONG_JUMP�Ǵ��� ���Ѻκ���
        //     {
        //         case JumpState.State.IDLE:
        //             //Debug.Log("�Ϲ�");
        //             break;
        //         case JumpState.State.NORMAL_JUMP:
        //             //Debug.Log("����");
        //             //Jump();
        //             break;
        //         case JumpState.State.LONG_JUMP:
        //             //Debug.Log("�� ����");
        //             //Debug.Log("���� ����");
        //             JumpHigher();//�� ���� ����
        //             break;

        //     }

        // }

        //������ �����ѵ� �������� �Է� ���� ���¸� ���� �ʾ����� �������� �ƴҶ�, �� �׳� �̵� + �������¸� ����

        Move(new Vector3(HorizontalForce(), VerticalForce()) * Time.deltaTime);

    }
    private float VerticalForce()
    {
        velocity += gravity * Time.deltaTime;
        if(isGround && velocity < 0) velocity = 0;
        if(isJumping)
        {
            if(velocity <= 0) velocity = jumpForce/2;
            velocity = Mathf.Sin(velocity/jumpForce) * jumpForce * 1.2f;
        }
        return velocity;
    }
    private float HorizontalForce()
    {
        return moveVec * moveSpeed;
    }
    public void Move(Vector3 force) => transform.Translate(force);

    private void _Attack()//���ݽ� �� ĳ���ͺ� ���� ����
    {
        Debug.Log("Attack");
        charactor.Attack();
    }

    private void Formchange()//��ü������ �ν��Ͻ��� �־ ������
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
        charactor.SetInfo();
        //charactor.Setspeed();
        moveSpeed = charactor.speed;

    }
    private void InputManager()//���� InputManager���� Update���� ȥ�� ������
    {
        if (Input.GetMouseButtonDown(0))//��Ŭ�� ���ݽ�
        {
            _Attack();


        }
        if (Input.GetMouseButtonDown(1)&&!isCrouching)//��Ŭ��, ũ���ġ �� �ϰ������� ��ü����
        {
            Formchange();
        }

        // if (Input.GetKeyDown(KeyCode.W) && isGround&&!WereWolf.Instance().isAttacking)//����Ű ���� ���� Ű�� ���Ѵٸ� keycode�� �����ϸ� ��
        // {//"W"�� ������� ���������� ��������
        //     // Jump();
        // }
        // else if (Input.GetKey(KeyCode.W) && jumpState.isJump && Time.time - jumpState.jumpStartTime < jumpDuration)//���� ���̸� ��� ������������ �������¸� LONG_JUMP�� ����
        // {
        //     //Debug.Log("HOLDDDDDDDDDDDDD");
        //     jumpState.jumptype = JumpState.State.LONG_JUMP;
        // }
        // else//�̰� �ƴ϶�� �׳� �������� �ƴ϶�� �Ǵ� ���� isJump�� false�� �����ϰ� ���� ���¸� IDLE�� ����
        // {
        //     //Debug.Log("else");
        //     jumpState.isJump = false;
        //     jumpState.jumptype = JumpState.State.IDLE;

        // }
        jumpKey = Input.GetKey(KeyCode.W);
        if(!jumpKeyUp) jumpKeyUp = Input.GetKeyUp(KeyCode.W);
        if(isGround && jumpKeyUp)
        {
            isJumpReady = true;
            jumpKeyUp = false;
        }
        if(isJumpReady && jumpKey)
        {
            isJumping = true;
            if(jumpKeyUp) isJumpReady = false;
            if(velocity/jumpForce >= 0.9f) isJumpReady = false;
        }
        else isJumping = false;

        //if (Input.GetKeyDown(KeyCode.S))
        if (Input.GetKey(KeyCode.S))//S�� ������
        {
            if (isHide)//���� ������Ʈ�� ��ȣ �ۿ��� �����ϴٸ�
            {
                guard.transform.position = this.gameObject.transform.GetChild(0).transform.position;//���� ��ġ�� ���� ����Ʈ ��ġ�� �ű�
                transform.position = charactor.hidePos;
                rg2d.velocity = Vector2.zero;//���� ������ �Ƿ����ϸ� x�� 0���� �ʱ�ȭ
                //rg2d.position = transform.position;
                charactor.Crouch(guard);
                isCrouching = true;
                isMoving = false;
            }
            if (currentOneWayPlatform != null)//�� �Ʒ� ���� ������ ������Ʈ�� ��������� ,�켱���� ���� ���� �ø��� return�� �ʿ��ҵ� 
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
            guard.SetActive(false);//s�� ������ �ʾ����Ƿ� ���带 Ǭ��
            isCrouching = false;
        }

        // if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))//�̵� �Է��� ��������
        // {
        //     InxPos = Input.GetAxis("Horizontal") * moveSpeed;
        //     isMoving = true;

        //     //Debug.Log("�¿�");
        // }
        // else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))//�̵� �Է��� Ǯ������ isMoving�� false���·� ����
        // {
        //     //InxPos = 0;
        //     isMoving = false;

        //     //Debug.Log("�¿� �Է°� " + InxPos);
        // }
        if(!isCrouching&&!WereWolf.Instance().isAttacking)
        {
            isGround = Physics2D.Raycast(transform.position, Vector2.down, distanceToCheck, lm);
            moveVec += (Input.GetAxisRaw("Horizontal")-moveVec) * accelerate;
            if(Input.GetAxisRaw("Horizontal") == 0) moveVec += (Input.GetAxisRaw("Horizontal")-moveVec) * accelerate;
            moveVec = Mathf.Clamp(moveVec, -1, 1);
        }
        else if(WereWolf.Instance().isAttacking)
        {
            moveVec = sign * 1.5f;
            velocity = 3.5f;
        }
    }

    private void Update()
    {
        InputManager();
        // ConditionUpdate();


        if (charactor.isHuman)//�ΰ������϶� ���������� �ؾ��ϴ� �Լ��� �ֱ�����
        {
            //StartCoroutine(UIController.Instance.DrawReload());
            charactor.Reload();//������µ� �������� �����ϵ��� �Ѵٸ� �����Լ��� ���� ������ ���� �����ϴٰ� ������
        }
        else//���� ���¶� ���������� üũ�ϰų� �����ؾ��� �κ� �־����
        {

        }
        //_Raycast();
    }

    void _Raycast()//ó���� �Ѿ˰��� �浹, ���� ������Ʈ Ȯ���� ���� ray�� ��Ҵµ� ����� ���̵� ���ư��� ������� ���� ���Ŀ� ray�� �ʿ��ϴٸ� ���� �� �������� ������
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
        jumpState.jumpStartTime = Time.time;//���� �ð� ����
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
            if (canDown)//�Ʒ� ���� ������ ������Ʈ �������
            {
                Debug.Log("hi");
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), true);//������ �׳� ������ �ð����� �ش� �÷������� �׳� �����ϴ½����� �������� �ٵ� ���� �����غ��� ���� �÷����� �޾ƿͼ� �÷����� ������ �����ϴ½����� �ص� ���������� �ϴ� ������ ������
                yield return new WaitForSeconds(downTime);//downtime������ ���߿� �߷� ������ �������� ��ٸ� �����ʿ�
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), false);
                canDown = false;
            }
            yield return null;


        }


    }

    Vector3 HideDir(Collider2D collision)//���� �Լ�
    {
        int check = CheckDir(collision.transform.position);//���� ������Ʈ�� �÷��̾��� ��ġ�� Ȯ���ؼ� ��ġ�� ���� ������
        Vector3 correct_pos = Vector3.zero;//���� ������ ��ġ�� ������� ����

        Debug.Log($"Ʈ���� üũ {Mathf.Sign(check)}");
        isHide = true;//����
        Debug.Log("Ʈ���ž��买");

        if ((int)Mathf.Sign(check) < 0)//���� = ������ ������Ʈ�� ���� �� �����ʿ� �ִ�
        {
            this.gameObject.transform.GetChild(0).transform.localPosition = new Vector3(-0.78f, 0, 0);//0.78�̶�� ���� ���� �������� �����Ѱ�
            correct_pos = new Vector3(collision.bounds.max.x, transform.position.y, transform.position.z);//���� ������Ʈ�� ��ġ�� ���� ���°� �̰� ���ݺ��ϱ� �׳� ��ġ���� �Ǿ����� ��ġ���� �� �Ǿ��־ �����ʿ�
        }
        else//��� ������ ������Ʈ�� ���� ���ʿ� �ִ�
        {
            this.gameObject.transform.GetChild(0).transform.localPosition = new Vector3(0.78f, 0, 0);
            correct_pos = new Vector3(collision.bounds.min.x, transform.position.y, transform.position.z);//���� ������Ʈ�� ��ġ�� ���� ���°� �̰� ���ݺ��ϱ� �׳� ��ġ���� �Ǿ����� ��ġ���� �� �Ǿ��־ �����ʿ�
        }

        return correct_pos;
    }
    int CheckDir(Vector3 tr)
    {
        int check = (int)Mathf.Sign(tr.x - this.gameObject.transform.position.x);//������Ʈ�� ������ ���� ���� ������
        return check;
    }

    public bool _DrawReload(ref bool r_bool)//������ ���ӽð�? �ִϸ��̼� ���ӽð� ������ ����, �׸��� �ش��Լ��� ��ź�� true,false�� �����ָ鼭 �������� ������ �������̳ĸ� �˷���
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

    int sign;

    public Vector3 ClickPos()//Ŭ���� �·Ḧ �����ָ� ���� ���� Ŭ���� ĳ������ �ٶ󺸴� ���⵵ ���ؾ��Ѵٰ� �����ؼ� �ʿ��ߴ� �κ�
    {
        var screenPoint = Input.mousePosition;//���콺 ��ġ ������
        screenPoint.z = Camera.main.transform.position.z;
        worldPosition=Camera.main.ScreenToWorldPoint(screenPoint);
        int check = CheckDir(worldPosition);//Ŭ���� �κа� �÷��̾��� ��ġ�� ���� ���� ��������
        if (check<0){//�ϴ��� ĳ���Ͱ� ������ ���°��� �⺻�̶�� �����ߴµ� ���߿� �̹��� �޾ƿ��� �ش� �̹��� �־ �ٽ� �����ؾ��Ҽ������� rotation���ø� 
            this.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
            sign = -1;
        }
        else
        {
            this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            sign = 1;
        }



        return worldPosition;
    }
}