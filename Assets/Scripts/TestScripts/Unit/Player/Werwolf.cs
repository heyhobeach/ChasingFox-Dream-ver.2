using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 늑대인간 상태 클래스, PlayerUnit 클래스를 상속함
/// </summary>
public class Werwolf : PlayerUnit
{
    public GameObject MeleeAttack;

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
    private int fixedDir = 0;

    /// <summary>
    /// 공격 코루틴을 저장하는 변수, 공격 중 여부 겸용
    /// </summary>
    private Coroutine attackCoroutine;
    /// <summary>
    /// 대쉬 코루틴을 저장하는 변수, 대쉬 중 여부 겸용
    /// </summary>
    private Coroutine dashCoroutine;
    public int air_attack_count = 1;
    protected override void OnEnable()
    {
        base.OnEnable();
        var pi = CameraManager.Instance.proCamera2DPointerInfluence;
        pi.MaxHorizontalInfluence = 2.5f;
        pi.MaxVerticalInfluence = 1.5f;
        pi.InfluenceSmoothness = 0.0f;
        CameraManager.Instance.ChangeSize = 6f;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        StopDash();
        StopAttack();
        StopHoldingWall();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        switch(CheckMapType(collision))
        {
            case MapType.Wall:
                if(unitState == UnitState.Air)
                {
                    foreach(var hit in Physics2D.BoxCastAll(transform.position, new Vector2(boxSizeX, boxSizeY * 2.1f), 0, Vector2.right * (transform.position.x - collision.contacts[0].point.x))) if(hit.transform.CompareTag("ground")) return;
                    HoldingWall(collision);
                }
                break;
            case MapType.Ground:
                if(unitState == UnitState.HoldingWall) StopHoldingWall();
            break;
        }
    }

    protected override void OnCollisionExit2D(Collision2D collision)
    {
        base.OnCollisionExit2D(collision);
        switch(CheckMapType(collision))
        {
            case MapType.Wall:
                if(unitState == UnitState.HoldingWall) StopHoldingWall();
                break;
        }
    }
    

    public override bool Attack(Vector3 clickPos)
    {
        if((unitState != UnitState.Default && unitState != UnitState.Air && unitState != UnitState.HoldingWall && unitState != UnitState.Dash) || unitState == UnitState.FormChange ||
            attackCoroutine != null || anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) return false; // 제어가 불가능한 상태일 경우 동작을 수행하지 않음
        if(dashCoroutine != null) StopDash();
        if(unitState == UnitState.HoldingWall) StopHoldingWall();
        //Vector2 testvec = new Vector2(1 * CheckDir(clickPos), clickPos.y - transform.position.y);//이렇게 되면 대각선으로 갈 수록 좁아짐
        //Vector2 testvec = (Vector2.up * (clickPos.y - transform.position.y)).normalized;
        //MeleeAttack.transform.localPosition = testvec;
        //MeleeAttack.transform.localPosition = (Vector2.right * CheckDir(clickPos))+testvec; // 클릭 방향으로 공격 위치 설정
        if (unitState == UnitState.Air)
        {
            if (air_attack_count < 1)
            {
                return false;
            }
            else
            {
                air_attack_count--;
            }
        }
        Debug.Log("공격횟수" + air_attack_count);
        
        Vector2 subvec = clickPos - transform.position;
        float deg = Mathf.Atan2(subvec.y, subvec.x) ;//mathf.de
        //deg*=Mathf.Deg2Rad;//라디안으로 바꿔주기는 하는데 이렇게 하면 좀 문제생김
        //Debug.Log(deg);
        MeleeAttack.transform.localPosition = new Vector3(Mathf.Cos(deg), Mathf.Sin(deg)*2,transform.localPosition.z);
        MeleeAttack.transform.localEulerAngles = new Vector3(0, 0, Quaternion.FromToRotation(Vector2.up, transform.position - MeleeAttack.transform.position).eulerAngles.z - 90);

        attackCoroutine = StartCoroutine(Attacking(deg));
        base.Attack(clickPos);
        return true;
    }

    /// <summary>
    /// 공격 중의 동작을 수행
    /// </summary>
    private IEnumerator Attacking(float deg)
    {
        unitState = UnitState.Attack;
        // 지속시간만큼 히트박스를 온오프
        MeleeAttack.SetActive(true);
        var tempDir = MeleeAttack.transform.localPosition.normalized;
        SetHorizontalVelocity(tempDir.x);
        SetHorizontalForce(tempDir.x * attackImpulse);
        SetVerticalForce(tempDir.y * attackImpulse * 0.25f);
        // SetHorizontalForce(Mathf.Sign(MeleeAttack.transform.localPosition.x)*attackImpulse);
        // float high = Mathf.Sin(deg) * 10;
        // if (high > 3)
        // {
        //     high = 3;
        // }
        // SetVerticalForce(high);
        yield return new WaitForSeconds(attackDuration);
        StopAttack();
    }

    private void StopAttack()
    {
        unitState = UnitState.Default;
        MeleeAttack.SetActive(false);
        attackCoroutine = null;
    }

    public override bool Move(float dir)
    {
        if(ControllerChecker() || unitState == UnitState.HoldingWall || 
            unitState == UnitState.Dash || unitState == UnitState.Attack) return false; // 제어가 불가능한 상태일 경우 동작을 수행하지 않음
        fixedDir = (int)dir; // 대쉬 방향을 저장
        return base.Move(dir);
    }
    public override bool Jump(KeyState jumpKey)
    {
        switch(jumpKey)
        {
            case KeyState.KeyDown:
            if(dashCoroutine != null) StopDash();
            if(unitState == UnitState.HoldingWall)
            {
                StopHoldingWall();
                isJumping = false;
                SetVerticalForce(jumpImpulse); // 윗 방향 힘 추가
                base.Move(fixedDir * movementSpeed * 3);
                return true;
            }
            else return base.Jump(jumpKey);
            case KeyState.KeyStay:
            if(unitState == UnitState.HoldingWall) return false;
            return base.Jump(jumpKey);
            case KeyState.KeyUp:
            return base.Jump(jumpKey);
        }
        return false;
    }

    Coroutine holdingCoroutine;
    private void HoldingWall(Collision2D collision)
    {
        anim.SetBool("isHoldingWall", true);
        unitState = UnitState.HoldingWall; // 벽붙기 상태로 변경
        fixedDir = -CheckDir(collision.contacts[0].point); // 벽의 반대 방향을 저장
        ResetForce();
        SetHorizontalVelocity(0);
        SetHorizontalForce(fixedDir * 0.11f);
        holdingCoroutine = StartCoroutine(Holding(-fixedDir));
    }
    private void StopHoldingWall()
    {
        anim.SetBool("isHoldingWall", false);
        unitState = UnitState.Air;
        StopCoroutine(holdingCoroutine);
        holdingCoroutine = null;
    }

    private IEnumerator Holding(float dir)
    {
        yield return new WaitUntil(() => 
            !Physics2D.Raycast(transform.position, Vector2.right*dir, boxSizeX*1.05f, 1<<LayerMask.NameToLayer("Map")) ||
            Physics2D.Raycast(transform.position, Vector2.down, boxSizeY*1.05f, 1<<LayerMask.NameToLayer("Map"))
        );
        StopHoldingWall();
    }

    public override bool Crouch(KeyState crouchKey)
    {
        if(ControllerChecker() || unitState == UnitState.Dash || unitState == UnitState.HoldingWall || unitState == UnitState.Attack) return false;
        return base.Crouch(crouchKey);
    }

    // 수정 필요함
    public override bool Dash()
    {
        if(ControllerChecker() || unitState == UnitState.HoldingWall || unitState == UnitState.FormChange || dashCoroutine != null) return false; // 제어가 불가능한 상태일 경우 동작을 수행하지 않음
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
        if(unitState == UnitState.HoldingWall) StopHoldingWall();
        unitState = UnitState.Dash; // 대쉬 상태로 변경
        var tempVel = fixedDir == 0 ? spriteRenderer.flipX ? -1 : 1 : Mathf.Sign(fixedDir);
        SetHorizontalVelocity(tempVel);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Dash"));
        while(anim.GetCurrentAnimatorStateInfo(0).IsName("Dash")) // 대쉬 지속 시간 동안
        {
            SetHorizontalForce(tempVel * movementSpeed * 2f);
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
        if(dashCoroutine != null) 
        {
            anim.SetTrigger("dash");
            StopCoroutine(dashCoroutine);
        }
        dashCoroutine = null;
        invalidation=false;
        unitState = UnitState.Default;
    }

    public override bool FormChange()
    {
        if(unitState != UnitState.Default) return false;
        else return base.FormChange();
    }

    public override bool Reload() => false;
}
