
public class NeutralState : IState
{
    bool IState.IsStateDone => throw new System.NotImplementedException();

    public IState NextState { get => null; set => throw new System.NotImplementedException(); }

    public void OnEnterState()
    {
        
    }

    public void OnUpdateState()
    {
        
    }

    public void OnExitState()
    {
        
    }
}
