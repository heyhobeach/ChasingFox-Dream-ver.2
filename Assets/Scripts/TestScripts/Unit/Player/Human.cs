using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인간 상태 클래스, PlayerUnit 클래스를 상속함
/// </summary>
public class Human : PlayerUnit
{
    public GameObject bullet;

    /// <summary>
    /// 총알 피해량
    /// </summary>
    public int bulletDamage;

    /// <summary>
    /// 총알 속도
    /// </summary>
    public float bulletSpeed;

    /// <summary>
    /// 대쉬 코루틴을 저장하는 변수, 대쉬 중 여부 겸용
    /// </summary>
    private Coroutine dashCoroutine;

    protected override void OnDisable()
    {
        base.OnDisable();
        StopDash();
    }

    public override bool Attack(Vector3 clickPos)
    {
        Vector2 pos = Vector2.zero;
        GetSignedAngle((Vector2) transform.position, clickPos, out pos);
        GameObject _bullet = Instantiate(bullet);//총알을 공격포지션에서 생성함
        GameObject gObj = this.gameObject;
        _bullet.GetComponent<Bullet>().Set(transform.position, clickPos, bulletDamage, bulletSpeed, gObj, (Vector3)pos);
        return true;
    }

    public override bool Move(float dir)
    {
        if(ControllerChecker() || unitState == UnitState.Dash) return false; // 조작이 불가능한 상태일 경우 동작을 수행하지 않음
        return base.Move(dir);
    }

    public override bool Dash()
    {
        if(ControllerChecker() || unitState == UnitState.Air) return false; // 조작이 불가능한 상태일 경우 동작을 수행하지 않음
        if(dashCoroutine == null)
        {
            dashCoroutine = StartCoroutine(DashAffterInput());
            return true;
        }
        else return false;
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

    /// <summary>
    /// 대쉬 중 동작을 수행
    /// </summary>
    private IEnumerator DashAffterInput()
    {
        float t = 0;
        unitState = UnitState.Dash;
        while(t < dashDuration) // 대쉬 지속 시간 동안
        {
            t += Time.deltaTime;
            AddHorizontalForce(Mathf.Sign(hzVel) * movementSpeed * 2f);
            yield return null;
        }
        StopDash();
    }
}
