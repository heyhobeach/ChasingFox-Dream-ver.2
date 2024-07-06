using System;
using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

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

    private int bulletTimeCount;
    private GameManager.BrutalData brutalData;

    public static GameObject pObject;

    /// <summary>
    /// 폼 체인지 딜레이 시간
    /// </summary>
    public float changeDelay;

    /// <summary>
    /// 늑대인간 폼 유지를 위한 게이지 변수
    /// </summary>
    public float changeGage;

    /// <summary>
    /// 입력 방향을 저장할 변수
    /// </summary>
    private float fixedDir;

    /// <summary>
    /// 불릿타임 시간
    /// </summary>
    public float bulletTime;

    private void OnEnable()
    {
        if(changing != null)
        {
            StopCoroutine(changing);
            changing = null;
        }
    }

    private void Start()
    {
        foreach(PlayerUnit form in forms) form.gameObject.SetActive(false);
        // 인간 상태를 현재 상태로 변경
        changedForm = forms[0]; 
        changedForm.gameObject.SetActive(true);
        health = maxHealth; // 체력 초기화
        fixedDir = 1;
        bulletTimeCount = GameManager.Instance.GetHumanData();
        brutalData = GameManager.Instance.GetBrutalData();
        changeGage = brutalData.maxGage;
    }

    public bool Crouch(KeyState crouchKey) => changedForm.Crouch(crouchKey);

    public bool Jump(KeyState jumpKey) => changedForm.Jump(jumpKey);

    public bool Move(float dir)
    {
        if(dir != 0)fixedDir = dir;
        return changedForm.Move(dir);
    }

    public bool Attack(Vector3 clickPos)
    {
        if(changedForm is Werwolf) changeGage -= brutalData.atk;
        return changedForm.Attack(clickPos);
    }

    public bool Dash() => changedForm.Dash();

    /// <summary>
    /// 임시
    /// </summary>
    public IEnumerator ChangeDelay()
    {
        yield return new WaitUntil(() => changeGage <= 0);
    }

    public void Death()
    {
        Debug.Log("유저 사망");
        ProCamera2DShake.Instance.Shake("Hit ShakePreset");
        invalidation = true;
        if(changedForm is not Berserker) // 버서커 상태가 아닐 시
        {
            Debug.Log("버서커");
            changedForm.Death();
            StartCoroutine(Test());
        }
        else
        {
            Debug.Log("진짜 죽음");
            StartCoroutine(Test2());
            //PageManger.Instance.RoadRetry();
        }
    }
    
    IEnumerator Test()
    {
        yield return new WaitForSeconds(3f);
        foreach(PlayerUnit form in forms) form.gameObject.SetActive(false);
        health = maxHealth; // 체력 초기화
        changedForm = forms[2]; // 상태를 버서커 상태로 변경
        changedForm.gameObject.SetActive(true);
        invalidation = false;
    }
    IEnumerator Test2()
    {
        yield return new WaitForSeconds(3f);
        // changedForm.gameObject.SetActive(false);
        PopupManager.Instance.DeathPop();

    }
    private Coroutine changing;
    public bool FormChange()
    {
        if(changedForm is Berserker || changing != null) return false; // 대쉬 중이거나 제어가 불가능한 상태일 경우 동작을 수행하지 않음
        bool b = false;
        if(changedForm is Human)
        {
            if(changeGage > 0 && changedForm.FormChange())
            {
                changing = StartCoroutine(ChangeWerewolf());
                b = true;
            }
            else if(changeGage <= 0 && bulletTimeCount > 0)
            {
                changedForm.UnitState = UnitState.Default;
                Dash();
                changing = StartCoroutine(BulletTime());
            }
        }
        else if(changedForm is Werwolf && changedForm.FormChange())
        {
            changing = StartCoroutine(ChangeHuman());
            b = true;
        }
        return b;
    }

    private IEnumerator BulletTime()
    {
        var tempDir = fixedDir;
        yield return new WaitUntil(() => changedForm.anim.GetCurrentAnimatorStateInfo(0).IsName("Dash"));
        yield return new WaitUntil(() => {
            FixMove();
            return !(changedForm is Human && changedForm.anim.GetCurrentAnimatorStateInfo(0).IsName("Dash") && changedForm.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.3f);
        });
        if(changedForm is Human && bulletTimeCount-- > 0)
        {
            Time.timeScale = 0.05f;
            Time.fixedDeltaTime = 0.05f * 0.02f;
            changedForm.anim.speed = 0;
            yield return new WaitForSecondsRealtime(bulletTime);
            changedForm.anim.speed = 1;
        }
        Time.timeScale = 1;
        Time.fixedDeltaTime = 1 * 0.02f;
        yield return new WaitUntil(() => {
            FixMove();
            return !(changedForm.anim.GetCurrentAnimatorStateInfo(0).IsName("Dash") && changedForm.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f);
        });

        changing = null;

        void FixMove()
        {
            changedForm.SetHorizontalForce(tempDir * changedForm.movementSpeed); // 자연스러운 대쉬 동작을 위한 부분
            changedForm.SetHorizontalVelocity(tempDir * Time.deltaTime * changedForm.movementSpeed);
        }
    }
    private IEnumerator ChangeWerewolf()
    {
        // changedForm.UnitState = UnitState.FormChange;
        // float t = 0;
        // while (t <= bulletTime&&changedForm is Human&&bulletTimeCount>=0)
        // while()
        // {
        //     t += Time.unscaledDeltaTime;
        //     Debug.Log("불릿 타임 시작");
        //     #region 불릿타임
        //     Time.timeScale = 0.05f;
        //     Time.fixedDeltaTime = 0.05f * 0.02f;
        //     #endregion  
        //     yield return null;
        // }
        // Debug.Log("불릿 타임 종료");
        // #region 불릿타임 해제
        // #endregion
        // t = 0;
        // var tempDir = fixedDir;
        // while(t <= changeDelay)
        // {
        //     t += Time.unscaledDeltaTime;
        //     changedForm.SetHorizontalForce(tempDir);
        //     yield return null;
        // }
        var tempDir = fixedDir;
        yield return new WaitUntil(() => changedForm.anim.GetCurrentAnimatorStateInfo(0).IsName("FormChange"));
        yield return new WaitUntil(() => {
            FixMove();
            return !(changedForm is Human && changedForm.anim.GetCurrentAnimatorStateInfo(0).IsName("FormChange") && changedForm.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.2f);
        });
        if(changedForm is Human && bulletTimeCount-- > 0)
        {
            Time.timeScale = 0.5f;
            Time.fixedDeltaTime = 0.5f * 0.02f;
            changedForm.anim.speed = 0;
            float t = 0;
            while((t += Time.unscaledDeltaTime) < bulletTime)
            {
                FixMove();
                yield return null;
            }
            changedForm.anim.speed = 1;
            Time.timeScale = 1;
            Time.fixedDeltaTime = 1 * 0.02f;
        }
        yield return new WaitUntil(() => {
            FixMove();
            return !(changedForm.anim.GetCurrentAnimatorStateInfo(0).IsName("FormChange") && changedForm.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f);
        });

        foreach(PlayerUnit form in forms) form.gameObject.SetActive(false);
        changedForm = forms[1]; // 인간 상태일 시 늑대인간으로 변경
        changedForm.gameObject.SetActive(true);
        FixMove();
        changedForm.UnitState = UnitState.Default;

        changing = null;

        void FixMove()
        {
            changedForm.SetHorizontalForce(tempDir * changedForm.movementSpeed); // 자연스러운 대쉬 동작을 위한 부분
            changedForm.SetHorizontalVelocity(tempDir * Time.deltaTime * changedForm.movementSpeed);
        }
    }
    private IEnumerator ChangeHuman()
    {
        var tempDir = fixedDir;
        yield return new WaitUntil(() => changedForm.anim.GetCurrentAnimatorStateInfo(0).IsName("FormChange"));
        yield return new WaitUntil(() => {
            FixMove();
            return !(changedForm.anim.GetCurrentAnimatorStateInfo(0).IsName("FormChange") && changedForm.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f);
        });
        changeGage -= brutalData.frm;
        foreach(PlayerUnit form in forms) form.gameObject.SetActive(false);
        changedForm = forms[0]; // 늑대인간 상태일 시 인간으로 변경
        changedForm.gameObject.SetActive(true);
        FixMove();
        changedForm.UnitState = UnitState.Default;

        changing = null;

        void FixMove()
        {
            changedForm.SetHorizontalForce(tempDir * changedForm.movementSpeed); // 자연스러운 대쉬 동작을 위한 부분
            changedForm.SetHorizontalVelocity(tempDir * Time.deltaTime * changedForm.movementSpeed);
        }
    }

    public bool Reload() => changedForm.Reload(); 

    void Awake()
    {
        pObject = this.gameObject; 
    }

    void Update()
    {
        pObject = this.gameObject;
        if(changedForm is Werwolf)
        {
            if(changeGage >= 0) changeGage -= brutalData.sec * Time.deltaTime;
            else FormChange();
        }
    }

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
}
