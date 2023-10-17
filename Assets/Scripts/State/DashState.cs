public class DashState : IState
{
    bool _isStateDone;
    IState _nextState;
    AvatarAspect _avatarBody;

    public DashState(IState nextState, AvatarAspect avatar)
    {
        _nextState = nextState;
        _avatarBody = avatar;

    }
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
    }
}
