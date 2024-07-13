using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    // void OnEnable() => GameManager.PushController(this);
    // void OnDisable() => GameManager.PopController(this);
    public void AddController() => GameManager.PushController(this);
    public void RemoveController() => GameManager.PopController(this);

    public abstract void Controller();
}
