

public class AirborneState : IState
{
    bool _isStateDone;
    IState _nextState;
    public AvatarAspect AvatarBody;

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
