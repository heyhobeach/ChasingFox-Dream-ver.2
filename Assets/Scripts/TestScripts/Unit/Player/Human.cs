using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(ShootingAnimationController))]
/// <summary>
/// 인간 상태 클래스, PlayerUnit 클래스를 상속함
/// </summary>
public class Human : PlayerUnit
{
    public GameObject bullet;

    AudioSource sound;
    public AudioClip soundClip;

    public GameObject userGunsoud;

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
    /// 잔여 탄약 수
    /// </summary>
    private float _residualAmmo;
    public float residualAmmo 
    { 
        get => _residualAmmo;
        set
        {
            _residualAmmo = value;
            reloadGauge?.Invoke(_residualAmmo, maxAmmo);
        }
    }

    /// <summary>
    /// 최대 탄약 수
    /// </summary>
    public float maxAmmo;

    public int bulletTimeCount;

    public GaugeBar<Human>.GaugeUpdateDel reloadGauge;

    /// <summary>
    /// 대쉬 코루틴을 저장하는 변수, 대쉬 중 여부 겸용
    /// </summary>
    private Coroutine dashCoroutine;
    private Coroutine reloadCoroutine;
    private Coroutine attackCoroutine;

    private Vector2 fixedDir = Vector2.zero;

    protected override void OnEnable()
    {
        base.OnEnable();
        attackCoroutine = StartCoroutine(AttackDelay());
        isAttack = false;
        var pi = CameraManager.Instance.proCamera2DPointerInfluence;
        pi.MaxHorizontalInfluence = 5.15f;
        pi.MaxVerticalInfluence = 0.35f;
        pi.InfluenceSmoothness = 0.275f;
        CameraManager.Instance.ChangeSize = 5.15f;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        StopDash();
        ReloadCancel();
        if(attackCoroutine != null) StopCoroutine(attackCoroutine);
    }

    protected override void Start() => Init();

    public override void Init()
    {
        base.Init();
        sound=GetComponent<AudioSource>(); 
        //sound.PlayOneShot(soundClip, 0.3f);
        bulletTimeCount = GameManager.GetHumanData();
        residualAmmo = maxAmmo;
        if(!anim.runtimeAnimatorController) anim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("PlayerAnim/Human/Human Lib Ani");
    }

    protected override void Update()
    {
        if(GameManager.Instance.isPaused || ControllerChecker()) return;

        base.Update();
        
        var screenPoint = Input.mousePosition;
        screenPoint.z = Camera.main.transform.position.z;
        screenPoint = Camera.main.ScreenToWorldPoint(screenPoint);
        screenPoint.z = 0;

        shootingAnimationController.targetPosition = screenPoint;
    }

    bool isAttack = false;
    Vector3 clickPos;
    public override bool Attack(Vector3 clickPos)
    {
        if (ControllerChecker() || unitState == UnitState.FormChange || unitState == UnitState.Dash || unitState == UnitState.Reload || shootingAnimationController.isAttackAni || residualAmmo <= 0) return false;
        isAttack = true;
        this.clickPos = clickPos;
        return true;
    }
    private IEnumerator AttackDelay()
    {
        while(true)
        {
            yield return new WaitUntil(() => isAttack);
            shootingAnimationController.AttackAni();
            yield return null;
            // Debug.Log("공격");
            // Debug.Log("문제 이전");
            //sound.GetComponent<AudioSource>().Play();

            // Debug.Log("여기 문제");
            base.Attack(clickPos);
//            Assert.IsNotNull(sound, "총기 격발음 셋팅 안됨");
            sound?.PlayOneShot(soundClip, 0.3f);
            SoundManager.Instance.CoStartBullet(userGunsoud, false);
            ProCamera2DShake.Instance.Shake("GunShot ShakePreset");
            Assert.IsNotNull(bullet, "총알 셋팅 안됨");
            if(bullet)
            {
                GameObject _bullet = Instantiate(bullet);//총알을 공격포지션에서 생성함
                GameObject gObj = this.gameObject;
                _bullet.GetComponent<Bullet>().Set(shootingAnimationController.GetShootPosition(), clickPos, shootingAnimationController.GetShootRotation(), bulletDamage, bulletSpeed, gObj);
                residualAmmo--;
            }
            isAttack = false;
        }
    }

    public override bool Move(Vector2 dir)
    {
        if(ControllerChecker() || unitState == UnitState.Dash) return false; // 조작이 불가능한 상태일 경우 동작을 수행하지 않음
        fixedDir = dir; // 대쉬 방향을 저장
        //Anim
        return base.Move(dir);
    }

    public override bool Jump(KeyState jumpKey) => base.Jump(jumpKey);

    public override bool Crouch(KeyState crouchKey)
    {
        if(ControllerChecker() || unitState == UnitState.Dash) return false;
        return base.Crouch(crouchKey);
    }

    public override bool Dash()
    {
        if(unitState != UnitState.Default || dashCoroutine != null) return false; // 조작이 불가능한 상태일 경우 동작을 수행하지 않음
        base.Dash();
        dashCoroutine = StartCoroutine(DashAffterInput());
        return true;
    }

    /// <summary>
    /// 대쉬 동작 중지
    /// </summary>
    public void StopDash()
    {
        if(dashCoroutine != null) StopCoroutine(dashCoroutine);
        // invalidation = false;
        dashCoroutine = null;
        unitState = UnitState.Default;
        //여기 false
    }

    /// <summary>
    /// 대쉬 중 동작을 수행
    /// </summary>
    private IEnumerator DashAffterInput()
    {
        unitState = UnitState.Dash;
        ResetForce();
        var tempVel = fixedDir.x == 0 ? spriteRenderer.flipX ? -1 : 1 : Mathf.Sign(fixedDir.x);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Dash"));
        while(anim.GetCurrentAnimatorStateInfo(0).IsName("Dash")) // 대쉬 지속 시간 동안
        {
            SetHorizontalVelocity(tempVel * movementSpeed * 1.2f);
            anim.SetFloat("hzForce", -0.5f);
            yield return null;
        }
        StopDash();
    }

    public override bool FormChange()
    {
        if(unitState != UnitState.Default) return false;
        else
        {
            ReloadCancel();
            return base.FormChange();
        }
    }

    public override bool Reload()
    {
        if(ControllerChecker() || reloadCoroutine != null || residualAmmo >= maxAmmo) return false;
        base.Reload();
        residualAmmo = 0;
        reloadCoroutine = StartCoroutine(Reloading());
        return true;
    }

    private IEnumerator Reloading()
    {
        if(unitState != UnitState.Dash) unitState = UnitState.Reload;
        yield return new WaitUntil(() => shootingAnimationController.isReloadAni);
        float animTime = shootingAnimationController.armAnim.GetCurrentAnimatorStateInfo(0).length;
        float t = 0;
        while(animTime >= t)
        {
            if(unitState == UnitState.Dash) shootingAnimationController.NomalAni();
            t += Time.deltaTime;
            yield return null;
        }
        residualAmmo = maxAmmo;
        ReloadCancel();
    }
    private void ReloadCancel()
    {
        if(isGrounded) unitState = UnitState.Default;
        else unitState = UnitState.Air;
        if(reloadCoroutine != null) StopCoroutine(reloadCoroutine);
        shootingAnimationController?.NomalAni();
        reloadCoroutine = null;
    }

    public override void StopAllC()
    {
        StopDash();
        ReloadCancel();
        unitState = UnitState.Death;
    }
}

    // protected override void OnCollisionEnter2D(Collision2D collision)
    // {
    //     base.OnCollisionEnter2D(collision);
    //     switch(CheckMapType(collision))
    //     {
    //         case MapType.Wall:
    //             SetHorizontalForce(0);
    //             SetHorizontalVelocity(0);
    //             break;
    //     }
    // }
    // protected override void OnCollisionStay2D(Collision2D collision)
    // {
    //     base.OnCollisionEnter2D(collision);
    //     switch(CheckMapType(collision))
    //     {
    //         case MapType.Wall:
    //             SetHorizontalForce(0);
    //             SetHorizontalVelocity(0);
    //             break;
    //     }
    // }