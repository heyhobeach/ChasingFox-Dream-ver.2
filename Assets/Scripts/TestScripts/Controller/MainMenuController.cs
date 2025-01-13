using System;
using UnityEngine;

public class MainMenuController : MonoBehaviour, IBaseController
{
    private Action _onDown;
    public Action onDown { get => _onDown; set => throw new NotImplementedException(); }
    private Action _onUp;
    public Action onUp { get => _onUp; set => throw new NotImplementedException(); }
    
    public void Controller()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && MainMenuManager.Instance.isMoving == false)
        {
            MainMenuManager.Instance.MoveBackNode();
        }
    }

    void Start()
    {
        ((IBaseController)this).AddController();
    }

    void OnDestroy()
    {
        ((IBaseController)this).RemoveController();
    }
}
