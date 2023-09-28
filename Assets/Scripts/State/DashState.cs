public class DashState : IState
{
    bool _isStateDone;
    IState _nextState;
    public PlayerController TempBody;

    public void OnEnterState()
    {

    }

    public void OnUpdateState()
    {

    }

    public void OnExitState()
    {

    }

    public bool IsStateDone
    {
        get => _isStateDone;
    }

    public IState NextState
    {
        get => _nextState;
        set => _nextState = value;
    }
}
