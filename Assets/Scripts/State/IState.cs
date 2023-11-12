public interface IState 
{
    bool IsStateDone { get; }
    IState NextState { get; }
    public void OnEnterState() { }
    public void OnUpdateState() { }
    public void OnExitState() { }
}
