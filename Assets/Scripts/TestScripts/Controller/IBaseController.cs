using System;

public interface IBaseController
{
    public void AddController() => ControllerManager.PushController(this);
    public void RemoveController() => ControllerManager.PopController(this);

    public Action onDown { get; set; }
    public Action onUp { get; set; }

    public void Controller();
}
