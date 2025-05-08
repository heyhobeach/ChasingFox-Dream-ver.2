using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    private Stack<IBaseController> controllers;
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
        controllers.Push(@base);
    }
    public static void PopController(IBaseController @base)
    {
        var controllers = ServiceLocator.Get<ControllerManager>().controllers;
        if(controllers == null)
        {
            Debug.LogError("controllers not set");
            return;
        }
        if(controllers.Count > 0 && controllers.Contains(@base))
        {
            if(controllers.Peek().Equals(@base))
            {
                controllers.Pop();
                if(controllers.Count > 0) controllers.Peek().onUp?.Invoke();
            }
            else
            {
                Stack<IBaseController> temp = new();
                while(controllers.Count > 0 && !temp.Equals(controllers.Peek())) temp.Push(controllers.Pop());
                if(controllers.Peek().Equals(@base))
                {
                    controllers.Pop();
                    if(controllers.Count > 0) controllers.Peek().onUp?.Invoke();                   
                }
                while(temp.Count > 0) controllers.Push(temp.Pop());
            }
        }
    }

    private void OnDestroy() => ServiceLocator.Unregister(this);

    private void Awake()
    {
        ServiceLocator.Register(this);
        controllers = new();
    }

    private void Update()
    {
        IBaseController controller = null;
        if(controllers.TryPeek(out controller)) controller.Controller();
    }
}
