public class GetUpState : IState
{
    bool _isStateDone;
    IState _nextState;
    AvatarAspect _avatarAspect;

    public GetUpState(ActiveState activeState, AvatarAspect avatar)
    {
        _nextState = activeState;
        _avatarAspect = avatar;
    }
    public void OnEnterState()
    {
        _isStateDone = false;
    }

    public void OnUpdateState()
    {
        if (!_avatarAspect.IsGettingUp)
        {
            _isStateDone = true;
        }
    }

    public void OnExitState()
    {
        _isStateDone = false;
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
