public class DownState : IState
{
    AvatarAspect _avatarAspect;
    BlastAspect _blastAspect;
    bool _isStateDone;
    IState _nextState;

    public DownState(IState nextState, AvatarAspect avatarAspect)
    {
        _nextState = nextState;
        _avatarAspect = avatarAspect;
    }

    public void OnEnterState()
    {

    }

    public void OnUpdateState()
    {
        if (!_avatarAspect.IsKnockedDown)
        {
            _isStateDone = true;
        }
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
