using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    private static ControllerManager instance;
    public static ControllerManager Instance { get => instance; }

    private Stack<IBaseController> controllers = new();
    public static void PushController(IBaseController @base)
    {
        if(instance.controllers.Contains(@base)) return;
        if(instance.controllers.Count > 0) instance.controllers.Peek().onDown?.Invoke();
        instance.controllers.Push(@base);
    }
    public static void PopController(IBaseController @base)
    {
        if(instance.controllers.Count > 0 && instance.controllers.Peek() != @base)
        {
            Stack<IBaseController> temp = new();
            while(instance.controllers.Count > 0 && !temp.Equals(instance.controllers.Peek())) temp.Push(instance.controllers.Pop());
            while(temp.Count > 0) instance.controllers.Push(temp.Pop());
        }
        if(instance.controllers.Count > 0) instance.controllers.Pop();
        if(instance.controllers.Count > 0) instance.controllers.Peek().onUp?.Invoke();
    }
    public static IBaseController GetTopController() => instance.controllers.Peek();

    private void OnDestroy()
    {
        instance = null;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        if(controllers == null) controllers = new();
    }

    private void Update()
    {
        if(controllers.Count > 0) controllers.Peek().Controller();
    }
}
