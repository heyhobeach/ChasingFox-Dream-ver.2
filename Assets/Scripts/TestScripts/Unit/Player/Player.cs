using System;
using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using Damageables;
using UnityEngine;
using UnityEngine.PlayerLoop;


[RequireComponent(typeof(PlayerController))]
/// <summary>
/// 플레이어 클래스. IUnitController 및 IDamageable 인터페이스를 상속
/// </summary>
public class Player : MonoBehaviour, IUnitController, IDamageable
{
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

    [SerializeField] private int _maxHealth;    //?private아닌가 A : 맞음
    public int maxHealth { get => _maxHealth; set => _maxHealth = value; }
    public int health { get; set; }
    public bool invalidation { get; set; }

    [SerializeField] private GameObject playerUI;

    public static GameObject pObject;


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

    private void OnEnable()
    {
        if(changing != null)
        {
            StopCoroutine(changing);
            changing = null;
        }
    }

    public void Init(PlayerData playerData = null)
    {
        foreach(PlayerUnit form in forms) 
        {
            form.gameObject.SetActive(false);
            form.Init();
        }
        health = maxHealth; // 체력 초기화
        fixedDir = 1;
        invalidation = false;

        if(playerData == null) 
        {
            forms[0].gameObject.SetActive(true);
            changedForm = forms[0];
            return;
        }
        health = playerData.health;
        forms[playerData.formIdx].gameObject.SetActive(true);
        changedForm = forms[playerData.formIdx];
        ((Werewolf)forms[1]).brutalData = playerData.brutalData;
        ((Werewolf)forms[1]).changeGauge = playerData.brutalGaugeRemaining;
        // foreach(PlayerUnit form in forms) form.Init();
        pObject = this.gameObject;
    }
    public PlayerData DataSet()
    {
        PlayerData playerData = ScriptableObject.CreateInstance<PlayerData>();
        playerData.health = health;
        playerData.formIdx = Array.FindIndex(forms, (form) => form == changedForm);
        playerData.brutalData = ((Werewolf)forms[1]).brutalData;
        playerData.brutalGaugeRemaining = ((Werewolf)forms[1]).changeGauge;

        return playerData;
    }

    public void DeathFeedBack(Vector2 dir)
    {
        changedForm.SetHorizontalVelocity(dir.x * 5);
    }
    public bool Crouch(KeyState crouchKey) => changedForm.Crouch(crouchKey);

    public bool Jump(KeyState jumpKey) => changedForm.Jump(jumpKey);

    public bool Move(Vector2 dir)
    {
        if(dir.x != 0)fixedDir = dir.x;
        return changedForm.Move(dir);
    }

    public bool Attack(Vector3 clickPos)
    {
        bool temp = false;
        changedForm.Attack(clickPos);
        return temp;
    }

    public bool Dash() 
    {
        changedForm.Dash();
        return true;
    }

    public void Death()
    {
        // Debug.Log("유저 사망");
        ProCamera2DShake.Instance.Shake("Hit ShakePreset");
        invalidation = true;
        changedForm.Death();
        StartCoroutine(Test2());
        if(changing != null) StopCoroutine(changing);
        changing = null;
    }
    

    IEnumerator Test2()
    {
        yield return new WaitForSeconds(3f);
        // changedForm.gameObject.SetActive(false);
        PopupManager.Instance.DeathPop();

    }
    private Coroutine changing;
    private bool isBulletTime;
    public void FormChange(int i)
    {
        if(changing != null)
        {
            StopCoroutine(changing);
            Time.timeScale = 1;
        }
        for(int j = 0; j < forms.Length; j++)
        {
            forms[j].gameObject.SetActive(false);
            if(i == j) forms[j].gameObject.SetActive(true);
        }
        changedForm = forms[i];
    }
    public bool FormChange()
    {
        if(changedForm.GetType() == typeof(Berserker) || changing != null) return false; // 대쉬 중이거나 제어가 불가능한 상태일 경우 동작을 수행하지 않음
        bool b = false;
        if(changedForm.GetType() == typeof(Human) && !((Werewolf) forms[1]).isFormChangeReady && changedForm.FormChange())
        {
            changing = StartCoroutine(ChangeWerewolf());
            b = true;

        }

        else if(changedForm.GetType() == typeof(Werewolf) && changedForm.FormChange())
        {
            changing = StartCoroutine(ChangeHuman());
            b = true;
        }
        return b;
    }
    private IEnumerator ChangeWerewolf()
    {
        var tempDir = fixedDir;
        invalidation = true;
        yield return new WaitUntil(() => changedForm.anim.GetCurrentAnimatorStateInfo(0).IsName("FormChange"));
        yield return new WaitUntil(() => {
            FixMove();
            return !(changedForm.GetType() == typeof(Human) && changedForm.anim.GetCurrentAnimatorStateInfo(0).IsName("FormChange") && changedForm.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.3f);
        });
        yield return new WaitUntil(() => {
            FixMove();
            return !(changedForm.anim.GetCurrentAnimatorStateInfo(0).IsName("FormChange") && changedForm.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f);
        });
        invalidation = false;

        foreach(PlayerUnit form in forms) form.gameObject.SetActive(false);
        changedForm = forms[1]; // 인간 상태일 시 늑대인간으로 변경
        changedForm.gameObject.SetActive(true);
        FixMove();
        changedForm.UnitState = UnitState.Default;

        changing = null;

        void FixMove()
        {
            changedForm.SetHorizontalVelocity(tempDir * changedForm.movementSpeed);
        }
    }
    private IEnumerator ChangeHuman()
    {
        var tempDir = fixedDir;
        invalidation = true;
        yield return new WaitUntil(() => changedForm.anim.GetCurrentAnimatorStateInfo(0).IsName("FormChange"));
        yield return new WaitUntil(() => {
            FixMove();
            return !(changedForm.anim.GetCurrentAnimatorStateInfo(0).IsName("FormChange") && changedForm.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f);
        });
        invalidation = false;
        ((Werewolf) forms[1]).changeGauge -= ((Werewolf) forms[1]).brutalData.frm;
        foreach(PlayerUnit form in forms) form.gameObject.SetActive(false);
        changedForm = forms[0]; // 늑대인간 상태일 시 인간으로 변경
        changedForm.gameObject.SetActive(true);
        FixMove();
        changedForm.UnitState = UnitState.Default;

        changing = null;

        void FixMove()
        {
            changedForm.SetHorizontalVelocity(tempDir * changedForm.movementSpeed);
        }
    }

    public bool Reload() => changedForm.Reload(); 

    void Update()
    {
        pObject = this.gameObject;
        if(changedForm.GetType() == typeof(Werewolf) && ((Werewolf) changedForm).isFormChangeReady) FormChange();
        if (changedForm.UnitState == UnitState.Dash)
        {
            invalidation = true;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Bullet"), true);
        }
        else
        {
            invalidation = false;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Bullet"), false);
        }

        playerUI.SetActive(!GameManager.Instance.isPaused);

        //OverlapTest();
    }

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