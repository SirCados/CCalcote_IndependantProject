public interface IState 
{
    bool IsStateDone { get; }
    IState NextState { get; }
    void OnEnterState() { }
    void OnExitState() { }
    void OnUpdateState() { }
}
