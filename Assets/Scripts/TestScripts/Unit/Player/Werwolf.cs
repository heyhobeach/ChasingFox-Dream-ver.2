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
    /// 대쉬 공격 여부
    /// </summary>
    private bool isDashAttack;

    /// <summary>
    /// 고정된 방향을 저장하기 위한 변수
    /// </summary>
    private int fixedDir;

    /// <summary>
    /// 공격 코루틴을 저장하는 변수, 공격 중 여부 겸용
    /// </summary>
    private Coroutine attackCoroutine;
    /// <summary>
    /// 대쉬 코루틴을 저장하는 변수, 대쉬 중 여부 겸용
    /// </summary>
    private Coroutine dashCoroutine;
    /// <summary>
    /// 벽점프 코루틴을 저장하는 변수, 벽점프 중 여부 겸용
    /// </summary>
    private Coroutine wallCoroutine;

    protected override void OnDisable()
    {
        base.OnDisable();
        StopDash();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if(collision.gameObject.CompareTag("Wall") && unitState == UnitState.Air) // 공중에서 벽에 붙을 시
        {
            unitState = UnitState.HoldingWall; // 벽붙기 상태로 변경
            fixedDir = -CheckDir(collision.transform.position); // 벽의 반대 방향을 저장
        }
    }
    protected override void OnCollisionExit2D(Collision2D collision)
    {
        base.OnCollisionExit2D(collision);
        if(collision.gameObject.CompareTag("Wall") && unitState == UnitState.HoldingWall && isGrounded) unitState = UnitState.Default; // 벽붙기 상태일 시 기본 상태로 변경
    }

    public override bool Attack(Vector3 clickPos)
    {
        if(ControllerChecker() || unitState == UnitState.Hide || 
            unitState == UnitState.HoldingWall) return false; // 제어가 불가능한 상태일 경우 동작을 수행하지 않음
        else if(unitState == UnitState.Dash) isDashAttack = true; // 대쉬 중 공격 입력 시 대쉬 공격 트리거 On
        else if(attackCoroutine == null) // 공격 중이 아닐 경우
        {
            MeleeAttack.transform.localPosition = Vector2.right * CheckDir(clickPos); // 클릭 방향으로 공격 위치 설정
            attackCoroutine = StartCoroutine(Attacking());
        }
        return true;
    }

    /// <summary>
    /// 공격 중의 동작을 수행
    /// </summary>
    private IEnumerator Attacking()
    {
        // 지속시간만큼 히트박스를 온오프
        MeleeAttack.SetActive(true);
        yield return new WaitForSeconds(attackDuration);
        MeleeAttack.SetActive(false);
        attackCoroutine = null;
    }

    public override bool Move(float dir)
    {
        if(ControllerChecker() || unitState == UnitState.HoldingWall || 
            unitState == UnitState.Dash) return false; // 제어가 불가능한 상태일 경우 동작을 수행하지 않음
        fixedDir = (int)dir; // 대쉬 방향을 저장
        return base.Move(dir);
    }
    public override bool Jump(KeyState jumpKey)
    {
        if(unitState == UnitState.HoldingWall && jumpKey == KeyState.KeyDown &&
            wallCoroutine == null) wallCoroutine = StartCoroutine(WallJump()); // 벽에 붙은 상태일 경우 벽점프 실행
        else if(ControllerChecker() || unitState == UnitState.HoldingWall) return false; // 제어가 불가능한 상태일 경우 동작을 수행하지 않음
        return base.Jump(jumpKey);
    }

    /// <summary>
    /// 벽점프 중의 동작을 수행
    /// </summary>
    private IEnumerator WallJump()
    {
        float wallJumpDuration = 0.3f;
        float t = 0;
        ResetForce();
        AddVerticalForce(jumpForce); // 윗 방향 힘 추가
        while(t < wallJumpDuration)
        {
            t += Time.deltaTime;
            AddHorizontalForce(fixedDir * movementSpeed); // 벽의 반대 방향 힘 추가
            yield return null;
        }
        unitState = UnitState.Default;
        SetVel(fixedDir); // 벽의 반대 방향으로 힘 강제 변경, 자연스러운 동작을 위한 부분
        wallCoroutine = null;
    }

    // 수정 필요함
    public override bool Dash()
    {
        bool returnVal = false;
        if(ControllerChecker() || unitState == UnitState.Air) returnVal = false; // 제어가 불가능한 상태일 경우 동작을 수행하지 않음
        if(dashCoroutine != null)
        {
            isDashAttack = true;
            returnVal = false;
        }
        else
        {
            dashCoroutine = StartCoroutine(DashAffterInput());
            returnVal = true;
        }
        return returnVal;
    }

    /// <summary>
    /// 대쉬 중의 동작을 수행
    /// </summary>
    private IEnumerator DashAffterInput()
    {
        float t = 0;
        unitState = UnitState.Dash; // 대쉬 상태로 변경
        while(t < dashDuration) // 대쉬 지속시간만큼 동작
        {
            t += Time.deltaTime;
            AddHorizontalForce(Mathf.Sign(hzVel) * movementSpeed * 2f); // 수평 방향으로 힘 추가
            if(isDashAttack) // 대쉬 중 공격 감지 시 대쉬 공격 수행
            {
                DashAttack();
                break;
            }
            yield return null;
        }
        StopDash();
    }

    /// <summary>
    /// 대쉬 중 공격 입력 시 동작을 수행
    /// </summary>
    public void DashAttack()
    {
        // 대쉬 및 공격을 초기화
        StopDash();
        if(attackCoroutine != null) StopCoroutine(attackCoroutine);

        MeleeAttack.transform.localPosition = new Vector3(fixedDir, 0); // 공격 위치 재설정
        StartCoroutine(DashAttacking());
    }
    /// <summary>
    /// 대쉬공격 중 동작을 수행
    /// </summary>
    private IEnumerator DashAttacking()
    {
        float t = 0;
        unitState = UnitState.Pause; // 모든 입력을 제한 시키기 위한 상태 변경
        AddVerticalForce(jumpForce * 0.5f); // 윗 방향 힘 추가
        StartCoroutine(Attacking()); // 공격 시작
        while(t < dashDuration) // 지속시간 동안 수평 힘 추가
        {
            AddHorizontalForce(fixedDir * movementSpeed * 2f);
            yield return null;
        }
        isDashAttack = false; // 대쉬 공격 트리거 초기화
        unitState = UnitState.Default; // 기본 상태로 변경
    }

    /// <summary>
    /// 대쉬 동작 중지
    /// </summary>
    public void StopDash()
    {
        if(dashCoroutine != null) StopCoroutine(dashCoroutine);
        dashCoroutine = null;
        unitState = UnitState.Default;
    }
}