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

    protected bool isJumping;

    protected float hzVel;

    public Rigidbody2D rg;

    private GameObject currentOneWayPlatform;
    private float downTime;
    private bool canDown;
    private bool cTemp;
    // private bool isHide;
    private bool findRayPlatform;
    // private float check;
    // private float checkHigh;
    private float distanceToCheck;
    private LayerMask lm;

    // private Vector3 hidePos;

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(CheckMapType(collision));
        Debug.Log(collision.gameObject.tag);
        switch (CheckMapType(collision))
        {
            case MapType.Platform:

                currentOneWayPlatform = collision.gameObject;//플랫폼이라면 현재 플렛폼을 담음
                // Debug.Log(collision.gameObject.GetComponent<PlatformScript>().dObject);//다운 오브젝트 타입확인용 로그
                switch (collision.gameObject.GetComponent<PlatformScript>().dObject)//대각선 직선 오브젝트 마다 떨어지는 시간이 다를수도 있으니  
                {
                    case PlatformScript.downJumpObject.STRAIGHT://직선
                        downTime = 1f;//떨어지는 시간 다르게 하기 위함
                        break;
                    case PlatformScript.downJumpObject.DIAGONAL://대각선
                        downTime = 0.8f;//떨어지는 시간 다르게 하기 위함 , 0.7초까지도 1칸에 대해서는 가능하지만 만약에 쭉 앞으로 가면서 떨어진다고 하면 안전한 시간은 0.75~0.8사이임
                        break;
                }
                //canDown = true;
                break;
            //default:
            //    
            //    break;
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        switch(CheckMapType(collision))
        {
            case MapType.Platform:
                Debug.Log("플랫폼 벗어남");
                if (isJumping)
                {
                    Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), false);
                    Debug.Log("점프");
                }
                currentOneWayPlatform = null;//platform에서 벗어난거라면 플랫폼 변수를 비움
                break;
        }
        // if(collision.gameObject.CompareTag("ground") || collision.gameObject.CompareTag("platform")) isGrounded = false;
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log(CheckMapType(collision));
        switch(CheckMapType(collision))
        {
            case MapType.Ceiling:
                isJumping = false;
                break;
            case MapType.Platform:
                currentOneWayPlatform = collision.gameObject;
                break;
        }
        if(CheckMapType(collision) == MapType.Ceiling) isJumping = false;
        // if(collision.gameObject.CompareTag("ground") || collision.gameObject.CompareTag("platform")) isGrounded = true;
    }

    protected override void Start()
    {
        base.Start();
        lm = ~(1 << gameObject.layer);
        distanceToCheck = boxSizeY * 1.05f;
        StartCoroutine(DownJump());
    }

    protected override void Update()
    {
        if(!isGrounded && unitState == UnitState.Default) unitState = UnitState.Air; // 기본 상태에서 공중에 뜰 시 공중 상태로 변경
        else if(isGrounded && unitState == UnitState.Air) unitState = UnitState.Default; // 공중 상태에서 바닥에 닿을 시 기본 상태로 변경
        Debug.Log(string.Format("{0}은 현재 오브젝트", currentOneWayPlatform));
        CrouchUpdate();
        base.Update();
    }

    private void FixedUpdate()
    {
        AddGravity();
        // AddFrictional();
        var hit = Physics2D.Raycast(transform.position, Vector2.right*Mathf.Sign(hzForce), boxSizeX*1.1f, 1<<LayerMask.NameToLayer("Map"));
        if(hit) SetHorizontalForce(0);
        Movement();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ResetForce();
    }

    private float jumpingHight;
    public override bool Jump(KeyState jumpKey)
    {
        float temp = -gravity * Time.deltaTime; // 중력 무시를 위해 중력 값 만큼 힘 추가
        switch(jumpKey)
        {
            case KeyState.KeyDown:
                if(isJumping) return false;
                isJumping = true;
                temp += jumpImpulse * Mathf.Cos(0); // 점프 시작 시 힘을 초기화

                return SetVerticalForce(temp);
            case KeyState.KeyStay:
                jumpingHight += Time.deltaTime / jumpTime;
                temp += Mathf.Lerp(jumpImpulse, jumpForce, jumpingHight) * Mathf.Cos(jumpingHight + -((jumpImpulse - jumpForce) / jumpImpulse)); // 점프가 고점에 다다를수록 적게 힘을 추가
                if(!isJumping || jumpingHight >= 1)
                {
                    isJumping = false;
                    jumpingHight = 0;
                    return false;
                }
                return SetVerticalForce(temp);
            case KeyState.KeyUp:
                isJumping = false;
                jumpingHight = 0;
            break;
        }
        return false;
    }

    public override bool Move(float dir)
    {
        if(dir == 0 || ControllerChecker()) hzVel += -hzVel * accelerate * Time.deltaTime;
        if(ControllerChecker()) return false; // 제어가 불가능한 상태일 경우 동작을 수행하지 않음
        base.Move(dir);
        hzVel += (dir-hzVel) * accelerate * Time.deltaTime; // 가속도만큼 입력 방향에 힘을 추가
        hzVel = Mathf.Clamp(hzVel, -1, 1); // 움직임 가속 제한
        return SetHorizontalForce(hzVel * movementSpeed);
    }

    // 수정 필요함
    public override bool Crouch(KeyState crouchKey)
    {
        switch(crouchKey)
        {
            case KeyState.KeyDown:
                Debug.Log("하향점프");
            case KeyState.KeyStay:
                if (currentOneWayPlatform != null)//밑 아래 점프 가능한 오브젝트와 닿아있을때 ,우선순위 따라서 위로 올리고 return이 필요할듯 
                {
                    Debug.Log("hello");
                    // canDown = !isHide;
                    canDown = true;
                    AddVerticalForce(gravity * Time.deltaTime);
                }
                else
                {
                    Debug.Log("여기에 걸림");
                }
                return true;
            case KeyState.KeyStay:
            break;
            case KeyState.KeyUp:
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
        RaycastHit2D dHit = Physics2D.Raycast(transform.position, test,(MathF.Sqrt(charBoxCollider.size.x / 2) + MathF.Sqrt(charBoxCollider.size.y/2)) * 0.6f, 1<<LayerMask.NameToLayer("OneWayPlatform"));//플랫폼감지용 레이,하드 코딩때 값 0.75f,0.5에서 0.45로 수정함으로서 collider보다 더 길게설정 
        

        Debug.DrawRay(transform.position,test, Color.green);
        Vector2 test2 = new Vector2(transform.position.x - charBoxCollider.size.x / 2, transform.position.y - charBoxCollider.size.y / 2) - (Vector2)transform.position;
        RaycastHit2D d2Hit = Physics2D.Raycast(transform.position, test2, (MathF.Sqrt(charBoxCollider.size.x / 2) + MathF.Sqrt(charBoxCollider.size.y / 2))*0.6f, 1 << LayerMask.NameToLayer("OneWayPlatform"));//플랫폼감지용 레이,0.5에서 0.45로 수정함으로서 collider보다 더 길게설정
        Debug.DrawRay(transform.position, test2, Color.blue);
        if(dHit.collider == null)
        {
            Debug.Log("dHit null");
        }
        if (d2Hit.collider == null)
        {
            Debug.Log("d2Hit null");
        }
        
        if (hit != null)
        {
            var indexG = Array.FindIndex(hit, x => x.transform.tag == "ground");//만약 람다를 안 쓰려면 for로 hit만큼 돌ㅡㅜ   아가면서 태그가 맞는지 확인해야함
            var indexP = Array.FindIndex(hit, x => x.transform.tag == "platform");

            isGrounded = indexP >= 0 | indexG >= 0 | dHit | d2Hit;
            findRayPlatform = indexP >= 0;
            cTemp = indexG >= 0;
            Debug.Log(string.Format("{0}", findRayPlatform));
            
            // var index = Array.FindIndex(hit, x => x.transform.tag == "ground");//만약 람다를 안 쓰려면 for로 hit만큼 돌ㅡㅜ   아가면서 태그가 맞는지 확인해야함
            // if(Array.FindIndex(hit, x => x.transform.tag == "platform") != -1)
            // {
            //     // Debug.Log("플랫폼 감지중");
            //     isGrounded = true;
            //     findRayPlatform = true;//여기는 없어도 무방
            // }
            // else
            // {
            //     // Debug.Log("플랫폼 가지 못 함");
            //     // if (index == -1)
            //     // {
            //     //     //아무것도 못 찾음
            //     //     isGrounded = false;
            //     // }
            //     findRayPlatform=false;
            // }
            // //var index = Array.FindIndex(hit, x => x.transform.tag == "ground");
            // if (index != -1)
            // {
            //     // Debug.Log("그라운드 찾음");
            //     isGrounded = true;
            //     cTemp = true;
            // }
            // else
            // {
            //     // Debug.Log("그라운드 못 찾음");
            //     isGrounded = false;
            //     cTemp = false;
            // }
            
        }

        //if (findRayPlatform)//temp가 true 일때 무시함
        //{
        //    Debug.Log("무시");
        //    //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), true);
        //}
        //else 
        if(!canDown&&(dHit.collider==null&&d2Hit.collider==null))//캐릭터 좌우 대각선 부분에서 플랫폼이 감지가 안될경우
        {
            Debug.Log("예외부분");
            Debug.Log(string.Format("{0}", canDown));
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
                findRayPlatform = true;
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), true);//원리는 그냥 설정한 시간동안 해당 플렛폼들을 그냥 무시하는식으로 설정했음 근데 지금 생각해보면 지금 플렛폼을 받아와서 플렛폼의 네임을 무시하는식으로 해도 되지않을까 하는 영감이 떠오름
                Debug.Log("무시중");
                currentOneWayPlatform.GetComponent<PlatformEffector2D>().useColliderMask = false;
                yield return new WaitForSeconds(downTime);//downtime변수는 나중에 중력 설정시 이질감이 든다면 변경필요
                Debug.Log("무시 끝");
                findRayPlatform = false;
                currentOneWayPlatform.GetComponent<PlatformEffector2D>().useColliderMask = true;
                //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), false);
                canDown = false;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 중력을 추가
    /// </summary>
    private void AddGravity() => AddVerticalForce(unitState == UnitState.HoldingWall ? gravity * Time.fixedDeltaTime * 0.2f : gravity * Time.fixedDeltaTime);

    /// <summary>
    /// 마찰력을 추가
    /// </summary>
    private void AddFrictional()
    {
        if(Mathf.Abs(hzForce) > accelerate) AddHorizontalForce(-hzForce * accelerate * Time.deltaTime); // 수평힘에 마찰력 추가
        else SetHorizontalForce(0);
    }

    /// <summary>
    /// 수직 방향 힘을 추가
    /// </summary>
    protected void AddVerticalForce(float force)
    {
        if(isGrounded && !canDown && vcForce < 0) vcForce = 0; // 바닥에 붙어있을 시 아래 방향의 힘 초기화
        vcForce += force;
    }
    /// <summary>
    /// 수평 방향 힘을 추가
    /// </summary>
    protected void AddHorizontalForce(float force)
    {
        hzForce += force;
        return true;
    }
    /// <summary>
    /// 수직 방향 힘을 설정
    /// </summary>
    public bool SetVerticalForce(float force)
    {
        vcForce = force;
        return true;
    }
    /// <summary>
    /// 수평 방향 힘을 설정
    /// </summary>
    public bool SetHorizontalForce(float force)
    {
        hzForce = force;
    }

    public void SetHorizontalVelocity(float vel) => hzVel = vel;

    /// <summary>
    /// 플레이어 유닛 힘을 리지드바디로 전달
    /// </summary>
    private void Movement() => rg.MovePosition(transform.position + (new Vector3(hzForce, vcForce) * Time.deltaTime));

    /// <summary>
    /// 현재 플레이어 유닛의 모든 힘을 초기화
    /// </summary>
    public void ResetForce()
    {
        SetHorizontalForce(0);
        SetVerticalForce(0);
    }

    /// <summary>
    /// 충돌면의 MapType을 반환
    /// </summary>
    /// <param name="collision">충돌체</param>
    /// <returns>충돌면의 MapType</returns>
    protected MapType CheckMapType(Collision2D collision)
    {
        float angle = 0;
        return CheckMapType(collision, ref angle);
    }
    /// <summary>
    /// 충돌면의 MapType을 반환
    /// </summary>
    /// <param name="collision">충돌체</param>
    /// <param name="ref angle">충돌각 반환 (0 ~ 180)</param>
    /// <returns>충돌면의 MapType</returns>
    protected MapType CheckMapType(Collision2D collision, ref float angle)
    {
        if(!(collision.gameObject.CompareTag("Map") || collision.gameObject.CompareTag("platform")) || collision.contactCount <= 0) return MapType.None;
        if(collision.gameObject.CompareTag("platform")) return MapType.Platform;
        angle = Mathf.Abs(Vector2.Angle(Vector2.up, collision.contacts[0].normal));
        if(angle <= 45) return MapType.Ground;
        else if(angle >= 135) return MapType.Ceiling;
        else return MapType.Wall;
    }
}

/* Hiding On */

                // if (isHide && unitState != UnitState.Air)//숨는 오브젝트와 상호 작용이 가능하다면
                // {
                //     unitState = UnitState.Hide;
                //     // if (check < 0)
                //     // {//일단은 캐릭터가 우츨을 보는것을 기본이라고 생각했는데 나중에 이미지 받아오면 해당 이미지 넣어서 다시 수정해야할수도있음 rotation관련만 
                //     //     this.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                //     // }
                //     // else
                //     // {
                //     //     this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                //     // }
                //     coverBox.SetActive(true);
                //     // coverBox.transform.position = this.gameObject.transform.GetChild(0).transform.position;//가드 위치를 가드 포인트 위치로 옮김
                //     transform.position = hidePos;
                //     rg.velocity = Vector2.zero;//만약 점프가 되려고하면 x만 0으로 초기화
                //     rg.position = transform.position;
                //     SetVel(0);
                //     // charactor.Crouch(guard);
                //     // isCrouching = true;
                //     // isMoving = false;
                // }
/* Hiding Off */

                // if(unitState == UnitState.Hide) unitState = UnitState.Default;
                // coverBox.SetActive(false);//s를 누르지 않았으므로 가드를 푼다
                // isCrouching = false;