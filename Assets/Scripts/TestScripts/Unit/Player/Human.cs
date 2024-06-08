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
    /// 재장전 시간
    /// </summary>
    public float reloadTime;

    /// <summary>
    /// 총알 피해량
    /// </summary>
    public int bulletDamage;

    /// <summary>
    /// 총알 속도
    /// </summary>
    public float bulletSpeed;

    /// <summary>
    /// 최대 탄약 수
    /// </summary>
    public float maxAmmo;

    /// <summary>
    /// 현재 잔탄 수
    /// </summary>
    private float residualAmmo;

    /// <summary>
    /// 재장전 진행도
    /// </summary>
    [HideInInspector] private float reloadProgress;

    /// <summary>
    /// 대쉬 코루틴을 저장하는 변수, 대쉬 중 여부 겸용
    /// </summary>
    private Coroutine dashCoroutine;
    private Coroutine reloadCoroutine;

    protected override void OnDisable()
    {
        base.OnDisable();
        StopDash();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        switch(CheckMapType(collision))
        {
            case MapType.Wall:
                SetHorizontalForce(0);
                break;
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        switch(CheckMapType(collision))
        {
            case MapType.Wall:
                SetHorizontalForce(0);
                break;
        }
    }

    protected override void Start()
    {
        base.Start();
        residualAmmo = maxAmmo;
    }

    public override bool Attack(Vector3 clickPos)
    {
        if(residualAmmo <= 0) return false;
        Vector2 pos = Vector2.zero;
        GetSignedAngle((Vector2) transform.position, clickPos, out pos);
        GameObject _bullet = Instantiate(bullet);//총알을 공격포지션에서 생성함
        GameObject gObj = this.gameObject;
        _bullet.GetComponent<Bullet>().Set(transform.position, clickPos, bulletDamage, bulletSpeed, gObj, (Vector3)pos);
        residualAmmo--;
        return true;
    }

    public override bool Move(float dir)
    {
        if(ControllerChecker() || unitState == UnitState.Dash) return false; // 조작이 불가능한 상태일 경우 동작을 수행하지 않음
        //Anim
        return base.Move(dir);
    }

    public override bool Jump(KeyState jumpKey)
    {
        switch(jumpKey)
        {
            case KeyState.KeyDown:
                if(unitState != UnitState.Default) return false;
                return base.Jump(jumpKey);
            case KeyState.KeyStay:
            case KeyState.KeyUp:
                return base.Jump(jumpKey);
        }
        return false;
    }

    public override bool Dash()
    {
        if(unitState != UnitState.Default) return false; // 조작이 불가능한 상태일 경우 동작을 수행하지 않음
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
            SetHorizontalForce(Mathf.Sign(hzVel) * movementSpeed * 2f);
            yield return null;
        }
        StopDash();
    }

    public override bool FormChange()
    {
        if(unitState != UnitState.Default) return false;
        else return true;
    }

    public override bool Reload()
    {
        if(reloadCoroutine != null) return false;
        reloadCoroutine = StartCoroutine(Reloading());
        return true;
    }

    private IEnumerator Reloading()
    {
        float t = 0;
        while(t <= reloadTime)//1 = duration temp/duration 
        {
            t += Time.deltaTime;
            UIController.Instance.DrawReload(t / reloadTime);
            yield return null;
        }
        UIController.Instance.DrawReload(0);
        residualAmmo = maxAmmo;
        reloadCoroutine = null;
    }
}
