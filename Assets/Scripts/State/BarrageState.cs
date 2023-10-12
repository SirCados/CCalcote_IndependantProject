public class BarrageState : IState
{
    bool _isStateDone;
    IState _nextState;
    BarrageAspect _barrageAspect;

    public BarrageState(BarrageAspect manifestedBarrage)
    {
        _barrageAspect = manifestedBarrage;
    }

    public void OnEnterState()
    {
        _isStateDone = false;
        HandleBarrageInput();
    }

    public void OnUpdateState()
    {
        if (!_barrageAspect.IsBarraging)
        {
            _isStateDone = true;
        }     
    }

    public void OnExitState()
    {
        _isStateDone = false;
    }

    public void HandleBarrageInput()
    {
        _barrageAspect.PerformBarrage();
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
