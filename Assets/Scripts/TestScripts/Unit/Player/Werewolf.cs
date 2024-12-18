using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// 늑대인간 상태 클래스, PlayerUnit 클래스를 상속함
/// </summary>
public class Werewolf : PlayerUnit
{
    public GameObject MeleeAttack;

    AudioSource sound;
    public AudioClip soundClip;

    /// <summary>
    /// 공격 지속시간
    /// </summary>
    public float attackDuration;

    /// <summary>
    /// 공격 추진력
    /// </summary>
    public float attackImpulse;

    /// <summary>
    /// 고정된 방향을 저장하기 위한 변수
    /// </summary>
    private Vector2 fixedDir = Vector2.zero;

    /// <summary>
    /// 공격 코루틴을 저장하는 변수, 공격 중 여부 겸용
    /// </summary>
    private Coroutine attackCoroutine;
    /// <summary>
    /// 대쉬 코루틴을 저장하는 변수, 대쉬 중 여부 겸용
    /// </summary>
    private Coroutine dashCoroutine;
    public int air_attack_count = 1;
    public bool isFormChangeReady { get => changeGauge <= 0; }

    /// <summary>
    /// 늑대인간 폼 유지를 위한 게이지 변수
    /// </summary>
    [SerializeField] private float _changeGauge;
    public float changeGauge { 
        get => _changeGauge;
        set 
        {
            _changeGauge = value; 
            brutalGauge?.Invoke(_changeGauge, brutalData.maxGage);
        }
    }
    public BrutalData brutalData;
    public GaugeBar<Werewolf>.GaugeUpdateDel brutalGauge;

    protected override void OnEnable()
    {
        base.OnEnable();
        var pi = CameraManager.Instance.proCamera2DPointerInfluence;
        pi.MaxHorizontalInfluence = 5.15f;
        pi.MaxVerticalInfluence = 0.3f;
        pi.InfluenceSmoothness = 0.2f;
        CameraManager.Instance.ChangeSize = 5.45f;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        StopDash();
        StopAttack();
        // StopHoldingWall();
    }

    protected override void Start() => Init();
    public override void Init()
    {
        base.Init();
        MeleeAttack.GetComponent<MaleeAttack>().Set(1, gameObject);
        brutalData = GameManager.GetBrutalData();
        changeGauge = brutalData.maxGage;
        
        if(!anim.runtimeAnimatorController) anim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("PlayerAnim/WereWolf/WereWolf Lib Ani");
    }

    protected override void Update()
    {
        base.Update();
        if(isGrounded)
        {
            air_attack_count = 1;
            // isholded = false;
        }
        changeGauge -= brutalData.sec * Time.deltaTime;
    }
    
    public override bool Attack(Vector3 clickPos)
    {
        if((unitState != UnitState.Default && unitState != UnitState.Air && unitState != UnitState.HoldingWall && unitState != UnitState.Dash) || unitState == UnitState.FormChange ||
            isFormChangeReady || attackCoroutine != null || anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) return false; // 제어가 불가능한 상태일 경우 동작을 수행하지 않음
        if(dashCoroutine != null) StopDash();
        if (unitState == UnitState.Air) air_attack_count--;
        isJumping = false;
        Vector2 subvec = clickPos - transform.position;
        float deg = Mathf.Atan2(subvec.y, subvec.x) ;//mathf.de
        MeleeAttack.transform.localPosition = new Vector3(Mathf.Cos(deg), Mathf.Sin(deg)*2,transform.localPosition.z);
        MeleeAttack.transform.localEulerAngles = new Vector3(0, 0, Quaternion.FromToRotation(Vector2.up, transform.position - MeleeAttack.transform.position).eulerAngles.z - 90);

        attackCoroutine = StartCoroutine(Attacking());
        changeGauge -= brutalData.atk;
        base.Attack(clickPos);
        return true;
    }

    /// <summary>
    /// 공격 중의 동작을 수행
    /// </summary>
    private IEnumerator Attacking()
    {
        unitState = UnitState.Attack;
        // 지속시간만큼 히트박스를 온오프
        MeleeAttack.SetActive(true);
        var tempDir = MeleeAttack.transform.localPosition.normalized;
        if(air_attack_count >= 0)
        {
            SetVerticalForce(tempDir.y * attackImpulse * 0.5f);
            SetHorizontalVelocity(tempDir.x * attackImpulse);
        }
        float t = 0;
        while(t < attackDuration)
        {
            t += Time.deltaTime;
            if(!isFormChangeReady) base.Crouch(KeyState.KeyStay);
            else base.Crouch(KeyState.KeyUp);
            yield return null;
        }
            SetVerticalForce(0);
        base.Crouch(KeyState.KeyUp);
        StopAttack();
    }

    private void StopAttack()
    {
        if(isGrounded) unitState = UnitState.Default;
        else unitState = UnitState.Air;
        if(attackCoroutine != null) StopCoroutine(attackCoroutine);
        MeleeAttack.SetActive(false);
        attackCoroutine = null;
    }

    public override bool Move(Vector2 dir)
    {
        if(ControllerChecker() || unitState == UnitState.HoldingWall || 
            unitState == UnitState.Dash || unitState == UnitState.Attack) return false; // 제어가 불가능한 상태일 경우 동작을 수행하지 않음
        fixedDir = dir.normalized; // 대쉬 방향을 저장
        return base.Move(dir);
    }
    public override bool Jump(KeyState jumpKey) => base.Jump(jumpKey);

    public override bool Crouch(KeyState crouchKey)
    {
        if(ControllerChecker() || isFormChangeReady || unitState == UnitState.Dash || unitState == UnitState.HoldingWall || unitState == UnitState.Attack) return false;
        return base.Crouch(crouchKey);
    }

    // 수정 필요함
    public override bool Dash()
    {
        if(ControllerChecker() || isFormChangeReady || unitState == UnitState.HoldingWall || unitState == UnitState.FormChange || dashCoroutine != null) return false; // 제어가 불가능한 상태일 경우 동작을 수행하지 않음
        if(attackCoroutine != null) StopAttack();
        base.Dash();
        dashCoroutine = StartCoroutine(DashAffterInput());
        return true;
    }

    /// <summary>
    /// 대쉬 중의 동작을 수행
    /// </summary>
    private IEnumerator DashAffterInput()
    {
        // if(unitState == UnitState.HoldingWall) StopHoldingWall();
        unitState = UnitState.Dash; // 대쉬 상태로 변경
        var tempVel = fixedDir.x == 0 ? spriteRenderer.flipX ? -1 : 1 : Mathf.Sign(fixedDir.x);
        SetVerticalForce(0);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Dash"));
        while(anim.GetCurrentAnimatorStateInfo(0).IsName("Dash")) // 대쉬 지속 시간 동안
        {
            SetHorizontalVelocity(tempVel * movementSpeed * 1.2f);
            anim.SetFloat("hzForce", -0.5f);
            yield return null;
        }
        StopDash();
    }

    /// <summary>
    /// 대쉬 동작 중지
    /// </summary>
    public void StopDash()
    {
        if(dashCoroutine != null) StopCoroutine(dashCoroutine);
        dashCoroutine = null;
        // invalidation=false;
        if(isGrounded) unitState = UnitState.Default;
        else unitState = UnitState.Air;
    }

    public override bool FormChange()
    {
        if(unitState != UnitState.Default) return false;
        else return base.FormChange();
    }

    public override bool Reload() => false;

    public override void StopAllC()
    {
        StopAttack();
        StopDash();
        // StopHoldingWall();
        unitState = UnitState.Death;
    }
}


    // Coroutine holdingCoroutine;
    // bool isholded;
    // private void HoldingWall(Collision2D collision)
    // {
    //     fixedDir = Vector2.right * -CheckDir(collision.contacts[0].point); // 벽의 반대 방향을 저장
    //     if(isholded || holdingCoroutine != null || !Physics2D.Raycast(transform.position + Vector3.down * boxSizeY * 0.9f, Vector2.right*-fixedDir, boxSizeX*1.05f, 1<<LayerMask.NameToLayer("Map"))) return;
    //     if(holdingCoroutine != null) StopHoldingWall();
    //     isJumping = false;
    //     isholded = true;
    //     anim.SetBool("isHoldingWall", true);
    //     unitState = UnitState.HoldingWall; // 벽붙기 상태로 변경
    //     if(fixedDir.x < 0) spriteRenderer.flipX = true;
    //     else spriteRenderer.flipX = false;
    //     ResetForce();
    //     SetHorizontalVelocity(0);
    //     holdingCoroutine = StartCoroutine(Holding(-fixedDir));
    // }
    // private void StopHoldingWall()
    // {
    //     anim.SetBool("isHoldingWall", false);
    //     if(isGrounded) unitState = UnitState.Default;
    //     else unitState = UnitState.Air;
    //     if(holdingCoroutine != null) 
    //     {
    //         StopCoroutine(holdingCoroutine);
    //         holdingCoroutine = null;
    //     }
    //     isholded = false;
    // }

    // private IEnumerator Holding(Vector2 dir)
    // {
    //     yield return new WaitUntil(() => isFormChangeReady ||
    //         !Physics2D.Raycast(transform.position + Vector3.down * boxSizeY * 0.9f, Vector2.right*dir, boxSizeX*1.05f, 1<<LayerMask.NameToLayer("Map")) ||
    //         Physics2D.Raycast(transform.position, Vector2.down, boxSizeY*1.2f, 1<<LayerMask.NameToLayer("Map"))
    //     );
    //     StopHoldingWall();
    // }

    //     public override bool Jump(KeyState jumpKey)
    // {
    //     return base.Jump(jumpKey);
    //     // switch(jumpKey)
    //     // {
    //     //     case KeyState.KeyDown:
    //     //     if(unitState == UnitState.HoldingWall)
    //     //     {
    //     //         StopHoldingWall();
    //     //         isJumping = false;
    //     //         SetVerticalForce(jumpImpulse); // 윗 방향 힘 추가
    //     //         SetHorizontalVelocity(fixedDir.x * 20);
    //     //         SetHorizontalForce(fixedDir.x * 20);
    //     //         return true;
    //     //     }
    //     //     else return base.Jump(jumpKey);
    //     //     case KeyState.KeyStay:
    //     //     if(unitState == UnitState.HoldingWall) return false;
    //     //     return base.Jump(jumpKey);
    //     //     case KeyState.KeyUp:
    //     //     return base.Jump(jumpKey);
    //     // }
    //     // return false;
    // }

        // protected override void OnCollisionEnter2D(Collision2D collision)
    // {
    //     // base.OnCollisionEnter2D(collision);
    //     switch(CheckMapType(collision))
    //     {
    //         case MapType.Wall:
    //             if(unitState == UnitState.Air && !isFormChangeReady) HoldingWall(collision);
    //             break;
    //         case MapType.Ground:
    //             if(unitState == UnitState.HoldingWall) StopHoldingWall();
    //         break;
    //     }
    // }
    // protected override void OnCollisionStay2D(Collision2D collision)
    // {
    //     // base.OnCollisionStay2D(collision);
    //     switch(CheckMapType(collision))
    //     {
    //         case MapType.Wall:
    //             if(unitState == UnitState.Air && !isFormChangeReady) HoldingWall(collision);
    //             break;
    //         case MapType.Ground:
    //             if(unitState == UnitState.HoldingWall) StopHoldingWall();
    //         break;
    //     }
    // }

    // protected override void OnCollisionExit2D(Collision2D collision)
    // {
    //     // base.OnCollisionExit2D(collision);
    //     switch(CheckMapType(collision))
    //     {
    //         case MapType.Wall:
    //             if(unitState == UnitState.HoldingWall) StopHoldingWall();
    //             break;
    //     }
    // }

    //     public override bool Attack(Vector3 clickPos)
    // {
    //     if((unitState != UnitState.Default && unitState != UnitState.Air && unitState != UnitState.HoldingWall && unitState != UnitState.Dash) || unitState == UnitState.FormChange ||
    //         isFormChangeReady || attackCoroutine != null || anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) return false; // 제어가 불가능한 상태일 경우 동작을 수행하지 않음
    //     if(dashCoroutine != null) StopDash();
    //     // if(unitState == UnitState.HoldingWall) StopHoldingWall();
    //     //Vector2 testvec = new Vector2(1 * CheckDir(clickPos), clickPos.y - transform.position.y);//이렇게 되면 대각선으로 갈 수록 좁아짐
    //     //Vector2 testvec = (Vector2.up * (clickPos.y - transform.position.y)).normalized;
    //     //MeleeAttack.transform.localPosition = testvec;
    //     //MeleeAttack.transform.localPosition = (Vector2.right * CheckDir(clickPos))+testvec; // 클릭 방향으로 공격 위치 설정
    //     if (unitState == UnitState.Air) air_attack_count--;
    //     isJumping = false;
    //     Vector2 subvec = clickPos - transform.position;
    //     float deg = Mathf.Atan2(subvec.y, subvec.x) ;//mathf.de
    //     //deg*=Mathf.Deg2Rad;//라디안으로 바꿔주기는 하는데 이렇게 하면 좀 문제생김
    //     //Debug.Log(deg);
    //     MeleeAttack.transform.localPosition = new Vector3(Mathf.Cos(deg), Mathf.Sin(deg)*2,transform.localPosition.z);
    //     MeleeAttack.transform.localEulerAngles = new Vector3(0, 0, Quaternion.FromToRotation(Vector2.up, transform.position - MeleeAttack.transform.position).eulerAngles.z - 90);

    //     attackCoroutine = StartCoroutine(Attacking());
    //     changeGauge -= brutalData.atk;
    //     base.Attack(clickPos);
    // }