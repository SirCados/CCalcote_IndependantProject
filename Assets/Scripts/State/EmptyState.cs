public class EmptyState : IState
{
    bool _isStateDone;
    IState _nextState;

    public bool IsStateDone
    {
        get => _isStateDone;
    }
    public IState NextState
    {
        get => _nextState;
    }
}
