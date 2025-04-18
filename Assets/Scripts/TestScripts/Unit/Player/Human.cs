using System.Collections;
using Com.LuisPedroFonseca.ProCamera2D;
using Damageables;
using MyUtiles;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.U2D.Animation;

[RequireComponent(typeof(ShootingAnimationController))]
[RequireComponent(typeof(SpriteLibrary))]
[RequireComponent(typeof(SpriteResolver))]
/// <summary>
/// 인간 상태 클래스, PlayerUnit 클래스를 상속함
/// </summary>
public class Human : PlayerUnit, IDoorInteractable
{
    [Header("Test"), Space(10)]
    public AttackType attackType = AttackType.Projectile;
    public float aimRadius = 0.5f;
    public enum AttackType { Projectile, Hitscan }

    [Space(5)]
    public ReloadType reloadType = ReloadType.Tap;
    public float reloadHoldTime = 0.5f;
    public int ammoToReload = 1;
    public enum ReloadType { Tap, Hold }

    public JumpType jumpType;
    private bool canJump;
    private Vector2 contactPos;
    public enum JumpType { Jump, Parkour }

    
    [Header("Human"), Space(10)]
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
    private bool _canInteract { get => UnitState == UnitState.Default; }
    public bool canInteract { get => _canInteract; }

    /// <summary>
    /// 대쉬 코루틴을 저장하는 변수, 대쉬 중 여부 겸용
    /// </summary>
    private Coroutine dashCoroutine;
    private Coroutine reloadCoroutine;
    private Coroutine attackCoroutine;

    private Vector2 fixedDir = Vector2.zero;

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        foreach(var map in mapTypesEnter)
        {
            switch(map)
            {
                case MapType.Wall:
                    canJump = true;
                    contactPos = collision.GetContact(0).point;
                break;
                case MapType.Platform:
                    mapSensor.platformSensor.gameObject.SetActive(true);
                break;
            }
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        base.OnCollisionStay2D(collision);
        foreach(var map in mapTypesEnter)
        {
            switch(map)
            {
                case MapType.Wall:
                    canJump = true;
                    contactPos = collision.GetContact(0).point;
                break;
                case MapType.Platform:
                    mapSensor.platformSensor.gameObject.SetActive(true);
                break;
            }
        }
    }

    protected override void OnCollisionExit2D(Collision2D collision)
    {
        base.OnCollisionExit2D(collision);
        foreach(var map in mapTypesExit)
        {
            if(jumpType == JumpType.Parkour)
            {
                switch(map)
                {
                    case MapType.Wall:
                        canJump = false;
                        var dir = contactPos-rg.position;
                        SetVerticalForce(dir.y * 10);
                        SetVerticalVelocity(dir.y * 10);
                        if(-fixedDir.normalized.x != dir.normalized.x)
                        {
                            SetHorizontalForce(dir.x * 10);
                            SetHorizontalVelocity(dir.x * 10);
                            if(isJumping) isJumping = false;
                        }
                    break;
                    case MapType.Platform:
                        mapSensor.platformSensor.gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        attackCoroutine = StartCoroutine(AttackDelay());
        isAttack = false;
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
        cameraState = new() {
            maxHorizontalInfluence = 5.15f,
            maxVerticalInfluence = 0.35f,
            influenceSmoothness = 0.275f,
            changeSize = 5.15f
        };
        bulletTimeCount = GameManager.GetHumanData();
        residualAmmo = maxAmmo;
        if(!anim.runtimeAnimatorController) anim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("PlayerAnim/Human/Human Lib Ani");
    }

    protected override void Update()
    {
        if(ServiceLocator.Get<GameManager>().isPaused || ControllerChecker()) return;

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
        if(attackType == AttackType.Projectile) return AttackType1(clickPos);
        else if(attackType == AttackType.Hitscan) return AttackType2(clickPos);
        else return false;
    }

    private bool AttackType1(Vector3 clickPos)
    {
        if (ControllerChecker() || UnitState == UnitState.FormChange || UnitState == UnitState.Dash || UnitState == UnitState.Reload || shootingAnimationController.isAttackAni || residualAmmo <= 0) return false;
        if (((Vector2)transform.position-(Vector2)clickPos).magnitude < ((Vector2)transform.position-shootingAnimationController.GetShootPosition()).magnitude) return false;
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
                _bullet.GetComponent<Bullet>().Set(
                    shootingAnimationController.GetShootPosition(), 
                    clickPos, 
                    shootingAnimationController.GetShootRotation(), 
                    bulletDamage, 
                    bulletSpeed, 
                    gObj, 
                    Vector2.zero, 
                    () => {
                        var player = rg.GetComponent<Player>();
                        ((Werewolf)player.forms[1]).currentGauge += player.brutalData.getGage;
                    }
                );
                residualAmmo--;
            }
            isAttack = false;
        }
    }
    private bool AttackType2(Vector3 clickPos)
    {
        if(ControllerChecker() || UnitState == UnitState.FormChange || UnitState == UnitState.Dash || UnitState == UnitState.Reload || shootingAnimationController.isAttackAni || residualAmmo <= 0) return false;
        if(((Vector2)transform.position-(Vector2)clickPos).magnitude < ((Vector2)transform.position-shootingAnimationController.GetShootPosition()).magnitude) return false;
        shootingAnimationController.AttackAni();
        this.clickPos = clickPos;
        sound?.PlayOneShot(soundClip, 0.3f);
        SoundManager.Instance.CoStartBullet(userGunsoud, false);
        ProCamera2DShake.Instance.Shake("GunShot ShakePreset");
        residualAmmo--;
        var vec = (Vector2)clickPos - shootingAnimationController.GetShootPosition();
        var hit = Physics2D.Raycast(clickPos, vec.normalized, vec.magnitude, 1<<LayerMask.NameToLayer("Map") | 1<<LayerMask.NameToLayer("Wall"));
        if(hit) 
        {
            GameObject obj = SoundManager.Instance.bullet.standbyBullet.Dequeue();
            obj.transform.position = hit.point;
            SoundManager.Instance.CoStartBullet(obj);
            return false;
        }
        hit = Physics2D.CircleCast(clickPos, aimRadius, Vector2.up, 0, 1<<LayerMask.NameToLayer("Enemy"));
        if(hit)
        {
            GameObject obj = SoundManager.Instance.bullet.standbyBullet.Dequeue();
            obj.transform.position = clickPos;
            SoundManager.Instance.CoStartBullet(obj);

            var temp = hit.collider.gameObject.GetInterface<IDamageable>();
            if(temp != null) temp.GetDamage(bulletDamage, transform, () => {
                var player = rg.GetComponent<Player>();
                ((Werewolf)player.forms[1]).currentGauge += player.brutalData.getGage;
            });
        }
        return true;
    }

    public override bool Move(Vector2 dir)
    {
        if(ControllerChecker() || UnitState == UnitState.Dash) return false; // 조작이 불가능한 상태일 경우 동작을 수행하지 않음
        fixedDir = dir;
        return base.Move(dir);
    }

    public override bool Jump(KeyState jumpKey)
    {
        if(jumpType == JumpType.Jump) return base.Jump(jumpKey);
        else if(jumpType == JumpType.Parkour)
        {
            switch(jumpKey)
            {
                case KeyState.KeyDown:
                    mapSensor.platformSensor.gameObject.SetActive(false);
                break;
                case KeyState.KeyUp:
                    mapSensor.platformSensor.gameObject.SetActive(true);
                break;
            }
            if(canJump) return base.Jump(jumpKey);
        }
        return false;
    }

    public override bool Crouch(KeyState crouchKey)
    {
        if(ControllerChecker() || UnitState == UnitState.Dash) return false;
        return base.Crouch(crouchKey);
    }

    public override bool Dash()
    {
        if(UnitState != UnitState.Default || dashCoroutine != null) return false; // 조작이 불가능한 상태일 경우 동작을 수행하지 않음
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
        UnitState = UnitState.Default;
        //여기 false
    }

    /// <summary>
    /// 대쉬 중 동작을 수행
    /// </summary>
    private IEnumerator DashAffterInput()
    {
        UnitState = UnitState.Dash;
        ResetForce();
        var tempVel = fixedDir.x == 0 ? spriteRenderer.flipX ? -1 : 1 : Mathf.Sign(fixedDir.x);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Dash"));
        while(anim.GetCurrentAnimatorStateInfo(0).IsName("Dash")) // 대쉬 지속 시간 동안
        {
            float t = Utils.EaseFromTo(0, 1, anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
            float mul = Mathf.Lerp(1.5f, 0.5f, t);
            SetHorizontalVelocity(tempVel * movementSpeed * mul);
            anim.SetFloat("hzForce", -0.5f);  
            yield return null;
        }
        StopDash();
    }

    public override bool FormChange()
    {
        if(UnitState != UnitState.Default) return false;
        else
        {
            ReloadCancel();
            return base.FormChange();
        }
    }

    public override bool Reload(KeyState reloadState)
    {
        if(reloadType == ReloadType.Tap) return ReloadType1(reloadState);
        else if(reloadType == ReloadType.Hold) return ReloadType2(reloadState);
        else return false;
    }

    private bool ReloadType1(KeyState reloadState)
    {
        if(ControllerChecker() || reloadCoroutine != null || residualAmmo >= maxAmmo || reloadState != KeyState.KeyDown) return false;
        base.Reload(reloadState);
        residualAmmo = 0;
        reloadCoroutine = StartCoroutine(Reloading());
        return true;
    }

    private IEnumerator Reloading()
    {
        if(UnitState != UnitState.Dash) UnitState = UnitState.Reload;
        yield return new WaitUntil(() => shootingAnimationController.isReloadAni);
        float animTime = shootingAnimationController.armAnim.GetCurrentAnimatorStateInfo(0).length;
        float t = 0;
        while(animTime >= t)
        {
            if(UnitState == UnitState.Dash) shootingAnimationController.NomalAni();
            t += Time.deltaTime;
            yield return null;
        }
        residualAmmo = maxAmmo;
        ReloadCancel();
    }
    private void ReloadCancel()
    {
        if(isGrounded) UnitState = UnitState.Default;
        else UnitState = UnitState.Air;
        if(reloadCoroutine != null) StopCoroutine(reloadCoroutine);
        shootingAnimationController?.NomalAni();
        reloadCoroutine = null;
    }
    
    private float reloadHoldingTime = 0f;
    private bool ReloadType2(KeyState reloadState)
    {
        if(ControllerChecker() || reloadCoroutine != null || residualAmmo >= maxAmmo || UnitState == UnitState.Dash) return false;
        switch(reloadState)
        {
            case KeyState.KeyDown:
                base.Reload(reloadState);
                UnitState = UnitState.Reload;
                reloadHoldingTime += Time.deltaTime;
                if(reloadHoldingTime >= reloadHoldTime)
                {
                    residualAmmo += ammoToReload;
                    reloadHoldingTime = 0f;
                    UnitState = UnitState.Default;
                }
                break;
            case KeyState.KeyStay:
                UnitState = UnitState.Reload;
                reloadHoldingTime += Time.deltaTime;
                if(reloadHoldingTime >= reloadHoldTime)
                {
                    residualAmmo += ammoToReload;
                    reloadHoldingTime = 0f;
                    UnitState = UnitState.Default;
                }
                break;
            case KeyState.KeyUp:
                shootingAnimationController.armAnim.ResetTrigger("reload");
                shootingAnimationController?.NomalAni();
                UnitState = UnitState.Default;
                reloadHoldingTime = 0f;
                break;
        }
        return true;
    }

    public override bool Skill2(KeyState skileKey)
    {
        switch(skileKey)
        {
            case KeyState.KeyDown:
                CameraManager.Instance.SetState(new(){
                    maxHorizontalInfluence = 15f,
                    maxVerticalInfluence = 15f,
                    influenceSmoothness = 0.5f,
                    changeSize = 9f
                });
            break;
            case KeyState.KeyUp:
                CameraManager.Instance.SetState(cameraState);
            break;
        }
        return base.Skill2(skileKey);
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