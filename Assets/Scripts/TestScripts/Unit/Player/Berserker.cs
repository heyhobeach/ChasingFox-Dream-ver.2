using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 버서커 상태 클래스, PlayerUnit 클래스를 상속함
/// </summary>
public class Berserker : PlayerUnit
{
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
        throw new System.NotImplementedException();
    }

    public override bool Dash()
    {
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
    }

    /// <summary>
    /// 대쉬 중의 동작을 수행
    /// </summary>
    private IEnumerator DashAffterInput()
    {
        float t = 0;
        while(t < dashDuration)
        {
            t += Time.deltaTime;
            AddHorizontalForce(hzVel * movementSpeed * 2f);
            yield return null;
        }
        dashCoroutine = null;
    }

    public override bool FormChange() => false;

    public override bool Reload() => false;
}
