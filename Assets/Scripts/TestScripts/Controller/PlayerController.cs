using System;
using System.Collections;
using System.Collections.Generic;
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
    public PlayerControllerMask pcm;

    void Awake() => ((IBaseController)this).AddController();
    void Start() => unitController = GetComponent<IUnitController>();
    void OnDestroy() => ((IBaseController)this).RemoveController();

    public void Controller()
    {
        bool isKeyDown = false;
        if(Input.GetKeyDown(KeyCode.Escape)) GameManager.Instance.Pause();

        if(Input.GetKeyDown(KeyCode.Mouse1) && KeyControll(PlayerControllerMask.FormChange, ref isKeyDown)) unitController.FormChange();
        if(Input.GetKeyDown(KeyCode.R) && KeyControll(PlayerControllerMask.Reload, ref isKeyDown)) unitController.Reload();
        if(Input.GetKeyDown(KeyCode.Mouse0) && KeyControll(PlayerControllerMask.Attack, ref isKeyDown)) unitController.Attack(ClickPos());
        if(Input.GetKeyDown(KeyCode.Space) && KeyControll(PlayerControllerMask.Dash, ref isKeyDown)) unitController.Dash();
        
        if (Input.GetKey(KeyCode.S) && KeyControll(PlayerControllerMask.Crouch, ref isKeyDown)) unitController.Crouch(KeyState.KeyDown);//GetKeyDown -> GetKey
        else if(Input.GetKeyUp(KeyCode.S) && KeyControll(PlayerControllerMask.Crouch)) unitController.Crouch(KeyState.KeyUp);
        if(Input.GetKeyDown(KeyCode.W) && KeyControll(PlayerControllerMask.Jump, ref isKeyDown)) unitController.Jump(KeyState.KeyDown);
        else if(Input.GetKey(KeyCode.W) && KeyControll(PlayerControllerMask.Jump)) unitController.Jump(KeyState.KeyStay);
        else if(Input.GetKeyUp(KeyCode.W) && KeyControll(PlayerControllerMask.Jump)) unitController.Jump(KeyState.KeyUp);
        else unitController.Jump(KeyState.None);
        if (Input.GetAxisRaw("Horizontal") != 0 && KeyControll(PlayerControllerMask.Move)) unitController.Move(Vector2.right * Input.GetAxisRaw("Horizontal"));
        else unitController.Move(Vector2.zero);
    }

    private bool KeyControll(PlayerControllerMask mask, ref bool isKeyDown)
    {
        if(isKeyDown || ((pcm & mask) != mask)) return false;
        isKeyDown = true;
        return true;
    }
    private bool KeyControll(PlayerControllerMask mask)
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
