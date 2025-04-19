using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    private Stack<IBaseController> controllers = new();
    public static void PushController(IBaseController @base)
    {
        var controllers = ServiceLocator.Get<ControllerManager>().controllers;
        if(controllers == null)
        {
            Debug.LogError("controllers not set");
            return;
        }
        if(controllers.Contains(@base)) return;
        if(controllers.Count > 0) controllers.Peek().onDown?.Invoke();
        ServiceLocator.Get<ControllerManager>().controllers.Push(@base);
    }
    public static void PopController(IBaseController @base)
    {
        var controllers = ServiceLocator.Get<ControllerManager>().controllers;
        if(controllers == null)
        {
            Debug.LogError("controllers not set");
            return;
        }
        if(controllers.Count > 0 && controllers.Peek() != @base)
        {
            Stack<IBaseController> temp = new();
            while(controllers.Count > 0 && !temp.Equals(controllers.Peek())) temp.Push(ServiceLocator.Get<ControllerManager>().controllers.Pop());
            while(temp.Count > 0) controllers.Push(temp.Pop());
        }
        if(controllers.Count > 0) 
        {
            controllers.Pop();
        }
        if(controllers.Count > 0)controllers.Peek().onUp?.Invoke();
    }

    private void OnDestroy()
    {
        ServiceLocator.Unregister(this);
    }

    private void Awake()
    {
        ServiceLocator.Register(this);

    }
    private void Start()
    {
        if(controllers == null) controllers = new();
    }

    private void Update()
    {
        if(controllers.Count > 0) controllers.Peek().Controller();
    }
}
