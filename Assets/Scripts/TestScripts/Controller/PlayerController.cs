using System;
using UnityEngine;

public class PlayerController : MonoBehaviour, IBaseController
{
    [Flags]
    public enum PlayerControllerMask
    {
        None = 0,
        FormChange = 1<<1,
        Reload = 1<<2,
        Attack = 1<<3,
        Dash = 1<<4,
        Crouch = 1<<5,
        Jump = 1<<6,
        Move = 1<<7
    }

    private IUnitController unitController;
    [SerializeField] private PlayerControllerMask pcm;

    public GameObject playerUI;

    private float resetTimer;

    private Action _onDown;
    public Action onDown { get => _onDown; set => throw new NotImplementedException(); }
    private Action _onUp;
    public Action onUp { get => _onUp; set => throw new NotImplementedException(); }

    void Awake() => ((IBaseController)this).AddController();
    void Start()
    {
        unitController = GetComponent<IUnitController>();
        _onDown = () => playerUI.SetActive(false);
        _onUp = () => playerUI.SetActive(true);
    }
    void OnEnable() => resetTimer = 0;
    void OnDestroy() => ((IBaseController)this).RemoveController();

    public void Init(PlayerData playerData) => pcm = playerData.pcm;
    public PlayerControllerMask DataSet() => pcm;

    public void Controller()
    {
        bool isKeyDown = false;

        // System Control
        if(Input.GetButtonDown("Cancel")) GameManager.Instance.Pause();
        if(Input.GetButtonDown("Reload") || Input.GetButton("Reload"))
        {
            resetTimer += Time.unscaledDeltaTime;
            if(resetTimer > 2f) GameManager.Instance.RetryScene();
        }
        else resetTimer = 0;

        if(GameManager.Instance.isPaused) return;

        // Player Control
        if(Input.GetButtonDown("Fire2") && KeyControl(PlayerControllerMask.FormChange, ref isKeyDown)) unitController.FormChange();
        if(Input.GetButtonDown("Reload") && KeyControl(PlayerControllerMask.Reload, ref isKeyDown)) unitController.Reload();
        if(Input.GetButtonDown("Fire1") && KeyControl(PlayerControllerMask.Attack, ref isKeyDown)) unitController.Attack(ClickPos());
        if(Input.GetButtonDown("Dash") && KeyControl(PlayerControllerMask.Dash, ref isKeyDown)) unitController.Dash();
        
        if (Input.GetButton("Crouch") && KeyControl(PlayerControllerMask.Crouch, ref isKeyDown)) unitController.Crouch(KeyState.KeyDown);//GetKeyDown -> GetKey
        else if(Input.GetButtonUp("Crouch") && KeyControl(PlayerControllerMask.Crouch)) unitController.Crouch(KeyState.KeyUp);
        if(Input.GetButtonDown("Jump") && KeyControl(PlayerControllerMask.Jump, ref isKeyDown)) unitController.Jump(KeyState.KeyDown);
        else if(Input.GetButton("Jump") && KeyControl(PlayerControllerMask.Jump)) unitController.Jump(KeyState.KeyStay);
        else if(Input.GetButtonUp("Jump") && KeyControl(PlayerControllerMask.Jump)) unitController.Jump(KeyState.KeyUp);
        else unitController.Jump(KeyState.None);
        if (Input.GetAxisRaw("Horizontal") != 0 && KeyControl(PlayerControllerMask.Move)) unitController.Move(Vector2.right * Input.GetAxisRaw("Horizontal"));
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

    public void AddControl(int mask) => AddControl((PlayerControllerMask) (1<<mask));
    public void DelControl(int mask) => DelControl((PlayerControllerMask) (1<<mask));
    public void AddControl(PlayerControllerMask mask) => pcm |= mask;
    public void DelControl(PlayerControllerMask mask) => pcm = (pcm & mask) == mask ? pcm ^ mask : pcm;

    public void Move(float dir) => unitController.Move(Vector2.right * dir);
    public void Attack(float angle) => unitController.Attack(new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle) * 10000, Mathf.Cos(Mathf.Deg2Rad * angle) * 10000));
    public void Reload() => unitController.Reload();
    public void Dash() => unitController.Dash();

    public Vector3 ClickPos()//클릭한 좌료를 보내주며 현재 공격 클릭시 캐릭터의 바라보는 방향도 변해야한다고 생각해서 필요했던 부분
    {
        var screenPoint = Input.mousePosition;//마우스 위치 가져옴
        screenPoint.z = Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(screenPoint);
    }
}
