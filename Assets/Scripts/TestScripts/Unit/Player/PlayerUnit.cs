using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 유닛의 기본 동작을 정의, UnitBase를 상속한 추상 클래스
/// </summary>
public abstract class PlayerUnit : UnitBase
{
    public GameObject coverBox;

    protected bool isGrounded;
    private bool isJumpReady;
    protected bool isJumping;
    private bool jumpKey;
    private bool jumpKeyUp;

    protected float hzVel;

    public Rigidbody2D rg;

    private GameObject currentOneWayPlatform;
    private float downTime;
    private bool canDown;
    private bool cTemp;
    private bool isHide;
    private bool findRayPlatform;
    private float check;
    private float checkHigh;
    private float distanceToCheck;
    private LayerMask lm;
    private Vector3 hidePos;

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("ground") || collision.gameObject.CompareTag("platform")) isGrounded = true;

        if (collision.gameObject.tag == "platform")//지면 확인 점프용
        {
            checkHigh= -100;
            //WereWolf.Instance().isAttacking = false;// 이 부분이 있으면 땅에서 연속 공격 가능 
            currentOneWayPlatform = collision.gameObject;//플랫폼이라면 현재 플렛폼을 담음
            // Debug.Log(collision.gameObject.GetComponent<PlatformScript>().dObject);//다운 오브젝트 타입확인용 로그
            switch (collision.gameObject.GetComponent<PlatformScript>().dObject)//대각선 직선 오브젝트 마다 떨어지는 시간이 다를수도 있으니
            {
                case PlatformScript.downJumpObject.STRAIGHT://직선
                    downTime = 0.4f;//떨어지는 시간 다르게 하기 위함
                    break;
                case PlatformScript.downJumpObject.DIAGONAL://대각선
                    downTime = 0.6f;//떨어지는 시간 다르게 하기 위함
                    break;
            }
            //canDown = true;
        }

        if (collision.gameObject.tag == "cover")//엄페물 확인용 엄페용 지금 보니까 필요없는거같음 왜냐하면 엄페물은 지금 trigger로 발동중
        {
            Debug.Log("엄페물");
        }
    }
    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("ground") || collision.gameObject.CompareTag("platform")) isGrounded = false;
        if (collision.gameObject.tag == "platform")
        {
            if (isJumping)
            {
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), false);
            }
            currentOneWayPlatform = null;//platform에서 벗어난거라면 플랫폼 변수를 비움
        }
    }
    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("ground") || collision.gameObject.CompareTag("platform")) isGrounded = true;

        if (collision.gameObject.tag == "platform")//플랫폼이라면 현재 플렛폼을 담음
        {
            currentOneWayPlatform = collision.gameObject;
        }
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("트리거");

        if (collision.gameObject.tag == "cover")//엄폐물일때
        {
            // Debug.Log($"경계 {collision.bounds.min.x}");
            hidePos = HideDir(collision);//HideDir 함수 실행 해당 함수는 숨음 + 방향 고정
        }
    }
    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "cover")//위와 같음 이부분을 추가로 해둔것은 숨는 위치 오브젝트 속에 들어온상태에서 숨었을때를 위함
        {
            hidePos = HideDir(collision);
        }
    }
    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "cover")//만약 밖으로 벗어날경우를 위해 만든것 하지만 지금 엄폐중에 이동 불가능하기에 굳이 필요는 없긴함 만약 이동가능하다면 필요한부분
        {
            // Debug.Log("사라짐");
            isHide = false;
            coverBox.SetActive(false);
        }
    }

    protected override void Start()
    {
        base.Start();
        lm = ~(1 << gameObject.layer);
        distanceToCheck = boxSizeY * 1.05f;
        isJumpReady = true;
        StartCoroutine(DownJump());
    }

    protected override void Update()
    {
        if(!isGrounded && unitState == UnitState.Default) unitState = UnitState.Air; // 기본 상태에서 공중에 뜰 시 공중 상태로 변경
        else if(isGrounded && unitState == UnitState.Air) unitState = UnitState.Default; // 공중 상태에서 바닥에 닿을 시 기본 상태로 변경
        // Debug.Log(rg.velocity);
        JumpUpdate();
        CrouchUpdate();
        AddGravity();
        Movement();
        base.Update();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ResetForce();
    }

    public override bool Jump(KeyState jumpKey)
    {
        JumpUpdate(jumpKey); // 점프 입력 후처리

        if(!isJumping) return false; // 점프 중이 아닐 시 동작 종료
        if(ControllerChecker()) // 조작이 가능하지 않은 상태일 때 점프키 리셋 & 동작 종료
        {
            JumpReset();
            return false;
        }

        float temp = 0;
        if(vcForce <= 0) temp = jumpForce * 0.2f; // 점프 시작 시 힘을 초기화
        temp += Mathf.Lerp(temp, jumpForce, 0.02f); // 점프가 고점에 다다를수록 적게 힘을 추가

        temp += -gravity * Time.deltaTime; // 중력 무시를 위해 중력 값 만큼 힘 추가
        return AddVerticalForce(temp);
    }

    /// <summary>
    /// 점프 입력과 관련된 변수를 초기화하는 작업을 수행
    /// </summary>
    protected void JumpReset()
    {
        this.jumpKey = false;
        jumpKeyUp = false;
        isJumping = false;
        isJumpReady = true;
    }

    
    /// <summary>
    /// 점프 입력을 후처리하는 작업을 수행
    /// <param name="jumpKey">점프 키 입력 상태</param>
    /// </summary>
    protected void JumpUpdate(KeyState jumpKey)
    {
        switch(jumpKey)
        {
            case KeyState.KeyDown:
            case KeyState.KeyStay:
                this.jumpKey = true;
            break;
            case KeyState.KeyUp:
                if(!jumpKeyUp) jumpKeyUp = true;
                this.jumpKey = false;
            break;
            case KeyState.None:
            default:
                this.jumpKey = false;
            break;
        }
        JumpUpdate();
    }
    /// <summary>
    /// 점프 입력을 후처리하는 작업을 수행
    /// </summary>
    protected void JumpUpdate()
    {
        if(isGrounded && jumpKeyUp) // 바닥에 있으면서 점프키를 땠었던 상태일 시 점프 가능
        {
            isJumpReady = true;
            jumpKeyUp = false;
        }
        if(isJumpReady && this.jumpKey) // 점프 가능 & 점프 누를 시
        {
            isJumping = true; // 점프 중
            if(jumpKeyUp || vcForce >= jumpForce * 0.8f) isJumpReady = false; // 고점 도달 시 점프 비활성화
        }
        else isJumping = false; // 점프 중 아님
    }

    public override bool Move(float dir)
    {
        if(ControllerChecker() || unitState == UnitState.Hide) return false; // 제어가 불가능한 상태일 경우 동작을 수행하지 않음
        hzVel += (dir-hzVel) * accelerate; // 가속도만큼 입력 방향에 힘을 추가
        if(dir == 0) hzVel += -hzVel * accelerate; // 입력이 없을 시 수평힘을 줄임
        hzVel = Mathf.Clamp(hzVel, -1, 1); // 움직임 가속 제한
        return AddHorizontalForce(hzVel * movementSpeed);
    }

    // 수정 필요함
    public override bool Crouch(KeyState crouchKey)
    {
        switch(crouchKey)
        {
            case KeyState.KeyDown:
            case KeyState.KeyStay:
                if (isHide && unitState != UnitState.Air)//숨는 오브젝트와 상호 작용이 가능하다면
                {
                    unitState = UnitState.Hide;
                    // if (check < 0)
                    // {//일단은 캐릭터가 우츨을 보는것을 기본이라고 생각했는데 나중에 이미지 받아오면 해당 이미지 넣어서 다시 수정해야할수도있음 rotation관련만 
                    //     this.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                    // }
                    // else
                    // {
                    //     this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                    // }
                    coverBox.SetActive(true);
                    // coverBox.transform.position = this.gameObject.transform.GetChild(0).transform.position;//가드 위치를 가드 포인트 위치로 옮김
                    transform.position = hidePos;
                    rg.velocity = Vector2.zero;//만약 점프가 되려고하면 x만 0으로 초기화
                    rg.position = transform.position;
                    SetVel(0);
                    // charactor.Crouch(guard);
                    // isCrouching = true;
                    // isMoving = false;
                }
                if (currentOneWayPlatform != null)//밑 아래 점프 가능한 오브젝트와 닿아있을때 ,우선순위 따라서 위로 올리고 return이 필요할듯 
                {
                    // Debug.Log("hello");
                    canDown = !isHide;
                }
                return true;
            case KeyState.KeyUp:
                if(unitState == UnitState.Hide) unitState = UnitState.Default;
                coverBox.SetActive(false);//s를 누르지 않았으므로 가드를 푼다
                // isCrouching = false;
                return true;
        }
        return false;
    }
    private void CrouchUpdate()
    {
        var charBoxCollider = GetComponent<BoxCollider2D>();
        RaycastHit2D[] hit =Physics2D.RaycastAll(transform.parent.position, Vector2.down, distanceToCheck, lm);
        Debug.DrawRay(transform.position, Vector2.down * distanceToCheck, Color.red);
        //BoxCollider2D box = GetComponent<BoxCollider2D>();
        Vector2 test = new Vector2(transform.position.x + charBoxCollider.size.x/2, transform.position.y - charBoxCollider.size.y/2)  - (Vector2)transform.position;
        RaycastHit2D dHit = Physics2D.Raycast(transform.position, test,(MathF.Sqrt(charBoxCollider.size.x / 2) + MathF.Sqrt(charBoxCollider.size.y/2)) * 1.45f, 1<<LayerMask.NameToLayer("OneWayPlatform"));//플랫폼감지용 레이,하드 코딩때 값 0.75f,0.5에서 0.45로 수정함으로서 collider보다 더 길게설정 
        Vector2 test2 = new Vector2(transform.position.x - charBoxCollider.size.x / 2, transform.position.y - charBoxCollider.size.y / 2) - (Vector2)transform.position;
        RaycastHit2D d2Hit = Physics2D.Raycast(transform.position, test2, (MathF.Sqrt(charBoxCollider.size.x / 2) + MathF.Sqrt(charBoxCollider.size.y / 2))*1.45f, 1 << LayerMask.NameToLayer("OneWayPlatform"));//플랫폼감지용 레이,0.5에서 0.45로 수정함으로서 collider보다 더 길게설정
        Debug.DrawRay(transform.position, test.normalized*0.75f, Color.blue);
        
        if (hit != null)
        {
            
            var index = Array.FindIndex(hit, x => x.transform.tag == "ground");//만약 람다를 안 쓰려면 for로 hit만큼 돌ㅡㅜ   아가면서 태그가 맞는지 확인해야함
            if(Array.FindIndex(hit, x => x.transform.tag == "platform") != -1)
            {
                // Debug.Log("플랫폼 감지중");
                isGrounded = true;
                findRayPlatform = true;//여기는 없어도 무방
            }
            else
            {
                // Debug.Log("플랫폼 가지 못 함");
                if (index == -1)
                {
                    //아무것도 못 찾음
                    isGrounded = false;
                }
                findRayPlatform=false;
            }
            //var index = Array.FindIndex(hit, x => x.transform.tag == "ground");
            if (index != -1)
            {
                // Debug.Log("그라운드 찾음");
                isGrounded = true;
                cTemp = true;
            }
            else
            {
                // Debug.Log("그라운드 못 찾음");
                isGrounded = false;
                cTemp = false;
            }
            
        }

        if (dHit.collider == null)
        {
            // Debug.Log("dHit null");
        }
        else
        {
            // Debug.Log("find dHit"+dHit.collider.name);
        }
        if (cTemp&&!isHide)//temp가 true 일때 무시함
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), true);
        }
        else if(!canDown&&(dHit.collider==null&&d2Hit.collider==null))//캐릭터 좌우 대각선 부분에서 플랫폼이 감지가 안될경우
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), false);
        }
    }
    IEnumerator DownJump()
    {
        while (true)
        {
            if (canDown)//아래 점프 가능한 오브젝트 만날경우
            {
                // Debug.Log("hi");
                cTemp = true;
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), true);//원리는 그냥 설정한 시간동안 해당 플렛폼들을 그냥 무시하는식으로 설정했음 근데 지금 생각해보면 지금 플렛폼을 받아와서 플렛폼의 네임을 무시하는식으로 해도 되지않을까 하는 영감이 떠오름
                yield return new WaitForSeconds(downTime);//downtime변수는 나중에 중력 설정시 이질감이 든다면 변경필요
                cTemp = false;
                //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), false);
                canDown = false;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 현재 플레이어 유닛의 제어 가능 여부 확인
    /// </summary>
    /// <returns>플레이어 유닛이 제어 불가능한 상태일 시 true를 반환</returns>
    protected bool ControllerChecker() => ControllerChecker(this);

    /// <summary>
    /// 주어진 플레이어 유닛의 제어 가능 여부 확인
    /// </summary>
    /// <param name="playerUnit">확인할 플레이어 유닛</param>
    /// <returns>플레이어 유닛이 제어 불가능한 상태일 시 true를 반환</returns>
    public static bool ControllerChecker(PlayerUnit playerUnit)
    {
        var unitState = playerUnit.UnitState;
        if(unitState == UnitState.KnockBack || unitState == UnitState.Stiffen || 
            unitState == UnitState.Stiffen_er || unitState == UnitState.Death || 
            unitState == UnitState.Pause) return true;
        else return false;
    }

    /// <summary>
    /// 중력 힘을 추가
    /// </summary>
    private void AddGravity()
    {
        AddVerticalForce(gravity * Time.deltaTime);
    }

    /// <summary>
    /// 수직 방향 힘을 추가
    /// </summary>
    protected bool AddVerticalForce(float force)
    {
        if(isGrounded && vcForce < 0) vcForce = 0; // 바닥에 붙어있을 시 아래 방향의 힘 초기화
        vcForce += force;
        return true;
    }
    /// <summary>
    /// 수평 방향 힘을 추가
    /// </summary>
    protected bool AddHorizontalForce(float force)
    {
        hzForce = force;
        return true;
    }

    /// <summary>
    /// 플레이어 유닛 힘을 리지드바디로 전달
    /// </summary>
    private void Movement() => rg.velocity = new Vector3(hzForce, vcForce);

    /// <summary>
    /// 현재 플레이어 유닛의 모든 힘을 초기화
    /// </summary>
    public void ResetForce()
    {
        hzForce = 0;
        vcForce = 0;
    }

    /// <summary>
    /// 수평 속도 설정
    /// </summary>
    /// <param name="vel">설정할 수평 속도</param>
    public void SetVel(float vel) => hzVel = vel;

    Vector3 HideDir(Collider2D collision)//숨는 함수
    {
        check = CheckDir(collision.transform.position);//현재 오브젝트와 플레이어의 위치를 확인해서 위치의 차를 보내줌
        Vector3 correct_pos = Vector3.zero;//새로 수정함 위치를 담기위한 변수

        // Debug.Log($"트리거 체크 {Mathf.Sign(check)}");
        isHide = isGrounded;//땅에 없을때만 true하기 위해 ,isGround&&true와 동일
        // Debug.Log("트리거엄페물");
        //charBoxCollider = this.GetComponent<BoxCollider2D>();

        if ((int)Mathf.Sign(check) < 0)//음수 = 유저가 오브젝트에 비해 더 오른쪽에 있다
        {
            //this.gameObject.transform.GetChild(0).transform.localPosition = new Vector3(-0.78f, 0, 0);//0.78이라는 값은 내가 보았을때 적당한값
            
            correct_pos = new Vector3(collision.bounds.max.x+boxSizeX/2, transform.position.y, transform.position.z);//숨는 오브젝트의 위치를 따서 숨는것 이거 지금보니까 그냥 겹치도록 되어있음 걸치도록 안 되어있어서 수정필요
        }
        else//양수 유저가 오브젝트에 비해 왼쪽에 있다
        {
            //this.gameObject.transform.GetChild(0).transform.localPosition = new Vector3(0.78f, 0, 0);
            correct_pos = new Vector3(collision.bounds.min.x-boxSizeX/2, transform.position.y, transform.position.z);//숨는 오브젝트의 위치를 따서 숨는것 이거 지금보니까 그냥 겹치도록 되어있음 걸치도록 안 되어있어서 수정필요
        }
        

        return correct_pos;
    }
}