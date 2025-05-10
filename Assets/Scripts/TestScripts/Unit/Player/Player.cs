using System;
using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using Damageables;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(PlayerController))]
/// <summary>
/// 플레이어 클래스. IUnitController 및 IDamageable 인터페이스를 상속
/// </summary>
public class Player : MonoBehaviour, IUnitController, IDamageable
{
    [SerializeField, DisableInInspector] private PlayerData playerData;
    /// <summary>
    /// 폼을 저장하는 배열
    /// 0 : 인간, 1 : 늑대인간, 2 : 버서커
    /// </summary>
    public PlayerUnit[] forms;

    /// <summary>
    /// 현재 폼을 담는 변수
    /// </summary>
    private PlayerUnit changedForm;
    public PlayerUnit ChagedForm { get => changedForm; }
    public UnitState UnitState { get => changedForm.UnitState; set => changedForm.UnitState = value; }

    public BrutalData brutalData { get => ((Werewolf)forms[1]).brutalData; set => ((Werewolf)forms[1]).brutalData = value; }
    public int currentGauge { get => ((Werewolf)forms[1]).currentGauge; set => ((Werewolf)forms[1]).currentGauge = value; }

    public int maxHealth { get => playerData.maxHealth; set => playerData.maxHealth = value; }
    public int health { get => playerData.health; set => playerData.health = value; }
    public bool invalidation { get; set; }

    /// <summary>
    /// 폼 체인지 딜레이 시간
    /// </summary>
    // public float changeDelay;

    /// <summary>
    /// 입력 방향을 저장할 변수
    /// </summary>
    private float fixedDir;

    /// <summary>
    /// 불릿타임 시간
    /// </summary>
    // [SerializeField] private float bulletTime;

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    private void Awake()
    {
        var path = $"ScriptableObject Datas/Player Data/playerData";
        playerData = Resources.Load<PlayerData>(path);
        if(!playerData)
        {
            PlayerData asset = ScriptableObject.CreateInstance<PlayerData>();
#if UNITY_EDITOR
            AssetDatabase.CreateAsset(asset, "Assets/Resources/" + path + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
            playerData = asset;
            playerData.Init();
        }
        foreach(var form in forms)
        {
            form.GetComponent<Collider2D>().enabled = false;
        }
    }

    public void Init(PlayerData.JsonData playerData)
    {
        fixedDir = 1;
        invalidation = false;

        foreach(var form in forms)
        {
            form.GetComponent<Collider2D>().enabled = true;
        }

        this.playerData.Init(playerData);

        changedForm = forms[playerData.formIdx];
        brutalData = GameManager.GetBrutalData();
        currentGauge = playerData.brutalGaugeRemaining;
        ((Werewolf)forms[1]).formChangeTest += () => FormChange();
        formChangeDelegate = ToWerewolf;
        ((Werewolf)forms[1]).currentCount = brutalData.isDoubleTime ? 2 : 1;
        foreach(PlayerUnit form in forms) 
        {
            form.gameObject.SetActive(false);
            form.Init();
        }
        changedForm.gameObject.SetActive(true);
    }

    public PlayerData.JsonData GetJsonData()
    {
        playerData.brutalGaugeRemaining = currentGauge;
        return playerData;
    }

    public void DeathFeedBack(Vector2 dir)
    {
        var impulse = Mathf.Sign(((Vector2)transform.position - dir).normalized.x) * 5;
        changedForm.SetHorizontalVelocity(impulse);
        changedForm.SetHorizontalForce(impulse);
    }
    public bool Crouch(KeyState crouchKey) => changedForm.Crouch(crouchKey);

    public bool Jump(KeyState jumpKey) => changedForm.Jump(jumpKey);

    public bool Move(Vector2 dir)
    {
        if(dir.x != 0) fixedDir = dir.x;
        return changedForm.Move(dir);
    }

    public bool Attack(Vector3 clickPos) => changedForm.Attack(clickPos);

    public bool Dash()
    {
        if(changedForm.Dash())
        {
            invalidation = true;
            StartCoroutine(DashDelay());
            return true;
        }
        return false;
    }

    IEnumerator DashDelay()
    {
        yield return new WaitForSeconds(0.3f);
        invalidation = false;
    }

    public void Death()
    {
        // Debug.Log("유저 사망");
        ProCamera2DShake.Instance.Shake("Hit ShakePreset");
        invalidation = true;
        changedForm.Death();
        StartCoroutine(PopupDelay());
        // if(changing != null) StopCoroutine(changing);
        // changing = null;
    }
    IEnumerator PopupDelay()
    {
        ServiceLocator.Get<GameManager>().RetryScene();
        yield return new WaitForSeconds(1.5f);
        PopupManager.Instance.DeathPop();
    }

    public bool Skill1(Vector2 pos) => changedForm.Skill1(pos);
    public bool Skill2(KeyState skileKey) => changedForm.Skill2(skileKey);

    private delegate bool FormChangeDelegate();
    private FormChangeDelegate formChangeDelegate;

    public bool FormChange() => formChangeDelegate.Invoke();
    private bool ToWerewolf()
    {
        if(changedForm.UnitState == UnitState.Default && changedForm.GetType() != typeof(Werewolf) && ((Werewolf) forms[1]).isFormChangeReady())
        {
            invalidation = true;
            changedForm.gameObject.SetActive(false);
            changedForm = forms[1];
            changedForm.gameObject.SetActive(true);
            formChangeDelegate = ToHuman;
            changedForm.spriteRenderer.flipX = fixedDir > 0 ? false : true;
        }
        return true;
    }
    private bool ToHuman()
    {
        if(changedForm.GetType() != typeof(Human)) 
        {
            invalidation = false;
            changedForm.gameObject.SetActive(false);
            changedForm = forms[0];
            changedForm.gameObject.SetActive(true);
            // Attack(ClickPos());
            Dash();
            // Reload(); 
            formChangeDelegate = ToWerewolf;
        }
        return true;

        // Vector3 ClickPos()
        // {
        //     var screenPoint = Input.mousePosition;//마우스 위치 가져옴
        //     screenPoint.z = Camera.main.transform.position.z;
        //     Vector3 pos = Camera.main.ScreenToWorldPoint(screenPoint);
        //     pos.z = 0;
        //     return pos;
        // }
    }

    public bool Reload(KeyState reloadKey) => changedForm.Reload(reloadKey); 

    // void Update()
    // {
        // if(changedForm.GetType() != typeof(Werewolf))
        // {
        //     if (changedForm.UnitState == UnitState.Dash) invalidation = true;
        //     else invalidation = false;
        // }
        // if(changedForm.GetType() == typeof(Werewolf) && ((Werewolf) changedForm).isFormChangeReady) FormChange();
        // if (changedForm.UnitState == UnitState.Dash)
        // {
        //     invalidation = true;
        //     Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Bullet"), true);
        // }
        // else
        // {
        //     invalidation = false;
        //     Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Bullet"), false);
        // }

        //OverlapTest();
    // }

    public void OverlapTest()//여기 부분 수정필요
    {
        CompositeCollider2D ringcol = GetComponent<CompositeCollider2D>();
        //ringcol.Overlap(filter, results);
        // float radius = 3f;
        List<Collider2D> colliders = null;
        Physics2D.OverlapCollider(ringcol, colliders);
        foreach(var col in colliders)
        {
            Debug.Log("안의 콜라이더" + col.transform.gameObject.name);
        }
    }

    public void FormPostitionReset()
    {
        foreach(var form in forms) form.transform.localPosition = Vector2.zero;
    }
    public void PlayerPositionSet(Transform transform)
    {
        this.transform.position = transform.position;
        changedForm.ResetForce();
    }
    public void PlayerPositionSet(Vector3 pos)
    {
        transform.position = pos;
        changedForm.ResetForce();
    }
    public void FreezePostion(bool isFreeze)
    {
        if(isFreeze) GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
        else GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void SetForTimeline()
    {
        invalidation = true;
        changedForm.gameObject.SetActive(false);
        changedForm = forms[2];
        changedForm.gameObject.SetActive(true);
        formChangeDelegate = () => false;
    }
    public void SetoffForTimeline()
    {
        invalidation = false;
        changedForm.gameObject.SetActive(false);
        changedForm = forms[0];
        changedForm.gameObject.SetActive(true);
        formChangeDelegate = ToWerewolf;
    }
}


    // private IEnumerator BulletTime()
    // {
    //     var tempDir = fixedDir;
    //     changedForm.UnitState = UnitState.FormChange;
    //     yield return new WaitUntil(() => changedForm.anim.GetCurrentAnimatorStateInfo(0).IsName("Dash"));
    //     yield return new WaitUntil(() => {
    //         FixMove();
    //         return !(changedForm.GetType() == typeof(Human) && changedForm.anim.GetCurrentAnimatorStateInfo(0).IsName("Dash") && changedForm.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.3f);
    //     });
    //     invalidation = true;
    //     if(changedForm.GetType() == typeof(Human) && ((Human) forms[0]).bulletTimeCount-- > 0)
    //     {
    //         isBulletTime = true;
    //         Time.timeScale = 0.05f;
    //         Time.fixedDeltaTime = 0.05f * 0.02f;
    //         changedForm.anim.speed = 0;
    //         yield return new WaitForSecondsRealtime(bulletTime);
    //         isBulletTime = false;
    //         changedForm.shootingAnimationController.NomalAni();

    //         changedForm.anim.speed = 1;
    //         Time.timeScale = 1;
    //         Time.fixedDeltaTime = 1 * 0.02f;
    //     }
    //     yield return new WaitUntil(() => {
    //         FixMove();
    //         return !(changedForm.anim.GetCurrentAnimatorStateInfo(0).IsName("Dash") && changedForm.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f);
    //     });
    //     invalidation = false;
    //     changedForm.UnitState = UnitState.Default;

    //     changing = null;

    //     void FixMove()
    //     {
    //         changedForm.SetHorizontalVelocity(tempDir * changedForm.movementSpeed);
    //     }
    // }

        // public bool FormChange()
    // {
    //     if(changedForm.UnitState == UnitState.Dash || PlayerUnit.ControllerChecker(changedForm)) return false; // 대쉬 중이거나 제어가 불가능한 상태일 경우 동작을 수행하지 않음
    //     foreach(PlayerUnit form in forms) form.gameObject.SetActive(false);
    //     if(changedForm is Human) changedForm = forms[1]; // 인간 상태일 시 늑대인간으로 변경
    //     else if(changedForm is Werwolf) changedForm = forms[0]; // 늑대인간 상태일 시 인간으로 변경
    //     changedForm.gameObject.SetActive(true);
    //     changedForm.SetVel(dashVel); // 자연스러운 대쉬 동작을 위한 부분
    //     return true;
    // }

        // IEnumerator Test()
    // {
    //     yield return new WaitForSeconds(3f);
    //     foreach(PlayerUnit form in forms) form.gameObject.SetActive(false);
    //     health = maxHealth; // 체력 초기화
    //     changedForm = forms[2]; // 상태를 버서커 상태로 변경
    //     changedForm.gameObject.SetActive(true);
    //     invalidation = false;
    // }

    
    // public bool FormChange()
    // {
    //     if(changedForm.GetType() == typeof(Berserker) || changing != null) return false; // 대쉬 중이거나 제어가 불가능한 상태일 경우 동작을 수행하지 않음
    //     bool b = false;
    //     if(changedForm.GetType() == typeof(Human) && !((Werewolf) forms[1]).isFormChangeReady && changedForm.FormChange())
    //     {
    //         changing = StartCoroutine(ChangeWerewolf());
    //         b = true;

    //     }

    //     else if(changedForm.GetType() == typeof(Werewolf) && changedForm.FormChange())
    //     {
    //         changing = StartCoroutine(ChangeHuman());
    //         b = true;
    //     }
    //     return b;
    // }
    // private IEnumerator ChangeWerewolf()
    // {
    //     var tempDir = fixedDir;
    //     invalidation = true;
    //     yield return new WaitUntil(() => changedForm.anim.GetCurrentAnimatorStateInfo(0).IsName("FormChange"));
    //     yield return new WaitUntil(() => {
    //         FixMove();
    //         return !(changedForm.GetType() == typeof(Human) && changedForm.anim.GetCurrentAnimatorStateInfo(0).IsName("FormChange") && changedForm.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.3f);
    //     });
    //     yield return new WaitUntil(() => {
    //         FixMove();
    //         return !(changedForm.anim.GetCurrentAnimatorStateInfo(0).IsName("FormChange") && changedForm.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f);
    //     });
    //     invalidation = false;

    //     foreach(PlayerUnit form in forms) form.gameObject.SetActive(false);
    //     changedForm = forms[1]; // 인간 상태일 시 늑대인간으로 변경
    //     changedForm.gameObject.SetActive(true);
    //     FixMove();
    //     changedForm.UnitState = UnitState.Default;

    //     changing = null;

    //     void FixMove()
    //     {
    //         changedForm.SetHorizontalVelocity(tempDir * changedForm.movementSpeed);
    //     }
    // }
    // private IEnumerator ChangeHuman()
    // {
    //     var tempDir = fixedDir;
    //     invalidation = true;
    //     yield return new WaitUntil(() => changedForm.anim.GetCurrentAnimatorStateInfo(0).IsName("FormChange"));
    //     yield return new WaitUntil(() => {
    //         FixMove();
    //         return !(changedForm.anim.GetCurrentAnimatorStateInfo(0).IsName("FormChange") && changedForm.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f);
    //     });
    //     invalidation = false;
    //     ((Werewolf) forms[1]).changeGauge -= ((Werewolf) forms[1]).brutalData.frm;
    //     foreach(PlayerUnit form in forms) form.gameObject.SetActive(false);
    //     changedForm = forms[0]; // 늑대인간 상태일 시 인간으로 변경
    //     changedForm.gameObject.SetActive(true);
    //     FixMove();
    //     changedForm.UnitState = UnitState.Default;

    //     changing = null;

    //     void FixMove()
    //     {
    //         changedForm.SetHorizontalVelocity(tempDir * changedForm.movementSpeed);
    //     }
    // }
    // private Coroutine changing;
    // private bool isBulletTime;
    // public void FormChange(int i)
    // {
    //     if(changing != null)
    //     {
    //         StopCoroutine(changing);
    //         Time.timeScale = 1;
    //     }
    //     for(int j = 0; j < forms.Length; j++)
    //     {
    //         forms[j].gameObject.SetActive(false);
    //         if(i == j) forms[j].gameObject.SetActive(true);
    //     }
    //     changedForm = forms[i];
    // }