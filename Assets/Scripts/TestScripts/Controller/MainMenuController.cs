using UnityEngine;

public class MainMenuController : MonoBehaviour, IBaseController
{
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
