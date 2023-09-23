public interface IState 
{
    bool IsStateDone { get; }
    IState NextState { get; set; }
    void OnEnterState() { }
    void OnExitState() { }
    void OnUpdateState() { }
}
