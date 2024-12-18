public interface IBaseController
{
    public void AddController() => ControllerManager.PushController(this);
    public void RemoveController() => ControllerManager.PopController(this);

    public void Controller();
}
