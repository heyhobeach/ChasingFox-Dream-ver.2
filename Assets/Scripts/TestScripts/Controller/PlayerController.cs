using System;
using UnityEngine;

public class PlayerController : MonoBehaviour, IBaseController
{
    [Flags]
    public enum PlayerControllerMask
    {
        None = 0,
        FormChange = 1<<1,
        Skill1 = 1<<2,
        Skill2 = 1<<3,
        Reload = 1<<4,
        Attack = 1<<5,
        Dash = 1<<6,
        Crouch = 1<<7,
        Jump = 1<<8,
        Move = 1<<9
    }

    private GameManager gameManager;
    private IUnitController unitController;
    [SerializeField] private PlayerControllerMask pcm;

    public GameObject playerUI;

    private Action _onDown;
    public Action onDown { get => _onDown; set => throw new NotImplementedException(); }
    private Action _onUp;
    public Action onUp { get => _onUp; set => throw new NotImplementedException(); }

    // void Awake() => ((IBaseController)this).AddController();
    void Start()
    {
        unitController = GetComponent<IUnitController>();
        _onDown = () => playerUI.SetActive(false);
        _onUp = () => playerUI.SetActive(true);
    }
    void OnDestroy() => ((IBaseController)this).RemoveController();

    public void Init(PlayerData.JsonData playerData = null)
    {
        ((IBaseController)this).AddController();
        if(playerData != null) pcm = playerData.pcm;
        else pcm = (PlayerControllerMask)~0;
        gameManager = ServiceLocator.Get<GameManager>();

    }
    public PlayerControllerMask DataSet() => pcm;

    public void Controller()
    {
        if(gameManager == null) return;
        bool isKeyDown = false;

        // System Control
        if(Input.GetButtonDown("Cancel")) gameManager.Pause();
        // if(Input.GetButtonDown("Reload") || Input.GetButton("Reload"))
        // {
        //     resetTimer += Time.unscaledDeltaTime;
        //     if(resetTimer > 2f) ServiceLocator.Get<GameManager>().RetryScene();
        // }
        // else resetTimer = 0;

        if(gameManager.isPaused) return;

        // Player Control
        if(SystemManager.GetButtonDown("formChange") && KeyControl(PlayerControllerMask.FormChange, ref isKeyDown)) unitController.FormChange();

        if(SystemManager.GetButtonDown("skill1") && KeyControl(PlayerControllerMask.Skill1, ref isKeyDown)) unitController.Skill1(ClickPos());
        if(SystemManager.GetButtonDown("skill2")) unitController.Skill2(KeyState.KeyDown);
        else if(SystemManager.GetButtonUp("skill2")) unitController.Skill2(KeyState.KeyUp);

        if(SystemManager.GetButtonDown("reload") && KeyControl(PlayerControllerMask.Reload, ref isKeyDown)) unitController.Reload(KeyState.KeyDown);
        else if(SystemManager.GetButton("reload") && KeyControl(PlayerControllerMask.Reload, ref isKeyDown)) unitController.Reload(KeyState.KeyStay);
        else if(SystemManager.GetButtonUp("reload") && KeyControl(PlayerControllerMask.Reload, ref isKeyDown)) unitController.Reload(KeyState.KeyUp);

        if(SystemManager.GetButtonDown("attack") && KeyControl(PlayerControllerMask.Attack, ref isKeyDown)) unitController.Attack(ClickPos());

        if(SystemManager.GetButtonDown("dash") && KeyControl(PlayerControllerMask.Dash, ref isKeyDown)) unitController.Dash();
        
        if (SystemManager.GetButton("crouch") && KeyControl(PlayerControllerMask.Crouch, ref isKeyDown)) unitController.Crouch(KeyState.KeyDown);//GetKeyDown -> GetKey
        else if(SystemManager.GetButtonUp("crouch") && KeyControl(PlayerControllerMask.Crouch)) unitController.Crouch(KeyState.KeyUp);

        if(SystemManager.GetButtonDown("jump") && KeyControl(PlayerControllerMask.Jump, ref isKeyDown)) unitController.Jump(KeyState.KeyDown);
        else if(SystemManager.GetButton("jump") && KeyControl(PlayerControllerMask.Jump)) unitController.Jump(KeyState.KeyStay);
        else if(SystemManager.GetButtonUp("jump") && KeyControl(PlayerControllerMask.Jump)) unitController.Jump(KeyState.KeyUp);
        else unitController.Jump(KeyState.None);
        
        if (SystemManager.GetAxis("Horizontal") != 0 && KeyControl(PlayerControllerMask.Move)) unitController.Move(Vector2.right * SystemManager.GetAxis("Horizontal"));
        else unitController.Move(Vector2.zero);
    }

    private bool KeyControl(PlayerControllerMask mask, ref bool isKeyDown)
    {
        if(isKeyDown || ((pcm & mask) != mask)) return false;
        isKeyDown = true;
        return true;
    }
    private bool KeyControl(PlayerControllerMask mask)
    {
        if((pcm & mask) == mask) return true;
        return false;
    }

    [VisibleEnum(typeof(PlayerControllerMask))] public void AddControl(int mask) => AddControl((PlayerControllerMask)mask);
    [VisibleEnum(typeof(PlayerControllerMask))] public void DelControl(int mask) => DelControl((PlayerControllerMask)mask);
    public void AddControl(PlayerControllerMask mask) => pcm |= mask;
    public void DelControl(PlayerControllerMask mask) => pcm = (pcm & mask) == mask ? pcm ^ mask : pcm;

    public void Move(float dir) => unitController.Move(Vector2.right * dir);
    public void Attack() => unitController.Attack(ClickPos());
    public void Reload(KeyState keyState) => unitController.Reload(keyState);
    public void Dash() => unitController.Dash();
    public void FormChange() => unitController.FormChange();

    public Vector3 ClickPos()//클릭한 좌료를 보내주며 현재 공격 클릭시 캐릭터의 바라보는 방향도 변해야한다고 생각해서 필요했던 부분
    {
        var screenPoint = Input.mousePosition;//마우스 위치 가져옴
        screenPoint.z = Camera.main.transform.position.z;
        Vector3 pos = Camera.main.ScreenToWorldPoint(screenPoint);
        pos.z = 0;
        return pos;
    }
}
