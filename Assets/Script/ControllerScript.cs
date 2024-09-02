using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static PlatformScript;
using Debug = UnityEngine.Debug;//아까 모호하다고 해서 해둔것




public class JumpState//굳이 클래스여야할까
{
    public float jumpPower = 0;//점프 힘
    public bool isJump = false;//아래랑 같음ㄴ
    public float jumpStartTime = 0;
    public float jumpHight;
    public State jumptype;

    

    public enum State { IDLE, NORMAL_JUMP, LONG_JUMP };//점프 상태에따라서 다른점프를 만들기 위해 만든 enum이었음
}

public partial class ControllerScript : MonoBehaviour
{
    //bool isCancle = false;
    
    bool isCancle = false;//캔슬에 들어가는 변수들 캔슬 제어용
    float t = 0;//캔슬에 들어가는 변수들 캔슬 제어용

    private static ControllerScript instance = null;
    public static ControllerScript Instance
    {
        get
        {
            if (instance == null) return null;
            return instance;
        }
    }

    private ControllerScript() { }

    //IEnumerator pointFiveTimer;


    IEnumerator ReloadCancleTimer()
    {
        t = 0;
        while (true)
        {
            if (isCancle)
            {
                t += Time.deltaTime;
                if (t < 2)
                {
                    UIController.Instance.ImageSetFalse();
                    ControllerScript.instance.currentTime = 0;//?
                }
                else
                {
                    UIController.Instance.ImageSetTrue();
                }
                Debug.Log("재장전");
            }
            else
            {
                isCancle = false;
                t = 0;
            }
            yield return null;

        }
    }

    public bool b_reload = false;//재장전 컨트롤 bool값
    public float currentTime = 0;//재장전관련부분
    public float duration = 1f;//재장전 시간 나중에 늑대 재장전은 2초로 설정하도록 클래스 수정필요 SetDuration()가상함수로 만들고 인간은 1초 늑대는 2초로 설정

    Rigidbody2D rg2d;//이번 프로젝트에서는 rigdbody를 이용해서 움직일것임

    JumpState jumpState = new JumpState();//점프 상태를 위하 해당 클래스 현재 사용안함
    Charactor charactor = new Charactor();


    public float jumpForce = 5f;//점프 힘 해당 부분은 나중에 클래스로 통합해서 보기 편하게 수정해야함
    public float jumpDuration = 0.5f;//긴 점프 할 수 있는 시간 해당 부분은 나중에 클래스로 통합해서 보기 편하게 수정해야함
    // [SerializeField]
    // private bool isMoving = false;//update에서 fixedUpdate로 움직임 전해주기 위함 
    [SerializeField]
    private bool isGround = true;//점프 및 공격한것에 대한 테스트를 위함움직이면서 만약 안 된다면 다시 변수 만들어야함 공격을 했는지 판단하기 위함  
    public bool isHide = false;//크라우치 할 수 있는지 확인 용변수
    public bool isCrouching = false;//크라우치 중인지 확인뇽,나중에 charactor로 들어가도 괜찮을듯
    private bool isJumping;
    private bool isJumpReady;
    private const float gravity = -9.81f;

    private LayerMask lm;

    [SerializeField] private float speed;
    [SerializeField] private float accelerate;


    private float moveVec = 0;
    private float velocity = 0;

    private float distanceToCheck = 0.5f;

    private bool jumpKey;
    private bool jumpKeyUp;

    int check;//방향체크 위한 변수
    bool test = false;


    [SerializeField] private GameObject currentOneWayPlatform;//떨어질수 있느 ㄴ바닥관련 오브젝트 담는 변수

    public float downTime = 0.4f;//다운 점프하면 오브젝트 충돌 무시하는 시간
    public bool canDown = false;//다운 점프 가능 구간 확인


    public Vector3 worldPosition;//클릭하는 위치를 담기위한 변수
    public float moveSpeed = 5;//캐릭터 이동속도 관련 부분 해당 부분은 캐릭터 폼 체인지시 speed오 ㅏ관련 있음 아래부분 보면 폼체인지에 붙어있음
    private float InxPos;//이동 관련 변수 이 부분은 나중에 이동과 통합시 삭제 가능

    Vector2 vec;//레이 위한 것

    [SerializeField] private GameObject guard;//숨었을때 만는는것

    int test_vec;//해당변수는 벽 점프테스트를 위해 만든 변수로 벽점프가 완전히 구현이 되고나서 사용하지 않는다면 지워도 무방 해당 변수는 collisionEnter시에 캐릭터가 어느편에서 벽에 충돌한지 확인하는 변수임

    bool temp = false;
    float checkHigh = -100;
    bool isDown = false;

    public bool findRayPlatform = false;

    BoxCollider2D charBoxCollider;

    private void Awake()
    {
        vec = Vector2.left;//캐릭터 방향키 입력에 따른 레이 변경을 위해
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        StartCoroutine(DownJump());//아래 점프 코루틴 해당 함
        StartCoroutine(ReloadCancleTimer());

        charactor = Human.Instance();
        charactor.isHuman = true;
    }

    void Start()
    {
        rg2d = GetComponent<Rigidbody2D>();
        lm = ~(1 << gameObject.layer);
        isJumpReady = true;
        charBoxCollider =this.GetComponent<BoxCollider2D>();
        distanceToCheck =charBoxCollider.size.y/2 * 0.7f;
    }


    private Vector2 collisionVec;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground" || collision.gameObject.tag == "platform")//지면 확인 점프용
        {
            isGround = true;
            checkHigh= -100;
            //WereWolf.Instance().isAttacking = false;// 이 부분이 있으면 땅에서 연속 공격 가능 
            if (collision.gameObject.tag == "platform")
            {
                currentOneWayPlatform = collision.gameObject;//플랫폼이라면 현재 플렛폼을 담음
                Debug.Log(collision.gameObject.GetComponent<PlatformScript>().dObject);//다운 오브젝트 타입확인용 로그
                switch (collision.gameObject.GetComponent<PlatformScript>().dObject)//대각선 직선 오브젝트 마다 떨어지는 시간이 다를수도 있으니
                {
                    case downJumpObject.STRAIGHT://직선
                        downTime = 0.4f;//떨어지는 시간 다르게 하기 위함
                        break;
                    case downJumpObject.DIAGONAL://대각선
                        downTime = 2f;//떨어지는 시간 다르게 하기 위함
                        break;
                }
                //canDown = true;
            }
        }

        if (collision.gameObject.tag == "cover")//엄페물 확인용 엄페용 지금 보니까 필요없는거같음 왜냐하면 엄페물은 지금 trigger로 발동중
        {
            Debug.Log("엄페물");
        }

        if (collision.gameObject.tag == "enemyBullet")//플레이어가 총알을 맞을 경우를 적은것 좌에서 맞은지 우에서 맞은지
        {
            Debug.Log("collision hit");
            Vector2 pos = collision.GetContact(0).point;




            float posCheck = Mathf.Sign(transform.position.x - pos.x);
            string leftright = "";
            leftright = (posCheck > 0) ? "좌" : "우";
            Debug.Log($"충돌 위치 : {pos} 위치 차이 {posCheck} {leftright} 에서 피격");

        }

        if (collision.gameObject.tag == "Wall" && !charactor.isHuman)
        {

            Debug.Log("벽 충돌 위치" + CheckDir(collision.transform.position));

            test = true;
            rg2d.velocity = Vector2.zero;
            test_vec = CheckDir(collision.transform.position); 
            collisionVec = collision.transform.position;
        }



    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground" || collision.gameObject.tag == "platform")//땅에서 벗어날경우
        {
            //isGround = false;
            if (collision.gameObject.tag == "platform")
            {
                if (isJumping)
                {
                    Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), false);
                }
                currentOneWayPlatform = null;//platform에서 벗어난거라면 플랫폼 변수를 비움

            }
        }
        if (collision.gameObject.tag == "Wall")
        {
            rg2d.gravityScale = 1;
            test = false;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground" || collision.gameObject.tag == "platform")
        {
            isGround = true;
            if (collision.gameObject.tag == "platform")//플랫폼이라면 현재 플렛폼을 담음
            {
                currentOneWayPlatform = collision.gameObject;
            }
        }
        if (collision.gameObject.tag == "Wall" && !charactor.isHuman)//벽에 쭉 붙어있는 지금 모습 수정하기 위해 만들었던 변수
        {

            Debug.Log("벽 스테이 템플스테이 아님");

            test = true;


            if (test)
            {
                rg2d.gravityScale = 0f;
                if(velocity <= 0) velocity = 0;
                //rg2d.velocity = Vector2.zero;
            }

        }


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("트리거");

        if (collision.gameObject.tag == "cover")//엄폐물일때
        {
            Debug.Log($"경계 {collision.bounds.min.x}");
            charactor.hidePos = HideDir(collision);//HideDir 함수 실행 해당 함수는 숨음 + 방향 고정
        }

        if (collision.gameObject.tag == "ammo")//총알이라면
        {
            Debug.Log("총알 획득");
            if (Charactor.spare_ammo + Charactor.ammo < 2)
            {
                Debug.Log("총알을 획득합니다");
                charactor.GetAmmo();//GetAmmo의 조건을 없애도 될듯 중복된 조건임
                Destroy(collision.gameObject);//습득했으므로 탄창오브젝트삭제
                //Human.Instance().Reload();
            }

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "cover")//위와 같음 이부분을 추가로 해둔것은 숨는 위치 오브젝트 속에 들어온상태에서 숨었을때를 위함
        {
            charactor.hidePos = HideDir(collision);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "cover")//만약 밖으로 벗어날경우를 위해 만든것 하지만 지금 엄폐중에 이동 불가능하기에 굳이 필요는 없긴함 만약 이동가능하다면 필요한부분
        {
            Debug.Log("사라짐");
            isHide = false;
            guard.SetActive(false);
        }
    }

    private void FixedUpdate()
    {

        if (!isCrouching && !WereWolf.Instance().isAttacking)//조건이 복잡한데 움직임을 입력 받은 상태며 숨지 않았으며 공격중이 아닐때, 즉 그냥 이동 + 점프상태만 받음
        {
            if (!test)
            {
                Move(new Vector3(HorizontalForce(), VerticalForce()) * Time.deltaTime);
            }
            
        }

    }
    private float VerticalForce()
    {
        velocity += gravity * Time.deltaTime;
        if (isGround && velocity < 0) velocity = 0;
        if (isJumping)
        {
            if (velocity <= 0) velocity = jumpForce / 2;
            velocity = Mathf.Sin(velocity / jumpForce) * jumpForce * 1.2f;
        }
        return velocity;
    }

    private bool CheckHigh()
    {
        if (checkHigh <= transform.position.y)
        {
            checkHigh = transform.position.y;
            return false;
        }
        else
        {
            return true;
        }
    }
    private float HorizontalForce()
    {
        isDown = CheckHigh();
        if (transform.rotation.y != 0)
        {
            return -(moveVec * speed);
        }
        return moveVec * speed;
    }
    private void Move(Vector3 force) => transform.Translate(force);

    private void _Attack()//공격시 각 캐릭터별 공격 수행
    {
        Debug.Log("Attack");
        charactor.Attack();

    }
    public void ReloadCancle()//장전 캔슬을 위함
    {
        if (b_reload)
        {
            Debug.Log("공격 캔슬");
            isCancle = b_reload;
            t = 0;
        }
    }

    private void Formchange()//폼체인지시 인스턴스를 넣어서 실행함
    {
        //if(charactor is WereWolf)//아래와 동일
        //{
        //
        //}
        //else
        //{
        //
        //}
        if (charactor.isHuman)//사람일때 변신
        {
            charactor = WereWolf.Instance();
            charactor.isHuman = false;
            Debug.Log("늑대로 변경");
        }
        else//늑대일때 변신
        {
            charactor = Human.Instance();
            charactor.isHuman = true;
            Debug.Log("사람으로 변경");
        }
        charactor.SetInfo();
        //charactor.Setspeed();
        moveSpeed = charactor.speed;


        //구르기시 앞으로
        rg2d.AddForce(Vector2.right * 100 * Mathf.Sign(transform.rotation.y));

    }

    private void InputManager()//말이 InputManager지만 Update에서 혼자 움직임
    {
        if (Input.GetMouseButtonDown(0))//좌클릭 공격시
        {
            _Attack();
            //currentTime = 0;//해당부분 수정 필요 지금은 즉시 취소 되고 다시 재장전이 되지만 

        }
        if (Input.GetMouseButtonDown(1) && !isCrouching)//우클릭, 크라우치 안 하고있을때 폼체인지
        {
            Formchange();
            ReloadCancle();//재장전 중이었다면 장전캔슬
            //currentTime = 0;//해당부분 수정 필요 지금은 즉시 취소 되고 다시 재장전이 되지만 
        }
        if (!WereWolf.Instance().isAttacking)
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }


        jumpKey = Input.GetKey(KeyCode.W);



        if (!jumpKeyUp) jumpKeyUp = Input.GetKeyUp(KeyCode.W);
        if (isGround && jumpKeyUp)
        {
            isJumpReady = true;
            jumpKeyUp = false;
        }
        if (isJumpReady && jumpKey)
        {
            Debug.Log("점프");
            isJumping = true;
            if (jumpKeyUp) isJumpReady = false;
            if (velocity / jumpForce >= 0.9f) isJumpReady = false;
        }
        else isJumping = false;


        if (Input.GetKey(KeyCode.S))//S를 누를때
        {
            if (isHide)//숨는 오브젝트와 상호 작용이 가능하다면
            {
                if (check < 0)
                {//일단은 캐릭터가 우츨을 보는것을 기본이라고 생각했는데 나중에 이미지 받아오면 해당 이미지 넣어서 다시 수정해야할수도있음 rotation관련만 
                    this.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                guard.transform.position = this.gameObject.transform.GetChild(0).transform.position;//가드 위치를 가드 포인트 위치로 옮김
                transform.position = charactor.hidePos;
                rg2d.velocity = Vector2.zero;//만약 점프가 되려고하면 x만 0으로 초기화
                //rg2d.position = transform.position;
                charactor.Crouch(guard);
                isCrouching = true;
                // isMoving = false;
            }
            if (currentOneWayPlatform != null)//밑 아래 점프 가능한 오브젝트와 닿아있을때 ,우선순위 따라서 위로 올리고 return이 필요할듯 
            {
                Debug.Log("hello");

                canDown = !isHide;

            }

        }
        else
        {
            guard.SetActive(false);//s를 누르지 않았으므로 가드를 푼다
            isCrouching = false;
        }

        RaycastHit2D[] hit =Physics2D.RaycastAll(transform.position, Vector2.down, distanceToCheck, lm);
        Debug.DrawRay(transform.position, Vector2.down * distanceToCheck, Color.red);
        //BoxCollider2D box = GetComponent<BoxCollider2D>();
        Vector2 test = new Vector2(transform.position.x + charBoxCollider.size.x/2, transform.position.y - charBoxCollider.size.y/2)  - (Vector2)transform.position;
        RaycastHit2D dHit = Physics2D.Raycast(transform.position, test,(MathF.Sqrt(charBoxCollider.size.x / 2) + MathF.Sqrt(charBoxCollider.size.y/2)) * 0.48f, 1<<LayerMask.NameToLayer("OneWayPlatform"));//플랫폼감지용 레이,하드 코딩때 값 0.75f,0.5에서 0.45로 수정함으로서 collider보다 더 길게설정 
        Vector2 test2 = new Vector2(transform.position.x - charBoxCollider.size.x / 2, transform.position.y - charBoxCollider.size.y / 2) - (Vector2)transform.position;
        RaycastHit2D d2Hit = Physics2D.Raycast(transform.position, test2, (MathF.Sqrt(charBoxCollider.size.x / 2) + MathF.Sqrt(charBoxCollider.size.y / 2))*0.48f, 1 << LayerMask.NameToLayer("OneWayPlatform"));//플랫폼감지용 레이,0.5에서 0.45로 수정함으로서 collider보다 더 길게설정
        Debug.DrawRay(transform.position, test.normalized*0.75f, Color.blue);

        if (hit != null)
        {
            
            var index = Array.FindIndex(hit, x => x.transform.tag == "ground");//만약 람다를 안 쓰려면 for로 hit만큼 돌ㅡㅜ   아가면서 태그가 맞는지 확인해야함
            if(Array.FindIndex(hit, x => x.transform.tag == "platform") != -1)
            {
                Debug.Log("플랫폼 감지중");
                isGround = true;
                findRayPlatform = true;//여기는 없어도 무방
            }
            else
            {
                Debug.Log("플랫폼 가지 못 함");
                if (index == -1)
                {
                    //아무것도 못 찾음
                    isGround = false;
                }
                findRayPlatform=false;
            }
            //var index = Array.FindIndex(hit, x => x.transform.tag == "ground");
            if (index != -1)
            {
                Debug.Log("그라운드 찾음");
                isGround = true;
                temp = true;
            }
            else
            {
                Debug.Log("그라운드 못 찾음");
                //isGround = false;
                temp = false;
            }
            
        }

        if (dHit.collider == null)
        {
            Debug.Log("dHit null");
        }
        else
        {
            Debug.Log("find dHit"+dHit.collider.name);
        }
        if (temp&&!isHide)//temp가 true 일때 무시함
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), true);
        }
        else if(!canDown&&(dHit.collider==null&&d2Hit.collider==null))//캐릭터 좌우 대각선 부분에서 플랫폼이 감지가 안될경우
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), false);
        }

        moveVec += (Input.GetAxisRaw("Horizontal") - moveVec) * accelerate;

        if (Input.GetAxisRaw("Horizontal") == 0) moveVec += (Input.GetAxisRaw("Horizontal") - moveVec) * accelerate;
        moveVec = Mathf.Clamp(moveVec, -1, 1);

    }

    private void Update()
    {
        InputManager();
        // ConditionUpdate();


        if (charactor.isHuman)//인간상태일때 지속적으로 해야하는 함수들 넣기위함
        {

        }
        else//늑대 상태때 지속적으로 체크하거나 수행해야할 부분 넣어야함
        {
            if(test && Input.GetKeyDown(KeyCode.W))
            {
                rg2d.AddForce(new Vector2(-CheckDir(collisionVec), 1) * 1, ForceMode2D.Impulse);
            }
        }
        charactor.Reload();
        //_Raycast();
    }

    void _Raycast()//처음에 총알관련 충돌, 숨는 오브젝트 확인을 위해 ray를 쏘았는데 현재는 없이도 돌아가서 사용하지 않은 추후에 ray가 필요하다면 참고 할 생각으로 남겼음
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
        jumpState.jumpStartTime = Time.time;//점프 시간 측정
        rg2d.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        jumpState.isJump = true;
        jumpState.jumptype = JumpState.State.NORMAL_JUMP;
    }

    private void JumpHigher()
    {
        //Debug.Log("higer");
        rg2d.AddForce(Vector3.up * jumpForce * Time.deltaTime * 2, ForceMode2D.Impulse);//추가적인 힘을 계속 주는것 time.deltatime은 아주작은 소수점이 나올것이므로 곱하게 되면 값이 아주작아짐
    }

    IEnumerator DownJump()
    {
        while (true)
        {
            if (canDown)//아래 점프 가능한 오브젝트 만날경우
            {
                Debug.Log("hi");
                temp = true;
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), true);//원리는 그냥 설정한 시간동안 해당 플렛폼들을 그냥 무시하는식으로 설정했음 근데 지금 생각해보면 지금 플렛폼을 받아와서 플렛폼의 네임을 무시하는식으로 해도 되지않을까 하는 영감이 떠오름
                yield return new WaitForSeconds(downTime);//downtime변수는 나중에 중력 설정시 이질감이 든다면 변경필요
                temp = false;
                //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), false);
                canDown = false;
            }
            yield return null;


        }


    }

    Vector3 HideDir(Collider2D collision)//숨는 함수
    {
        check = CheckDir(collision.transform.position);//현재 오브젝트와 플레이어의 위치를 확인해서 위치의 차를 보내줌
        Vector3 correct_pos = Vector3.zero;//새로 수정함 위치를 담기위한 변수

        Debug.Log($"트리거 체크 {Mathf.Sign(check)}");
        isHide = isGround;//땅에 없을때만 true하기 위해 ,isGround&&true와 동일
        Debug.Log("트리거엄페물");
        //charBoxCollider = this.GetComponent<BoxCollider2D>();

        if ((int)Mathf.Sign(check) < 0)//음수 = 유저가 오브젝트에 비해 더 오른쪽에 있다
        {
            //this.gameObject.transform.GetChild(0).transform.localPosition = new Vector3(-0.78f, 0, 0);//0.78이라는 값은 내가 보았을때 적당한값
            
            correct_pos = new Vector3(collision.bounds.max.x+charBoxCollider.size.x/4, transform.position.y, transform.position.z);//숨는 오브젝트의 위치를 따서 숨는것 이거 지금보니까 그냥 겹치도록 되어있음 걸치도록 안 되어있어서 수정필요
        }
        else//양수 유저가 오브젝트에 비해 왼쪽에 있다
        {
            //this.gameObject.transform.GetChild(0).transform.localPosition = new Vector3(0.78f, 0, 0);
            correct_pos = new Vector3(collision.bounds.min.x-charBoxCollider.size.x/4, transform.position.y, transform.position.z);//숨는 오브젝트의 위치를 따서 숨는것 이거 지금보니까 그냥 겹치도록 되어있음 걸치도록 안 되어있어서 수정필요
        }
        

        return correct_pos;
    }
    int CheckDir(Vector3 tr)
    {
        int check = (int)Mathf.Sign(tr.x - this.gameObject.transform.position.x);//오브젝트와 유저의 값에 따라 보내줌
        return check;
    }

    public bool _DrawReload(ref bool r_bool)//재장전 지속시간? 애니메이션 지속시간 설정을 위함, 그리고 해당함수가 끄탄면 true,false를 전해주면서 재장전이 끝났냐 지속중이냐를 알려줌
    {
        //재장전 애니메이션 및 내용들
        if (currentTime <= charactor.reloadTime)//1 = duration temp/duration 
        {
            currentTime += Time.deltaTime;
            Debug.Log("아직" + currentTime);
            r_bool = true;
            return false;
        }
        else
        {

            Debug.Log("1초 완");

            currentTime = 0;
            r_bool = false;
            return true;
        }
    }

    public Vector3 ClickPos()//클릭한 좌료를 보내주며 현재 공격 클릭시 캐릭터의 바라보는 방향도 변해야한다고 생각해서 필요했던 부분
    {
        var screenPoint = Input.mousePosition;//마우스 위치 가져옴
        screenPoint.z = Camera.main.transform.position.z;
        worldPosition = Camera.main.ScreenToWorldPoint(screenPoint);
        int check = CheckDir(worldPosition);//클릭한 부분과 플레이어의 위치에 대한 값을 전달해줌
        if (check < 0)
        {//일단은 캐릭터가 우츨을 보는것을 기본이라고 생각했는데 나중에 이미지 받아오면 해당 이미지 넣어서 다시 수정해야할수도있음 rotation관련만 
            this.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
            //GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            //GetComponent<SpriteRenderer>().flipX = false;
            this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }



        return worldPosition;
    }
}