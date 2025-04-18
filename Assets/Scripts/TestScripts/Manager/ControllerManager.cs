using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    private Stack<IBaseController> controllers = new();
    public static void PushController(IBaseController @base)
    {
        if(ServiceLocator.Get<ControllerManager>().controllers.Contains(@base)) return;
        if(ServiceLocator.Get<ControllerManager>().controllers.Count > 0) ServiceLocator.Get<ControllerManager>().controllers.Peek().onDown?.Invoke();
        ServiceLocator.Get<ControllerManager>().controllers.Push(@base);
    }
    public static void PopController(IBaseController @base)
    {
        if(ServiceLocator.Get<ControllerManager>().controllers.Count > 0 && ServiceLocator.Get<ControllerManager>().controllers.Peek() != @base)
        {
            Stack<IBaseController> temp = new();
            while(ServiceLocator.Get<ControllerManager>().controllers.Count > 0 && !temp.Equals(ServiceLocator.Get<ControllerManager>().controllers.Peek())) temp.Push(ServiceLocator.Get<ControllerManager>().controllers.Pop());
            while(temp.Count > 0) ServiceLocator.Get<ControllerManager>().controllers.Push(temp.Pop());
        }
        if(ServiceLocator.Get<ControllerManager>().controllers.Count > 0) 
        {
            ServiceLocator.Get<ControllerManager>().controllers.Pop();
        }
        if(ServiceLocator.Get<ControllerManager>().controllers.Count > 0) ServiceLocator.Get<ControllerManager>().controllers.Peek().onUp?.Invoke();
    }
    public static IBaseController GetTopController() => ServiceLocator.Get<ControllerManager>().controllers.Peek();

    private void OnDestroy()
    {
        ServiceLocator.Unregister(this);
    }

    private void Awake()
    {
        ServiceLocator.Register(this);

        if(controllers == null) controllers = new();
    }

    private void Update()
    {
        if(controllers.Count > 0) controllers.Peek().Controller();
    }
}
