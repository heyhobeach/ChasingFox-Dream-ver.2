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
        if(collision.gameObject.CompareTag("Wall") && unitState == UnitState.Air &&
            Mathf.Abs(Vector2.Angle(Vector2.up, collision.contacts[0].normal)) != 180) // 공중에서 벽에 붙을 시
        {
            unitState = UnitState.HoldingWall; // 벽붙기 상태로 변경
            fixedDir = -CheckDir(collision.transform.position); // 벽의 반대 방향을 저장
            ResetForce();
            SetVel(0);
        }
        if(collision.gameObject.CompareTag("ground") && unitState == UnitState.HoldingWall) unitState = UnitState.Default;
    }
    protected override void OnCollisionStay2D(Collision2D collision)
    {
        base.OnCollisionStay2D(collision);
        
        if(collision.gameObject.CompareTag("ground") && wallCoroutine != null &&
            Mathf.Abs(Vector2.Angle(Vector2.up, collision.contacts[0].normal)) == 180)
            {
                unitState = UnitState.Default;
                StopCoroutine(wallCoroutine);
                wallCoroutine = null;
            }
    }

    public override bool Attack(Vector3 clickPos)
    {
        if((unitState != UnitState.Default && unitState != UnitState.Air && unitState != UnitState.HoldingWall)
             || attackCoroutine != null) return false; // 제어가 불가능한 상태일 경우 동작을 수행하지 않음
        //Vector2 testvec = new Vector2(1 * CheckDir(clickPos), clickPos.y - transform.position.y);//이렇게 되면 대각선으로 갈 수록 좁아짐
        //Vector2 testvec = (Vector2.up * (clickPos.y - transform.position.y)).normalized;
        //MeleeAttack.transform.localPosition = testvec;
        //MeleeAttack.transform.localPosition = (Vector2.right * CheckDir(clickPos))+testvec; // 클릭 방향으로 공격 위치 설정
        Vector2 subvec = clickPos - transform.position;
        float deg = Mathf.Atan2(subvec.y, subvec.x) ;//mathf.de
        //deg*=Mathf.Deg2Rad;//라디안으로 바꿔주기는 하는데 이렇게 하면 좀 문제생김
        Debug.Log(deg);
        MeleeAttack.transform.localPosition = new Vector3(Mathf.Cos(deg), Mathf.Sin(deg)*2,transform.localPosition.z);

        attackCoroutine = StartCoroutine(Attacking(deg));
        return true;
    }

    /// <summary>
    /// 공격 중의 동작을 수행
    /// </summary>
    private IEnumerator Attacking(float deg)
    {
        // 지속시간만큼 히트박스를 온오프
        MeleeAttack.SetActive(true);
        Move(Mathf.Sign(MeleeAttack.transform.localPosition.x)*attackImpulse);
        float high = Mathf.Sin(deg) * 10;
        if (high > 3)
        {
            high = 3;
        }
        AddVerticalForce(high);
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
        float wallJumpDuration = 0.2f;
        float t = 0;
        ResetForce();
        AddVerticalForce(-gravity * Time.deltaTime + jumpImpulse); // 윗 방향 힘 추가
        while(t < wallJumpDuration)
        {
            t += Time.deltaTime;
            AddVerticalForce(-gravity * Time.deltaTime + jumpForce * 2);
            AddHorizontalForce(fixedDir * movementSpeed); // 벽의 반대 방향 힘 추가
            yield return new WaitForFixedUpdate();
        }
        unitState = UnitState.Default;
        SetVel(fixedDir); // 벽의 반대 방향으로 힘 강제 변경, 자연스러운 동작을 위한 부분
        wallCoroutine = null;
    }

    // 수정 필요함
    public override bool Dash()
    {
        if(unitState != UnitState.Default || dashCoroutine != null) return false; // 제어가 불가능한 상태일 경우 동작을 수행하지 않음
        dashCoroutine = StartCoroutine(DashAffterInput());
        return true;
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
        unitState = UnitState.Default;
    }

    public override bool FormChange()
    {
        if(unitState != UnitState.Default) return false;
        else return true;
    }

    public override bool Reload() => false;
}
