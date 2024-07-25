public interface IBaseController
{
    public void AddController() => GameManager.PushController(this);
    public void RemoveController() => GameManager.PopController(this);

    public void Controller();
}
