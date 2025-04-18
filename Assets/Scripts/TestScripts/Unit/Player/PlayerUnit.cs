using UnityEngine;
using UnityEngine.U2D.Animation;

/// <summary>
/// 플레이어 유닛의 기본 동작을 정의, UnitBase를 상속한 추상 클래스
/// </summary>
public abstract class PlayerUnit : UnitBase
{
    public GameObject coverBox;

    protected bool isJumping;

    public class Velocity
    {
        public Vector2 value;

        public Velocity() {}
        public Velocity(Vector2 vector) => value = vector;

        public static implicit operator Velocity(Vector2 vector) => new Velocity(vector);
        public static implicit operator Vector2(Velocity vel) => vel.value;
    }
    private float _hzVel;
    private float _vcVel;
    protected float hzVel
    {
        get => _hzVel;
        set
        {
            _hzVel = value;
            velocity.value.x = _hzVel;
        }
    }
    protected float vcVel
    {
        get => _vcVel;
        set
        {
            _vcVel = value;
            velocity.value.y = _vcVel;
        }
    }
    public Velocity velocity = new();

    public MapSensor mapSensor;


    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        switch (CheckMapType(collision))
        {
            case MapType.Ground:
            case MapType.Platform:
                if(isJumping) 
                {
                    SetVerticalForce(0);
                    SetVerticalVelocity(0);
                    isJumping = false;
                }
                break;
            case MapType.Floor:
                isJumping = false;
                if(vcForce > 0) SetVerticalVelocity(0);
                AddVerticalVelocity(gravity * Time.fixedDeltaTime);
                break;
            case MapType.Wall:
                SetHorizontalForce(0);
                SetHorizontalVelocity(0);
                break;
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        switch (CheckMapType(collision))
        {
            case MapType.Floor:
                isJumping = false;
                if(vcForce > 0) SetVerticalVelocity(0);
                AddVerticalVelocity(gravity * Time.fixedDeltaTime);
                break;
            case MapType.Wall:
                SetHorizontalForce(0);
                SetHorizontalVelocity(0);
                break;
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {

    }

    protected override void Update()
    {
        if(!isGrounded && unitState == UnitState.Default) unitState = UnitState.Air; // 기본 상태에서 공중에 뜰 시 공중 상태로 변경
        else if(isGrounded && unitState == UnitState.Air) unitState = UnitState.Default; // 공중 상태에서 바닥에 닿을 시 기본 상태로 변경
        base.Update();
    }

    protected virtual void FixedUpdate()
    {
        AddGravity();
        AddFrictional();
        SetHorizontalForce(hzVel);
        SetVerticalForce(vcVel);
        if(Mathf.Abs(hzForce) <= 0.01f * movementSpeed) hzForce = 0;
        if(vcForce < 0 && isGrounded) vcForce = 0;
        Movement();
        isGrounded = mapSensor.isGrounded;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        mapSensor.Set(rg, GetComponent<CapsuleCollider2D>());
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        ResetForce();
        SetHorizontalVelocity(0);
        SetVerticalVelocity(0);
    }

    private float jumpingHight;
    public override bool Jump(KeyState jumpKey)
    {
        if(ControllerChecker() || unitState == UnitState.FormChange || unitState == UnitState.Dash)
        {
            isJumping = false;
            return false;
        }
        switch(jumpKey)
        {
            case KeyState.KeyDown:
                if(isJumping || !isGrounded) return false;
                isJumping = true;
                jumpingHight = 0;
                SetVerticalForce(0);
                AddVerticalForce(jumpImpulse);
                SetVerticalVelocity(0);
                AddVerticalVelocity(jumpImpulse);
                return true;
            case KeyState.KeyStay:
                jumpingHight += Time.deltaTime / jumpTime;
                if(!isJumping || jumpingHight >= 1)
                {
                    isJumping = false;
                    return false;
                }
                AddVerticalVelocity(Mathf.Cos(jumpingHight) * jumpForce * Time.deltaTime);
                return true;
            case KeyState.KeyUp:
                isJumping = false;
                break;
        }
        return false;
    }

    public override bool Move(Vector2 dir)
    {
        if(ControllerChecker()) return false;
        AddHorizontalVelocity(dir.x * movementSpeed * accelerate * Time.deltaTime);  // 가속도만큼 입력 방향에 힘을 추가
        if(dir.x == 0 && Mathf.Abs(hzVel) < 0.01f) hzVel = 0;
        if(unitState == UnitState.FormChange || dir.x == 0) // 제어가 불가능한 상태일 경우 동작을 수행하지 않음
        {
            base.Move(Vector2.zero);
            return false;
        }
        else base.Move(dir);
        return true;
    }

    // 수정 필요함
    public override bool Crouch(KeyState crouchKey)
    {
        if (ControllerChecker() || unitState == UnitState.FormChange) return false;
        switch (crouchKey)
        {
            case KeyState.KeyDown:
            case KeyState.KeyStay:
                mapSensor.currentPlatform?.RemoveColliderMask(1<<gameObject.layer);
                mapSensor.currentPlatform = null;
                mapSensor.platformSensor.normal = Vector2.up;
                return true;
            case KeyState.KeyUp:
                return true;
        }
        return false;
    }

    /// <summary>
    /// 중력을 추가
    /// </summary>
    private void AddGravity()
    {
        if(isGrounded && vcVel < 0) SetVerticalVelocity(0);
        else AddVerticalVelocity(gravity * 1 * Time.fixedDeltaTime);
    }
    /// <summary>
    /// 마찰력을 추가
    /// </summary>
    private void AddFrictional() // 수평힘에 마찰력 추가
    {
        if(Mathf.Abs(hzVel) > 1) AddHorizontalVelocity(-hzVel * accelerate * Time.fixedDeltaTime);
        else if(Mathf.Abs(hzVel) > 0.1f) AddHorizontalVelocity(-Mathf.Sign(hzVel) * accelerate * Time.fixedDeltaTime);
        else SetHorizontalVelocity(0);
    }

    /// <summary>
    /// 수직 방향 힘을 추가
    /// </summary>
    protected void AddVerticalForce(float force)
    {
        vcForce += force;
    }
    /// <summary>
    /// 수평 방향 힘을 추가
    /// </summary>
    protected void AddHorizontalForce(float force) => hzForce += force;
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
        return true;
    }

    public void SetVerticalVelocity(float vel) => vcVel = vel;
    public void AddVerticalVelocity(float vel) => vcVel += vel;
    public void SetHorizontalVelocity(float vel) => hzVel = vel;
    public void AddHorizontalVelocity(float vel) => hzVel += vel;

    /// <summary>
    /// 플레이어 유닛 힘을 리지드바디로 전달
    /// </summary>
    private void Movement()
    {
        Debug.Assert(mapSensor.normal != Vector2.right, "Vector2.right(1, 0) 아님\nVector2.up으로 교체해주세요.");

        if(isGrounded && Mathf.Abs(hzForce) > 0)
        {
            int layerMask = 0;
            switch(mapSensor.groundType)
            {
                case MapType.Ground : 
                    layerMask = 1 << LayerMask.NameToLayer("Map");
                    break;
                case MapType.Platform :
                    layerMask = 1 << LayerMask.NameToLayer("OneWayPlatform");
                    break;
            }
            Vector2 dir = Vector2.Perpendicular(-mapSensor.normal) * hzForce;
            float mul = Mathf.Clamp(1 / mapSensor.normal.y, 1, 1.41f);
            dir *= mul;

            rg.position = rg.position + (dir * Time.fixedDeltaTime);

            var hit = Physics2D.CircleCast(rg.position + (Vector2.up * BoxSizeX), BoxSizeX, Vector2.down, 1f, layerMask);
            if(hit && !isJumping) rg.MovePosition(rg.position + (Vector2.down * hit.distance));
            else rg.MovePosition(rg.position + (Vector2.up * (vcForce * Time.fixedDeltaTime)));
        }
        else 
        {
            Vector2 dir = new Vector2(hzForce, vcForce);

            rg.MovePosition(rg.position + (dir * Time.fixedDeltaTime));
        }


        // Debug.DrawRay(transform.position, Vector2.Perpendicular(-mapSensor.normal), Color.blue);
        // Debug.DrawRay(transform.position, dir.normalized, Color.magenta);
        // Debug.DrawRay(transform.position, dir.normalized, Color.yellow);

    }

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
        angle = Mathf.Abs(Vector2.Angle(Vector2.up, collision.GetContact(collision.contactCount-1).normal));
        if(collision.gameObject.CompareTag("platform") && angle <= 50) return MapType.Platform;
        else if(collision.gameObject.CompareTag("platform") && angle > 50) return MapType.None;
        if(angle <= 50) return MapType.Ground;
        else if(angle >= 130) return MapType.Floor;
        else return MapType.Wall;
    }
}

/* isGround - Old */

    // private GameObject currentOneWayPlatform;
    // private float downTime;
    // private bool canDown;
    // private bool cTemp;
    // private bool isHide;
    // private bool findRayPlatform;
    // private float check;
    // private float checkHigh;
    // private float distanceToCheck;
    // private LayerMask lm;
    // private Coroutine coroutine;

    // private Vector3 hidePos;

    /* OnCollisionEnter */
        // switch(CheckMapType(collision))
        // {
            // case MapType.Platform:
            //     // Debug.Log("플랫폼 들어옴");
            //     // if(!currentOneWayPlatform) currentOneWayPlatform = collision.gameObject;//플랫폼이라면 현재 플렛폼을 담음
            //     // else collision.gameObject.GetComponent<PlatformScript>()?.RemoveColliderMask(1<<gameObject.layer);
            //     // Debug.Log(collision.gameObject.GetComponent<PlatformScript>().dObject);//다운 오브젝트 타입확인용 로그
            //     // switch (collision.gameObject.GetComponent<PlatformScript>().dObject)//대각선 직선 오브젝트 마다 떨어지는 시간이 다를수도 있으니  
            //     // {
            //     //     case PlatformScript.downJumpObject.STRAIGHT://직선
            //     //         downTime = 1f;//떨어지는 시간 다르게 하기 위함
            //     //         break;
            //     //     case PlatformScript.downJumpObject.DIAGONAL://대각선
            //     //         downTime = 0.8f;//떨어지는 시간 다르게 하기 위함 , 0.7초까지도 1칸에 대해서는 가능하지만 만약에 쭉 앞으로 가면서 떨어진다고 하면 안전한 시간은 0.75~0.8사이임
            //     //         break;
            //     // }
            //     //canDown = true;
            //     break;
        // }
    // protected virtual void OnCollisionExit2D(Collision2D collision)
    // {
        // switch(CheckMapType(collision))
        // {
        //     case MapType.Platform:
        //         // Debug.Log("플랫폼 벗어남");
        //         // if (isJumping)
        //         // {
        //         //     Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), false);
        //         //     // Debug.Log("점프");
        //         // }
        //         currentOneWayPlatform = null;//platform에서 벗어난거라면 플랫폼 변수를 비움
        //         break;
        // }
        // if(collision.gameObject.CompareTag("platform")) currentOneWayPlatform = null;
    // }
    // protected virtual void OnCollisionStay2D(Collision2D collision)
    // {
    //     switch(CheckMapType(collision))
    //     {
            // case MapType.Platform:
            //     if(!currentOneWayPlatform) currentOneWayPlatform = collision.gameObject;//플랫폼이라면 현재 플렛폼을 담음
            //     break;
        // }
        // if(CheckMapType(collision) == MapType.Floor) isJumping = false;
    // }
    // protected override void Start()
    // {
    //     // lm = ~(1 << gameObject.layer);
    //     // distanceToCheck = boxSizeY * 1.05f;
    // }
    // protected override void Update()
    // {
    //     CrouchUpdate();
    // }
    // protected override void OnEnable()
    // {
    //     // coroutine = StartCoroutine(DownJump());
    // }
    // protected override void OnEnable()
    // {
    //     // coroutine = StartCoroutine(DownJump());
    // }
    // protected override void OnDisable()
    // {
    //     // StopCoroutine(coroutine);
    //     // coroutine = null;
    // }
    // public override bool Crouch(KeyState crouchKey)
    // {
    //     // Debug.Log("findRayPlatform" + findRayPlatform);
    //     if (ControllerChecker() || unitState == UnitState.FormChange) return false;
    //     // if (currentOneWayPlatform == null) Debug.Log("크라우치에서 플랫폼 없음");
    //     // else Debug.Log("크라우치에서 플랫폼 있음");
    //     switch (crouchKey)
    //     {
    //         case KeyState.KeyDown:
    //         case KeyState.KeyStay:
    //             // canDown = true; 
    //             // if (currentOneWayPlatform != null)//밑 아래 점프 가능한 오브젝트와 닿아있을때 ,우선순위 따라서 위로 올리고 return이 필요할듯 
    //             // {
    //             //     // Debug.Log("hello");
    //             //     // canDown = !isHide;
    //             //     canDown = true;
    //             // }
    //             // else
    //             // {
    //             //     // Debug.Log("여기에 걸림");
    //             // }
    //             return true;
    //         case KeyState.KeyUp:
    //             return true;
    //     }
    //     return false;
    // }
        // public void GetCurrenttPlatform(RaycastHit2D ray)
    // {
    //     // Debug.Log("플랫폼 들어옴");
    //     currentOneWayPlatform = ray.transform.gameObject;//플랫폼이라면 현재 플렛폼을 담음
    //     // Debug.Log(collision.gameObject.GetComponent<PlatformScript>().dObject);//다운 오브젝��� 타입확인용 로그
    //     switch (ray.transform.gameObject.GetComponent<PlatformScript>().dObject)//대각선 직선 오브젝트 마다 떨어지는 시간이 다를수도 있으니  
    //     {
    //         case PlatformScript.downJumpObject.STRAIGHT://직선
    //             downTime = 1f;//떨어지는 시간 다르게 하기 위함
    //             break;
    //         case PlatformScript.downJumpObject.DIAGONAL://대각선
    //             downTime = 0.8f;//떨어지는 시간 다르게 하기 위함 , 0.7초까지도 1칸에 대해서는 가능하지만 만약에 쭉 앞으로 가면서 떨어진다고 하면 안전한 시간은 0.75~0.8사이임
    //             break;
    //     }
    //     // switch (CheckMapType(ray))
    //     // {
    //     //     case MapType.Platform:
    //     //         //canDown = true;
    //     //         break;
    //     //     case MapType.Floor:
    //     //         isJumping = false;
    //     //         SetVerticalForce(gravity * Time.fixedDeltaTime);
    //     //         break;
    //     //         //default:
    //     //         //    
    //     //         //    break;
    //     // }
    // }

    // private void CrouchUpdate()
    // {
    //     // var charBoxCollider = GetComponent<BoxCollider2D>();
    //     // Debug.Log("candown" + canDown);
    //     float player_dialog = Mathf.Sqrt(MathF.Pow(boxSizeX, 2) + MathF.Pow(boxSizeY, 2));
    //     RaycastHit2D[] hit =Physics2D.RaycastAll(transform.parent.position + Vector3.up * boxOffsetY, Vector2.down, distanceToCheck, lm);
    //     Debug.DrawRay(transform.position + Vector3.up * boxOffsetY, Vector2.down * distanceToCheck, Color.red);
    //     //BoxCollider2D box = GetComponent<BoxCollider2D>();
    //     Vector2 test = new Vector2(transform.position.x + boxSizeX + boxOffsetX, transform.position.y - boxSizeY + boxOffsetY - 0.05f)  - (Vector2)transform.position;//우측대각선
    //     //RaycastHit2D dHit = Physics2D.Raycast(transform.position, test,(MathF.Sqrt(charBoxCollider.size.x / 2) + MathF.Sqrt(charBoxCollider.size.y/2)) * 0.65f, 1<<LayerMask.NameToLayer("OneWayPlatform"));//플랫폼감지용 레이,하드 코딩때 값 0.75f,0.5에서 0.45로 수정함으로서 collider보다 더 길게설정 
    //     RaycastHit2D dHit = Physics2D.Raycast(transform.position + Vector3.up * boxOffsetY, test, player_dialog*1.05f, 1 << LayerMask.NameToLayer("OneWayPlatform") | 1 << LayerMask.NameToLayer("Ground"));//플랫폼감지용 레이,하드 코딩때 값 0.75f,0.5에서 0.45로 수정함으로서 collider보다 더 길게설정 
    //     RaycastHit2D []dHitarr = Physics2D.RaycastAll(transform.position + Vector3.up * boxOffsetY, test, player_dialog * 1.05f, 1 << LayerMask.NameToLayer("OneWayPlatform") | 1 << LayerMask.NameToLayer("Ground"));//플랫폼감지용 레이,하드 코딩때 값 0.75f,0.5에서 0.45로 수정함으로서 collider보다 ��� 길게설정 


    //     Debug.DrawRay(transform.position,test, Color.green);
    //     Vector2 test2 = new Vector2(transform.position.x - boxSizeX + boxOffsetX , transform.position.y - boxSizeY + boxOffsetY - 0.05f) - (Vector2)transform.position;//좌측 대각선
    //     //RaycastHit2D d2Hit = Physics2D.Raycast(transform.position, test2, (MathF.Sqrt(charBoxCollider.size.x / 2) + MathF.Sqrt(charBoxCollider.size.y / 2))*0.65f, 1 << LayerMask.NameToLayer("OneWayPlatform"));//플랫폼감지용 레이,0.5에서 0.45로 수정함으로서 collider보다 더 길게설정
    //     RaycastHit2D d2Hit = Physics2D.Raycast(transform.position + Vector3.up * boxOffsetY, test2, player_dialog*1.05f , 1 << LayerMask.NameToLayer("OneWayPlatform") | 1 << LayerMask.NameToLayer("Ground"));//플랫폼감지용 레이,0.5에서 0.45로 수정함으로서 collider보다 더 길게설정
    //     RaycastHit2D []d2Hitarr = Physics2D.RaycastAll(transform.position + Vector3.up * boxOffsetY, test2, player_dialog * 1.05f, 1 << LayerMask.NameToLayer("OneWayPlatform") | 1 << LayerMask.NameToLayer("Ground"));//플랫폼감지용 레이,0.5에서 0.45로 수정함으로서 collider보다 더 길게설정

    //     Debug.DrawRay(transform.position, test2, Color.blue);
    //     if(dHit.collider == null)
    //     {
    //         // Debug.Log("dHit null");
    //     }
    //     else
    //     {
    //         var indexP = Array.FindIndex(dHitarr, x => x.transform.tag == "platform" && x.distance > boxOffsetY / 2);
            
        
    //         if (indexP != -1)
    //         {
                
    //             GetCurrenttPlatform(dHitarr[indexP]);
    //             // Debug.Log("플랫폼 찾음"+canDown);
    //         }
    //         else
    //         {
    //             // Debug.Log("플랫폼 못찾음");
    //         }
    //     }
    //     if (d2Hit.collider == null)
    //     {
    //         // Debug.Log("d2Hit null");
    //     }
    //      {
    //         var indexP = Array.FindIndex(d2Hitarr, x => x.transform.tag == "platform" && x.distance > boxOffsetY / 2);
    //         // Debug.Log(indexP);
    //         if (indexP != -1)
    //         {
                
    //             GetCurrenttPlatform(d2Hitarr[indexP]);
    //             // Debug.Log("플랫폼 찾음" + canDown);
    //         }
    //         else
    //         {
    //             // Debug.Log("���랫폼 못찾음");
    //         }
    //      }
    //     // Debug.Log("hit=>"+hit.Length);

    //     bool sideRay = (dHit) | (d2Hit);
        
    //     if (hit != null)
    //     {
    //         var indexG = Array.FindIndex(hit, x => x.transform.tag == "ground"&&x.distance>boxOffsetY/2);//만약 람다를 안 쓰려면 for로 hit만큼 돌ㅡㅜ   아가면서 태그가 맞는지 확인해야함
            
    //         // var indexP = Array.FindIndex(hit, x => x.transform.tag == "platform" && x.distance > charBoxCollider.size.y / 2);
    //         var indexP = Array.FindIndex(hit, x => x.transform.tag == "platform" && x.distance > boxOffsetY / 2);
    //         var indexH = Array.FindIndex(hit, x => x.transform.tag == "Hatch" && x.distance > boxOffsetY / 2);

    //         //if (indexP != -1)
    //         //{
    //         //    GetCurrenttPlatform(hit[indexP]);
    //         //}

    //         if (indexP != -1)
    //         {
    //             Debug.Log("플랫폼 밟는중");
    //             GetCurrenttPlatform(hit[indexP]);
    //         }
    //         if (indexH != -1)
    //         {
    //             Debug.Log("해치 밟는중");
    //         }
            

    //         // Debug.Log(string.Format("indexG {0} indexP{1}", indexG, indexP));
    //         // Debug.Log(string.Format("indexG=>{0}  indexP=>{1}", indexG,indexP));


    //         //isGrounded = indexP >= 0 | indexG >= 0 | dHit | d2Hit;    
    //         isGrounded = indexP >= 0 | indexG >= 0 | (dHit.distance>player_dialog|dHitarr.Length>1) | (d2Hit.distance>player_dialog|d2Hitarr.Length>1)| sideRay;

    //         // Debug.Log(string.Format("dhit length=>{0} d2hit length=>{1}", dHitarr.Length, d2Hitarr.Length));
    //         //isGrounded = indexP >= 0 | indexG >= 0 ;
    //         // Debug.Log(string.Format("isGrounded => {0} indexP=>{1} indexG=>{2} dhit=>{3} d2hit=>{4}",isGrounded,indexP,indexG,(bool)dHit,(bool)d2Hit));
    //         findRayPlatform = indexP >= 0|sideRay;
    //         cTemp = indexG >= 0;
    //         // Debug.Log(string.Format("{0}", findRayPlatform));
            
        
            
    //     }
    //     // Debug.Log(string.Format("Attack test isGrounded ={0} dhit = {1} canDown={2}", isGrounded,dHit|d2Hit,canDown));
        

    //     //if (findRayPlatform)//temp가 true 일때 무시함
    //     //{
    //     //    Debug.Log("무시");
    //     //    //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), true);
    //     //}
    //     //else 
    //     if(dHit.collider==null&&d2Hit.collider==null)//캐릭터 좌우 대각선 부분에서 플랫폼이 감지가 안될경우
    //     {
    //         // Debug.Log("예외부분");
    //         // Debug.Log(string.Format("{0}", canDown));
    //         Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), false);
    //     }
    //     //else if(!canDown&&(dHit.collider == true && d2Hit.collider == true))
    //     //{
    //     //    Debug.Log("test 부분");
    //     //    Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), true);
    //     //}
    // }
    // IEnumerator DownJump()
    // {
    //     while (true)
    //     {
    //         if (canDown)//아래 점프 가능한 오브젝트 만날경우
    //         {
    //             // Debug.Log("hi");
    //             findRayPlatform = true;
    //             Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), true);//원리는 그냥 설정한 시간동안 해당 플렛폼들을 그냥 무시하는식으로 설정했음 근데 지금 생각해보면 지금 플렛폼을 받아와서 플렛폼의 네임을 무시하는식으로 해도 되지않을까 하는 영감이 떠오름
    //             AddVerticalForce(gravity * Time.deltaTime);
    //             // Debug.Log("무시중");
    //             if(currentOneWayPlatform != null)currentOneWayPlatform.GetComponent<PlatformEffector2D>().useColliderMask = false;
    //             float t = 0;
    //             var tempP = currentOneWayPlatform;
    //             yield return new WaitUntil(() => {
    //                 t += Time.deltaTime;
    //                 if(currentOneWayPlatform == null || t >= downTime) return true;
    //                 else return false;
    //             });
    //             // yield return new WaitForSeconds(downTime);//downtime변수는 나중에 중력 설정시 이질감이 든다면 변경필요
    //             // Debug.Log("무시 끝");
    //             findRayPlatform = false;
    //             // yield return new WaitForSeconds(downTime);//downtime변수는 나중에 중력 설정시 이질감이 든다면 변경필요
    //             // Debug.Log("무시 끝");
    //             findRayPlatform = false;
    //             if(currentOneWayPlatform != null)currentOneWayPlatform.GetComponent<PlatformEffector2D>().useColliderMask = true;
    //             currentOneWayPlatform = null;
    //             //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("OneWayPlatform"), false);
    //             canDown = false;
    //         }
    //         yield return null;
    //     }
    // }

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